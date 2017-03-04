using Mince.Types;

namespace Mince.Keywords
{
    public class Keyword
    {
        public string token;

        public virtual MinceObject Evaluate(Interpreter interpreter)
        {
            return new MinceNull();
        }
    }
}
