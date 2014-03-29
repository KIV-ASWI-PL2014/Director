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
            var mainWindow = new Window()
            {
                Title = "Xwt Demo Application",
                Width = 500,
                Height = 400
            };
            mainWindow.Show();
            Application.Run();
            mainWindow.Dispose();
        }
    }
}
