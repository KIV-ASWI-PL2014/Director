using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Controls;
using Director.DataStructures;

namespace Director.Forms.Panels
{
    /// <summary>
    /// Server description.
    /// </summary>
    class ServerPage : VBox
    {
		/// <summary>
		/// Current server instance.
		/// </summary>
		/// <value>The active server.</value>
		private Server ActiveServer { get; set; }

		public ServerPage()
		{
			InfoBox _infoBox = new InfoBox("Server", DirectorImages.HOMEPAGE_IMAGE);
			PackStart(_infoBox);
			MarginLeft = 10;
		}

		/// <summary>
		/// Set server instance.
		/// </summary>
		/// <param name="server">Server.</param>
		public void SetServer(Server server)
		{
			// Fill columns
			ActiveServer = server;
		}
    }
}
