using Director.DataStructures;
using Director.Forms.Inputs;
using System;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Controls
{
    internal class ResponseWidget : VBox
    {
        /// <summary>
        /// Expected response status code.
        /// </summary>
        private TextEntry ExpectedStatusCode { get; set; }

        /// <summary>
        /// Active request.
        /// </summary>
        public Request ActiveRequest;

        /// <summary>
        /// Render box
        /// </summary>
        public JSONCanvas RenderBox { get; set; }

        /// <summary>
        /// Request menu.
        /// </summary>
        private Menu RequestHelperMenu { get; set; }

        /// <summary>
        /// Invalid scenario name description.
        /// </summary>
        private Label InvalidStatusCode = new Label()
        {
            Markup = "<b>" + Director.Properties.Resources.InvalidStatusCode + "</b>",
            Visible = false,
            TextColor = Colors.Red,
            TextAlignment = Alignment.End,
            MarginRight = 10
        };

        /// <summary>
        /// Scroll view.
        /// </summary>
        private ScrollView CanvasScrollView { get; set; }


        public ResponseWidget(Request _request)
        {
            // Set request
            ActiveRequest = _request;

            // Set margin
            Margin = 10;

            // Expected status code
            PackStart(new Label(Director.Properties.Resources.ExpectedStatusCode));
            ExpectedStatusCode = new TextEntry()
            {
                Text = ActiveRequest.ExpectedStatusCode + "",
                ExpandHorizontal = true
            };
            PackStart(ExpectedStatusCode, expand: false, fill: false);
            PackStart(InvalidStatusCode, vpos: WidgetPlacement.End);
            ExpectedStatusCode.Changed += delegate
            {
                var t = ExpectedStatusCode.Text;
                if (t.Length == 0)
                {
                    InvalidStatusCode.Visible = false;
                    ActiveRequest.ExpectedStatusCode = -1;
                    return;
                }

                try
                {
                    int x = int.Parse(t);
                    if (x <= 0)
                        throw new InvalidCastException();

                    ActiveRequest.ExpectedStatusCode = x;
                    InvalidStatusCode.Visible = false;
                }
                catch
                {
                    InvalidStatusCode.Visible = true;
                }
            };

            // Label
            PackStart(new Label() { Markup = "<b>" + Director.Properties.Resources.ResponseContent + ":</b>" });

            // Json canvas
            RenderBox = new JSONCanvas()
            {
                ExpandHorizontal = true,
                ExpandVertical = true,
                Template = _request.ResponseTemplate
            };

            // Create scroll view
            CanvasScrollView = new ScrollView()
            {
                HorizontalScrollPolicy = ScrollPolicy.Automatic,
                VerticalScrollPolicy = ScrollPolicy.Automatic
            };
            PackStart(CanvasScrollView, expand: true, fill: true);
            CanvasScrollView.Content = RenderBox;

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
                RequestHelperMenu.Popup();
        }

        /// <summary>
        /// Set request.
        /// </summary>
        public void SetResponse(String response)
        {
            ActiveRequest.ResponseTemplate = RenderBox.Template = response;
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
            SetWindow _window = new SetWindow(null, this);
            _window.Closed += delegate
            {
                Parent.Sensitive = true;
            };
            _window.Show();
        }
    }
}