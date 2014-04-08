using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    public class Parser
    {
        public Parser()
        {
        }

        public ParserResult generateRequest(string template, Dictionary<string, string> customVariables)
        {
            return null;
        }

        public ParserResult validateResponse(string template, Dictionary<string, string> customVariables)
        {
            return null;
        }

        public ParserResult parseResponse(string template, string response, Dictionary<string, string> customVariables)
        {
            using (StreamWriter writer = new StreamWriter("scenarios.txt", true))
            {
                writer.WriteLine("template: " + template);
                writer.WriteLine("response: " + response);
                writer.WriteLine("");
            }
            return null;
        }
    }
}
