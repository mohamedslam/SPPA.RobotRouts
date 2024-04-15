using System;

namespace Stimulsoft.System.Windows.Forms
{
    public class OSFeature
    {
        public static OSFeature Feature { get; }

        public static readonly object LayeredWindows;

        public static readonly object Themes;
        public Version GetVersionPresent(object feature)
        {
            return null;
        }
    }
}
