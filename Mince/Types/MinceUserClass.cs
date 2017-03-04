using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    public class MinceUserClass : MinceClonable
    {
        public List<Variable> userMembers = new List<Variable>();

        public override MinceObject clone()
        {
            MinceObject copy = new MinceObject();
            copy.CreateMembers();

            foreach (var member in userMembers)
            {
                copy.members.Add(Cloner.Clone(member));
            }

            return copy;
        }
    }
}
