using System.Drawing;

namespace Stimulsoft.System.Windows.Forms
{
    public class Screen
    {
        public static System.Windows.Forms.Screen PrimaryScreen { get; }
        public static System.Windows.Forms.Screen[] AllScreens { get; }
        public Rectangle Bounds { get; }
        public Rectangle WorkingArea { get; set; }
        public bool Primary { get; set; }
        
        public static Screen FromControl(Control control)
        {
            return new Screen();
        }
    }
}
