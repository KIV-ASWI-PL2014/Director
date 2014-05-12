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
        

        public static DirectoryInfo createTempDirectory(Boolean export)
        {
            DirectoryInfo tmpDirectory;
            String tmpDir;
            try
            {
                if(export)
                    tmpDir = Export.exportDirectory;
                else
                    tmpDir = Export.importDirectory;

                tmpDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), tmpDir));
                if(tmpDirectory.Exists)
                    tmpDirectory.Delete(true);
                tmpDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), tmpDir));
                Directory.CreateDirectory(Path.Combine(tmpDirectory.FullName, resourceDirectory));
            }
            catch (Exception e)
            {
                Console.WriteLine(Director.Properties.Resources.ExportProcessTempDirEx + e.Message);
                return null;
            }
            return tmpDirectory;
        }


        public static String getFileNameFromAbsolutePath(String absPath)
        {
            int pos = absPath.LastIndexOf('\\');
            //backslash not found
            if (pos == -1)
                return absPath;
            //backslash at the end of string
            else if(pos == absPath.Length - 1)
                return "";
            return absPath.Substring(pos + 1);
        }
    }
}