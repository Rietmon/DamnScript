using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Assemblers;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

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
            
            var program = parser.program();

            scriptData->name = name;

            var regions = new NativeList<RegionData>(16);

            var strings = new NativeList<UnsafeStringPtr>(16);
            var methods = new NativeList<UnsafeStringPtr>(16);
            var context = new ScriptParserContext();
            for (var i = 0; i < program.ChildCount; i++)
            {
                var assembler = new ScriptAssembler(0);
                context.strings = &strings;
                context.methods = &methods;
                context.assembler = &assembler;

                var regionContext = program.GetChild(i) as DamnScriptParser.RegionContext;
                ParseRegion(regionContext, &context);

                var byteCode = context.assembler->FinishAlloc();
                context.assembler->Dispose();
                
                var region = new RegionData(context.name, byteCode);
                regions.Add(region);
            }

            if (context.isError)
            {
                Debugging.LogError($"[{nameof(ScriptParser)}] ({nameof(ParseScript)}) " +
                                   $"Error while parsing script {scriptData->name}!");
                scriptData->Dispose();
                *scriptData = default;
                regions.Dispose();
                strings.Dispose();
                return;
            }

            scriptData->regions = regions.ToArrayAlloc();
            regions.Dispose();

            scriptData->metadata = new ScriptMetadata(new ConstantsData(strings.ToArrayAlloc(), methods.ToArrayAlloc()));
            strings.Dispose();
        }

        public static void ParseRegion(DamnScriptParser.RegionContext region, ScriptParserContext* context)
        {
            var regionName = region.name().GetText();
            var regionBlock = region.block();
            context->name = new String32(regionName);
            ParseBlock(regionBlock, context);
        }
        
        public static void ParseBlock(DamnScriptParser.BlockContext block, ScriptParserContext* context)
        {
            var statements = block.statement();
            for (var i = 0; i < statements.Length; i++)
            {
                var statement = statements[i].GetChild(0);
                switch (statement)
                {
                    case DamnScriptParser.CallStatementContext callStatement:
                        ParseCallStatement(callStatement, context);
                        break;
                    case DamnScriptParser.IfStatementContext ifStatement:
                        ParseIfStatement(ifStatement, context);
                        break;
                    case DamnScriptParser.ForStatementContext forStatement:
                        ParseForStatement(forStatement, context);
                        break;
                    case DamnScriptParser.WhileStatementContext whileStatement:
                        ParseWhileStatement(whileStatement, context);
                        break;
                }
            }
        }
        
        public static void ParseCallStatement(DamnScriptParser.CallStatementContext callStatement, ScriptParserContext* context)
        {
            var functionCall = callStatement.funcCall();
            var functionName = functionCall.name().GetText();
            var arguments = functionCall.arguments();
            var argumentCount = arguments?.ChildCount ?? 0;
            if (argumentCount > 0)
            {
                for (var i = 0; i < arguments!.ChildCount; i++)
                {
                    var argument = arguments.GetChild<DamnScriptParser.ArgumentContext>(i);
                    var expression = argument.expression();
                    ParseExpression(expression, context);
                }
            }

            var index = AddStringToConstantsIfNotExists(context->methods, functionName);
            context->assembler->NativeCall(index, argumentCount);
        }
        
        public static void ParseIfStatement(DamnScriptParser.IfStatementContext ifStatement, ScriptParserContext* context)
        {
            var branches = stackalloc int[32];
            var branchCount = 0;

            var conditions = ifStatement.condition();
            var blocks = ifStatement.block();
            
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
        
        public static void ParseForStatement(DamnScriptParser.ForStatementContext forStatement, ScriptParserContext* context)
        {
            var variable = forStatement.var();
            var variableName = new String32(variable.GetText());
            
            var register = context->ReserveIdentifier(variableName);
            
            context->assembler->PushToStack(0);
            context->assembler->StoreToRegister(register);
            
            var beginOffset = context->assembler->offset;

            var block = forStatement.block();
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
        
        public static void ParseWhileStatement(DamnScriptParser.WhileStatementContext whileStatement, ScriptParserContext* context)
        {
            var beginOffset = context->assembler->offset;
            var conditionExpression = whileStatement.condition().expression();
            ParseExpression(conditionExpression, context);
            context->assembler->PushToStack(1);
            var jumpConditionOffset = context->assembler->offset;
            context->assembler->JumpNotEquals(-1);
            
            var block = whileStatement.block();
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
                        AssemblyAddOp(addOp, context);
                        break;
                }
            }
        }
        
        public static void AssemblyAddOp(DamnScriptParser.AddOpContext addOp, ScriptParserContext* context)
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
            var factor = term.factor();
            var mulOp = term.mulOp();
            for (var i = 0; i < factor.Length; i++)
            {
                var child = factor[i];
                switch (child)
                {
                    case DamnScriptParser.NumberContext number:
                        AssemblyNumber(number, context);
                        break;
                    case DamnScriptParser.ParensContext parensExpression:
                        AssemblyParens(parensExpression, context);
                        break;
                    case DamnScriptParser.MethodCallContext functionCall:
                        AssemblyMethodCall(functionCall, context);
                        break;
                    case DamnScriptParser.StringContext stringContext:
                        AssemblyString(stringContext, context);
                        break;
                    case DamnScriptParser.VariableContext variable:
                        AssemblyVariable(variable, context);
                        break;
                }
                
                var mulIndex = i - 1;
                if (mulIndex >= 0 && mulIndex % 2 == 0)
                    AssemblyMulOp(mulOp[mulIndex], context);
            }
        }
        
        public static void AssemblyMulOp(DamnScriptParser.MulOpContext mulOp, ScriptParserContext* context)
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
        
        public static void AssemblyNumber(DamnScriptParser.NumberContext numberContext, ScriptParserContext* context)
        {
            var number = numberContext.GetText();
            var value = long.Parse(number);
            context->assembler->PushToStack(new ScriptValue(value));
        }
        
        public static void AssemblyParens(DamnScriptParser.ParensContext parensExpression, ScriptParserContext* context)
        {
            var expression = parensExpression.expression();
            ParseExpression(expression, context);
        }

        public static void AssemblyMethodCall(DamnScriptParser.MethodCallContext methodCall, ScriptParserContext* context)
        {
            var functionCall = methodCall.funcCall();
            var functionName = functionCall.name().GetText();
            var arguments = functionCall.arguments();
            if (arguments != null)
            {
                for (var i = 0; i < arguments.ChildCount; i++)
                {
                    var argument = arguments.GetChild<DamnScriptParser.ArgumentContext>(i);
                    var expression = argument.expression();
                    ParseExpression(expression, context);
                }
            }

            var index = AddStringToConstantsIfNotExists(context->methods, functionName);
            context->assembler->NativeCall(index, arguments?.ChildCount ?? 0);
        }
        
        public static void AssemblyString(DamnScriptParser.StringContext stringContext, ScriptParserContext* context)
        {
            var text = stringContext.GetText();
            
            var index = AddStringToConstantsIfNotExists(context->strings, text);
            context->assembler->PushStringToStack(index);
        }
        
        public static void AssemblyVariable(DamnScriptParser.VariableContext variable, ScriptParserContext* context)
        {
            var variableName = variable.GetText();
            var str32 = new String32(variableName);
            var registerIndex = context->GetRegisterIndex(str32);
            if (registerIndex == -1)
            {
                Debugging.LogError($"[{nameof(ScriptParser)}] ({nameof(AssemblyVariable)}) " +
                                   $"Variable {variableName} not found!");
                context->isError = true;
                return;
            }
            
            context->assembler->LoadFromRegister(registerIndex);
        }
        
        public static int AddStringToConstantsIfNotExists(NativeList<UnsafeStringPtr>* strings, string value)
        {
            var offset = value[0] == '"' ? 1 : 0;
            var length = value.Length - offset * 2;
            var str = UnsafeString.Alloc(value, offset, length);
            var index = strings->IndexOf((p) => 
                p.value->length == str->length 
                && UnsafeUtilities.Memcmp(p.value, str, p.value->length));
            
            if (index == -1)
            {
                strings->Add(new UnsafeStringPtr(str));
                return strings->Count - 1;
            }

            UnsafeUtilities.Free(str);
            return index;
        }
    }
}