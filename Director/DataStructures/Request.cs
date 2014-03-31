using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.DataStructures
{
    public class Request
    {
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
        }
    }
}
