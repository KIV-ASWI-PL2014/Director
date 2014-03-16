using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Director.Forms.Export
{
    public partial class ExportSettings : Form
    {
        public ExportSettings()
        {
            InitializeComponent();
        }

        private void SelectPath_Click(object sender, EventArgs e)
        {
            SaveFileDialog _sfd = new SaveFileDialog();
            _sfd.Filter = "API Director Files|*.adf";
            _sfd.Title = "Export API director files";

            if (_sfd.ShowDialog() == DialogResult.OK)
            {
                // Save to file from _ofd.FileName();
                SaveFileName.Text = _sfd.FileName;
            }
        }
    }
}
