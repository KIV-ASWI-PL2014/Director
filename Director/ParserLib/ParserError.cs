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
        private string source;

        public ParserError(int line, int position, string message, string source)
        {
            this.line = line;
            this.position = position;
            this.message = message;
            this.source = source;
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

        public string getSource()
        {
            return source;
        }
    }
}