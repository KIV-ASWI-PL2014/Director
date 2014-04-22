using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Director.DataStructures.Exceptions;
using System.IO;

namespace Director.DataStructures
{
    /// <summary>
    /// Request representation class.
    /// </summary>
    [Serializable]
    public class Request
    {
		/// <summary>
		/// Parent scenario (non serialized - cycles)
		/// </summary>
		/// <value>The parent scenario.</value>
		[XmlIgnore]
		public Scenario ParentScenario { get; set; }

        /// <summary>
        /// Request ID in scenario list.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Request position - working position.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Request name.
        /// </summary>
        public String Name { get; set; }

		/// <summary>
		/// Requests url address.
		/// </summary>
		/// <value>The URL.</value>
		public String Url { get; set; }

		/// <summary>
		/// HTTP headers.
		/// </summary>
		public List<Header> Headers = new List<Header> ();

		/// <summary>
		/// Authentication name.
		/// </summary>
		public String AuthName { get; set; }

		/// <summary>
		/// Authentication password.
		/// </summary>
		/// <value>The auth password.</value>
		public String AuthPassword { get; set; }

		/// <summary>
		/// Use http authentication.
		/// </summary>
		public Boolean Authentication { get; set; }

		/// <summary>
		/// Request Method (POST|GET|PUT)
		/// </summary>
		/// <value>The HTT p_ METHO.</value>
		public String HTTP_METHOD { get; set; }

        /// <summary>
        /// Empty constructor for XML serialization!
        /// </summary>
        public Request() : this(0, 0, "") { }

        /// <summary>
        /// Parametrize contructor for manually creating request.
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="position">Position</param>
        /// <param name="name">Request identifier</param>
        public Request(int id, int position, String name)
        {
            Id = id;
            Position = position;
            Name = name;
			HTTP_METHOD = "GET";
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
        /// Get clone of myself.
        /// </summary>
        /// <returns></returns>
        public Request Clone()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Request));
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            ser.Serialize(writer, this);
            TextReader reader = new StringReader(sb.ToString());
            Request t = (Request)ser.Deserialize(reader);
            t.ParentScenario = this.ParentScenario;
            return t;
        }

        /// <summary>
        /// Get string markdown info.
        /// </summary>
        public String GetMarkdownInfo()
        {
            string text = "";

            // Request method
            text += "# " + Director.Properties.Resources.RequestMethod + "\n";
            if (HTTP_METHOD == null)
            {
                text += "* " + Director.Properties.Resources.NoRequestMethod + "\n\n";
            }
            else
                text += "* " + HTTP_METHOD + "\n\n";

            // Headers
            if (Headers.Count > 0)
            {
                text += "# " + Director.Properties.Resources.RequestHeaders + "\n";

                // Get all headers
                foreach (var h in Headers)
                {
                    text += "* " + h.Name + " - " + h.Value + "\n";
                }
            }

            return text;
        }
    }
}
