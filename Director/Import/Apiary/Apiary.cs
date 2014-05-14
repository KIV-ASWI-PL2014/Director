using Director.DataStructures;
using Director.Forms.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Import.Apiary
{
    class Apiary
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_s"></param>
        /// <param name="_file"></param>
        public static bool ProcessApiaryFile(Server _s, String _file)
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

            // Parse content
            ApiaryServer s;
            try
            {
                s = JsonConvert.DeserializeObject<ApiaryServer>(content);
            }
            catch
            {
                MessageDialog.ShowError(Director.Properties.Resources.InvalidApiaryFile);
                return false;
            }

            // Set URL
            String url = "";
            while (true)
            {
                var d = new Dialog();
                d.Title = Director.Properties.Resources.FillEndPointUrl;
                d.Width = 340;
                d.Icon = Image.FromResource(DirectorImages.EDIT_ICON);


                VBox DialogContent = new VBox();
                DialogContent.PackStart(new Label(Director.Properties.Resources.FillEndPointUrl));
                TextEntry NewValidUrl = new TextEntry();
                if (url.Length == 0)
                    NewValidUrl.Text = "http://example.com";
                DialogContent.PackStart(NewValidUrl, true, true);

                d.Buttons.Add(new DialogButton(Director.Properties.Resources.ButtonOK, Command.Ok));
                d.Buttons.Add(new DialogButton(Director.Properties.Resources.ButtonStorno, Command.Cancel));
                d.Content = DialogContent;

                var c = d.Run();
                d.Dispose();

                if (c == Command.Ok)
                {
                    Uri _newUri;

                    // Uri validation
                    if (Uri.TryCreate(NewValidUrl.Text, UriKind.Absolute, out _newUri))
                    {
                        url = _newUri.AbsoluteUri;
                        break;
                    }
                    else
                    {
                        MessageDialog.ShowError(Director.Properties.Resources.InvalidUrlError);
                    }
                }
                else
                {
                    return false;
                }
            }

            // Fill data
            if (_s.Name == null || _s.Name.Length == 0)
                _s.Name = s.name;

            if (_s.Url == null || _s.Url.Length == 0)
                _s.Url = url;

            // Z resources group vyrobit scenare
            foreach (var group in s.resourceGroups)
            {
                var scNew = _s.CreateNewScenario();
                scNew.Name = group.name;

                // Z akcí - example vyrobit requesty
                foreach (var resource in group.resources)
                {
                    foreach (var action in resource.actions)
                    {
                        var reqNew = scNew.CreateNewRequest();
                        
                        reqNew.Name = string.Format("{0}/{1}", resource.name, action.name);
                        reqNew.HTTP_METHOD = action.method;

                        // Create URL
                        if (resource.uriTemplate.StartsWith("/") && url.EndsWith("/"))
                        {
                            reqNew.Url = url.Substring(0, url.Length - 1) + resource.uriTemplate;
                        }
                        else
                            reqNew.Url = url + resource.uriTemplate;

                        // Get first request
                        var req = (action.examples.Count > 0 && action.examples[0].requests.Count > 0) ? action.examples[0].requests[0] : null;
                        var res = (action.examples.Count > 0 && action.examples[0].responses.Count > 0) ? action.examples[0].responses[0] : null;

                        if (req != null)
                        {
                            reqNew.RequestTemplate = req.body;
                            foreach (var h in req.headers)
                            {
                                reqNew.Headers.Add(new Header()
                                {
                                    Name = h.name, Value = h.value
                                });
                            }
                        }

                        if (res != null)
                        {
                            reqNew.ResponseTemplate = res.body;
                        }
                        scNew.Requests.Add(reqNew);
                    }
                }
            }

            return true;
        }
    }

    /// Support clases
    public class ApiaryServer
    {
        /// <summary>
        /// Server name.
        /// </summary>
        public String name { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public String description { get; set; }

        /// <summary>
        /// Resoruce groups.
        /// </summary>
        public List<ApiaryResourceGroup> resourceGroups { get; set; }
    }

    public class ApiaryResourceGroup
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
        /// Apiary resources.
        /// </summary>
        public List<ApiaryResources> resources { get; set; }
    }

    public class ApiaryResources
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
        /// Description.
        /// </summary>
        public String uriTemplate { get; set; }

        /// <summary>
        /// Apiary actions.
        /// </summary>
        public List<ApiaryAction> actions { get; set; }
    }

    public class ApiaryAction
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
        /// HTTP method.
        /// </summary>
        public String method { get; set; }

        /// <summary>
        /// Examples.
        /// </summary>
        public List<ApiaryExample> examples { get; set; }
    }

    public class ApiaryExample
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
        /// Requests.
        /// </summary>
        public List<ApiaryRequest> requests { get; set; }

        /// <summary>
        /// Responses.
        /// </summary>
        public List<ApiaryRequest> responses { get; set; }
    }

    public class ApiaryRequest
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
        /// Body.
        /// </summary>
        public String body { get; set; }

        /// <summary>
        /// Headers.
        /// </summary>
        public List<ApiaryHeaader> headers { get; set; }
    }

    public class ApiaryHeaader
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
