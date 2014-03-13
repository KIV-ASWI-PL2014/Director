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

namespace Director.Forms
{
    public partial class MainWindow : Form
    {

        // Prepare Info Pannels!
        private EmptyPanel _emptyPanel = new EmptyPanel();
        private ServerPanel _serverPanel = new ServerPanel();
        private RequestPanel _requestPanel = new RequestPanel();
        private ScenarioPanel _scenarioPanel = new ScenarioPanel();

        public MainWindow()
        {
            InitializeComponent();

            // Create temporary tree view
            CreateTreeView();

            // New empty panel!
            ContentPanel.Controls.Add(_emptyPanel);
        }

        private void CreateTreeView() {
            ImageList myImageList = new ImageList();
            myImageList.Images.Add(Director.Properties.Resources.ServerRoot);
            myImageList.Images.Add(Director.Properties.Resources.lightning);
            myImageList.Images.Add(Director.Properties.Resources.bullet_blue);
            myImageList.Images.Add(Director.Properties.Resources.fail);
            myImageList.Images.Add(Director.Properties.Resources.ok);
            myImageList.Images.Add(Director.Properties.Resources.Processing);

            // Set image list to view
            ScenarioView.ImageList = myImageList;
            ScenarioView.ImageIndex = 0;
            ScenarioView.SelectedImageIndex = 0;


            // Fill a bit
            TreeNode rootNode = new TreeNode("Server #1");
            rootNode.Tag = "root:0";
            rootNode.ImageIndex = 0;
            rootNode.SelectedImageIndex = 0;

            // Create several Scenario nodes
            TreeNode[] nodes = new TreeNode[4];
            for(int i = 0; i < 4; i++) {
                nodes[i] = CreateScenarioNode("Scenario #"+i.ToString());

                TreeNode [] requestNodes = new TreeNode[4];

                requestNodes[0] = CreateRequestNode("Request #1", 2);
                requestNodes[1] = CreateRequestNode("Request #2", 4);
                requestNodes[2] = CreateRequestNode("Request #3", 3);
                requestNodes[3] = CreateRequestNode("Request #4", 5);
                nodes[i].Nodes.AddRange(requestNodes);
            }
            rootNode.Nodes.AddRange(nodes);


            ScenarioView.Nodes.Add(rootNode);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }


        private TreeNode CreateScenarioNode(String name, int id = 0) {
            TreeNode scenarioNode = new TreeNode(name);
            scenarioNode.ImageIndex = 1;
            scenarioNode.SelectedImageIndex = 1;
            scenarioNode.Tag = "scenario:" + id;
            return scenarioNode;
        }

        private TreeNode CreateRequestNode(String name, int icon, int requestId = 0) {
            TreeNode requestNode = new TreeNode(name);
            requestNode.ImageIndex = icon;
            requestNode.SelectedImageIndex = icon;
            requestNode.Tag = "request:" + requestId;
            return requestNode;            
        }

        private void ScenarioView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) {
                Point p = new Point(e.X, e.Y);

                TreeNode node = ScenarioView.GetNodeAt(p);
                if (node != null) {
                    // Select cursor node
                    ScenarioView.SelectedNode = node;

                    // Get Tag
                    String [] tag_s = (Convert.ToString(node.Tag)).Split(":"[0]);

                    if (e.Button == MouseButtons.Left)
                    {
                        switch (tag_s[0]) { 
                            case "scenario":
                                ContentPanel.Controls.Clear();
                                ContentPanel.Controls.Add(_scenarioPanel);
                                break;

                            case "request":
                                ContentPanel.Controls.Clear();
                                ContentPanel.Controls.Add(_requestPanel);
                                break;

                            case "root":
                                ContentPanel.Controls.Clear();
                                ContentPanel.Controls.Add(_serverPanel);
                                break;
                        }
                    }
                    else
                    {
                        switch (tag_s[0])
                        {
                            case "scenario":
                                ScenarioContextMenu.Show(ScenarioView, p);
                                break;
                            case "request":
                                RequestContextMenu.Show(ScenarioView, p);
                                break;
                            case "root":
                                RootContextMenu.Show(ScenarioView, p);
                                break;
                        }
                    }
                }
            }
        }

        private void exitMenu_Click(object sender, EventArgs e)
        {
            if (CloseWindowPrompt())
                System.Environment.Exit(0);
        }

        private bool CloseWindowPrompt() {
            var result = MessageBox.Show("Are you sure that you would like to quit application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return !(result == DialogResult.No);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CloseWindowPrompt()) {
                e.Cancel = true;
            }
        }

        private void MainWindow_ResizeEnd(object sender, EventArgs e)
        {
            // Resize end, Resize all elements
        }

        private void AboutProgram_Click(object sender, EventArgs e)
        {
            About aboutWindow = new About();
            aboutWindow.Show();
        }
    }
}
