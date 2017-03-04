
using System;
using System.IO;
using System.Linq;

namespace Mince.Types
{
    [StaticClass("mince")]
    public class MinceReflection : MinceObject
    {
        public Interpreter interpreter
        {
            get { return Interpreter.CurrentInterpreter; }
        }

        public MinceReflection()
        {
            CreateMembers();
        }

        [Exposed]
        public MinceBool optionalSemicolons
        {
            get
            {
                return new MinceBool(Interpreter.optionalSemicolons);
            }

            set
            {
                Interpreter.optionalSemicolons = value.ToBool();
            }
        }

        [Exposed]
        public MinceArray variables
        {
            get
            {
                return new MinceArray(interpreter.variables.variables.Select(x => new MinceVariableInfo(x)).ToArray());
            }
        }

        [Exposed]
        public MinceBool isNull(MinceObject obj)
        {
            return new MinceBool(obj == null || obj.value == null || obj.GetType() == typeof(MinceNull));
        }

        [Exposed]
        public MinceVariableInfo getVariable(MinceString str)
        {
            return new MinceVariableInfo(str.ToString());
        }

        [Exposed]
        public MinceObject evaluate(MinceString str)
        {
            Lexer lexer = new Lexer();
            Evaluation eval = new Evaluation();

            Variables v = interpreter.variables;

            Interpreter intr = new Interpreter(eval);

            intr.tokens = lexer.ScanString(str.value.ToString());

            eval.Init(intr);

            intr.variables = v;

            MinceObject result = eval.Evaluate();

            intr.Dispose();

            return result;
        }

        [Exposed]
        public MinceObject execute(MinceString str)
        {
            Lexer lexer = new Lexer();

            int before = interpreter.pointer;
            int beforeDepth = interpreter.depth;
            
            interpreter.tokens.RemoveAt(interpreter.tokens.Count - 1); // remove EOF;

            int to = interpreter.tokens.Count;

            interpreter.tokens.AddRange(lexer.ScanString(str.value.ToString()));

            interpreter.GoTo(to);
            interpreter.depth = 0;

            MinceObject returnValue = new MinceNull();

            while (interpreter.currentToken.type != "EOF" && returnValue.GetType() == typeof(MinceNull))
            {
                returnValue = interpreter.EvaluateOnce();
            }

            interpreter.GoTo(before);
            interpreter.depth = beforeDepth;

            return returnValue;
        }

        [Exposed]
        public MinceObject executeOnce(MinceString str)
        {
            Lexer lexer = new Lexer();

            int before = interpreter.pointer;
            int beforeDepth = interpreter.depth;

            interpreter.tokens.RemoveAt(interpreter.tokens.Count - 1); // remove EOF;

            int to = interpreter.tokens.Count;

            interpreter.tokens.AddRange(lexer.ScanString(str.value.ToString()));

            interpreter.GoTo(to);
            interpreter.depth = 0;

            var result = interpreter.EvaluateOnce();

            interpreter.GoTo(before);
            interpreter.depth = beforeDepth;

            return result;
        }

        [Exposed]
        public MinceObject executeFile(MinceString path)
        {
            Lexer lexer = new Lexer();

            int before = interpreter.pointer;
            int beforeDepth = interpreter.depth;

            interpreter.tokens.RemoveAt(interpreter.tokens.Count - 1); // remove EOF;

            int to = interpreter.tokens.Count;

            interpreter.tokens.AddRange(lexer.ScanFile(path.value.ToString()));

            interpreter.GoTo(to);
            interpreter.depth = 0;

            MinceObject returnValue = new MinceNull();

            while (interpreter.currentToken.type != "EOF" && returnValue.GetType() == typeof(MinceNull))
            {
                returnValue = interpreter.EvaluateOnce();
            }

            interpreter.GoTo(before);
            interpreter.depth = beforeDepth;

            return returnValue;
        }
    }
}
