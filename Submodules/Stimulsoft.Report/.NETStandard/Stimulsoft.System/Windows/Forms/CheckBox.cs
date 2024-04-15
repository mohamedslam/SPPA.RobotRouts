using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class CheckBox : Control
    {
        public bool Checked { get; set; }
        public EventHandler CheckedChanged { get; set; }
    }
}
