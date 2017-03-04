using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("TRY")]
    public class Try : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            int initialDepth = interpreter.depth;

            interpreter.Eat();

            interpreter.Eat("L_CURLY_BRACE");
            interpreter.depth++;

            bool wasError = false;

            try
            {
                interpreter.EvaluateBlock(initialDepth);
            }
            catch
            {
                wasError = true;
            }

            if (wasError)
            {
                interpreter.SkipBlock(initialDepth);

                interpreter.Eat("CATCH");

                interpreter.Eat("L_CURLY_BRACE");
                interpreter.depth++;

                interpreter.EvaluateBlock(initialDepth);
            }
            else
            {
                interpreter.Eat("CATCH");

                interpreter.Eat("L_CURLY_BRACE");
                interpreter.depth++;

                interpreter.SkipBlock(initialDepth);
            }


            return new MinceNull();
        }
    }
}
