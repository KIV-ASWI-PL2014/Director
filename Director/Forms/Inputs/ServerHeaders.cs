using Director.DataStructures;
using Director.Forms.Controls;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Inputs
{
    internal class ServerHeaders : Dialog
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
            Icon = Image.FromResource(DirectorImages.EDIT_CONTENT_ICON);
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

            // Close button
            Buttons.Add(new DialogButton(Director.Properties.Resources.Confirm, Command.Ok));

            // Set content
            Content = MainContent;
        }
    }
}