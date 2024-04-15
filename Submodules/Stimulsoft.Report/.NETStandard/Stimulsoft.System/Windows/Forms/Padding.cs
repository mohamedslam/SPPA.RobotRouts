using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.System.Windows.Forms
{
    public class Padding
    {
        //
        // Summary:
        //     Gets the combined padding for the right and left edges.
        //
        // Returns:
        //     Gets the sum, in pixels, of the System.Windows.Forms.Padding.Left and System.Windows.Forms.Padding.Right
        //     padding values.
        [Browsable(false)]
        public int Horizontal { get; }
        //
        // Summary:
        //     Gets or sets the padding value for the top edge.
        //
        // Returns:
        //     The padding, in pixels, for the top edge.
        [RefreshProperties(RefreshProperties.All)]
        public int Top { get; set; }
        //
        // Summary:
        //     Gets or sets the padding value for the right edge.
        //
        // Returns:
        //     The padding, in pixels, for the right edge.
        [RefreshProperties(RefreshProperties.All)]
        public int Right { get; set; }
        //
        // Summary:
        //     Gets or sets the padding value for the left edge.
        //
        // Returns:
        //     The padding, in pixels, for the left edge.
        [RefreshProperties(RefreshProperties.All)]
        public int Left { get; set; }
        //
        // Summary:
        //     Gets or sets the padding value for the bottom edge.
        //
        // Returns:
        //     The padding, in pixels, for the bottom edge.
        [RefreshProperties(RefreshProperties.All)]
        public int Bottom { get; set; }
        //
        // Summary:
        //     Gets or sets the padding value for all the edges.
        //
        // Returns:
        //     The padding, in pixels, for all edges if the same; otherwise, -1.
        [RefreshProperties(RefreshProperties.All)]
        public int All { get; set; }
        //
        // Summary:
        //     Gets the padding information in the form of a System.Drawing.Size.
        //
        // Returns:
        //     A System.Drawing.Size containing the padding information.
        [Browsable(false)]
        public Size Size { get; }
        //
        // Summary:
        //     Gets the combined padding for the top and bottom edges.
        //
        // Returns:
        //     Gets the sum, in pixels, of the System.Windows.Forms.Padding.Top and System.Windows.Forms.Padding.Bottom
        //     padding values.
        [Browsable(false)]
        public int Vertical { get; }

        public Padding(int all)
        {
        }

        public Padding(int left, int top, int right, int bottom)
        {
        }
    }
}
