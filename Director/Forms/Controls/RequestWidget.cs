using Director.DataStructures;
using Director.Forms.Inputs;
using Director.ParserLib;
using System;
using System.Collections.Generic;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Controls
{
    internal class RequestWidget : VBox
    {
        /// <summary>
        /// Text view.
        /// </summary>
        public TextEntry TextView { get; set; }

        /// <summary>
        /// Render box
        /// </summary>
        public JSONCanvas RenderBox { get; set; }

        /// <summary>
        /// Active request.
        /// </summary>
        public Request ActiveRequest;

        /// <summary>
        /// Request menu.
        /// </summary>
        private Menu RequestHelperMenu { get; set; }

        /// <summary>
        /// Canvas scroll view.
        /// </summary>
        private ScrollView CanvasScrollView { get; set; }

        /// <summary>
        /// Dictionary template!
        /// </summary>
        private Dictionary<string, ParserItem> Template { get; set; }
        

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

            // Create Text view
            PackStart(new Label() { Markup = "<b>" + Director.Properties.Resources.RequestContent + "</b>" });
            
            // Parse Request
            if (_request.RequestTemplate != null)
            {
                List<ParserError> errors = new List<ParserError>();
                Template = Parser.deserialize(_request.RequestTemplate, errors, "template");
            }

            // Create canvas
            RenderBox = new JSONCanvas()
            {
                ExpandHorizontal = true,
                ExpandVertical = true,
                Template = Template
            };

            // Create Scroll view
            CanvasScrollView = new ScrollView()
            {
                HorizontalScrollPolicy = ScrollPolicy.Automatic,
                VerticalScrollPolicy = ScrollPolicy.Automatic
            };
            PackStart(CanvasScrollView, expand: true, fill: true);
            CanvasScrollView.Content = RenderBox;

            // Set template
            RenderBox.Template = Template;

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

        /// <summary>
        /// Mouse button right click on canvas.
        /// </summary>
        void RenderBox_ButtonPressed(object sender, ButtonEventArgs e)
        {
            if (e.Button == PointerButton.Right)
            {
                ParserItem item = RenderBox.MouseTargetItemAt(e.X, e.Y);
                if (item != null)
                {
                    RequestHelperMenu.Items[0].Label = string.Format("{0} - {1}", item.value.GetType().ToString(), item.value.ToString());
                    RequestHelperMenu.Popup();
                }
            }
        }

        /// <summary>
        /// Set request.
        /// </summary>
        public void SetRequest(String requestTemplate)
        {
            ActiveRequest.RequestTemplate = requestTemplate;
            List<ParserError> errors = new List<ParserError>();
            RenderBox.Template = Template = Parser.deserialize(requestTemplate, errors, "template");
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
    }
}