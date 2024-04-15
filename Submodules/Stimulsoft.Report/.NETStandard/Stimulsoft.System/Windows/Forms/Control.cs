using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Font = Stimulsoft.Drawing.Font;
using Icon = Stimulsoft.Drawing.Icon;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class Control : Component, IWin32Window, IComponent, IDisposable
    {
        public static Keys ModifierKeys { get; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Text { get; set; }
        public virtual global::System.Drawing.Color ForeColor { get; set; }
        public virtual global::System.Drawing.Color BackColor { get; set; }
        public Font Font { get; set; }

        public IntPtr Handle { get; }
        public RightToLeft RightToLeft { get; set; }
        public bool Enabled { get; set; }
        public global::System.Drawing.Rectangle ClientRectangle { get; set; }
        public global::System.Drawing.Point Location { get; set; }
        public global::System.Drawing.Size Size { get; set; }
        public object Tag { get; set; }
        public EventHandler Click { get; set; }
        public EventHandler DoubleClick { get; set; }
        public EventHandler Enter { get; set; }
        public EventHandler Leave { get; set; }
        public MouseEventHandler MouseDown { get; set; }
        public MouseEventHandler MouseUp { get; set; }
        public MouseEventHandler MouseMove { get; set; }
        public EventHandler MouseEnter { get; set; }
        public EventHandler MouseLeave { get; set; }

        public event EventHandler FontChanged;

        public bool Visible { get; set; }
        public DockStyle Dock { get; set; }
        protected bool ResizeRedraw { get; set; }
        public int TabIndex { get; set; }
        public List<Control> Controls { get; set; }

        public virtual Cursor Cursor { get; set; }

        public string Name { get; set; }

        public Icon Icon { get; set; }

        public virtual global::System.Drawing.Size MinimumSize { get; set; }

        public Padding Margin { get; set; }

        public Padding Padding { get; set; }

        public virtual AnchorStyles Anchor { get; set; }

        public global::System.Drawing.SizeF AutoScaleDimensions { get; set; }

        public AutoScaleMode AutoScaleMode { get; set; }

        public event KeyEventHandler KeyDown = null;

        //protected EventHandlerList Events { get; }
        public Control Parent { get; set; }
        public bool IsHandleCreated { get; }

        protected virtual void OnHandleCreated(EventArgs e)
        {

        }

        public void SuspendLayout()
        {

        }

        public new void Dispose()
        {
            // remove warning
            if (KeyDown == null) KeyDown = null;
        }

        protected new virtual void Dispose(bool disposing)
        {

        }

        public void DrawToBitmap(Bitmap bitmapPanel, object clientRectangle)
        {
            throw new NotImplementedException();
        }

        public Form FindForm()
        {
            throw new NotImplementedException();
        }

        public void ResumeLayout(bool performLayout)
        {

        }

        public void PerformLayout()
        {

        }

        public void Invalidate()
        {

        }

        public void Invalidate(global::System.Drawing.Rectangle rc)
        {

        }

        public IAsyncResult BeginInvoke(Delegate method)
        {
            throw new NotImplementedException();
        }

        public global::System.Drawing.Point PointToClient(global::System.Drawing.Point p)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPaint(PaintEventArgs e)
        {

        }

        protected virtual void OnSystemColorsChanged(EventArgs e)
        {
        }

        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {

        }

        protected virtual void OnGotFocus(EventArgs e)
        {

        }

        protected virtual void OnLostFocus(EventArgs e)
        {

        }

        protected virtual void OnMouseEnter(EventArgs e)
        {

        }

        protected virtual void OnMouseLeave(EventArgs e)
        {

        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {

        }

        protected virtual void OnClick(EventArgs e)
        {

        }

        protected virtual void OnLayout(LayoutEventArgs levent)
        {

        }

        protected virtual void OnMouseDown(MouseEventArgs e)
        {

        }

        protected virtual void OnMouseUp(MouseEventArgs e)
        {

        }

        protected virtual void OnMouseWheel(MouseEventArgs e)
        {

        }

        protected virtual void OnSizeChanged(EventArgs e)
        {

        }

        protected void SetStyle(ControlStyles flag, bool value)
        {

        }

        protected virtual void WndProc(ref Message m)
        {

        }

        protected virtual void DefWndProc(ref Message m)
        {

        }

        public void SetBounds(int x, int y, int width, int height)
        {

        }

        public global::System.Drawing.Point PointToScreen(global::System.Drawing.Point p)
        {
            return p;
        }

        public virtual void Refresh()
        {

        }

        public bool Focus()
        {
            if (FontChanged != null) return true; // only for fix warning
            return true;
        }

        public class ControlAccessibleObject : AccessibleObject
        {
        }
    }
}
