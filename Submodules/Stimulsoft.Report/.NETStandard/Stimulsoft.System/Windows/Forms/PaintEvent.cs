using System;
using System.Diagnostics;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsState = Stimulsoft.Drawing.Drawing2D.GraphicsState;
#else
using System.Drawing;
using System.Drawing.Drawing2D;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class PaintEventArgs : EventArgs, IDisposable
    {
        private Graphics graphics = null;

        // See ResetGraphics()
        private GraphicsState savedGraphicsState = null;

        private readonly IntPtr dc = IntPtr.Zero;
        IntPtr oldPal = IntPtr.Zero;

        private readonly global::System.Drawing.Rectangle clipRect;
        //private Control paletteSource;

        public PaintEventArgs(Graphics graphics, global::System.Drawing.Rectangle clipRect)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            this.graphics = graphics;
            this.clipRect = clipRect;
        }

        // Internal version of constructor for performance
        // We try to avoid getting the graphics object until needed
        internal PaintEventArgs(IntPtr dc, global::System.Drawing.Rectangle clipRect)
        {
            Debug.Assert(dc != IntPtr.Zero, "dc is not initialized.");

            this.dc = dc;
            this.clipRect = clipRect;
        }

        ~PaintEventArgs()
        {
            Dispose(false);
        }

        public global::System.Drawing.Rectangle ClipRectangle
        {
            get
            {
                return clipRect;
            }
        }

        internal IntPtr HDC
        {
            get
            {
                if (graphics == null)
                    return dc;
                else
                    return IntPtr.Zero;
            }
        }

        public Graphics Graphics
        {
            get
            {
                //if (graphics == null && dc != IntPtr.Zero) {
                //oldPal = Control.SetUpPalette(dc, false /*force*/, false /*realize*/);
                //graphics = Graphics.FromHdcInternal(dc);
                //graphics.PageUnit = GraphicsUnit.Pixel;
                //savedGraphicsState = graphics.Save(); // See ResetGraphics() below
                //}
                return graphics;
            }
        }

        // We want a way to dispose the GDI+ Graphics, but we don't want to create one
        // simply to dispose it
        // cpr: should be internal
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <include file='doc\PaintEvent.uex' path='docs/doc[@for="PaintEventArgs.Dispose2"]/*' />
        protected virtual void Dispose(bool disposing)
        {
            /*if (disposing) {
                 //only dispose the graphics object if we created it via the dc.
                 if (graphics != null && dc != IntPtr.Zero) {
                     graphics.Dispose();
                 }
             }

             if (oldPal != IntPtr.Zero && dc != IntPtr.Zero) {
                 SafeNativeMethods.SelectPalette(new HandleRef(this, dc), new HandleRef(this, oldPal), 0);
                 oldPal = IntPtr.Zero;
             }*/
        }

        // If ControlStyles.AllPaintingInWmPaint, we call this method
        // after OnPaintBackground so it appears to OnPaint that it's getting a fresh
        // Graphics.  We want to make sure AllPaintingInWmPaint is purely an optimization,
        // and doesn't change behavior, so we need to make sure any clipping regions established
        // in OnPaintBackground don't apply to OnPaint. See ASURT 44682.
        internal void ResetGraphics()
        {
            if (graphics != null)
            {
                Debug.Assert(dc == IntPtr.Zero || savedGraphicsState != null, "Called ResetGraphics more than once?");
                if (savedGraphicsState != null)
                {
                    graphics.Restore(savedGraphicsState);
                    savedGraphicsState = null;
                }
            }
        }
    }
}
