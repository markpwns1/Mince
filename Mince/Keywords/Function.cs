using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("FUNCTION")]
    public class Function : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            int initialDepth = interpreter.depth;

            interpreter.Eat();

            MinceUserFunction userFunc = new MinceUserFunction();

            List<Token> tokens = new List<Token>();

            string funcName = interpreter.Eat().ToString();

            List<string> paramNames = new List<string>();

            if (interpreter.currentToken.type == "COLON")
            {
                interpreter.Eat();

                while (interpreter.currentToken.type == "IDENTIFIER")
                {
                    string name = interpreter.Eat().ToString();

                    if (interpreter.variables.Exists(name))
                    {
                        throw new InterpreterException(interpreter.previousToken, "A variable called '" + name + "' already exists!");
                    }

                    paramNames.Add(name);

                    if (interpreter.currentToken.type == "L_CURLY_BRACE")
                    {
                        break;
                    }
                    else if (interpreter.currentToken.type == "COMMA")
                    {
                        interpreter.Eat();
                    }
                    else
                    {
                        throw new InterpreterException(interpreter.currentToken, "Expected ',' or '{' after parameter name");
                    }
                }
            }

            interpreter.Eat("L_CURLY_BRACE");
            interpreter.depth++;

            tokens.Add(new Token(0, 0, "L_CURLY_BRACE"));

            tokens = tokens.Concat(interpreter.SkipBlock()).ToList();

            userFunc.value = tokens;
            userFunc.variableNames = paramNames;

            Variable v = new Variable(funcName, userFunc, false, initialDepth);

            interpreter.variables.variables.Add(v);

            return new MinceNull();
        }
    }
}
