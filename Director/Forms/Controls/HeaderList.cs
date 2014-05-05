using System;
using Xwt;
using System.Collections.Generic;
using Xwt.Drawing;
using Director.Forms.Controls;
using Director.DataStructures;

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
        /// Header list items.
        /// </summary>
        public List<HeaderListItem> HeaderListItems { get; set; }

        /// <summary>
        /// Scenario for parser variables.
        /// </summary>
        public Scenario ActiveScenario { get; private set; }

        /// <summary>
        /// Create header list.
        /// </summary>
        public HeaderList(List<Header> headers, Scenario sc = null)
        {
            // Set headers
            Headers = headers;

            // Expand
            ExpandHorizontal = true;
            ExpandVertical = true;

            // Margins
            Margin = 10;

            // Scenario
            ActiveScenario = sc;

            // Header list items
            HeaderListItems = new List<HeaderListItem>();

            // Prepare
            _initializeComponents();

            // Refresh headers
            RefreshHeaderList();
        }

        /// <summary>
        /// Refresh header list items.
        /// </summary>
        public void RefreshHeaderList()
        {
            HeaderListBox.Clear();
            var x = 0;
            TAB_INDEX = 0;
            foreach (var h in Headers)
            {
                var tmp = new HeaderListItem(this, h, (x%2 == 0) ? Colors.White : Colors.LightGray);
                HeaderListBox.PackStart(tmp);
                HeaderListItems.Add(tmp);
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
            HeaderListBox = new VBox()
            {
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
        private void NewHeader_Clicked(object sender, EventArgs e)
        {
            Header NewHeader = new Header();
            Headers.Add(NewHeader);
            HeaderListItem NewHeaderListItem = new HeaderListItem(this, NewHeader,
                ((Headers.Count - 1)%2 == 0) ? Colors.White : Colors.LightGray);
            HeaderListBox.PackStart(NewHeaderListItem);
            HeaderListItems.Add(NewHeaderListItem);
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
        /// Variable button!
        /// </summary>
        private Button VariableButton { get; set; }

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
            Types.Changed += delegate { ActiveHeader.Name = Types.Text; };
            Types.GotFocus += GotChildFocusHandler;

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
            Values.Changed += delegate { ActiveHeader.Value = Values.Text; };
            Values.GotFocus += GotChildFocusHandler;

            // Parent list contains Scenario
            if (ParentList.ActiveScenario != null)
            {
                VariableButton = new Button(Image.FromResource(DirectorImages.HEADER_IMAGE))
                {
                    HorizontalPlacement = WidgetPlacement.Center,
                    VerticalPlacement = WidgetPlacement.Center,
                    ExpandHorizontal = false,
                    ExpandVertical = false
                };
                VariableButton.Clicked += VariableButton_Clicked;
            }

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

            PackStart(Types, expand: true);
            PackStart(Values, expand: true);
            if (VariableButton != null)
                PackStart(VariableButton, expand: false, fill: false);
            PackStart(RemoveBtn, expand: false, fill: false);
        }

        /// <summary>
        /// Variable expression items!
        /// </summary>
        private TextEntry TextBefore { get; set; }
        private TextEntry TextAfter { get; set; }
        private ComboBox Variables { get; set; }
        private Label Example { get; set; }

        /// <summary>
        /// Set variable.
        /// </summary>
        void VariableButton_Clicked(object sender, EventArgs e)
        {
            // Test if variables are ready
            if (ParentList.ActiveScenario.customVariables.Count == 0)
            {
                MessageDialog.ShowError(Director.Properties.Resources.NoVariablesFound);
                return;
            }

            // Create Dialog window
            var expressionDialog = new Dialog()
            {
                InitialLocation = WindowLocation.CenterParent,
                Width = 370,
                Resizable = false
            };

            // Set Title
            expressionDialog.Title = Director.Properties.Resources.HeaderSettings;

            // Prepare content
            VBox t = new VBox()
            {
                ExpandHorizontal = true,
                ExpandVertical = true
            };

            // Text before
            TextBefore = new TextEntry();
            t.PackStart(new Label(Director.Properties.Resources.TextBefore + ":"));
            t.PackStart(TextBefore, false, true);
            
            // Variable
            Variables = new ComboBox();
            t.PackStart(new Label(Director.Properties.Resources.Variable + ":"));
            Variables.Items.Add("");
            foreach (String k in ParentList.ActiveScenario.customVariables.Keys)
                Variables.Items.Add(k);
            t.PackStart(Variables, false, true);

            // Text after
            TextAfter = new TextEntry();
            t.PackStart(new Label(Director.Properties.Resources.TextAfter + ":"));
            t.PackStart(TextAfter, false, true);

            // Example
            Example = new Label();
            t.PackStart(new Label(Director.Properties.Resources.Example + ":"));
            t.PackStart(Example, false, true);

            // Expression solver split by $$
            String value = ActiveHeader.Value;
            var arr = value.Split('$');

            if (arr.Length == 1)
            {
                TextBefore.Text = arr[0];
                Variables.SelectedIndex = 0;
                TextAfter.Text = "";
            }
            else if (arr.Length == 3)
            {
                TextBefore.Text = arr[0];
                TextAfter.Text = arr[2];
                try
                {
                    Variables.SelectedText = arr[1];
                }
                catch
                {
                    TextBefore.Text = value;
                    TextAfter.Text = "";
                    Variables.SelectedIndex = 0;
                }
            }
            else
            {
                TextBefore.Text = value;
                TextAfter.Text = "";
                Variables.SelectedIndex = 0;
            }
            Example.Text = value;

            // Set hooks
            TextBefore.Changed += ExpressionSolver;
            TextAfter.Changed += ExpressionSolver;
            Variables.SelectionChanged += ExpressionSolver;

            // Image
            HBox ContentBox = new HBox()
            {
                ExpandHorizontal = true,
                ExpandVertical = true
            };

            // Image view
            ImageView ImageIcon = new ImageView(Image.FromResource(DirectorImages.HEADER_EDIT_IMAGE))
            {
                WidthRequest = 32, HeightRequest = 32,
                Margin = 20
            };
            ContentBox.PackStart(ImageIcon, false, false);
            ContentBox.PackStart(t, true, true);

            // Set content
            expressionDialog.Content = ContentBox;

            // Prepare buttons
            expressionDialog.Buttons.Add(new DialogButton(Director.Properties.Resources.OkComand, Command.Ok));
            expressionDialog.Buttons.Add(new DialogButton(Director.Properties.Resources.Cancel, Command.Cancel));

            // Run?
            var result = expressionDialog.Run(this.ParentList.ParentWindow);

            if (result == Command.Ok)
                Values.Text = ActiveHeader.Value = Example.Text;

            // Dispose dialog
            expressionDialog.Dispose();
        }

        /// <summary>
        /// Expression solver.
        /// </summary>
        void ExpressionSolver(object sender, EventArgs e)
        {
            Example.Text = TextBefore.Text;

            if (Variables.SelectedText.Length > 0)
            {
                Example.Text += string.Format("${0}$", Variables.SelectedText);
            }

            Example.Text += TextAfter.Text;
        }

        

        /// <summary>
        /// Hide all other children helpers.
        /// </summary>
        private void GotChildFocusHandler(object sender, EventArgs e)
        {
            foreach (HeaderListItem it in ParentList.HeaderListItems)
            {
                if (it == this)
                {
                    it.Values.HelperVisibility = true;
                    it.Types.HelperVisibility = true;
                }
                else
                {
                    it.Values.HelperVisibility = false;
                    it.Types.HelperVisibility = false;
                }
            }
        }

        /// <summary>
        /// Remove email from list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveBtn_Clicked(object sender, EventArgs e)
        {
            ParentList.RemoveHeader(ActiveHeader);
        }
    }
}