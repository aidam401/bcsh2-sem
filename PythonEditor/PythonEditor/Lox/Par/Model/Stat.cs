using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonEditor.Lox.Interp.Model.Interfaces;
using PythonEditor.Lox.Par.Model.Interfaces;

namespace PythonEditor.Lox.Par.Model
{
    public abstract class Statement : IExecutable
    {
        public abstract PossibleReturn Execute(IExecutableVisitor visitor);

    }
    public class SetStatement : Statement
    {
        public TypedIdent Ident { set; get; }
        public IEvaluable Expr { set; get; }

        public override PossibleReturn Execute(IExecutableVisitor visitor) { return visitor.VisitSetStatement(this);}

    }

    public class IfStatement : Statement
    {
        public Condition Condition { set; get; }
        public List<IExecutable> TrueStatements { set; get; } = new List<IExecutable>();
        public List<IExecutable> ElseStatements { set; get; } = new List<IExecutable>();
        public override PossibleReturn Execute(IExecutableVisitor visitor) { return visitor.VisitIfStatement(this);}
    }

    public class WhileStatement : Statement
    {
        public Condition Condition { set; get; }
        public List<IExecutable> Statements { set; get; } = new List<IExecutable>();

        public override PossibleReturn Execute(IExecutableVisitor visitor) {return visitor.VisitWhileStatement(this); }
    }
    public class ForStatement : Statement
    {
        public string Iterator { set; get; }
        public int Min { set; get; } = 0;
        public int Step { set; get; } = 1;
        public int Max { set; get; }
        public List<IExecutable> Statements { init; get; } = new List<IExecutable>();
        public override PossibleReturn Execute(IExecutableVisitor visitor) { return visitor.VisitForStatement(this); }

    }

    public class ReturnStatement : Statement
    {
        public IEvaluable Expr { get; init; }
        public override PossibleReturn Execute(IExecutableVisitor visitor){return visitor.VisitReturnStatement(this);}
    }
    
    //SPECIAL STATEMENTS
    public class PrintStatement : Statement
    {
        public IEvaluable Evaluable { set; get; }
        public override PossibleReturn Execute(IExecutableVisitor visitor) { return visitor.VisitPrintStatement(this); }
        
    }






}
