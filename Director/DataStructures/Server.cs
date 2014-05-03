using Director.DataStructures.Exceptions;
using Director.DataStructures.SupportStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Xwt;

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
        [XmlIgnore]
        public List<Scenario> Scenarios { get; set; }

        /// <summary>
        /// Tree position.
        /// </summary>
        [XmlIgnore]
        public TreePosition TreePosition { get; set; }

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
            int _scenarioId = 1, _position = 1;

            // There are some members of scenario in list
            if (Scenarios.Count > 0)
            {
                _scenarioId = Scenarios.Max(x => x.Id) + 1;
                _position = Scenarios.Max(x => x.Position) + 1;
            }

            // Create a new one
            var newScenario = new Scenario(_scenarioId, _position, Director.Properties.Resources.NewScenarioName)
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

        /// <summary>
        /// Switch scenario with scenario after.
        /// </summary>
        public Scenario MoveScenarioDown(Scenario _scenario)
        {
            // Get max position ID
            int _maxPosition = Scenarios.Max(n => n.Position);

            if (_maxPosition == _scenario.Position)
                return null;

            // Find scenario after
            for (int i = _scenario.Position + 1; i <= _maxPosition; i++)
            {
                var _newScenario = Scenarios.First(n => n.Position == i);
                if (_newScenario != null)
                {
                    _newScenario.Position = _scenario.Position;
                    _scenario.Position = i;
                    return _newScenario;
                }
            }
            return null;
        }

        /// <summary>
        /// Switch scenario with scenario before.
        /// </summary>
        public Scenario MoveScenarioUp(Scenario _scenario)
        {
            // Get max position ID
            int _minPosition = Scenarios.Min(n => n.Position);

            if (_minPosition == _scenario.Position)
                return null;

            // Find scenario after
            for (int i = _scenario.Position - 1; i >= _minPosition; i--)
            {
                var _newScenario = Scenarios.First(n => n.Position == i);
                if (_newScenario != null)
                {
                    _newScenario.Position = _scenario.Position;
                    _scenario.Position = i;
                    return _newScenario;
                }
            }
            return null;
        }
    }
}