using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class SystemInformation
    {
        public static int CaptionHeight
        {
            get
            {
                return 0;
            }
        }

        public static int HorizontalScrollBarArrowWidth { get; set; }
        public static Size Border3DSize { get; set; }
        public static int HorizontalScrollBarThumbWidth { get; set; }
        public static Size DragSize { get; }
    }
}
