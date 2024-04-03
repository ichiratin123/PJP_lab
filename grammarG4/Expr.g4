
program: stat+ EOF;

stat:  line stat?
    | condition stat?
    | loop stat?
    | block stat?
    ;

line: type ID (',' ID)* (';')+
    | expr (';')+
    | WRITE_KW expr (',' expr)*   (';')+
    | READ_KW ID (',' ID)*   (';')+
    | COMMENT 
    ;

condition: IF_KW '('expr')' block
    | IF_KW '('expr')' block ELSE_KW block
    | IF_KW '('expr')' block ELSE_KW line
    | IF_KW '('expr')' line
    | IF_KW '('expr')' line ELSE_KW block
    | IF_KW '('expr')' line ELSE_KW line
    ;

loop: WHILE_KW '('expr')' block
    | WHILE_KW '('expr')' line
    ;

block: '{' line+ '}';

expr: expr op=(MUL|DIV|MOD) expr
    | expr op=(ADD|SUB|CONCAT) expr
    | expr op=(LT|GT|EQ|NOTEQ) expr
    | expr op=(AND|OR) expr
    | SUB expr
    | NOT expr
    | STRING
    | INT
    | ID
    | BOOL
    | '(' expr ')'
    | ID '=' expr
    ;

type: type=INT_KW
    | type=FLOAT_KW
    | type=BOOL_KW
    | type=STRING_KW
    ;

INT_KW : 'int';
FLOAT_KW : 'float';
STRING_KW : 'string';
BOOL_KW : 'bool';
WRITE_KW : 'write';
READ_KW : 'read';
WHILE_KW : 'while';
FOR_KW : 'for';
IF_KW : 'if' ;
ELSE_KW : 'else' ;
SEMI: ';';
COMMA: ',';
MUL : '*' ; 
DIV : '/' ;
ADD : '+' ;
SUB : '-' ;
MOD : '%' ;
GT : '>' ;
LT : '<' ;
EQ : '==' ;
AND : '&&' ;
OR : '||' ;
NOT : '!' ;
NOTEQ: '!=' ;
CONCAT : '.' ;
STRING :  ('"' [a-zA-Z0-9(){}<>,._!?:/*+%=; ]* '-'? [a-zA-Z0-9(){}<>,._!?:/*+%=; ]* '"') | '""';
BOOL : ('true'|'false');
ID : [a-zA-Z_] [a-zA-Z0-9_]* ; 
FLOAT : [0-9]+'.'[0-9]+ ;
INT : [0-9]+ ; 
WS : [ \t\r\n]+ -> skip ;
COMMENT: '//' [a-zA-Z0-9.()"*-+/%,; ]+;