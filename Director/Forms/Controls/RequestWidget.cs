using Director.DataStructures;
using Director.Forms.Inputs;
using Director.ParserLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Controls
{
    internal class RequestWidget : VBox
    {
        /// <summary>
        /// Text view.
        /// </summary>
        public TextEntry TextView { get; set; }

        /// <summary>
        /// Render box
        /// </summary>
        public JSONCanvas RenderBox { get; set; }

        /// <summary>
        /// Active request.
        /// </summary>
        public Request ActiveRequest;

        /// <summary>
        /// Request menu.
        /// </summary>
        private Menu RequestHelperMenu { get; set; }

        /// <summary>
        /// Canvas scroll view.
        /// </summary>
        private ScrollView CanvasScrollView { get; set; }

        /// <summary>
        /// Dictionary template!
        /// </summary>
        private Dictionary<string, ParserItem> Template { get; set; }

        /// <summary>
        /// Edit variable item.
        /// </summary>
        private MenuItem EditVariable { get; set; }

        /// <summary>
        /// Create widget.
        /// </summary>
        /// <param name="_request"></param>
        public RequestWidget(Request _request)
        {
            // Set request
            ActiveRequest = _request;

            // Set margin
            Margin = 10;

            // Create Text view
            PackStart(new Label() { Markup = "<b>" + Director.Properties.Resources.RequestContent + "</b>" });

            // Parse Request
            if (_request.RequestTemplate != null)
            {
                List<ParserError> errors = new List<ParserError>();
                Template = Parser.deserialize(_request.RequestTemplate, errors, "template");
            }

            // Create canvas
            RenderBox = new JSONCanvas()
            {
                ExpandHorizontal = true,
                ExpandVertical = true,
                Template = Template
            };

            // Create Scroll view
            CanvasScrollView = new ScrollView()
            {
                HorizontalScrollPolicy = ScrollPolicy.Automatic,
                VerticalScrollPolicy = ScrollPolicy.Automatic
            };
            PackStart(CanvasScrollView, expand: true, fill: true);
            CanvasScrollView.Content = RenderBox;

            // Set template
            RenderBox.Template = Template;

            // Action items
            RenderBox.ButtonPressed += RenderBox_ButtonPressed;

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
            EditVariable = new MenuItem(Director.Properties.Resources.EditVariable)
            {
                Image = Image.FromResource(DirectorImages.EDIT_CONTENT_ICON)
            };
            RequestHelperMenu.Items.Add(EditVariable);
            EditVariable.Clicked += EditVariable_Clicked;
        }

        /// <summary>
        /// Clickable delegate.
        /// </summary>
        void EditVariable_Clicked(object sender, EventArgs e)
        {
            // Active parser item
            if (ActiveParserItem == null) return;

            // Create dialog
            EditRequestVariable _edit = new EditRequestVariable(ActiveParserItem);

            // Run dialog
            var d = _edit.Run();

            // Ok
            if (d == Command.Ok)
            {
                ActiveParserItem.value = _edit.GetValue();
                SetRequest(Parser.serialize(Template));
            }

            _edit.Dispose();
        }

        /// <summary>
        /// Active parser item.
        /// </summary>
        private ParserItem ActiveParserItem { get; set; }

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
                    //RequestHelperMenu.Items[0].Label = string.Format("{0} - {1}", item.value.GetType().ToString(), item.value.ToString());
                    RequestHelperMenu.Popup();
                }
            }
        }

        /// <summary>
        /// Set request.
        /// </summary>
        public void SetRequest(String requestTemplate)
        {
            ActiveRequest.RequestTemplate = requestTemplate;
            List<ParserError> errors = new List<ParserError>();
            RenderBox.Template = Template = Parser.deserialize(requestTemplate, errors, "template");
            RenderBox.QueueDraw();
        }

        /// <summary>
        /// Set content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SetContent_Clicked(object sender, EventArgs e)
        {
            Parent.Sensitive = false;
            SetWindow _window = new SetWindow(this, null);
            _window.Closed += delegate
            {
                Parent.Sensitive = true;
            };
            _window.Show();
        }
    }

    struct RequestVariable
    {
        public int Start, End, DecimalPlaces, Type;
        public string Text, Pattern, Method;
        public bool Format;
    }

    internal class EditRequestVariable : Dialog
    {
        /// <summary>
        /// Active item.
        /// </summary>
        private ParserItem ActiveItem { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        public RadioButton Value { get; set; }

        /// <summary>
        /// Format directive.
        /// </summary>
        public RadioButton Directive { get; set; }

        /// <summary>
        /// Guide.
        /// </summary>
        public RadioButton Guide { get; set; }

        /// <summary>
        /// Value text.
        /// </summary>
        public TextEntry ValueText { get; set; }

        /// <summary>
        /// Value options.
        /// </summary>
        public ComboBox ValueOptions { get; set; }

        /// <summary>
        /// Directive text.
        /// </summary>
        public TextEntry DirectiveText { get; set; }

        /// <summary>
        /// Variables.
        /// </summary>
        public List<RequestVariable> Variables { get; set; }

        /// <summary>
        /// Output example label.
        /// </summary>
        public Label OutputExampleLabel { get; set; }

        /// <summary>
        /// Output example string.
        /// </summary>
        public String OutputExample { get; set; }

        /// <summary>
        /// Types.
        /// </summary>
        public const int TYPE_STRING = 0;
        public const int TYPE_INT = 1;
        public const int TYPE_DOUBLE = 2;
        public const int TYPE_NULL = 3;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item"></param>
        public EditRequestVariable(ParserItem item)
        {
            // Title and dialog variables
            Title = Director.Properties.Resources.EditVariable;
            Icon = Image.FromResource(DirectorImages.EDIT_CONTENT_ICON);
            Resizable = false;
            ActiveItem = item;
            Variables = new List<RequestVariable>();

            // Widht height
            Width = 450;
            Height = 380;

            // Init content
            _initializeComponents();

            // Buttons
            Buttons.Add(new DialogButton(Director.Properties.Resources.ConfirmInput, Command.Ok));
            Buttons.Add(new DialogButton(Director.Properties.Resources.Cancel, Command.Cancel));

            // Set data
            ParseData();
        }

        /// <summary>
        /// Set data.
        /// </summary>
        private void ParseData()
        {
            ParserItem it = ActiveItem;

            if (it.value is System.Int64 || it.value is System.Int32)
            {
                Variables.Add(new RequestVariable()
                {
                    Text = it.value + "",
                    Type = TYPE_INT
                });
            }
            else if (it.value is System.Double)
            {
                Variables.Add(new RequestVariable()
                {
                    Text = (it.value + "").Replace(',', '.'),
                    Type = TYPE_DOUBLE
                });
            }
            else if (it.value == null)
            {
                Variables.Add(new RequestVariable()
                {
                    Text = "",
                    Type = TYPE_NULL
                });
            }
            else
            {
                Variables.Add(new RequestVariable()
                {
                    Text = it.value.ToString(),
                    Type = TYPE_STRING
                });
            }

            // One variable parse
            if (Variables.Count == 1)
            {
                var i = Variables[0];

                if (i.Format)
                {
                    Directive.Active = true;
                }
                else
                {
                    Value.Active = true;
                    ValueOptions.SelectedIndex = i.Type;
                    ValueText.Text = i.Text;
                }

            }
        }

        /// <summary>
        /// Return new value.
        /// </summary>
        public object GetValue()
        {
            if (Value.Active)
            {
                var i = ValueOptions.SelectedIndex;
                var text = ValueText.Text;

                if (i == TYPE_INT)
                {
                    try
                    {
                        return int.Parse(text);
                    }
                    catch
                    {
                        return 0;
                    }
                }
                else if (i == TYPE_DOUBLE)
                {
                    try
                    {
                        return double.Parse(text, CultureInfo.InvariantCulture.NumberFormat);
                    }
                    catch
                    {
                        return 0.0;
                    }
                }
                else if (i == TYPE_STRING)
                {
                    return ValueText.Text;
                }
                else
                    return null;
            }
            else if (Directive.Active)
            {
                return ValueText.Text;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Guide options.
        /// </summary>
        private ScrollView GuideOptions { get; set; }

        /// <summary>
        /// Guide items.
        /// </summary>
        private VBox GuideItems { get; set; }

        /// <summary>
        /// Add guide item.
        /// </summary>
        private Button AddGuideItem { get; set; }

        /// <summary>
        /// Create components.
        /// </summary>
        private void _initializeComponents()
        {
            VBox ContentBox = new VBox();

            // Options
            Value = new RadioButton(Director.Properties.Resources.JsonValue);
            Directive = new RadioButton(Director.Properties.Resources.FormatDirective);
            Guide = new RadioButton(Director.Properties.Resources.FormatGuide);
            Value.Group = Directive.Group = Guide.Group;
            Value.Active = true;

            // Prepare
            Value.ActiveChanged += Value_ActiveChanged;

            // Add VALUE
            ContentBox.PackStart(Value);
            HBox ValueBox = new HBox();

            // Value option
            ValueOptions = new ComboBox()
            {
                WidthRequest = 80
            };
            ValueOptions.Items.Add("String");
            ValueOptions.Items.Add("Integer");
            ValueOptions.Items.Add("Double");
            ValueOptions.Items.Add("Null");
            ValueOptions.SelectedIndex = 0;
            ValueBox.PackStart(ValueOptions);

            // Value text edit
            ValueText = new TextEntry()
            {
                ExpandVertical = false,
                ExpandHorizontal = true
            };
            ValueBox.PackStart(ValueText);
            ValueText.Changed += ValueText_Changed;
            ContentBox.PackStart(ValueBox);


            // Add Directive
            ContentBox.PackStart(Directive);
            DirectiveText = new TextEntry()
            {
                Sensitive = false
            };
            ContentBox.PackStart(DirectiveText, false, true);

            // Add Guide
            ContentBox.PackStart(Guide);

            // Add guide item
            AddGuideItem = new Button(Image.FromResource(DirectorImages.ADD_ICON))
            {
                MinWidth = 30,
                WidthRequest = 30,
                Sensitive = false
            };
            AddGuideItem.Clicked += AddGuideItem_Clicked;
            ContentBox.PackStart(AddGuideItem, vpos: WidgetPlacement.Center, hpos: WidgetPlacement.End);

            // Guide options
            GuideOptions = new ScrollView()
            {
                ExpandHorizontal = true,
                ExpandVertical = true,
                HorizontalScrollPolicy = ScrollPolicy.Never,
                VerticalScrollPolicy = ScrollPolicy.Always,
                Sensitive = false
            };
            ContentBox.PackStart(GuideOptions, expand: true, fill: true);


            OutputExampleLabel = new Label()
            {
                Sensitive = false
            };
            ContentBox.PackStart(OutputExampleLabel, expand: false, fill: true);

            // Set content box
            Content = ContentBox;
        }

        /// <summary>
        /// Add item.
        /// </summary>
        void AddGuideItem_Clicked(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Control propriet type.
        /// </summary>
        void ValueText_Changed(object sender, EventArgs e)
        {
            String text = ValueText.Text;
            var i = ValueOptions.SelectedIndex;

            if (i == TYPE_INT)
            {
                try
                {
                    ValueText.Text = int.Parse(text) + "";
                }
                catch
                {
                    ValueText.Text = "0";
                    MessageDialog.ShowError(Director.Properties.Resources.InvalidNumberInput);
                }
            }
            else if (i == TYPE_DOUBLE)
            {
                try
                {
                    ValueText.Text = double.Parse(text, CultureInfo.InvariantCulture.NumberFormat) + "";
                }
                catch
                {
                    ValueText.Text = "0.0";
                    MessageDialog.ShowError(Director.Properties.Resources.InvalidNumberInput);
                }
            }
        }

        /// <summary>
        /// Change value.
        /// </summary>
        void Value_ActiveChanged(object sender, EventArgs e)
        {
            // Value active
            ValueOptions.Sensitive = Value.Active;
            ValueText.Sensitive = Value.Active;

            // Directive active
            DirectiveText.Sensitive = Directive.Active;

            // Guide options
            GuideOptions.Sensitive = Guide.Active;
            OutputExampleLabel.Sensitive = Guide.Active;
            AddGuideItem.Sensitive = Guide.Active;
        }
    }
}