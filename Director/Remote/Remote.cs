using Director.DataStructures;
using Director.ParserLib;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Director.Remote
{
    class Remote
    {
        /// <summary>
        /// Send request.
        /// </summary>
        public static RestResponse SendRemoteRequest(Request _request, String body, String BodyParameter)
        {
            // Create client
            var client = new RestClient(_request.Url);

            // Authentication?
			Server server = _request.ParentScenario.ParentServer;
			if (server.Authentication)
				client.Authenticator = new HttpBasicAuthenticator (server.AuthName, server.AuthPassword);

            // Create request
            var request = new RestRequest(_request.RequestMethod);

            // Create request from template
            if (body != null)
                request.AddParameter(BodyParameter, body, ParameterType.RequestBody);
          
            // Set headers
            foreach (var h in _request.Headers)
            {
                // TODO: Catch exception
                ParserResult result = Parser.parseHeader(h.Value, _request.ParentScenario.customVariables);
                request.AddHeader(h.Name, result.getResult());
            }

            // Add files
            foreach (var f in _request.Files)
                request.AddFile(f.FileName, f.FilePath);

            // Get response
            return (RestResponse) client.Execute(request);
        }
    }
}
