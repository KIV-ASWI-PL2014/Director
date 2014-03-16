using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.DataStructures
{
    public class Scenario
    {
        // Request list
        private List<Request> _requests = new List<Request>();
        private int _id;
        public int Id { get; set; }
        private int _position;
        public int Position { get; set; }
        private String _name;
        public String Name { get; set; }

        public Scenario() : this(0,0)
        { 
            
        }

        public Scenario(int id, int position) {
            Position = position;
            Id = id;
        }
    }
}
