using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms;

namespace Director
{
    static class ProgramMac
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Initialize(ToolkitType.Cocoa);
            MainWindow _mainWindow = new MainWindow();

			// Close window handlers
			_mainWindow.Closed += delegate {
				Application.Exit();
			};

            _mainWindow.Show();
            Application.Run();
        }
    }
}
