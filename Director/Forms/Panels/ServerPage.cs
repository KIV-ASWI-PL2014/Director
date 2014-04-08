using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Controls;
using Director.DataStructures;
using Xwt.Drawing;
using Director.DataStructures.SupportStructures;

namespace Director.Forms.Panels
{
    /// <summary>
    /// Server description.
    /// </summary>
    class ServerPage : PanelBase
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

        /// <summary>
        /// Frequency.
        /// </summary>
        private ComboBox FrequencyRunning { get; set; }

        /// <summary>
        /// Authorization.
        /// </summary>
        private CheckBox AuthRequired { get; set; }

        /// <summary>
        /// User name for auth.
        /// </summary>
        private TextEntry AuthUserName { get; set; }

        /// <summary>
        /// User password.
        /// </summary>
        private PasswordEntry AuthUserPassword { get; set; }

        /// <summary>
        /// Authentication frame.
        /// </summary>
        private Frame Authentication { get; set; }

        private ListView EmailNotifications { get; set; }

        private DataField<string> EmailAddress { get; set; }
        private DataField<bool> Errors { get; set; }
        private DataField<bool> Notifications { get; set; }
        private ListStore EmailAddressStore { get; set; }

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
        public ServerPage(MainWindow _mainWindow) : base(_mainWindow, Director.Locales.Language.ServerInfoBox, DirectorImages.SERVER_IMAGE)
		{

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
            AuthUserName.Text = server.AuthName;
            AuthUserPassword.Password = server.AuthPassword;
            AuthRequired.State = (server.Authentication) ? CheckBoxState.On : CheckBoxState.Off;
            
            // Refresh data
            AuthRequired_Toggled(null, null);

            EmailAddressStore.Clear();

            // Emails
            foreach (Email e in server.Emails) {
                var i = EmailAddressStore.AddRow();
                EmailAddressStore.SetValue(i, EmailAddress, e.UserEmail);
                EmailAddressStore.SetValue(i, Errors, e.Errors);
                EmailAddressStore.SetValue(i, Notifications, e.Notifications);
            }
		}

        /// <summary>
        /// Initialize window.
        /// </summary>
        public override void _initializeComponents()
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


            // Authorization
            AuthRequired = new CheckBox(Director.Locales.Language.Authorization);
            AuthRequired.MarginLeft = 10;
            PackStart(AuthRequired);

            // Create Authentication Frame
            Authentication = new Frame()
            {
                Label =  Director.Locales.Language.AuthorizationSettings,
                Padding = 10
            };

            // Login and Password fields
            VBox AuthBox = new VBox();

            AuthBox.PackStart(new Label()
            {
                Text = Director.Locales.Language.Username
            });
            AuthUserName = new TextEntry();
            AuthUserName.Changed += AuthUserName_Changed;
            AuthBox.PackStart(AuthUserName);

            AuthBox.PackStart(new Label()
            {
                Text = Director.Locales.Language.Password
            });
            AuthUserPassword = new PasswordEntry();
            AuthUserPassword.Changed += AuthUserPassword_Changed;
            AuthBox.PackStart(AuthUserPassword);

            // Authentication content
            Authentication.Content = AuthBox;
            PackStart(Authentication);

            // Change value
            AuthRequired.Toggled += AuthRequired_Toggled;


            // Email settings
            Frame EmailFrame = new Frame()
            {
                Label = Director.Locales.Language.EmailNotifications,
                Padding = 10,
                MinHeight = 180
            };

            // Create containers
            EmailAddress = new DataField<string>();
            Notifications = new DataField<bool>();
            Errors = new DataField<bool>();

            // Big container
            EmailAddressStore = new ListStore(EmailAddress, Notifications, Errors);

            // New store
            EmailNotifications = new ListView();
            EmailNotifications.DataSource = EmailAddressStore;

            // Create columns
            EmailNotifications.Columns.Add(
                new ListViewColumn(
                    Director.Locales.Language.Email,
                    new TextCellView { Editable = true, TextField = EmailAddress }
                )
            );
            EmailNotifications.Columns.Add(
                new ListViewColumn(
                    Director.Locales.Language.Errors,
                    new CheckBoxCellView { Editable = true, ActiveField = Errors }
                )
            );
            EmailNotifications.Columns.Add(
                new ListViewColumn(
                    Director.Locales.Language.Notification,
                    new CheckBoxCellView { Editable = true, ActiveField = Notifications }
                )
            );
            EmailFrame.Content = EmailNotifications;
            PackStart(EmailFrame, expand: true, fill: true);
        }

        /// <summary>
        /// Toggle auth required checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AuthRequired_Toggled(object sender, EventArgs e)
        {
            bool active = (AuthRequired.State == CheckBoxState.On);
            ActiveServer.Authentication = active;
            Authentication.Sensitive = active;
        }

        /// <summary>
        /// Auth user password change save!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AuthUserPassword_Changed(object sender, EventArgs e)
        {
            ActiveServer.AuthPassword = AuthUserPassword.Password; 
        }

        /// <summary>
        /// Save auth user name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AuthUserName_Changed(object sender, EventArgs e)
        {
            ActiveServer.AuthName = AuthUserName.Text;
        }

        /// <summary>
        /// Handler URL server was changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServerURL_Changed(object sender, EventArgs e)
        {
            try
            {
                ActiveServer.SetUrl(ServerURL.Text); 
                InvalidServerURL.Visible = false;
            }
            catch (Exception ex)
            {
                InvalidServerURL.Visible = true;
            }
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
