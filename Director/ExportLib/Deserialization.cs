using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Director.DataStructures.SupportStructures;

namespace Director.ExportLib
{
    class Deserialization
    {
        public static DirectoryInfo tmpDirectory;
        public static String errorMessage { get; set; }

        public static Server DeserializeAll(String adfeFile)
        {
            errorMessage = "";
            tmpDirectory = Export.createTempDirectory(false);
            if (tmpDirectory == null)
                return null;

            try
            {
                ZipUtil.ExtractZipFile(adfeFile, tmpDirectory.FullName);
            }
            catch (Exception e)
            {
                errorMessage = Director.Properties.Resources.ExportZipDecompressionEx + e.Message;
                return null;
            }

            // Deserialize of server
            Server server = DeserializeServer();
            if (server == null)
                return null;

            // Deserialize of scenarios
            List<Scenario> scenarios = DeserializeScenarios();
            if (scenarios == null)
                return null;

            server.Scenarios = scenarios;
            return server;
        }


        public static List<Scenario> DeserializeScenarios()
        {
            List<Scenario> scenarios;
            try
            {
                StreamReader sr = new StreamReader(Path.Combine(tmpDirectory.FullName, Export.scenarioOverview));
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Scenario>));
                scenarios = (List<Scenario>)xmlSerializer.Deserialize(sr);
                // Close the stream reader
                sr.Close();
            }
            catch (Exception e)
            {
                errorMessage = Director.Properties.Resources.ExportDeserialScenariosEx + e.Message;
                return null;
            }

            foreach (Scenario sc in scenarios)
            {
                if (sc.customVariablesExp != null)
                {
                    Dictionary<string, string> custVarDes = new Dictionary<string, string>();
                    foreach (CustomVariableItem cvi in sc.customVariablesExp)
                        custVarDes.Add(cvi.id, cvi.value);
                    sc.customVariables = custVarDes;
                }
                ProcessFiles(sc);
            }

            return scenarios;
        }


        public static void ProcessFiles(Scenario sc)
        {
            String resourceDir = Path.Combine(tmpDirectory.FullName, Export.resourceDirectory);
            foreach (Request req in sc.Requests)
            {
                foreach (FileItem f in req.Files)
                {
                    //for now use the old name
                    //var groups = Regex.Match(f.FilePath, @"(\d+)_(\d+)_(\d+)_(.*)").Groups;
                    //String newFileName = groups[4].Value;    
                    String newFileName = f.FilePath;    
                    f.FilePath = Path.Combine(resourceDir, newFileName);
                }
            }
        }


        public static Server DeserializeServer()
        {
            Server server;
            try
            {
                StreamReader sr = new StreamReader(Path.Combine(tmpDirectory.FullName, Export.serverOverview));
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Server));
                server = (Server)xmlSerializer.Deserialize(sr);

                // Close the stream reader
                sr.Close();
            }
            catch (Exception e)
            {
                errorMessage = Director.Properties.Resources.ExportDeserialServerEx + e.Message;
                return null;
            }
            return server;
        }
    }
}
