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
    [Serializable]
    class InvalidUrlException : Exception
    {
        public InvalidUrlException() : base("Invalid URL")
        { 
            
        }
    }
}
