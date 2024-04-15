using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class DataGridTextBoxColumn : Control
    {
        public string MappingName { get; set; }
        public HorizontalAlignment Alignment { get; set; }
        public string NullText { get; set; }
        public string HeaderText { get; set; }
    }
}
