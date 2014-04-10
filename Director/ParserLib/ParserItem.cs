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

        public ParserItem(int line, int position, object value)
        {
            this.line = line;
            this.position = position;
            this.value = value;
        }
    }
}
