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
            EditVariable _edit = new EditVariable(ActiveParserItem, ActiveRequest);

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

    internal class EditVariable : Dialog
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
        private RadioButton FormatGuide { get; set; }

        /// <summary>
        /// Variables.
        /// </summary>
        public List<ParserOccurence> Occurences { get; set; }

        /// <summary>
        /// Format value items.
        /// </summary>
        public List<FormatValue> FormatValues { get; set; }

        /// <summary>
        /// Format wrapper package.
        /// </summary>
        public VBox FormatWrapper { get; set; }

        /// <summary>
        /// Foramt items.
        /// </summary>
        public VBox FormatItems { get; set; }

        /// <summary>
        /// New format.
        /// </summary>
        public Button NewFormat { get; set; }

        /// <summary>
        /// Example output.
        /// </summary>
        public Label ExampleOutput { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditVariable(ParserItem _item, Request _request)
        {
            // Set title, icon, resizable
            Title = Director.Properties.Resources.EditVariable;
            Icon = Image.FromResource(DirectorImages.EDIT_CONTENT_ICON);
            Resizable = false;

            // Set variables
            ActiveItem = _item;
            ActiveRequest = _request;
            Occurences = new List<ParserOccurence>();
            FormatValues = new List<FormatValue>();

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

            if (it.value is System.Boolean)
            {
                ValueType.SelectedIndex = (int) DataType.TYPE_BOOL;
                ValueText.Text = ((System.Boolean)it.value) ? "true" : "false";
            }
            else if (it.value is System.Int64 || it.value is System.Int32)
            {
                ValueType.SelectedIndex = (int) DataType.TYPE_INT;
                ValueText.Text = it.value + "";
            }
            else if (it.value is System.Double)
            {
                ValueType.SelectedIndex = (int) DataType.TYPE_DOUBLE;
                ValueText.Text = (it.value + "").Replace(',', '.');
            }
            else if (it.value == null)
            {
                ValueType.SelectedIndex = (int) DataType.TYPE_NULL;
                ValueText.Text = "";
            }
            else
            {
                // Parse occurrences
                List<ParserError> errors = new List<ParserError>();
                Occurences = Parser.findMarkUpOccurences((String)it.value, ActiveRequest.ParentScenario.customVariables, errors);
                if (Occurences.Count == 1 && errors.Count == 0 && Occurences[0].type == "text")
                {
                    ValueType.SelectedIndex = (int)DataType.TYPE_STRING;
                    ValueText.Text = (String)it.value;
                }
                else
                    RefreshOccurencies();
            }
        }

        /// <summary>
        /// Refresh occurences.
        /// </summary>
        private void RefreshOccurencies()
        {
            FormatGuide.Active = true;
            FormatValues.Clear();
            FormatWrapper.Clear();

            // Iterate occurencies
            foreach (var p in Occurences)
            {
                var tmp = new FormatValue(p, this, (FormatValues.Count % 2 == 0) ? Colors.LightGray : Colors.White);
                FormatValues.Add(tmp);
                FormatWrapper.PackStart(tmp);
            }
        }

        /// <summary>
        /// Remove item from view.
        /// </summary>
        /// <param name="v"></param>
        public void RemoveItem(FormatValue v)
        {
            FormatValues.Add(v);
            FormatWrapper.Remove(v);
            // Recolorized!
            int i = 0;
            foreach (var s in FormatValues)
            {
                s.BackgroundColor = (i % 2 == 0) ? Colors.LightGray : Colors.White;
                i++;
            }
        }

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
            ValueBox.PackStart(ValueType);

            // Value
            ValueText = new TextEntry()
            {
                ExpandVertical = false,
                ExpandHorizontal = true,
                MinWidth = 250
            };
            ValueBox.PackStart(ValueText);
            ContentBox.PackStart(ValueBox);

            // Guide
            FormatGuide = new RadioButton(Director.Properties.Resources.FormatGuide);
            ContentBox.PackStart(FormatGuide);
            Value.Group = FormatGuide.Group;
            Value.Active = true;
            Value.Group.ActiveRadioButtonChanged += Group_ActiveRadioButtonChanged;

            // First line
            HBox FirstLine = new HBox();

            // Value option
            Label Variable = new Label(Director.Properties.Resources.VariableType)
            {
                HorizontalPlacement = WidgetPlacement.Center,
                ExpandHorizontal = true,
                ExpandVertical = false,
                MarginLeft = 10
            };
            Label VariableValue = new Label(Director.Properties.Resources.VariableValue) {
                ExpandHorizontal = true,
                ExpandVertical = false,
                HorizontalPlacement = WidgetPlacement.Center
            };
            NewFormat = new Button(Image.FromResource(DirectorImages.ADD_ICON))
            {
                MinWidth = 30,
                WidthRequest = 30,
                MarginRight = 30
            };
            FirstLine.PackStart(Variable, true, true);
            FirstLine.PackStart(VariableValue, true, true);
            FirstLine.PackStart(NewFormat, false, false);
            ContentBox.PackStart(FirstLine);
            NewFormat.Clicked += NewFormat_Clicked;

            // Variables
            FormatWrapper = new VBox();
            ScrollView FormatWrapperSV = new ScrollView()
            {
                HorizontalScrollPolicy = ScrollPolicy.Never,
                VerticalScrollPolicy = ScrollPolicy.Always,
                Content = FormatWrapper,
                BackgroundColor = Colors.LightGray
            };
            ContentBox.PackStart(FormatWrapperSV, true, true);

            // Example output
            ExampleOutput = new Label()
            {
                ExpandHorizontal = true
            };
            ContentBox.PackStart(ExampleOutput, false, true);


            Content = ContentBox;
        }

        /// <summary>
        /// Add new format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NewFormat_Clicked(object sender, EventArgs e)
        {
            var tmp = FormatValue.CreateFormatValue(this, (FormatValues.Count % 2 == 0) ? Colors.LightGray : Colors.White);
            FormatValues.Add(tmp);
            FormatWrapper.PackStart(tmp);
            int i = 0;
            foreach (var s in FormatValues)
            {
                s.BackgroundColor = (i % 2 == 0) ? Colors.LightGray : Colors.White;
                i++;
            }
        }

        /// <summary>
        /// Radio button active changed.
        /// </summary>
        void Group_ActiveRadioButtonChanged(object sender, EventArgs e)
        {
            ValueType.Sensitive = Value.Active;
            ValueText.Sensitive = Value.Active;
            NewFormat.Sensitive = !Value.Active;
            FormatWrapper.Sensitive = !Value.Active;
        }

        /// <summary>
        /// Return object.
        /// </summary>
        /// <returns></returns>
        internal object GetValue()
        {
            if (Value.Active)
            {
                var i = (DataType) ValueType.SelectedItem;
                var text = ValueText.Text;

                if (i == DataType.TYPE_INT)
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
                else if (i == DataType.TYPE_BOOL)
                {
                    return text.Trim() == "true" || text.Trim() == "1";
                }
                else if (i == DataType.TYPE_DOUBLE)
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
                else if (i == DataType.TYPE_NULL)
                {
                    return null;
                }
                else
                {
                    return ValueText.Text;
                }
            }
            else
            {
                string ret = "";
                foreach (var i in FormatValues)
                {
                    ret += i.GetValue();
                }
                return ret;
            }
        }
    }

    /// <summary>
    /// Form Value.
    /// </summary>
    internal class FormatValue : VBox
    {
        /// <summary>
        /// Ocurence.
        /// </summary>
        public ParserOccurence Occruence { get; set; }

        /// <summary>
        /// Parent edit variable.
        /// </summary>
        public EditVariable ParentEdit { get; set; }

        /// <summary>
        /// Type.
        /// </summary>
        public ComboBox Type { get; set; }

        /// <summary>
        /// Format value.
        /// </summary>
        /// <param name="_window"></param>
        /// <param name="_color"></param>
        public static FormatValue CreateFormatValue(EditVariable _window, Color _color)
        {
            List<string> vars = new List<string>();
            ParserOccurence p = new ParserOccurence("", "text", vars);
            return new FormatValue(p, _window, _color);
        }

        /// <summary>
        /// Parserv value new.
        /// </summary>
        public FormatValue(ParserOccurence occurence, EditVariable _window, Color _color)
        {
            // Parent occurence
            Occruence = occurence;

            // Parent window
            ParentEdit = _window;

            // Color
            BackgroundColor = _color;

            // Horizontal resize true
            ExpandHorizontal = true;

            // No margins
            Margin = 0;
            MinHeight = 30;

            // Init components
            _initComponents();
        }

        /// <summary>
        /// Active component.
        /// </summary>
        public Widget ActiveComponent { get; set; }

        /// <summary>
        /// Component content.
        /// </summary>
        public VBox ComponentContent { get; set; }

        private enum WidgetTypes : int
        {
            TEXT = 1, VARIABLE, RAND_INT, RAND_STRING, RAND_FLOAT, SEQUENCE
        };


        /// <summary>
        /// Generate components.
        /// </summary>
        private void _initComponents()
        {
            Type = new ComboBox()
            {
                MarginLeft = 5,
                VerticalPlacement = WidgetPlacement.Center,
                HorizontalPlacement = WidgetPlacement.Fill,
                MarginRight = 5,
                MarginTop = 5
            };
            Type.Items.Add(WidgetTypes.TEXT, "Text");
            Type.Items.Add(WidgetTypes.VARIABLE, "Variable");
            Type.Items.Add(WidgetTypes.RAND_INT, "Random integer");
            Type.Items.Add(WidgetTypes.RAND_STRING, "Random string");
            Type.Items.Add(WidgetTypes.RAND_FLOAT, "Random float");
            Type.Items.Add(WidgetTypes.SEQUENCE, "Sequence");

            HBox HorizontalOption = new HBox();
            HorizontalOption.PackStart(Type, expand: true);

            Button RemoveBtn = new Button(Image.FromResource(DirectorImages.CROSS_ICON))
            {
                MarginRight = 5,
                HorizontalPlacement = WidgetPlacement.Center,
                VerticalPlacement = WidgetPlacement.Center,
                ExpandHorizontal = false,
                ExpandVertical = false,
                MarginTop = 5
            };
            HorizontalOption.PackStart(RemoveBtn, expand: false, fill: false);
            RemoveBtn.Clicked += RemoveBtn_Clicked;
            PackStart(HorizontalOption, expand: true);

            // Prepare contents
            ComponentContent = new VBox()
            {
                MarginBottom = (BackgroundColor == Colors.White) ? 10 : 5
            };
            PackStart(ComponentContent, expand: true);

            // Bind handler
            Type.SelectionChanged += Type_SelectionChanged;

            // Select text default
            Type.SelectedIndex = 0;

            // Prepare occurences
            var type = Occruence.type;
            var fct = Occruence.name;

            if (type == "text")
            {
                Type.SelectedItem = WidgetTypes.TEXT;
            }
            else if (type == "variable")
            {
                Type.SelectedItem = WidgetTypes.VARIABLE;
            }
            else
            {
                if (fct == "randInt")
                {
                    Type.SelectedItem = WidgetTypes.RAND_INT;
                }
                else if (fct == "randString")
                {
                    Type.SelectedItem = WidgetTypes.RAND_STRING;
                }
                else if (fct == "randFloat")
                {
                    Type.SelectedItem = WidgetTypes.RAND_FLOAT;
                }
                else
                {
                    Type.SelectedItem = WidgetTypes.SEQUENCE;
                }
            }

            // Set data
            ((IVariable)ActiveComponent).SetData(Occruence.name, Occruence.arguments);
        }

        /// <summary>
        /// Remove.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveBtn_Clicked(object sender, EventArgs e)
        {
            ParentEdit.RemoveItem(this);
        }

        private TextVariable TextVariableComponent = null;
        private VariableVariable VariableComponent = null;
        private NumberVariable NumberComponent = null;
        private FloatVariable FloatComponent = null;
        private StringVariable StringComponent = null;
        private SequenceVariable SequenceComponent = null;

        /// <summary>
        /// Change items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Type_SelectionChanged(object sender, EventArgs e)
        {
            if (ActiveComponent != null)
                ComponentContent.Remove(ActiveComponent);

            WidgetTypes type = (WidgetTypes)Type.SelectedItem;

            switch (type)
            {
                case WidgetTypes.SEQUENCE:
                    if (SequenceComponent == null)
                        SequenceComponent = new SequenceVariable();
                    ActiveComponent = SequenceComponent;
                    SequenceComponent.ClearData();
                    ComponentContent.PackStart(SequenceComponent, true, true);
                    break;

                case WidgetTypes.VARIABLE:
                    if (VariableComponent == null)
                        VariableComponent = new VariableVariable();
                    ActiveComponent = VariableComponent;
                    VariableComponent.ClearData();
                    ComponentContent.PackStart(VariableComponent, true, true);
                    break;

                case WidgetTypes.RAND_INT:
                    if (NumberComponent == null)
                        NumberComponent = new NumberVariable();
                    ActiveComponent = NumberComponent;
                    NumberComponent.ClearData();
                    ComponentContent.PackStart(NumberComponent, true, true);
                    break;

                case WidgetTypes.RAND_STRING:
                    if (StringComponent == null)
                        StringComponent = new StringVariable();
                    ActiveComponent = StringComponent;
                    StringComponent.ClearData();
                    ComponentContent.PackStart(StringComponent, true, true);
                    break;

                case WidgetTypes.RAND_FLOAT:
                    if (FloatComponent == null)
                        FloatComponent = new FloatVariable();
                    ActiveComponent = FloatComponent;
                    FloatComponent.ClearData();
                    ComponentContent.PackStart(FloatComponent, true, true);
                    break;


                default:
                    if (TextVariableComponent == null)
                        TextVariableComponent = new TextVariable();
                    ActiveComponent = TextVariableComponent;
                    TextVariableComponent.ClearData();
                    ComponentContent.PackStart(TextVariableComponent, true, true);
                    break;
            }
        }   

        /// <summary>
        /// Get value.
        /// </summary>
        /// <returns></returns>
        public String GetValue()
        {
            if (ActiveComponent != null)
            {
                return ((IVariable)ActiveComponent).GetRepresentation();
            }
            return "";
        }
    
    }

    public interface IVariable
    {
        String GetRepresentation();
        void ClearData();
        void SetData(String data, List<string> variables);
    }

    internal class SequenceVariable : VBox, IVariable
    {
        TextEntry Min { get; set; }
        TextEntry Max { get; set; }
        TextEntry Add { get; set; }
        TextEntry Vars { get; set; }

        public SequenceVariable()
        {
            HBox FL = new HBox()
            {
                ExpandHorizontal = true
            };
            HBox SL = new HBox()
            {
                ExpandHorizontal = true
            };
            FL.PackStart(new Label("Start: ")
            {
                MarginLeft = 5
            });
            Min = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            FL.PackStart(Min, true, true);

            FL.PackStart(new Label("End: "));
            Max = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            FL.PackStart(Max, true, true);

            SL.PackStart(new Label("Add: ")
            {
                MarginLeft = 5
            });

            Add = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            SL.PackStart(Add, true, true);

            SL.PackStart(new Label("Var: "));

            Vars = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            SL.PackStart(Vars, true, true);

            PackStart(FL, true, true);
            PackStart(SL, true, true);
        }

        /// <summary>
        /// Return string representation.
        /// </summary>
        /// <returns></returns>
        public String GetRepresentation()
        {
            return string.Format("#sequence({0},{1},{2},{3})#", Min.Text, Max.Text, Add.Text, Vars.Text);
        }

        /// <summary>
        /// Clear data.
        /// </summary>
        public void ClearData()
        {
            Min.Text = Max.Text = Add.Text = Vars.Text = "";
        }

        /// <summary>
        /// Set data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="variables"></param>
        public void SetData(String data, List<string> variables)
        {
            if (variables.Count == 4)
            {
                Min.Text = variables[0] + "";
                Max.Text = variables[1] + "";
                Add.Text = variables[2] + "";
                Vars.Text = variables[3] + "";
            }
        }
    }

    internal class StringVariable : HBox, IVariable
    {
        TextEntry Min { get; set; }
        TextEntry Max { get; set; }
        TextEntry Characters { get; set; }

        public StringVariable()
        {
            PackStart(new Label("Min: ")
            {
                MarginLeft = 5
            });
            Min = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Min, true, true);

            PackStart(new Label("Max: "));
            Max = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Max, true, true);

            PackStart(new Label("Symbols: "));

            Characters = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Characters, true, true);
        }

        /// <summary>
        /// Return string representation.
        /// </summary>
        /// <returns></returns>
        public String GetRepresentation()
        {
            return string.Format("#randString({0},{1},{2})#", Min.Text, Max.Text, Characters.Text);
        }

        /// <summary>
        /// Clear data.
        /// </summary>
        public void ClearData()
        {
            Min.Text = Max.Text = Characters.Text = "";
        }

        /// <summary>
        /// Set data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="variables"></param>
        public void SetData(String data, List<string> variables)
        {
            if (variables.Count == 3)
            {
                Min.Text = variables[0] + "";
                Max.Text = variables[1] + "";
                Characters.Text = variables[2] + "";
            }
        }
    }

    internal class FloatVariable : HBox, IVariable
    {
        TextEntry Min { get; set; }
        TextEntry Max { get; set; }
        TextEntry Precision { get; set; }

        public FloatVariable()
        {
            PackStart(new Label("Min: ")
            {
                MarginLeft = 5
            });
            Min = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Min, true, true);

            PackStart(new Label("Max: "));
            Max = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Max, true, true);

            PackStart(new Label("Precision: "));

            Precision = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Precision, true, true);
        }

        /// <summary>
        /// Return string representation.
        /// </summary>
        /// <returns></returns>
        public String GetRepresentation()
        {
            return string.Format("#randFloat({0},{1},{2})#", Min.Text, Max.Text, Precision.Text);
        }

        /// <summary>
        /// Clear data.
        /// </summary>
        public void ClearData()
        {
            Min.Text = Max.Text = Precision.Text = "";
        }

        /// <summary>
        /// Set data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="variables"></param>
        public void SetData(String data, List<string> variables)
        {
            if (variables.Count == 3)
            {
                Min.Text = variables[0] + "";
                Max.Text = variables[1] + "";
                Precision.Text = variables[2] + "";
            }
        }
    }

    internal class NumberVariable : HBox, IVariable
    {
        TextEntry Min { get; set; }
        TextEntry Max { get; set; }

        public NumberVariable()
        {
            PackStart(new Label("Min: ")
            {
                MarginLeft = 5
            });
            Min = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Min, true, true);

            PackStart(new Label("Max: "));
            Max = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Max, true, true);
        }

        /// <summary>
        /// Return string representation.
        /// </summary>
        /// <returns></returns>
        public String GetRepresentation()
        {
            return string.Format("#randInt({0},{1})#", Min.Text, Max.Text);
        }

        /// <summary>
        /// Clear data.
        /// </summary>
        public void ClearData()
        {
            Min.Text = Max.Text = "";
        }

        /// <summary>
        /// Set data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="variables"></param>
        public void SetData(String data, List<string> variables)
        {
            if (variables.Count == 2)
            {
                Min.Text = variables[0] + "";
                Max.Text = variables[1] + "";
            }
        }
    }

    internal class VariableVariable : VBox, IVariable
    {
        /// <summary>
        /// Text entry.
        /// </summary>
        public TextEntry Text { get; set; }

        /// <summary>
        /// Text variable.
        /// </summary>
        public VariableVariable()
        {
            Text = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Text, true, true);
        }

        /// <summary>
        /// Clear data.
        /// </summary>
        public void ClearData()
        {
            Text.Text = "";
        }

        /// <summary>
        /// Set data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="variables"></param>
        public void SetData(String data, List<string> variables)
        {
            Text.Text = data;    
        }


        /// <summary>
        /// Return string representation.
        /// </summary>
        /// <returns></returns>
        public String GetRepresentation()
        {
            return string.Format("${0}$", Text.Text);
        }

    }

    internal class TextVariable : VBox, IVariable 
    {
        /// <summary>
        /// Text entry.
        /// </summary>
        public TextEntry Text { get; set; }

        /// <summary>
        /// Text variable.
        /// </summary>
        public TextVariable()
        {
            Text = new TextEntry()
            {
                MarginLeft = 5,
                MarginRight = 5
            };
            PackStart(Text, true, true);
        }

        /// <summary>
        /// Return string representation.
        /// </summary>
        public String GetRepresentation()
        {
            return Text.Text;
        }

        /// <summary>
        /// Clear data.
        /// </summary>
        public void ClearData()
        {
            Text.Text = "";
        }

        /// <summary>
        /// Set data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="variables"></param>
        public void SetData(String data, List<string> variables)
        {
            Text.Text = data;    
        }
    }
}