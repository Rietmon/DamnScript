parser grammar DamnScriptParser;

options { tokenVocab=DamnScriptLexer; }

file: region+ EOF;

region: REGION NAME statement;

statement: LEFT_BRACKET statementBody* RIGHT_BRACKET;

statementBody
    : ifStatement
    | callStatement;
    
ifStatement: IF logicalStatement (ELSEIF logicalStatement)? (ELSE statement)?;

logicalStatement: LEFT_PAREN expression RIGHT_PAREN statement+;

callStatement: (keyword | methodCall)+ SEMICOLON;

args: arg (COMMA arg)*;

arg: expression;

keyword: KEYWORD;

expression
    : LEFT_PAREN expression RIGHT_PAREN                             # ParenExpr
    | term ((ADD | SUBTRACT) term)*      # AdditiveExpression
    | expression op=(EQUAL | NOT_EQUAL | LESS | LESS_EQUAL | GREATER | GREATER_EQUAL) expression # ComparisonExpr
    | expression op=(AND | OR) expression                           # LogicalExpr
    ;

term
    : factor ((MULTIPLY | DIVIDE | MODULO) factor)*    # MultiplicativeExpression
    ;

factor
    : NUMBER                                          # NumberExpression
    | LEFT_PAREN expression RIGHT_PAREN                              # ParenthesizedExpression
    | methodCall # MethodCallExpression
    | literal                                        # LiteralExpression
    ;

methodCall: NAME LEFT_PAREN args? RIGHT_PAREN;

literal: NUMBER | STRING;
