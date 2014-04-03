using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Controls;

namespace Director.Forms.Panels
{
    class Homepage : VBox
    {

        public Homepage()
        {
            InfoBox _infoBox = new InfoBox("Api Director", DirectorImages.HOMEPAGE_IMAGE);
            PackStart(_infoBox);
            MarginLeft = 10;

			TextEntry ta = new TextEntry ();
			ta.ButtonPressed += HandleButtonPressed;
			Label la = new Label ("Right click here to show the context menu");
			menu = new Menu ();
			menu.Items.Add (new MenuItem ("One"));
			menu.Items.Add (new MenuItem ("Two"));
			menu.Items.Add (new MenuItem ("Three"));
			menu.Items.Add (new SeparatorMenuItem ());
			menu.Items.Add (new MenuItem ("End"));

			la.ButtonPressed += HandleButtonPressed;
			PackStart (la);
			PackStart (ta);
        }
			
		Menu menu;

		void HandleButtonPressed (object sender, ButtonEventArgs e)
		{
			if (e.Button == PointerButton.Right)
				menu.Popup ();
		}
    }
}