
using System;
using System.Windows.Forms;

namespace Mince.Types
{
    [StaticClass("console")]
    public class MinceConsole : MinceObject
    {
        [Exposed]
        public MinceString title
        {
            get { return new MinceString(Console.Title); }
            set { Console.Title = value.ToString(); }
        }

        [Exposed]
        public MinceNumber cursorX
        {
            get { return new MinceNumber(Console.CursorLeft); }
            set { Console.CursorLeft = value.ToInt(); }
        }

        [Exposed]
        public MinceNumber cursorY
        {
            get { return new MinceNumber(Console.CursorTop); }
            set { Console.CursorTop = value.ToInt(); }
        }

        [Exposed]
        public MinceConsoleColor foregroundColor
        {
            get { return new MinceConsoleColor(Console.ForegroundColor); }
            set { Console.ForegroundColor = value.GetConsoleColor(); }
        }

        [Exposed]
        public MinceConsoleColor backgroundColor
        {
            get { return new MinceConsoleColor(Console.BackgroundColor); }
            set { Console.BackgroundColor = value.GetConsoleColor(); }
        }

        public MinceConsole()
        {
            CreateMembers();
        }

        [Exposed]
        public MinceNull beep(MinceNumber frequency, MinceNumber duration)
        {
            Console.Beep(frequency.ToInt(), duration.ToInt());
            return new MinceNull();
        }

        [Exposed]
        public MinceChar readKey()
        {
            return new MinceChar(Console.ReadKey().KeyChar);
        }

        [Exposed]
        public MinceString readLine()
        {
            return new MinceString(Console.ReadLine());
        }

        [Exposed]
        public MinceNull clear()
        {
            Console.Clear();
            return new MinceNull();
        }

        [Exposed]
        public MinceNull print(MinceObject obj)
        {
            Console.WriteLine(obj.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull write(MinceObject obj)
        {
            Console.Write(obj.ToString());
            return new MinceNull();
        }

        [Exposed]
        public MinceNull exit()
        {
            Environment.Exit(0);
            return new MinceNull();
        }

        [Exposed]
        public MinceNull alert(MinceObject obj)
        {
            MessageBox.Show(obj.ToString());
            return new MinceNull();
        }
    }
}
