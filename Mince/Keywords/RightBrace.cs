using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("R_CURLY_BRACE")]
    public class RightBrace : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            interpreter.Eat();
            interpreter.variables.DeleteAllAtDepth(interpreter.depth);
            interpreter.depth--;
            return new MinceNull();
        }
    }
}
