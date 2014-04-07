using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Controls
{
    class InfoBox : Canvas
    {
        /// <summary>
        /// Default margin.
        /// </summary>
        private double margin = 1;

        /// <summary>
        /// Box size.
        /// </summary>
        private Size BoxSize = new Size(40, 40);

        /// <summary>
        /// Label name.
        /// </summary>
        private String Label { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        private String Logo { get; set; }
				
        /// <summary>
        /// Default constructor for info box.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="image"></param>
        public InfoBox(string label, String image)
        {
            // Set size
            MinWidth = BoxSize.Width + margin * 2;
            MinHeight = BoxSize.Height + margin * 2;

            // Set label and image
            Label = label;
            Logo = image;
        }
		
		protected override void OnDraw (Context ctx, Rectangle dirtyRect)
		{
            // Line arround
			ctx.SetColor(Colors.DarkGray);

            // Drive line arround
			ctx.Rectangle (Bounds);
			ctx.Fill ();


			ctx.SetColor (Colors.Gray);
			ctx.Rectangle (Bounds.Inflate (-margin, -margin)); 
			ctx.Fill ();
			
            // Draw image
            ctx.DrawImage(Image.FromResource(Logo), 5.0, 5.0);

            // Draw text
            ctx.SetColor(Colors.White);
            TextLayout _layout = new TextLayout();
            _layout.Font = Font.WithSize(22);
            _layout.Text = Label;
            _layout.SetFontWeight(FontWeight.Bold, 0, Label.Length);

			// Cocoa layouts
			ctx.DrawTextLayout (_layout, 45, ((Config.Cocoa || Config.Gtk) ? 5 : 2));
		}
    }
}
