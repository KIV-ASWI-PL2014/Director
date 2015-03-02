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
			Config.SetAppType (ToolkitType.Cocoa); 
          	Application.Initialize(Config.GetAppType());

			// Main window
			MainWindow _mainWindow = new MainWindow();
            _mainWindow.Closed += delegate { Application.Exit(); };

			// show
            _mainWindow.Show();
            Application.Run();
        }
    }
}
