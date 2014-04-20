using Director.DataStructures;
using Director.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Inputs
{
    /// <summary>
    /// Class edit window for 
    /// </summary>
    class EditWindow : Window
    {
        /// <summary>
        /// Active window.
        /// </summary>
        private MainWindow ActiveWindow { get; set; }

        /// <summary>
        /// Active request.
        /// </summary>
        private Request ActiveRequest { get; set; }

        /// <summary>
        /// Request settings tabs.
        /// </summary>
        public Notebook RequestSettings { get; set; }

		/// <summary>
		/// Invalid scenario name description.
		/// </summary>
		private Label InvalidRequestUrl = new Label()
		{
			Markup = "<b>Invalid request URL!</b>",
			Visible = false,
			TextColor = Colors.Red,
			TextAlignment = Alignment.End,
			MarginRight = 10
		};

        /// <summary>
        /// Create window instance.
        /// </summary>
        /// <param name="_window">Main window</param>
        /// <param name="_request">Active request</param>
        public EditWindow(MainWindow _window, Request _request)
        {
            // Set default size
            Width = 450;
            Height = 500;

            // This window can not be maximalized
			Resizable = true;

			// Set title
			Title = "Request: " + _request.Name;

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Set window and request
            ActiveWindow = _window;
            ActiveRequest = _request;

            // Initialize components
            _initializeComponents();
        }

        private void _initializeComponents()
        { 
            // Parent Vbox
            VBox ParentContent = new VBox();

            // Request URL in frame
            Frame RequestUrl = new Frame()
            {
				Label = "Request URL", 
				Padding = 10,
				HeightRequest = 70
            };
			VBox RequestUrlContent = new VBox ();
			TextEntry RequestUrlField = new TextEntry() {
				Text = ActiveRequest.Url
			};
			RequestUrlContent.PackStart (RequestUrlField, expand: true, fill: true);
			RequestUrlContent.PackStart (InvalidRequestUrl, vpos: WidgetPlacement.End);
			RequestUrl.Content = RequestUrlContent;
			ParentContent.PackStart(RequestUrl, expand: false, fill: true);

			// Change request URL field
			RequestUrlField.Changed += delegate {
				try {
					ActiveRequest.SetUrl(RequestUrlField.Text);
					InvalidRequestUrl.Hide();
				} catch (Exception e) {
					InvalidRequestUrl.Show();
				}
			};

            // Create Notebook
            RequestSettings = new Notebook()
            {
                ExpandHorizontal = true, ExpandVertical = true, TabOrientation = NotebookTabOrientation.Top
            };
            _initializeTabs();
			RequestSettings.CurrentTabChanged += delegate {
				if (RequestSettings.CurrentTab.Child is OverviewWidget)
					((OverviewWidget) RequestSettings.CurrentTab.Child).RefreshOverview();
			};
            ParentContent.PackStart(RequestSettings, true, true);

            // Close btn
            Button ConfirmButton = new Button(Image.FromResource(DirectorImages.OK_ICON), Director.Properties.Resources.Confirm)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            ParentContent.PackStart(ConfirmButton, expand: false, hpos: WidgetPlacement.End);

            // Set content
            Content = ParentContent;
        }

		/// <summary>
		/// Create tabs.
		/// </summary>
        private void _initializeTabs()
        { 
			RequestSettings.Add (new OverviewWidget(ActiveRequest), "Overview");
			RequestSettings.Add (new HeaderList (ActiveRequest.Headers), "Headers");
			RequestSettings.Add (new RequestWidget(ActiveRequest), "Request");
			RequestSettings.Add (new Label ("A"), "Response");
        }
    }

	public class OverviewWidget : VBox
	{
		/// <summary>
		/// Request.
		/// </summary>
		/// <value>The active request.</value>
		private Request ActiveRequest { get; set; }

		/// <summary>
		/// Overview widget.
		/// </summary>
		/// <value>The overview.</value>
		private MarkdownView Overview { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Director.Forms.Inputs.OverviewWidget"/> class.
		/// </summary>
		/// <param name="request">Request.</param>
		public OverviewWidget(Request request)
		{
			// Set active request
			ActiveRequest = request;

			// Set margin
			Margin = 10;
			ExpandHorizontal = true;
			ExpandVertical = true;

			// Create markdown
			Overview = new MarkdownView ();
			PackStart (Overview, expand: true, fill: true);

			// Refresh
			RefreshOverview ();
		}

		/// <summary>
		/// Refresh overview summary.
		/// </summary>
		public void RefreshOverview()
		{
			string text = "";

			// Request method
			text += "### Method\n";
			if (ActiveRequest.HTTP_METHOD == null) {
				text += "* Not set\n";
			} else
				text += "* " + ActiveRequest.HTTP_METHOD + "\n";

			// Headers
			text += "### Headers\n";

			// Set text
			Overview.Markdown = text;
		}
	}

	public class RequestWidget : VBox
	{
		/// <summary>
		/// Type.
		/// </summary>
		/// <value>The type of the request.</value>
		public ComboBox RequestType { get; set; }

		/// <summary>
		/// Method.
		/// </summary>
		/// <value>The request method.</value>
		public ComboBox RequestMethod { get; set; }

		private Request ActiveRequest;

		public RequestWidget(Request _request)
		{
			// Set request
			ActiveRequest = _request;

			// Set margin
			Margin = 10;

			// Set default GET method
			if (ActiveRequest.HTTP_METHOD == null || ActiveRequest.HTTP_METHOD.Length == 0)
				ActiveRequest.HTTP_METHOD = "GET";

			PackStart (new Label ("Type: "));

			RequestMethod = new ComboBox ();
			RequestMethod.Items.Add (1, "GET");
			RequestMethod.Items.Add (2, "POST");
			RequestMethod.Items.Add (3, "PUT");
			try {
				RequestMethod.SelectedText = ActiveRequest.HTTP_METHOD;
			} catch {
				RequestMethod.SelectedText = "GET";
				ActiveRequest.HTTP_METHOD = "GET";
			}
			PackStart (RequestMethod);
		}
	}
}
