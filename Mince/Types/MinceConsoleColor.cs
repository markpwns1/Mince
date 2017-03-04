using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Types
{
    [Instantiatable("ConsoleColor")]
    public class MinceConsoleColor : MinceObject
    {
        public MinceConsoleColor(MinceString color)
        {
            CreateMembers();

            ConsoleColor dummy;

            if (!Enum.TryParse<ConsoleColor>(color.ToString(), true, out dummy))
            {
                throw new Exception("The console color '" + color.ToString() + "' is not available");
            }

            this.value = dummy;
        }

        public MinceConsoleColor(ConsoleColor color)
        {
            CreateMembers();
            this.value = color;
        }

        public MinceConsoleColor() { }

        public ConsoleColor GetConsoleColor()
        {
            return (ConsoleColor)this.value;
        }
    }
}
