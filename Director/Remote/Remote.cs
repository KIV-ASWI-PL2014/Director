﻿using Director.DataStructures;
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
        public static RestResponse SendRemoteRequest(Request _request)
        {
            // Create client
            var client = new RestClient(_request.Url);

            // Authentication?
            if (_request.Authentication)
                client.Authenticator = new HttpBasicAuthenticator(_request.AuthName, _request.AuthPassword);

            // Create request
            var request = new RestRequest(_request.RequestMethod);

            // Create custom variables if not exists!
            if (_request.ParentScenario.customVariables == null)
                _request.ParentScenario.customVariables = new Dictionary<string, string>();

            // Create request from template
			if (_request.RequestTemplate != null && _request.RequestTemplate.Length > 0) {
				Parser p = new Parser ();
				ParserResult result = p.generateRequest (_request.RequestTemplate, _request.ParentScenario.customVariables);


                if (result.isSuccess() == false)
                    throw new InvalidOperationException();
            
    	        // Set body
	            request.AddParameter("application/json", result.getResult(), ParameterType.RequestBody);

				// Set body
				request.AddBody(result.getResult());
			}
            
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
