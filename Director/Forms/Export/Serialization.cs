using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Director.Forms.Export
{
    class Serializator
    {
        public void SerializeServer(Server server, string fileToSave)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Server));
            using (TextWriter writer = new StreamWriter(fileToSave))
            {
                foreach (Scenario sc in server.Scenarios)
                {
                    foreach (Request req in sc.Requests)
                    {
                        if (req.customVariables != null)
                            req.customVariablesExp = req.customVariables.Select(kv => new CustomVariableItem() { id = kv.Key, value = kv.Value }).ToArray();
                    }
                }
                serializer.Serialize(writer, server);
            }
        }
    }
}
