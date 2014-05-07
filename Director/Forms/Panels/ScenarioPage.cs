using System;
using Xwt;
using Director.Forms.Controls;
using Director.DataStructures;
using Xwt.Drawing;
using Director.DataStructures.SupportStructures;
using System.Collections.Generic;

namespace Director.Forms.Panels
{
    /// <summary>
    /// Scenario description.
    /// </summary>
    internal class ScenarioPage : PanelBase
    {
        /// <summary>
        /// Active scenario.
        /// </summary>
        public Scenario ActiveScenario { get; set; }

        /// <summary>
        /// Invalid scenario name description.
        /// </summary>
        private Label InvalidScenarioName = new Label()
        {
            Markup = "<b>" + Director.Properties.Resources.InvalidScenarioName + "</b>",
            Visible = false,
            TextColor = Colors.Red,
            TextAlignment = Alignment.End,
            MarginRight = 10
        };

        /// <summary>
        /// Invalid time delay.
        /// </summary>
        private Label InvalidTimeDelay = new Label()
        {
            Markup = "<b>" + Director.Properties.Resources.InvalidTimeDelay + "</b>",
            Visible = false,
            TextColor = Colors.Red,
            TextAlignment = Alignment.End,
            MarginRight = 10
        };

        /// <summary>
        /// Scenario name.
        /// </summary>
        private TextEntry ScenarioName { get; set; }

        /// <summary>
        /// Frequency running.
        /// </summary>
        private ComboBox FrequencyRunning { get; set; }

        /// <summary>
        /// Time delay in seconds from previous scenario running.
        /// </summary>
        private TextEntry TimeDelay { get; set; }

        /// <summary>
        /// Run by frequency.
        /// </summary>
        private RadioButton PeriodicityRunning { get; set; }

        /// <summary>
        /// Time Delay running.
        /// </summary>
        private RadioButton TimeDelayRunning { get; set; }

        /// <summary>
        /// Scenario information message.
        /// </summary>
        private MarkdownView ScenarioInformationMessage { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="_window"></param>
        public ScenarioPage(MainWindow _window)
            : base(_window, Director.Properties.Resources.ScenarioInfoBox, DirectorImages.SCENARIO_IMAGE)
        {
        }

        /// <summary>
        /// Set scenario and other properties.
        /// </summary>
        /// <param name="_scenario"></param>
        public void SetScenario(Scenario _scenario)
        {
            ActiveScenario = _scenario;

            // Skip Change Event
            ScenarioName.Changed -= ScenarioName_Changed;
            ScenarioName.Text = _scenario.Name;
            ScenarioName.Changed += ScenarioName_Changed;

            // Runing frequency
            FrequencyRunning.SelectedIndex = ActiveScenario.RunningFrequency;

            // Time dealay time
            TimeDelay.Text = ActiveScenario.TimeAfterPrevious + "";

            // Running type
            PeriodicityRunning.Active = ActiveScenario.PeriodicityRunning;
            ChangeFrequencyOption(null, null);

            // Markdown summary
            ScenarioInformationMessage.Markdown = GetScenarioSummary();
        }

        /// <summary>
        /// Initialize components.
        /// </summary>
        public override void _initializeComponents()
        {
            Frame f = new Frame()
            {
                Label = Director.Properties.Resources.ScenarioLabel,
                Padding = 10
            };
            VBox ScenarioSettings = new VBox();

            // Create scenario name
            ScenarioName = new TextEntry();
            ScenarioName.Changed += ScenarioName_Changed;
            ScenarioSettings.PackStart(new Label(Director.Properties.Resources.ScenarioName));
            ScenarioSettings.PackStart(ScenarioName);
            ScenarioSettings.PackStart(InvalidScenarioName);
            f.Content = ScenarioSettings;
            PackStart(f);

            Frame h = new Frame()
            {
                Label = Director.Properties.Resources.RunningOptionsLabel,
                Padding = 10
            };
            VBox RunningOptionsSettings = new VBox();

            // Select scenario run
            PeriodicityRunning = new RadioButton(Director.Properties.Resources.FrequencyLabel);
            TimeDelayRunning = new RadioButton(Director.Properties.Resources.TimeDelayLabel);
            PeriodicityRunning.Group = TimeDelayRunning.Group;
            RunningOptionsSettings.PackStart(PeriodicityRunning);
            RunningOptionsSettings.PackStart(TimeDelayRunning);
            PeriodicityRunning.Group.ActiveRadioButtonChanged += ChangeFrequencyOption;

            // Frequency settings
            RunningOptionsSettings.PackStart(new Label()
            {
                Text = Director.Properties.Resources.RunningPeriodicity
            });
            FrequencyRunning = new ComboBox();
            FrequencyHelper.FillComboBox(FrequencyRunning);
            RunningOptionsSettings.PackStart(FrequencyRunning);
            FrequencyRunning.SelectedIndex = 0;
            FrequencyRunning.SelectionChanged += FrequencyRunning_SelectionChanged;

            // Time delay settings
            RunningOptionsSettings.PackStart(new Label()
            {
                Text = Director.Properties.Resources.TimeDelayInSeconds
            });
            TimeDelay = new TextEntry()
            {
                Text = "0"
            };
            TimeDelay.Changed += delegate
            {
                try
                {
                    ActiveScenario.TimeAfterPrevious = int.Parse(TimeDelay.Text);

                    InvalidTimeDelay.Visible = false;
                }
                catch
                {
                    InvalidTimeDelay.Visible = true;
                }
            };
            RunningOptionsSettings.PackStart(TimeDelay);
            RunningOptionsSettings.PackStart(InvalidTimeDelay);

            // Add to form
            h.Content = RunningOptionsSettings;
            PackStart(h);


            // Scenario information
            Frame si = new Frame()
            {
                Label = Director.Properties.Resources.ScenarioOverview,
                Padding = 10
            };

            ScrollView ScenarioInformationScrollView = new ScrollView()
            {
                VerticalScrollPolicy = ScrollPolicy.Automatic
            };

            ScenarioInformationMessage = new MarkdownView();
            ScenarioInformationScrollView.Content = ScenarioInformationMessage;
            si.Content = ScenarioInformationScrollView;
            PackStart(si, true, true);
        }

        /// <summary>
        /// Return markdown scenario summary.
        /// </summary>
        /// <returns></returns>
        private String GetScenarioSummary()
        {
            string ret = "";

            // List of requests
            ret += "# Requests\n";

            // Collect al request names
            foreach (Request i in ActiveScenario.Requests)
            {
                ret += "- " + i.Name + "\n";
            }

            ret += "\n";

            // Variables
            ret += "# Variables\n";

            foreach (KeyValuePair<string, string> pair in ActiveScenario.customVariables)
            {
                ret += string.Format("* {0} - {1}\n", pair.Key, pair.Value);
            }

            ret += "\n";

            return ret;
        }

        /// <summary>
        /// Change radio button frequency running options.
        /// </summary>
        private void ChangeFrequencyOption(object sender, EventArgs e)
        {
            bool periodicity = PeriodicityRunning.Group.ActiveRadioButton == PeriodicityRunning;
            ActiveScenario.PeriodicityRunning = periodicity;
            FrequencyRunning.Sensitive = periodicity;
            TimeDelay.Sensitive = !periodicity;
        }

        /// <summary>
        /// Handle changing running frequencíes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrequencyRunning_SelectionChanged(object sender, EventArgs e)
        {
            ActiveScenario.RunningFrequency = FrequencyRunning.SelectedIndex;
        }

        /// <summary>
        /// Change scenario name - save to server and change tree view description.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioName_Changed(object sender, EventArgs e)
        {
            ActiveScenario.Name = ScenarioName.Text;
            bool invalid = ScenarioName.Text.Length == 0;
            InvalidScenarioName.Visible = invalid;
            if (invalid)
                ScenarioName.SetFocus();
            CurrentMainWindow.UpdateTreeStoreText(ActualPosition, ActiveScenario.Name);
        }
    }
}