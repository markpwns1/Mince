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
                return new Token(pointer, "EOF");
            }

            if (char.IsWhiteSpace(GetCurrentChar()))
            {
                SkipWhitespace();
            }

            if (pointer >= inputText.Length)
            {
                return new Token(pointer, "EOF");
            }

            char c = GetCurrentChar();

            if (char.IsDigit(c))
            {
                return new Token(pointer, "NUMBER", GetNumber());
            }

            if (char.IsLetter(c))
            {
                Token tk = new Token(pointer, "IDENTIFIER", GetIdentifier());

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
                pointer++;

                if (char.IsDigit(GetCurrentChar()))
                {
                    return new Token(pointer, "NUMBER", GetNegativeNumber());
                }

                return new Token(pointer, "MINUS");
            }

            if (c == '=')
            {
                Token tk = new Token(pointer, "EQUALS", GetEquals());
                if (tk.value.ToString() == "==")
                {
                    tk.type = "IS_EQUAL";
                }
                return tk;
            }

            if (c == '>' || c == '<')
            {
                Token tk = new Token(pointer, null, GetComparator(c.ToString()));
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
                Token tk = new Token(pointer, "NOT", GetComparator(c.ToString()));
                if (tk.value.ToString() == "!=")
                {
                    tk.type = "NOT_EQUAL";
                }
                return tk;
            }

            if (c == '\'')
            {
                Token tk = new Token(pointer, "CHAR");
                pointer++;
                tk.value = new MinceChar(GetCurrentChar());
                pointer += 2;
                return tk;
            }

            if (c == '/')
            {
                Token tk = new Token(pointer, "DIVIDE");
                pointer++;

                if (GetCurrentChar() == '/')
                {
                    pointer++;
                    SkipSingleLineComment();
                    return GetNextToken();
                }
                else if (GetCurrentChar() == '*')
                {
                    pointer++;
                    SkipMultilineComment();
                    return GetNextToken();
                }

                return tk;
            }

            switch (c)
            {
                case '+': pointer++; return new Token(pointer, "PLUS");
                case '*': pointer++; return new Token(pointer, "MULTIPLY");
                case '%': pointer++; return new Token(pointer, "MODULUS");
                case '^': pointer++; return new Token(pointer, "EXPONENT");
                case '(': pointer++; return new Token(pointer, "L_BRACKET");
                case ')': pointer++; return new Token(pointer, "R_BRACKET");
                case '.': pointer++; return new Token(pointer, "DOT");
                case ',': pointer++; return new Token(pointer, "COMMA");
                case '{': pointer++; return new Token(pointer, "L_CURLY_BRACE");
                case '}': pointer++; return new Token(pointer, "R_CURLY_BRACE");
                case ':': pointer++; return new Token(pointer, "COLON");
                case '#': pointer++; return new Token(pointer, "HASH");
                case '@': pointer++; return new Token(pointer, "AT_SYMBOL");
                case ';': pointer++; return new Token(pointer, "SEMICOLON");
                case '&': pointer++; return new Token(pointer, "AND");
                case '|': pointer++; return new Token(pointer, "OR");
                case '[': pointer++; return new Token(pointer, "L_SQUARE_BRACKET");
                case ']': pointer++; return new Token(pointer, "R_SQUARE_BRACKET");
                case '\"': return new Token(pointer, "STRING", GetString());
            }

            throw new Exception("Unrecognized character " + c.ToString() + " at position " + pointer);
        }

        private MinceString GetString()
        {
            string result = "";
            pointer++;
            while (GetCurrentChar() != '\"')
            {
                if (GetCurrentChar() == '\\')
                {
                    pointer++;
                }

                result += GetCurrentChar();
                pointer++;
            }
            pointer++;
            return new MinceString(result);
        }

        private MinceNumber GetNegativeNumber()
        {
            string result = "-";

            bool usedDecimal = false;

            while (pointer < inputText.Length && (char.IsDigit(GetCurrentChar()) || (GetCurrentChar() == '.' && !usedDecimal && char.IsDigit(GetNextChar()))))
            {
                result += GetCurrentChar();

                if (GetCurrentChar() == '.')
                {
                    usedDecimal = true;
                }

                pointer++;
            }

            return new MinceNumber(float.Parse(result));
        }

        private MinceNumber GetNumber()
        {
            string result = "";

            bool usedDecimal = false;

            while (pointer < inputText.Length && (char.IsDigit(GetCurrentChar()) || (GetCurrentChar() == '.' && !usedDecimal && char.IsDigit(GetNextChar()))))
            {
                result += GetCurrentChar();

                if (GetCurrentChar() == '.')
                {
                    usedDecimal = true;
                }

                pointer++;
            }

            return new MinceNumber(float.Parse(result));
        }

        private string GetIdentifier()
        {
            string result = "";

            while (pointer < inputText.Length && (char.IsLetterOrDigit(GetCurrentChar()) || GetCurrentChar() == '_'))
            {
                result += GetCurrentChar();
                pointer++;
            }

            return result;
        }

        private void SkipWhitespace()
        {
            while (pointer < inputText.Length && char.IsWhiteSpace(GetCurrentChar()))
            {
                pointer++;
            }
        }

        private char GetCurrentChar()
        {
            return inputText[pointer];
        }

        private char GetNextChar()
        {
            return inputText[pointer + 1];
        }

        private string GetEquals()
        {
            string result = "=";
            pointer++;
            if (GetCurrentChar() == '=')
            {
                result += '=';
                pointer++;
            }
            return result;
        }

        private string GetComparator(string result)
        {
            pointer++;
            if (GetCurrentChar() == '=')
            {
                result += '=';
                pointer++;
            }
            return result;
        }

        private void SkipSingleLineComment()
        {
            while (pointer < inputText.Length && GetCurrentChar() != '\n')
            {
                pointer++;
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

                if (GetCurrentChar() == '*' && GetNextChar() == '/')
                {
                    pointer += 2;
                    break;
                }

                pointer++;
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
