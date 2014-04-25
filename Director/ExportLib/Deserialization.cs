using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO.Compression;

namespace Director.ExportLib
{
    class Deserialization
    {
        public static DirectoryInfo tmpDirectory;


        public static Server DeserializeAll(String adfeFile)
        {
            tmpDirectory = Export.createTempDirectory();
            if (tmpDirectory == null)
                return null;

            ZipFile.ExtractToDirectory(adfeFile, tmpDirectory.FullName);

            // Deserialize of server
            Server server = DeserializeServer();

            // Deserialize of scenarios
            List<Scenario> scenarios = DeserializeScenarios();
  
            server.Scenarios = scenarios;
            return server;
        }


        public static List<Scenario> DeserializeScenarios()
        {
            StreamReader sr = new StreamReader(Path.Combine(tmpDirectory.FullName, Export.scenarioOverview));
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Scenario>));
            List<Scenario> scenarios = (List<Scenario>)xmlSerializer.Deserialize(sr);

            foreach (Scenario sc in scenarios)
            {
                if (sc.customVariablesExp != null)
                {
                    Dictionary<string, string> custVarDes = new Dictionary<string, string>();
                    foreach (CustomVariableItem cvi in sc.customVariablesExp)
                        custVarDes.Add(cvi.id, cvi.value);
                    sc.customVariables = custVarDes;
                }
            }

            // Close the stream reader
            sr.Close();
            return scenarios;
        }


        public static Server DeserializeServer()
        {
            StreamReader sr = new StreamReader(Path.Combine(tmpDirectory.FullName, Export.serverOverview));
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Server));
            Server server = (Server)xmlSerializer.Deserialize(sr);

            // Close the stream reader
            sr.Close();
            return server;
        }
    }
}
