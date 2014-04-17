using System;
using Xwt;
using Director.DataStructures;
using Xwt.Drawing;
using System.Collections.Generic;
using Director.DataStructures.SupportStructures;
using Director.Forms.Controls;

namespace Director
{
	public class EmailList : VBox
	{
		/// <summary>
		/// Row height.
		/// </summary>
		public const int ROW_HEIGHT = 30;

		/// <summary>
		/// Email field width.
		/// </summary>
		public const int EMAIL_WIDTH = 150;

        /// <summary>
        /// Notification width.
        /// </summary>
        public const int NOTIFICATION_WIDTH = 80;

        /// <summary>
        /// Error checkbox width.
        /// </summary>
        public const int ERROR_WIDTH = 80;

        /// <summary>
        /// Destroy button field width.
        /// </summary>
        public const int BTN_WIDTH = 80;

		/// <summary>
		/// Current server instance.
		/// </summary>
		private Server ActiveServer { get; set; }

		/// <summary>
		/// Email List Content with email list items.
		/// </summary>
		private VBox EmailListContent { get; set; }

		/// <summary>
		/// Email list items.
		/// </summary>
		private List<EmailListItem> EmailItems = new List<EmailListItem> ();

		/// <summary>
		/// Default contstructor of email list widget.
		/// </summary>
		public EmailList ()
		{
			// Expand?
			ExpandVertical = true;
			ExpandVertical = true;

			// Init components
			_initializeComponents ();
		}

		/// <summary>
		/// Init object components.
		/// </summary>
		private void _initializeComponents()
		{
			// Create first line with informations
			HBox FirstLine = new HBox ();
			Label EmailName = new Label ("Email") {
				MinWidth = EMAIL_WIDTH + 50, 
				WidthRequest = EMAIL_WIDTH + 50, 
				HeightRequest = 20, 
				MinHeight =  20,
				ExpandVertical = true, 
				VerticalPlacement = WidgetPlacement.Start,
				HorizontalPlacement = WidgetPlacement.Center,
				MarginLeft = 10,
				MarginTop = 5
			};
			Label Errors = new Label ("Error") {
                MinWidth = ERROR_WIDTH,
                WidthRequest = ERROR_WIDTH,
                ExpandVertical = false,
                VerticalPlacement = WidgetPlacement.Fill
			};
			Label Notif = new Label ("Notifications") {
                MinWidth = NOTIFICATION_WIDTH,
                WidthRequest = NOTIFICATION_WIDTH,
                ExpandVertical = false,
                VerticalPlacement = WidgetPlacement.Fill
			};			
			Button NewEmail = new Button("Add") {
                MinWidth = BTN_WIDTH,
                WidthRequest = BTN_WIDTH,
                ExpandVertical = false,
                VerticalPlacement = WidgetPlacement.Fill
			};
			NewEmail.Clicked += NewEmail_Clicked;
			FirstLine.PackStart (EmailName, true, true);
			FirstLine.PackStart (Errors, false, false);
			FirstLine.PackStart (Notif, false, false);
			FirstLine.PackStart (NewEmail, false, false);
			PackStart (FirstLine);

			// Create content
            EmailListContent = new VBox()
            {
                BackgroundColor = Colors.White,
                ExpandVertical = true,
                ExpandHorizontal = false
            };

            ScrollView EmailListScroll = new ScrollView()
            {
                HorizontalScrollPolicy = ScrollPolicy.Never,
                VerticalScrollPolicy = ScrollPolicy.Automatic,
                Content = EmailListContent,
                BackgroundColor = Colors.LightGray
            };

			// Clear list
			EmailItems.Clear ();

			// Add item list
            PackStart(EmailListScroll, true, true);
		}

		/// <summary>
		/// Add new email to email list in server & create email list box.
		/// </summary>
		private void NewEmail_Clicked(object sender, EventArgs e)
		{
			var _new = new Email ();
			ActiveServer.Emails.Add (_new);
			int size = ActiveServer.Emails.Count + 1;
			var tmp = new EmailListItem (this, _new, (size % 2 == 0) ? Colors.White : Colors.LightGray) {
				BackgroundColor = Colors.Red
			};
			EmailItems.Add (tmp);
			EmailListContent.PackStart (tmp);
		}

		/// <summary>
		/// Remove active email.
		/// </summary>
		public void RemoveEmail(Email email)
		{
			ActiveServer.Emails.Remove (email);
			SetServer (ActiveServer);
		}


		/// <summary>
		/// Set server to email list view.
		/// </summary>
		/// <param name="_server">_server.</param>
		public void SetServer(Server _server)
		{
			// Servers
			ActiveServer = _server;

			// Clear items
			EmailItems.Clear ();

			// Clear list content
			EmailListContent.Clear ();

			// Add all emails from Server
			int x = 0;
			foreach (Email i in ActiveServer.Emails) {
				var tmp = new EmailListItem (this, i, (x%2 == 0) ? Colors.White : Colors.LightGray);
				EmailListContent.PackStart (tmp);
				EmailItems.Add (tmp);
				x++;
			}
		}
	}

	class EmailListItem : HBox
	{
		/// <summary>
		/// Email text entry.
		/// </summary>
		/// <value>The email text.</value>
		public TextEntry EmailText { get; set; }

		/// <summary>
		/// Parent.
		/// </summary>
		public EmailList ListParent { get; set; }

		/// <summary>
		/// Email for chaning data.
		/// </summary>
		/// <value>The active scenario.</value>
		public Email ActiveEmail { get; set; }

		/// <summary>
		/// Notification email?
		/// </summary>
		public CheckBox Notification { get; set; } 

		/// <summary>
		/// Errors
		/// </summary>
		public CheckBox Errors { get; set; }

		/// <summary>
		/// Button for removing email address from list.
		/// </summary>
		public Button RemoveEmail { get; set; }

		/// <summary>
		/// Default color.
		/// </summary>
		/// <value>The default color.</value>
		public Color DefaultColor { get; set; }

		/// <summary>
		/// Create list item with specific conditions.
		/// </summary>
		public EmailListItem(EmailList parent, Email s, Color bgColor)
		{
            // Set height
            MinHeight = EmailList.ROW_HEIGHT;
            HeightRequest = EmailList.ROW_HEIGHT;

			// Set background color
			BackgroundColor = DefaultColor = bgColor;

			// Self expand horizontal and vertical
			ExpandHorizontal = true;
			ExpandVertical = true;

			// No margin at all
			Margin = 0;

            // Set parent & active email
			ListParent = parent;
			ActiveEmail = s;
			EmailText = new TextEntry () {
				Text = s.UserEmail,
				MarginLeft = 5,
				HorizontalPlacement = WidgetPlacement.Fill,
				VerticalPlacement = WidgetPlacement.Center,
				MarginTop = 1,
				ExpandHorizontal = true,
				ExpandVertical =  false
			};
            EmailText.Changed += delegate
            {
                s.UserEmail = EmailText.Text;
				if (Email.IsValid(EmailText.Text)) {
					BackgroundColor = DefaultColor;
				} else {
					BackgroundColor = Colors.Red;
				}
            };
			PackStart (EmailText, true, true);

			// Error
			Errors = new CheckBox () {
                State = (s.Errors) ? CheckBoxState.On : CheckBoxState.Off,
                WidthRequest = EmailList.ERROR_WIDTH,
                MinWidth = EmailList.ERROR_WIDTH,
                HorizontalPlacement = WidgetPlacement.Center,
				VerticalPlacement = WidgetPlacement.Center,
				MarginLeft = 25
			};
            Errors.Toggled += delegate
            {
                s.Errors = Errors.State == CheckBoxState.On;
            };
			PackStart(Errors);

            // Notification
            Notification = new CheckBox()
            {
                State = (s.Notifications) ? CheckBoxState.On : CheckBoxState.Off,
                WidthRequest = EmailList.ERROR_WIDTH,
                MinWidth = EmailList.ERROR_WIDTH,
                HorizontalPlacement = WidgetPlacement.Center,
				VerticalPlacement = WidgetPlacement.Center
            };
            Notification.Toggled += delegate
            {
                s.Notifications = Notification.State == CheckBoxState.On;
            };
			PackStart(Notification);

            // Button
			RemoveEmail = new Button (Image.FromResource (DirectorImages.CROSS_ICON)) {
				HorizontalPlacement = WidgetPlacement.Center,
				VerticalPlacement = WidgetPlacement.Center,
				Style =  ButtonStyle.Borderless,
				MarginRight = 20
			};
			RemoveEmail.Clicked += delegate {
				parent.RemoveEmail(ActiveEmail);
			};
			PackStart (RemoveEmail);
		}
	}
}

