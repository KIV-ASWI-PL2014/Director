using Director.DataStructures;
using Director.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace Director.Forms.Export
{
    /// <summary>
    /// Export window.
    /// </summary>
    class ExportWindow : Window
    {
        /// <summary>
        /// Active server.
        /// </summary>
        private Server ActiveServer { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server"></param>
        public ExportWindow(Server server)
        {
            // Set active server 
            ActiveServer = server;

            /// Title and Initial size
            Title = Director.Properties.Resources.ExportDialog;

            // Set default size
            Width = 450;
            Height = 500;

            // This window can not be maximalized
            Resizable = false;

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Initialize content
            _initializeComponents();
        }

        /// <summary>
        /// Initialize components.
        /// </summary>
        private void _initializeComponents()
        {
            InfoBox _infoBox = new InfoBox("Test", DirectorImages.SERVER_IMAGE);
            VBox _contentBox = new VBox();
            _contentBox.PackStart(_infoBox);
            Content = _contentBox;
        }


    }
}
