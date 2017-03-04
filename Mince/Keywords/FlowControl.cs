using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("RETURN")]
    public class Return : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            interpreter.Eat();
            MinceObject returnValue = interpreter.evaluation.Evaluate();
            interpreter.Eat("SEMICOLON");
            return returnValue;
        }
    }

    [InterpreterKeyword("BREAK")]
    public class Break : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            if (interpreter.loopPositions.Count > 0)
            {
                interpreter.SkipBlock(interpreter.loopPositions[0].depth);
                interpreter.loopPositions.RemoveAt(0);
            }
            else
            {
                interpreter.Eat();
                interpreter.Eat("SEMICOLON");
            }
            return new MinceNull();
        }
    }

    [InterpreterKeyword("CONTINUE")]
    public class Continue : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            if (interpreter.loopPositions.Count > 0)
            {
                interpreter.GoTo(interpreter.loopPositions[0].pointer);
                interpreter.depth = interpreter.loopPositions[0].depth;
                if (interpreter.loopPositions[0].index == -1)
                {
                    interpreter.loopPositions.RemoveAt(0);
                }
            }
            else
            {
                interpreter.Eat();
                interpreter.Eat("SEMICOLON");
            }
            return new MinceNull();
        }
    }
}
