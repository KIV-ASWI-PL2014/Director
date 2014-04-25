using Director.DataStructures;
using Director.ParserLib;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.Remote
{
    class Remote
    {
        /// <summary>
        /// Send request.
        /// </summary>
        public static RestResponse SendRemoteRequest(Request _request)
        {
            // Create client
            var client = new RestClient(_request.Url);

            // Authentication?
            if (_request.Authentication)
                client.Authenticator = new HttpBasicAuthenticator(_request.AuthName, _request.AuthPassword);

            // Create request
            var request = new RestRequest();

            // Create custom variables if not exists!
            if (_request.ParentScenario.customVariables == null)
                _request.ParentScenario.customVariables = new Dictionary<string, string>();

            // Create request from template
            Parser p = new Parser();
            ParserResult result = p.generateRequest(_request.RequestTemplate, _request.ParentScenario.customVariables);

            if (result.isSuccess() == false)
                throw new InvalidOperationException();
            
            // Set body
            request.AddBody(result.getResult());

            // Set headers
            foreach (var h in _request.Headers)
                request.AddHeader(h.Name, h.Value);

            // Add files
            foreach (var f in _request.Files)
                request.AddFile(f.FileName, f.FilePath);

            // Get response
            return (RestResponse) client.Execute(request);
        }
    }
}
