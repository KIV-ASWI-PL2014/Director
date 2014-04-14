using System;
using Xwt;
using Director.DataStructures;
using Xwt.Drawing;
using System.Collections.Generic;
using Director.DataStructures.SupportStructures;

namespace Director
{
	public class EmailList : VBox
	{
		/// <summary>
		/// Row height.
		/// </summary>
		public const int ROW_HEIGHT = 20;

		/// <summary>
		/// Email field width.
		/// </summary>
		public const int EMAIL_WIDTH = 150;

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
				MinWidth = EMAIL_WIDTH, 
				WidthRequest = EMAIL_WIDTH, 
				HeightRequest = 20, 
				MinHeight =  20,
				ExpandVertical = true, 
				VerticalPlacement = WidgetPlacement.Start,
				HorizontalPlacement = WidgetPlacement.Center,
				MarginLeft = 10,
				MarginTop = 5
			};
			Label Errors = new Label ("Error") {
				MinWidth =  40, WidthRequest = 40, ExpandVertical = false, VerticalPlacement = WidgetPlacement.Fill
			};
			Label Notif = new Label ("Notifications") {
				MinWidth =  40, WidthRequest = 40, ExpandVertical = false, VerticalPlacement = WidgetPlacement.Fill
			};			
			Button NewEmail = new Button("Add") {
				MinWidth =  40, WidthRequest = 40, ExpandVertical = false, VerticalPlacement = WidgetPlacement.Fill
			};
			NewEmail.Clicked += NewEmail_Clicked;
			FirstLine.PackStart (EmailName, true, true);
			FirstLine.PackStart (Errors, false, false);
			FirstLine.PackStart (Notif, false, false);
			FirstLine.PackStart (NewEmail, false, false);
			PackStart (FirstLine);

			// Create content
			EmailListContent = new VBox () {
				BackgroundColor =  Colors.White, ExpandVertical = true, ExpandHorizontal = false
			};

			// Clear list
			EmailItems.Clear ();

			// Add item list
			PackStart (EmailListContent, true, true);
		}

		/// <summary>
		/// Add new email to email list in server & create email list box.
		/// </summary>
		private void NewEmail_Clicked(object sender, EventArgs e)
		{
			var _new = new Email ();
			ActiveServer.Emails.Add (_new);
			var tmp = new EmailListItem (this, _new);
			EmailItems.Add (tmp);
			EmailListContent.PackStart (tmp);
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
			foreach (Email i in ActiveServer.Emails) {
				var tmp = new EmailListItem (this, i);
				EmailListContent.PackStart (tmp);
				EmailItems.Add (tmp);
				Console.WriteLine (i.UserEmail);
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
		/// Create list item with specific conditions.
		/// </summary>
		public EmailListItem(EmailList parent, Email s)
		{
			ListParent = parent;
			ActiveEmail = s;
			EmailText = new TextEntry () {
				Text = s.UserEmail,
				MinWidth = EmailList.EMAIL_WIDTH, WidthRequest = EmailList.EMAIL_WIDTH,
				MinHeight = EmailList.ROW_HEIGHT, HeightRequest = EmailList.ROW_HEIGHT,
				MarginLeft = 5,
				HorizontalPlacement = WidgetPlacement.Center,
				VerticalPlacement = WidgetPlacement.Start
			};
			PackStart (EmailText);

			// Error
			Errors = new CheckBox () {

			};

			MouseEntered += delegate(object sender, EventArgs e) {
				BackgroundColor = Colors.LightBlue;
			};

			MouseExited += delegate(object sender, EventArgs e) {
				BackgroundColor = Colors.White;
			};
		}
	}
}

