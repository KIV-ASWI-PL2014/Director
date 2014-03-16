using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.DataStructures.Exceptions
{
    /// <summary>
    /// Invalid URL exception
    /// </summary>
    class InvalidUrlException : Exception
    {
        public InvalidUrlException() : base("Invalid URL")
        { 
            
        }
    }
}
