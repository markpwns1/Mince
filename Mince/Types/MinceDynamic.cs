using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Mince.Types
{
    [Instantiatable("Dynamic")]
    public class MinceDynamic : MinceObject
    {
        private int id = 0;
        private static int idCount = 0;

        public MinceDynamic()
        {
            id = idCount;
            idCount++;
            CreateMembers();
        }

        public override string ToString()
        {
            string toReturn = "Dynamic\n{";
            foreach (var item in members)
            {
                toReturn += "\n " + item.name + " = " + item.GetValue().ToString();
            }
            toReturn += "\n}";
            return toReturn;
        }

        public override MinceBool EqualTo(MinceObject other)
        {
            return new MinceBool(this.Equals(other));
        }

        public override MinceBool NotEqualTo(MinceObject other)
        {
            return new MinceBool(!this.Equals(other));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if(obj.GetType() != typeof(MinceDynamic)) {
                return false;
            }

            return this.id == ((MinceDynamic)obj).id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

    }
}
