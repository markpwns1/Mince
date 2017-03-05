using System;
using System.Collections.Generic;
using System.IO;
using Mince.Types;

namespace Mince
{
    public class Lexer
    {
        public string filename;
        public string inputText;

        public int pointer = 0;

        public int line = 1;
        public int column = 0;

        public void Eat()
        {
            pointer++;
            column++;
        }

        public void NewLine()
        {
            column = 0;
            line++;
        }

        public List<Token> ScanFile(string file)
        {
            filename = file;
            inputText = File.ReadAllText(file);

            List<Token> list = new List<Token>();

            while (true)
            {
                Token tk = GetNextToken();
                
                list.Add(tk);

                if (tk.type == "EOF")
                {
                    break;
                }
            }

            return list;
        }

        public List<Token> ScanString(string input)
        {
            filename = "untitled";
            inputText = input;

            List<Token> list = new List<Token>();

            while (true)
            {
                Token tk = GetNextToken();

                list.Add(tk);

                if (tk.type == "EOF")
                {
                    break;
                }
            }

            return list;
        }

        private Token GetNextToken()
        {
            if (pointer >= inputText.Length)
            {
                return new Token(line, column, "EOF");
            }

            if (CurrentChar == '\n')
            {
                NewLine();
            }

            if (char.IsWhiteSpace(CurrentChar))
            {
                SkipWhitespace();
            }

            if (pointer >= inputText.Length)
            {
                return new Token(line, column, "EOF");
            }

            char c = CurrentChar;

            if (char.IsDigit(c))
            {
                return new Token(line, column, "NUMBER", GetNumber());
            }

            if (char.IsLetter(c))
            {
                Token tk = new Token(line, column, "IDENTIFIER", GetIdentifier());

                switch (tk.value.ToString())
                {
                    case "true":
                        tk.type = "BOOL";
                        tk.value = new MinceBool(bool.Parse(tk.value.ToString()));
                        break;

                    case "false":
                        tk.type = "BOOL";
                        tk.value = new MinceBool(bool.Parse(tk.value.ToString()));
                        break;

                    case "null":
                        tk.type = "NULL";
                        tk.value = new MinceNull();
                        break;

                    case "if": tk.type = "IF"; break;
                    case "else": tk.type = "ELSE"; break;
                    case "while": tk.type = "WHILE"; break;
                    case "function": tk.type = "FUNCTION"; break;
                    case "return": tk.type = "RETURN"; break;
                    case "new": tk.type = "NEW"; break;
                    case "and": tk.type = "AND"; break;
                    case "or": tk.type = "OR"; break;
                    case "not": tk.type = "NOT"; break;
                    case "until": tk.type = "UNTIL"; break;
                    case "break": tk.type = "BREAK"; break;
                    case "continue": tk.type = "CONTINUE"; break;
                    case "foreach": tk.type = "FOR_EACH"; break;
                    case "free": tk.type = "FREE"; break;
                    case "for": tk.type = "FOR"; break;
                    case "global": tk.type = "GLOBAL"; break;
                    case "local": tk.type = "LOCAL"; break;
                    case "try": tk.type = "TRY"; break;
                    case "catch": tk.type = "CATCH"; break;
                    case "class": tk.type = "CLASS"; break;
                    case "method": tk.type = "METHOD"; break;
                    case "getter": tk.type = "GET"; break;
                    case "setter": tk.type = "SET"; break;
                    case "field": tk.type = "FIELD"; break;
                    case "property": tk.type = "PROPERTY"; break;
                    case "public": tk.type = "PUBLIC"; break;
                    case "private": tk.type = "PRIVATE"; break;
                } 

                return tk;
            }

            if (c == '-')
            {
                Eat();

                if (char.IsDigit(CurrentChar))
                {
                    return new Token(line, column, "NUMBER", GetNegativeNumber());
                }

                return new Token(line, column, "MINUS");
            }

            if (c == '=')
            {
                Token tk = new Token(line, column, "EQUALS", GetEquals());
                if (tk.value.ToString() == "==")
                {
                    tk.type = "IS_EQUAL";
                }
                return tk;
            }

            if (c == '>' || c == '<')
            {
                Token tk = new Token(line, column, null, GetComparator(c.ToString()));
                switch (tk.value.ToString())
                {
                    case ">": tk.type = "GREATER_THAN"; break;
                    case "<": tk.type = "LESS_THAN"; break;
                    case ">=": tk.type = "GREATER_OR_EQUAL"; break;
                    case "<=": tk.type = "LESS_OR_EQUAL"; break;
                }
                return tk;
            }

            if (c == '!')
            {
                Token tk = new Token(line, column, "NOT", GetComparator(c.ToString()));
                if (tk.value.ToString() == "!=")
                {
                    tk.type = "NOT_EQUAL";
                }
                return tk;
            }

            if (c == '\'')
            {
                Token tk = new Token(line, column, "CHAR");
                Eat();
                tk.value = new MinceChar(CurrentChar);
                pointer += 2;
                return tk;
            }

            if (c == '/')
            {
                Token tk = new Token(line, column, "DIVIDE");
                Eat();

                if (CurrentChar == '/')
                {
                    Eat();
                    SkipSingleLineComment();
                    return GetNextToken();
                }
                else if (CurrentChar == '*')
                {
                    Eat();
                    SkipMultilineComment();
                    return GetNextToken();
                }

                return tk;
            }

            switch (c)
            {
                case '+': Eat(); return new Token(line, column, "PLUS");
                case '*': Eat(); return new Token(line, column, "MULTIPLY");
                case '%': Eat(); return new Token(line, column, "MODULUS");
                case '^': Eat(); return new Token(line, column, "EXPONENT");
                case '(': Eat(); return new Token(line, column, "L_BRACKET");
                case ')': Eat(); return new Token(line, column, "R_BRACKET");
                case '.': Eat(); return new Token(line, column, "DOT");
                case ',': Eat(); return new Token(line, column, "COMMA");
                case '{': Eat(); return new Token(line, column, "L_CURLY_BRACE");
                case '}': Eat(); return new Token(line, column, "R_CURLY_BRACE");
                case ':': Eat(); return new Token(line, column, "COLON");
                case '#': Eat(); return new Token(line, column, "HASH");
                case '@': Eat(); return new Token(line, column, "AT_SYMBOL");
                case ';': Eat(); return new Token(line, column, "SEMICOLON");
                case '&': Eat(); return new Token(line, column, "AND");
                case '|': Eat(); return new Token(line, column, "OR");
                case '[': Eat(); return new Token(line, column, "L_SQUARE_BRACKET");
                case ']': Eat(); return new Token(line, column, "R_SQUARE_BRACKET");
                case '\"': return new Token(line, column, "STRING", GetString());
            }

            throw new InterpreterException(new Token(line, column), "Unrecognized character " + c.ToString() + " at position " + pointer);
        }

        private MinceString GetString()
        {
            string result = "";
            Eat();

            while (CurrentChar != '\"')
            {
                if (CurrentChar == '\\')
                {
                    Eat();

                    if (CurrentChar == 'n')
                    {
                        result += Environment.NewLine;
                        Eat();
                        continue;
                    }
                }

                result += CurrentChar;

                Eat();
            }

            Eat();
            return new MinceString(result);
        }

        private MinceNumber GetNegativeNumber()
        {
            string result = "-";

            bool usedDecimal = false;

            while (pointer < inputText.Length && (char.IsDigit(CurrentChar) || (CurrentChar == '.' && !usedDecimal && char.IsDigit(NextChar))))
            {
                result += CurrentChar;

                if (CurrentChar == '.')
                {
                    usedDecimal = true;
                }

                Eat();
            }

            return new MinceNumber(float.Parse(result));
        }

        private MinceNumber GetNumber()
        {
            string result = "";

            bool usedDecimal = false;

            while (pointer < inputText.Length && (char.IsDigit(CurrentChar) || (CurrentChar == '.' && !usedDecimal && char.IsDigit(NextChar))))
            {
                result += CurrentChar;

                if (CurrentChar == '.')
                {
                    usedDecimal = true;
                }

                Eat();
            }

            return new MinceNumber(float.Parse(result));
        }

        private string GetIdentifier()
        {
            string result = "";

            while (pointer < inputText.Length && (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_'))
            {
                result += CurrentChar;
                Eat();
            }

            return result;
        }

        private void SkipWhitespace()
        {
            while (pointer < inputText.Length && char.IsWhiteSpace(CurrentChar))
            {
                if (CurrentChar == '\n')
                {
                    NewLine();
                }

                Eat();
            }
        }

        private char CurrentChar
        {
            get { return inputText[pointer]; }
        }

        private char NextChar
        {
            get { return inputText[pointer + 1]; }
        }

        private string GetEquals()
        {
            string result = "=";
            Eat();
            if (CurrentChar == '=')
            {
                result += '=';
                Eat();
            }
            return result;
        }

        private string GetComparator(string result)
        {
            Eat();
            if (CurrentChar == '=')
            {
                result += '=';
                Eat();
            }
            return result;
        }

        private void SkipSingleLineComment()
        {
            while (pointer < inputText.Length && CurrentChar != '\n')
            {
                Eat();
            }
        }

        private void SkipMultilineComment()
        {
            while (true)
            {
                if (pointer >= inputText.Length)
                {
                    break;
                }

                if (CurrentChar == '*' && NextChar == '/')
                {
                    pointer += 2;
                    break;
                }

                Eat();
            }
        }

        private void GoTo(int position)
        {
            pointer = position;
        }

        private int GetPosition()
        {
            return pointer;
        }
    }
}
