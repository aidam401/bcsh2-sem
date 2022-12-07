using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using PythonEditor.Lox.Exceptions;

namespace PythonEditor.Lox.Lex
{
    public class Lexer
    {
        private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "ident", TokenType.IDENT },
        //{ "str", TokenType.STRING },
        //{ "int", TokenType.INT },
        //{ "double", TokenType.DOUBLE},
        //{ "bool", TokenType.BOOL},
        { "none", TokenType.NONE },
        { "and", TokenType.AND},
        { "for", TokenType.FOR},
        //{ "elif", TokenType.ELIF},
        //{ "from", TokenType.FROM},
        { "return", TokenType.RETURN},
        //{ "break", TokenType.BREAK},
        { "else", TokenType.ELSE},
        { "not", TokenType.NOT},
        { "if", TokenType.IF},
        { "or", TokenType.OR},
        { "while", TokenType.WHILE},
        //{ "continue", TokenType.CONTINUE},
        { "pass", TokenType.PASS},
        { "def", TokenType.DEF},
        { "in", TokenType.IN},
        { "range", TokenType.RANGE},
        //{ "print", TokenType.PRINT},


    };

        private string source;
        private List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private int tabulators = 0;

        public Lexer(string source)
        {
            this.source = source;
        }

        public List<Token> GetTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                GetToken();
            }
            AddToken(TokenType.EOF);
            return tokens;
        }

        private void GetToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(Match('>') ? TokenType.ARROW : TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case '%': AddToken(TokenType.MODULO); break;
                case ':': AddToken(TokenType.COLON); break;
                case '*': AddToken(Match('*') ? TokenType.DOUBLE_STAR : TokenType.STAR); break;
                case '/': AddToken(TokenType.SLASH); break;
                case '!':
                    if (Match('='))
                        AddToken(TokenType.NOT_EQUALS);
                    break;
                case '=': AddToken(Match('=') ? TokenType.EQUALS : TokenType.SET); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUALS : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUALS : TokenType.GREATER); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '"': str(); break;
                case ' ':
                case '\t':
                    break;
                case '\r':
                    if (Match('\n'))
                    {
                        line++;
                        int counter = 0;
                        while (IsTabulator())
                        {
                            counter++;
                        }
                        tabulators = counter;
                    }
                    break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    break;
            }
        }
        private void str()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
            {
                throw new LoxException("Unterminated string.");
            }

            // The closing ".
            Advance();

            // Trim the surrounding quotes.
            string value = source.Substring(start + 1, current - start - 2);
            addToken(TokenType.STRING, value);
        }

        private bool IsTabulator()
        {
            if (Peek() == '\t')
            {
                Advance();
                return true;
            }
            for (int i = 0; i < 4; i++)
                if (Peek(i) != ' ')
                    return false;
            for (int i = 0; i < 4; i++)
                Advance();
            return true;
        }
        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        private char Advance()
        {
            return source[current++];
        }

        private void AddToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line, tabulators));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        private bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                    c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek())) Advance();
            }
            var numStr = source.Substring(start, current - start);
            if(numStr.Contains('.'))
                addToken(TokenType.NUMBER, double.Parse(numStr, System.Globalization.CultureInfo.InvariantCulture));
            else
                addToken(TokenType.NUMBER, Convert.ToInt32(numStr));

        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }
        private char Peek(int offset)
        {
            if (current + offset >= source.Length) return '\0';
            return source[current + offset];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = source.Substring(start, current - start);
            TokenType type;
            keywords.TryGetValue(text.ToLower(), out type);

            if (type == null) type = TokenType.IDENT;
            AddToken(type);
        }
    }
}
