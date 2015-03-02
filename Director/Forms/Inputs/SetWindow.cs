using Director.Formatters;
using Director.Forms.Controls;
using Director.ParserLib;
using System;
using System.Collections.Generic;
using Xwt;
using Xwt.Drawing;
using System.Xml;
using System.IO;

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
		public TextEntry TextInput { get; set; }

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
		///  Content type.
		/// </summary>
		/// <value>The type of the content.</value>
		public ComboBox ContentTypeSelect { get; set; }

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
			TextInput = new TextEntry()
            {
                ExpandVertical = true,
                ExpandHorizontal = true,
				MultiLine = true
            };
					
			TextInput.Text = "";

			// Content type combo box
			ContentTypeSelect = new ComboBox ();
			ContentTypeSelect.Items.Add (ContentType.JSON, "JSON");
			ContentTypeSelect.Items.Add (ContentType.XML, "XML");
			ContentTypeSelect.Items.Add (ContentType.PLAIN, "PLAIN");
			ContentTypeSelect.SelectedIndex = 0;

            if (ReqWidget != null)
            {
				if (ReqWidget.ActiveRequest.RequestTemplateType == ContentType.JSON) {
					TextInput.Text = JSONFormatter.Format (ReqWidget.ActiveRequest.RequestTemplate);
				}
				if (TextInput.Text.Length == 0) {
					TextInput.Text = ReqWidget.ActiveRequest.RequestTemplate;
				}
				ContentTypeSelect.SelectedItem = ReqWidget.ActiveRequest.RequestTemplateType;
            }
            else if (ResWidget != null)
            {
				if (ResWidget.ActiveRequest.ResponseTemplateType == ContentType.JSON) {
					TextInput.Text = JSONFormatter.Format (ResWidget.ActiveRequest.ResponseTemplate);
				}
				if (TextInput.Text.Length == 0) {
					TextInput.Text = ResWidget.ActiveRequest.ResponseTemplate;
				}
				ContentTypeSelect.SelectedItem = ResWidget.ActiveRequest.ResponseTemplateType;
			}

			// Add
			InputArea.PackStart(new Label() { Markup = "<b>Content type:</b>" });
			InputArea.PackStart (ContentTypeSelect, false, true);


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
			// Type
			ContentType type = (ContentType) ContentTypeSelect.SelectedItem;

			// Data
			String error = null;

			// Parse
			if (type == ContentType.JSON) {
				List<ParserError> errors = new List<ParserError>();
				Parser.deserialize(TextInput.Text, errors, "json");
				if (errors.Count > 0)
					error = ParserResult.CreateMarkdownReport(errors);

			} else if (type == ContentType.XML) {
				try {
					using (XmlReader reader = XmlReader.Create(new StringReader(TextInput.Text)))
					{
						// OK
					}
				} catch (Exception er) {
					error = er.Message;
				}
			}

			if (error != null) {
				ErrorReport.Markdown = error;
				return;
			}

			if (ReqWidget != null) {
				ReqWidget.SetRequest (TextInput.Text, type);
			} else {
				ResWidget.SetResponse (TextInput.Text, type);
			}

			Close ();
        }

    }
}