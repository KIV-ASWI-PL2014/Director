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
			Application.Initialize(ToolkitType.Wpf);
            MainWindow _mainWindow = new MainWindow();
            _mainWindow.Show();
            _mainWindow.Disposed += delegate
            {
                Application.Exit();
            };
            _mainWindow.Closed += delegate
            {
                Application.Exit();
            };
            Application.Run();
        }
    }
}
