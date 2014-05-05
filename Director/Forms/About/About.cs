using Director.Forms.Controls;
using System;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.About
{
    class About : Dialog
    {
        /// <summary>
        /// X.
        /// </summary>
        private int TableX { get; set; }

        /// <summary>
        /// Informations table.
        /// </summary>
        private Table Informations { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public About()
        {
            // Set title, size and icon
            Title = Director.Properties.Resources.About;
            Icon = Image.FromResource(DirectorImages.HELP_ICON);
            Height = 320;
            Width = 480;
            Resizable = false;

            // Set X
            TableX = 0;

            // Init components
            _initializeComponnents();
        }

        /// <summary>
        /// Initialize components.
        /// </summary>
        private void _initializeComponnents()
        {
            // Content box
            VBox ContentBox = new VBox();

            // Image
            ImageView img = new ImageView();
            img.Image = Image.FromResource(DirectorImages.DIRECTOR_IMAGE);
            ContentBox.PackStart(img);

            // Informations
            Informations = new Table()
            {
                Margin = 10
            };

            // Director name
            AddTwoColumnsLabel(new Label() { Markup = "<b>Director</b>" });
            
            // Version
            AddInformation(Director.Properties.Resources.Version + ":", "0.1a");

            // Homepage
            AddLink(Director.Properties.Resources.HomepageUrl + ":", "http://director.strnadj.eu/", "http://director.strnadj.eu/");

            // Authors
            AddInformation(Director.Properties.Resources.Authors + ":", "Jan Strnadek, David Pejrimovsky, Vaclav Stengl");

            // Icons by
            AddLink(Director.Properties.Resources.Icons + ":", "FatCow Web Icons", "http://www.fatcow.com/");

            // Set content box
            ContentBox.PackStart(Informations, expand: true, fill: true);

            // Button
            Buttons.Add(new DialogButton(Director.Properties.Resources.Close, Image.FromResource(DirectorImages.OK_ICON), Command.Ok));

            // Set content
            Content = ContentBox;
        }

        /// <summary>
        /// Add informations.
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="value"></param>
        void AddInformation(String desc, String value)
        {
            Informations.Add(new Label(desc), 0, TableX);
            Informations.Add(new Label(value), 1, TableX);
            TableX++;
        }

        /// <summary>
        /// Add link.
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="linkDesc"></param>
        /// <param name="link"></param>
        void AddLink(String desc, String linkDesc, String link)
        {
            Informations.Add(new Label(desc), 0, TableX);
            LinkLabel _link = new LinkLabel(linkDesc)
            {
                Uri = new Uri(link)
            };
            Informations.Add(_link, 1, TableX);
            TableX++;
        }

        /// <summary>
        /// Add label!
        /// </summary>
        /// <param name="l"></param>
        void AddTwoColumnsLabel(Label l)
        {
            Informations.Add(l, 0, TableX, colspan: 2);
            TableX++;
        }
    }
}
