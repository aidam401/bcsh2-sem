using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PythonEditor.Lox.Exceptions;
using PythonEditor.Lox.Interp.Model;
using PythonEditor.Lox.Interp.Model.Interfaces;
using PythonEditor.Lox.Lex;
using PythonEditor.Lox.Par.Model;
using PythonEditor.Lox.Par.Model.Interfaces;
using PythonEditor.ViewModels;

namespace PythonEditor.Lox.Interp
{

    public class Interpreter
    {
        private Program Program;
        private EditorViewModel EditorViewModel;
        public Interpreter(Program program, EditorViewModel editorViewModel)
        {
            this.Program = program;
            this.EditorViewModel = editorViewModel;
        }
        public void Run()
        {
            Visitors visitors = Visitors.GetInstance();
            visitors.EditorViewModel = EditorViewModel;
            visitors.Exec(this.Program);

        }
    }
    public class Visitors : IExecutableVisitor, IEvaluableVisitor
    {
        private static readonly Context ProgramContext = new Context();

        public EditorViewModel EditorViewModel { set; get; }
        private static Visitors VisitorsSingleton { get; set; }
        public static Visitors GetInstance()
        {
            if (VisitorsSingleton == null)
                VisitorsSingleton = new Visitors();
            return VisitorsSingleton;
        }
        //IExecutableVisitor
        public PossibleReturn VisitProgram(Program program)
        {
            Exec(program.Block);
            return new PossibleReturn();
        }
        public PossibleReturn VisitBlock(Block block)
        {
            block.Functions.ForEach(func => Eval(func));
            foreach (var stat in block.Statements)
            {
                var ret = Exec(stat);
                if (!ret.IsVoid && ret.Value == null)
                    continue;
                return ret;
            }
            return new PossibleReturn() { IsVoid = true };

        }

        public PossibleReturn VisitExecFunctionCall(FunctionCall functionCall)
        {
            return ProgramContext.ExecuteFunction(functionCall);
        }
        public PossibleReturn VisitPrintStatement(PrintStatement printStatement)
        {
            EditorViewModel.ConsoleText = EditorViewModel.ConsoleText + Convert.ToString(Eval(printStatement.Evaluable)) + "\n";
            return new PossibleReturn();
        }
        public PossibleReturn VisitSetStatement(SetStatement setStatement)
        {
            var evaluation = Eval(setStatement.Expr);
            var alreadyExist = ProgramContext.GetIdent(new IdentExpression() { Name = setStatement.Ident.Name });
            if (alreadyExist != null)
            {
                if (evaluation.GetType() != Eval(alreadyExist.Value).GetType()) throw new InterpreterException("Invalid type.");
                alreadyExist.Value = new LiteralExpression() { Value = evaluation };
                ProgramContext.AddIdent(alreadyExist);
                return new PossibleReturn();
            }
            if (evaluation.GetType() != setStatement.Ident.Type) throw new InterpreterException("Invalid type.");
            ProgramContext.AddIdent(new IdentExpression()
            {
                Name = setStatement.Ident.Name,
                Value = new LiteralExpression() { Value = evaluation }
            });
            return new PossibleReturn();
        }

        public PossibleReturn VisitIfStatement(IfStatement ifStatement)
        {
            ProgramContext.CreateNewContext();
            if (Convert.ToBoolean(Eval(ifStatement.Condition)))
            {

                foreach (var stat in ifStatement.TrueStatements)
                {
                    var ret = Exec(stat);
                    if (!ret.IsVoid && ret.Value == null)
                        continue;
                    ProgramContext.ReturnToLastContext();
                    return ret;
                }

            }
            else
            {
                if (ifStatement.ElseStatements.Count() != 0)
                {
                    foreach (var stat in ifStatement.ElseStatements)
                    {
                        var ret = Exec(stat);
                        if (!ret.IsVoid && ret.Value == null)
                            continue;
                        ProgramContext.ReturnToLastContext();
                        return ret;
                    }
                }
            }
            ProgramContext.ReturnToLastContext();
            return new PossibleReturn() { IsVoid = true };
        }


        public PossibleReturn VisitWhileStatement(WhileStatement whileStatement)
        {
            ProgramContext.CreateNewContext();
            while (Convert.ToBoolean(Eval(whileStatement.Condition)))
            {
                foreach (var statement in whileStatement.Statements)
                {
                    var ret = Exec(statement);
                    if (!ret.IsVoid && ret.Value == null)
                        continue;
                    ProgramContext.ReturnToLastContext();
                    return ret;
                }
            }
            ProgramContext.ReturnToLastContext();
            return new PossibleReturn();
        }

        public PossibleReturn VisitForStatement(ForStatement forStatement)
        {
            ProgramContext.CreateNewContext();
            for (int i = forStatement.Min; i < forStatement.Max; i = i + forStatement.Step)
            {
                //Adding "iterator" variable to context
                Exec(new SetStatement()
                {
                    Ident = new TypedIdent() { Name = forStatement.Iterator, Type = typeof(int) },
                    Expr = new LiteralExpression() { Value = i }
                });

                foreach (var statement in forStatement.Statements)
                {
                    var ret = Exec(statement);
                    if (!ret.IsVoid && ret.Value == null)
                        continue;
                    ProgramContext.ReturnToLastContext();
                    return ret;
                }
            }
            ProgramContext.ReturnToLastContext();
            return new PossibleReturn();
        }
        public PossibleReturn VisitReturnStatement(ReturnStatement returnStatement)
        {
            return new PossibleReturn() { Value = Eval(returnStatement.Expr) };
        }
        public object VisitConvertExpression(ConvertExpression convertStatement)
        {
            try { return Convert.ChangeType(Eval(convertStatement.Expr), convertStatement.NewType); }
            catch { throw new InterpreterException("Invalid conversion"); }
        }
        public object VisitInputExpression(InputExpression inputExpression)
        {
            object ret = "";
            //this.EditorViewModel.InputCommand.IsEnabled = true;
            
            while (!this.EditorViewModel.InputCommand.IsReady)
            {
                Thread.Sleep(1);
            }
            this.EditorViewModel.InputCommand.IsReady = false;
            ret = this.EditorViewModel.InputText;
            this.EditorViewModel.InputCommand.IsReaded= true;

            return ret;
        }
        //IEvaluableVisitor

        public void VisitFunction(Function function) { ProgramContext.AddFunction(function); }

        public object VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            object val = Eval(unaryExpression.Eval);
            if (val.GetType() == typeof(string))
            {
                throw new InterpreterException("Interpreter exception - not supported operator for string type.");
            }
            else if (val.GetType() == typeof(int))
            {
                switch (unaryExpression.Operator)
                {
                    case TokenType.PLUS: return val;
                    case TokenType.MINUS: return -Convert.ToInt32(val);
                    default: throw new InterpreterException("Interpreter exception - not supported operator for int type.");
                }
            }
            else if (val.GetType() == typeof(double))
            {
                switch (unaryExpression.Operator)
                {
                    case TokenType.PLUS: return val;
                    case TokenType.MINUS: return -Convert.ToDouble(val);
                    default: throw new InterpreterException("Interpreter exception - not supported operator for double type.");
                }
            }
            throw new InterpreterException("Interpreter exception - NOT SUPPORTED");
        }

        public object VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            var left = Eval(binaryExpression.Left);
            var right = Eval(binaryExpression.Right);
            CheckSameTypes(right, left);
            if (left.GetType() == typeof(string))
            {
                if (binaryExpression.Operator == TokenType.PLUS) return Convert.ToString(left) + Convert.ToString(right);
                else throw new InterpreterException("Interpreter exception - not supported operator for string type.");
            }
            if (left.GetType() == typeof(int))
            {
                switch (binaryExpression.Operator)
                {
                    case TokenType.PLUS: return Convert.ToInt32(left) + Convert.ToInt32(right);
                    case TokenType.MINUS: return Convert.ToInt32(left) - Convert.ToInt32(right);
                    case TokenType.STAR: return Convert.ToInt32(left) * Convert.ToInt32(right);
                    case TokenType.DOUBLE_STAR: return Math.Pow(Convert.ToInt32(left), Convert.ToInt32(right));
                    case TokenType.SLASH: return Convert.ToInt32(left) / Convert.ToInt32(right);
                    case TokenType.MODULO: return Convert.ToInt32(left) % Convert.ToInt32(right);
                    default: throw new InterpreterException("Interpreter exception - not supported operator for int type.");
                }
            }
            if (left.GetType() == typeof(double))
            {
                switch (binaryExpression.Operator)
                {
                    case TokenType.PLUS: return Convert.ToDouble(left) + Convert.ToDouble(right);
                    case TokenType.MINUS: return Convert.ToDouble(left) - Convert.ToDouble(right);
                    case TokenType.STAR: return Convert.ToDouble(left) * Convert.ToDouble(right);
                    case TokenType.DOUBLE_STAR: return Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right));
                    case TokenType.SLASH: return Convert.ToDouble(left) / Convert.ToDouble(right);
                    case TokenType.MODULO: return Convert.ToDouble(left) % Convert.ToDouble(right);
                    default: throw new InterpreterException("Interpreter exception - not supported operator for double type.");
                }
            }
            throw new InterpreterException("Interpreter exception - not supported.");
        }

        public object VisitIdentExpression(IdentExpression identExpression)
        {
            if (identExpression.Value != null)
                return Eval(identExpression.Value);
            return Eval(ProgramContext.GetIdent(identExpression).Value);
        }

        public object VisitLiteralExpression(LiteralExpression literalExpression) { return literalExpression.Value; }

        public bool VisitBinaryCondition(BinaryCondition binaryCondition)
        {
            var left = Eval(binaryCondition.Left);
            var right = Eval(binaryCondition.Right);

            if (left.GetType() != right.GetType())
                throw new InterpreterException("Interpreter exception - same types required.");
            if (left.GetType() == typeof(string) && (binaryCondition.Operator != TokenType.EQUALS || binaryCondition.Operator != TokenType.NOT_EQUALS))
                throw new InterpreterException("Interpreter exception - string types with wrong operator in condition.");
            if (left.GetType() == typeof(bool) && (binaryCondition.Operator != TokenType.AND || binaryCondition.Operator != TokenType.OR))
                throw new InterpreterException("Interpreter exception - boolean types with wrong operator in condition.");

            switch (binaryCondition.Operator)
            {
                case (TokenType.EQUALS):
                    return left.ToString() == right.ToString();
                case (TokenType.NOT_EQUALS):
                    return left.ToString() != right.ToString();
                case (TokenType.LESS):
                    return Convert.ToDouble(left) < Convert.ToDouble(right);
                case (TokenType.LESS_EQUALS):
                    return Convert.ToDouble(left) <= Convert.ToDouble(right);
                case (TokenType.GREATER):
                    return Convert.ToDouble(left) > Convert.ToDouble(right);
                case (TokenType.GREATER_EQUALS):
                    return Convert.ToDouble(left) >= Convert.ToDouble(right);
                case (TokenType.OR):
                    return Convert.ToBoolean(left) || Convert.ToBoolean(right);
                case (TokenType.AND):
                    return Convert.ToBoolean(left) && Convert.ToBoolean(right);
            }
            throw new InterpreterException("Interpreter exception - unexpected operator");
        }
        public object VisitAcceptFunctionCall(FunctionCall functionCall)
        {
            var ret = ProgramContext.ExecuteFunction(functionCall);
            if (ret.IsVoid) throw new InterpreterException("Void function call cant be used in expression.");
            return ret.Value;
        }
        public bool VisitNotCondition(NotCondition oddCondition) { return !Convert.ToBoolean(Eval(oddCondition)); }
        public object Eval(IEvaluable ev) { return ev.Accept(this); }
        public PossibleReturn Exec(IExecutable ex) { return ex.Execute(this); }
        private void CheckSameTypes(object first, object second)
        {
            if (first.GetType() != second.GetType())
                throw new InterpreterException("Interpreter exception. Expected same types.");
        }

       
    }

}
