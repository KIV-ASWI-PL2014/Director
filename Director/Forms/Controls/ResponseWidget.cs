using Director.DataStructures;
using Director.Forms.Inputs;
using Director.ParserLib;
using System;
using System.Collections.Generic;
using Xwt;
using Xwt.Drawing;
using System.Xml;
using Newtonsoft.Json;
using System.Globalization;

namespace Director.Forms.Controls
{
    internal class ResponseWidget : VBox
    {
        /// <summary>
        /// Expected response status code.
        /// </summary>
        private TextEntry ExpectedStatusCode { get; set; }

        /// <summary>
        /// Active request.
        /// </summary>
        public Request ActiveRequest;

        /// <summary>
        /// Request menu.
        /// </summary>
        private Menu RequestHelperMenu { get; set; }

        /// <summary>
        /// Invalid scenario name description.
        /// </summary>
        private Label InvalidStatusCode = new Label()
        {
            Markup = "<b>" + Director.Properties.Resources.InvalidStatusCode + "</b>",
            Visible = false,
            TextColor = Colors.Red,
            TextAlignment = Alignment.End,
            MarginRight = 10
        };


		/// <summary>
		/// Content box!
		/// </summary>
		/// <value>The content box.</value>
		public VBox ContentBox { get; set; }

		/// <summary>
		/// Menu edit variable.
		/// </summary>
		/// <value>The edit variable.</value>
		private MenuItem EditVariable { get; set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="Director.Forms.Controls.ResponseWidget"/> class.
		/// </summary>
		/// <param name="_request">_request.</param>
        public ResponseWidget(Request _request)
        {
            // Set request
            ActiveRequest = _request;

            // Set margin
            Margin = 10;

            // Expected status code
            PackStart(new Label(Director.Properties.Resources.ExpectedStatusCode));
            ExpectedStatusCode = new TextEntry()
            {
                Text = ActiveRequest.ExpectedStatusCode + "",
                ExpandHorizontal = true
            };
            PackStart(ExpectedStatusCode, expand: false, fill: false);
            PackStart(InvalidStatusCode, vpos: WidgetPlacement.End);
            ExpectedStatusCode.Changed += delegate
            {
                var t = ExpectedStatusCode.Text;
                if (t.Length == 0)
                {
                    InvalidStatusCode.Visible = false;
                    ActiveRequest.ExpectedStatusCode = -1;
                    return;
                }

                try
                {
                    int x = int.Parse(t);
                    if (x <= 0)
                        throw new InvalidCastException();

                    ActiveRequest.ExpectedStatusCode = x;
                    InvalidStatusCode.Visible = false;
                }
                catch
                {
                    InvalidStatusCode.Visible = true;
                }
            };

            // Label
            PackStart(new Label() { Markup = "<b>" + Director.Properties.Resources.ResponseContent + ":</b>" });

			// Content box
			ContentBox = new VBox () {
				ExpandHorizontal = true,
				ExpandVertical = true
			};
			PackStart(ContentBox, expand: true, fill: true);

            // Edit btn
            Button SetContent = new Button(Image.FromResource(DirectorImages.EDIT_CONTENT_ICON), Director.Properties.Resources.EditContent)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            PackStart(SetContent, expand: false, hpos: WidgetPlacement.End);

            // Click events
            SetContent.Clicked += SetContent_Clicked;

            // Request menu helper
            RequestHelperMenu = new Menu();

			// Edit
			EditVariable = new MenuItem(Director.Properties.Resources.EditVariable)
			{
				Image = Image.FromResource(DirectorImages.EDIT_CONTENT_ICON)
			};
			RequestHelperMenu.Items.Add(EditVariable);
			EditVariable.Clicked += EditVariable_Clicked;
			RefreshContent ();
        }

		/// <summary>
		/// Menu item edit variable clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void EditVariable_Clicked(object sender, EventArgs e)
		{
			// Active parser item
			if (ActiveParserItem == null) return;

			if (ActiveParserItem.value is List<ParserItem>) {

			} else {
				// Create dialog
				EditResponseVariable _edit = new EditResponseVariable (ActiveParserItem, ActiveRequest);

				// Run dialog
				var d = _edit.Run ();

				// Ok
				if (d == Command.Ok) {
					// Set value
					ActiveParserItem.value = _edit.GetValue ();

					// Create string
					string template = Parser.serialize (Template);
					if (ActiveRequest.ResponseTemplateType == ContentType.XML) {
						XmlDocument doc = JsonConvert.DeserializeXmlNode (template);
						Console.WriteLine (doc.ToString ());
						ActiveRequest.ResponseTemplate = doc.ToString ();
					} else {
						ActiveRequest.ResponseTemplate = template;
					}

					// Test
					RefreshContent ();
				}
				_edit.Dispose();
			}
		}

        /// <summary>
        /// Mouse button right click on canvas.
        /// </summary>
        void RenderBox_ButtonPressed(object sender, ButtonEventArgs e)
        {
            if (e.Button == PointerButton.Right)
            {
                ParserItem item = RenderBox.MouseTargetItemAt(e.X, e.Y);
                if (item != null)
                {
					ActiveParserItem = item;
                    RequestHelperMenu.Popup();
                }
            }
        }

		/// <summary>
		/// Active parser item.
		/// </summary>
		private ParserItem ActiveParserItem { get; set; }

		/// <summary>
		/// Scroll view.
		/// </summary>
		private ScrollView CanvasScrollView { get; set; }

		/// <summary>
		/// Dictionary template!
		/// </summary>
		private Dictionary<string, ParserItem> Template { get; set; }

		/// <summary>
		/// Render box
		/// </summary>
		public JSONCanvas RenderBox { get; set; }

		/// <summary>
		/// Render box for plain / text.
		/// </summary>
		public MultiLineTextEntry TextEntry { get; set; }

		/// <summary>
		/// Scroll view.
		/// </summary>
		/// <value>The text entry S.</value>
		public ScrollView TextEntrySV { get; set; }

		/// <summary>
		/// Active item.
		/// </summary>
		/// <value>The active item.</value>
		public Widget ActiveItem { get; set; }


		/// <summary>
		/// Refresh content box.
		/// </summary>
		public void RefreshContent()
		{
			// Guess type?
			if (ActiveRequest.ResponseTemplateType == ContentType.PLAIN) {
				if (TextEntry == null) {
					TextEntry = new MultiLineTextEntry () {
						Sensitive = false
					};
					TextEntrySV = new ScrollView () {
						Content = TextEntry,
						HorizontalScrollPolicy = ScrollPolicy.Automatic,
						VerticalScrollPolicy = ScrollPolicy.Automatic
					};
				}
				if (ActiveItem != TextEntrySV) {
					if (ActiveItem != null) {
						ContentBox.Remove (ActiveItem);
					}
					ContentBox.PackStart (TextEntrySV, true, true);
				}
				TextEntry.Text = ActiveRequest.ResponseTemplate;
				ActiveItem = TextEntrySV;
			} else {
				// From XML to JSON
				String jsonData = ActiveRequest.ResponseTemplate;
				if (ActiveRequest.RequestTemplateType == ContentType.XML) {
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(ActiveRequest.ResponseTemplate);
					jsonData = Newtonsoft.Json.JsonConvert.SerializeXmlNode (doc);
				}
				// Create render box
				if (RenderBox == null) {
					RenderBox = new JSONCanvas () {
						ExpandVertical = true,
						ExpandHorizontal = true,
						Template = null,
						DrawArray = true
					};
					RenderBox.ButtonPressed += RenderBox_ButtonPressed;
					CanvasScrollView = new ScrollView () {
						Content = RenderBox,
						HorizontalScrollPolicy = ScrollPolicy.Automatic,
						VerticalScrollPolicy = ScrollPolicy.Automatic
					};
				}

				if (ActiveItem != CanvasScrollView) {
					if (ActiveItem != null) {
						ContentBox.Remove (ActiveItem);
					}
					// Set
					ActiveItem = CanvasScrollView;
					ContentBox.PackStart (CanvasScrollView, true, true);
				}

				// Parse
				List<ParserError> errors = new List<ParserError>();
				RenderBox.Template = Template = Parser.deserialize(jsonData, errors, "template");
				RenderBox.QueueDraw();
			}
		}


        /// <summary>
        /// Set request.
        /// </summary>
		public void SetResponse(String responseTemplate, ContentType type)
        {
            ActiveRequest.ResponseTemplate = responseTemplate;
			ActiveRequest.ResponseTemplateType = type;
			RefreshContent ();
        }

        /// <summary>
        /// Set content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SetContent_Clicked(object sender, EventArgs e)
        {
            Parent.Sensitive = false;
            SetWindow _window = new SetWindow(null, this);
            _window.Closed += delegate
            {
                Parent.Sensitive = true;
            };
            _window.Show();
        }
    }

	internal class EditResponseVariable : Dialog
	{
		/// <summary>
		/// Item.
		/// </summary>
		private ParserItem ActiveItem { get; set; }

		/// <summary>
		/// Request.
		/// </summary>
		private Request ActiveRequest { get; set; }

		/// <summary>
		/// Value.
		/// </summary>
		private RadioButton Value { get; set; }

		/// <summary>
		/// Value type.
		/// </summary>
		private ComboBox ValueType { get; set; }

		/// <summary>
		/// Data types enum.
		/// </summary>
		private enum DataType : int {
			TYPE_STRING = 0,
			TYPE_INT = 1,
			TYPE_DOUBLE = 2,
			TYPE_NULL = 3,
			TYPE_BOOL = 4
		}

		/// <summary>
		/// Value text.
		/// </summary>
		private TextEntry ValueText { get; set; }

		/// <summary>
		/// Format guide.
		/// </summary>
		private RadioButton Format { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public EditResponseVariable(ParserItem _item, Request _request)
		{
			// Set title, icon, resizable
			Title = Director.Properties.Resources.EditVariable;
			Icon = Image.FromResource(DirectorImages.EDIT_CONTENT_ICON);
			Resizable = false;

			// Set variables
			ActiveItem = _item;
			ActiveRequest = _request;

			// Widht height
			Width = 520;
			Height = 390;

			// Init components
			_initializeComponents();

			// Add buttons
			Buttons.Add(new DialogButton(Director.Properties.Resources.ConfirmInput, Command.Ok));
			Buttons.Add(new DialogButton(Director.Properties.Resources.Cancel, Command.Cancel));

			// Parse data
			ParseData();
		}

		/// <summary>
		/// Parse data.
		/// </summary>
		private void ParseData()
		{
			ParserItem it = ActiveItem;
			Value.Active = true;

			if (it.value is System.Boolean)
			{
				ValueType.SelectedItem = DataType.TYPE_BOOL;
				ValueText.Text = ((System.Boolean)it.value) ? "true" : "false";
			}
			else if (it.value is System.Int64 || it.value is System.Int32)
			{
				ValueType.SelectedItem = DataType.TYPE_INT;
				ValueText.Text = it.value + "";
			}
			else if (it.value is System.Double)
			{
				ValueType.SelectedItem = DataType.TYPE_DOUBLE;
				ValueText.Text = (it.value + "").Replace(',', '.');
			}
			else if (it.value == null)
			{
				ValueType.SelectedItem = DataType.TYPE_NULL;
				ValueText.Text = "";
			}
			else
			{
				String text = (String)it.value;
				String[] tokens = text.Split ('#');
				try {
					if (tokens.Length == 6) {
						// Set format active
						Format.Active = true;

					// value
					var value = tokens [3];
					ConditionValue.Text = value;

					// Var name
					var variable = tokens [4];
					SaveToVarible.Text = variable;

						// Type
						var type = tokens[1];
						FormatTypeSelect.SelectedItem = type;

						if (FormatTypeSelect.SelectedIndex < 0)
							throw new Exception ("Invalid method");

						// Method
						var method = tokens [2];

						// Simple method?
						UseVariable.State = (method.IndexOf ("uv", 0) >= 0) ? CheckBoxState.On : CheckBoxState.Off;
						EvalIfPresent.State = (method.IndexOf("ip", 0) >= 0) ? CheckBoxState.On : CheckBoxState.Off;

						// Method name
						FormatOperationsSelect.SelectedItem = method.Replace("uv_", "").Replace("ip_", "");
						if (FormatOperationsSelect.SelectedIndex < 0)
							throw new Exception ("Invalid operation");
					} else {
						throw new Exception("Cannot evaluate");
					}
				} catch {
					Value.Active = true;
				}
				ValueType.SelectedItem = DataType.TYPE_STRING;
				ValueText.Text = text;
			}

			// Visible items
			GroupChanged_Event (null, null);
		}

		/// <summary>
		/// Format type.
		/// </summary>
		/// <value>The type of the format.</value>
		public ComboBox FormatTypeSelect { get; set; }

		/// <summary>
		/// Operations select.
		/// </summary>
		/// <value>The format operations select.</value>
		public ComboBox FormatOperationsSelect { get; set; }

		/// <summary>
		/// Use variable.
		/// </summary>
		/// <value>The use variable.</value>
		public CheckBox UseVariable { get; set; }

		/// <summary>
		/// Evaluate if present.
		/// </summary>
		/// <value>The eval if present.</value>
		public CheckBox EvalIfPresent { get; set; }

		/// <summary>
		/// Condition value.
		/// </summary>
		/// <value>The condition value.</value>
		public TextEntry ConditionValue { get; set; }

		/// <summary>
		/// Save to variable.
		/// </summary>
		/// <value>The save to varible.</value>
		public TextEntry SaveToVarible { get; set; }
			
		/// <summary>
		/// Init components.
		/// </summary>
		private void _initializeComponents()
		{
			VBox ContentBox = new VBox();

			// First value
			Value = new RadioButton(Director.Properties.Resources.JsonValue);
			ContentBox.PackStart(Value);

			// Horizontal
			HBox ValueBox = new HBox();

			// Options
			ValueType = new ComboBox()
			{
				WidthRequest = 80
			};
			ValueType.Items.Add(DataType.TYPE_STRING, "String");
			ValueType.Items.Add(DataType.TYPE_INT, "Integer");
			ValueType.Items.Add(DataType.TYPE_DOUBLE, "Double");
			ValueType.Items.Add(DataType.TYPE_NULL, "Null");
			ValueType.Items.Add(DataType.TYPE_BOOL, "Boolean");
			ValueType.SelectedIndex = 0;
			ValueBox.PackStart(ValueType, false, false);

			// Value
			ValueText = new TextEntry();
			ValueBox.PackStart(ValueText, true, true);
			ContentBox.PackStart(ValueBox);

			// Guide
			Format = new RadioButton(Director.Properties.Resources.FormatGuide);
			ContentBox.PackStart(Format);
			Value.Group = Format.Group;
			Value.Active = true;
			Value.Group.ActiveRadioButtonChanged += GroupChanged_Event;

			// Type
			ContentBox.PackStart (new Label (Director.Properties.Resources.FormatType) { Font = Font.SystemFont.WithWeight (FontWeight.Bold) }, true, false);

			// Combo box
			FormatTypeSelect = new ComboBox ();

			// Add items
			FormatTypeSelect.Items.Add ("string", "String");
			FormatTypeSelect.Items.Add ("integer", "Integer");
			FormatTypeSelect.Items.Add ("real", "Real");
			FormatTypeSelect.Items.Add ("boolean", "Boolean");
			FormatTypeSelect.SelectionChanged += delegate {
				var selectedItem = FormatOperationsSelect.SelectedItem;

				FormatOperationsSelect.Items.Clear();

				if (((String)FormatTypeSelect.SelectedItem) == "boolean") {
					FormatOperationsSelect.Items.Add ("eq", Director.Properties.Resources.OperationEquals);
					FormatOperationsSelect.Items.Add ("ne", Director.Properties.Resources.OperationNotEquals);
				} else {
					FormatOperationsSelect.Items.Add ("eq", Director.Properties.Resources.OperationEquals);
                    FormatOperationsSelect.Items.Add("ne", Director.Properties.Resources.OperationNotEquals);
					FormatOperationsSelect.Items.Add ("lt", Director.Properties.Resources.OperationLessThan);
                    FormatOperationsSelect.Items.Add("lte", Director.Properties.Resources.OperationLessThanOrEquals);
                    FormatOperationsSelect.Items.Add("gt", Director.Properties.Resources.OperationGreatherThan);
                    FormatOperationsSelect.Items.Add("gte", Director.Properties.Resources.OperationGreatherThanOrEqual);

					if (((String)FormatTypeSelect.SelectedItem) == "string")
						FormatOperationsSelect.Items.Add ("mp", Director.Properties.Resources.OperationMatchingRegex);
				}

				FormatOperationsSelect.SelectedItem = selectedItem;
				if (FormatOperationsSelect.SelectedIndex == -1)
					FormatOperationsSelect.SelectedIndex = 0;
			};
			ContentBox.PackStart (FormatTypeSelect, true, false);

			// Operations
			ContentBox.PackStart (new Label (Director.Properties.Resources.Operation) { Font = Font.SystemFont.WithWeight (FontWeight.Bold) }, true, false);
			FormatOperationsSelect = new ComboBox ();
            FormatOperationsSelect.Items.Add("eq", Director.Properties.Resources.OperationEquals);
            FormatOperationsSelect.Items.Add("ne", Director.Properties.Resources.OperationNotEquals);
            FormatOperationsSelect.Items.Add("lt", Director.Properties.Resources.OperationLessThan);
            FormatOperationsSelect.Items.Add("lte", Director.Properties.Resources.OperationLessThanOrEquals);
            FormatOperationsSelect.Items.Add("gt", Director.Properties.Resources.OperationGreatherThan);
            FormatOperationsSelect.Items.Add("gte", Director.Properties.Resources.OperationGreatherThanOrEqual);
            FormatOperationsSelect.Items.Add("mp", Director.Properties.Resources.OperationMatchingRegex);
			FormatOperationsSelect.SelectedIndex = 0;

			// Select - fill combo box
			FormatTypeSelect.SelectedIndex = 0;

			// Add
			ContentBox.PackStart (FormatOperationsSelect, true, false);

			// Use variable instead of value
            UseVariable = new CheckBox(Director.Properties.Resources.UseVarInsteadOfValue);
			ContentBox.PackStart (UseVariable, true, false);

			// Evaluate this if present
            EvalIfPresent = new CheckBox(Director.Properties.Resources.EvalCondIfParIsPresent);
			ContentBox.PackStart (EvalIfPresent, true, false);

			// Value
            ContentBox.PackStart(new Label(Director.Properties.Resources.ConditionValue) { Font = Font.SystemFont.WithWeight(FontWeight.Bold) }, true, false);
			ConditionValue = new TextEntry ();
			ContentBox.PackStart (ConditionValue, true, false);


			// Save to variable
            ContentBox.PackStart(new Label(Director.Properties.Resources.SaveToVariable) { Font = Font.SystemFont.WithWeight(FontWeight.Bold) }, true, false);
			SaveToVarible = new TextEntry ();
			ContentBox.PackStart (SaveToVarible, true, false);

			// Content
			Content = ContentBox;
		}

		/// <summary>
		/// Change item sensitivity.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="arg">Argument.</param>
		internal void GroupChanged_Event(object sender, EventArgs arg) {
			ValueText.Sensitive = Value.Active;
			ValueType.Sensitive = Value.Active;

			FormatTypeSelect.Sensitive = !Value.Active;
			FormatOperationsSelect.Sensitive = !Value.Active;
			UseVariable.Sensitive = !Value.Active;
			EvalIfPresent.Sensitive = !Value.Active;
			ConditionValue.Sensitive = !Value.Active;
			SaveToVarible.Sensitive = !Value.Active;
		}

		/// <summary>
		/// Return object.
		/// </summary>
		/// <returns></returns>
		internal object GetValue()
		{
			if (Value.Active) {
				var i = (DataType)ValueType.SelectedItem;
				var text = ValueText.Text;

				if (i == DataType.TYPE_INT) {
					try {
						return int.Parse (text);
					} catch {
						return 0;
					}
				} else if (i == DataType.TYPE_BOOL) {
					return text.Trim () == "true" || text.Trim () == "1";
				} else if (i == DataType.TYPE_DOUBLE) {
					try {
						return double.Parse (text, CultureInfo.InvariantCulture.NumberFormat);
					} catch {
						return 0.0;
					}
				} else if (i == DataType.TYPE_NULL) {
					return null;
				} else {
					return ValueText.Text;
				}
			} else {
				string ret = "#" + (String) FormatTypeSelect.SelectedItem + "#";

				if (EvalIfPresent.Active)
					ret += "ip_";

				if (UseVariable.Active)
					ret += "uv_";

				ret += (String)FormatOperationsSelect.SelectedItem + "#";

				// Value
				ret += string.Format("{0}#{1}#", ConditionValue.Text, SaveToVarible.Text);

				return ret;
			}
		}
	}
}