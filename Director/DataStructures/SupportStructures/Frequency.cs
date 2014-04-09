﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace Director.DataStructures.SupportStructures
{
    /// <summary>
    /// Its not pretty, but it is because we need this enumeration on server.
    /// </summary>
    public enum Frequency : int
    {
        EveryFiveMinutes = 0,
        EveryTenMinutes,
        EveryThirtyMinutes,
        EveryHour,
        EverySixHours,
        EveryTwelveHours,
        EveryDay
    }

    public class FrequencyHelper
    { 
        /// <summary>
        /// Add Frequency item to combo box.
        /// </summary>
        /// <param name="_combo"></param>
        public static void FillComboBox(ComboBox _combo)
        {
            _combo.Items.Add(Director.Locales.Language.Every5Minutes);
            _combo.Items.Add(Director.Locales.Language.Every10Minutes);
            _combo.Items.Add(Director.Locales.Language.Every30Minutes);
            _combo.Items.Add(Director.Locales.Language.EveryHour);
            _combo.Items.Add(Director.Locales.Language.Every6Hours);
            _combo.Items.Add(Director.Locales.Language.Every12Hours);
            _combo.Items.Add(Director.Locales.Language.EveryDay);
        }
    }
}
