
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
            Evaluation eval = new Evaluation();

            Variables v = interpreter.variables;

            Interpreter intr = new Interpreter(eval);

            intr.tokens = Lexer.ScanString(str.ToString());

            eval.Init(intr);

            intr.variables = v;

            MinceObject result = eval.Evaluate();

            intr.Dispose();

            return result;
        }

        [Exposed]
        public MinceObject execute(MinceString str)
        {
            Evaluation eval = new Evaluation();

            Variables v = interpreter.variables;

            Interpreter intr = new Interpreter(eval);

            intr.tokens.Add(new Token(1, 0, "L_CURLY_BRACE"));
            intr.tokens.AddRange(Lexer.ScanString(str.ToString()));
            intr.tokens.Add(new Token(1, 0, "R_CURLY_BRACE"));

            eval.Init(intr);

            intr.variables = v;

            MinceObject result = intr.EvaluateBlock();

            intr.Dispose();

            return result;
        }

        [Exposed]
        public MinceObject executeOnce(MinceString str)
        {
            Evaluation eval = new Evaluation();

            Variables v = interpreter.variables;

            Interpreter intr = new Interpreter(eval);

            intr.tokens.AddRange(Lexer.ScanString(str.ToString()));

            eval.Init(intr);

            intr.variables = v;

            MinceObject result = intr.EvaluateOnce();

            intr.Dispose();

            return result;
        }

        [Exposed]
        public MinceObject executeFile(MinceString path)
        {
            Evaluation eval = new Evaluation();

            Variables v = interpreter.variables;

            Interpreter intr = new Interpreter(eval);

            intr.tokens.Add(new Token(1, 0, "L_CURLY_BRACE"));
            intr.tokens.AddRange(Lexer.ScanFile(path.ToString()));
            intr.tokens.Add(new Token(1, 0, "R_CURLY_BRACE"));

            eval.Init(intr);

            intr.variables = v;

            MinceObject result = intr.EvaluateBlock();

            intr.Dispose();

            return result;
        }
    }
}
