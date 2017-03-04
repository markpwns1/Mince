using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("GLOBAL")]
    public class Global : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            interpreter.Eat();
            string name = interpreter.Eat("IDENTIFIER").ToString();
            interpreter.Eat("EQUALS");
            MinceObject value = interpreter.evaluation.Evaluate();

            Variable v = new Variable(name, value, false);

            interpreter.variables.variables.Add(v);

            interpreter.Eat("SEMICOLON");

            return new MinceNull();
        }
    }
}
