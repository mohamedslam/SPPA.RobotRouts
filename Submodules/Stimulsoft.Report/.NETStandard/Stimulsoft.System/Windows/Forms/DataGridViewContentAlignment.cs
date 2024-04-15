using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    //
    // Summary:
    //     Defines constants that indicate the alignment of content within a System.Windows.Forms.DataGridView
    //     cell.
    public enum DataGridViewContentAlignment
    {
        //
        // Summary:
        //     The alignment is not set.
        NotSet = 0,
        //
        // Summary:
        //     The content is aligned vertically at the top and horizontally at the left of
        //     a cell.
        TopLeft = 1,
        //
        // Summary:
        //     The content is aligned vertically at the top and horizontally at the center of
        //     a cell.
        TopCenter = 2,
        //
        // Summary:
        //     The content is aligned vertically at the top and horizontally at the right of
        //     a cell.
        TopRight = 4,
        //
        // Summary:
        //     The content is aligned vertically at the middle and horizontally at the left
        //     of a cell.
        MiddleLeft = 16,
        //
        // Summary:
        //     The content is aligned at the vertical and horizontal center of a cell.
        MiddleCenter = 32,
        //
        // Summary:
        //     The content is aligned vertically at the middle and horizontally at the right
        //     of a cell.
        MiddleRight = 64,
        //
        // Summary:
        //     The content is aligned vertically at the bottom and horizontally at the left
        //     of a cell.
        BottomLeft = 256,
        //
        // Summary:
        //     The content is aligned vertically at the bottom and horizontally at the center
        //     of a cell.
        BottomCenter = 512,
        //
        // Summary:
        //     The content is aligned vertically at the bottom and horizontally at the right
        //     of a cell.
        BottomRight = 1024
    }
}
