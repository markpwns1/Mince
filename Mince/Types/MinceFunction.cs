using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    public class MinceFunction : MinceObject
    {
        public MinceFunction(Func<MinceObject[], MinceObject> value)
        {
            this.value = value;
        }

        public MinceFunction() { }

        public MinceObject Call(MinceObject[] args)
        {
            return ((Func<MinceObject[], MinceObject>)value).Invoke(args);
        }

        public override string ToString()
        {
            return "function (" + string.Join(", ", ((Func<MinceObject[], MinceObject>)value).Method.GetGenericArguments().Select(x => x.Name)) + ")";
        }
    }
}
