using System;
using PythonEditor.Lox.Interp.Model.Interfaces;
using PythonEditor.Lox.Lex;
using PythonEditor.Lox.Par.Model.Interfaces;

namespace PythonEditor.Lox.Par.Model
{
    public abstract class Expression : IEvaluable
    {
        public abstract object Accept(IEvaluableVisitor visitor);

    }
    public class UnaryExpression : Expression
    {
        public IEvaluable Eval { init; get; }
        public TokenType Operator { get; init; }
        public override object Accept(IEvaluableVisitor visitor) { return visitor.VisitUnaryExpression(this); }
    }


    public class BinaryExpression : Expression
    {
        public IEvaluable Left { init; get; }
        public TokenType Operator { init; get; }
        public IEvaluable Right { init; get; }
        public override object Accept(IEvaluableVisitor visitor) { return visitor.VisitBinaryExpression(this); }

    }



    public class IdentExpression : Expression
    {
        public string Name { set; get; }
        public IEvaluable? Value { set; get; }
        public override object Accept(IEvaluableVisitor visitor) { return visitor.VisitIdentExpression(this); }

    }


    public class LiteralExpression : Expression
    {
        public object Value { init; get; }
        
        public override object Accept(IEvaluableVisitor visitor) { return visitor.VisitLiteralExpression(this); }
    }
    //Special expressions

    public class ConvertExpression : Expression
    {
        public IEvaluable Expr { set; get; }
        public Type NewType { set; get; }

        public override object Accept(IEvaluableVisitor visitor){return visitor.VisitConvertExpression(this);}
    }

    public class InputExpression : Expression
    {
        public override object Accept(IEvaluableVisitor visitor) { return visitor.VisitInputExpression(this);}
        
    }
}
