using System;
using Xwt;
using System.Collections.Generic;
using Xwt.Drawing;
using Director.Forms.Controls;

namespace Director
{
	public class HeaderList : VBox
	{
		/// <summary>
		/// Default row height.
		/// </summary>
		public const int ROW_HEIGHT = 30;

        /// <summary>
        /// Tab indexes.
        /// </summary>
        private int _TabIndex = 0;

        /// <summary>
        /// Tab indexes mngment.
        /// </summary>
        public int TAB_INDEX
        {
            get { return _TabIndex++; }
            set { _TabIndex = value; }
        }

		/// <summary>
		/// Current map with headers.
		/// </summary>
		public List<Header> Headers { get; set; }

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
			HeaderListBox.Clear ();
			int x = 0;
            TAB_INDEX = 0;
			foreach (var h in Headers) {
                HeaderListItem tmp = new HeaderListItem(this, h, (x % 2 == 0) ? Colors.White : Colors.LightGray);
				HeaderListBox.PackStart (tmp);
				x++;
			}
		}

        /// <summary>
        /// Remove header.
        /// </summary>
        /// <param name="header"></param>
        public void RemoveHeader(Header header)
        {
            Headers.Remove(header);
            RefreshHeaderList();
        }

		/// <summary>
		/// Initializes the components.
		/// </summary>
		private void _initializeComponents()
		{
			// Create first header line
            HBox FirstLine = new HBox();
            Label HeaderType = new Label(Director.Properties.Resources.HeaderHeaderType)
            {
                HorizontalPlacement = WidgetPlacement.Center,
                ExpandHorizontal = true,
                ExpandVertical = false,
                MarginLeft = 10
            };
            Label HeaderValue = new Label(Director.Properties.Resources.HeaderHeaderValue)
            {
                ExpandHorizontal = true,
                ExpandVertical = false,
                HorizontalPlacement = WidgetPlacement.Center
            };
            Button NewHeader = new Button(Image.FromResource(DirectorImages.ADD_ICON))
            {
                MinWidth = 30,
                WidthRequest = 30,
                MarginRight = 30
            };
            FirstLine.PackStart(HeaderType, true, true);
            FirstLine.PackStart(HeaderValue, true, true);
            FirstLine.PackStart(NewHeader, false, false);
            PackStart(FirstLine);

            // New header event
            NewHeader.Clicked += NewHeader_Clicked;


            // Create header list box
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


        /// <summary>
        /// Create new header item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NewHeader_Clicked(object sender, EventArgs e)
        {
            Header NewHeader = new Header();
            Headers.Add(NewHeader);
            HeaderListItem NewHeaderListItem = new HeaderListItem(this, NewHeader, ((Headers.Count - 1) % 2 == 0) ? Colors.White : Colors.LightGray);
            HeaderListBox.PackStart(NewHeaderListItem);
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
        private TextEntryHelper Types { get; set; }

		/// <summary>
		/// Values.
		/// </summary>
        private TextEntryHelper Values { get; set; }

		/// <summary>
		/// Remove button.
		/// </summary>
		private Button RemoveBtn { get; set; }

        /// <summary>
        /// Most used headers.
        /// </summary>
        private List<String> MostUsedHeaders = new List<String>
        {
            "Accept",
            "Accept-Charset", 
            "Accept-Encoding",
            "Accept-Language",
            "Accept-Datetime",
            "Authorization", 
            "Cache-Control", 
            "Connection",
            "Cookie",
            "Content-Length",
            "Content-MD5",
            "Content-Type",
            "Date", 
            "Expect", 
            "From",
            "Host",
            "If-Match", 
            "If-Modified-Since", 
            "If-None-Match", 
            "If-Range", 
            "If-Unmodified-Since", 
            "Max-Forwards", 
            "Origin", 
            "Pragma", 
            "Proxy-Authorization", 
            "Range",
            "Referer", 
            "TE", 
            "User-Agent",
            "Via", 
            "Warning"
        };

        /// <summary>
        /// Most used values.
        /// </summary>
        private List<String> MostUsedValues = new List<String>
        {
            "application/json",
            "application/xml",
            "en-US",
            "keep-alive",
            "no-cache",
            "text/plain",
            "utf-8"
        };

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

            // Set background color
            BackgroundColor = DefaultColor = _color;

            // Set height
            MinHeight = HeaderList.ROW_HEIGHT;

            // Horizontal resize true
            ExpandHorizontal = true;

            // No margins
            Margin = 0;

            // Init component
            _initializeComponents();
		}

		/// <summary>
		/// Intialize hbox components.
		/// </summary>
        private void _initializeComponents() 
		{
            // Type
            Types = new TextEntryHelper()
            {
                Text = ActiveHeader.Name,
                MarginLeft = 5,
                VerticalPlacement = WidgetPlacement.Center,
                HorizontalPlacement = WidgetPlacement.Fill,
                HelpStrings = MostUsedHeaders,
                TabIndex = ParentList.TAB_INDEX
            };
            Types.Changed += delegate
            {
                ActiveHeader.Name = Types.Text;
            };
            Types.ShowHelper += delegate
            {
                Values.ShowListHelper(null, null);
            };
            Types.HideHelper += delegate
            {
                Values.HideListHelper(null, null);
            };

            // Value
            Values = new TextEntryHelper()
            {
				Text = ActiveHeader.Value,
                MarginLeft = 5,
                VerticalPlacement = WidgetPlacement.Center,
                HorizontalPlacement = WidgetPlacement.Fill,
                HelpStrings = MostUsedValues,
                TabIndex = ParentList.TAB_INDEX
			};
            Values.Changed += delegate
            {
                ActiveHeader.Value = Values.Text;
            };
            Values.ShowHelper += delegate
            {
                Types.ShowListHelper(null, null);
            };
            Values.HideHelper += delegate
            {
                Types.HideListHelper(null, null);
            };

            // Remove button
            RemoveBtn = new Button(Image.FromResource(DirectorImages.CROSS_ICON))
            {
                MarginRight = 20,
                HorizontalPlacement = WidgetPlacement.Center,
                VerticalPlacement = WidgetPlacement.Center,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            RemoveBtn.Clicked += RemoveBtn_Clicked;

			PackStart (Types, expand: true);
            PackStart(Values, expand: true);
            PackStart(RemoveBtn, expand: false, fill: false);
		}

        /// <summary>
        /// Remove email from list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RemoveBtn_Clicked(object sender, EventArgs e)
        {
            ParentList.RemoveHeader(ActiveHeader);
        }
	}
}

