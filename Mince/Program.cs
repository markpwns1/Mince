using System;
using System.IO;
using Mince.Types;

namespace Mince
{
    /*class Program
    {
        public static Lexer lexer;
        public static Interpreter interpreter;
        public static Evaluation eval;

        public static void Main(string[] args)
        {
            string filename = args.Length > 0 ? args[0] : "program.mnc";

            lexer = new Lexer();
            eval = new Evaluation();
            interpreter = new Interpreter(eval);
            interpreter.LoadAssemblies();

            eval.Init(interpreter);

            try
            {
                interpreter.tokens = lexer.ScanFile(filename);
            }
            catch (Exception e)
            {
                Console.WriteLine("SCANNING EXCEPTION: " + e.ToString());
                Console.ReadKey();
                return;
            }

            interpreter.Evaluate();

            /*try
            {
                interpreter.Evaluate();
            }
            catch (Exception e)
            {
                Console.WriteLine("RUNTIME EXCEPTION: " + e.ToString());
                Console.ReadKey();
                return;
            }
        }
    }*/
}
