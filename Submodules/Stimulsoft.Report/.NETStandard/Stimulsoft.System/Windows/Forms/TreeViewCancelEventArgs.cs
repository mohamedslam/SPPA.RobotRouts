using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class TreeViewCancelEventArgs : CancelEventArgs
    {
        public TreeNode Node { get; }
    }
}
