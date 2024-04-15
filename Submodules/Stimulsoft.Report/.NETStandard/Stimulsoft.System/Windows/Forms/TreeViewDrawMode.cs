using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    //
    // Summary:
    //     Defines constants that represent the ways a System.Windows.Forms.TreeView can
    //     be drawn.
    public enum TreeViewDrawMode
    {
        //
        // Summary:
        //     The System.Windows.Forms.TreeView is drawn by the operating system.
        Normal = 0,
        //
        // Summary:
        //     The label portion of the System.Windows.Forms.TreeView nodes are drawn manually.
        //     Other node elements are drawn by the operating system, including icons, checkboxes,
        //     plus and minus signs, and lines connecting the nodes.
        OwnerDrawText = 1,
        //
        // Summary:
        //     All elements of a System.Windows.Forms.TreeView node are drawn manually, including
        //     icons, checkboxes, plus and minus signs, and lines connecting the nodes.
        OwnerDrawAll = 2
    }
}
