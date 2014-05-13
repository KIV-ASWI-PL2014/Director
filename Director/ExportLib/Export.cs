using System;
using System.IO;

namespace Director.ExportLib
{
    class Export
    {
        public const string serverOverview = "server.xml";
        public const string scenarioOverview = "scenario.xml";
        public const string exportDirectory = "director_export";
        public const string importDirectory = "director_import";
        public const string resourceDirectory = "resource";

        public static string errorMessage = "";

        public static DirectoryInfo createTempDirectory(Boolean export)
        {
            errorMessage = "";
            DirectoryInfo tmpDirectory;
            String tmpDir;
            try
            {
                if(export)
                    tmpDir = Export.exportDirectory + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                else
                    tmpDir = Export.importDirectory + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                tmpDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), tmpDir));
                if(tmpDirectory.Exists)
                    tmpDirectory.Delete(true);
                tmpDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), tmpDir));
                Directory.CreateDirectory(Path.Combine(tmpDirectory.FullName, resourceDirectory));
            }
            catch (Exception e)
            {
                errorMessage += Director.Properties.Resources.ExportProcessTempDirEx + e.Message;
                return null;
            }
            return tmpDirectory;
        }
    }
}