using Director.Formatters;
using Director.Forms.Controls;
using Director.ParserLib;
using System;
using System.Collections.Generic;
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

            // Icon
            Icon = Image.FromResource(DirectorImages.EDIT_ICON);

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
                TextInput.Text = JSONFormatter.Format(ReqWidget.ActiveRequest.RequestTemplate);
            }
            else if (ResWidget != null)
            {
                TextInput.Text = JSONFormatter.Format(ResWidget.ActiveRequest.ResponseTemplate);
            }

            ScrollView ScrollTextInput = new ScrollView()
            {
                Content = TextInput
            };
            InputArea.PackStart(new Label() { Markup = "<b>" + Director.Properties.Resources.PasteInput + "</b>" });
            InputArea.PackStart(ScrollTextInput, true, true);

            // Prepare output
            InputArea.PackStart(new Label() { Markup = "<b>" + Director.Properties.Resources.Output + "</b>" });
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
            List<ParserError> errors = new List<ParserError>();
            Dictionary<string, ParserItem> result = Parser.deserialize(TextInput.Text, errors, "json");
            if (errors.Count == 0)
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
                ErrorReport.Markdown = ParserResult.CreateMarkdownReport(errors);
            }
        }

    }
}