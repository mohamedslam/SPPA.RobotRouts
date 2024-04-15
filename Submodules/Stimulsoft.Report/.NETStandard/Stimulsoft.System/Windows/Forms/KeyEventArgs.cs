using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class KeyEventArgs
    {
        public Keys KeyCode { get; set; }
        public bool Handled { get; set; }
    }
}
