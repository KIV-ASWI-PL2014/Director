using Director.Forms.InfoPanels;
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
        private ServerPanel _serverPanel;
        private RequestPanel _requestPanel = new RequestPanel();
        private ScenarioPanel _scenarioPanel;

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
            _initiateTreeView();

            // New empty panel!
            _setUserControl(_emptyPanel);

            // TODO: DEBUG
            NewScenario_Click(null, null);

            // Create all panels with current instance of main window
            _serverPanel = new ServerPanel(this);
            _scenarioPanel = new ScenarioPanel(this);
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

        /// <summary>
        /// Initiate all tree view images! - server, processing, error etc..
        /// </summary>
        private void _initiateTreeView()
        {
            // Set handlers
            ScenarioView.ItemDrag += new ItemDragEventHandler(TreeView_ItemDrag);
            ScenarioView.DragEnter += new DragEventHandler(TreeView_DragEnter);
            ScenarioView.DragOver += new DragEventHandler(TreeView_DragOver);
            ScenarioView.DragDrop += new DragEventHandler(TreeView_DragDrop);

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

        /// <summary>
        /// When is main window closed, close whole Application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
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
                _scenarioPanel.SetScenario((Scenario)node.Tag);
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
            _rootServer = new Server("New server", "http://example.com/api/");

            // Refresh tree view
            RefreshTreeView();

            // Disable invalid menus
            _disableWithoutActiveScenarioMenus();
        }


        /// <summary>
        /// Refreshing scenario view with selected structures.
        /// </summary>
        public void RefreshTreeView()
        {
            if (_rootServer != null)
            {
                ProjectTreeGenerator.GenerateTree(_rootServer, ScenarioView, this);
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

        /// <summary>
        /// Add scenario from root menu!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddScenarioRootMenu_Click(object sender, EventArgs e)
        {
            // Create new scenario
            _rootServer.CreateNewScenario();

            // Refresh view
            RefreshTreeView();
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
                RefreshTreeView();
            }
        }

        public void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        public void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        public void TreeView_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            Point targetPoint = ScenarioView.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = ScenarioView.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            // Confirm that the node at the drop location is not  
            // the dragged node or a descendant of the dragged node. 
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                // If it is a move operation, remove the node from its current  
                // location and add it to the node at the drop location. 
                if (e.Effect == DragDropEffects.Move)
                {
                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);
                }

                // If it is a copy operation, clone the dragged node  
                // and add it to the node at the drop location. 
                else if (e.Effect == DragDropEffects.Copy)
                {
                    targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                }

                // Expand the node at the location  
                // to show the dropped node.
                targetNode.Expand();
            }
        }

        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node. 
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node,  
            // call the ContainsNode method recursively using the parent of  
            // the second node. 
            return ContainsNode(node1, node2.Parent);
        }

        public void TreeView_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = ScenarioView.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.
            ScenarioView.SelectedNode = ScenarioView.GetNodeAt(targetPoint);      
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
                    RefreshTreeView();
                }
            }
        }
    }
}
