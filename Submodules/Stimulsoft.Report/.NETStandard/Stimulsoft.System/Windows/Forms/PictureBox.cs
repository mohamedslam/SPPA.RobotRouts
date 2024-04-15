#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#else
using System.Drawing;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class PictureBox : Control
    {
        public PictureBoxSizeMode SizeMode { get; set; }
        public BorderStyle BorderStyle { get; set; }
        public Image Image { get; set; }

        public bool ReadOnly { get; set; }

        public bool TabStop { get; set; }
    }
}
