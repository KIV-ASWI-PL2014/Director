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
