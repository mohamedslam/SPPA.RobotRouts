using System.Drawing;

namespace Stimulsoft.System.Windows.Forms
{
    public class Label : Control
    {
        public ContentAlignment TextAlign { get; set; }

        public bool TabStop { get; set; }
        public bool AutoEllipsis { get; set; }
    }
}
