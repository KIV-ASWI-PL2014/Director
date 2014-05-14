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
		public static RestResponse SendRemoteRequest(Request _request, RestRequest request, String URL)
        {
            // Create client
			var client = new RestClient(URL);

            // Authentication?
			Server server = _request.ParentScenario.ParentServer;
			if (server.Authentication)
				client.Authenticator = new HttpBasicAuthenticator (server.AuthName, server.AuthPassword);


            // Add files
            foreach (var f in _request.Files)
                request.AddFile(f.FileName, f.FilePath);

            // Get response
            return (RestResponse) client.Execute(request);
        }
    }
}
