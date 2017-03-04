using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    public class MinceClonable : MinceObject
    {
        public override MinceObject clone()
        {
            return this;
        }
    }
}
