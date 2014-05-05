using Director.DataStructures;
using Director.Forms.Inputs;
using System;
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
            
            // Create canvas
            RenderBox = new JSONCanvas()
            {
                ExpandHorizontal = true,
                ExpandVertical = true,
                Template = _request.RequestTemplate
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