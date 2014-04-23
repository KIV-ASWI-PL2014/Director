using Director.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace Director.Forms.Controls
{
    internal class ResponseWidget : VBox
    {
        /// <summary>
        /// Expected response status code.
        /// </summary>
        private TextEntry ExpectedStatusCode { get; set; }

        /// <summary>
        /// Active request.
        /// </summary>
        private Request ActiveRequest;

        public ResponseWidget(Request _request)
        {
            // Set request
            ActiveRequest = _request;

            // Set margin
            Margin = 10;

            // Expected status code
            PackStart(new Label(Director.Properties.Resources.ExpectedStatusCode));
            ExpectedStatusCode = new TextEntry()
            {
                Text = ActiveRequest.ExpectedStatusCode + "",
                ExpandHorizontal = true
            };
            PackStart(ExpectedStatusCode, expand: false, fill: false);
            ExpectedStatusCode.Changed += ExpectedStatusCode_Changed;
        }

        /// <summary>
        /// Expected status code validation.
        /// </summary>
        private void ExpectedStatusCode_Changed(object sender, EventArgs e)
        {
        }
    }
}