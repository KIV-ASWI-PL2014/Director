using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    class ParserError
    {
        private int line;
        private int position;
        private string message;

        ParserError(int line, int position, string message)
        {
            this.line = line;
            this.position = position;
            this.message = message;
        }

        int getLine()
        {
            return line;
        }

        int getPosition()
        {
            return position;
        }

        string getMessage()
        {
            return message;
        }
    }
}
