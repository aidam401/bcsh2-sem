using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonEditor.Lox.Lex
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Lexeme { get; set; }
        public object Literal { get; set; }
        public int Line { get; set; }
        public int Tabulators { get; set; }
        public Token(TokenType type, string lexeme, object literal, int line, int tabulators)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
            Tabulators = tabulators;
        }

        public override string? ToString()
        {
            return Type + " " + Lexeme; ;
        }
    }
}
