using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Controls;
using Director.DataStructures;
using Xwt.Drawing;

namespace Director.Forms.Panels
{
	/// <summary>
	/// Request description.
	/// </summary>
    class RequestPage : PanelBase
	{

        /// <summary>
        /// Current request.
        /// </summary>
        private Request ActiveRequest { get; set; }

        /// <summary>
        /// Request name.
        /// </summary>
        private TextEntry RequestName { get; set; }

        /// <summary>
        /// Invalid request name description.
        /// </summary>
        private Label InvalidRequestName = new Label()
        {
            Markup = "<b>" + Director.Properties.Resources.InvalidRequestName + "</b>",
            Visible = false,
            TextColor = Colors.Red,
            TextAlignment = Alignment.End,
            MarginRight = 10
        };


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="_window"></param>
        public RequestPage(MainWindow _window)
            : base(_window, Director.Properties.Resources.RequestInfoBox, DirectorImages.REQUEST_IMAGE)
		{

		}

        /// <summary>
        /// Set request.
        /// </summary>
        /// <param name="request"></param>
        public void SetRequest(Request request)
        {
            ActiveRequest = request;
            RequestName.Changed -= RequestName_Changed;
            RequestName.Text = request.Name;
            RequestName.Changed += RequestName_Changed;
        }

        /// <summary>
        /// Initialize other components.
        /// </summary>
        public override void _initializeComponents()
        {
            // Frame Request Name
            Frame RequestNameFrame = new Frame()
            {
                Label = Director.Properties.Resources.RequestSettings,
                Padding = 10
            };
            VBox RequestNameBox = new VBox();
            RequestNameBox.PackStart(new Label(Director.Properties.Resources.RequestName));
            RequestName = new TextEntry()
            {
                ExpandHorizontal = true
            };
            RequestName.Changed += RequestName_Changed;
            RequestNameBox.PackStart(RequestName);
            RequestNameBox.PackStart(InvalidRequestName);
            RequestNameFrame.Content = RequestNameBox;
            PackStart(RequestNameFrame);

            // Initialize HPaned for Request Response
            VBox RRPanel = new VBox();

            // Request
            Frame RequestFrame = new Frame()
            {
                Label = Director.Properties.Resources.RequestRequest,
                Padding = 10
            };
            RequestFrame.Content = new RichTextView()
            {
                ExpandHorizontal = true, ExpandVertical = true
            };
            RRPanel.PackStart(RequestFrame, true, true);

            // Response
            Frame ResponseFrame = new Frame()
            {
                Label = Director.Properties.Resources.RequestResponse,
                Padding = 10
            };
            ResponseFrame.Content = new RichTextView()
            {
                ExpandHorizontal = true,
                ExpandVertical = true
            };
            RRPanel.PackStart(ResponseFrame, true, true);
            PackStart(RRPanel, true, true);
        }

        /// <summary>
        /// Request name change event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RequestName_Changed(object sender, EventArgs e)
        {
            ActiveRequest.Name = RequestName.Text;
            bool invalid = RequestName.Text.Length == 0;
            InvalidRequestName.Visible = invalid;
            if (invalid)
                RequestName.SetFocus();
            CurrentMainWindow.UpdateTreeStoreText(ActualPosition, ActiveRequest.Name);
        }
    }
}