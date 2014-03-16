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
        private String _name;
        public String Name { get; set; }

        /// <summary>
        /// Default server end-point api url (pre-filled)
        /// </summary>
        private String Url;

        /// <summary>
        /// Server running frequency!
        /// </summary>
        private String RunningFrequency { get; set; }

        /// <summary>
        /// Authentication name.
        /// </summary>
        private String AuthName { get; set; }

        /// <summary>
        /// Authentication password.
        /// </summary>
        private String AuthPassword { get; set; }

        /// <summary>
        /// Default constructor for serialization purposes.
        /// </summary>
        public Server() : this ("") { }

        /// <summary>
        /// Create server instance with defined name!
        /// </summary>
        /// <param name="name">New name</param>
        public Server(String name)
        {
            // Set name
            this.Name = name;
            
            // Create scenarios and emails
            Scenarios = new List<Scenario>();
            Emails = new List<Email>();
        }

        /// <summary>
        /// Return actual URL.
        /// </summary>
        /// <returns>URL</returns>
        public String getUrl()
        {
            return this.Url;
        }

        /// <summary>
        /// Url settings
        /// </summary>
        /// <param name="url"></param>
        public void setUrl(String url)
        {
            Uri _newUri;

            // Uri validation
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _newUri))
                this.Url = url;
            else
                throw new InvalidUrlException();
        }


        /// <summary>
        /// Create new next scenario!
        /// </summary>
        public void CreateNewScenario()
        {
            // Id and position is actual scenario list size
            int idAndPosition = this.Scenarios.Count;

            // Create a new one
            Scenarios.Add(new Scenario(idAndPosition, idAndPosition));
        }
    }
}
