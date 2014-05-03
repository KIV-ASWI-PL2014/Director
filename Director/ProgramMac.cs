using System;
using Xwt;
using Director.DataStructures;
using Director.Forms.Inputs;
using Director.Forms;

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

			Server s = new Server() { Name = "OCR" };
			s.DefaultHeaders.Add(new Header() { Name = "content/type", Value = "application/json" });
			s.SetUrl("http://localhost:3000/api/");
			Scenario sc = s.CreateNewScenario();
			sc.Name = "test";
			sc.ParentServer = s;
			Request test = sc.CreateNewRequest();
			test.ParentScenario = sc;


			//EditWindow _mainWindow = new EditWindow (null, test);

			// Close window handlers

            _mainWindow.Closed += delegate { Application.Exit(); };

            _mainWindow.Show();
            Application.Run();
        }
    }
}
