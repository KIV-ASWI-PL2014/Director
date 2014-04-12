using Director.Forms;
using System;
using Xwt;
using RestSharp;

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
