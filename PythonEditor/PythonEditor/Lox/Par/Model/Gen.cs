using System;
using System.Collections.Generic;
using PythonEditor.Lox.Interp.Model.Interfaces;
using PythonEditor.Lox.Lex;
using PythonEditor.Lox.Par.Model.Interfaces;

namespace PythonEditor.Lox.Par.Model
{
    public class Program: IExecutable
    {
        public Block Block { init; get; }

        public PossibleReturn Execute(IExecutableVisitor visitor) { return visitor.VisitProgram(this); }
    }

    public class TypedIdent
    {
        public string Name { set; get; }
        public Type Type { set; get; }
    }


    public class Block : IExecutable
    {
        public List<Function> Functions { set; get; } = new List<Function>();
        public List<IExecutable> Statements { set; get; } = new List<IExecutable>();
        public PossibleReturn Execute(IExecutableVisitor visitor) { return visitor.VisitBlock(this); }
        
    }

}
