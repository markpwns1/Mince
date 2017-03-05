using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mince.Types;

namespace Mince.Keywords
{
    [InterpreterKeyword("CLASS")]
    public class Class : Keyword
    {
        public override MinceObject Evaluate(Interpreter interpreter)
        {
            interpreter.Eat();

            MinceUserClass obj = new MinceUserClass();
            obj.CreateMembers();

            string className = interpreter.Eat("IDENTIFIER").ToString();

            interpreter.Eat("L_CURLY_BRACE");

            while (new string[] { "METHOD", "FIELD", "PROPERTY", "PUBLIC", "PRIVATE" }.Contains(interpreter.currentToken.type))
            {
                bool isPrivate = true;

                if (interpreter.currentToken.type == "PUBLIC")
                {
                    interpreter.Eat();
                    isPrivate = false;
                }
                else if (interpreter.currentToken.type == "PRIVATE")
                {
                    interpreter.Eat();
                    isPrivate = true;
                }

                Variable member;

                if (interpreter.currentToken.type == "METHOD")
                {
                    member = GetMethod(interpreter, isPrivate);
                }
                else if(interpreter.currentToken.type == "FIELD")
                {
                    member = GetField(interpreter, isPrivate);
                }
                else if (interpreter.currentToken.type == "PROPERTY")
                {
                    member = GetProperty(interpreter, isPrivate);
                }
                else
                {
                    throw new InterpreterException(interpreter.currentToken, "Expected 'method', 'field', or 'property' in the class");
                }

                obj.userMembers.Add(member);
            }

            interpreter.Eat("R_CURLY_BRACE");

            Interpreter.types.Add(className, args => NewClass(obj, args));

            return new MinceNull();
        }

        public Variable GetProperty(Interpreter interpreter, bool isPrivate)
        {
            interpreter.Eat();

            string varName = interpreter.Eat("IDENTIFIER").ToString();

            interpreter.Eat("L_CURLY_BRACE");

            bool got = false;
            bool setted = false;

            Property p = new Property();
            p.name = varName;

            while (interpreter.currentToken.type == "GET" || interpreter.currentToken.type == "SET")
            {
                bool isGet = interpreter.currentToken.type == "GET";

                if (isGet && got)
                {
                    throw new InterpreterException(interpreter.currentToken, varName + " already has a get statement!");
                }
                else if (!isGet && setted)
                {
                    throw new InterpreterException(interpreter.currentToken, varName + " already has a set statement!");
                }

                interpreter.Eat();

                List<Token> tokens = new List<Token>();
                MinceUserFunction func = new MinceUserFunction();

                if (!isGet)
                {
                    interpreter.Eat("COLON");
                    string vName = interpreter.Eat("IDENTIFIER").ToString();

                    func.variableNames.Add(vName);
                }

                interpreter.Eat("L_CURLY_BRACE");
                interpreter.depth++;

                tokens.Add(new Token(0, 0, "L_CURLY_BRACE"));

                tokens = tokens.Concat(interpreter.SkipBlock()).ToList();

                func.value = tokens;

                if (isGet)
                {
                    p.getFunc = func;
                    got = true;
                }
                else
                {
                    p.setFunc = func;
                    setted = true;
                }
            }

            interpreter.Eat("R_CURLY_BRACE");

            p.isReadOnly = setted;
            p.isPrivate = isPrivate;

            return p;
        }

        public Variable GetField(Interpreter interpreter, bool isPrivate)
        {
            interpreter.Eat();

            string varName = interpreter.Eat("IDENTIFIER").ToString();

            Variable variable = new Variable();
            variable.name = varName;

            MinceObject value;

            if (interpreter.currentToken.type == "EQUALS")
            {
                interpreter.Eat();

                value = interpreter.evaluation.Evaluate();
            }
            else
            {
                value = new MinceNull();
            }

            variable.SetValue(value);
            variable.isPrivate = isPrivate;

            interpreter.Eat("SEMICOLON");

            return variable;
        }

        public Variable GetMethod(Interpreter interpreter, bool isPrivate)
        {
            int initialDepth = interpreter.depth;

            interpreter.Eat();

            MinceUserFunction userFunc = new MinceUserFunction();

            List<Token> tokens = new List<Token>();

            string funcName = interpreter.Eat().ToString();

            List<string> paramNames = new List<string>();

            if (interpreter.currentToken.type == "COLON")
            {
                interpreter.Eat();

                while (interpreter.currentToken.type == "IDENTIFIER")
                {
                    string name = interpreter.Eat().ToString();

                    if (interpreter.variables.Exists(name))
                    {
                        throw new InterpreterException(interpreter.currentToken, "A variable called '" + name + "' already exists!");
                    }

                    paramNames.Add(name);

                    if (interpreter.currentToken.type == "L_CURLY_BRACE")
                    {
                        break;
                    }
                    else if (interpreter.currentToken.type == "COMMA")
                    {
                        interpreter.Eat();
                    }
                    else
                    {
                        throw new InterpreterException(interpreter.currentToken, "Expected ',' or '{' after parameter name");
                    }
                }
            }

            interpreter.Eat("L_CURLY_BRACE");
            interpreter.depth++;

            tokens.Add(new Token(0, 0, "L_CURLY_BRACE"));

            tokens = tokens.Concat(interpreter.SkipBlock()).ToList();

            userFunc.value = tokens;
            userFunc.variableNames = paramNames;

            Variable v = new Variable(funcName, userFunc, true);
            v.isPrivate = isPrivate;

            return v;
        }

        public MinceObject NewClass(MinceObject original, MinceObject[] args)
        {
            MinceObject obj = original.clone();

            foreach (var member in obj.members)
            {
                if (member.GetType() == typeof(Property))
                {
                    Property prop = (Property)member;
                    prop.getFunc.parent = obj;

                    if (prop.setFunc != null)
                    {
                        prop.setFunc.parent = obj;
                    }

                    prop.Init();
                }
                else if (member.GetValue().GetType() == typeof(MinceUserFunction))
                {
                    ((MinceUserFunction)member.GetValue()).parent = obj;
                }
            }

            foreach (var method in obj.members)
            {
                if (method.GetType() == typeof(Variable) && method.GetValue().GetType() == typeof(MinceUserFunction) && method.name == "new")
                {
                    ((MinceUserFunction)method.GetValue()).call(args);
                    obj.members.Remove(method);
                    break;
                }
            }

            return obj;
        }
    }
}
