using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    public class ParserResult
    {
        private List<ParserError> errors;
        private string result;

        public ParserResult(List<ParserError> errors, string result)
        {
            this.errors = errors;
            this.result = result;
        }

        public bool isSuccess()
        {
            if (errors == null || errors.Count == 0)
                return true;

            return false;
        }

        public List<ParserError> getErrors()
        {
            return errors;
        }

        public string getResult()
        {
            return result;
        }

        public string GetMarkdownReport()
        {
            if (isSuccess())
                return "";

            string ret = "";

            ret += "# Errors found\n";
            ret += "- " + errors.Count + "\n\n";

            ret += "# Problems\n";

            foreach (var i in errors)
                ret += String.Format("- [{0}, {1}] - {2}\n", i.Line, i.Position, i.Message);

            return ret;
        }
    }
}