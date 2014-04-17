using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ParserLib
{
    class ParserItem
    {
        public object value;

        public int line;
        public int position;

        public ParserCompareDefinition comp_def { get; set; }

        public ParserItem(int line, int position, object value)
        {
            this.line = line;
            this.position = position;
            this.value = value;
            this.comp_def = null;
        }
    }
}
