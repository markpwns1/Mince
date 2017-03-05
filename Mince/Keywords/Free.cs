using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mince.Types;

namespace Mince.Keywords
{
    [InterpreterKeyword("FREE")]
    public class Free : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            interpreter.Eat();

            Variable v = Identifier.GetIdentifier(interpreter);

            v = null;
            interpreter.Eat("SEMICOLON");

            return new MinceNull();
        }
    }
}
