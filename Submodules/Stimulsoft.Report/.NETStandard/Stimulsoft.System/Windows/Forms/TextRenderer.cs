using System;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
#else
using System.Drawing;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class TextRenderer
    {
        public static void DrawText(Graphics g, string text, Font font, global::System.Drawing.Rectangle gdiTextRect, global::System.Drawing.Color color, TextFormatFlags flags)
        {
            throw new NotImplementedException();
        }

        public static global::System.Drawing.Size MeasureText(string text, Font font)
        {
            throw new NotImplementedException();
        }
    }
}
