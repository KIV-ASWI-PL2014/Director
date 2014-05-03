using Director.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.About
{
    class About : Window
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public About()
        {
            Title = Director.Properties.Resources.About;
            Height = 320;
            Width = 480;
            Resizable = false;

            // Content box
            VBox ContentBox = new VBox();

            // Image
            ImageView img = new ImageView();
            img.Image = Image.FromResource(DirectorImages.DIRECTOR_IMAGE);
            ContentBox.PackStart(img);

            // Informations
            Table Informations = new Table()
            {
                Margin = 10
            };
            Informations.Add(new Label() { Markup = "<b>Director</b>" }, 0, 0, colspan: 2);

            // Version
            Informations.Add(new Label(Director.Properties.Resources.Version + ":"), 0, 1);
            Informations.Add(new Label("0.1a"), 1, 1);

            // Homepage
            Informations.Add(new Label(Director.Properties.Resources.HomepageUrl + ":"), 0, 2);
            LinkLabel _info = new LinkLabel("http://director.strnadj.eu/")
            {
                Uri = new Uri("http://director.strnadj.eu/")
            };
            Informations.Add(_info, 1, 2);

            // Authors
            Informations.Add(new Label(Director.Properties.Resources.Authors + ":"), 0, 3);
            Informations.Add(new Label("Jan Strnadek, David Pejrimovsky, Vaclav Stengl"), 1, 3);

            ContentBox.PackStart(Informations, expand: true, fill: true);

            // Button
            Button ConfirmButton = new Button(Image.FromResource(DirectorImages.OK_ICON), Director.Properties.Resources.Close)
            {
                WidthRequest = 150,
                ExpandHorizontal = false,
                ExpandVertical = false
            };
            ContentBox.PackStart(ConfirmButton, expand: false, hpos: WidgetPlacement.End);
            ConfirmButton.Clicked += delegate
            {
                this.Close();
            };


            // Set content
            Content = ContentBox;
        }
    }
}
