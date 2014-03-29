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
            InfoBox _infoBox = new InfoBox("Api Director", "Director.Resources.homepage.png");
            PackStart(_infoBox);
            MarginLeft = 10;
        }
    }
}