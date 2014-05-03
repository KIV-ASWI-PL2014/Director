using Director.ParserLib;
using System;
using System.Collections.Generic;

namespace Director.Formatters
{
    public class JSONFormatter
    {
        /// <summary>
        /// End line.
        /// </summary>
        const string ENDL = "\n";

        /// <summary>
        /// Format string.
        /// </summary>
        /// <param name="template">JSON string</param>
        /// <param name="space_count">Space count</param>
        /// <returns></returns>
        public static string Format(String template, int space_count = 4)
        {
            // Template
            if (template == null || template.Length == 0) return "";

            // Parse template
            List<ParserError> errors = new List<ParserError>();
            Dictionary<String, ParserItem> items = Parser.deserialize(template, errors, "template");

            // Invalid formatting!
            if (errors.Count > 0) return null;

            // Create instance
            JSONFormatter _f = new JSONFormatter(space_count);

            // Start drawing
            _f.PrintObject(items);

            // Return
            return _f.Result;
        }

        /// <summary>
        /// Spaces per space.
        /// </summary>
        public int Spaces { get; set; }

        /// <summary>
        /// Space string.
        /// </summary>
        public String Space { get; private set; }

        /// <summary>
        /// Prviate string result..
        /// </summary>
        private String result;

        /// <summary>
        /// Result.
        /// </summary>
        public String Result
        {
            get
            {
                return result;
            }
            private set
            {
                result = value;
            }
        }

        /// <summary>
        /// Create json formatter!
        /// </summary>
        public JSONFormatter(int spaces)
        {
            // Set spaces
            Spaces = spaces;

            // Create space string
            CreateSpaceString();

            // Result
            Result = "";
        }

        /// <summary>
        /// Create space string.
        /// </summary>
        private void CreateSpaceString()
        {
            Space = "";

            for (int i = 0; i < Spaces; i++)
                Space += " ";
        }

        /// <summary>
        /// Draw json.
        /// </summary>
        private void PrintObject(Dictionary<String, ParserItem> items, int spaces = 0)
        {
            int i = items.Keys.Count;
            Result += "{" + ENDL;
            foreach (KeyValuePair<string, ParserItem> pair in items)
            {
                i--;
                DrawParserItem(pair.Key, pair.Value, i != 0, spaces + 1);
            }
            AppendSpaces(spaces);
            Result += "}";

            if (spaces == 0)
                Result += ENDL;
        }

        /// <summary>
        /// Draw parser item
        /// </summary>
        private void DrawParserItem(string key, ParserItem item, bool comma, int spaces)
        {
            AppendSpaces(spaces);
            if (key != null) 
                Result += string.Format("\"{0}\" : ", key);

            if (item.value == null)
            {
                Result += "null";
            }
            else if (item.value is Dictionary<string, ParserItem>)
            {
                PrintObject((Dictionary<string, ParserItem>)item.value, spaces);
            }
            else if (item.value is List<ParserItem>)
            {
                Result += "[" + ENDL;
                DrawArray((List<ParserItem>) item.value, spaces + 1);
                AppendSpaces(spaces);
                Result += "]";
            }
            else if (item.value is System.Int64 || item.value is System.Int32 || item.value is System.Double)
            {
                Result += (item.value + "").Replace(',', '.');
            }
            else
            {
                Result += string.Format("\"{0}\"", item.value.ToString());
            }

            // Comma?
            if (comma)
                Result += ",";

            // Endline
            Result += ENDL;
        }

        /// <summary>
        /// Draw array.
        /// </summary>
        private void DrawArray(List<ParserItem> list, int spaces)
        {
            int x = list.Count;
            foreach (var item in list)
            {
                x--;
                DrawParserItem(null, item, x != 0, spaces);
            }
        }

        /// <summary>
        /// Append string spaces.
        /// </summary>
        private void AppendSpaces(int spaces)
        {
            for (int i = 0; i < spaces; i++)
                Result += Space;
        }
    }
}
