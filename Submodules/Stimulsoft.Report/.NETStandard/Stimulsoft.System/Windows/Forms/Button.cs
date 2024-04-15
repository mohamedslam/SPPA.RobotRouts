using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class Button : Control
    {
        public DialogResult DialogResult { get; set; }
        public Image Image { get; set; }
        public ContentAlignment ImageAlign { get; set; }
        public ContentAlignment TextAlign { get; set; }

        public bool UseVisualStyleBackColor { get; set; }
    }
}
