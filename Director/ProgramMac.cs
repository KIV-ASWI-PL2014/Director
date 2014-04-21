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
    static class ProgramMac
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			// Set toolkit type
			Config.SetAppType (ToolkitType.Cocoa); 

			Application.Initialize(Config.GetAppType());

			// Todo: Remove
			Server s = new Server () { Name = "OCR" };
			s.DefaultHeaders.Add (new Header () { Name = "content/type", Value = "application/json" });
			s.SetUrl ("http://localhost:3000/api/");
			Scenario sc = s.CreateNewScenario ();
			sc.Name = "test";
			sc.ParentServer = s;
			Request test = sc.CreateNewRequest ();
			test.ParentScenario = sc;

			//EditWindow _mainWindow = new EditWindow (null, test);

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
