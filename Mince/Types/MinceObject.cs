
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Mince.Types
{
    public class MinceObject 
    {
        // FIX: MinceArray.List<MinceObject> is the same reference after cloning

        public object value = null;

        public List<Variable> members = new List<Variable>();

        public bool MemberExists(string member)
        {
            return members.Exists(x => x.name == member);
        }

        public Variable GetMember(string member)
        {
            return members.Find(x => x.name == member);
        }

        public virtual void CreateMembers()
        {
            // TODO: different parameters

            foreach (MethodInfo method in GetType().GetMethods())
            {
                foreach (Attribute attr in method.GetCustomAttributes(true))
                {
                    if (attr.GetType() == typeof(Exposed))
                    {
                        Variable v = new Variable(method.Name, new MinceFunction(args => (MinceObject)method.Invoke(this, args)), true, -1);
                        members.Add(v);
                        break;
                    }
                }
            }

            foreach (PropertyInfo property in GetType().GetProperties())
            {
                foreach (Attribute attr in property.GetCustomAttributes(true))
                {
                    if (attr.GetType() == typeof(Exposed))
                    {
                        ExposedVariable v = new ExposedVariable();
                        v.instance = this;
                        v.property = property;

                        v.name = property.Name;
                        v.isReadOnly = !property.CanWrite;
                        v.depth = -1;

                        members.Add(v);
                    }
                }
            }
        }

        public virtual MinceObject Plus(MinceObject other) { throw new Exception("Cannot use '+' on " + this.GetType().ToString()); }
        public virtual MinceObject Minus(MinceObject other) { throw new Exception("Cannot use '-' on " + this.GetType().ToString()); }

        public virtual MinceObject Multiply(MinceObject other) { throw new Exception("Cannot use '*' on " + this.GetType().ToString()); }
        public virtual MinceObject Divide(MinceObject other) { throw new Exception("Cannot use '/' on " + this.GetType().ToString()); }

        public virtual MinceObject Exponent(MinceObject other) { throw new Exception("Cannot use '^' on " + this.GetType().ToString()); }

        public virtual MinceBool EqualTo(MinceObject other)
        {
            return new MinceBool(this.value.Equals(other.value));
        }

        public virtual MinceBool NotEqualTo(MinceObject other)
        {
            return new MinceBool(!this.value.Equals(other.value));
        }

        public virtual MinceBool GreaterThan(MinceObject other) { throw new Exception("Cannot use '>' on " + this.GetType().ToString()); }
        public virtual MinceBool LessThan(MinceObject other) { throw new Exception("Cannot use '<' on " + this.GetType().ToString()); }

        public virtual MinceBool GreaterOrEqual(MinceObject other)
        {
            return new MinceBool((bool)this.GreaterThan(other).value || (bool)this.EqualTo(other).value);
        }

        public virtual MinceBool LessOrEqual(MinceObject other)
        {
            return new MinceBool((bool)this.LessThan(other).value || (bool)this.EqualTo(other).value);
        }

        public virtual MinceBool And(MinceBool other)
        {
            if (this.GetType() != typeof(MinceBool))
            {
                throw new Exception("Cannot use 'and' on " + this.GetType().ToString());
            }

            return new MinceBool((bool)this.value && (bool)other.value);
        }

        public virtual MinceBool Or(MinceBool other)
        {
            if (this.GetType() != typeof(MinceBool))
            {
                throw new Exception("Cannot use 'or' on " + this.GetType().ToString());
            }

            return new MinceBool((bool)this.value || (bool)other.value);
        }

        public MinceObject(object value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value == null ? this.GetType().Name : value.ToString();
        }

        public static bool operator ==(MinceObject obj1, MinceObject obj2)
        {
            if (object.ReferenceEquals(obj1, null) || object.ReferenceEquals(obj2, null))
            {
                return object.ReferenceEquals(obj1, null) && object.ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(MinceObject obj1, MinceObject obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            if (((MinceObject)obj).value == this.value)
            {
                return false;
            }

            return true;
        }

        /*public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }*/

        [Exposed]
        public MinceString toString()
        {
            return new MinceString(this.ToString());
        }

        [Exposed]
        public MinceString getType()
        {
            return new MinceString(this.GetType().Name);
        }

        [Exposed]
        public MinceBool hasMember(MinceString name)
        {
            return new MinceBool(MemberExists(name.ToString()));
        }

        [Exposed]
        public virtual MinceObject clone()
        {
            return Cloner.Clone(this);
        }

        [Exposed]
        public MinceBool referenceEquals(MinceObject other)
        {
            return new MinceBool(object.ReferenceEquals(this, other));
        }

        public MinceObject() { }
    }

    public class MinceNull : MinceObject
    {
        public MinceNull()
        {
            CreateMembers();
        }

        public override MinceBool EqualTo(MinceObject other)
        {
            return new MinceBool(other.GetType() == typeof(MinceNull));
        }

        public override MinceBool NotEqualTo(MinceObject other)
        {
            return new MinceBool(other.GetType() != typeof(MinceNull));
        }

        public override bool Equals(object obj)
        {
            return obj == null || obj.GetType() == typeof(MinceNull);
        }

        public override string ToString()
        {
            return "null";
        }
    }
}
