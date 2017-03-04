using Mince.Types;

namespace Mince
{
    public class Token
    {
        public string type = null;
        public object value = null;
        public int pos = -1;

        public Token(int pos, string type = null, object value = null)
        {
            this.pos = pos;
            this.type = type;
            this.value = value;
        }

        public Token() { }

        public override string ToString()
        {
            return "Token(" + this.pos + ", " + this.type + ", " + this.value + ")";
        }
    }
}
