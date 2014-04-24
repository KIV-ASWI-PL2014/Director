using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    /// <summary>
    /// This class represents an error which may occur during Parser execution. All errors have to be traceable to the specific position (line and
    /// position variables) of a specific file (source).
    /// Other classes need to be prevented from changing of an error's inner data, so once the error have been created you can only get the data.
    /// </summary>
    public class ParserError
    {
        private int line; // line of string/file on which the error occured
        private int position; // position of concrete line of string/file on which the error occured
        private string message;
        private string source; // string definition of source string/file in which error occured

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