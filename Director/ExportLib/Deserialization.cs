using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Director.ExportLib
{
    class Deserialization
    {
        public static Server DeserializeServer(String path)
        {
            StreamReader sr = new StreamReader(path);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Server));
            // Deserialize from the StreamReader.
            Server server = (Server)xmlSerializer.Deserialize(sr);
            foreach (Scenario sc in server.Scenarios)
            {
                foreach (Request req in sc.Requests)
                {
                    if (req.customVariablesExp != null)
                    {
                        Dictionary<string, string> custVarDes = new Dictionary<string, string>();
                        foreach (CustomVariableItem cvi in req.customVariablesExp)
                            custVarDes.Add(cvi.id, cvi.value);
                        req.customVariables = custVarDes;
                    }
                }
            }
            // Close the stream reader
            sr.Close();
            return server;
        }
    }
}
