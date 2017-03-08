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

            var tree = Identifier.GetTree(interpreter);

            if (tree.length == 1)
            {
                interpreter.variables.variables.Remove(tree.lastVariable);
            }
            else
            {
                tree[tree.length - 2].GetValue().members.Remove(tree.lastVariable);
            }

            interpreter.Eat("SEMICOLON");

            return new MinceNull();
        }
    }
}
