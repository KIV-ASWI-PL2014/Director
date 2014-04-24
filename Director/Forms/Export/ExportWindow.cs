using Director.DataStructures;
using Director.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Export
{
    /// <summary>
    /// Support structure for list of checkboxes.
    /// </summary>
    public struct CheckBoxListItem
    {
        public CheckBox ScenarioCheckBox;
        public Scenario ScenarioInstance;
    }

    /// <summary>
    /// Export window.
    /// </summary>
    internal class ExportWindow : Window
    {
        /// <summary>
        /// Active server.
        /// </summary>
        private Server ActiveServer { get; set; }

        /// <summary>
        /// Scenario view.
        /// </summary>
        private ListView ScenarioView { get; set; }

        /// <summary>
        /// Export path.
        /// </summary>
        private TextEntry ExportPath { get; set; }

        /// <summary>
        /// Scenario list widget.
        /// </summary>
        private ScenarioList ScenarioListWidget { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server"></param>
        public ExportWindow(Server server)
        {
            // Set active server 
            ActiveServer = server;

            /// Title and Initial size
            Title = Director.Properties.Resources.ExportDialog;

            // Set default size
            Width = 450;
            Height = 500;

            // This window can not be maximalized
            Resizable = false;

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Initialize content
            _initializeComponents();
        }

        /// <summary>
        /// Initialize components.
        /// </summary>
        private void _initializeComponents()
        {
            InfoBox _infoBox = new InfoBox(Director.Properties.Resources.ExportDialog, DirectorImages.SERVER_IMAGE);
            VBox _contentBox = new VBox();
            _contentBox.PackStart(_infoBox);

            // Create scenarios frame
            Frame f = new Frame()
            {
                Label = Director.Properties.Resources.SelectScenarios,
                Padding = 10
            };

            // Scenario view
            ScenarioListWidget = new ScenarioList(ActiveServer);
            f.Content = ScenarioListWidget;

            // Add to content box
            _contentBox.PackStart(f);

            // Add Export path
            ExportPath = new TextEntry()
            {
                Sensitive = false,
                HorizontalPlacement = WidgetPlacement.Fill
            };

            // Button for selecting path
            Button SelectPath = new Button("...")
            {
                WidthRequest = 35,
                MinWidth = 35,
                HorizontalPlacement = WidgetPlacement.End
            };
            SelectPath.Clicked += delegate
            {
                SaveFileDialog dlg = new SaveFileDialog(Director.Properties.Resources.DialogSaveScenario)
                {
                    Multiselect = false,
                    InitialFileName = ActiveServer.Name
                };
                dlg.Filters.Add(new FileDialogFilter("Director files", "*.adfe"));
                if (dlg.Run() && dlg.FileNames.Count() == 1)
                    ExportPath.Text = dlg.FileName;
            };
            Frame save = new Frame()
            {
                Label = Director.Properties.Resources.DialogSaveScenario,
                Padding = 10
            };
            HBox SaveBox = new HBox();
            SaveBox.PackStart(ExportPath, true, true);
            SaveBox.PackStart(SelectPath, false, false);
            save.Content = SaveBox;
            _contentBox.PackStart(save);

            // Save button
            Button SaveBtn = new Button(Director.Properties.Resources.SaveServer)
            {
                HorizontalPlacement = WidgetPlacement.End,
                ExpandHorizontal = false,
                ExpandVertical = false,
                WidthRequest = (Config.Cocoa) ? 95 : 80,
                MinWidth = (Config.Cocoa) ? 95 : 80,
                Image = Image.FromResource(DirectorImages.SAVE_SCENARIO_ICON)
            };
            SaveBtn.Clicked += SaveBtn_Clicked;

            // Add to form
            _contentBox.PackStart(SaveBtn, false, WidgetPlacement.End, WidgetPlacement.End);

            // Set content box as content
            Content = _contentBox;
        }


        /// <summary>
        /// Save scenarios.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBtn_Clicked(object sender, EventArgs e)
        {
            // Items
            List<CheckBoxListItem> items = ScenarioListWidget.SelectedScenarios;

            // Pre-save validations
            if (items.Count() == 0)
            {
                MessageDialog.ShowError(Director.Properties.Resources.NoScenarioSelected);
            }
            else if (ExportPath.Text.Length == 0)
            {
                MessageDialog.ShowError(Director.Properties.Resources.OutputFileNotSelected);
            }
            else
            {
                // List
                String infoList = "Scenario list to export: ";
                Serializator fr = new Serializator();
                foreach (var i in items)
                {
                    fr.SerializeServer(i.ScenarioInstance.ParentServer, ExportPath.Text);
                    //infoList += i.ScenarioInstance.Name + ", ";
                    break;
                }
                Close();
            }
        }
    }

    internal class ScenarioList : Widget
    {
        /// <summary>
        /// Server
        /// </summary>
        private Server ActiveServer { get; set; }

        /// <summary>
        /// Scenario list.
        /// </summary>
        private VBox ScenarioCheckboxList { get; set; }

        /// <summary>
        /// Select all scneraios.
        /// </summary>
        private CheckBox SelectAll { get; set; }

        /// <summary>
        /// Scroll bar.
        /// </summary>
        /// <param name="_server"></param>
        private ScrollView ListScrollBar { get; set; }

        /// <summary>
        /// Scenario checkboxes.
        /// </summary>
        private List<CheckBoxListItem> CheckBoxes = new List<CheckBoxListItem>();

        /// <summary>
        /// Selected scenarios.
        /// </summary>
        public List<CheckBoxListItem> SelectedScenarios
        {
            get { return CheckBoxes.FindAll(s => s.ScenarioCheckBox.State == CheckBoxState.On); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_server"></param>
        public ScenarioList(Server _server)
        {
            // Intialize components
            ActiveServer = _server;

            // Init
            _initializeComponents();

            // Height
            HeightRequest = 270;
            MinHeight = 270;
        }

        /// <summary>
        /// Create all checkboxes in VBox layout.
        /// </summary>
        private void _initializeComponents()
        {
            // Create list
            ScenarioCheckboxList = new VBox();

            // Create select all checkbox
            SelectAll = new CheckBox(Director.Properties.Resources.SelectAll);
            SelectAll.Toggled += SelectAll_Toggled;
            ScenarioCheckboxList.PackStart(SelectAll);

            // Create all scenarios checkboxes
            foreach (Scenario s in ActiveServer.Scenarios)
            {
                CheckBoxListItem c = new CheckBoxListItem()
                {
                    ScenarioCheckBox = new CheckBox(s.Name),
                    ScenarioInstance = s
                };
                c.ScenarioCheckBox.Toggled += ScenarioList_Clicked;
                ScenarioCheckboxList.PackStart(c.ScenarioCheckBox);
                CheckBoxes.Add(c);
            }

            // Scroll view
            ListScrollBar = new ScrollView()
            {
                VerticalScrollPolicy = ScrollPolicy.Automatic,
                HorizontalScrollPolicy = ScrollPolicy.Never,
                Content = ScenarioCheckboxList
            };

            // Add as content
            Content = ListScrollBar;
        }

        /// <summary>
        /// Select all check fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAll_Toggled(object sender, EventArgs e)
        {
            foreach (CheckBoxListItem it in CheckBoxes)
            {
                it.ScenarioCheckBox.State = SelectAll.State;
            }
        }

        /// <summary>
        /// Controll for check all checlist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioList_Clicked(object sender, EventArgs e)
        {
            if (sender is CheckBox)
            {
                CheckBox tmp = (CheckBox) sender;
                if (SelectAll.State == CheckBoxState.On && tmp.State == CheckBoxState.Off)
                {
                    SelectAll.Toggled -= SelectAll_Toggled;
                    SelectAll.State = CheckBoxState.Off;
                    SelectAll.Toggled += SelectAll_Toggled;
                }
            }
        }
    }
}