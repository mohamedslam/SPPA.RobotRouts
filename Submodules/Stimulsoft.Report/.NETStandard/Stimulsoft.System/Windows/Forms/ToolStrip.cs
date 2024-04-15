using System;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.System.Windows.Forms
{
    public class ToolStrip : ScrollableControl, IComponent, IDisposable
    {
        public virtual ToolStripItemCollection Items { get; }

        public Size ImageScalingSize { get; set; }

        public Padding GripMargin { get; set; }

        public class ToolStripAccessibleObject : ControlAccessibleObject
        {
        }
    }
}
