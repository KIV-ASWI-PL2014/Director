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
        /// <summary>
        /// User email address.
        /// </summary>
        public String UserEmail { get; set; }

        /// <summary>
        /// Information notifications?
        /// </summary>
        public bool Notifications { get; set; }

        /// <summary>
        /// Error notifications?
        /// </summary>
        public bool Errors { get; set; }

        /// <summary>
        /// Position.
        /// </summary>
        public int Position { set; get; }


        /// <summary>
        /// Determines if is valid the specified email.
        /// </summary>
        /// <returns><c>true</c> if is valid the specified email; otherwise, <c>false</c>.</returns>
        /// <param name="email">Email.</param>
        public static bool IsValid(string email)
        {
            try
            {
                new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}