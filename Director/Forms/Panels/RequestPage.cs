using System;
using Xwt;
using Director.Forms.Controls;
using Director.DataStructures;
using Xwt.Drawing;

namespace Director.Forms.Panels
{
    /// <summary>
    /// Request description.
    /// </summary>
    internal class RequestPage : PanelBase
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
        /// Request overview.
        /// </summary>
        private VBox RequestOverview { get; set; }

        /// <summary>
        /// Request status - response.
        /// </summary>
        private VBox RequestStatus { get; set; }

        /// <summary>
        /// Request details.
        /// </summary>
        private Notebook RequestDetails { get; set; }

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
        public RequestPage(MainWindow _window)
            : base(_window, Director.Properties.Resources.RequestInfoBox, DirectorImages.REQUEST_IMAGE)
        {
            CaptionFont = Font.WithSize(17).WithWeight(FontWeight.Bold);
        }

        /// <summary>
        /// Caption font.
        /// </summary>
        public Font CaptionFont { get; set; }

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
            ActiveRequest.CreateResult(RequestStatus, CaptionFont);
            ActiveRequest.CreateOverview(RequestOverview, CaptionFont);
            
        }

        /// <summary>
        /// Initialize other components.
        /// </summary>
        public override void _initializeComponents()
        {
            ExpandVertical = false;
            ExpandHorizontal = false;

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

            // Init notebook
            RequestDetails = new Notebook()
            {
                ExpandHorizontal = true,
                ExpandVertical = true,
                TabOrientation = NotebookTabOrientation.Top
            };

            // Prepare tabs
            PackStart(RequestDetails, true, true);

            // Request overview
            RequestOverview = new VBox();
            ScrollView RequestOverviewSV = new ScrollView()
            {
				Content = RequestOverview, Margin = (Config.Windows()) ? 0 : 10
            };
            RequestDetails.Add(RequestOverviewSV, Director.Properties.Resources.RequestRequest);

            // Response overview
            RequestStatus = new VBox();
            ScrollView RequestStatusSV = new ScrollView()
            {
				Content = RequestStatus, Margin = (Config.Windows()) ? 0 : 10
            };
            RequestDetails.Add(RequestStatusSV, Director.Properties.Resources.RequestResponse);

            // Add edit button
            Button EditBtn = new Button(Image.FromResource(DirectorImages.EDIT_ICON),
                Director.Properties.Resources.MenuEditRequest)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            EditBtn.Clicked += delegate { CurrentMainWindow.OpenEditRequest(ActiveRequest); };
            PackStart(EditBtn, expand: false, hpos: WidgetPlacement.End);
        }

        /// <summary>
        /// Request name change event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestName_Changed(object sender, EventArgs e)
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