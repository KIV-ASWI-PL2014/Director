using System;
using Xwt;

namespace Director
{
    /// <summary>
    /// Static configuraiton class.
    /// </summary>
    public static class Config
    {
        private static ToolkitType AppType = ToolkitType.Cocoa;

        public static bool Cocoa
        {
            get { return AppType == ToolkitType.Cocoa; }
        }

        public static bool Wpf
        {
            get { return AppType == ToolkitType.Wpf; }
        }

        public static bool Gtk
        {
            get { return AppType == ToolkitType.Gtk; }
        }

        public static void SetAppType(ToolkitType type)
        {
            AppType = type;
        }

        public static ToolkitType GetAppType()
        {
            return AppType;
        }
    }
}