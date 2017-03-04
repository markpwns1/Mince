using Mince.Keywords;
using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Mince
{
    public class Interpreter
    {
        #region STATIC
        public static List<Interpreter> interpreters = new List<Interpreter>();
        public static string[] dlls;
        public static bool optionalSemicolons = true;
        public static List<Keyword> keywords = new List<Keyword>();
        public static Dictionary<string, Func<MinceObject[], MinceObject>> types = new Dictionary<string, Func<MinceObject[], MinceObject>>();

        public static Interpreter CurrentInterpreter
        {
            get { return interpreters[0]; }
        }

        public static void LoadKeywords()
        {
            Type[] classes = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "Mince.Keywords", StringComparison.Ordinal)).ToArray();
            foreach (Type c in classes)
            {
                foreach (Attribute attr in c.GetCustomAttributes(true))
                {
                    if (attr.GetType() == typeof(InterpreterKeyword) && c.BaseType == typeof(Keyword))
                    {
                        Keyword k = (Keyword)Activator.CreateInstance(c);
                        k.token = ((InterpreterKeyword)attr).token;

                        keywords.Add(k);
                        break;
                    }
                }
            }
        }

        public static void LoadLibraries()
        {
            if (!Directory.Exists("lib"))
            {
                Directory.CreateDirectory("lib");
            }

            string[] files = Directory.GetFiles("lib");

            dlls = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].EndsWith(".dll"))
                {
                    continue;
                }

                dlls[i] = AppDomain.CurrentDomain.BaseDirectory + "\\" + files[i];
            }
        }
        #endregion

        public List<Token> tokens = new List<Token>();
        public Variables variables = new Variables();
        public int pointer = 0;

        public MinceObject parent = null;

        public Token currentToken
        {
            get { return tokens[pointer]; }
        }

        public int depth = 0;

        public List<LoopPosition> loopPositions = new List<LoopPosition>();

        public Evaluation evaluation;

        public Interpreter(Evaluation eval)
        {
            this.evaluation = eval;
            this.depth = 0;
            interpreters.Insert(0, this);
        }

        public object Eat(string type)
        {
            if (currentToken.type == type)
            {
                object result = currentToken.value;

                pointer++;
                return result;
            }
            else
            {
                if (type == "SEMICOLON" && optionalSemicolons)
                {
                    return currentToken.value;
                }

                throw new Exception("Expected " + type + " but got " + currentToken + " at token #" + pointer);
            }
        }

        public void TryEat(string type)
        {
            if (currentToken.type == type)
            {
                pointer++;
            }
        }

        public object Eat()
        {
            object result = currentToken.value;
            pointer++;
            return result;
        }

        public Token LookAhead(int amount)
        {
            return tokens[pointer + amount];
        }

        public void Evaluate()
        {
            while (currentToken.type != "EOF")
            {
                EvaluateOnce();
            }
        }

        public MinceObject EvaluateOnce()
        {
            foreach (Keyword keyword in keywords)
            {
                if (currentToken.type == keyword.token)
                {
                    return keyword.Evaluate(this);
                }
            }

            if (currentToken.type == "EOF")
            {
                return new MinceNull();
            }

            throw new Exception("Unrecognized token: " + currentToken.ToString());
        }

        public MinceObject EvaluateBlock(int? startDepth = null)
        {
            startDepth = startDepth == null ? depth - 1 : startDepth;

            MinceObject result = new MinceNull();

            while (depth > startDepth)
            {
                var r = EvaluateOnce();

                if (result.GetType() == typeof(MinceNull) && r.GetType() != typeof(MinceNull))
                {
                    result = r;
                }
            }

            return result;
        }

        public List<Token> SkipBlock(int? startDepth = null)
        {
            List<Token> eatenTokens = new List<Token>();

            startDepth = startDepth == null ? depth - 1 : startDepth;

            while (depth > startDepth)
            {
                if (currentToken.type == "R_CURLY_BRACE")
                {
                    depth--;
                }

                if (currentToken.type == "L_CURLY_BRACE")
                {
                    depth++;
                }

                eatenTokens.Add(currentToken);
                Eat();
            }

            return eatenTokens;
        }

        public MinceObject[] GetParameters()
        {
            List<MinceObject> p = new List<MinceObject>();

            if (currentToken.type == "R_BRACKET")
            {
                return new MinceObject[0];
            }

            do
            {
                p.Add(evaluation.Evaluate());

                if (currentToken.type == "COMMA")
                {
                    Eat();
                }
                else
                {
                    break;
                }

            } while (true);

            return p.ToArray();
        }

        public void GoTo(int position)
        {
            pointer = position;
        }

        public void LoadAssemblies()
        {
            Type[] classes = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "Mince.Types", StringComparison.Ordinal)).ToArray();
            foreach (Type c in classes)
            {
                foreach (Attribute attr in c.GetCustomAttributes(true))
                {
                    if (attr.GetType() == typeof(StaticClass))
                    {
                        var st = (StaticClass)attr;

                        MinceObject value = (MinceObject)Activator.CreateInstance(c);

                        Variable v = new Variable(st.name, value);
                        variables.variables.Add(v);
                        break;
                    }
                    else if (attr.GetType() == typeof(Instantiatable))
                    {
                        if (c.GetConstructor(new Type[0]) == null)
                        {
                            Console.WriteLine("Could not load " + c + ". All instantiatable types must have a parameterless constructor.");
                        }
                        Func<MinceObject[], MinceObject> func = args => (MinceObject)Activator.CreateInstance(c, args, new object[0]);
                        string name = ((Instantiatable)attr).name;

                        types.Add(name, func);
                        break;
                    }
                }
            }

            foreach (string file in dlls)
            {
                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFile(file);
                }
                catch
                {
                    Console.WriteLine("Couldn't load lib/" + file.Substring(file.LastIndexOf("\\") + 1));
                    continue;
                }

                foreach (Type c in assembly.GetTypes())
                {
                    foreach (Attribute attr in c.GetCustomAttributes(true))
                    {
                        if (attr.GetType() == typeof(StaticClass))
                        {
                            var st = (StaticClass)attr;

                            MinceObject value = (MinceObject)Activator.CreateInstance(c);

                            Variable v = new Variable(st.name, value);
                            variables.variables.Add(v);
                            break;
                        }
                        else if (attr.GetType() == typeof(Instantiatable))
                        {
                            Func<MinceObject[], MinceObject> func = args => (MinceObject)Activator.CreateInstance(c, args, new object[0]);
                            string name = ((Instantiatable)attr).name;

                            types.Add(name, func);
                            break;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            interpreters.Remove(this);
        }

        public void SetParent(MinceObject parent)
        {
            this.parent = parent;
            variables.variables.Add(new Variable("this", this.parent, true, depth));
        }
    }
}