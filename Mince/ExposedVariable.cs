using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mince
{
    public class ExposedVariable : Variable
    {
        public PropertyInfo property;
        public MinceObject instance;

        public override MinceObject GetValue()
        {
            return (MinceObject)property.GetValue(instance);
        }

        public override void SetValue(MinceObject assignment)
        {
            property.SetValue(instance, assignment);
        }
    }
}
