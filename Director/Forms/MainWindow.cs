﻿using Director.Forms.InfoPanels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Director.Forms.Help;
using Director.Forms.Export;
using Director.DataStructures;
using Director.Forms.Modules;

namespace Director.Forms
{
    public partial class MainWindow : Form
    {
        // Images constant
        public const int SERVER_IMAGE = 0;
        public const int SCENARIO_IMAGE = 1;
        public const int REQUEST_NOT_SENT = 2;
        public const int REQUEST_OK = 3;
        public const int REQUEST_FAIL = 4;

        // Image processing revert compatibility
        public const int REQUEST_PROCESS_START = 5;
        public const int REQUEST_PROCESS_STOP = 12;

        // Prepare Info Pannels
        private EmptyPanel _emptyPanel = new EmptyPanel();
        private ServerPanel _serverPanel = new ServerPanel();
        private RequestPanel _requestPanel = new RequestPanel();
        private ScenarioPanel _scenarioPanel = new ScenarioPanel();

        // Processing images
        private int _processingImageIndex = REQUEST_PROCESS_START;
        private List<TreeNode> _processingNodes = new List<TreeNode>();

        // Actual selected panel for resizing!
        private UserControl _selectedPanel = null;

        // Actual server parent instance
        private Server _rootServer = null;

        public MainWindow()
        {
            InitializeComponent();

            // Initiate image List for tree view panel
            _initiateTreeViewImages();

            // New empty panel!
            _setUserControl(_emptyPanel);

            // TODO: DEBUG
            NewScenario_Click(null, null);
        }

        /// <summary>
        /// Set user control to main content panel!
        /// </summary>
        /// <param name="panel">New panel for replace</param>
        private void _setUserControl(UserControl panel)
        {
            ContentPanel.Controls.Clear();
            panel.Dock = DockStyle.Fill;
            ContentPanel.Controls.Add(panel);
            _selectedPanel = panel;
        }

        private void _initiateTreeViewImages()
        {
            // Create imagelist
            ImageList myImageList = new ImageList();
            myImageList.Images.Add(Director.Properties.Resources.ServerRoot);
            myImageList.Images.Add(Director.Properties.Resources.lightning);
            myImageList.Images.Add(Director.Properties.Resources.bullet_blue);
            myImageList.Images.Add(Director.Properties.Resources.ok);
            myImageList.Images.Add(Director.Properties.Resources.fail);

            // Processing images
            myImageList.Images.Add(Director.Properties.Resources.processing_0);
            myImageList.Images.Add(Director.Properties.Resources.processing_1);
            myImageList.Images.Add(Director.Properties.Resources.processing_2);
            myImageList.Images.Add(Director.Properties.Resources.processing_3);
            myImageList.Images.Add(Director.Properties.Resources.processing_4);
            myImageList.Images.Add(Director.Properties.Resources.processing_5);
            myImageList.Images.Add(Director.Properties.Resources.processing_6);
            myImageList.Images.Add(Director.Properties.Resources.processing_7);

            // Set images
            ScenarioView.ImageList = myImageList;
            ScenarioView.ImageIndex = 0;
            ScenarioView.SelectedImageIndex = 0;
        }

        private void CreateTreeView()
        {
            // Fill a bit
            TreeNode rootNode = CreateServerNode("Server #1");

            // Create several Scenario nodes
            TreeNode[] nodes = new TreeNode[4];
            for (int i = 0; i < 4; i++)
            {
                nodes[i] = CreateScenarioNode("Scenario #" + i.ToString());

                TreeNode[] requestNodes = new TreeNode[4];

                requestNodes[0] = CreateRequestNode("Request #1", REQUEST_FAIL);
                requestNodes[1] = CreateRequestNode("Request #2", REQUEST_NOT_SENT);
                requestNodes[2] = CreateRequestNode("Request #3", REQUEST_OK);
                requestNodes[3] = CreateRequestNode("Request #4", REQUEST_PROCESS_START);
                nodes[i].Nodes.AddRange(requestNodes);
            }
            rootNode.Nodes.AddRange(nodes);


            ScenarioView.Nodes.Add(rootNode);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Creating server node
        /// </summary>
        /// <param name="name">Server node name</param>
        /// <param name="icon">Default server icon</param>
        /// <returns>Instance of tree node</returns>
        private TreeNode CreateServerNode(String name, int icon = SERVER_IMAGE)
        {
            TreeNode _serverNode = new TreeNode(name, icon, icon);
            _serverNode.Tag = "root:0";
            return _serverNode;
        }

        /// <summary>
        /// Create Scenario node
        /// </summary>
        /// <param name="name">Scenario name</param>
        /// <param name="id">Scenario ID</param>
        /// <returns></returns>
        private TreeNode CreateScenarioNode(String name, int id = 0)
        {
            TreeNode scenarioNode = new TreeNode(name);
            scenarioNode.ImageIndex = SCENARIO_IMAGE;
            scenarioNode.SelectedImageIndex = SCENARIO_IMAGE;
            scenarioNode.Tag = "scenario:" + id;
            return scenarioNode;
        }

        private TreeNode CreateRequestNode(String name, int icon, int requestId = 0)
        {
            TreeNode requestNode = new TreeNode(name);
            requestNode.ImageIndex = icon;
            requestNode.SelectedImageIndex = icon;
            requestNode.Tag = "request:" + requestId;

            if (icon == REQUEST_PROCESS_START)
                _processingNodes.Add(requestNode);

            return requestNode;
        }

        private void ScenarioView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);

                TreeNode node = ScenarioView.GetNodeAt(p);
                if (node != null)
                {
                    // Select cursor node
                    ScenarioView.SelectedNode = node;

                    // Type decision
                    if (node.Tag.GetType() == typeof(Server))
                    {
                        RootContextMenu.Show(ScenarioView, p);
                    }
                    else if (node.Tag.GetType() == typeof(Request))
                    {
                        RequestContextMenu.Show(ScenarioView, p);
                    }
                    else if (node.Tag.GetType() == typeof(Scenario))
                    {
                        ScenarioContextMenu.Show(ScenarioView, p);
                    }
                }
            }
        }

        private void exitMenu_Click(object sender, EventArgs e)
        {
            if (CloseWindowPrompt())
                System.Environment.Exit(0);
        }

        private bool CloseWindowPrompt()
        {
            var result = MessageBox.Show("Are you sure that you would like to quit application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return !(result == DialogResult.No);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CloseWindowPrompt())
            {
                e.Cancel = true;
            }
        }


        /// <summary>
        /// About window click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutProgram_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void processingIcons_Tick(object sender, EventArgs e)
        {
            foreach (TreeNode _it in _processingNodes)
            {
                // Set all images to actual
                _it.ImageIndex = _processingImageIndex;
                _it.SelectedImageIndex = _processingImageIndex;
            }

            // Continue with image index
            _processingImageIndex++;

            // Iteration
            if (_processingImageIndex > REQUEST_PROCESS_STOP)
                _processingImageIndex = REQUEST_PROCESS_START;
        }

        /// <summary>
        /// Menu click on export scenarios.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportScenarios_Click(object sender, EventArgs e)
        {
            new ExportSettings().ShowDialog();
        }

        private void OpenSavedScenario_Click(object sender, EventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.Filter = "API Director Files|*.adf";
            _ofd.Title = "Open saved API Director scenario";

            if (_ofd.ShowDialog() == DialogResult.OK)
            {
                // Load file from _ofd.FileName();
            }
        }

        /// <summary>
        /// Show propriet view after select node! (key arrows)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Get node
            TreeNode node = ScenarioView.SelectedNode;

            if (node.Tag.GetType() == typeof(Server))
            {
                _serverPanel.SetServer((Server)node.Tag);
                _setUserControl(_serverPanel);
            }
            else if (node.Tag.GetType() == typeof(Request))
            {
                _setUserControl(_requestPanel);
            }
            else if (node.Tag.GetType() == typeof(Scenario))
            {
                _setUserControl(_scenarioPanel);
            }
        }

        /// <summary>
        /// New scenario create!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewScenario_Click(object sender, EventArgs e)
        {
            // Create new default name server
            _rootServer = new Server("New server");

            // Refresh tree view
            _refreshTreeView();

            // Disable invalid menus
            _disableWithoutActiveScenarioMenus();
        }


        /// <summary>
        /// Refreshing scenario view with selected structures.
        /// </summary>
        private void _refreshTreeView()
        {
            if (_rootServer != null)
            {
                ProjectTreeGenerator.GenerateTree(_rootServer, ScenarioView);
            }
        }

        /// <summary>
        /// Disabling and enabling menu which are useless when new project is open!
        /// </summary>
        private void _disableWithoutActiveScenarioMenus()
        {
            NewScenario.Enabled = false;
            SaveScenario.Enabled = true;
            TestsMainMenu.Enabled = true;
        }

        private void AddScenarioRootMenu_Click(object sender, EventArgs e)
        {
            // Create new scenario
            _rootServer.CreateNewScenario();

            // Refresh view
            _refreshTreeView();
        }


        /// <summary>
        /// Add request to scenario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get selected node!
            TreeNode _selectedNode = ScenarioView.SelectedNode;

            if (_selectedNode != null && _selectedNode.Tag.GetType() == typeof(Scenario))
            {
                // Add node to scenario
                Scenario _selectedScenario = (Scenario)_selectedNode.Tag;

                // Create request in scenario
                _selectedScenario.CreateNewRequest();

                // Refresh view
                _refreshTreeView();
            }
        }

        /// <summary>
        /// Removing scenario completely.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeScenarioMenu_Click(object sender, EventArgs e)
        {
            // Get selected node!
            TreeNode _selectedNode = ScenarioView.SelectedNode;

            // Selected node exist and it is scenario = msg box
            if (_selectedNode != null && _selectedNode.Tag.GetType() == typeof(Scenario))
            {
                // Retype
                Scenario _selectedScenario = (Scenario) _selectedNode.Tag;

                // Dialog
                DialogResult _dialogResult = MessageBox.Show("Are you sure, you want to destroy scenario: " + _selectedScenario.Name + "?", "Removing scenario", MessageBoxButtons.YesNo);

                // If yes destroy node and refresh view! and refresh view
                if (_dialogResult == DialogResult.Yes) 
                { 
                    // Remove from server list!
                    _rootServer.Scenarios.Remove(_selectedScenario);

                    // Set selected node to first
                    ScenarioView.SelectedNode = ScenarioView.Nodes[0];

                    // Refresh
                    _refreshTreeView();
                }
            }
        }
    }
}
