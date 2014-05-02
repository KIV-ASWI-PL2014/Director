using Director.Forms.Controls;
using Director.ParserLib;
using Newtonsoft.Json.Schema;
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
    /// Class window.
    /// </summary>
    internal class SetWindow : Window
    {
        /// <summary>
        /// Text input.
        /// </summary>
		public MultiLineTextEntry TextInput { get; set; }

        /// <summary>
        /// Formated input text.
        /// </summary>
        public Label FormatInput { get; set; }

        /// <summary>
        /// Request widget.
        /// </summary>
        public RequestWidget ReqWidget { get; set; }

        /// <summary>
        /// Error report.
        /// </summary>
        private MarkdownView ErrorReport { get; set; }

        /// <summary>
        /// Response widget.
        /// </summary>
        public ResponseWidget ResWidget { get; set; }

        /// <summary>
        /// Create set window.
        /// </summary>
        public SetWindow(RequestWidget _reqW, ResponseWidget _resP)
        {
            // Set
            ReqWidget = _reqW;
            ResWidget = _resP;

            // Set default size
            Width = 450;
            Height = 500;

            // This window can not be maximalized
            Resizable = true;

            // Set content
            Title = Director.Properties.Resources.SetContentTitle;

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Create input area
            VBox InputArea = new VBox();

            // Prepare input
			TextInput = new MultiLineTextEntry()
            {
                ExpandVertical = true,
                ExpandHorizontal = true
            };
            if (ReqWidget != null)
            {
                TextInput.Text = ReqWidget.ActiveRequest.RequestTemplate;
            }
            else if (ResWidget != null)
            {
                TextInput.Text = ResWidget.ActiveRequest.ResponseTemplate;
            }

            ScrollView ScrollTextInput = new ScrollView()
            {
                Content = TextInput
            };
            InputArea.PackStart(new Label("Paste input:"));
            InputArea.PackStart(ScrollTextInput, true, true);

            // Prepare output
            InputArea.PackStart(new Label("Output: "));
            ErrorReport = new MarkdownView();
            InputArea.PackStart(ErrorReport, true, true);

            // Btn
            Button ConfirmButton = new Button(Image.FromResource(DirectorImages.OK_ICON),
                Director.Properties.Resources.ConfirmInput)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            InputArea.PackStart(ConfirmButton, expand: false, hpos: WidgetPlacement.End);

            // Save
            ConfirmButton.Clicked += ConfirmButton_Clicked;

            // Content is input area
            Content = InputArea;
        }


        /// <summary>
        /// Validate.
        /// </summary>
        void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            Parser p = new Parser();
            ParserResult _results = p.generateRequest(TextInput.Text, new Dictionary<string,string>());
            if (_results.isSuccess())
            {
                if (ReqWidget != null)
                {
                    ReqWidget.SetRequest(TextInput.Text);
                    Close();
                }
                else if (ResWidget != null)
                {
                    ResWidget.SetResponse(TextInput.Text);
                    Close();
                }
                else
                    Close();
            }
            else
            {
                ErrorReport.Markdown = _results.GetMarkdownReport();
            }
        }

    }
}