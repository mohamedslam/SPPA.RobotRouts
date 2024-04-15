using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class PopupEventArgs : CancelEventArgs
    {
        public Size ToolTipSize { get; set; }
    }
}
