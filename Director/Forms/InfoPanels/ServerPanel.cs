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
        /// <summary>
        ///  Server settings!
        /// </summary>
        private Server _server;

        /// <summary>
        ///  Main control panel - refresh list from gui.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Intialize server panel.
        /// </summary>
        /// <param name="_mainWindow">Main window instance.</param>
        public ServerPanel(MainWindow _mainWindow)
        {
            InitializeComponent();
            this._mainWindow = _mainWindow;
        }

        /// <summary>
        /// Set server instance.
        /// </summary>
        /// <param name="server"></param>
        public void SetServer(Server server)
        {
            // Set server
            _server = server;

            // Set server name
            ServerName.Text = _server.Name;
            EndPointUrl.Text = _server.GetUrl();
            AuthUsername.Text = _server.AuthName;
            AuthPassword.Text = _server.AuthPassword;

            // Set email data source
            EmailList.DataSource = _server.Emails;
        }

        /// <summary>
        /// Toggle authentication on / off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthenticationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or disable elements
            bool _state = AuthenticationCheckBox.Checked;

            // Set variables and update GUI
            AuthenticationCredentials.Enabled = _state;
            AuthUsername.Enabled = _state;
            AuthPassword.Enabled = _state;
            _server.Authentication = _state;
        }


        /// <summary>
        /// Server name leaving action - save changes!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerName_Leave(object sender, EventArgs e)
        {
            // Get new server name
            String _serverName = ServerName.Text;

            if (_serverName.Count() == 0)
            {
                ServerNameError.Visible = true;
                ServerName.Focus();
            }
            else
            {
                ServerNameError.Visible = false;
                _server.Name = _serverName;

                // Refresh list
                _mainWindow.RefreshTreeView();
            }
        }

        /// <summary>
        /// Set end point url!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndPointUrl_Leave(object sender, EventArgs e)
        {
            // Get api url
            String _apiUrl = EndPointUrl.Text;

            try
            {
                // Set url - throws exception when url is invalid!
                _server.SetUrl(_apiUrl);

                // Hide label
                ErrorUrl.Visible = false;
            }
            catch (Exception)
            {
                // Set label visible and set focus! (invalid url)
                ErrorUrl.Visible = true;
                EndPointUrl.Focus();
            }
        }

        /// <summary>
        /// Set authentication name!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthUsername_Leave(object sender, EventArgs e)
        {
            _server.AuthName = AuthUsername.Text;
        }

        /// <summary>
        /// Set authentication password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthPassword_Leave(object sender, EventArgs e)
        {
            _server.AuthPassword = AuthPassword.Text;
        }
    }
}
