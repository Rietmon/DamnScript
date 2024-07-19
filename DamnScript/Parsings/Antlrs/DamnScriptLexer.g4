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

EQUALS    : '==';
NOT_EQUALS: '!=';
LESS_THAN : '<';
GREATER_THAN: '>';
LESS_THAN_OR_EQUAL: '<=';
GREATER_THAN_OR_EQUAL: '>=';

WS: [ \t\r\n]+ -> skip;
