using Mono.Cecil;
using System;
using System.IO;
using System.Reflection;

namespace MinceCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename;
            string outputName = "program.exe";

            if (args.Length > 0)
            {
                filename = args[0];
            }
            else
            {
                Console.Write("File to compile: ");
                filename = Console.ReadLine();
            }

            if (!File.Exists(filename))
            {
                Console.WriteLine("'" + filename + "' does not exist!");
                Console.ReadKey();
                return;
            }

            var module = ModuleDefinition.ReadModule("MinceInterpreter.exe");

            Directory.CreateDirectory("Build");
            Directory.CreateDirectory("Build\\lib");

            foreach (string file in Directory.GetFiles("lib"))
            {
                File.Copy(file, "Build\\lib\\" + file.Substring(file.LastIndexOf("\\") + 1), true);
            }

            module.Resources.Add(
                new EmbeddedResource(
                    "MinceInterpreter.program.mnc",
                    ManifestResourceAttributes.Private,
                    File.ReadAllBytes(filename)));

            module.Write("Build\\" + outputName);

            File.Copy("Mince.dll", "Build\\Mince.dll", true);
            
            Console.WriteLine("Successfully compiled to 'Build\\program.exe'\nHowever the program may not execute successfully if the code is incorrect");

            if (args.Length == 0)
            {
                Console.ReadKey();
            }
        }
    }
}
