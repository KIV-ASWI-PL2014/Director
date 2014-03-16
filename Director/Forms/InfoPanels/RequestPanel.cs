using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Director.DataStructures;

namespace Director.Forms.InfoPanels
{
    public partial class RequestPanel : UserControl
    {
        private Request _request;

        public RequestPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set request data, this data can be changed!
        /// </summary>
        /// <param name="request">Request instance</param>
        public void SetRequest(Request request)
        {
            _request = request;
        }
    }
}
