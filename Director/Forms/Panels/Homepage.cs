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
		/// Homepage initializer.
		/// </summary>
        public Homepage()
        {
            InfoBox _infoBox = new InfoBox("Api Director", DirectorImages.HOMEPAGE_IMAGE);
            PackStart(_infoBox);
            MarginLeft = 10;

			var markdown = new MarkdownView() {
				Markdown = MarkDownText
			};
			markdown.Margin = 10;

			var scrolled = new ScrollView (markdown) {
				MinHeight = 400
			};
			PackStart (scrolled, true);
        }
    }
}