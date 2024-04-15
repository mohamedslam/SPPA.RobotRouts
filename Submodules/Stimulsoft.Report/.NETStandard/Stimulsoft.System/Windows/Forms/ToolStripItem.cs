using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public abstract class ToolStripItem : Component, IComponent, IDisposable
    {
        public virtual Image Image { get; set; }

        public AccessibleObject AccessibilityObject { get; }
        public string ToolTipText { get; set; }
    }
}
