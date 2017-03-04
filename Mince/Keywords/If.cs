using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("IF")]
    public class If : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            interpreter.Eat();

            bool shouldRun = (bool)((MinceBool)interpreter.evaluation.Evaluate()).value;
            bool hasRan = false;

            interpreter.Eat("L_CURLY_BRACE");
            interpreter.depth++;

            if (shouldRun)
            {
                interpreter.EvaluateBlock();
                hasRan = true;
            }
            else
            {
                interpreter.SkipBlock();
            }

            while (interpreter.currentToken.type == "ELSE")
            {
                interpreter.Eat();
                if (hasRan)
                {
                    if (interpreter.currentToken.type != "L_CURLY_BRACE" && interpreter.currentToken.type != "IF")
                    {
                        throw new Exception("Expected 'if' or '{' after 'else'");
                    }

                    while (interpreter.currentToken.type != "L_CURLY_BRACE")
                    {
                        interpreter.Eat();
                    }

                    interpreter.Eat();
                    interpreter.depth++;

                    interpreter.SkipBlock();
                }
                else
                {
                    interpreter.EvaluateOnce();
                    hasRan = true;
                }
            }


            return new MinceNull();
        }
    }
}
