using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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
		/// Time delay after previous scenario ends! [seconds]
		/// </summary>
		public int TimeAfterPrevious {
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
        public Scenario() : this(0, 0, "") { }

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
            int _requestId = 1;

            // There are some members of request in list (get Max)
            if (Requests.Count > 0)
                _requestId = Requests.Max(x => x.Id) + 1;

            // Create request
			Request ret = new Request (_requestId, Requests.Count, "New request") {
				ParentScenario = this,
				Url = ParentServer.GetUrl(),
				Authentication = ParentServer.Authentication,
				AuthName = ParentServer.AuthName,
				AuthPassword = ParentServer.AuthPassword
			};

			// Copy Server Headers
			// TODO: Use marshalling for that!
			foreach (var h in ParentServer.DefaultHeaders)
				ret.Headers.Add (new Header (h));

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
            XmlSerializer ser = new XmlSerializer(typeof(Scenario));
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            ser.Serialize(writer, this);
            TextReader reader = new StringReader(sb.ToString());
            Scenario t = (Scenario) ser.Deserialize(reader);
            t.ParentServer = this.ParentServer;
            foreach (var r in t.Requests)
                r.ParentScenario = t;

            return t;
        }
    }
}
