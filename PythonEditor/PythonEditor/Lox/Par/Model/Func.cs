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
    public class Arg
    {
        public Type Type { set; get; }
        public string Name { set; get; }

        public override bool Equals(object? obj)
        {
            return obj is Arg arg &&
                   Type == arg.Type &&
                   Name == arg.Name;
        }
    }

    public class Function : IEvaluable
    {
        public string Name { get; set; }
        public List<Arg> Args { set; get; } = new List<Arg>();
        public List<IExecutable> Statements { set; get; } = new List<IExecutable>();
        
        public Type? ReturnType { set; get; }

        public object Accept(IEvaluableVisitor visitor)
        {
            visitor.VisitFunction(this);
            return new object();
        }

    }

    public class FunctionCall : IExecutable, IEvaluable
    {
        public string Name { set; get; }
        public List<IEvaluable> ArgEvals { set; get; } = new List<IEvaluable>();

        public object Accept(IEvaluableVisitor visitor){return visitor.VisitAcceptFunctionCall(this);}
        public PossibleReturn Execute(IExecutableVisitor visitor) { return visitor.VisitExecFunctionCall(this); }

    }
}
