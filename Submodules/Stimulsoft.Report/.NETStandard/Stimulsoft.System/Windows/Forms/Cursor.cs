using System.Drawing;
using System.IO;

namespace Stimulsoft.System.Windows.Forms
{
    public class Cursor
    {
        private Stream stream;

        public Cursor(Stream stream)
        {
            this.stream = stream;
        }

        public static global::System.Drawing.Point Position { get; set; }
    }
}
