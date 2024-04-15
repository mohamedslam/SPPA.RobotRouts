using System;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class DataGridViewCellStyle : ICloneable
    {
        public DataGridViewContentAlignment Alignment { get; set; }

        public DataGridViewTriState WrapMode { get; set; }

        public Font Font { get; set; }

        public global::System.Drawing.Color BackColor { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
