using Director.DataStructures;
using Director.Forms.Inputs;
using Director.ParserLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;
using Xwt.Formats;

namespace Director.Forms.Controls
{
    internal class ClickableItems
    {
        
    }

    internal class RequestDrawing : Canvas
    {
        /// <summary>
        /// Template.
        /// </summary>
        public String Template { get; set; }
        
        /// <summary>
        /// Json parser.
        /// </summary>
        public Parser JSonParser { get; set; }

        /// <summary>
        /// Clickable items.
        /// </summary>
        public List<ClickableItems> CanvasItems { get; set; }

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
        public RequestDrawing()
        {
            CanvasItems = new List<ClickableItems>();
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
            Dictionary<string, ParserItem> data = JSonParser.deserialize(Template);
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

            foreach (KeyValuePair<string, ParserItem> pair in data)
            {
                // Print key
                String KeyInfo = string.Format("\"{0}\" : ", pair.Key);
                

                if (pair.Value.value is Dictionary<string, ParserItem>)
                {
                    DrawText(KeyInfo+"{");
                    X += 20;
                    NextLine();
                    DrawJson((Dictionary<string, ParserItem>)pair.Value.value);
                    X -= 20;
                    DrawText("}");
                    NextLine();
                }
                else if (pair.Value.value is System.String)
                {
                    DrawProperty(pair.Key, (String) pair.Value.value);
                    NextLine();
                }
                else
                {
                    DrawText(pair.Value.value.GetType().ToString());
                    NextLine();
                }
            }
        }

        TextLayout CreateTextLayout(string text)
        {
            var Item = new TextLayout();
            Item.Text = text;
            Item.Font.WithSize(10);
            return Item;
        }

        void DrawProperty(string key, string value)
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

            var end = CreateTextLayout(",");
            CTX.DrawTextLayout(end, newX + s.Width + 8, Y);
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
            if (color != null) {
                text.SetBackground(color, start, end);
            }
            CTX.SetColor(Colors.Black);
            text.Text = data;
            CTX.DrawTextLayout(text, X, Y);
        }
    }

    internal class RequestWidget : VBox
    {
        /// <summary>
        /// Text view.
        /// </summary>
        public TextEntry TextView { get; set; }

        /// <summary>
        /// Render box
        /// </summary>
        public RequestDrawing RenderBox { get; set; }

        /// <summary>
        /// Active request.
        /// </summary>
        public Request ActiveRequest;

        /// <summary>
        /// Request menu.
        /// </summary>
        private Menu RequestHelperMenu { get; set; }

        /// <summary>
        /// Json parser.
        /// </summary>
        public Parser JsonParser { get; set; }

        /// <summary>
        /// Create widget.
        /// </summary>
        /// <param name="_request"></param>
        public RequestWidget(Request _request)
        {
            // Set request
            ActiveRequest = _request;

            // Set margin
            Margin = 10;

            // Json parser
            JsonParser = new Parser();

            // Create Text view
            RenderBox = new RequestDrawing()
            {
                ExpandHorizontal = true, ExpandVertical = true,
                JSonParser = JsonParser
            };
            PackStart(new Label(Director.Properties.Resources.RequestContent));
            PackStart(RenderBox, expand: true, fill: true);

            // Set template
            RenderBox.Template = ActiveRequest.RequestTemplate;

            // Action items
            RenderBox.ButtonPressed += RenderBox_ButtonPressed;

            // Edit btn
            Button SetContent = new Button(Image.FromResource(DirectorImages.EDIT_CONTENT_ICON), Director.Properties.Resources.EditContent)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            PackStart(SetContent, expand: false, hpos: WidgetPlacement.End);

            // Click events
            SetContent.Clicked += SetContent_Clicked;

            // Request menu helper
            RequestHelperMenu = new Menu();

            MenuItem a = new MenuItem("Test");
            RequestHelperMenu.Items.Add(a);
        }

        void RenderBox_ButtonPressed(object sender, ButtonEventArgs e)
        {
            if (e.Button == PointerButton.Right)
            {
                RequestHelperMenu.Popup(RenderBox, e.X, e.Y);
            }
        }

        /// <summary>
        /// Set request.
        /// </summary>
        public void SetRequest(String request)
        {
            ActiveRequest.RequestTemplate = RenderBox.Template = request;
            RenderBox.QueueDraw();
        }

        /// <summary>
        /// Set content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SetContent_Clicked(object sender, EventArgs e)
        {
            Parent.Sensitive = false;
            SetWindow _window = new SetWindow(this, null);
            _window.Closed += delegate
            {
                Parent.Sensitive = true;
            };
            _window.Show();
        }

        /// <summary>
        /// Text view.
        /// </summary>
        void TextView_Changed(object sender, EventArgs e)
        {
            ActiveRequest.RequestTemplate = TextView.Text;  
        }
    }
}