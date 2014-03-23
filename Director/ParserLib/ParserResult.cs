using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    class ParserResult
    {
        private bool success;
        private Dictionary<string, string> customVariables;
        private List<ParserError> errors;

        ParserResult(Dictionary<string, string> customVariables, List<ParserError> errors)
        {
            this.customVariables = customVariables;
            this.errors = errors;
        }

        bool isSuccess()
        {
            if (errors == null || errors.Count == 0)
                return true;

            return false;
        }

        Dictionary<string, string> getCustomVariables()
        {
            return customVariables;
        }

        List<ParserError> getErrors()
        {
            return errors;
        }
    }
}
