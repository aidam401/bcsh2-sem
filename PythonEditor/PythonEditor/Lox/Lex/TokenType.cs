using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonEditor.Lox.Lex
{
    public enum TokenType
    {
        //KEYWORDS
        IDENT,

        STRING,
        NUMBER,
        NONE,
        COMMA,
        AND,
        FOR,
        ELIF,
        FROM,
        RETURN,
        ELSE,
        NOT,
        IF,
        OR,
        WHILE,
        ARROW,
        PASS,
        DEF,
        IN,
        RANGE,
        PLUS,
        MINUS,
        STAR,
        SLASH,
        DOUBLE_STAR,
        LESS,
        LESS_EQUALS,
        GREATER,
        GREATER_EQUALS,
        NOT_EQUALS,
        EQUALS,
        LEFT_PAREN,
        RIGHT_PAREN,
        COLON,
        DOT,
        SET,
        MODULO,
        EOF
    }
}
