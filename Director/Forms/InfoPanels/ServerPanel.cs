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
    public partial class ServerPanel : UserControl
    {
        private Server _server;

        public ServerPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set server instance.
        /// </summary>
        /// <param name="server"></param>
        public void SetServer(Server server)
        {
            _server = server;
        }

        private void AuthenticationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or disable elements
            if (AuthenticationCheckBox.Checked == true)
            {
                AuthenticationCredentials.Enabled = true;
                AuthUsername.Enabled = true;
                AuthPassword.Enabled = true;
            }
            else
            {
                AuthenticationCredentials.Enabled = false;
                AuthUsername.Enabled = false;
                AuthPassword.Enabled = false;
            }
        }
    }
}
