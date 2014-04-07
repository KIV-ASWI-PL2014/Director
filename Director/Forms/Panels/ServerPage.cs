using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Controls;
using Director.DataStructures;
using Xwt.Drawing;

namespace Director.Forms.Panels
{
    /// <summary>
    /// Server description.
    /// </summary>
    class ServerPage : VBox
    {
		/// <summary>
		/// Current server instance.
		/// </summary>
		/// <value>The active server.</value>
		private Server ActiveServer { get; set; }

        /// <summary>
        /// Current server name entry.
        /// </summary>
        private TextEntry ServerName { get; set; }

        private ComboBox FrequencyRunning { get; set; }

        /// <summary>
        /// Invalid scenario name description.
        /// </summary>
        private Label InvalidServerName = new Label()
        {
            Markup = "<b>" + Director.Locales.Language.InvalidServerName + "</b>",
            Visible = false,
            TextColor = Colors.Red,
            TextAlignment = Alignment.End,
            MarginRight = 10
        };

        /// <summary>
        /// Invalid server URL error message.
        /// </summary>
        private Label InvalidServerURL = new Label()
        {
            Markup = "<b>" + Director.Locales.Language.InvalidServerURL + "</b>",
            Visible = false,
            TextColor = Colors.Red,
            TextAlignment = Alignment.End,
            MarginRight = 10
        };

        /// <summary>
        /// Server URL text entry.
        /// </summary>
        private TextEntry ServerURL { get; set; }

        /// <summary>
        /// Create server page.
        /// </summary>
		public ServerPage()
		{
			InfoBox _infoBox = new InfoBox(Director.Locales.Language.ServerInfoBox, DirectorImages.SERVER_IMAGE);
			PackStart(_infoBox);
			MarginLeft = 10;
            _initializeComponents();
		}

		/// <summary>
		/// Set server instance.
		/// </summary>
		/// <param name="server">Server.</param>
		public void SetServer(Server server)
		{
			// Fill columns
			ActiveServer = server;
            ServerName.Text = server.Name;
            ServerURL.Text = server.GetUrl();
		}

        /// <summary>
        /// Initialize window.
        /// </summary>
        private void _initializeComponents()
        { 
            // Server Name + URL + Periodicity window
            Frame f = new Frame();
            f.Label = Director.Locales.Language.ServerSettings;
            f.Padding = 10;

            // Create VBOX
            VBox ServerSettings = new VBox();
            
            // Prepare text box
            ServerSettings.PackStart(new Label()
            {
                Text = Director.Locales.Language.ServerName
            });
            ServerName = new TextEntry();
            ServerName.Changed += ServerName_Changed;
            ServerSettings.PackStart(ServerName);

            // Add invalid server name
            ServerSettings.PackStart(InvalidServerName);

            // Server URL
            ServerSettings.PackStart(new Label()
            {
                Text = Director.Locales.Language.ServerURL
            });
            ServerURL = new TextEntry();
            ServerURL.Changed += ServerURL_Changed;
            ServerSettings.PackStart(ServerURL);

            // Invalid URL
            ServerSettings.PackStart(InvalidServerURL);

            // Frequency settings
            ServerSettings.PackStart(new Label()
            {
                Text = Director.Locales.Language.RunningPeriodicity
            });
            FrequencyRunning = new ComboBox();
            FrequencyRunning.Items.Add("Every 5 minutes");
            FrequencyRunning.Items.Add("Every 10 minutes");
            FrequencyRunning.Items.Add("Every 30 minutes");
            FrequencyRunning.Items.Add("Every hour");
            FrequencyRunning.Items.Add("Every 6 hours");
            FrequencyRunning.Items.Add("Every 12 hours");
            FrequencyRunning.Items.Add("Every day");
            FrequencyRunning.SelectedIndex = 5;
            ServerSettings.PackStart(FrequencyRunning);

            // Add Frame to server settings
            f.Content = ServerSettings;
            PackStart(f);
        }

        /// <summary>
        /// Handler URL server was changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServerURL_Changed(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Set server name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServerName_Changed(object sender, EventArgs e)
        {
            ActiveServer.Name = ServerName.Text;
            bool invalid = ServerName.Text.Length == 0;
            InvalidServerName.Visible = invalid;
            if (invalid)
                ServerName.SetFocus();
        }
    }
}
