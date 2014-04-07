using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Controls;

namespace Director.Forms.Panels
{
	/// <summary>
	/// Request description.
	/// </summary>
	class RequestPage : VBox
	{
		public RequestPage()
		{
			InfoBox _infoBox = new InfoBox(Director.Locales.Language.RequestInfoBox, DirectorImages.REQUEST_IMAGE);
			PackStart(_infoBox);
			MarginLeft = 10;
		}
	}
}