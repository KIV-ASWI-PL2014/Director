using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.ExportLib
{
    class Export
    {
        public static string serverOverview = "server.xml";
        public static string scenarioOverview = "scenario.xml";
        public static string exportDirectory = "a_director_export";


        public static DirectoryInfo createTempDirectory()
        {
            DirectoryInfo tmpDirectory;
            try
            {
                tmpDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Export.exportDirectory));
                foreach (FileInfo file in tmpDirectory.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return tmpDirectory;
        }
    }
}