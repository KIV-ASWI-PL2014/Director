using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Controls;
using Director.DataStructures;
using Xwt.Drawing;
using Director.DataStructures.SupportStructures;

namespace Director.Forms.Panels
{
	/// <summary>
	/// Scenario description.
	/// </summary>
	class ScenarioPage : PanelBase
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
        /// Scenario name.
        /// </summary>
        private TextEntry ScenarioName { get; set; }

        /// <summary>
        /// Frequency running.
        /// </summary>
        private ComboBox FrequencyRunning { get; set; }

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
        }

        /// <summary>
        /// Initialize components.
        /// </summary>
        public override void _initializeComponents()
        {
            Frame f = new Frame()
            {
                Label = Director.Properties.Resources.ScenarioSettings,
                Padding = 10
            };
            VBox ScenarioSettings = new VBox();

            // Create scenario name
            ScenarioName = new TextEntry();
            ScenarioName.Changed += ScenarioName_Changed;
            ScenarioSettings.PackStart(new Label(Director.Properties.Resources.ScenarioName));
            ScenarioSettings.PackStart(ScenarioName);
            ScenarioSettings.PackStart(InvalidScenarioName);

            // Frequency settings
            ScenarioSettings.PackStart(new Label()
            {
                Text = Director.Properties.Resources.RunningPeriodicity
            });
            FrequencyRunning = new ComboBox();
            FrequencyHelper.FillComboBox(FrequencyRunning);
            ScenarioSettings.PackStart(FrequencyRunning);
            FrequencyRunning.SelectedIndex = 0;
            FrequencyRunning.SelectionChanged += FrequencyRunning_SelectionChanged;

            // Add to form
            f.Content = ScenarioSettings;
            PackStart(f);
        }

        /// <summary>
        /// Handle changing running frequencíes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FrequencyRunning_SelectionChanged(object sender, EventArgs e)
        {
            ActiveScenario.RunningFrequency = FrequencyRunning.SelectedIndex;
        }

        void ScenarioName_Changed(object sender, EventArgs e)
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
