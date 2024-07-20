lexer grammar DamnScriptLexer;

REGION : 'region';
KEYWORD: '%' NAME '%';

NUMBER  : [0-9]+;
STRING  : '"' .*? '"';

IF     : 'if';
ELSEIF : 'elseif';
ELSE   : 'else';

FOR    : 'for';
IN    : 'in';

WHILE  : 'while';

AND    : '&&';
OR     : '||';

NAME   : [a-zA-Z_][a-zA-Z_0-9]*;

COMMA         : ',';
DOT           : '.';
SEMICOLON     : ';';
LEFT_BRACKET  : '{';
RIGHT_BRACKET : '}';
LEFT_PAREN    : '(';
RIGHT_PAREN   : ')';

MULTIPLY  : '*';
DIVIDE    : '/';
ADD       : '+';
SUBTRACT  : '-';
MODULO    : '%';

EQUAL    : '==';
NOT_EQUAL: '!=';
LESS : '<';
GREATER: '>';
LESS_EQUAL: '<=';
GREATER_EQUAL: '>=';

WS: [ \t\r\n]+ -> skip;
