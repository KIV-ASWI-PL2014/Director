using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    public class ParserError
    {
        private int line;
        private int position;
        private string message;

        public ParserError(int line, int position, string message)
        {
            this.line = line;
            this.position = position;
            this.message = message;
        }

        public int getLine()
        {
            return line;
        }

        public int getPosition()
        {
            return position;
        }

        public string getMessage()
        {
            return message;
        }
    }
}
