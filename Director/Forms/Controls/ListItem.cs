using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Controls
{
    class ListItem : HBox
    {
        /// <summary>
        /// List image!
        /// </summary>
        public static Image ListImage = Image.FromResource(DirectorImages.BULLET_ICON);

        /// <summary>
        /// List item.
        /// </summary>
        public ListItem(string text)
        {
            MarginLeft = 10;
            HeightRequest = 20;
            MinHeight = 20;
            PackStart(new ImageView(ListImage)
            {
                WidthRequest = 20, HeightRequest = 20, MarginTop = 2
            });
            PackStart(new Label(text) { MarginTop = 3 });
        }
    }
}
