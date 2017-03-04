using System;
using System.Collections.Generic;

using Mince.Types;

namespace Mince
{
    public class Variable
    {
        public string name;
        public int depth;

        public bool isReadOnly = false;
        public bool isPrivate = false;

        protected MinceObject value;

        public Variable() { }

        public virtual MinceObject GetValue()
        {
            return value;
        }

        public virtual void SetValue(MinceObject assignment)
        {
            value = assignment;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Variable))
            {
                return false;
            }

            Variable other = (Variable)obj;

            if (this.name != other.name)
            {
                return false;
            }

            /*if (this.depth != other.depth)
            {
                return false;
            }

            if (this.readOnly != other.readOnly)
            {
                return false;
            }

            if (this.GetValue() != other.GetValue())
            {
                return false;
            }*/

            return true;
        }

        public Variable(string n, MinceObject v = null, bool ro = false, int d = -1)
        {
            this.name = n;
            this.depth = d;
            this.value = v == null ? new MinceNull() : v;
            this.isReadOnly = ro;
        }

        public static Variable Create(string n, MinceObject v = null, bool ro = false, int d = -1)
        {
            Variable vari = new Variable();
            vari.name = n;
            vari.depth = d;
            vari.value = v == null ? new MinceNull() : v;
            vari.isReadOnly = ro;
            return vari;
        }

        public static Variable CreateFunction(string n, Func<MinceObject[], MinceObject> func)
        {
            Variable vari = new Variable();
            vari.name = n;
            vari.depth = -1;
            vari.value = new MinceFunction(func);
            return vari;
        }

        public override string ToString()
        {
            return value.GetType().Name + " " + name;
        }
    }
}
