using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    public class MinceArray : MinceObject
    {
        [Exposed]
        public MinceNumber length
        {
            get { return new MinceNumber(GetItems().Count); }
        }

        public MinceArray(MinceObject[] items)
        {
            this.value = new List<MinceObject>();
            foreach (var item in items)
            {
                if (item == this)
                {
                    throw new Exception("Cannot add an array to itself!");
                }

                GetItems().Add(item);
            }
            CreateMembers();
        }

        public MinceArray()
        {
            CreateMembers();
        }

        public MinceObject this[int index]
        {
            get
            {
                return GetItems()[index];
            }
            set
            {
                GetItems()[index] = value;
            }
        }

        public override string ToString()
        {
            return "{ " + string.Join(", ", GetItems()) + " }";
        }

        public override MinceObject Plus(MinceObject other)
        {
            var otherList = (other as MinceArray).value as List<MinceObject>;
            return new MinceArray(GetItems().Concat(otherList).ToArray());
        }

        [Exposed]
        public MinceArray concat(MinceArray other)
        {
            return this.Plus(other) as MinceArray;
        }

        [Exposed]
        public MinceBool contains(MinceObject item)
        {
            return new MinceBool(GetItems().Exists(x => x.EqualTo(item).ToBool()));
        }

        [Exposed]
        public MinceObject get(MinceNumber i)
        {
            int index = Convert.ToInt32(i.value);
            return GetItems()[index];
        }

        [Exposed]
        public MinceNull add(MinceObject arg)
        {
            if (arg == this)
            {
                throw new Exception("Cannot add an array to itself!");
            }
            GetItems().Add(arg);
            return new MinceNull();
        }

        [Exposed]
        public MinceString join(MinceString seperator)
        {
            return new MinceString(string.Join(seperator.ToString(), GetItems()));
        }

        [Exposed]
        public MinceNull remove(MinceObject arg)
        {
            GetItems().Remove(arg);
            return new MinceNull();
        }

        [Exposed]
        public MinceNumber indexOf(MinceObject arg)
        {
            return new MinceNumber(GetItems().IndexOf(arg));
        }

        [Exposed]
        public MinceNumber lastIndexOf(MinceObject arg)
        {
            return new MinceNumber(GetItems().LastIndexOf(arg));
        }

        [Exposed]
        public MinceNull removeAt(MinceNumber arg)
        {
            GetItems().RemoveAt(Convert.ToInt32(arg.value));
            return new MinceNull();
        }

        [Exposed]
        public MinceNull insert(MinceObject arg, MinceNumber index)
        {
            if (arg == this)
            {
                throw new Exception("Cannot add an array to itself!");
            }
            GetItems().Insert(Convert.ToInt32(index.value), arg);
            return new MinceNull();
        }

        [Exposed]
        public MinceNull clear()
        {
            GetItems().Clear();
            return new MinceNull();
        }

        public List<MinceObject> GetItems()
        {
            return (List<MinceObject>)this.value;
        }

        //TODO: fix stackoverflow when copying arrays
        /*public override bool Equals(object obj)
        {
            
        }*/
    }
}
