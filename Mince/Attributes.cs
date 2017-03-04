using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class Exposed : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class Instantiatable : Attribute
    {
        public string name;

        public Instantiatable(string n)
        {
            this.name = n;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class InterpreterKeyword : Attribute
    {
        public string token;

        public InterpreterKeyword(string t)
        {
            this.token = t;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class StaticClass : Attribute
    {
        public string name;

        public StaticClass(string n)
        {
            this.name = n;
        }
    }
}
