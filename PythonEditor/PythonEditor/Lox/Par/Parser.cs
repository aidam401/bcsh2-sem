using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PythonEditor.Lox.Exceptions;
using PythonEditor.Lox.Lex;
using PythonEditor.Lox.Par.Model;
using PythonEditor.Lox.Par.Model.Interfaces;

namespace PythonEditor.Lox.Par
{
    public class Parser
    {
        private List<Token> Tokens;
        int Index = 0;

        public Parser(List<Token> tokens)
        {
            this.Tokens = tokens;
        }

        public Program Parse()
        {
            return new Program() { Block = ReadBlock() };
        }

        private Block ReadBlock()
        {
            Block block = new Block();
            while (PeekToken().Type == TokenType.DEF)
                block.Functions.Add(ReadFunction());
            while (!IsEnd()) block.Statements.Add(ReadStatement());

            return block;

        }

        private Function ReadFunction()
        {
            var function = new Function();
            if (ReadToken().Type != TokenType.DEF) throw new ParserException("Invalid token. Expected DEF");
            function.Name = ReadToken().Lexeme;
            if (ReadToken().Type != TokenType.LEFT_PAREN) throw new ParserException("Invalid token. Expected LEFT_PAREN");
            while (PeekToken().Type != TokenType.RIGHT_PAREN)
            {
                function.Args.Add(ReadArg());
                if (PeekToken().Type == TokenType.COMMA)
                    ReadToken();
                continue;
            }
            ReadToken();

            if (ReadToken().Type != TokenType.ARROW) throw new ParserException("Invalid token. Expected ARROW");
            if (!Match(TokenType.IDENT)) throw new ParserException("Invalid token. Expected IDENT");
            switch (ReadToken().Lexeme)
            {
                case "int": function.ReturnType = typeof(int); break;
                case "double": function.ReturnType = typeof(double); break;
                case "str": function.ReturnType = typeof(string); break;
                case "none": break;
                default: throw new ParserException("Invalid token. Expected valid datatype");
            }

            if (PeekToken().Type != TokenType.COLON) throw new ParserException("Invalid token. Expected COLON");
            int functionLevelTabulators = ReadToken().Tabulators;
            if (PeekToken().Tabulators > functionLevelTabulators) new ParserException("Expected TABULATOR on new line.");
            if (PeekToken().Type == TokenType.PASS)
            {
                ReadToken();
                return function;
            }
            while (PeekToken().Tabulators > functionLevelTabulators)
            {
                function.Statements.Add(ReadStatement());   
            }
            return function;
        }

        private Arg ReadArg()
        {
            var arg = new Arg();
            if (PeekToken().Type != TokenType.IDENT) throw new ParserException("Invalid token. Expected IDENT");
            arg.Name = ReadToken().Lexeme;
            if (ReadToken().Type != TokenType.COLON) throw new ParserException("Invalid token. Expected COLON");
            if (!Match(TokenType.IDENT)) throw new ParserException("Invalid token. Expected IDENT");
            switch (ReadToken().Lexeme)
            {
                case "int": arg.Type = typeof(int);break;
                case "double": arg.Type = typeof(double); break;
                case "str": arg.Type = typeof(string); break;
                default: throw new ParserException("Invalid token. Invalid type.");
            }
            return arg;
        }

        private IExecutable ReadStatement()
        {
            switch (PeekToken().Type)
            {
                case TokenType.IF:
                    return ReadIfStatement();
                case TokenType.WHILE:
                    return ReadWhileStatement();
                case TokenType.FOR:
                    return ReadForStatement();
                case TokenType.RETURN:
                    return ReadReturnStatement();

            }
            if (PeekToken(1).Type == TokenType.SET || PeekToken(3).Type == TokenType.SET) return ReadSetStatement();
            if (PeekToken().Type == TokenType.IDENT) return ReadFunctionCall();
            throw new ParserException("Invalid token, Expected IF or WHILE or FOR or IDENT or FUNCTION CALL or RETURN");
        }
        private ReturnStatement ReadReturnStatement()
        {
            if (ReadToken().Type != TokenType.RETURN) throw new ParserException("Invalid token. Expected RETURN.");
            return new ReturnStatement() { Expr = ReadExpression() };
        }
        private FunctionCall ReadFunctionCall()
        {
            var call = new FunctionCall();

            if (PeekToken().Type != TokenType.IDENT) throw new ParserException("Invalid token. Expected IDENT");
            call.Name = ReadToken().Lexeme;
            if (ReadToken().Type != TokenType.LEFT_PAREN) throw new ParserException("Invalid token. Expected LEFT_PAREN");
            while (PeekToken().Type != TokenType.RIGHT_PAREN)
            {

                call.ArgEvals.Add(ReadExpression());
                if (PeekToken().Type == TokenType.COMMA)
                    ReadToken();
            }
            ReadToken();
            return call;
        }

        private Statement ReadIfStatement()
        {
            var statement = new IfStatement();
            if (ReadToken().Type != TokenType.IF) throw new ParserException("Invalid token. Expected IF");
            statement.Condition = ReadCondition();
            int ifLevelTabulators = PeekToken().Tabulators;
            if (ReadToken().Type != TokenType.COLON) throw new ParserException("Invalid token. Expected COLON");

            if (PeekToken().Type == TokenType.PASS)
            {
                ReadToken();
                goto NoIfCode;
            }

            while (PeekToken().Tabulators > ifLevelTabulators) statement.TrueStatements.Add(ReadStatement());
            NoIfCode:
            if (PeekToken().Type == TokenType.ELSE)
            {
                ReadToken();
                if (ReadToken().Type != TokenType.COLON) throw new ParserException("Invalid token. Expected COLON");
                if (PeekToken().Type == TokenType.PASS)
                {
                    ReadToken();
                    return statement;
                };

                while (PeekToken().Tabulators > ifLevelTabulators) statement.ElseStatements.Add(ReadStatement());
            }
            return statement;

        }
        private Statement ReadWhileStatement()
        {
            var statement = new WhileStatement();
            if (ReadToken().Type != TokenType.WHILE) throw new ParserException("Invalid token. Expected WHILE");
            statement.Condition = ReadCondition();
            int whileLevelTabulators = PeekToken().Tabulators;
            if (ReadToken().Type != TokenType.COLON) throw new ParserException("Invalid token. Expected COLON");
            if (PeekToken().Type == TokenType.PASS)
            {
                ReadToken();
                return statement;
            };
            while (PeekToken().Tabulators > whileLevelTabulators) statement.Statements.Add(ReadStatement());
            return statement;
        }
        private Statement ReadForStatement()
        {
            var statement = new ForStatement();
            if (ReadToken().Type != TokenType.FOR) throw new ParserException("Invalid token. Expected FOR");
            int forLevelTabulators = PeekToken().Tabulators;
            statement.Iterator = ReadToken().Lexeme;
            if (ReadToken().Type != TokenType.IN) throw new ParserException("Invalid token. Expected IN");
            if (ReadToken().Type != TokenType.RANGE) throw new ParserException("Invalid token. Expected RANGE");
            if (ReadToken().Type != TokenType.LEFT_PAREN) throw new ParserException("Invalid token. Expected LEFT_PAREN");
            if (PeekToken().Type != TokenType.NUMBER || PeekToken().Literal.GetType() != typeof(int)) throw new ParserException("Invalid token. Expected NUMBER with int literal");
            int first_val = Convert.ToInt32(ReadToken().Literal);
            if (PeekToken().Type == TokenType.RIGHT_PAREN) statement.Max = first_val;
            else
            {
                if (ReadToken().Type != TokenType.COMMA) throw new ParserException("Invalid token. Expected COLON");
                int second_val = Convert.ToInt32(ReadToken().Literal);
                statement.Min = first_val;
                statement.Max = second_val;

                if (PeekToken().Type == TokenType.COMMA)
                {
                    ReadToken();
                    int third_val = Convert.ToInt32(ReadToken().Literal);
                    statement.Step = third_val;

                }
            }
            if (ReadToken().Type != TokenType.RIGHT_PAREN) throw new ParserException("Invalid token. Expected RIGHT_PAREN");
            if (ReadToken().Type != TokenType.COLON) throw new ParserException("Invalid token. Expected COLON");
            if (PeekToken().Type == TokenType.PASS)
            {
                ReadToken();
                return statement;
            };
            while (PeekToken().Tabulators > forLevelTabulators) statement.Statements.Add(ReadStatement());
            return statement;
        }
        private Statement ReadSetStatement()
        {
            var statement = new SetStatement();
            var ident = new TypedIdent();
            ident.Name = ReadToken().Lexeme;
            if (PeekToken().Type == TokenType.COLON)
            {
                ReadToken();
                if (!Match(TokenType.IDENT)) throw new ParserException("Invalid token. Expected IDENT");
                switch (ReadToken().Lexeme)
                {
                    case "int": ident.Type = typeof(int); break;
                    case "double": ident.Type = typeof(double); break;
                    case "str": ident.Type = typeof(string); break;
                    default: throw new ParserException("Invalid token. Expected valid datatype");
                }
            }
            if (ReadToken().Type != TokenType.SET) throw new ParserException("Invalid token. Expected SET");
            statement.Expr = ReadExpression();
            statement.Ident = ident;
            return statement;
        }
        private IEvaluable ReadExpression()
        {
            IEvaluable left = ReadBinaryExpression();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token oper = ReadToken();
                IEvaluable right = ReadBinaryExpression();
                left = new BinaryExpression() { Left = left, Operator = oper.Type, Right = right };
            }

            return left;
        }

        private IEvaluable ReadBinaryExpression()
        {

            IEvaluable left = ReadUnaryExpression();

            while (Match(TokenType.SLASH, TokenType.STAR, TokenType.MODULO, TokenType.DOUBLE_STAR))
            {
                Token oper = ReadToken();
                IEvaluable right = ReadUnaryExpression();
                left = new BinaryExpression() { Left = left, Operator = oper.Type, Right = right };
            }

            return left;
        }

        private IEvaluable ReadUnaryExpression()
        {
            if (TokenType.MINUS == PeekToken().Type)
            {
                TokenType oper = ReadToken().Type;
                IEvaluable right = ReadUnaryExpression();
                return new UnaryExpression() { Eval = right, Operator = oper };
            }
            if (PeekToken().Type == TokenType.LEFT_PAREN)
            {
                ReadToken();
                var exp = ReadExpression();
                if (ReadToken().Type != TokenType.RIGHT_PAREN) throw new ParserException("Invalid token. Expected RIGHT_PARENT.");
                return exp;
            }
            if (PeekToken().Type == TokenType.IDENT && PeekToken(1).Type == TokenType.LEFT_PAREN)
                return ReadFunctionCall();

            if (PeekToken().Type == TokenType.IDENT)
                return new IdentExpression() { Name = ReadToken().Lexeme };
            

            return ReadLiteralExpression();
        }

        private Expression ReadLiteralExpression()
        {
            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new LiteralExpression() { Value = ReadToken().Literal };
            }


            throw new ParserException("Invalid token. Expected STRING or INT of DOUBLE");
        }


        private Condition ReadCondition()
        {
            if (PeekToken().Type == TokenType.NOT)
            {
                ReadToken();
                return new NotCondition() { Condition = ReadCondition() };
            }

            var left = ReadExpression();

            if (!Match(TokenType.EQUALS, TokenType.LESS, TokenType.LESS_EQUALS, TokenType.GREATER, TokenType.GREATER_EQUALS, TokenType.NOT_EQUALS))
                throw new ParserException("Invalid token. Expected LESS or LESS_EQUALS or GREATER or GREATER_EQUALS or NOT_EQUALS");

            var oper = ReadToken().Type;
            var right = ReadExpression();
            var cond = new BinaryCondition() { Left = left, Operator = oper, Right = right };

            if (Match(TokenType.AND, TokenType.OR))
                return new BinaryCondition() { Left = cond, Operator = ReadToken().Type, Right = ReadCondition() };

            return cond;
        }


        private Token PeekToken() { return this.Tokens[this.Index]; }
        private Token PeekToken(int offset) {
            if (Index+offset >= Tokens.Count) return new Token(TokenType.EOF, "EOF", 0, 0,0);
            return this.Tokens[this.Index + offset]; 
        }
        private bool Match(params TokenType[] types)
        {
            if (Index == Tokens.Count) return false;
            return types.Contains(PeekToken().Type);
        }
        private Token ReadToken() { return this.Tokens[this.Index++]; }

        private bool IsEnd() { return this.Index >= Tokens.Count || PeekToken().Type == TokenType.EOF; }

    }
}
