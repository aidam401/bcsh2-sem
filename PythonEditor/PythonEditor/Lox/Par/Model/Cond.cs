using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonEditor.Lox.Interp.Model.Interfaces;
using PythonEditor.Lox.Lex;
using PythonEditor.Lox.Par.Model.Interfaces;

namespace PythonEditor.Lox.Par.Model
{
    public abstract class Condition : IEvaluable
    {
        public abstract object Accept(IEvaluableVisitor visitor);

    }


    public class BinaryCondition :Condition
    {
        public IEvaluable Left { init; get; }
        public TokenType Operator {init; get;}

        public IEvaluable Right { init; get; }

        public override object Accept(IEvaluableVisitor visitor) { return visitor.VisitBinaryCondition(this); }
    }



    public class NotCondition : Condition
    {
        public IEvaluable Condition { init; get; }

        public override object Accept(IEvaluableVisitor visitor){return visitor.VisitNotCondition(this);}
    }

    

}
