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
            Resizable = false;

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
                Label = "Request URL"
            };
            ParentContent.PackStart(RequestUrl, true, true);

            // Create Notebook
            RequestSettings = new Notebook()
            {
                ExpandHorizontal = true, ExpandVertical = true, TabOrientation = NotebookTabOrientation.Top
            };
            _initializeTabs();
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

        private void _initializeTabs()
        { 
        
        }
    }
}
