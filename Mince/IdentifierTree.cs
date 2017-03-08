using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince
{
    public class VariableTree
    {
        private List<Variable> tree = new List<Variable>();

        public int length
        {
            get { return tree.Count; }
        }

        public Variable lastVariable
        {
            get
            {
                return tree[length - 1];
            }

            set
            {
                tree[length - 1] = value;
            }
        }

        public Variable this[int index]
        {
            get
            {
                return tree[index];
            }

            set
            {
                tree[index] = value;
            }
        }

        public void Add(Variable v)
        {
            tree.Add(v);
        }
    }
}
