using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.Assemblers;
using DamnScript.Runtimes.VirtualMachines.OpCodes;
using DamnScript.Runtimes.VirtualMachines.Scripts;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScript.Parsings.Antlrs
{
    public static unsafe class ScriptParser
    {
        public static void ParseScript(Stream input, String32 name, ScriptData* scriptData)
        {
            var charStream = new AntlrInputStream(input);
            var lexer = new DamnScriptLexer(charStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new DamnScriptParser(tokenStream);
            
            var entry = parser.entry();

            scriptData->name = name;

            var regions = new NativeList<RegionData>(2);
            var strings = new NativeList<NativeStringPtr>(16);
            var methods = new NativeList<NativeStringPtr>(16);
            var context = new ScriptParserContext();
            for (var i = 0; i < entry.ChildCount; i++)
            {
                var assembler = new ScriptAssembler(0);
                context.strings = &strings;
                context.methods = &methods;
                context.assembler = &assembler;

                var regionContext = entry.GetChild(i) as DamnScriptParser.RegionGroupContext;
                ParseRegion(regionContext, &context);

                var byteCode = context.assembler->FinishAlloc();
                context.assembler->Dispose();
                
                var region = new RegionData(context.name, byteCode);
                regions.Add(region);
            }

            if (context.isError)
            {
                scriptData->Dispose();
                *scriptData = default;
                regions.Dispose();
                strings.Dispose();
                
                throw new Exception($"Error while parsing script {scriptData->name}!");
            }

            scriptData->regions = regions.ToArrayAlloc();
            regions.Dispose();

            scriptData->metadata = new ScriptMetadata(new ConstantsData(strings.ToArrayAlloc(), methods.ToArrayAlloc()));
            strings.Dispose();
            methods.Dispose();
        }

        public static void ParseRegion(DamnScriptParser.RegionGroupContext region, ScriptParserContext* context)
        {
            var regionName = region.name().GetText();
            var regionBlock = region.blockGroup();
            context->name = new String32(regionName);
            ParseBlock(regionBlock, context);
        }
        
        public static void ParseBlock(DamnScriptParser.BlockGroupContext block, ScriptParserContext* context)
        {
            var statements = block.statementUnion();
            for (var i = 0; i < statements.Length; i++)
            {
                var statement = statements[i].GetChild(0);
                switch (statement)
                {
                    case DamnScriptParser.CallStatementGroupContext callStatement:
                        ParseCallStatement(callStatement, context);
                        context->assembler->SetSavePoint();
                        break;
                    case DamnScriptParser.IfStatementGroupContext ifStatement:
                        ParseIfStatement(ifStatement, context);
                        break;
                    case DamnScriptParser.ForStatementGroupContext forStatement:
                        ParseForStatement(forStatement, context);
                        break;
                    case DamnScriptParser.WhileStatementGroupContext whileStatement:
                        ParseWhileStatement(whileStatement, context);
                        break;
                }
            }
        }
        
        public static void ParseCallStatement(DamnScriptParser.CallStatementGroupContext callStatement, ScriptParserContext* context)
        {
            ParseAnyCall(callStatement.anyCall(), context);
        }
        
        public static void ParseIfStatement(DamnScriptParser.IfStatementGroupContext ifStatement, ScriptParserContext* context)
        {
            var branches = stackalloc int[32];
            var branchCount = 0;

            var conditions = ifStatement.condition();
            var blocks = ifStatement.blockGroup();
            
            var hasElse = blocks.Length > conditions.Length;
            
            for (var i = 0; i < conditions.Length; i++)
            {
                var conditionExpression = conditions[i].expression();
                ParseExpression(conditionExpression, context);

                context->assembler->PushToStack(1);
                var conditionJumpOffset = context->assembler->offset;
                context->assembler->JumpNotEquals(-1);

                var block = blocks[i];
                ParseBlock(block, context);

                branches[branchCount++] = context->assembler->offset;
                context->assembler->Jump(-1);
                
                var prevOffset = context->assembler->offset;
                context->assembler->offset = conditionJumpOffset;
                context->assembler->JumpNotEquals(prevOffset);
                context->assembler->offset = prevOffset;
            }
            
            if (hasElse)
            {
                var elseBlock = blocks[^1];
                ParseBlock(elseBlock, context);
            }
            
            var endOffset = context->assembler->offset;
            for (var i = 0; i < branchCount; i++)
            {
                var branch = branches[i];
                var prevOffset = context->assembler->offset;
                context->assembler->offset = branch;
                context->assembler->Jump(endOffset);
                context->assembler->offset = prevOffset;
            }
        }
        
        public static void ParseForStatement(DamnScriptParser.ForStatementGroupContext forStatement, ScriptParserContext* context)
        {
            var variable = forStatement.var();
            var variableName = new String32(variable.GetText());
            
            var register = context->ReserveIdentifier(variableName);
            
            context->assembler->PushToStack(0);
            context->assembler->StoreToRegister(register);
            
            var beginOffset = context->assembler->offset;

            var block = forStatement.blockGroup();
            ParseBlock(block, context);
            
            context->assembler->LoadFromRegister(register);
            context->assembler->PushToStack(1);
            context->assembler->ExpressionCall(ExpressionCall.ExpressionCallType.Add);
            context->assembler->DuplicateStack();
            context->assembler->StoreToRegister(register);
            var countExpression = forStatement.expression();
            ParseExpression(countExpression, context);
            context->assembler->JumpNotEquals(beginOffset);
            
            context->FreeRegister(register);
        }
        
        public static void ParseWhileStatement(DamnScriptParser.WhileStatementGroupContext whileStatement, ScriptParserContext* context)
        {
            var beginOffset = context->assembler->offset;
            var conditionExpression = whileStatement.condition().expression();
            ParseExpression(conditionExpression, context);
            context->assembler->PushToStack(1);
            var jumpConditionOffset = context->assembler->offset;
            context->assembler->JumpNotEquals(-1);
            
            var block = whileStatement.blockGroup();
            ParseBlock(block, context);
            context->assembler->Jump(beginOffset);
            var prevOffset = context->assembler->offset;
            context->assembler->offset = jumpConditionOffset;
            context->assembler->JumpNotEquals(prevOffset);
            context->assembler->offset = prevOffset;
        }

        public static void ParseExpression(DamnScriptParser.ExpressionContext expression, ScriptParserContext* context)
        {
            var additiveExpression = expression.additiveExpression();
            var logicalOp = expression.logicalOp();
            for (var i = 0; i < additiveExpression.Length; i++)
            {
                var child = additiveExpression[i];
                ParseAddictiveExpressionNodes(child, context);
                
                var logicalIndex = i - 1;
                if (logicalIndex >= 0 && logicalIndex % 2 == 0)
                    AssemblyLogicalOp(logicalOp[logicalIndex], context);
            }
        }
        
        public static void AssemblyLogicalOp(DamnScriptParser.LogicalOpContext logicalOp, ScriptParserContext* context)
        {
            var type = logicalOp.Stop.Type;
            var operation = type switch
            {
                DamnScriptParser.EQUAL => ExpressionCall.ExpressionCallType.Equal,
                DamnScriptParser.NOT_EQUAL => ExpressionCall.ExpressionCallType.NotEqual,
                DamnScriptParser.GREATER => ExpressionCall.ExpressionCallType.Greater,
                DamnScriptParser.GREATER_EQUAL => ExpressionCall.ExpressionCallType.GreaterOrEqual,
                DamnScriptParser.LESS => ExpressionCall.ExpressionCallType.Less,
                DamnScriptParser.LESS_EQUAL => ExpressionCall.ExpressionCallType.LessOrEqual,
                DamnScriptParser.AND => ExpressionCall.ExpressionCallType.And,
                DamnScriptParser.OR => ExpressionCall.ExpressionCallType.Or,
                _ => ExpressionCall.ExpressionCallType.Invalid
            };
            
            context->assembler->ExpressionCall(operation);
        }
        
        public static void AssemblyKeyword(DamnScriptParser.KeywordsContext keywords, ScriptParserContext* context)
        {
            var type = keywords.Stop.Type;
            switch (type)
            {
                case DamnScriptParser.NULL:
                    context->assembler->PushNullToStack();
                    return;
                case DamnScriptParser.TRUE:
                    context->assembler->PushToStack(1);
                    return;
                case DamnScriptParser.FALSE:
                    context->assembler->PushToStack(0);
                    return;
            }
        }
        
        public static void ParseAddictiveExpressionNodes(IParseTree tree, ScriptParserContext* context)
        {
            for (var i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                switch (child)
                {
                    case DamnScriptParser.TermContext term:
                        ParseTerm(term, context);
                        break;
                    
                    case DamnScriptParser.AddOpContext addOp:
                        var nextTerm = tree.GetChild(++i) as DamnScriptParser.TermContext;
                        ParseTerm(nextTerm, context);
                        ParseAddOp(addOp, context);
                        break;
                }
            }
        }
        
        public static void ParseAddOp(DamnScriptParser.AddOpContext addOp, ScriptParserContext* context)
        {
            var type = addOp.Stop.Type;
            var operation = type switch
            {
                DamnScriptParser.ADD => ExpressionCall.ExpressionCallType.Add,
                DamnScriptParser.SUBTRACT => ExpressionCall.ExpressionCallType.Subtract,
                _ => ExpressionCall.ExpressionCallType.Invalid
            };
            
            context->assembler->ExpressionCall(operation);
        }
        
        public static void ParseTerm(DamnScriptParser.TermContext term, ScriptParserContext* context)
        {
            var factor = term.factorUnion();
            var mulOp = term.mulOp();
            for (var i = 0; i < factor.Length; i++)
            {
                var child = factor[i].GetChild(0);
                switch (child)
                {
                    case DamnScriptParser.NumContext number:
                        ParseNumber(number, context);
                        break;
                    case DamnScriptParser.KeywordsContext keywords:
                        AssemblyKeyword(keywords, context);
                        break;
                    case DamnScriptParser.ParensContext parensExpression:
                        ParseParens(parensExpression, context);
                        break;
                    case DamnScriptParser.AnyCallContext anyMethodCall:
                        ParseAnyCall(anyMethodCall, context);
                        break;
                    case DamnScriptParser.StrContext stringContext:
                        ParseString(stringContext, context);
                        break;
                    case DamnScriptParser.VarContext variable:
                        ParseVariable(variable, context);
                        break;
                }
                
                var mulIndex = i - 1;
                if (mulIndex >= 0 && mulIndex % 2 == 0)
                    ParseMulOp(mulOp[mulIndex], context);
            }
        }
        
        public static void ParseParens(DamnScriptParser.ParensContext parensExpression, ScriptParserContext* context)
        {
            var expression = parensExpression.expression();
            ParseExpression(expression, context);
        }

        public static void ParseAnyCall(DamnScriptParser.AnyCallContext methodCall, ScriptParserContext* context)
        {
            var funcCall = methodCall.funcCall();
            if (funcCall != null)
            {
                ParseFuncCall(funcCall, context);
                return;
            }
            
            var objCall = methodCall.objectCall();
            if (objCall != null)
                ParseObjectCall(objCall, context);
        }

        public static void ParseObjectCall(DamnScriptParser.ObjectCallContext objectCall, ScriptParserContext* context)
        {
            ParseFuncCall(objectCall.funcCall(), context);
            context->isObjectCall = true;
            ParseAnyCall(objectCall.anyCall(), context);
            context->isObjectCall = false;
        }
        
        public static void ParseFuncCall(DamnScriptParser.FuncCallContext funcCall, ScriptParserContext* context)
        {
            var functionName = funcCall.name().GetText();
            var arguments = funcCall.arguments();
            if (arguments != null)
            {
                for (var i = 0; i < arguments.ChildCount; i++)
                {
                    var argument = arguments.GetChild<DamnScriptParser.ArgumentContext>(i);
                    if (argument == null)
                        continue;
                    var expression = argument.expression();
                    ParseExpression(expression, context);
                }
            }

            var index = AddStringToConstantsIfNotExists(context->methods, functionName);
            var argumentCount = arguments?.ChildCount / 2 + 1 ?? 0;
            if (context->isObjectCall)
                argumentCount++;
            context->assembler->NativeCall(index, argumentCount);
        }
        
        public static void ParseString(DamnScriptParser.StrContext stringContext, ScriptParserContext* context)
        {
            var text = stringContext.GetText();
            
            var index = AddStringToConstantsIfNotExists(context->strings, text);
            context->assembler->PushStringToStack(index);
        }
        
        public static void ParseVariable(DamnScriptParser.VarContext variable, ScriptParserContext* context)
        {
            var variableName = variable.GetText();
            var str32 = new String32(variableName);
            var registerIndex = context->GetRegisterIndex(str32);
            if (registerIndex == -1)
            {
                Debugging.LogError($"[{nameof(ScriptParser)}] ({nameof(ParseVariable)}) PARSING:" +
                                   $"Variable {variableName} not found!");
                context->isError = true;
                return;
            }
            
            context->assembler->LoadFromRegister(registerIndex);
        }
        
        public static void ParseNumber(DamnScriptParser.NumContext numberContext, ScriptParserContext* context)
        {
            var number = numberContext.GetText();
            var value = long.Parse(number);
            context->assembler->PushToStack(new ScriptValue(value));
        }
        
        public static void ParseMulOp(DamnScriptParser.MulOpContext mulOp, ScriptParserContext* context)
        {
            var type = mulOp.Stop.Type;
            var operation = type switch
            {
                DamnScriptParser.MULTIPLY => ExpressionCall.ExpressionCallType.Multiply,
                DamnScriptParser.DIVIDE => ExpressionCall.ExpressionCallType.Divide,
                DamnScriptParser.MODULO => ExpressionCall.ExpressionCallType.Modulo,
                _ => ExpressionCall.ExpressionCallType.Invalid
            };
            
            context->assembler->ExpressionCall(operation);
        }
        
        public static int AddStringToConstantsIfNotExists(NativeList<NativeStringPtr>* strings, string value)
        {
            var offset = value[0] == '"' ? 1 : 0;
            var length = value.Length - offset * 2;
            var unmanagedString = (NativeString.UnmanagedString*)UnsafeUtilities.ReferenceToPointer(value);
            for (var i = 0; i < strings->Count; i++)
            {
                var p = strings->Begin[i];
                if (p.value->length != length
                    || !UnsafeUtilities.Memcmp(p.value->data, unmanagedString->data + offset, p.value->length)) 
                    continue;
                
                return i;
            }

            var str = NativeString.Alloc(value, offset, length);
            strings->Add(new NativeStringPtr(str));
            return strings->Count - 1;
        }
    }
}