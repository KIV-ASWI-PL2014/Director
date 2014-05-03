using System;

namespace Director.DataStructures.Exceptions
{
    /// <summary>
    /// Invalid URL exception
    /// </summary>
    [Serializable]
    internal class InvalidUrlException : Exception
    {
        public InvalidUrlException() : base("Invalid URL")
        {
        }
    }
}