using Xwt;
using Director.Forms.Controls;

namespace Director.Forms.Panels
{
    internal class Homepage : PanelBase
    {
        private string MarkDownText
        {
            get { return @"
Project Site: http://director.strnadj.eu/

Icons from: http://www.fatcow.com/

# Overview

Director is project for testing REST APIs by scenarios.

# Contact

- Email: jan.strnadek@gmail.com
- Webpage: http://director.strnadj.eu/

# Missing parts

- Better edit of Response template
- Last open files

# Changelog

- Mac OS X View optimalization

"; }
        }

        /// <summary>
        /// Homepage constructor.
        /// </summary>
        public Homepage() : base(null, "Api Director", DirectorImages.HOMEPAGE_IMAGE)
        {
        }

        /// <summary>
        /// Create components
        /// </summary>
        public override void _initializeComponents()
        {
            var markdown = new MarkdownView()
            {
                Markdown = MarkDownText
            };
            markdown.Margin = 10;

            var scrolled = new ScrollView(markdown)
            {
                MinHeight = 400
            };
            PackStart(scrolled, true);
        }
    }
}