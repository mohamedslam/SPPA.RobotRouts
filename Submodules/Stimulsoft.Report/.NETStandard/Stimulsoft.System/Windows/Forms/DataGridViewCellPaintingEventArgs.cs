using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#else
using System.Drawing;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class DataGridViewCellPaintingEventArgs : HandledEventArgs
    {
        public global::System.Drawing.Rectangle CellBounds { get; }

        public int ColumnIndex { get; }

        public object FormattedValue { get; }

        public Graphics Graphics { get; }

        public int RowIndex { get; }

        public void Paint(global::System.Drawing.Rectangle clipBounds, DataGridViewPaintParts paintParts)
        {
            throw new NotImplementedException();
        }
    }
}
