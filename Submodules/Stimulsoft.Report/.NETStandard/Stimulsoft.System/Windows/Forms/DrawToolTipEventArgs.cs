using System;
using System.Collections.Generic;
using System.Text;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#else
using System.Drawing;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class DrawToolTipEventArgs : EventArgs
    {
        public Graphics Graphics { get; }
        public global::System.Drawing.Rectangle Bounds { get; }
    }
}
