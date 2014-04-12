using Director.Forms;
using RestSharp;
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
            //var client = new RestClient("http://test.cz");
            //var request = new RestRequest("test/", Method.POST);
            //RestResponse response = (RestResponse) client.Execute(request);
            //Console.WriteLine(response.Content);

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
