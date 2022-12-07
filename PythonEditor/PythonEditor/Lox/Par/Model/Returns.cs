using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonEditor.Lox.Par.Model
{
    public class PossibleReturn
    {
        public bool IsVoid { get; set; } = false;
        public object? Value { get; set; }

    }


}
