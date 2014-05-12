using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    public class ParserOccurence
    {
        public string name;
        public string type; // values: "text", "variable", "function"
        public List<string> arguments;

        public ParserOccurence(string name, string type, List<string> arguments)
        {
            this.name = name;
            this.type = type;
            this.arguments = arguments;
        }
    }
}
