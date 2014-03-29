using Director.Forms;
using System;
using Xwt;

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
			Application.Initialize(ToolkitType.Cocoa);

            MainWindow _mainWindow = new MainWindow();
            _mainWindow.Show();
            Application.Run();
			_mainWindow.Dispose();
			Application.Dispose();
        }
    }
}
