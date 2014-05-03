using Director.ParserLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Inputs
{
    internal class ClickableResponseItem
    {

    }

    class JSONCanvas : Canvas
    {
        /// <summary>
        /// Template.
        /// </summary>
        public String Template { get; set; }

        /// <summary>
        /// Clickable items.
        /// </summary>
        public List<ClickableResponseItem> CanvasItems { get; set; }

        /// <summary>
        /// x
        /// </summary>
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Active context.
        /// </summary>
        public Context CTX { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public JSONCanvas()
        {
            CanvasItems = new List<ClickableResponseItem>();
        }

        /// <summary>
        /// Draw text.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dirtyRect"></param>
        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            if (Template == null)
                return;

            X = 20;
            Y = 0;
            CTX = ctx;

            // Data set
            DrawText("{");
            NextLine();
            X += 20;
            List<ParserError> errors = new List<ParserError>();
            Dictionary<string, ParserItem> data = Parser.deserialize(Template, errors, "template");
            DrawJson(data);
            X -= 20;
            DrawText("}");
        }

        void NextLine()
        {
            Y += 18;
        }

        void DrawJson(Dictionary<string, ParserItem> data)
        {
            int i = data.Keys.Count;
            foreach (KeyValuePair<string, ParserItem> pair in data)
            {
                i--;

                // Print Items
                DrawParserItem(pair.Key, pair.Value, i != 0);
            }
        }

        void DrawParserItem(string key, ParserItem item, bool comma = false)
        {

            string commaStr = (comma) ? "," : "";

            if (item.value is Dictionary<string, ParserItem>)
            {
                if (key != null)
                {
                    DrawText(string.Format("\"{0}\" : ", key) + "{");
                } else
                    DrawText("{");

                X += 20;
                NextLine();
                DrawJson((Dictionary<string, ParserItem>)item.value);
                X -= 20;
                DrawText("}" + commaStr);
            }
            else if (item.value is System.String)
            {
                if (key != null)
                {
                    DrawProperty(key, (String)item.value, comma);
                }
                else
                    DrawText(item.value + commaStr);
            }
            else if (item.value is System.Int64 || item.value is System.Double)
            {
                if (key != null)
                {
                    DrawProperty(key, item.value + "", comma);
                }
                else
                    DrawText(item.value + commaStr);
            }
            else if (item.value == null)
            {
                if (key != null)
                {
                    DrawProperty(key, "null", comma);
                }
                else
                    DrawText("null" + commaStr);
            }
            else if (item.value is List<ParserItem>)
            {
                DrawArray(key, (List<ParserItem>)item.value, comma);
            }
            else
            {
                DrawText(item.value.GetType().ToString());
            }
            NextLine();
        }

        private void DrawArray(string key, List<ParserItem> list, bool comma = false)
        {
            string commaStr = (comma) ? "," : "";

            if (key != null)
            {
                DrawText(string.Format("\"{0}\" : ", key) + "[");
            }
            else
                DrawText("[");

            X += 20;
            NextLine();
            var lastItem = list[list.Count - 1];
            foreach (var item in list)
            {
                DrawParserItem(null, item, item != lastItem);
            }

            X -= 20;
            DrawText("]" + commaStr);
        }

        TextLayout CreateTextLayout(string text)
        {
            var Item = new TextLayout();
            Item.Text = text;
            Item.Font.WithSize(10);
            return Item;
        }

        void DrawProperty(string key, string value, bool comma = true)
        {
            // Draw item
            var Item = CreateTextLayout(string.Format("\"{0}\" : ", key));
            CTX.DrawTextLayout(Item, X, Y);

            double newX = X + Item.GetSize().Width;

            // Create value
            var Value = new TextLayout();
            Value.Font.WithSize(10);
            Value.Text = value;
            var s = Value.GetSize();
            var rect = new Rectangle(newX, Y, s.Width, s.Height).Inflate(0.2, 0.2);
            CTX.SetColor(Colors.DarkBlue);
            CTX.SetLineWidth(1);
            CTX.Rectangle(rect);
            CTX.Stroke();
            CTX.SetColor(Colors.Blue);
            CTX.Rectangle(rect);
            CTX.Fill();
            CTX.SetColor(Colors.White);
            CTX.DrawTextLayout(Value, newX, Y);
            CTX.SetColor(Colors.Black);

            if (comma)
            {
                var end = CreateTextLayout(",");
                CTX.DrawTextLayout(end, newX + s.Width + 8, Y);
            }
        }

        void DrawText(string data)
        {
            var text = new TextLayout();
            text.Font = this.Font.WithSize(12);
            text.Text = data;
            CTX.DrawTextLayout(text, X, Y);
        }

        void DrawText(string data, Color color, int start, int end)
        {
            var text = new TextLayout();
            text.Font = this.Font.WithSize(10);
            CTX.SetColor(color);
            if (color != null)
            {
                text.SetBackground(color, start, end);
            }
            CTX.SetColor(Colors.Black);
            text.Text = data;
            CTX.DrawTextLayout(text, X, Y);
        }
    }
}
