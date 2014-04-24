using Director.Forms;
using System;
using Xwt;
using RestSharp;
using Director.DataStructures;
using Director.Forms.Inputs;

namespace Director
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Setapp type
            Config.SetAppType(ToolkitType.Wpf);

            // Initialize
            Application.Initialize(Config.GetAppType());

            // Create main window and show
            //MainWindow _mainWindow = new MainWindow();

            Server s = new Server() { Name = "OCR" };
            s.DefaultHeaders.Add(new Header() { Name = "content/type", Value = "application/json" });
            s.SetUrl("http://localhost:3000/api/");
            Scenario sc = s.CreateNewScenario();
            sc.Name = "test";
            sc.ParentServer = s;
            Request test = sc.CreateNewRequest();
            test.ParentScenario = sc;


            EditWindow _mainWindow = new EditWindow (null, test);

            _mainWindow.Show();
            _mainWindow.Closed += delegate {
                Application.Exit();
            };

            // Run application
            Application.Run();
        }
    }
}