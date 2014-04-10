using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;
using Director.Forms.Controls;

namespace Director.Forms.Panels
{
    class Homepage : PanelBase
    {

		string MarkDownText
		{
			get
			{
				return @"
# Director - Rest Scenario Tester

Project Site: http://director.strnadj.eu/

## Overview

Director is project for testing REST APIs by scenarios.

## Contact

- Email: jan.strnadek@gmail.com
- Webpage: http://director.strnadj.eu/

## Changelog

- Mac OS X View optimalization

";
			}
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