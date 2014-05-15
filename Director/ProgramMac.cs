using System;
using Xwt;
using Director.DataStructures;
using Director.Forms.Inputs;
using Director.Forms;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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

			// Locales
			Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Director.Properties.Settings.Default.language);

			// Set toolkit type
			Config.SetAppType (ToolkitType.Gtk); 
          	Application.Initialize(Config.GetAppType());

			//MainWindow _mainWindow = new MainWindow();

			Server s = new Server() { Name = "OCR" };
			s.DefaultHeaders.Add(new Header() { Name = "content/type", Value = "application/json" });
			s.SetUrl("http://localhost:3000/api/");
			Scenario sc = s.CreateNewScenario();
			sc.Name = "test";
			sc.ParentServer = s;
			Request test = sc.CreateNewRequest();
			test.ParentScenario = sc;
			test.ResponseTemplateType = ContentType.JSON;
			test.RequestTemplate = @"{
  'Name': 'Bad Boys',
  'ReleaseDate': '$first$ thing and #randString(1,$x$,A1)# for sure', 'franta': [ 2,3,4,5 ]
}"; ;
			test.RequestTemplateType = ContentType.JSON;
			test.ResponseTemplate = @"{
  'Name': 'Bad Boys',
  'ReleaseDate': '#string#eq#franta#variable#', 'test' : [ 1, 2, 3, 4 ]
}"; ;

			EditWindow _mainWindow = new EditWindow (null, test);

			// Close window handlers

            _mainWindow.Closed += delegate { Application.Exit(); };

            _mainWindow.Show();
            Application.Run();
        }
    }
}
