
using System;
using System.Linq;

namespace Mince.Types
{
    public class MinceString : MinceClonable
    {
        [Exposed]
        public MinceNumber length
        {
            get { return new MinceNumber(this.value.ToString().Length); }
        }

        public MinceString(string value)
        {
            this.value = value;
            CreateMembers();
        }

        public MinceString()
        {
            this.value = "";
            CreateMembers();
        }

        public override MinceObject Plus(MinceObject other)
        {
            return new MinceString(this.value.ToString() + other.value.ToString());
        }

        public override MinceBool GreaterThan(MinceObject other)
        {
            return new MinceBool(this.value.ToString().Length > other.value.ToString().Length);
        }

        public override MinceBool LessThan(MinceObject other)
        {
            return new MinceBool(this.value.ToString().Length < other.value.ToString().Length);
        }

        [Exposed]
        public MinceChar getChar(MinceNumber index)
        {
            return new MinceChar(this.value.ToString()[index.ToInt()]);
        }

        [Exposed]
        public MinceString toUpperCase()
        {
            return new MinceString(this.value.ToString().ToUpper());
        }

        [Exposed]
        public MinceString toLowerCase()
        {
            return new MinceString(this.value.ToString().ToLower());
        }

        [Exposed]
        public MinceString trim()
        {
            return new MinceString(this.value.ToString().Trim());
        }

        [Exposed]
        public MinceString trimEnd()
        {
            return new MinceString(this.value.ToString().TrimEnd());
        }

        [Exposed]
        public MinceString trimStart()
        {
            return new MinceString(this.value.ToString().TrimStart());
        }

        [Exposed]
        public MinceString remove(MinceNumber index)
        {
            return new MinceString(this.value.ToString().Remove(index.ToInt()));
        }

        [Exposed]
        public MinceString substring(MinceNumber index)
        {
            return new MinceString(this.value.ToString().Substring(index.ToInt()));
        }

        [Exposed]
        public MinceNumber indexOf(MinceString phrase)
        {
            return new MinceNumber(this.value.ToString().IndexOf(phrase.ToString()));
        }

        [Exposed]
        public MinceNumber lastIndexOf(MinceString phrase)
        {
            return new MinceNumber(this.value.ToString().LastIndexOf(phrase.ToString()));
        }

        [Exposed]
        public MinceString replace(MinceString phrase, MinceString with)
        {
            return new MinceString(this.value.ToString().Replace(phrase.ToString(), with.ToString()));
        }

        [Exposed]
        public MinceBool endsWith(MinceString other)
        {
            return new MinceBool(this.value.ToString().EndsWith(other.ToString()));
        }

        [Exposed]
        public MinceBool startsWith(MinceString other)
        {
            return new MinceBool(this.value.ToString().StartsWith(other.ToString()));
        }

        [Exposed]
        public MinceBool contains(MinceString other)
        {
            return new MinceBool(this.value.ToString().Contains(other.ToString()));
        }

        [Exposed]
        public MinceNumber toNumber()
        {
            return new MinceNumber(float.Parse(this.value.ToString()));
        }

        [Exposed]
        public MinceBool isNumber()
        {
            float dummy;
            return new MinceBool(float.TryParse(this.value.ToString(), out dummy));
        }

        [Exposed]
        public MinceArray split(MinceString delimiter)
        {
            string[] splitted = this.value.ToString().Split(new string[] { delimiter.ToString() }, StringSplitOptions.RemoveEmptyEntries);
            return new MinceArray(splitted.Select(x => new MinceString(x)).ToArray());
        }

        [Exposed]
        public MinceArray getChars()
        {
            MinceArray a = new MinceArray(new MinceArray[] { });
            var chars = this.value.ToString().ToCharArray();
            foreach (var c in chars)
            {
                a.add(new MinceChar(c));
            }
            return a;
        }

        [Exposed]
        public override MinceObject clone()
        {
            return new MinceString(this.value.ToString());
        }
    }
}
