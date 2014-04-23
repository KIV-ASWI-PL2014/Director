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
    /// Class window.
    /// </summary>
    internal class SetWindow : Window
    {
        /// <summary>
        /// Text input.
        /// </summary>
        public RichTextView TextInput { get; set; }

        /// <summary>
        /// Formated input text.
        /// </summary>
        public Label FormatInput { get; set; }

        /// <summary>
        /// Create set window.
        /// </summary>
        public SetWindow()
        {
            // Set default size
            Width = 450;
            Height = 500;

            // This window can not be maximalized
            Resizable = false;

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Create input area
            VBox InputArea = new VBox();

            // Prepare input
            ScrollView ScrollTextInput = new ScrollView()
            {
                Content = TextInput = new RichTextView()
            };
            InputArea.PackStart(new Label("Add input:"));
            InputArea.PackStart(ScrollTextInput, true, true);

            // Prepare output
            InputArea.PackStart(new Label("Output: "));
            ScrollView ScrollTextOutput = new ScrollView()
            {
                Content = FormatInput = new Label()
            };
            InputArea.PackStart(ScrollTextOutput, true, true);

            // Btn
            Button ConfirmButton = new Button(Image.FromResource(DirectorImages.OK_ICON),
                Director.Properties.Resources.Confirm)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            InputArea.PackStart(ConfirmButton, expand: false, hpos: WidgetPlacement.End);

            // Content is input area
            Content = InputArea;
        }
    }
}