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
    internal class EditWindow : Window
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
        /// Request method.
        /// </summary>
        public ComboBox RequestHttpMethod { get; set; }

        /// <summary>
        /// Invalid scenario name description.
        /// </summary>
        private Label InvalidRequestUrl = new Label()
        {
            Markup = "<b>" + Director.Properties.Resources.InvalidServerURL + "</b>",
            Visible = false,
            TextColor = Colors.Red,
            TextAlignment = Alignment.End,
            MarginRight = 10
        };

        /// <summary>
        /// Invalid waiting time.
        /// </summary>
        private Label InvalidTime = new Label()
        {
            Markup = "<b>" + Director.Properties.Resources.InvalidWaitTime + "</b>",
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
            Width = 790;
            Height = 750;

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
                Label = Director.Properties.Resources.RequestInfoBox,
                Padding = 10
            };
            VBox RequestUrlContent = new VBox();
            RequestUrlContent.PackStart(new Label(Director.Properties.Resources.RequestUrl));
            TextEntry RequestUrlField = new TextEntry()
            {
                Text = ActiveRequest.Url
            };
            RequestUrlContent.PackStart(RequestUrlField, expand: true, fill: true);
            RequestUrlContent.PackStart(InvalidRequestUrl, vpos: WidgetPlacement.End);

            // Method
            RequestUrlContent.PackStart(new Label(Director.Properties.Resources.RequestMethod));
            RequestHttpMethod = new ComboBox();
            RequestHttpMethod.Items.Add(1, "GET");
            RequestHttpMethod.Items.Add(2, "HEAD");
            RequestHttpMethod.Items.Add(3, "POST");
            RequestHttpMethod.Items.Add(4, "PUT");
            RequestHttpMethod.Items.Add(5, "PATCH");
            RequestHttpMethod.Items.Add(6, "DELETE");
            try
            {
                RequestHttpMethod.SelectedText = ActiveRequest.HTTP_METHOD;
            }
            catch (Exception e)
            {
                RequestHttpMethod.SelectedText = ActiveRequest.HTTP_METHOD = "GET";
            }
            RequestUrlContent.PackStart(RequestHttpMethod);
            RequestHttpMethod.SelectionChanged += delegate
            {
                ActiveRequest.HTTP_METHOD = RequestHttpMethod.SelectedText;
                if (RequestSettings.CurrentTab.Child is OverviewWidget)
                    ((OverviewWidget) RequestSettings.CurrentTab.Child).RefreshOverview();
            };

            // Wait in seconds
            RequestUrlContent.PackStart(new Label(Director.Properties.Resources.WaitPreviousRequest));
            TextEntry WaitTime = new TextEntry()
            {
                Text = ActiveRequest.WaitAfterPreviousRequest + ""
            };
            RequestUrlContent.PackStart(WaitTime, expand: true, fill: true);
            RequestUrlContent.PackStart(InvalidTime, vpos: WidgetPlacement.End);
            WaitTime.Changed += delegate
            {
                try
                {
                    ActiveRequest.WaitAfterPreviousRequest = int.Parse(WaitTime.Text);

                    if (ActiveRequest.WaitAfterPreviousRequest < 0)
                    {
                        ActiveRequest.WaitAfterPreviousRequest = 0;
                        WaitTime.Text = "0";
                        throw new InvalidCastException();
                    }

                    InvalidTime.Visible = false;
                }
                catch
                {
                    InvalidTime.Visible = true;
                }
            };


            RequestUrl.Content = RequestUrlContent;
            ParentContent.PackStart(RequestUrl, expand: false, fill: true);

            // Change request URL field
            RequestUrlField.Changed += delegate
            {
                try
                {
                    ActiveRequest.SetUrl(RequestUrlField.Text);
                    InvalidRequestUrl.Hide();
                }
                catch (Exception e)
                {
                    InvalidRequestUrl.Show();
                }
            };

            // Create Notebook
            RequestSettings = new Notebook()
            {
                ExpandHorizontal = true,
                ExpandVertical = true,
                TabOrientation = NotebookTabOrientation.Top
            };
            _initializeTabs();
            RequestSettings.CurrentTabChanged += delegate
            {
                if (RequestSettings.CurrentTab.Child is OverviewWidget)
                    ((OverviewWidget) RequestSettings.CurrentTab.Child).RefreshOverview();
            };
            ParentContent.PackStart(RequestSettings, true, true);

            // Close btn
            Button ConfirmButton = new Button(Image.FromResource(DirectorImages.OK_ICON),
                Director.Properties.Resources.Confirm)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            ConfirmButton.Clicked += delegate { Close(); };
            ParentContent.PackStart(ConfirmButton, expand: false, hpos: WidgetPlacement.End);

            // Set content
            Content = ParentContent;
        }

        /// <summary>
        /// Create tabs.
        /// </summary>
        private void _initializeTabs()
        {
            RequestSettings.Add(new OverviewWidget(ActiveRequest), Director.Properties.Resources.RequestOverview);
            RequestSettings.Add(new HeaderList(ActiveRequest.Headers), Director.Properties.Resources.RequestHeaders);
            RequestSettings.Add(new FileList(ActiveRequest.Files), Director.Properties.Resources.RequestFiles);
            RequestSettings.Add(new RequestWidget(ActiveRequest), Director.Properties.Resources.RequestTab);
            RequestSettings.Add(new ResponseWidget(ActiveRequest), Director.Properties.Resources.RequestResponse);
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
        /// Scroll overview.
        /// </summary>
        private ScrollView ScrollOverview { get; set; }

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
            Overview = new MarkdownView();
            ScrollOverview = new ScrollView()
            {
                Content = Overview
            };
            PackStart(ScrollOverview, expand: true, fill: true);

            // Refresh
            RefreshOverview();
        }

        /// <summary>
        /// Refresh overview summary.
        /// </summary>
        public void RefreshOverview()
        {
            Overview.Markdown = ActiveRequest.GetMarkdownInfo();
        }
    }
}