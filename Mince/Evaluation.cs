using System;
using System.Collections.Generic;
using Mince.Types;
using System.Linq;

namespace Mince
{
    public class Evaluation
    {
        public Interpreter interpreter;
        //public Variables variables = new Variables();

        public void Init(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public MinceObject Evaluate()
        {
            return AndOr();
        }

        public MinceObject AndOr()
        {
            MinceObject result = Compare();
            while (interpreter.currentToken.type == "AND" || interpreter.currentToken.type == "OR")
            {
                switch (interpreter.currentToken.type)
                {
                    case "AND":
                        interpreter.Eat();
                        result = result.And((MinceBool)Compare());
                        break;

                    case "OR":
                        interpreter.Eat();
                        result = result.Or((MinceBool)Compare());
                        break;
                }
            }
            return result;
        }

        public MinceObject Compare()
        {
            MinceObject result = Add();

            while (
                new List<string>() {
                    "IS_EQUAL", "NOT_EQUAL", "GREATER_THAN", "LESS_THAN", "GREATER_OR_EQUAL", "LESS_OR_EQUAL"
                }.Contains(interpreter.currentToken.type))
            {
                switch (interpreter.currentToken.type)
                {
                    case "IS_EQUAL":
                        interpreter.Eat();
                        result = result.EqualTo(Add());
                        break;

                    case "NOT_EQUAL":
                        interpreter.Eat();
                        result = result.NotEqualTo(Add());
                        break;

                    case "GREATER_THAN":
                        interpreter.Eat();
                        result = result.GreaterThan(Add());
                        break;

                    case "LESS_THAN":
                        interpreter.Eat();
                        result = result.LessThan(Add());
                        break;

                    case "GREATER_OR_EQUAL":
                        interpreter.Eat();
                        result = result.GreaterOrEqual(Add());
                        break;

                    case "LESS_OR_EQUAL":
                        interpreter.Eat();
                        result = result.LessOrEqual(Add());
                        break;
                }
            }

            return result;
        }

        public MinceObject Add()
        {
            MinceObject result = Multiply();

            while (interpreter.currentToken.type == "PLUS" || interpreter.currentToken.type == "MINUS")
            {
                switch (interpreter.currentToken.type)
                {
                    case "PLUS":
                        interpreter.Eat();
                        result = result.Plus(Multiply());
                        break;

                    case "MINUS":
                        interpreter.Eat();
                        result = result.Minus(Multiply());
                        break;
                }
            }

            return result;
        }

        public MinceObject Multiply()
        {
            MinceObject result = Exponent();

            while (interpreter.currentToken.type == "MULTIPLY" || interpreter.currentToken.type == "DIVIDE")
            {
                switch (interpreter.currentToken.type)
                {
                    case "MULTIPLY":
                        interpreter.Eat();
                        result = result.Multiply(Exponent());
                        break;

                    case "DIVIDE":
                        interpreter.Eat();
                        result = result.Divide(Exponent());
                        break;
                }
            }

            return result;
        }

        public MinceObject Exponent()
        {
            MinceObject result = Members();

            while (interpreter.currentToken.type == "EXPONENT")
            {
                interpreter.Eat();
                result = result.Exponent(Members());
            }

            return result;
        }

        public MinceObject Members()
        {
            MinceObject result = Factor();

            while (interpreter.currentToken.type == "DOT")
            {
                interpreter.Eat();

                string memberName = interpreter.Eat("IDENTIFIER").ToString();

                if (result.MemberExists(memberName))
                {
                    if (result.GetMember(memberName).isPrivate)
                    {
                        if (interpreter.parent == null || !object.ReferenceEquals(result, interpreter.parent))
                        {
                            throw new InterpreterException(interpreter.currentToken, "'" + result.GetMember(memberName).name + "' is private!");
                        }
                    }

                    if (interpreter.currentToken.type == "L_BRACKET")
                    {
                        var member = result.GetMember(memberName).GetValue();

                        if (member.GetType() == typeof(MinceUserFunction))
                        {
                            var func = member as MinceUserFunction;

                            interpreter.Eat();
                            MinceObject[] args = interpreter.GetParameters();
                            interpreter.Eat("R_BRACKET");

                            result = func.call(args);
                        }
                        else if (member.GetType() == typeof(MinceFunction))
                        {
                            interpreter.Eat("L_BRACKET");

                            MinceObject[] p = interpreter.GetParameters();
                            result = (result.GetMember(memberName).GetValue() as MinceFunction).Call(p);

                            interpreter.Eat("R_BRACKET");
                        }
                        else
                        {
                            throw new InterpreterException(interpreter.currentToken, "You can only call functions, not " + member.GetType());
                        }
                        
                    }
                    else
                    {
                        result = result.GetMember(memberName).GetValue();
                    }
                }
                else
                {
                    throw new InterpreterException(interpreter.currentToken, "'" + memberName + "' is inaccessible.");
                }
            }

            return result;
        }

        public MinceObject Factor()
        {
            if (new List<string>() { "NUMBER", "STRING", "BOOL", "CHAR" }.Contains(interpreter.currentToken.type))
            {
                return (MinceObject) interpreter.Eat();
            }
            else if (interpreter.currentToken.type == "L_BRACKET")
            {
                interpreter.Eat("L_BRACKET");
                MinceObject result = Evaluate();
                interpreter.Eat("R_BRACKET");
                return result;
            }
            else if (interpreter.currentToken.type == "IDENTIFIER")
            {
                string name = interpreter.Eat().ToString();
                if (interpreter.variables.Exists(name))
                {
                    var result = interpreter.variables.Get(name).GetValue();

                    if (result.GetType() == typeof(MinceUserFunction))
                    {
                        if (interpreter.currentToken.type == "L_BRACKET")
                        {
                            var func = result as MinceUserFunction;

                            interpreter.Eat();
                            MinceObject[] args = interpreter.GetParameters();
                            interpreter.Eat("R_BRACKET");

                            return func.call(args);
                        }
                        else
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return result;
                    }
                }
                else
                {
                    throw new InterpreterException(interpreter.currentToken, "'" + name + "' does not exist in this context");
                }
            }
            else if (interpreter.currentToken.type == "NEW")
            {
                interpreter.Eat();

                var func = Interpreter.types[interpreter.Eat("IDENTIFIER").ToString()];
                interpreter.Eat("L_BRACKET");
                var p = interpreter.GetParameters();
                interpreter.Eat("R_BRACKET");

                return func.Invoke(p);
            }
            else if (interpreter.currentToken.type == "NOT")
            {
                interpreter.Eat();
                return new MinceBool(!(bool)(this.Evaluate() as MinceBool).value);
            }
            else if (interpreter.currentToken.type == "NULL")
            {
                interpreter.Eat();
                return new MinceNull();
            }
            else if (interpreter.currentToken.type == "FUNCTION")
            {
                int initialDepth = interpreter.depth;

                interpreter.Eat();

                MinceUserFunction userFunc = new MinceUserFunction();

                List<Token> tokens = new List<Token>();

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
                            throw new Exception("Expected ',' or '{' after parameter name");
                        }
                    }
                }

                interpreter.Eat("L_CURLY_BRACE");
                interpreter.depth++;

                tokens.Add(new Token(0, 0, "L_CURLY_BRACE"));

                tokens = tokens.Concat(interpreter.SkipBlock()).ToList();

                userFunc.value = tokens;
                userFunc.variableNames = paramNames;

                return userFunc;
            }
            else if (interpreter.currentToken.type == "L_SQUARE_BRACKET")
            {
                MinceArray ar = new MinceArray(new MinceObject[0]);
                interpreter.Eat();

                while (interpreter.currentToken.type != "R_SQUARE_BRACKET")
                {
                    ar.add(Evaluate());

                    if (interpreter.currentToken.type != "R_SQUARE_BRACKET")
                    {
                        interpreter.Eat("COMMA");
                    }
                }

                interpreter.Eat();

                return ar;
            }
            else if (interpreter.currentToken.type == "L_CURLY_BRACE")
            {
                MinceDynamic d = new MinceDynamic();
                interpreter.Eat();

                while (interpreter.currentToken.type == "IDENTIFIER")
                {
                    string name = interpreter.Eat().ToString();

                    interpreter.Eat("EQUALS");

                    MinceObject value = Evaluate();

                    Variable v = new Variable(name, value, true);

                    d.members.Add(v);

                    if (interpreter.currentToken.type != "COMMA")
                    {
                        break;
                    }

                    interpreter.Eat();
                }

                interpreter.Eat("R_CURLY_BRACE");

                return d;
            }
            else if (interpreter.currentToken.type == "EOF")
            {
                return new MinceNull();
            }

            throw new InterpreterException(interpreter.currentToken, 
                "Unexpected token " + interpreter.currentToken.ToString() + " found when looking for a factor at token #" + interpreter.pointer);
        }
    }
}
