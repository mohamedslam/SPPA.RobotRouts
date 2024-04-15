using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class DataGridTableStyle
    {
        public Color AlternatingBackColor { get; set; }
        public Color GridLineColor { get; set; }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color HeaderBackColor { get; set; }
        public Color HeaderForeColor { get; set; }
        public Color SelectionBackColor { get; set; }
        public string MappingName { get; set; }
        public Color SelectionForeColor { get; set; }
        public List<DataGridTextBoxColumn> GridColumnStyles { get; set; }
    }
}
