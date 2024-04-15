using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    //
    // Summary:
    //     Defines how a System.Windows.Forms.DataGridView column can be sorted by the user.
    public enum DataGridViewColumnSortMode
    {
        //
        // Summary:
        //     The column can only be sorted programmatically, but it is not intended for sorting,
        //     so the column header will not include space for a sorting glyph.
        NotSortable = 0,
        //
        // Summary:
        //     The user can sort the column by clicking the column header unless the column
        //     headers are used for selection. A sorting glyph will be displayed automatically.
        Automatic = 1,
        //
        // Summary:
        //     The column can only be sorted programmatically, and the column header will include
        //     space for a sorting glyph.
        Programmatic = 2
    }
}
