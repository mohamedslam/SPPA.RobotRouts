using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class NumericUpDown : Control
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Increment { get; set; }
        public int Value { get; set; }
        public EventHandler ValueChanged { get; set; }
    }
}
