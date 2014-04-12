﻿using System;
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
		/// Context menu for Add/remove Email notification adresses!
		/// </summary>
		/// <value>The email contenxt menu.</value>
		private Menu EmailContextMenu { get; set; }

        /// <summary>
        /// Invalid scenario name description.
        /// </summary>
        private Label InvalidServerName = new Label()
        {
            Markup = "<b>" + Director.Properties.Resources.InvalidServerName + "</b>",
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
            Markup = "<b>" + Director.Properties.Resources.InvalidServerURL + "</b>",
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
        public ServerPage(MainWindow _mainWindow)
            : base(_mainWindow, Director.Properties.Resources.ServerInfoBox, DirectorImages.SERVER_IMAGE)
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

            // SKip change event
            ServerName.Changed -= ServerName_Changed;
            ServerName.Text = server.Name;
            ServerName.Changed += ServerName_Changed;
            ServerURL.Text = server.GetUrl();
            AuthUserName.Text = server.AuthName;
            AuthUserPassword.Password = server.AuthPassword;
            AuthRequired.State = (server.Authentication) ? CheckBoxState.On : CheckBoxState.Off;
            
            // Set frequency running
            if (server.RunningFrequency >= 0)
                FrequencyRunning.SelectedIndex = server.RunningFrequency;

            // Refresh data
            AuthRequired_Toggled(null, null);

			// Refresh data in server emails
			RefreshServerEmails ();
		}

        /// <summary>
        /// Initialize window.
        /// </summary>
        public override void _initializeComponents()
        { 
            // Server Name + URL + Periodicity window
            Frame f = new Frame();
            f.Label = Director.Properties.Resources.ServerSettings;
            f.Padding = 10;

            // Create VBOX
            VBox ServerSettings = new VBox();
            
            // Prepare text box
            ServerSettings.PackStart(new Label()
            {
                Text = Director.Properties.Resources.ServerName
            });
            ServerName = new TextEntry();
            ServerName.Changed += ServerName_Changed;
            ServerSettings.PackStart(ServerName);

            // Add invalid server name
            ServerSettings.PackStart(InvalidServerName);

            // Server URL
            ServerSettings.PackStart(new Label()
            {
                Text = Director.Properties.Resources.ServerURL
            });
            ServerURL = new TextEntry();
            ServerURL.Changed += ServerURL_Changed;
            ServerSettings.PackStart(ServerURL);

            // Invalid URL
            ServerSettings.PackStart(InvalidServerURL);

            // Frequency settings
            ServerSettings.PackStart(new Label()
            {
                Text = Director.Properties.Resources.RunningPeriodicity
            });
            FrequencyRunning = new ComboBox();
            FrequencyHelper.FillComboBox(FrequencyRunning);
            ServerSettings.PackStart(FrequencyRunning);
            FrequencyRunning.SelectedIndex = 0;
            FrequencyRunning.SelectionChanged += FrequencyRunning_SelectionChanged;

            // Add Frame to server settings
            f.Content = ServerSettings;
            PackStart(f);

            // Authorization
            AuthRequired = new CheckBox(Director.Properties.Resources.Authorization);
            AuthRequired.MarginLeft = 10;
            PackStart(AuthRequired);

            // Create Authentication Frame
            Authentication = new Frame()
            {
                Label = Director.Properties.Resources.AuthorizationSettings,
                Padding = 10
            };

            // Login and Password fields
            VBox AuthBox = new VBox();

            AuthBox.PackStart(new Label()
            {
                Text = Director.Properties.Resources.Username
            });
            AuthUserName = new TextEntry();
            AuthUserName.Changed += AuthUserName_Changed;
            AuthBox.PackStart(AuthUserName);

            AuthBox.PackStart(new Label()
            {
                Text = Director.Properties.Resources.Password
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
                Label = Director.Properties.Resources.EmailNotifications,
                Padding = 10,
                MinHeight = 180
            };

            // Create containers
            EmailAddress = new DataField<string>();
            Notifications = new DataField<bool>();
            Errors = new DataField<bool>();

            // Big container
			EmailAddressStore = new ListStore (EmailAddress, Notifications, Errors);

            // New store
			EmailNotifications = new ListView () {
				DataSource = EmailAddressStore,
				SelectionMode = SelectionMode.Single
			};

			// RIght click menu for emails
			EmailContextMenu = new Menu ();
			MenuItem EditEmail = new MenuItem ("Edit email") {
				Image = Image.FromResource(DirectorImages.ADD_ICON)
			};
			MenuItem RemoveEmail = new MenuItem ("Remove email") {
				Image = Image.FromResource (DirectorImages.CROSS_ICON)
			};
			RemoveEmail.Clicked += RemoveEmail_Clicked;
			EmailContextMenu.Items.Add (EditEmail);
			EmailContextMenu.Items.Add (new SeparatorMenuItem ());
			EmailContextMenu.Items.Add (RemoveEmail);
			EmailNotifications.ButtonPressed += EmailNotifications_Clicked;

            // Create columns
            EmailNotifications.Columns.Add(
                new ListViewColumn(
                    Director.Properties.Resources.Email,
					new TextCellView { Editable = false, TextField = EmailAddress }
                )
            );
            EmailNotifications.Columns.Add(
                new ListViewColumn(
                    Director.Properties.Resources.Errors,
                    new CheckBoxCellView { Editable = true, ActiveField = Errors }
                )
            );
            EmailNotifications.Columns.Add(
                new ListViewColumn(
                    Director.Properties.Resources.Notification,
                    new CheckBoxCellView { Editable = true, ActiveField = Notifications }
                )
            );
			VBox EmailFrameContent = new VBox ();
			EmailFrameContent.PackStart (EmailNotifications, expand: true, fill: true);

			// New email address
			Button AddEmail = new Button (Image.FromResource (DirectorImages.ADD_ICON), "Add email");
			AddEmail.Clicked += delegate {
				Dialog AddEmailDialog = new Dialog()
				{	
					Title = "Add new email",
					Width = 300
				};
				AddEmailDialog.InitialLocation = WindowLocation.CenterParent;
				VBox AddEmailDialogContent = new VBox();
				AddEmailDialogContent.PackStart(new Label("Fill new email"), expand: false, fill: true);

				TextEntry NewEmail = new TextEntry();
				AddEmailDialogContent.PackStart(NewEmail, expand: false, fill: true);

				AddEmailDialog.Content = AddEmailDialogContent;
				AddEmailDialog.Buttons.Add (new DialogButton ("Cancel", Command.Cancel));
				AddEmailDialog.Buttons.Add (new DialogButton ("Ok", Command.Ok));

				Command result = AddEmailDialog.Run(this.ParentWindow);

				while (result == Command.Ok && !Email.IsValid(NewEmail.Text)) {
					MessageDialog.ShowError("Invalid email address!");
					AddEmailDialog.Dispose();
					result = AddEmailDialog.Run(this.ParentWindow);
				}

				AddEmailDialog.Dispose();

				if (result == Command.Ok) {
					ActiveServer.Emails.Add(new Email() { UserEmail = NewEmail.Text });
					RefreshServerEmails();
				}
			};

			// Add to frames
            HPaned p = new HPaned();
            AddEmail.WidthRequest = 120;
            AddEmail.HorizontalPlacement = WidgetPlacement.End;
			EmailFrameContent.PackStart (AddEmail, expand: false, fill: false);
			EmailFrame.Content = EmailFrameContent;
            PackStart(EmailFrame, expand: true, fill: true);
        }

		private void RemoveEmail_Clicked(object sender, EventArgs e)
		{
			int row = EmailNotifications.SelectedRow;
			if (row >= 0) {
				ActiveServer.Emails.RemoveAll (x => x.Position == row);
				RefreshServerEmails ();
			}
		}

		/// <summary>
		/// Email address store in consistent status!
		/// </summary>
		private void RefreshServerEmails()
		{
			EmailAddressStore.Clear();

			// Emails
			foreach (Email e in ActiveServer.Emails) {
				int i = EmailAddressStore.AddRow();
				EmailAddressStore.SetValue(i, EmailAddress, e.UserEmail);
				EmailAddressStore.SetValue(i, Errors, e.Errors);
				EmailAddressStore.SetValue(i, Notifications, e.Notifications);
				e.Position = i;
			}
		}

		/// <summary>
		/// Handle right mouse click on Email Notifications tree view.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void EmailNotifications_Clicked(object sender, ButtonEventArgs e)
		{
			if (e.Button == PointerButton.Right) {
				var row = EmailNotifications.GetRowAtPosition (e.X, e.Y);
				if (row >= 0) {
					EmailNotifications.SelectRow (row);
					EmailContextMenu.Popup (EmailNotifications, e.X, e.Y);
				}
			}
		}

        /// <summary>
        /// Change frequency selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FrequencyRunning_SelectionChanged(object sender, EventArgs e)
        {
            ActiveServer.RunningFrequency = FrequencyRunning.SelectedIndex;
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
			catch
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
            CurrentMainWindow.UpdateTreeStoreText(ActualPosition, ActiveServer.Name);
        }
    }
}
