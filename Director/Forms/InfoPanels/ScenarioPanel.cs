using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Director.DataStructures;

namespace Director.Forms.InfoPanels
{
    public partial class ScenarioPanel : UserControl
    {
        private Scenario _scenario { get; set; }
        private MainWindow _mainWindow { get; set; }

        public ScenarioPanel(MainWindow _mainWindow)
        {
            InitializeComponent();
            this._mainWindow = _mainWindow;
        }

        public void SetScenario(Scenario _scenario)
        {
            this._scenario = _scenario;
            ScenarioName.Text = _scenario.Name;
            DText.Text = _scenario.Id + " - pos: " + _scenario.Position;
        }

        private void ScenarioName_Leave(object sender, EventArgs e)
        {
            // Get new server name
            String _scenarioName = ScenarioName.Text;

            if (_scenarioName.Count() == 0)
            {
                ScenarioNameError.Visible = true;
                ScenarioName.Focus();
            }
            else
            {
                ScenarioNameError.Visible = false;
                _scenario.Name = _scenarioName;

                // Refresh list
                _mainWindow.RefreshTreeView();
            }
        }

        /// <summary>
        /// Change running frequency and save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
