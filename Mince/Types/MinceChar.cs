
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    public class MinceChar : MinceClonable
    {
        public MinceChar(char c)
        {
            this.value = c;
            CreateMembers();
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

        [Exposed]
        public MinceBool isDigit()
        {
            return new MinceBool(char.IsDigit((char)this.value));
        }

        [Exposed]
        public MinceBool isLetter()
        {
            return new MinceBool(char.IsLetter((char)this.value));
        }

        [Exposed]
        public MinceBool isSymbol()
        {
            return new MinceBool(char.IsSymbol((char)this.value));
        }

        [Exposed]
        public MinceBool isUpperCase()
        {
            return new MinceBool(char.IsUpper((char)this.value));
        }

        [Exposed]
        public MinceBool isLowerCase()
        {
            return new MinceBool(char.IsLower((char)this.value));
        }

        [Exposed]
        public MinceChar toUpperCase()
        {
            return new MinceChar(char.ToUpper(GetChar()));
        }

        [Exposed]
        public MinceChar toLowerCase()
        {
            return new MinceChar(char.ToLower(GetChar()));
        }

        [Exposed]
        public override MinceObject clone()
        {
            return new MinceChar((char)this.value);
        }

        public override MinceObject Plus(MinceObject other)
        {
            return new MinceString(this.ToString() + other.ToString());
        }

        public char GetChar()
        {
            return (char)this.value;
        }
    }
}
