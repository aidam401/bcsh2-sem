using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonEditor.Lox.Exceptions
{
    public class LoxException : Exception
    {
        public LoxException() { }
        public LoxException(string message) : base(message) { }
        public LoxException(string message, Exception inner) : base(message, inner) { }
    }
}
