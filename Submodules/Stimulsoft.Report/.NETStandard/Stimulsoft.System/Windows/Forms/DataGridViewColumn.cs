using System;
using System.ComponentModel;

namespace Stimulsoft.System.Windows.Forms
{
    public class DataGridViewColumn : DataGridViewBand, IComponent, IDisposable
    {
        public ISite Site { get; set; }

        public event EventHandler Disposed;

        public DataGridViewColumnSortMode SortMode { get; set; }

        public string HeaderText { get; set; }

        public int MinimumWidth { get; set; }

        public int Width { get; set; }

        public string Name { get; set; }

        public override DataGridViewCellStyle DefaultCellStyle { get; set; }

        public string DataPropertyName { get; set; }

        public bool Frozen { get; set; }

        public object Tag { get; set; }

        public override void Dispose()
        {
            base.Dispose();
            Disposed?.Invoke(this, new EventArgs());
        }
    }
}
