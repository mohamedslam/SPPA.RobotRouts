using System;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class RichTextBox : Control, IDisposable
    {
        public string Rtf { get; set; }

        public Font SelectionFont { get; set; }

        public global::System.Drawing.Color SelectionColor { get; set; }

        public bool WordWrap { get; set; }

        public int TextLength { get; set; }

        public HorizontalAlignment SelectionAlignment { get; set; }

        public bool SelectionBullet { get; set; }

        public int SelectionCharOffset { get; set; }

        public int SelectionHangingIndent { get; set; }

        public int SelectionIndent { get; set; }

        public bool SelectionProtected { get; set; }

        public int SelectionRightIndent { get; set; }

        public int[] SelectionTabs { get; set; }

        public bool DetectUrls { get; set; }

        protected virtual CreateParams CreateParams { get; }

        public int SelectionStart { get; set; }

        public virtual int SelectionLength { get; set; }

        public string SelectedRtf { get; set; }

        public string SelectedText { get; set; }

        protected virtual void OnHandleCreated(EventArgs e)
        {

        }

        public void Select(int start, int length)
        {

        }

        public void SelectAll()
        {
            
        }

        public void Cut()
        {

        }

        public void Paste()
        {

        }

        public RichTextBox()
        {
            Text = string.Empty;
        }

    }
}
