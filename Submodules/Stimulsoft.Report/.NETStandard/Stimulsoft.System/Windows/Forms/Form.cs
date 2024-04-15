using System;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.System.Windows.Forms
{
    public class Form : Control, IWin32Window
    {
        public Button AcceptButton { get; set; }
        public Button CancelButton { get; set; }
        public bool MaximizeBox { get; set; }
        public bool MinimizeBox { get; set; }
        public bool ShowInTaskbar { get; set; }
        public EventHandler Closed { get; set; }
        public CancelEventHandler Closing { get; set; }
        public FormBorderStyle FormBorderStyle { get; set; }
        public FormWindowState WindowState { get; set; }
        public FormStartPosition StartPosition { get; set; }
        public DialogResult DialogResult { get; set; }
        public Control ActiveControl { get; set; }
        public static Form ActiveForm { get; set; }
        public Control Owner { get; set; }

        public Size ClientSize { get; set; }

        public bool ControlBox { get; set; }

        public bool KeyPreview { get; set; }

        public bool ShowIcon { get; set; }

        public SizeGripStyle SizeGripStyle { get; set; }

        public BindingContext BindingContext { get; set; }

        public bool TopMost { get; set; }

        public Form MdiParent { get; set; }

        public Rectangle RestoreBounds { get; }

        public bool IsMdiContainer { get; set; }

        public Size MaximumSize { get; set; }

        public AutoSizeMode AutoSizeMode { get; set; }

        protected virtual void OnLoad(EventArgs e)
        {

        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }

        public void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
        {

        }

        protected void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {

        }

        public void Activate()
        {

        }
    }
}
