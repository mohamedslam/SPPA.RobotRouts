using System;
using System.Diagnostics;
using System.Drawing;

namespace Stimulsoft.System.Windows.Forms
{
    public class TreeNode : MarshalByRefObject, ICloneable
    {
        internal string text;

        private TreeNodeImageIndexer imageIndexer;
        private TreeNodeImageIndexer selectedImageIndexer;
        private TreeNodeImageIndexer stateImageIndexer;

        internal class TreeNodeImageIndexer : ImageList.Indexer
        {
            private TreeNode owner;

            public enum ImageListType
            {
                Default,
                State
            }

            private ImageListType imageListType;

            public TreeNodeImageIndexer(TreeNode node, ImageListType imageListType)
            {
                owner = node;
                this.imageListType = imageListType;
            }

            public override ImageList ImageList
            {
                get
                {
                    if (owner.TreeView != null)
                    {
                        if (imageListType == ImageListType.State)
                        {
                            return owner.TreeView.StateImageList;
                        }
                        else
                        {
                            return owner.TreeView.ImageList;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                set { Debug.Assert(false, "We should never set the image list"); }
            }

        }

        internal TreeNodeImageIndexer ImageIndexer
        {
            get
            {
                if (imageIndexer == null)
                {
                    imageIndexer = new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.Default);
                }
                return imageIndexer;
            }
        }

        internal TreeNodeImageIndexer SelectedImageIndexer
        {
            get
            {
                if (selectedImageIndexer == null)
                {
                    selectedImageIndexer = new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.Default);
                }

                return selectedImageIndexer;

            }
        }

        internal TreeNodeImageIndexer StateImageIndexer
        {
            get
            {
                if (stateImageIndexer == null)
                {
                    stateImageIndexer = new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.State);
                }
                return stateImageIndexer;
            }
        }


        internal int childCount = 0;
        internal TreeNode[] children;
        internal TreeNode parent = null;
        internal TreeView treeView;
        private TreeNodeCollection nodes = null;
        object userData;

        public TreeNode()
        {
        }

        internal TreeNode(TreeView treeView) : this()
        {
            this.treeView = treeView;
        }

        public TreeNode(string text) : this()
        {
            this.text = text;
        }

        public void EnsureVisible()
        {
            throw new NotImplementedException();
        }

        public TreeNode(string text, TreeNode[] children) : this()
        {
            this.text = text;
            this.Nodes.AddRange(children);
        }

        public void Expand()
        {
            throw new NotImplementedException();
        }

        public TreeNode(string text, int imageIndex, int selectedImageIndex) : this()
        {
            this.text = text;
            this.ImageIndexer.Index = imageIndex;
            this.SelectedImageIndexer.Index = selectedImageIndex;
        }

        public TreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children) : this()
        {
            this.text = text;
            this.ImageIndexer.Index = imageIndex;
            this.SelectedImageIndexer.Index = selectedImageIndex;
            this.Nodes.AddRange(children);
        }

        public int ImageIndex
        {
            get { return ImageIndexer.Index; }
            set
            {
                ImageIndexer.Index = value;
            }
        }

        public string ImageKey
        {
            get { return ImageIndexer.Key; }
            set
            {
                ImageIndexer.Key = value;
            }
        }

        public TreeNodeCollection Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new TreeNodeCollection(this);
                }
                return nodes;
            }
        }

        public TreeNode Parent
        {
            get
            {
                TreeView tv = TreeView;

                // Don't expose the virtual root publicly
                if (tv != null && parent == tv.root)
                {
                    return null;
                }

                return parent;
            }
        }


        public int SelectedImageIndex
        {
            get
            {
                return SelectedImageIndexer.Index;
            }
            set
            {
                SelectedImageIndexer.Index = value;
            }
        }

        public string SelectedImageKey
        {
            get
            {
                return SelectedImageIndexer.Key;
            }
            set
            {
                SelectedImageIndexer.Key = value;
            }
        }

        public object Tag
        {
            get
            {
                return userData;
            }
            set
            {
                userData = value;
            }
        }

        public string Text
        {
            get
            {
                return text == null ? "" : text;
            }
            set
            {
                this.text = value;
            }
        }

        public TreeView TreeView
        {
            get
            {
                if (treeView == null)
                    treeView = FindTreeView();
                return treeView;
            }
        }


        public virtual object Clone()
        {
            Type clonedType = this.GetType();
            TreeNode node = null;

            if (clonedType == typeof(TreeNode))
            {
                node = new TreeNode(text, ImageIndexer.Index, SelectedImageIndexer.Index);
            }
            else
            {
                node = (TreeNode)Activator.CreateInstance(clonedType);
            }

            node.Text = text;
            node.ImageIndexer.Index = ImageIndexer.Index;
            node.SelectedImageIndexer.Index = SelectedImageIndexer.Index;

            node.StateImageIndexer.Index = StateImageIndexer.Index;

            // only set the key if it's set to something useful
            if (!(string.IsNullOrEmpty(ImageIndexer.Key)))
            {
                node.ImageIndexer.Key = ImageIndexer.Key;
            }

            // only set the key if it's set to something useful
            if (!(string.IsNullOrEmpty(SelectedImageIndexer.Key)))
            {
                node.SelectedImageIndexer.Key = SelectedImageIndexer.Key;
            }

            // only set the key if it's set to something useful
            if (!(string.IsNullOrEmpty(StateImageIndexer.Key)))
            {
                node.StateImageIndexer.Key = StateImageIndexer.Key;
            }

            if (childCount > 0)
            {
                node.children = new TreeNode[childCount];
                for (int i = 0; i < childCount; i++)
                    node.Nodes.Add((TreeNode)children[i].Clone());
            }

            // Clone properties
            //

            node.Tag = this.Tag;

            return node;
        }

        internal TreeView FindTreeView()
        {
            TreeNode node = this;
            while (node.parent != null)
                node = node.parent;
            return node.treeView;
        }


        public override string ToString()
        {
            return "TreeNode: " + (text == null ? "" : text);
        }

        public static TreeNode FromHandle(TreeView tree, IntPtr handle)
        {
            throw new NotImplementedException();
        }

        public Rectangle Bounds { get; }

        public IntPtr Handle { get; }
        public bool IsExpanded { get; set; }

        public void Collapse()
        {
            throw new NotImplementedException();
        }
    }
}