using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("WHILE")]
    public class While : Keyword
    {
        public virtual bool ShouldRun(Interpreter interpreter)
        {
            return (bool)((MinceBool)interpreter.evaluation.Evaluate()).value;
        }

        public override MinceObject Evaluate(Interpreter interpreter)
        {
            int pos = interpreter.pointer;
            int initialDepth = interpreter.depth;

            interpreter.Eat();

            bool shouldRun = ShouldRun(interpreter);

            interpreter.Eat("L_CURLY_BRACE");
            interpreter.depth++;

            if (shouldRun)
            {
                interpreter.loopPositions.Insert(0, new LoopPosition(pos, initialDepth));
                interpreter.EvaluateBlock();

                if (interpreter.loopPositions.Count > 0)
                {
                    interpreter.GoTo(interpreter.loopPositions[0].pointer);
                    interpreter.depth = interpreter.loopPositions[0].depth;

                    interpreter.loopPositions.RemoveAt(0);
                }
            }
            else
            {
                interpreter.SkipBlock();
            }

            return new MinceNull();
        }
    }

    [InterpreterKeyword("UNTIL")]
    public class Until : While
    {
        public override bool ShouldRun(Interpreter interpreter)
        {
            return !base.ShouldRun(interpreter);
        }
    }
}
