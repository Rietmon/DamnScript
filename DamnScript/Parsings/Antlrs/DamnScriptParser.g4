parser grammar DamnScriptParser;

options { tokenVocab=DamnScriptLexer; }

program: region+;

region: REGION name block;

block: LEFT_BRACKET statement* RIGHT_BRACKET;

statement
    : ifStatement
    | forStatement
    | callStatement;

condition: LEFT_PAREN expression RIGHT_PAREN;

ifStatement: IF condition block (ELSEIF condition block)* (ELSE block)?;

forStatement: FOR LEFT_PAREN var IN expression RIGHT_PAREN block;

callStatement: funcCall SEMICOLON;

arguments: argument (COMMA argument)*;

argument: expression;

expression: additiveExpression (logicalOp additiveExpression)*;

additiveExpression: term (addOp term)*;

term: factor (mulOp factor)*;

factor
    : NUMBER                  # Number
    | LEFT_PAREN expression RIGHT_PAREN  # Parens
    | funcCall                # MethodCall
    | STRING                    # String
    | var                  # Variable
    ;

funcCall: name LEFT_PAREN arguments? RIGHT_PAREN;

var: name;

logicalOp: (EQUAL | NOT_EQUAL | LESS | LESS_EQUAL | GREATER | GREATER_EQUAL | AND | OR);
addOp: (ADD | SUBTRACT);
mulOp: (MULTIPLY | DIVIDE | MODULO);

name: NAME;
