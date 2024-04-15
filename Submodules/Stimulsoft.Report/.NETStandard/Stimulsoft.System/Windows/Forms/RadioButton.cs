using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class RadioButton : Control
    {
        public bool Checked { get; set; }
        public EventHandler CheckedChanged { get; set; }
    }
}
