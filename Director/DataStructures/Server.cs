using Director.DataStructures.Exceptions;
using Director.DataStructures.SupportStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.DataStructures
{
    /// <summary>
    /// Class Server represents default group of scenarios.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Scenario list.
        /// </summary>
        public List<Scenario> Scenarios { get; set; }

        /// <summary>
        /// List of server email list.
        /// </summary>
        public List<Email> Emails { get; set; }

        /// <summary>
        /// Server name or global scenarios test case name.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Default server end-point api url (pre-filled)
        /// </summary>
        public String Url;

        /// <summary>
        /// Server running frequency!
        /// </summary>
        public int RunningFrequency { get; set; }

        /// <summary>
        /// Use authentication credentials?
        /// </summary>
        public Boolean Authentication { get; set; }

        /// <summary>
        /// Authentication name.
        /// </summary>
        public String AuthName { get; set; }

        /// <summary>
        /// Authentication password.
        /// </summary>
        public String AuthPassword { get; set; }

        /// <summary>
        /// Default headers for requests!
        /// </summary>
        public List<Header> DefaultHeaders = new List<Header>();

        /// <summary>
        /// Default constructor for serialization purposes.
        /// </summary>
        public Server() : this("", "")
        {
        }

        /// <summary>
        /// Create server instance with defined name!
        /// </summary>
        /// <param name="name">New name</param>
        /// <param name="serverUrl">Server URL</param>
        public Server(String name, String serverUrl)
        {
            // Set name
            this.Name = name;
            this.Url = serverUrl;

            // Create scenarios and emails
            Scenarios = new List<Scenario>();
            Emails = new List<Email>();
        }

        /// <summary>
        /// Return actual URL.
        /// </summary>
        /// <returns>URL</returns>
        public String GetUrl()
        {
            return this.Url;
        }

        /// <summary>
        /// Url settings
        /// </summary>
        /// <param name="url"></param>
        public void SetUrl(String url)
        {
            Uri _newUri;

            // Uri validation
            if (Uri.TryCreate(url, UriKind.Absolute, out _newUri))
                this.Url = _newUri.AbsoluteUri;
            else
                throw new InvalidUrlException();
        }


        /// <summary>
        /// Create new next scenario!
        /// </summary>
        public Scenario CreateNewScenario()
        {
            // Id and position is actual scenario list size
            int _scenarioId = 1;

            // There are some members of scenario in list
            if (Scenarios.Count > 0)
                _scenarioId = Scenarios.Max(x => x.Id) + 1;

            // Create a new one
            var newScenario = new Scenario(_scenarioId, Scenarios.Count, Director.Properties.Resources.NewScenarioName)
            {
                ParentServer = this
            };

            // Add to list
            Scenarios.Add(newScenario);

            // Return scenario
            return newScenario;
        }

        /// <summary>
        /// Remove scenario item.
        /// </summary>
        /// <param name="sc">Sc.</param>
        public void RemoveScenario(Scenario sc)
        {
            Scenarios.Remove(sc);
        }
    }
}