using System;
using System.ComponentModel;

namespace Director.DataStructures.SupportStructures
{
    /// <summary>
    /// File item.
    /// </summary>
    public class FileItem
    {
        /// <summary>
        /// File path.
        /// </summary>
        [DefaultValue("")]
        public String FilePath { get; set; }

        /// <summary>
        /// File name for request.
        /// </summary>
        [DefaultValue("")]
        public String FileName { get; set; }
    }
}