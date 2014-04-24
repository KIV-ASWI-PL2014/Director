using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    public class ParserCompareDefinition
    {
        public bool use_variable; // if true, use value as a variable name and upon finding this variable use it's value instead
        public bool if_present; // if true, the item represented in this definition does not need to appear in received result JSON; but if it does appear, then the all defined rules apply as they normally would
        public System.Type type;
        public string operation;
        public object value;
        public string var_name;

        public ParserCompareDefinition()
        {
            use_variable = false;
            if_present = false;
            type = null;
            operation = null;
            value = null;
            var_name = null;
        }

    }
}
