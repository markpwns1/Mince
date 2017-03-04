using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    public class MinceUserFunction : MinceObject
    {
        public MinceObject parent = null;
        public List<string> variableNames = new List<string>();

        public MinceUserFunction()
        {
            this.value = new List<Token>();
            CreateMembers();
        }

        public MinceObject call(MinceObject[] args)
        {
            Evaluation e = new Evaluation();

            var beforeVars = Interpreter.CurrentInterpreter.variables.variables;

            Interpreter i = new Interpreter(e);

            e.Init(i);

            i.tokens = this.GetTokens();

            foreach (var v in beforeVars)
            {
                i.variables.variables.Add(v);
            }

            if (args.Length != variableNames.Count)
            {
                throw new Exception("Incorrect number of parameters! Expected " + variableNames.Count + " parameters but got " + args.Length);
            }

            i.Eat("L_CURLY_BRACE");
            i.depth++;

            for (int index = 0; index < args.Length; index++)
            {
                i.variables.variables.Add(new Variable(variableNames[index], args[index], false, i.depth));
            }

            if(parent != null) {
                i.SetParent(parent);
            }

            MinceObject returned = i.EvaluateBlock();

            i.Dispose();

            return returned;
        }

        public List<Token> GetTokens()
        {
            return this.value as List<Token>;
        }

        public override string ToString()
        {
            return "function (" + string.Join(", ", variableNames.ToArray()) + ")";
        }
    }
}
