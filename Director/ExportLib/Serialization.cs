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
    class Serialization
    {
        public static DirectoryInfo tmpDirectory;


        public static Boolean SerializeAll(Server server, string fileToSave, List<Scenario> ScenarioList)
        {
            tmpDirectory = Export.createTempDirectory();
            if (tmpDirectory == null)
                return false;

            //serialization of server
            SerializeServer(server);

            //serialization of scenarios
            SerializeScenarios(ScenarioList);

            //archive created files
            ZipFile.CreateFromDirectory(Path.Combine(Path.GetTempPath(), Export.exportDirectory), fileToSave);
            return true;
        }


        public static void SerializeScenarios(List<Scenario> scenarios)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Scenario>));

            foreach (Scenario sc in scenarios)
            {
                if (sc.customVariables != null)
                    sc.customVariablesExp = sc.customVariables.Select(kv => new CustomVariableItem() { id = kv.Key, value = kv.Value }).ToArray();
            }

            using (TextWriter writer = new StreamWriter(Path.Combine(tmpDirectory.FullName, Export.scenarioOverview)))
            {
                //serialization of scenarios
                serializer.Serialize(writer, scenarios);
            }
        }


        public static void SerializeServer(Server server)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Server));
            using (TextWriter writer = new StreamWriter(Path.Combine(tmpDirectory.FullName, Export.serverOverview)))
            {
                //serialization of server
                serializer.Serialize(writer, server);
            }
        }

    }
}
