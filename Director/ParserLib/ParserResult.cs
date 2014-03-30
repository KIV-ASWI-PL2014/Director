using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    class ParserResult
    {
        private List<ParserError> errors;
        private string result;

        ParserResult(List<ParserError> errors, string result)
        {
            this.errors = errors;
            this.result = result;
        }

        bool isSuccess()
        {
            if (errors == null || errors.Count == 0)
                return true;

            return false;
        }

        List<ParserError> getErrors()
        {
            return errors;
        }

        string getResult()
        {
            return result;
        }
    }
}
