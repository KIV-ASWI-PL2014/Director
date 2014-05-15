using Director.Forms;
using System;
using Xwt;
using Director.DataStructures;
using System.Threading;
using Director.Forms.Inputs;
using Director.ParserLib;
using System.Collections.Generic;

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
            // Set language
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Director.Properties.Settings.Default.language);

            // Setapp type
            Config.SetAppType(ToolkitType.Wpf);

            // Initialize
            Application.Initialize(Config.GetAppType());

            Server s = new Server() { Name = "OCR" };
            s.DefaultHeaders.Add(new Header() { Name = "content/type", Value = "application/json" });
            s.DefaultHeaders.Add(new Header() { Name = "Token", Value = "application/json" });
            s.SetUrl("http://localhost:3000/api/");
            Scenario sc = s.CreateNewScenario();
            sc.customVariables.Add("x", "franta");
            sc.customVariables.Add("first", "Firstik");
            sc.Name = "test";
            sc.ParentServer = s;
            Request test = sc.CreateNewRequest();

            test.RequestTemplate = @"{
  'Name': 'Bad Boys',
  'ReleaseDate': '$first$ thing and #randString(1,$x$,A1)# for sure'
}"; ;

            sc.customVariables.Add("test", "hovadina");
            test.ParentScenario = sc;

            //EditWindow _mainWindow = new EditWindow(null, test);

            // Create main window and show
            MainWindow _mainWindow = new MainWindow();
            _mainWindow.Show();
            _mainWindow.Closed += delegate {
                Application.Exit();
            };

            // Run application
            Application.Run();
        }
    }
}
