using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class DataGridView : Control
    {
        public object DataSource { get; set; }

        public DataGridViewColumnCollection Columns { get; }

        public bool ColumnHeadersVisible { get; set; }

        public bool RowHeadersVisible { get; set; }

        public DataGridViewCellStyle ColumnHeadersDefaultCellStyle { get; set; }

        public int ColumnHeadersHeight { get; set; }

        public DataGridViewRow RowTemplate { get; set; }

        public DataGridViewCellStyle RowsDefaultCellStyle { get; set; }

        public DataGridViewCellStyle AlternatingRowsDefaultCellStyle { get; set; }

        public bool ReadOnly { get; set; }

        public Color BackgroundColor { get; set; }

        public string DataMember { get; set; }

        public bool Disposing { get; set; }

        public bool IsDisposed { get; set; }


        public void AutoResizeColumns(DataGridViewAutoSizeColumnsMode autoSizeColumnsMode)
        {
            throw new NotImplementedException();
        }

        public void AutoResizeColumns()
        {
            throw new NotImplementedException();
        }
    }
}
