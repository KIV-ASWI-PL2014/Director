using System;
using Xwt;
using System.Collections.Generic;
using Director.DataStructures;
using Xwt.Drawing;
using Director.Forms.Controls;

namespace Director
{
	/// <summary>
	/// Dialog to run scenario with specific headers!
	/// </summary>
	public class SelectVariables : Dialog
	{
		/// <summary>
		/// Scenario.
		/// </summary>
		/// <value>The active scenario.</value>
		public Scenario ActiveScenario { get; set; }

		/// <summary>
		/// Content box.
		/// </summary>
		public VBox ContentBox { get; set; }

		/// <summary>
		/// Variable content.
		/// </summary>
		/// <value>The content of the variable.</value>
		public VBox VariableContent { get; set; }

		/// <summary>
		/// Default row height.
		/// </summary>
		public const int ROW_HEIGHT = 30;

		/// <summary>
		/// Variable list.
		/// </summary>
		public List<Variable> Variables = new List<Variable>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Director.RunWithHeaders"/> class.
		/// </summary>
		/// <param name="_scenario">_scenario.</param>
		public SelectVariables (Scenario _scenario)
		{
			ActiveScenario = _scenario;
			Width = 500;
			Height = 300;
			Title = "Run scenario with specific variables";

			// Init components
			_initializeComponents ();

			// CLose btns
			Buttons.Add (new DialogButton ("Ok", Command.Ok));
			Buttons.Add (new DialogButton ("Storno", Command.Cancel));

			// Refresh header list
			foreach (KeyValuePair<string, string> s in _scenario.customVariables) {

			}
		}


		/// <summary>
		/// Init components.
		/// </summary>
		void _initializeComponents ()
		{
			ContentBox = new VBox ();

			// First line
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
			Button NewVariable = new Button(Image.FromResource(DirectorImages.ADD_ICON))
			{
				MinWidth = 30,
				WidthRequest = 30,
				MarginRight = 30
			};
			FirstLine.PackStart(HeaderType, true, true);
			FirstLine.PackStart(HeaderValue, true, true);
			FirstLine.PackStart(NewVariable, false, false);
			ContentBox.PackStart(FirstLine);

			// New header event
			NewVariable.Clicked += NewVariable_Clicked;

			// Variable content
			VariableContent = new VBox ();
			ScrollView VariableContentSC = new ScrollView () {
				HorizontalScrollPolicy = ScrollPolicy.Never,
				VerticalScrollPolicy = ScrollPolicy.Always,
				Content = VariableContent,
				BackgroundColor = Colors.LightGray
			};
			ContentBox.PackStart (VariableContentSC, true, true);

			// Create temporary data
			int i = 0;
			foreach (KeyValuePair<string, string> kvp in ActiveScenario.customVariables) {
				var tmp = new Variable (this, kvp.Key, kvp.Value, (i % 2 == 0) ? Colors.LightGray : Colors.White);
				Variables.Add (tmp);
				VariableContent.PackStart (tmp);
				i++;
			}


			// Content
			Content = ContentBox;
		}

		/// <summary>
		/// Add new variable.
		/// </summary>
		public void NewVariable_Clicked(object sender, EventArgs e)
		{
			var tmp = new Variable (this, "", "", (Variables.Count % 2 == 0) ? Colors.LightGray : Colors.White);
			Variables.Add (tmp);
			VariableContent.PackStart (tmp);
		}

		/// <summary>
		/// Remove variable.
		/// </summary>
		/// <param name="var">Variable.</param>
		public void RemoveVariable(Variable variable)
		{
			Variables.Remove (variable);
			VariableContent.Remove (variable);
		}

		/// <summary>
		/// Return new variable list.
		/// </summary>
		public Dictionary<String, String> NewVariableList()
		{
			// Return dicitionary...
			Dictionary<String, String> ret = new Dictionary<string, string> ();

			// Iterate variables
			foreach (var v in Variables)
				ret.Add (v.Key, v.Value);

			return ret;
		}
	}

	/// <summary>
	/// Variable helper class.
	/// </summary>
	public class Variable : HBox
	{
		/// <summary>
		/// Key.
		/// </summary>
		/// <value>The key.</value>
		public String Key { get; set; }

		/// <summary>
		/// Value.
		/// </summary>
		/// <value>The value.</value>
		public String Value { get; set; }

		/// <summary>
		/// Parent dialog.
		/// </summary>
		/// <value>The parent dialog.</value>
		public SelectVariables ParentDialog { get; set; }

		/// <summary>
		/// Default background color.
		/// </summary>
		/// <value>The default background.</value>
		private Color DefaultColor { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public Variable(SelectVariables _parent, String k, String v, Color c)
		{
			// Set variables
			ParentDialog = _parent;
			Key = k;
			Value = v;
			BackgroundColor = DefaultColor = c;

			// Set height
			MinHeight = SelectVariables.ROW_HEIGHT;

			// Horizontal resize true
			ExpandHorizontal = true;

			// No margins
			Margin = 0;

			// Init component
			_initializeComponents();
		}

		/// <summary>
		/// Init components.
		/// </summary>
		void _initializeComponents ()
		{
			TextEntry KeyTE = new TextEntry () {
				Text = Key,
				MarginLeft = 5,
				VerticalPlacement = WidgetPlacement.Center,
				HorizontalPlacement = WidgetPlacement.Fill
			};
			KeyTE.Changed += delegate {
				Key = KeyTE.Text;
			};

			TextEntry ValueTE = new TextEntry() {
				Text = Value,
				MarginLeft = 5,
				VerticalPlacement = WidgetPlacement.Center,
				HorizontalPlacement = WidgetPlacement.Fill
			};
			ValueTE.Changed += delegate {
				Value = ValueTE.Text;
			};

			Button RemoveBtn = new Button(Image.FromResource(DirectorImages.CROSS_ICON))
			{
				MarginRight = 20,
				HorizontalPlacement = WidgetPlacement.Center,
				VerticalPlacement = WidgetPlacement.Center,
				ExpandHorizontal = false,
				ExpandVertical = false
			};
			RemoveBtn.Clicked += RemoveBtn_Clicked;

			PackStart(KeyTE, expand: true);
			PackStart(ValueTE, expand: true);
			PackStart(RemoveBtn, expand: false, fill: false);
		}

		/// <summary>
		/// Remove this variable.
		/// </summary>
		public void RemoveBtn_Clicked(object sender, EventArgs e) {
			ParentDialog.RemoveVariable (this);
		}
	}
}

