using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonEditor.Lox.Exceptions
{
    public class InterpreterException : Exception
    {
        public InterpreterException() { }
        public InterpreterException(string message) : base(message) { }
        public InterpreterException(string message, Exception inner) : base(message, inner) { }
    }
}
