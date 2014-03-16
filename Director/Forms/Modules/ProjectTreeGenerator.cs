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
        public static void GenerateTree(Server newServer, TreeView scenarioTreeView)
        { 
            // If scenario tree view is empty - just initilize!
            if (scenarioTreeView.Nodes.Count == 0)
            {
                scenarioTreeView.Nodes.Add(_generateCleanStructure(newServer));
            }
            else
            {
                // Iterate and add or destroy

            }
        }

        private static TreeNode _generateCleanStructure(Server newServer)
        {
            // Create root node
            TreeNode rootNode = GenerateRootNode(newServer.Name);

            // Create all scenarios


            // Return root node
            return rootNode;

        }

        private static TreeNode GenerateScenarioNode(String name)
        {
            return null;
        }

        /// <summary>
        /// Create root node.
        /// </summary>
        /// <param name="name">Root node name</param>
        /// <returns></returns>
        private static TreeNode GenerateRootNode(String name)
        {
            TreeNode node = new TreeNode(name, MainWindow.SERVER_IMAGE, MainWindow.SERVER_IMAGE);
            node.Tag = "root:0";
            return node;
        }
    }
}
