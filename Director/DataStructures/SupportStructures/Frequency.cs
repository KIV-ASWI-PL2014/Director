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
            _combo.Items.Add(Director.Properties.Resources.Every5Minutes);
            _combo.Items.Add(Director.Properties.Resources.Every10Minutes);
            _combo.Items.Add(Director.Properties.Resources.Every30Minutes);
            _combo.Items.Add(Director.Properties.Resources.EveryHour);
            _combo.Items.Add(Director.Properties.Resources.Every6Hours);
            _combo.Items.Add(Director.Properties.Resources.Every12Hours);
            _combo.Items.Add(Director.Properties.Resources.EveryDay);
        }
    }
}