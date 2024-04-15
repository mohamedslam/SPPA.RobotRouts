using System;
using System.Collections.Generic;
using System.Data;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class DataGrid : Control
    {
        public bool ColumnHeadersVisible { get; set; }
        public bool RowHeadersVisible { get; set; }
        public DataGridLineStyle GridLineStyle { get; set; }
        public Font HeaderFont { get; set; }
        public int PreferredColumnWidth { get; set; }
        public int PreferredRowHeight { get; set; }
        public int RowHeaderWidth { get; set; }
        public global::System.Drawing.Color AlternatingBackColor { get; set; }
        public global::System.Drawing.Color SelectionForeColor { get; set; }
        public global::System.Drawing.Color SelectionBackColor { get; set; }
        public global::System.Drawing.Color HeaderForeColor { get; set; }
        public global::System.Drawing.Color HeaderBackColor { get; set; }
        public global::System.Drawing.Color GridLineColor { get; set; }
        public global::System.Drawing.Color BackgroundColor { get; set; }
        public DataView DataSource { get; set; }
        public bool ReadOnly { get; set; }
        public bool CaptionVisible { get; set; }
        public bool AllowNavigation { get; set; }
        public string DataMember { get; set; }
        public List<DataGridTableStyle> TableStyles { get; set; }
    }
}
