using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    //
    // Summary:
    //     Defines values for specifying the parts of a System.Windows.Forms.DataGridViewCell
    //     that are to be painted.
    [Flags]
    public enum DataGridViewPaintParts
    {
        //
        // Summary:
        //     Nothing should be painted.
        None = 0,
        //
        // Summary:
        //     The background of the cell should be painted.
        Background = 1,
        //
        // Summary:
        //     The border of the cell should be painted.
        Border = 2,
        //
        // Summary:
        //     The background of the cell content should be painted.
        ContentBackground = 4,
        //
        // Summary:
        //     The foreground of the cell content should be painted.
        ContentForeground = 8,
        //
        // Summary:
        //     The cell error icon should be painted.
        ErrorIcon = 16,
        //
        // Summary:
        //     The focus rectangle should be painted around the cell.
        Focus = 32,
        //
        // Summary:
        //     The background of the cell should be painted when the cell is selected.
        SelectionBackground = 64,
        //
        // Summary:
        //     All parts of the cell should be painted.
        All = 127
    }
}
