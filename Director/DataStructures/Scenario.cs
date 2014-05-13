using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Xwt;

namespace Director.DataStructures
{
    /// <summary>
    /// Scenario representing class.
    /// </summary>
    [Serializable]
    public class Scenario
    {
        /// <summary>
        /// Parent server of this scenario list.
        /// </summary>
        [XmlIgnore]
        public Server ParentServer { get; set; }

        /// <summary>
        /// Tree position - set when creating.
        /// </summary>
        [XmlIgnore]
        public TreePosition TreePosition { get; set; }

        /// <summary>
        /// Request list.
        /// </summary>
        public List<Request> Requests { get; set; }

        /// <summary>
        /// Scenario id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Scenario ordering position (tree view, running order).
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Scenario name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Public int frequency
        /// </summary>
        public int RunningFrequency { get; set; }

        /// <summary>
        /// Private int.
        /// </summary>
        private int _TimeAfterPrevious;

        /// <summary>
        /// Custom variables for parser.
        /// </summary>
		[XmlIgnore]
        public Dictionary<string, string> customVariables { get; set; }

        /// <summary>
        /// Custom variables for serialization
        /// </summary>
        public CustomVariableItem[] customVariablesExp { get; set; }

        /// <summary>
        /// Time delay after previous scenario ends! [seconds]
        /// </summary>
        public int TimeAfterPrevious
        {
            get { return _TimeAfterPrevious; }
            set
            {
                if (value < 0)
                    throw new Exception("Value must be greather than 0");
                _TimeAfterPrevious = value;
            }
        }

        /// <summary>
        /// Running by Running Frequency, otherwise scenario after scenario...
        /// </summary>
        public Boolean PeriodicityRunning { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Scenario() : this(0, 0, "")
        {
        }

        /// <summary>
        /// Constructor wiht ID and position, name.
        /// </summary>
        /// <param name="id">Scenario ID</param>
        /// <param name="position">Scenario order position</param>
        /// <param name="name">Scenario name</param>
        public Scenario(int id, int position, String name)
        {
            Position = position;
            Id = id;
            Name = name;
            Requests = new List<Request>();

            // Set default values
            RunningFrequency = 0;
            TimeAfterPrevious = 10;
            PeriodicityRunning = false;
            customVariables = new Dictionary<string, string>();
        }

        /// <summary>
        /// Get request ID list for comparing.
        /// </summary>
        /// <returns>List request ID</returns>
        public List<int> GetRequestIds()
        {
            List<int> _res = new List<int>();
            foreach (Request _tmp in Requests)
                _res.Add(_tmp.Id);

            return _res;
        }

        /// <summary>
        /// Create new empty request in list.
        /// </summary>
        public Request CreateNewRequest()
        {
            // Get max ID
            int _requestId = 1, _position = 1;

            // There are some members of request in list (get Max)
            if (Requests.Count > 0)
            {
                _requestId = Requests.Max(x => x.Id) + 1;
                _position = Requests.Max(x => x.Position) + 1;
            }

            // Create request
            Request ret = new Request(_requestId, _position, Director.Properties.Resources.NewRequestName)
            {
                ParentScenario = this,
                Url = ParentServer.GetUrl()
            };

            // Copy Server Headers
            foreach (var h in ParentServer.DefaultHeaders)
                ret.Headers.Add(new Header(h));

            // Add new request
            Requests.Add(ret);

            // Return request
            return ret;
        }

        /// <summary>
        /// Get clone of myself.
        /// </summary>
        /// <returns></returns>
        public Scenario Clone()
        {
            XmlSerializer ser = new XmlSerializer(typeof (Scenario));
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            ser.Serialize(writer, this);
            TextReader reader = new StringReader(sb.ToString());
            Scenario t = (Scenario) ser.Deserialize(reader);
            t.ParentServer = this.ParentServer;
            foreach (var r in t.Requests)
                r.ParentScenario = t;

            // Set new position
            t.Position = this.ParentServer.Scenarios.Max(n => n.Position) + 1;

            // Tree position is set by GUI

            return t;
        }

        /// <summary>
        /// Move request up
        /// </summary>
        /// <param name="_request"></param>
        /// <returns></returns>
        internal Request MoveRequestDown(Request _request)
        {
            // Get max position ID
            int _maxPosition = Requests.Max(n => n.Position);

            if (_maxPosition == _request.Position)
                return null;

            // Find scenario after
            for (int i = _request.Position + 1; i <= _maxPosition; i++)
            {
                var _newRequest = Requests.First(n => n.Position == i);
                if (_newRequest != null)
                {
                    _newRequest.Position = _request.Position;
                    _request.Position = i;
                    return _newRequest;
                }
            }
            return null;
        }

        /// <summary>
        /// Move request up
        /// </summary>
        /// <param name="_request"></param>
        /// <returns></returns>
        internal Request MoveRequestUp(Request _request)
        {
            // Get max position ID
            int _minPosition = Requests.Min(n => n.Position);

            if (_minPosition == _request.Position)
                return null;

            // Find scenario after
            for (int i = _request.Position - 1; i >= _minPosition; i--)
            {
                var _newRequest = Requests.First(n => n.Position == i);
                if (_newRequest != null)
                {
                    _newRequest.Position = _request.Position;
                    _request.Position = i;
                    return _newRequest;
                }
            }
            return null;
        }

        /// <summary>
        /// Remove request.
        /// </summary>
        public void RemoveRequest(Request req)
        {
            Requests.Remove(req);
        }
    }
}