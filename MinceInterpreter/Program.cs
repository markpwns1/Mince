using Mince;
using Mince.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
namespace MinceInterpreter
{
    class Program
    {
        public static Interpreter interpreter;
        public static Evaluation eval;

        public static void Main(string[] args)
        {
            List<Token> tokens;
            eval = new Evaluation();
            interpreter = new Interpreter(eval);

            Interpreter.LoadKeywords();
            Interpreter.LoadLibraries();

            Assembly assem = Assembly.GetExecutingAssembly();
            bool hasEmbeddedFile = assem.GetManifestResourceNames().Contains("MinceInterpreter.program.mnc");

            if (hasEmbeddedFile)
            {
                string text;
                
                using (Stream stream = assem.GetManifestResourceStream("MinceInterpreter.program.mnc"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        text = reader.ReadToEnd();
                    }
                }

                tokens = Lexer.ScanString(text);

            }
            else if (args.Length > 0)
            {
                tokens = Lexer.ScanFile(args[0]);
            }
            else
            {
                string text;

                using (Stream stream = assem.GetManifestResourceStream("MinceInterpreter.backup.mnc"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        text = reader.ReadToEnd();
                    }
                }

                tokens = Lexer.ScanString(text);
            }

            interpreter.variables.variables.Add(new Variable("args", new MinceArray(args.Select(x => new MinceString(x)).ToArray())));

            interpreter.LoadAssemblies();

            eval.Init(interpreter);

            try
            {
                interpreter.tokens = tokens;
            }
            catch (Exception e)
            {
                Console.WriteLine("SCANNING EXCEPTION: " + e.ToString());
                Console.ReadKey();

                interpreter.Dispose();
                return;
            }

            try
            {
                interpreter.Evaluate();
            }
            catch (Exception e)
            {
                Console.WriteLine("RUNTIME EXCEPTION: " + e.ToString());
                Console.ReadKey();

                interpreter.Dispose();
                return;
            }
        }
    }
}
