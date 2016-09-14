grammar Mfpl;

root
	: statement*
	;

statement
	: '{' statement* '}'              #BlockStatement
	| 'var' SYNTAX '=' expression ';' #VarStatement
	| SYNTAX '=' expression ';'       #AssignStatement
	| expression ';'                  #ExpressionStatement
	//| 'if' '(' expression ')' statement ('else' statement)?
	//| 'for' '(' statement ';' expression ';' expression ')' statement
	//| 'function' SYNTAX '(' SYNTAX? (',' SYNTAX)* ')' '{' statement '}'
	;

expression
	: value                                           #ValueExpression
	| '-' expression                                  #UnaryExpression
	| '!' expression                                  #UnaryExpression
	| SYNTAX '(' expression? (',' expression)* ')'    #FunctionCallExpression
	| expression ('*' | '/') expression               #BinaryExpression
	| expression ('+' | '-') expression               #BinaryExpression
	| expression ('>' | '<' | '>=' | '<=') expression #BinaryExpression
	| expression ('&&' | '||') expression             #BinaryExpression
	| expression ('==' | '!=') expression             #BinaryExpression
	;



value
    : STRING
    | NUMBER
    //| object
    //| array
    | BOOL
    //| 'null'
    ;

//object
//    : '{' pair (',' pair)* '}'
//    | '{' '}'
//    ;

//pair
//    : STRING ':' value
//    ;

//array
//    : '[' value (',' value)* ']'
//    | '[' ']'
//    ;

BOOL
	: 'true' 
	| 'false'
	;

SYNTAX
	: [a-zA-Z_] [0-9a-zA-Z_]*
	;

STRING
    : '"' (('\\' (["\\/bfnrt] | UNICODE)) | ~ ["\\])* '"'
    | '\'' (('\\' (['\\/bfnrt] | UNICODE)) | ~ ['\\])* '\''
    ;

fragment UNICODE
    : 'u' HEX HEX HEX HEX
    ;

fragment HEX
    : [0-9a-fA-F]
    ;

NUMBER
    : '-'? INT '.' [0-9] + EXP? | '-'? INT EXP | '-'? INT
    ;

fragment INT
    : '0' | [1-9] [0-9]*
    ;

fragment EXP
    : [Ee] [+\-]? INT
    ;

WS
    : [ \t\n\r] +     -> skip
    ;

BlockComment
	: '/*' .*? '*/' -> skip
	;
LineComment
	: '//' ~[\r\n]* -> skip
	;