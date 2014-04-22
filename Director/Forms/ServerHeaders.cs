using Director.DataStructures;
using Director.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms
{
    class ServerHeaders : Window
    {
        /// <summary>
        /// Main window.
        /// </summary>
        private MainWindow ActiveWindow { get; set; }

        /// <summary>
        /// Server for edit.
        /// </summary>
        private Server ActiveServer { get; set; }

        /// <summary>
        /// Header list control.
        /// </summary>
        private HeaderList HeaderControl { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ServerHeaders(MainWindow _window, Server _currentServer)
        {
            ActiveWindow = _window;
            ActiveServer = _currentServer;
            Width = 500;
            Height = 550;
            Resizable = true;
            Title = Director.Properties.Resources.DefaultServerHeaders;
            _initializeComponents();
        }

        /// <summary>
        /// Init components.
        /// </summary>
        private void _initializeComponents()
        {
            VBox MainContent = new VBox()
            {
                Margin = 0
            };

            // Info window
            InfoBox MainInfoBox = new InfoBox(Director.Properties.Resources.DefaultServerHeaders, DirectorImages.SETTINGS_IMAGE);
            MainContent.PackStart(MainInfoBox);

            // Add header list
            HeaderControl = new HeaderList(ActiveServer.DefaultHeaders)
            {
                BackgroundColor = Colors.White
            };
            MainContent.PackStart(HeaderControl, expand: true, fill: true);

            // Add button
            // Close btn
            Button ConfirmButton = new Button(Image.FromResource(DirectorImages.OK_ICON), Director.Properties.Resources.Confirm)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            ConfirmButton.Clicked += delegate
            {
                Close();
            };
            MainContent.PackStart(ConfirmButton, expand: false, hpos: WidgetPlacement.End);

            Content = MainContent;
        }

    }
}
