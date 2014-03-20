using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Director.Forms.Modules
{
    /// <summary>
    /// Static class for generating whole project tree from given server.
    /// This class is only for GUI usage!
    /// </summary>
    class ProjectTreeGenerator
    {
        /// <summary>
        /// Regenerating tree view tree!
        /// </summary>
        /// <param name="newServer">Server</param>
        /// <param name="scenarioTreeView">Scenario tree view</param>
        public static void GenerateTree(Server newServer, TreeView scenarioTreeView)
        {
            _compareNodeStructure(newServer, scenarioTreeView);
        }

        /// <summary>
        /// Start comparing node structures, renaming etc..
        /// </summary>
        /// <param name="server"></param>
        /// <param name="scenarioTreeView"></param>
        private static void _compareNodeStructure(Server server, TreeView scenarioTreeView)
        {
            TreeNode rootNode = null;

            // First compare scenario root with server
            if (scenarioTreeView.Nodes.Count == 0)
            {
                // Create server
                rootNode = GenerateRootNode(server);

                // Add root node to nodes
                scenarioTreeView.Nodes.Add(rootNode);
            }
            else
            {
                // Get root node
                rootNode = scenarioTreeView.Nodes[0];

                // Compare server and node data
                rootNode.Text = server.Name;
                rootNode.Tag = server;
            }

            // Compare 
            _compareScenariosStructure(server, rootNode);
        }

        /// <summary>
        /// Comparing scenarios in root node.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="rootNode"></param>
        private static void _compareScenariosStructure(Server server, TreeNode rootNode)
        {
            // Get all scenario id!
            List<int> _scenarioIds = server.GetScenarioIds();

            // For iteration - can delete non-existing nodes!
            for (int i = 0; i < rootNode.Nodes.Count; i++)
            {
                TreeNode _node = rootNode.Nodes[i];
                Scenario _scenario = null;

                // Retype
                if (rootNode.Nodes[i].Tag.GetType() == typeof(Scenario))
                    _scenario = (Scenario)_node.Tag;

                // If scenario is NULL or not exist in _scenarioIds it was destroyed!
                if (_scenario == null || !_scenarioIds.Contains(_scenario.Id))
                {
                    // Deleted or it is not scenario!
                    rootNode.Nodes.RemoveAt(i);
                }
                else
                {
                    // Update position and name
                    _scenario.Position = i;
                    _node.Text = _scenario.Name;

                    // Remove from ID
                    _scenarioIds.Remove(_scenario.Id);

                    // Update into scenario
                    _compareRequestStructure(_node, _scenario);
                }
            }

            foreach (int id in _scenarioIds)
            {
                // Get scenario and create tree node!
                Scenario _scenario = server.Scenarios.Find(x => x.Id == id);
                if (_scenario != null)
                {
                    // Create node
                    TreeNode _node = GenerateScenarioNode(_scenario);

                    // Add node to list
                    rootNode.Nodes.Add(_node);

                    // Compare request structure
                    _compareRequestStructure(_node, _scenario);
                }
            }

        }

        /// <summary>
        /// Compare scenario´s requests.
        /// </summary>
        /// <param name="node">Scenario node</param>
        /// <param name="scenario">Scenario instnac</param>
        private static void _compareRequestStructure(TreeNode scenarioNode, Scenario scenario)
        {
            // Get request ID list
            List<int> _requestIds = scenario.GetRequestIds();

            // For iteration - can delete non-existing nodes!
            for (int i = 0; i < scenarioNode.Nodes.Count; i++)
            {
                TreeNode _node = scenarioNode.Nodes[i];
                Request _request = null;

                // Retype
                if (scenarioNode.Nodes[i].Tag.GetType() == typeof(Request))
                    _request = (Request)_node.Tag;

                // If scenario is NULL or not exist in _scenarioIds it was destroyed!
                if (_request == null || !_requestIds.Contains(_request.Id))
                {
                    // Deleted or it is not scenario!
                    scenarioNode.Nodes.RemoveAt(i);
                }
                else
                {
                    // Update position and name
                    _request.Position = i;
                    _node.Text = _request.Name;

                    // Remove from ID
                    _requestIds.Remove(_request.Id);
                }
            }

            // Add rest of scenarios ID (new scenarios)
            foreach (int id in _requestIds)
            {
                // Get scenario and create tree node!
                Request _request = scenario.Requests.Find(x => x.Id == id);
                if (_request != null)
                {
                    // Create node
                    TreeNode _node = GenerateRequestNode(_request);

                    // Add node to list
                    scenarioNode.Nodes.Add(_node);
                }
            }

        }

        /// <summary>
        /// Create scenario node.
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        private static TreeNode GenerateRequestNode(Request request)
        {
            TreeNode node = new TreeNode(request.Name, MainWindow.REQUEST_NOT_SENT, MainWindow.REQUEST_NOT_SENT);
            node.Tag = request;
            return node;
        }

        /// <summary>
        /// Create scenario node.
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        private static TreeNode GenerateScenarioNode(Scenario scenario)
        {
            TreeNode node = new TreeNode(scenario.Name, MainWindow.SCENARIO_IMAGE, MainWindow.SCENARIO_IMAGE);
            node.Tag = scenario;
            return node;
        }

        /// <summary>
        /// Create root node.
        /// </summary>
        /// <param name="name">Root node name</param>
        /// <returns></returns>
        private static TreeNode GenerateRootNode(Server server)
        {
            TreeNode node = new TreeNode(server.Name, MainWindow.SERVER_IMAGE, MainWindow.SERVER_IMAGE);
            node.Tag = server;
            return node;
        }
    }
}
