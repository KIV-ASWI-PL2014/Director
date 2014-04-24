using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    public class ParserError
    {
        public int Line
        {
            get;
            private set;
        }

        public int Position
        {
            get;
            private set;
        }

        public string Source
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public ParserError(int line, int position, string message, string source)
        {
            Line = line;
            Position = position;
            Message = message;
            Source = source;
        }

        public int getLine()
        {
            return Line;
        }

        public int getPosition()
        {
            return Position;
        }

        public string getMessage()
        {
            return Message;
        }

        public string getSource()
        {
            return Source;
        }
    }
}