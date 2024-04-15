using System;
using System.Drawing;
using System.Text;

namespace Stimulsoft.System.Drawing
{
    public static class ColorExt
    {
        public static bool GetIsSystemColor(this Color color)
        {
            return false;
        }

        public static bool GetIsKnownColor(this Color color)
        {
            try
            {
                var known = (int)Enum.Parse(typeof(KnownColor), color.Name);
                return known > 0;
            }
            catch
            {
                return false;
            }
        }

        public static Color FromKnownColor(KnownColor color)
        {
            return Color.FromName(color.ToString());
        }

        public static KnownColor ToKnownColor(this Color color)
        {
            return (KnownColor)Enum.Parse(typeof(KnownColor), color.Name);
        }
    }
}
