using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms;
using Director.DataStructures;
using Director.Forms.Inputs;

namespace Director
{
    internal static class ProgramMac
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
			// Set toolkit type
			Config.SetAppType (ToolkitType.Gtk); 
          Application.Initialize(Config.GetAppType());


            MainWindow _mainWindow = new MainWindow();

            // Close window handlers
            _mainWindow.Closed += delegate { Application.Exit(); };

            _mainWindow.Show();
            Application.Run();
        }
    }
}
