using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class ToolTip : Control
    {
        public bool OwnerDraw { get; set; }

#pragma warning disable 0067
        public event PopupEventHandler Popup;
        public event DrawToolTipEventHandler Draw;
#pragma warning restore 0067

        public void SetToolTip(Control control, string v)
        {
            throw new NotImplementedException();
        }

        public string GetToolTip(Control control)
        {
            throw new NotImplementedException();
        }

        public void Hide(IWin32Window win)
        {
            throw new NotImplementedException();
        }
    }
}
