using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace Director.Forms.Controls
{
    internal class RequestWidget : VBox
    {
        /// <summary>
        /// Type.
        /// </summary>
        /// <value>The type of the request.</value>
        public ComboBox RequestType { get; set; }

        /// <summary>
        /// Method.
        /// </summary>
        /// <value>The request method.</value>
        public ComboBox RequestMethod { get; set; }

        /// <summary>
        /// Active request.
        /// </summary>
        private Request ActiveRequest;

        public RequestWidget(Request _request)
        {
            // Set request
            ActiveRequest = _request;

            // Set margin
            Margin = 10;
        }
    }
}