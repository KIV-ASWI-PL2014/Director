using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Director.DataStructures.SupportStructures;

namespace Director.ExportLib
{
    class Serialization
    {
        public static DirectoryInfo tmpDirectory;
        public static String errorMessage {get; set;}

        public static Boolean SerializeAll(Server server, string fileToSave, List<Scenario> ScenarioList)
        {
            errorMessage = "";
            tmpDirectory = Export.createTempDirectory(true);
            if (tmpDirectory == null)
                return false;

            //serialization of server
            if (!SerializeServer(server))
                return false;

            //serialization of scenarios
            if (!SerializeScenarios(ScenarioList))
                return false;

            try
            {
                //delete original zip if exists
                if (File.Exists(fileToSave))
                    File.Delete(fileToSave);
            }
            catch (Exception e)
            {
                errorMessage = "Exception during serialization of scenarios: " + e.Message;
                return false;
            }

            try
            {
                //archive created files
                ZipUtil.CreateArchiveFromFolder(fileToSave, tmpDirectory.FullName);
            }
            catch(Exception e)
            {
                errorMessage = "Exception during a zip compression : " + e.Message;
                return false;
            }
            return true;
        }


        public static Boolean SerializeScenarios(List<Scenario> scenarios)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Scenario>));

            foreach (Scenario sc in scenarios)
            {
                if (sc.customVariables != null)
                    sc.customVariablesExp = sc.customVariables.Select(kv => new CustomVariableItem() { id = kv.Key, value = kv.Value }).ToArray();

                if (!ProcessFiles(sc))
                    return false;
            }
            try
            {
                using (TextWriter writer = new StreamWriter(Path.Combine(tmpDirectory.FullName, Export.scenarioOverview)))
                {
                    //serialization of scenarios
                    serializer.Serialize(writer, scenarios);
                }
            }
            catch (Exception e)
            {
                errorMessage = "Exception during serialization of scenarios: " + e.Message;
                return false;
            }
            return true;
        }

        public static Boolean ProcessFiles(Scenario sc)
        {
            int counter;
            String prefix;
            foreach (Request req in sc.Requests)
            {
                counter = 1;
                prefix = string.Format("{0}_{1}", sc.Id, req.Id);
                foreach (FileItem f in req.Files)
                {
                    try
                    {
                        if (f.FilePath == null) 
                        {
                            errorMessage += string.Format("File {0} in request {1} in scenario {2} has been skipped.\n", f.FileName,
                                req.Name, sc.Name);
                            continue;
                        }
                        String newFileName = string.Format("{0}_{1}_{2}", prefix, counter, Export.getFileNameFromAbsolutePath(f.FilePath));
                        String resourceDir = Path.Combine(tmpDirectory.FullName, Export.resourceDirectory);
                        File.Copy(f.FilePath, Path.Combine(resourceDir, newFileName), true);
                        f.FilePath = newFileName;
                        counter++;
                    }
                    catch (Exception e)
                    {
                        errorMessage = "Exception during processing files: " + e.Message;
                        return false;
                    }
                }
                // delete all invalid files
                req.Files.RemoveAll(item => item.FilePath == null);
            }
            return true;
        }


        public static Boolean SerializeServer(Server server)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Server));
                using (TextWriter writer = new StreamWriter(Path.Combine(tmpDirectory.FullName, Export.serverOverview)))
                {
                    //serialization of server
                    serializer.Serialize(writer, server);
                }
            }
            catch (Exception e)
            {
                errorMessage = "Exception during serialization of server: " + e.Message;
                return false;
            }
            return true;
        }

    }
}
