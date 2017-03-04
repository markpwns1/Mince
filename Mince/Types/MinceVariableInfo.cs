
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    public class MinceVariableInfo : MinceObject
    {
        public MinceVariableInfo(string varName)
        {
            this.value = varName;
            CreateMembers();
        }

        public MinceVariableInfo(Variable v)
        {
            this.value = v;
            CreateMembers();
        }

        [Exposed]
        public MinceBool exists()
        {
            return new MinceBool(Interpreter.CurrentInterpreter.variables.Exists(this.value.ToString()));
        }

        [Exposed]
        public MinceObject data
        {
            get { return GetVariable().GetValue(); }
            set { GetVariable().SetValue(value); }
        }

        [Exposed]
        public MinceNull delete()
        {
            Interpreter.CurrentInterpreter.variables.variables.Remove(GetVariable());
            this.data = null;
            return new MinceNull();
        }

        public Variable GetVariable()
        {
            return Interpreter.CurrentInterpreter.variables.Get(this.value.ToString());
        }
    }
}
