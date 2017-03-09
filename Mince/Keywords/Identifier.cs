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
        public static VariableTree GetTree(Interpreter interpreter)
        {
            VariableTree tree = new VariableTree();

            string identifier = interpreter.Eat().ToString();

            if (interpreter.variables.Exists(identifier))
            {
                Variable variable = interpreter.variables.Get(identifier);
                MinceObject value = variable.GetValue();
                bool lastWasFunc = value.GetType() == typeof(MinceFunction) || value.GetType() == typeof(MinceUserFunction);

                tree.Add(variable);

                while (true)
                {
                    if (interpreter.currentToken.type == "L_BRACKET")
                    {
                        interpreter.Eat();
                        MinceObject[] args = interpreter.GetParameters();
                        interpreter.Eat("R_BRACKET");

                        if (value.GetType() == typeof(MinceUserFunction))
                        {
                            value = (value as MinceUserFunction).call(args);
                        }
                        else if (value.GetType() == typeof(MinceFunction))
                        {
                            value = (value as MinceFunction).Call(args);
                        }
                        else
                        {
                            throw new InterpreterException(interpreter.currentToken, variable.name + " is not a function! It is a " + value.GetType().Name);
                        }

                        lastWasFunc = true;
                        continue;
                    }

                    if (interpreter.currentToken.type == "L_SQUARE_BRACKET")
                    {
                        if (value.GetType() != typeof(MinceArray))
                        {
                            throw new InterpreterException(interpreter.currentToken, "Can only apply an index to an Array, not a " + value.GetType().Name);
                        }

                        interpreter.Eat();
                        MinceObject index = interpreter.evaluation.Evaluate();

                        if (index.GetType() != typeof(MinceNumber))
                        {
                            throw new InterpreterException(interpreter.currentToken, "Index must be a number! Not a " + index.GetType().Name);
                        }

                        interpreter.Eat("R_SQUARE_BRACKET");

                        value = ((MinceArray)value).get((MinceNumber)index);

                        continue;
                    }

                    if (interpreter.currentToken.type == "DOT")
                    {
                        interpreter.Eat();
                        string name = interpreter.Eat("IDENTIFIER").ToString();

                        if (value.MemberExists(name))
                        {
                            variable = value.GetMember(name);
                            tree.Add(variable);

                            if (variable.isPrivate)
                            {
                                if (interpreter.parent == null || !object.ReferenceEquals(value, interpreter.parent))
                                {
                                    throw new InterpreterException(interpreter.previousToken, "'" + variable.name + "' is private!");
                                }
                            }

                            value = variable.GetValue();
                        }
                        else if (value.GetType() == typeof(MinceDynamic))
                        {
                            Variable v = new Variable(name, new MinceDynamic(), false, -1);
                            value.members.Add(v);

                            variable = v;
                            tree.Add(variable);
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

                tree.lastWasFunc = false;

                return tree;
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
                interpreter.pointer--;
                VariableTree tree = Identifier.GetTree(interpreter);

                if (!tree.lastWasFunc)
                {
                    if (interpreter.currentToken.type == "EQUALS")
                    {
                        if (tree.lastVariable.isReadOnly)
                        {
                            throw new InterpreterException(interpreter.previousToken, "'" + tree.lastVariable.name + "' is readonly!");
                        }

                        interpreter.Eat();

                        MinceObject result = interpreter.evaluation.Evaluate();

                        tree.lastVariable.SetValue(result);
                    }
                    else if (interpreter.currentToken.type == "PLUS" && interpreter.tokens[interpreter.pointer + 1].type == "PLUS")
                    {
                        if (tree.lastVariable.GetValue().GetType() != typeof(MinceNumber))
                        {
                            throw new InterpreterException(interpreter.previousToken, "Can only increment a MinceNumber, not a " + tree.lastVariable.GetValue().GetType().Name);
                        }

                        interpreter.Eat();
                        interpreter.Eat();

                        ((MinceNumber)tree.lastVariable.GetValue()).inc();
                    }
                    else if (interpreter.currentToken.type == "MINUS" && interpreter.tokens[interpreter.pointer + 1].type == "MINUS")
                    {
                        if (tree.lastVariable.GetValue().GetType() != typeof(MinceNumber))
                        {
                            throw new InterpreterException(interpreter.previousToken, "Can only increment a MinceNumber, not a " + tree.lastVariable.GetValue().GetType().Name);
                        }

                        interpreter.Eat();
                        interpreter.Eat();

                        MinceNumber num = (MinceNumber)tree.lastVariable.GetValue();
                        tree.lastVariable.SetValue(num.Minus(new MinceNumber(1)));
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
