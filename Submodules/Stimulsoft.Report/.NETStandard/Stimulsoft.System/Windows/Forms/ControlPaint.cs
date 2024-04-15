using System;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class ControlPaint
    {
        public static void DrawBorder3D(Graphics graphics, global::System.Drawing.Rectangle rectangle, Border3DStyle style)
        {

        }

        public static void DrawImageDisabled(Graphics graphics, Image image, int x, int y, global::System.Drawing.Color control)
        {
            throw new NotImplementedException();
        }

        public static void DrawButton(Graphics graphics, global::System.Drawing.Rectangle bounds, ButtonState state)
        {
            throw new NotImplementedException();
        }

        public static void DrawFocusRectangle(Graphics graphics, global::System.Drawing.Rectangle focusedRect)
        {
            throw new NotImplementedException();
        }

        public static void DrawBorder3D(Graphics g, global::System.Drawing.Rectangle rect, Border3DStyle raised, Border3DSide all)
        {
            throw new NotImplementedException();
        }

        public static void DrawStringDisabled(Graphics g, string text, Font font, global::System.Drawing.Color controlLight, global::System.Drawing.Rectangle rectangle, StringFormat sf)
        {
            throw new NotImplementedException();
        }
    }
}
