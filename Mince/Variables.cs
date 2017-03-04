using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince
{
    public class Variables
    {
        public List<Variable> variables = new List<Variable>();

        public bool Exists(string name)
        {
            return variables.Exists(x => x.name == name);
        }

        public Variable Get(string name)
        {
            if (Exists(name))
            {
                return variables.Find(x => x.name == name);
            }
            else
            {
                throw new Exception("Variable " + name + " does not exist in this scope, or may not exist at all");
            }
        }

        public List<Variable> GetAllAtDepth(int depth)
        {
            return variables.FindAll(x => x.depth == depth).ToList();
        }

        public void DeleteAllAtDepth(int depth)
        {
            foreach (var item in GetAllAtDepth(depth))
            {
                variables.Remove(item);
            }
        }
    }
}
