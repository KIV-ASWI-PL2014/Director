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
    }
}
