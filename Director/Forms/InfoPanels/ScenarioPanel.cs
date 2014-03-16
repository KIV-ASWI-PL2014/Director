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

        public ScenarioPanel()
        {
            InitializeComponent();
        }
    }
}
