using Director.Forms;
using System;
using Xwt;
using RestSharp;
using Director.DataStructures;
using Director.Forms.Inputs;

namespace Director
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			// Setapp type
			Config.SetAppType(ToolkitType.Wpf); 

			// Initialize
			Application.Initialize(Config.GetAppType());

			Application.Initialize(ToolkitType.Wpf);

            // Todo: Remove
            Server s = new Server() { Name = "OCR" };
            s.DefaultHeaders.Add(new Header() { Name = "content/type", Value = "application/json" });
            s.SetUrl("http://localhost:3000/api/");
            Scenario sc = s.CreateNewScenario();
            sc.Name = "test";
            sc.ParentServer = s;
            Request test = sc.CreateNewRequest();
            test.ParentScenario = sc;


            //EditWindow _mainWindow = new EditWindow(null, test);

            //
            MainWindow _mainWindow = new MainWindow();
            _mainWindow.Show();
            _mainWindow.Closed += delegate
            {
                Application.Exit();
            };
            Application.Run();
        }
    }
}
