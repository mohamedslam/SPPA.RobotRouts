#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Report.Dictionary;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
#endif

namespace Stimulsoft.Report.Check
{
    public abstract class StiDataSourceCheck : StiCheck
    {
        #region Fields
        private StiDataBuilder dataBuilder = new StiDataBuilder();
        #endregion

        #region Properties
        public override StiCheckObjectType ObjectType
        {
            get
            {
                return StiCheckObjectType.DataSource;
            }
        }

        public override string ElementName
        {
            get
            {
                if (Element == null)
                    return null;
                return ((StiDataSource)Element).Name;
            }
        }
        #endregion

        #region Methods
        private TreeNode GetTreeNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == this.Element)
                    return node;

                if (!node.IsExpanded) node.Expand();
                TreeNode tempNode = GetTreeNode(node.Nodes);
                if (tempNode != null)
                    return tempNode;

                node.Collapse();
            }
            return null;
        }

        public override void CreatePreviewImage(out Image elementImage, out Image highlightedElementImage, bool useScale = false)
        {
            #region Prepare element image
            int viewWidth = 300;
            int viewHeight = 300;

            if (useScale)
            {
                viewWidth *= 2;
                viewHeight *= 2;
            }

            var dataSource = this.Element as StiDataSource;

            var treeView = new TreeView();
            treeView.BeforeExpand += TreeNode_Expand;
            treeView.BeforeCollapse += TreeNode_Collapsed;
            treeView.Scrollable = false;
            treeView.ImageList = StiDataImages.Images;
            treeView.Width = viewWidth - 8;
            treeView.Height = viewHeight - 8;
            treeView.BorderStyle = BorderStyle.None;

            dataBuilder.Build(treeView.Nodes, dataSource.Dictionary, true, true);

            var treeNodeBounds = Rectangle.Empty;
            var dataNode = GetTreeNode(treeView.Nodes);
            if (dataNode != null)
            {
                dataNode.EnsureVisible();

                treeNodeBounds = dataNode.Bounds;
                treeNodeBounds.X += 4;
                treeNodeBounds.Y += 4;
            }

            var bmp = new Bitmap(viewWidth, viewHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                treeView.DrawToBitmap(bmp, new Rectangle(4, 4, treeView.Width, treeView.Height));
                g.DrawRectangle(Pens.Gray, new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1));
            }

            elementImage = bmp;
            #endregion

            #region Prepare highlighted element image
            highlightedElementImage = null;
            if (treeNodeBounds.IsEmpty) return;

            Image image = bmp.Clone() as Image;
            using (Graphics g = Graphics.FromImage(image))
            using (Pen pen = new Pen(Color.FromArgb(0x77ff0000)))
            {
                pen.Width = 3;
                g.DrawRectangle(pen, treeNodeBounds);
            }

            highlightedElementImage = image;
            #endregion

        }
        #endregion

        #region Handlers
        private void TreeNode_Expand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNodeEx nodeEx = e.Node as TreeNodeEx;
            if (nodeEx != null && e.Node.Tag != null)
            {
                dataBuilder.ExpandedNode(nodeEx, true);
            }
        }

        private void TreeNode_Collapsed(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is TreeNodeEx && e.Node.Tag != null)
            {
                dataBuilder.CollapsedNode(e.Node);
            }
        }
        #endregion
    }
}