using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.DataStructures.SupportStructures
{
    /// <summary>
    /// User emails
    /// </summary>
    public class Email
    {
        [DisplayName("Email address")]
        public String UserEmail { get; set; }

        [DisplayName("Information reports")]
        public bool Notifications { get; set; }

        [DisplayName("Erorr reports")]
        public bool Errors { get; set; }
    }
}
