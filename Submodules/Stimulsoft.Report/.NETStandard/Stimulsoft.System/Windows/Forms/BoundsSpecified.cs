using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    //
    // Summary:
    //     Specifies the bounds of the control to use when defining a control's size and
    //     position.
    [Flags]
    public enum BoundsSpecified
    {
        //
        // Summary:
        //     No bounds are specified.
        None = 0,
        //
        // Summary:
        //     The left edge of the control is defined.
        X = 1,
        //
        // Summary:
        //     The top edge of the control is defined.
        Y = 2,
        //
        // Summary:
        //     Both X and Y coordinates of the control are defined.
        Location = 3,
        //
        // Summary:
        //     The width of the control is defined.
        Width = 4,
        //
        // Summary:
        //     The height of the control is defined.
        Height = 8,
        //
        // Summary:
        //     Both System.Windows.Forms.Control.Width and System.Windows.Forms.Control.Height
        //     property values of the control are defined.
        Size = 12,
        //
        // Summary:
        //     Both System.Windows.Forms.Control.Location and System.Windows.Forms.Control.Size
        //     property values are defined.
        All = 15
    }
}
