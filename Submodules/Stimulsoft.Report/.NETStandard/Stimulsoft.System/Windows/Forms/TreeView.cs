using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace Stimulsoft.System.Windows.Forms
{
    public class TreeView : Control
    {
        public BorderStyle BorderStyle { get; set; }

        internal TreeNode root = null;

        public ImageList StateImageList { get; set; }

        public ImageList ImageList { get; set; }

        public bool Scrollable { get; set; }

        public TreeViewEventHandler AfterSelect { get; set; }

        public TreeNode SelectedNode { get; set; }

        public TreeNodeCollection Nodes { get; set; }

        public TreeViewDrawMode DrawMode { get; set; }

        public TreeNode GetNodeAt(global::System.Drawing.Point pt)
        {
            // remove warning
            if (BeforeCollapse == null) BeforeCollapse = null;
            if (BeforeExpand == null) BeforeExpand = null;
            if (DrawNode == null) DrawNode = null;

            throw new NotImplementedException();
        }

        public event TreeViewCancelEventHandler BeforeCollapse;

        public event TreeViewCancelEventHandler BeforeExpand;

        public event DrawTreeNodeEventHandler DrawNode;
    }
}
