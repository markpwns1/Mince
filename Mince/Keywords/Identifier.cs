using Mince.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince.Keywords
{
    [InterpreterKeyword("IDENTIFIER")]
    public class Identifier : Keyword
    {
        public static Variable GetIdentifier(Interpreter interpreter)
        {
            string identifier = interpreter.Eat().ToString();

            if (interpreter.variables.Exists(identifier))
            {
                Variable variable = interpreter.variables.Get(identifier);
                MinceObject value = variable.GetValue();

                while (true)
                {
                    if (interpreter.currentToken.type == "L_BRACKET")
                    {
                        if (variable.GetValue().GetType() == typeof(MinceUserFunction))
                        {
                            var func = variable.GetValue() as MinceUserFunction;

                            interpreter.Eat();
                            MinceObject[] args = interpreter.GetParameters();
                            interpreter.Eat("R_BRACKET");

                            value = func.call(args);
                        }
                        else if (variable.GetValue().GetType() == typeof(MinceFunction))
                        {
                            interpreter.Eat();
                            MinceObject[] p = interpreter.GetParameters();
                            interpreter.Eat("R_BRACKET");

                            value = (variable.GetValue() as MinceFunction).Call(p);
                        }
                        else
                        {
                            throw new InterpreterException(interpreter.currentToken, variable.name + " is not a function! It is a " + variable.GetValue().GetType().Name);
                        }
                    }

                    if (interpreter.currentToken.type == "DOT")
                    {
                        interpreter.Eat();
                        string name = interpreter.Eat("IDENTIFIER").ToString();

                        if (value.MemberExists(name))
                        {
                            variable = value.GetMember(name);

                            if (variable.isPrivate)
                            {
                                if (interpreter.parent == null || !object.ReferenceEquals(value, interpreter.parent))
                                {
                                    throw new InterpreterException(interpreter.previousToken, "'" + variable.name + "' is private!");
                                }
                            }

                            value = variable.GetValue();
                        }
                        else if (variable.GetValue().GetType() == typeof(MinceDynamic))
                        {
                            Variable v = new Variable(name, new MinceDynamic(), false, -1);
                            value.members.Add(v);

                            variable = v;
                        }
                        else
                        {
                            throw new InterpreterException(interpreter.previousToken, "'" + variable.name + "' (" + variable.GetValue().GetType().Name + ") does not contain member '" + name + "'");
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return variable;
            }
            else
            {
                throw new InterpreterException(interpreter.previousToken, "Variable '" + identifier + "' does not exist!");
            }
        }

        public override MinceObject Evaluate(Interpreter interpreter)
        {
            string identifier = interpreter.Eat().ToString();

            if (interpreter.variables.Exists(identifier))
            {
                Variable variable = interpreter.variables.Get(identifier);
                MinceObject value = variable.GetValue();
                bool lastWasFunc = variable.GetValue().GetType() == typeof(MinceFunction) || variable.GetValue().GetType() == typeof(MinceUserFunction);

                while (true)
                {
                    if (interpreter.currentToken.type == "L_BRACKET")
                    {
                        if (variable.GetValue().GetType() == typeof(MinceUserFunction))
                        {
                            var func = variable.GetValue() as MinceUserFunction;

                            interpreter.Eat();
                            MinceObject[] args = interpreter.GetParameters();
                            interpreter.Eat("R_BRACKET");

                            value = func.call(args);
                        }
                        else if (variable.GetValue().GetType() == typeof(MinceFunction))
                        {
                            interpreter.Eat();
                            MinceObject[] p = interpreter.GetParameters();
                            interpreter.Eat("R_BRACKET");

                            value = (variable.GetValue() as MinceFunction).Call(p);
                        }
                        else
                        {
                            throw new InterpreterException(interpreter.currentToken, variable.name + " is not a function! It is a " + variable.GetValue().GetType().Name);
                        }

                        lastWasFunc = true;
                    }

                    if (interpreter.currentToken.type == "DOT")
                    {
                        interpreter.Eat();
                        string name = interpreter.Eat("IDENTIFIER").ToString();

                        if (value.MemberExists(name))
                        {
                            variable = value.GetMember(name);

                            if (variable.isPrivate)
                            {
                                if (interpreter.parent == null || !object.ReferenceEquals(value, interpreter.parent))
                                {
                                    throw new InterpreterException(interpreter.previousToken, "'" + variable.name + "' is private!");
                                }
                            }

                            value = variable.GetValue();
                        }
                        else if (variable.GetValue().GetType() == typeof(MinceDynamic))
                        {
                            Variable v = new Variable(name, new MinceDynamic(), false, -1);
                            value.members.Add(v);

                            variable = v;
                        }
                        else
                        {
                            throw new InterpreterException(interpreter.previousToken, "'" + variable.name + "' (" + variable.GetValue().GetType().Name + ") does not contain member '" + name + "'");
                        }

                        lastWasFunc = false;
                    }
                    else
                    {
                        break;
                    }
                }

                if (!lastWasFunc)
                {
                    if (interpreter.currentToken.type == "EQUALS")
                    {
                        if (variable.isReadOnly)
                        {
                            throw new InterpreterException(interpreter.previousToken, "'" + variable.name + "' is readonly!");
                        }

                        interpreter.Eat();

                        MinceObject result = interpreter.evaluation.Evaluate();

                        variable.SetValue(result);
                    }
                    else if (interpreter.currentToken.type == "PLUS" && interpreter.tokens[interpreter.pointer + 1].type == "PLUS")
                    {
                        if (variable.GetValue().GetType() != typeof(MinceNumber))
                        {
                            throw new InterpreterException(interpreter.previousToken, "Can only increment a MinceNumber, not a " + variable.GetValue().GetType().Name);
                        }

                        interpreter.Eat();
                        interpreter.Eat();

                        ((MinceNumber)variable.GetValue()).inc();
                    }
                }

                interpreter.Eat("SEMICOLON");
            }
            else
            {
                Variable variable = new Variable();
                variable.name = identifier;
                variable.depth = interpreter.depth;

                interpreter.Eat("EQUALS");

                MinceObject value = interpreter.evaluation.Evaluate();

                variable.SetValue(value);

                interpreter.variables.variables.Add(variable);
                interpreter.Eat("SEMICOLON");
            }

            return new MinceNull();
        }
    }
}
