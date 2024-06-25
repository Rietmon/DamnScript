parser grammar DamnScriptParser;

options { tokenVocab=DamnScriptLexer; }

file: region+ EOF;

region: REGION identifier statement;

statement: LEFT_BRACKET statementBody* RIGHT_BRACKET;

statementBody
    : ifStatement
    | callStatement;
    
ifStatement: IF logicalStatement (ELSEIF logicalStatement)* (ELSE statement)?;

logicalStatement: LEFT_PAREN expression RIGHT_PAREN statement;

callStatement: (keyword | methodCall)+ SEMICOLON;

args: arg (COMMA arg)*;

arg: expression;

keyword: KEYWORD;

expression: additiveExpression ((logicalOperator additiveExpression)+)?;

additiveExpression: term ((additiveOperator term)+)?;

term: factor ((multiplicativeOperator factor)+)?;

factor
    : NUMBER                                          # NumberExpression
    | LEFT_PAREN expression RIGHT_PAREN              # ParenthesizedExpression
    | methodCall                                     # MethodCallExpression
    | literal                                        # LiteralExpression
    ;

methodCall: identifier LEFT_PAREN args? RIGHT_PAREN;

literal: NUMBER | STRING;

logicalOperator: (EQUAL | NOT_EQUAL | LESS | LESS_EQUAL | GREATER | GREATER_EQUAL | AND | OR);
additiveOperator: (ADD | SUBTRACT);
multiplicativeOperator: (MULTIPLY | DIVIDE | MODULO);

identifier: NAME;