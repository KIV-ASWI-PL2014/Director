using Director.ParserLib;
using System;
using System.Collections.Generic;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Inputs
{
    /// <summary>
    /// Clickable response item container!
    /// </summary>
    public struct ClickableResponseItem
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public ParserItem Item { get; set; }
    }

    public class JSONCanvas : Canvas
    {
        /// <summary>
        /// Scroll adjustments!
        /// </summary>
        ScrollAdjustment hscroll;
        ScrollAdjustment vscroll;

		/// <summary>
		/// Draw array.
		/// </summary>
		private bool _drawArray = false;

		/// <summary>
		/// Draw array.
		/// </summary>
		public Boolean DrawArray { 
			get { 
				return _drawArray; 
			} 
			set { 
				_drawArray = value; 
			} 
		}


        /// <summary>
        /// Template.
        /// </summary>
        public Dictionary<string, ParserItem> Template { get; set; }

        /// <summary>
        /// Clickable items.
        /// </summary>
        public List<ClickableResponseItem> CanvasItems { get; set; }

        /// <summary>
        /// Find X and Max X!
        /// </summary>
        public int x, maxX;
        public int X { 
            get { 
                return x;
            }
            set
            {
                if (value > maxX) maxX = value;
                x = value;
            }
        }
        public int Y { get; set; }

        /// <summary>
        /// Active context.
        /// </summary>
        public Context CTX { get; set; }

        /// <summary>
        /// Image size;
        /// </summary>
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }

        /// <summary>
        /// Draw operational?
        /// </summary>
        public bool CanDraw { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public JSONCanvas()
        {
            CanvasItems = new List<ClickableResponseItem>();
            ImageHeight = ImageWidth = 0;
        }

        /// <summary>
        /// Clear values.
        /// </summary>
        public void ClearValues()
        {
            maxX = 0;
            Y = 0;
            X = 20;
            CanvasItems.Clear();
        }

        /// <summary>
        /// Right.
        /// </summary>
        public void Right()
        {
            X += 20;
        }

        /// <summary>
        /// Left.
        /// </summary>
        public void Left()
        {
            X -= 20;
        }

        /// <summary>
        /// Next line.
        /// </summary>
        void NextLine()
        {
            Y += 18;
        }

        /// <summary>
        /// Draw text.
        /// </summary>
        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            // If no template dont draw!
            if (Template == null)
                return;

            // Stop drawing (count size for request)
            CanDraw = false;

            // Clear
            ClearValues();
           
            // Set Context
            CTX = ctx;

            // Data set
            DrawText("{");
            NextLine();
            Right();
            DrawJson(Template);
            Left();
            DrawText("}");

            // Set image size - scrollbars!
            if (hscroll != null && vscroll != null && (hscroll.UpperValue != maxX || vscroll.UpperValue != Y+20))
            {
                hscroll.UpperValue = maxX;
                vscroll.UpperValue = Y + 20;
            }

            // Start drawing
            CanDraw = true;

            // Clear and draw again! - translate context!
            if (hscroll != null && vscroll != null)
                ctx.Translate(-hscroll.Value, -vscroll.Value);

            // Clear values!
            ClearValues();

            // Data set
            DrawText("{");
            NextLine();
            Right();
            DrawJson(Template);
            Left();
            DrawText("}");
        }

        /// <summary>
        /// Return item click on!
        /// </summary>
        public ParserItem MouseTargetItemAt(double X, double Y)
        {
            List<ClickableResponseItem> items = CanvasItems.FindAll(n => n.X <= X && n.Y <= Y && n.Height >= Y && n.Width >= X);

            if (items.Count == 0)
            {
                return null;
            }
            else
            {
                return items[0].Item;
            }
        }

        /// <summary>
        /// Create clickable item on target destination!
        /// </summary>
        private void CreateClickableItem(double corX, double corY, double corWidth, double corHeight, ParserItem item)
        {
            if (hscroll != null && vscroll != null)
            {
                corX -= hscroll.Value;
                corY -= vscroll.Value;
            }

            // Not clickable (out of range)
            if (corX < 0 || corY < 0)
                return;

            // Create Item
            CanvasItems.Add(
                new ClickableResponseItem()
                {
                    X = corX, Y = corY, Height = corY + corHeight, Width = corX + corWidth, Item = item
                }
            );
        }


        /// <summary>
        /// Necessary override for scrolling.
        /// </summary>
        protected override bool SupportsCustomScrolling
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Create scroll horizontal and vertical adjustments and redraw if it is necessary!
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        protected override void SetScrollAdjustments(ScrollAdjustment horizontal, ScrollAdjustment vertical)
        {
            hscroll = horizontal;
            vscroll = vertical;

            hscroll.UpperValue = 0;
            hscroll.PageIncrement = Bounds.Width;
            hscroll.PageSize = Bounds.Width;
            hscroll.ValueChanged += delegate
            {
                QueueDraw();
            };

            vscroll.UpperValue = 0;
            vscroll.PageIncrement = Bounds.Height;
            vscroll.PageSize = Bounds.Height;
            vscroll.ValueChanged += delegate
            {
                QueueDraw();
            };
        }

        /// <summary>
        /// If bounds changed, change scrollbars.
        /// </summary>
        protected override void OnBoundsChanged()
        {
            if (vscroll == null || hscroll == null)
                return;
            vscroll.PageSize = vscroll.PageIncrement = Bounds.Height;
            hscroll.PageSize = hscroll.PageIncrement = Bounds.Width;
        }


        /// <summary>
        /// Draw Json classes!
        /// </summary>
        /// <param name="data"></param>
        void DrawJson(Dictionary<string, ParserItem> data)
        {
            int i = data.Keys.Count;
            foreach (KeyValuePair<string, ParserItem> pair in data)
            {
                // Last item?
                i--;

                // Print Items
                DrawParserItem(pair.Key, pair.Value, i != 0);
            }
        }

		public bool DrawVisibleProperty = true;

        // Draw item from parser:
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
					if (!DrawVisibleProperty) {
						DrawText(string.Format("\"{0}\" : \"{1}\"" + comma, key, (String) item.value));
					} else
                    	DrawProperty(key, string.Format("\"{0}\"", (string) item.value), item, comma);
                }
                else
                    DrawText(item.value + commaStr);
            }
            else if (item.value is System.Int64 || item.value is System.Int32 || item.value is System.Double)
            {
                if (key != null)
                {
                    DrawProperty(key, (item.value + "").Replace(',', '.'), item, comma);
                }
                else
                    DrawText(item.value + commaStr);
            }
            else if (item.value is System.Boolean)
            {
                if (key != null)
                { 
                    DrawProperty(key, ((System.Boolean) item.value) ? "true" : "false", item, comma);
                } else
                    DrawText((((System.Boolean) item.value) ? "true" : "false") + commaStr);
            }
            else if (item.value == null)
            {
                if (key != null)
                {
                    DrawProperty(key, "null", item, comma);
                }
                else
                    DrawText("null" + commaStr);
            }
            else if (item.value is List<ParserItem>)
            {
				PrintArray(key, item, (List<ParserItem>)item.value, comma);
            }
            else
            {
                DrawText(item.value.GetType().ToString());
            }
            NextLine();
        }

		private void PrintArray(string key, ParserItem it, List<ParserItem> list, bool comma = false)
        {
            string commaStr = (comma) ? "," : "";
			bool changed = false;
			// PRint item?
			if (DrawArray && DrawVisibleProperty) {
				DrawArrayProperty (key, it);
				DrawVisibleProperty = false;
				changed = true;
			} else if (key != null)
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

			if (changed)
				DrawVisibleProperty = true;
        }

        TextLayout CreateTextLayout(string text)
        {
            var Item = new TextLayout();
            Item.Text = text;
            Item.Font.WithSize(10);
            return Item;
        }

		void DrawArrayProperty(string key, ParserItem it) {
			if (CanDraw) {

				// Create Key Item
				var KeyItem = CreateTextLayout(string.Format("\"{0}\"", key));

				// Create rectangle
				var rect = new Rectangle (X, Y, KeyItem.GetSize().Width, KeyItem.GetSize().Height).Inflate (0.2, 0.2);

				// Clikable item
				CreateClickableItem((double) X, (double)Y, KeyItem.GetSize().Width, KeyItem.GetSize().Height, it);

				// Draw rect
				CTX.SetColor(Colors.DarkGreen);
				CTX.Rectangle(rect);
				CTX.Fill();
				CTX.SetLineWidth(1);
				CTX.Stroke();
				CTX.SetColor(Colors.White);
				CTX.DrawTextLayout(KeyItem, X, Y);
				CTX.SetColor(Colors.Black);

				// Draw [
				double newX = X + KeyItem.GetSize ().Width + 0.2;
				CTX.DrawTextLayout (CreateTextLayout (" : ["), newX, Y);
			}
		}
			
		void DrawProperty(string key, string value, ParserItem it, bool comma = true) {
			DrawColorProperty (key, value, Colors.DarkBlue, it, comma);
		}

		void DrawColorProperty(string key, string value, Color c, ParserItem item, bool comma = true)
        {
            if (CanDraw)
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
                CreateClickableItem((double) newX, (double)Y, s.Width, s.Height, item);
				CTX.SetColor(c);
                CTX.Rectangle(rect);
				CTX.Fill();
				CTX.SetLineWidth(1);
                CTX.Stroke();
                CTX.SetColor(Colors.White);
                CTX.DrawTextLayout(Value, newX, Y);
                CTX.SetColor(Colors.Black);

                if (comma)
                {
                    var end = CreateTextLayout(",");
                    CTX.DrawTextLayout(end, newX + s.Width + 8, Y);
                }
            }
        }

        void DrawText(string data)
        {
            if (CanDraw)
            {
                var text = new TextLayout();
                text.Font = this.Font.WithSize(12);
                text.Text = data;
                CTX.DrawTextLayout(text, X, Y);
            }
        }
    }
}
