using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("FOR_EACH")]
    public class ForEach : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            int pos = interpreter.pointer;
            int initialDepth = interpreter.depth;

            interpreter.Eat();

            string varName = interpreter.Eat("IDENTIFIER").ToString();

            if (interpreter.variables.Exists(varName))
            {
                throw new Exception("A variable named '" + varName + "' already exists in this scope");
            }

            interpreter.Eat("COLON");

            MinceArray array = (MinceArray)interpreter.evaluation.Evaluate();

            interpreter.Eat("L_CURLY_BRACE");
            interpreter.depth++;

            if (interpreter.loopPositions.Count <= 0 || interpreter.loopPositions[0].depth != initialDepth)
            {
                interpreter.loopPositions.Insert(0, new LoopPosition(pos, initialDepth) { index = 0 });
            }

            if ((array.value as List<MinceObject>).Count == 0)
            {
                interpreter.SkipBlock();
                interpreter.loopPositions.RemoveAt(0);
                return new MinceNull();
            }

            Variable indexVar = new Variable(varName, (array.value as List<MinceObject>)[interpreter.loopPositions[0].index], true, interpreter.depth);

            interpreter.variables.variables.Add(indexVar);

            interpreter.EvaluateBlock();

            if (interpreter.loopPositions.Count < 1 || interpreter.loopPositions[0].depth != initialDepth)
            {
                interpreter.variables.variables.Remove(indexVar);
                return new MinceNull();
            }

            interpreter.loopPositions[0].index++;
            interpreter.variables.variables.Remove(indexVar);

            if (interpreter.loopPositions[0].index >= (array.value as List<MinceObject>).Count)
            {
                interpreter.loopPositions.RemoveAt(0);
            }
            else
            {
                interpreter.depth = interpreter.loopPositions[0].depth;
                interpreter.GoTo(interpreter.loopPositions[0].pointer);
            }

            return new MinceNull();
        }
    }
}
