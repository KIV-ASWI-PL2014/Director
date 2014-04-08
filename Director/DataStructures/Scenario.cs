using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.DataStructures
{
    /// <summary>
    /// Scenario representing class.
    /// </summary>
    public class Scenario
    {
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
        /// Available frequencies
        /// </summary>
        public enum FREQUENCIES : int
        {
            Never = 0,
            PerThirdtyM,
            PerHour,
            PerThreeHours,
            PerSixHours,
            PerTwelveHours,
            PerDay
        }

        /// <summary>
        /// Public int frequency
        /// </summary>
        public int frequency = (int) FREQUENCIES.Never;

        /// <summary>
        /// Public frequency for settings from GUI.
        /// </summary>
        public int Frequency
        {
            get { return frequency; }
            set { frequency = (int) value; }
        }

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
			Request ret = new Request (_requestId, Requests.Count, "New request");
			Requests.Add(ret);

			// Return request
			return ret;
        }
    }
}
