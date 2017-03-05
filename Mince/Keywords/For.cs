using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("FOR")]
    public class For : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            int startPos = interpreter.pointer;

            interpreter.Eat();

            var varToken = interpreter.currentToken;

            string varName = interpreter.Eat("IDENTIFIER").ToString();
            interpreter.Eat("EQUALS");
            MinceNumber varValue = (MinceNumber)interpreter.evaluation.Evaluate();

            interpreter.Eat("COLON");

            if (interpreter.loopPositions.Count <= 0 || interpreter.loopPositions[0].depth != interpreter.depth)
            {
                // New for loop is created

                Variable indexVar = new Variable(varName, varValue, false, interpreter.depth);

                if (interpreter.variables.Exists(varName))
                {
                    throw new InterpreterException(varToken, "Cannot use variable '" + varName + "' in a for loop because it already exists!");
                }

                interpreter.variables.variables.Add(indexVar);

                interpreter.loopPositions.Insert(0, new LoopPosition(startPos, interpreter.depth) { index = 0 });
            }

            bool shouldRun = (bool)(interpreter.evaluation.Evaluate() as MinceBool).value;

            interpreter.Eat("COLON");

            int incrementPos = interpreter.pointer;

            while (interpreter.currentToken.type != "L_CURLY_BRACE")
            {
                interpreter.Eat();
            }

            interpreter.Eat("L_CURLY_BRACE");
            interpreter.depth++;

            if (!shouldRun)
            {
                interpreter.SkipBlock();
                interpreter.variables.variables.Remove(interpreter.variables.variables.Find(x => x.name == varName));
                interpreter.loopPositions.RemoveAt(0);
                return new MinceNull();
            }

            interpreter.EvaluateBlock();

            if (interpreter.loopPositions.Count < 1 || interpreter.loopPositions[0].depth != interpreter.depth)
            {
                interpreter.variables.variables.Remove(interpreter.variables.variables.Find(x => x.name == varName));
                if (interpreter.loopPositions.Count > 0)
                {
                    interpreter.loopPositions.RemoveAt(0);
                }
                return new MinceNull();
            }

            interpreter.GoTo(incrementPos);

            interpreter.EvaluateOnce();

            interpreter.GoTo(startPos);
            interpreter.depth = interpreter.loopPositions[0].depth;

            return new MinceNull();
        }
    }
}
