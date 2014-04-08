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
    }
}
