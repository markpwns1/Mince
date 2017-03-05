using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mince
{
    public class InterpreterException : Exception
    {
        public Token token;

        public InterpreterException(Token token, string message)
            : base("At line " + token.line + ", column " + token.column + ": " + message)
        {
            this.token = token;
        }
    }
}
