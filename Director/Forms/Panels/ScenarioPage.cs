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
	/// Scenario description.
	/// </summary>
	class ScenarioPage : VBox
	{
		public ScenarioPage()
		{
			InfoBox _infoBox = new InfoBox(Director.Locales.Language.ScenarioInfoBox, DirectorImages.SCENARIO_IMAGE);
			PackStart(_infoBox);
			MarginLeft = 10;
		}
	}
}
