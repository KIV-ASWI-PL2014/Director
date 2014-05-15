
namespace Director.ParserLib
{
    public class ParserItem
    {
        public object value;

        public int line;
        public int position;

		/// <summary>
		/// Array item? 
		/// </summary>
		public bool ArrayField { get; set; }

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
