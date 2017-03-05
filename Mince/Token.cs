using Mince.Types;

namespace Mince
{
    public class Token
    {
        public string type = null;
        public object value = null;

        public int line = 0;
        public int column = 0;

        public Token(int line, int column, string type = null, object value = null)
        {
            this.line = line;
            this.column = column;
            this.type = type;
            this.value = value;
        }

        public Token() { }

        public override string ToString()
        {
            return "Token(" + this.line + ", " + this.column + ", " + this.type + ", " + this.value + ")";
        }
    }
}
