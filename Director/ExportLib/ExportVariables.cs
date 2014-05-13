using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xwt;

namespace Director.ExportLib
{
    static class ExportVariables
    {
        public static void Export(Server UServer, String fileName)
        {
            // Create structures
            List<ExportScenario> scenarios = new List<ExportScenario>();

            foreach (var s in UServer.Scenarios)
            {
                var tmp = new ExportScenario()
                {
                    Name = s.Name
                };

                foreach (var r in s.Requests)
                {
                    tmp.Requests.Add(new ExportRequest()
                    {
                        Name = r.Name, Method = r.HTTP_METHOD, URL = r.Url
                    });
                }

                foreach (KeyValuePair<string, string> entry in s.customVariables)
                {
                    tmp.Variables.Add(new ExportScenarioVariable()
                    {
                        Name = entry.Key, Value = entry.Value
                    });
                }
                scenarios.Add(tmp);
            }

            try
            {
                // Remove file if exists!
                if (File.Exists(fileName))
                    File.Delete(fileName);

                // Create serializer
                XmlSerializer serializer = new XmlSerializer(typeof(List<ExportScenario>));

                // Create writer
                using (TextWriter writer = new StreamWriter(fileName))
                {
                    serializer.Serialize(writer, scenarios);
                }
                
                MessageDialog.ShowMessage(Director.Properties.Resources.VariableExportSuccess);
            }
            catch
            {
                MessageDialog.ShowError(Director.Properties.Resources.ErrorExportVariables);
            }
        }
    }

    [Serializable]
    public class ExportScenario
    {
        /// <summary>
        /// Scenario name.
        /// </summary>
        [XmlElement("Name")]
        public String Name { get; set; }

        /// <summary>
        /// List of requests in this scenario.
        /// </summary>
        [XmlElement("Requests")]
        public List<ExportRequest> Requests = new List<ExportRequest>();

        /// <summary>
        /// Variables.
        /// </summary>
        [XmlElement("Variables")]
        public List<ExportScenarioVariable> Variables = new List<ExportScenarioVariable>();
    }

    [Serializable]
    public class ExportRequest
    {
        /// <summary>
        /// Request name.
        /// </summary>
        [XmlElement("Name")]
        public String Name { get; set; }

        /// <summary>
        /// Request URL.
        /// </summary>
        [XmlElement("URL")]
        public String URL { get; set; }


        /// <summary>
        /// Request method.
        /// </summary>
        [XmlElement("Method")]
        public String Method { get; set; }
    }

    [Serializable]
    public class ExportScenarioVariable
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        [XmlElement("Name")]
        public String Name { get; set; }

        /// <summary>
        /// Variable value.
        /// </summary>
        [XmlElement("Value")]
        public String Value { get; set; }
    }
}
