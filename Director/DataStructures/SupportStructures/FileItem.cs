using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
