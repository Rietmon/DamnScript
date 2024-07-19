using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Assemblers;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Parsings.Antlrs
{
    internal static unsafe class ScriptParser
    {
        public static void ParseScript(Stream input, ConstString name, ScriptData* scriptData)
        {
            var charStream = new AntlrInputStream(input);
            var lexer = new DamnScriptLexer(charStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new DamnScriptParser(tokenStream);
            var file = parser.file();
        
            scriptData->name = name.ToString32();

            var regions = new NativeList<RegionData>(16);
        
            var strings = new NativeList<UnsafeStringPair>(16);
            var context = new ScriptParserContext();
            for (var i = 0; i < file.ChildCount - 1; i++)
            {
                var assembler = new ScriptAssembler();
                context.strings = &strings;
                context.assembler = &assembler;
            
                var region = ParseRegion(file.GetChild(i), context);
                regions.Add(region);
            
                context.assembler->Dispose();
            }
        
            if (context.isError)
            {
                scriptData->Dispose();
                *scriptData = default; 
                regions.Dispose();
                strings.Dispose();
                return;
            }
        
            scriptData->regions = regions.ToArrayAlloc();
            regions.Dispose();
        
            scriptData->metadata = new ScriptMetadata(new ConstantsData(strings.ToArrayAlloc()));
            strings.Dispose();
        }

        private static RegionData ParseRegion(IParseTree region, ScriptParserContext context)
        {
            var regionName = region.GetChild(1).GetText();
            var statement = region.GetChild(2);
        
            ParseNode(statement, context);

            var byteCode = context.assembler->FinishAlloc();
            return new RegionData(new String32(regionName), byteCode);
        }

        private static void ParseNode(IParseTree tree, ScriptParserContext context)
        {
            switch (tree)
            {
                case DamnScriptParser.CallStatementContext callStatement:
                    ParseCallStatementBody(callStatement, context);
                    return;
                case DamnScriptParser.AdditiveExpressionContext operatorContext:
                    ParseAdditiveExpression(operatorContext, context);
                    return;
                case DamnScriptParser.IfStatementContext ifStatement:
                    ParseIfStatement(ifStatement, context);
                    return;
                case DamnScriptParser.ForStatementContext forStatement:
                    ParseForStatement(forStatement, context);
                    return;
                case DamnScriptParser.LogicalStatementContext logicalExpression:
                    AssemblyLogicalExpression(logicalExpression, context);
                    return;
            
                case DamnScriptParser.NumberExpressionContext expression:
                    AssemblyNumberExpression(expression, context);
                    break;
                case DamnScriptParser.MethodCallContext methodCall:
                    AssemblyMethodCall(methodCall, context);
                    return;
                case DamnScriptParser.StringLiteralContext stringLiteral:
                    AssemblyStringLiteral(stringLiteral, context);
                    break;
                case DamnScriptParser.AdditiveOperatorContext operatorContext:
                    AssemblyAdditiveOperator(operatorContext, context);
                    break;
                case DamnScriptParser.MultiplicativeOperatorContext operatorContext:
                    AssemblyMultiplicativeOperator(operatorContext, context);
                    break;
                case DamnScriptParser.VariableExpressionContext variableExpression:
                    AssemblyVariableExpression(variableExpression, context);
                    break;
            
                case ErrorNodeImpl errorNode:
                    context.isError = true;
                    Debugging.LogError($"[{nameof(ScriptParser)}] ({nameof(ParseNode)}) Error node: \"{errorNode.GetText()}\"!" +
                                       $" Token number: {errorNode.SourceInterval.a}");
                    return;
            }
        
            if (context.isError)
                return;
        
            for (var i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                ParseNode(child, context);
            }
        }

        private static void ParseIfStatement(DamnScriptParser.IfStatementContext ifStatement, ScriptParserContext context)
        {
            var jumps = stackalloc int[16];
            var jumpsCount = 0;
            for (var i = 0; i < ifStatement.ChildCount; i++)
            {
                var child = ifStatement.GetChild(i);
                if (child is TerminalNodeImpl)
                    continue;
            
                ParseNode(child, context);
            
                if (i >= ifStatement.ChildCount - 1)
                    continue;
            
                jumps[jumpsCount++] = context.assembler->offset;
                context.assembler->Jump(-1);
            }
        
            var targetOffset = context.assembler->offset;
            for (var i = 0; i < jumpsCount; i++)
            {
                var offset = jumps[i];
                context.assembler->offset = offset;
                context.assembler->Jump(targetOffset);
            }
            context.assembler->offset = targetOffset;
        }

        private static void ParseForStatement(DamnScriptParser.ForStatementContext forStatement, ScriptParserContext context)
        {
            var variableName = GetNextNodeOfType<DamnScriptParser.VariableContext>(forStatement).GetText();
            
            var registerIndex = context.ReserveIdentifier(variableName);

            context.assembler->PushToStack(0);
            context.assembler->PushToRegister(registerIndex);
            var beginOffset = context.assembler->offset;
            
            var countExpression = GetNextNodeOfType<DamnScriptParser.ExpressionContext>(forStatement);
            
            var statementContext = GetNextNodeOfType<DamnScriptParser.StatementContext>(forStatement);
            ParseNode(statementContext, context);
            
            context.assembler->PeekFromRegister(registerIndex);
            context.assembler->PushToStack(1);
            context.assembler->ExpressionCall(ExpressionCall.ExpressionCallType.Add);
            context.assembler->DuplicateStack();
            context.assembler->PushToRegister(registerIndex);
            ParseNode(countExpression, context);
            context.assembler->JumpNotEquals(beginOffset);
        }

        private static void ParseCallStatementBody(IParseTree callStatement, ScriptParserContext context)
        {
            var start = callStatement.GetChild(0);
            var keyword = start as DamnScriptParser.KeywordContext;
            var nextIndex = keyword == null ? 0 : 1;
            var methodCall = callStatement.GetChild(nextIndex);
            if (keyword != null)
                AssemblyKeyword(keyword, context);
            AssemblyMethodCall((DamnScriptParser.MethodCallContext)methodCall, context);
        }

        private static void ParseAdditiveExpression(DamnScriptParser.AdditiveExpressionContext operatorContext, ScriptParserContext context)
        {
            var parent = operatorContext.GetChild(0);
            for (var i = 0; i < parent.ChildCount; i++)
            {
                var child = parent.GetChild(i);
                if (child is DamnScriptParser.AdditiveOperatorContext or DamnScriptParser.MultiplicativeOperatorContext)
                    ParseNode(parent.GetChild(++i), context);
                ParseNode(child, context);
            }
        }
    
        private static void AssemblyLogicalExpression(DamnScriptParser.LogicalStatementContext logicalExpression, ScriptParserContext context)
        {
            for (var i = 0; i < logicalExpression.ChildCount; i++)
            {
                var child = logicalExpression.GetChild(i);
                if (child is DamnScriptParser.ExpressionContext)
                    ParseNode(child, context);
            }

            context.assembler->PushToStack(new ScriptValue(1));
            var jumpOffset = context.assembler->offset;
            context.assembler->JumpNotEquals(-1);
        
            for (var i = 0; i < logicalExpression.ChildCount; i++)
            {
                var child = logicalExpression.GetChild(i);
                if (child is DamnScriptParser.StatementContext)
                    ParseNode(child, context);
            }
        
            var offset = context.assembler->offset;
            context.assembler->offset = jumpOffset;
            context.assembler->JumpNotEquals(offset + sizeof(Jump));
            context.assembler->offset = offset;
        }

        // Rietmon: TODO: That's not implemented for now.
        // Also move %noawait% const to lexer.
        private static void AssemblyKeyword(DamnScriptParser.KeywordContext keyword, ScriptParserContext context)
        {
            var text = keyword.GetText();
            var threadParameter = text switch
            {
                "%noawait%" => SetThreadParameters.ThreadParameters.NoAwait,
                _ => SetThreadParameters.ThreadParameters.None
            };
        
            if (threadParameter == SetThreadParameters.ThreadParameters.None)
            {
                Debugging.LogError($"[{nameof(ScriptParser)}] ({AssemblyKeyword}) Unknown keyword: {text}!");
                return;
            }
        
            context.assembler->SetThreadParameters(threadParameter);
        }

        private static void AssemblyMethodCall(DamnScriptParser.MethodCallContext methodCall, ScriptParserContext context)
        {
            var identifier = methodCall.GetChild(0);
            for (var i = 0; i < methodCall.ChildCount; i++)
            {
                var child = methodCall.GetChild(i);
                ParseNode(child, context);
            }
        
            context.assembler->NativeCall(identifier.GetText(), methodCall.ChildCount);
        }

        private static void AssemblyNumberExpression(DamnScriptParser.NumberExpressionContext numberExpression, ScriptParserContext context)
        {
            var number = numberExpression.GetText();
            var value = long.Parse(number);
            context.assembler->PushToStack(value);
        }

        private static void AssemblyStringLiteral(DamnScriptParser.StringLiteralContext stringLiteral, ScriptParserContext context)
        {
            var text = stringLiteral.GetText();
            var value = UnsafeString.Alloc(text, 1, text.Length - 2);
            var hash = value->GetHashCode();
            context.strings->Add(new UnsafeStringPair(hash, value));
            context.assembler->PushStringToStack(hash);
        }
        
        private static void AssemblyVariableExpression(DamnScriptParser.VariableExpressionContext variableExpression, ScriptParserContext context)
        {
            var variableName = variableExpression.GetText();
            var registerIndex = context.GetRegisterIndex(variableName);

            if (registerIndex == -1)
            {
                Debugging.LogError($"[{nameof(ScriptParser)}] ({AssemblyVariableExpression}) Unknown variable: {variableName}!");
                context.isError = true;
                return;
            }
            context.assembler->PeekFromRegister(registerIndex);
        }

        private static void AssemblyMultiplicativeOperator(DamnScriptParser.MultiplicativeOperatorContext multiplicativeOperator, ScriptParserContext context)
        {
            var operatorText = multiplicativeOperator.GetText();
            AssemblyOperator(operatorText, context);
        }

        private static void AssemblyAdditiveOperator(DamnScriptParser.AdditiveOperatorContext additiveOperator, ScriptParserContext context)
        {
            var operatorText = additiveOperator.GetText();
            AssemblyOperator(operatorText, context);
        }

        private static void AssemblyOperator(string operatorText, ScriptParserContext context)
        {
            var operatorType = operatorText switch
            {
                "+" => ExpressionCall.ExpressionCallType.Add,
                "-" => ExpressionCall.ExpressionCallType.Subtract,
                "*" => ExpressionCall.ExpressionCallType.Multiply,
                "/" => ExpressionCall.ExpressionCallType.Divide,
                "%" => ExpressionCall.ExpressionCallType.Modulo,
                "==" => ExpressionCall.ExpressionCallType.Equal,
                "!=" => ExpressionCall.ExpressionCallType.NotEqual,
                ">" => ExpressionCall.ExpressionCallType.Greater,
                ">=" => ExpressionCall.ExpressionCallType.GreaterOrEqual,
                "<" => ExpressionCall.ExpressionCallType.Less,
                "<=" => ExpressionCall.ExpressionCallType.LessOrEqual,
                "&&" => ExpressionCall.ExpressionCallType.And,
                "||" => ExpressionCall.ExpressionCallType.Or,
                "!" => ExpressionCall.ExpressionCallType.Not,
                _ => ExpressionCall.ExpressionCallType.Invalid
            };
            if (operatorType == ExpressionCall.ExpressionCallType.Invalid)
            {
                Debugging.LogError($"[{nameof(ScriptParser)}] ({AssemblyAdditiveOperator}) Unknown operator: {operatorText}!");
                return;
            }
            context.assembler->ExpressionCall(operatorType);
        }

        private static T GetNextNodeOfType<T>(IParseTree node)
        {
            for (var i = 0; i < node.ChildCount; i++)
            {
                var child = node.GetChild(i);
                if (child is T result)
                    return result;
                
                var t = GetNextNodeOfType<T>(child);
                if (t != null)
                    return t;
            }
            return default;
        }
    }
}