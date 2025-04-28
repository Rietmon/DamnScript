parser grammar DamnScriptParser;

options { tokenVocab=DamnScriptLexer; }

entry: regionGroup+ (subroutineGroup*?);

regionGroup: REGION name blockGroup;

subroutineGroup: ROUTINE name (LEFT_PAREN (var (COMMA var)*)* RIGHT_PAREN) blockGroup;

blockGroup: LEFT_BRACKET statementUnion* RIGHT_BRACKET;

statementUnion
    : ifStatementGroup
    | forStatementGroup
    | whileStatementGroup
    | callStatementGroup;

ifStatementGroup: IF condition blockGroup (ELSEIF condition blockGroup)* (ELSE blockGroup)?;
forStatementGroup: FOR LEFT_PAREN var IN expression RIGHT_PAREN blockGroup;
whileStatementGroup: WHILE condition blockGroup;

callStatementGroup: anyCall SEMICOLON;

factorUnion
    : num
    | keywords
    | parens
    | anyCall
    | str
    | var
    ;

num: NUMBER;

keywords
    : TRUE
    | FALSE
    | NULL
    ;

parens: LEFT_PAREN expression RIGHT_PAREN;

condition: LEFT_PAREN expression RIGHT_PAREN;

anyCall: (funcCall | objectCall);
objectCall: funcCall DOT anyCall;
funcCall: name LEFT_PAREN arguments? RIGHT_PAREN;

str: STRING;

var: name;

arguments: argument (COMMA argument)*;
argument: expression;

expression: additiveExpression (logicalOp additiveExpression)*;
additiveExpression: term (addOp term)*;
term: factorUnion (mulOp factorUnion)*;

logicalOp: (EQUAL | NOT_EQUAL | LESS | LESS_EQUAL | GREATER | GREATER_EQUAL | AND | OR);
addOp: (ADD | SUBTRACT);
mulOp: (MULTIPLY | DIVIDE | MODULO);

name: NAME;