using Director.DataStructures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace Director.Import.Postman
{
    class PostMan
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_s"></param>
        /// <param name="_file"></param>
        public static bool ProcessPostmanFile(Server _s, String _file)
        {
            // Read file to variable
            String content = "";
            try
            {
                using (StreamReader sr = new StreamReader(_file))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch
            {
                MessageDialog.ShowError(Director.Properties.Resources.CannotReadFromFile);
                return false;
            }

            PostmanServer s = new PostmanServer();
            try
            {
                JObject o = JObject.Parse(content);
                s.name = (String) o["name"];
                s.requests = new List<PostmanRequest>();

                foreach (JObject re in o["requests"])
                {
                    var tmp = new PostmanRequest();
                    tmp.name = (String)re["name"];
                    tmp.method = (String)re["method"];
                    tmp.url = (String)re["url"];
                    tmp.headers = (String)re["headers"];

                    // Data
                    if (((String)re["dataMode"]) == "raw")
                    {
                        tmp.data = (String)re["data"];
                    }
                    
                    s.requests.Add(tmp);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                MessageDialog.ShowError(Director.Properties.Resources.InvalidPostmanFile);
                return false;
            }

            // Set server header?
            if (_s.Name == null || _s.Name.Length == 0)
                _s.Name = s.name;

            if (_s.Url == null || _s.Url.Length == 0)
            {
                _s.Url = s.requests.Count > 0 ? s.requests[0].url : "";
            }

            // Create scenario
            var scNew = _s.CreateNewScenario();
            scNew.Name = s.name;

            foreach (var pr in s.requests)
            {
                var tmp = scNew.CreateNewRequest();
                tmp.Name = pr.name;
                tmp.HTTP_METHOD = pr.method;
				tmp.RequestTemplateType = ContentTypeUtils.GuessContentType (pr.data);
                tmp.RequestTemplate = pr.data;
                tmp.Url = pr.url;

                // Create headers
                foreach (var h in pr.HeaderList)
                {
                    tmp.Headers.Add(new Header()
                    {
                        Name = h.name, Value = h.value
                    });
                }
            }

            return true;
        }
    }

    /// Support clases
    public class PostmanServer
    {
        /// <summary>
        /// Server name.
        /// </summary>
        public String name { get; set; }

        /// <summary>
        /// Resoruce groups.
        /// </summary>
        public List<PostmanRequest> requests { get; set; }
    }

    public class PostmanRequest
    {
        /// <summary>
        /// Resrouce group name.
        /// </summary>
        public String name { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public String description { get; set; }

        /// <summary>
        /// Server url.
        /// </summary>
        public String url { get; set; }

        /// <summary>
        /// Body.
        /// </summary>
        public String data { get; set; }

        /// <summary>
        /// HTTP method.
        /// </summary>
        public String method { get; set; }

        /// <summary>
        /// HTTP headers.
        /// </summary>
        public String headers { get; set; }


        /// <summary>
        /// Get headerlist
        /// </summary>
        public List<PostmanHeader> HeaderList
        {
            get
            {
                List<PostmanHeader> ret = new List<PostmanHeader>();
                String[] h = headers.Split('\n');

                foreach (var s in h)
                {
                    int c = s.IndexOf(':');
                    if (c > 0)
                    {
                        ret.Add(new PostmanHeader()
                        {
                            name = s.Substring(0, c).Trim(),
                            value = s.Substring(c+1).Trim()
                        });
                    }
                }

                return ret;
            }
        }
    }


    public class PostmanHeader
    {
        /// <summary>
        /// Name.
        /// </summary>
        public String name { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        public String value { get; set; }
    }
}
