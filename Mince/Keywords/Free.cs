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
            string name = interpreter.Eat("IDENTIFIER").ToString();
            interpreter.variables.variables.Remove(interpreter.variables.variables.Find(x => x.name == name));
            interpreter.Eat("SEMICOLON");
            return new MinceNull();
        }
    }
}
