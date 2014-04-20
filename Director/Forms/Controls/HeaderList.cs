using System;
using Xwt;
using System.Collections.Generic;
using Xwt.Drawing;

namespace Director
{
	public class HeaderList : VBox
	{
		/// <summary>
		/// Default row height.
		/// </summary>
		public const int ROW_HEIGHT = 20;

		/// <summary>
		/// Current map with headers.
		/// </summary>
		public List<Header> Headers { get; set; }

		/// <summary>
		/// Headers list view.
		/// </summary>
		private ListView HeaderListView = new ListView();

		/// <summary>
		/// Header type.
		/// </summary>
		private DataField<string> HeaderTypes = new DataField<string> ();

		/// <summary>
		/// Header values.
		/// </summary>
		private DataField<string> HeaderValues = new DataField<string> ();

		/// <summary>
		/// Create list store.
		/// </summary>
		private ListStore HeaderListStore { get; set; }

		/// <summary>
		/// Header list content.
		/// </summary>
		private List<HeaderListItem> HeaderListContent { get; set; }

		/// <summary>
		/// Header list vbox.
		/// </summary>
		/// <value>The header list box.</value>
		private VBox HeaderListBox { get; set; }

		/// <summary>
		/// Create header list.
		/// </summary>
		public HeaderList (List<Header> headers)
		{
			// Set headers
			Headers = headers;

			// Expand
			ExpandHorizontal = true;
			ExpandVertical = true;

			// Margins
			Margin = 10;

			// Create header list content
			HeaderListContent = new List<HeaderListItem> ();

			// Prepare
			_initializeComponents ();

			// Refresh headers
			RefreshHeaderList ();
		}

		/// <summary>
		/// Refresh header list items.
		/// </summary>
		public void RefreshHeaderList()
		{
			HeaderListContent.Clear ();
			HeaderListBox.Clear ();
			int x = 0;
			foreach (var h in Headers) {
				var tmp = new HeaderListItem (this, h, (x % 2 == 0) ? Colors.White : Colors.LightGray);
				HeaderListContent.Add (tmp);
				HeaderListBox.PackStart (tmp);
				x++;
			}
		}

		/// <summary>
		/// Initializes the components.
		/// </summary>
		private void _initializeComponents()
		{
			// Create first header line
			HeaderListBox = new VBox () {
				BackgroundColor = Colors.White,
				ExpandVertical = true,
				ExpandHorizontal = false
			};

			ScrollView HeaderListScroll = new ScrollView()
			{
				HorizontalScrollPolicy = ScrollPolicy.Never,
				VerticalScrollPolicy = ScrollPolicy.Always,
				Content = HeaderListBox,
				BackgroundColor = Colors.LightGray
			};
					
			// Add item list
			PackStart(HeaderListScroll, true, true);
		}
	}

	/// <summary>
	/// Header list items.
	/// </summary>
	public class HeaderListItem : HBox
	{
		/// <summary>
		/// Active header - for changing values.
		/// </summary>
		private Header ActiveHeader { get; set; }

		/// <summary>
		/// Parent.
		/// </summary>
		private HeaderList ParentList { get; set; }

		/// <summary>
		/// Default background color.
		/// </summary>
		/// <value>The default background.</value>
		private Color DefaultColor { get; set; }

		/// <summary>
		/// Types.
		/// </summary>
		private TextEntry Types { get; set; }

		/// <summary>
		/// Values.
		/// </summary>
		private TextEntry Values { get; set; }

		/// <summary>
		/// Remove button.
		/// </summary>
		private Button Remove { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Director.HeaderListItem"/> class.
		/// </summary>
		/// <param name="_parent">_parent.</param>
		/// <param name="_header">_header.</param>
		/// <param name="_color">_color.</param>
		public HeaderListItem(HeaderList _parent, Header _header, Color _color)
		{
			ActiveHeader = _header;
			ParentList = _parent;
			DefaultColor = _color;	
			_initializeComponents ();
		}

		/// <summary>
		/// Intialize hbox components.
		/// </summary>
		private void _initializeComponents() 
		{
			Values = new TextEntry () {
				Text = ActiveHeader.Value
			};

			Types = new TextEntry () {
				Text = ActiveHeader.Name
			};

			PackStart (Types);
			PackStart (Values);
		}
	}
}

