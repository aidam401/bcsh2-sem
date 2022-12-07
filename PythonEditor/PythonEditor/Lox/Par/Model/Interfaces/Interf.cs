using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonEditor.Lox.Interp.Model.Interfaces;

namespace PythonEditor.Lox.Par.Model.Interfaces
{
    public interface IEvaluable
    {
        object Accept(IEvaluableVisitor visitor);
    }
    public interface IExecutable
    {
        PossibleReturn Execute(IExecutableVisitor visitor);
    }

}
