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

using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helper;
using Stimulsoft.Report.Components;
using System;
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
    public class StiDuplicatedReportNameCheck : StiReportCheck
    {
        #region Properties
        public override string ElementName
        {
            get
            {
                if (Element is StiDataSource)
                    return ((StiDataSource)Element).Name;

                if (Element is StiComponent)
                    return ((StiComponent)Element).Name;

                return null;
            }
        }

        public override bool PreviewVisible => Element is StiComponent || Element is StiDataSource;

        public override string ShortMessage => 
            string.Format(StiLocalizationExt.Get("CheckReport", "StiDuplicatedNameCheckShort"));

        public override string LongMessage => 
            string.Format(StiLocalizationExt.Get("CheckReport", IsDataSource ? "StiDuplicatedReportName2CheckLong" : "StiDuplicatedReportNameCheckLong"), this.ElementName);

        public override StiCheckStatus Status => StiCheckStatus.Error;

        public bool IsDataSource { get; set; }
        #endregion

        #region Fields
        private StiDataBuilder dataBuilder = new StiDataBuilder();
        #endregion

        #region Methods.CreatePreviewImage
        private TreeNode GetTreeNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == this.Element)
                    return node;

                if (!node.IsExpanded)
                    node.Expand();

                var tempNode = GetTreeNode(node.Nodes);
                if (tempNode != null)
                    return tempNode;

                node.Collapse();
            }
            return null;
        }

        public override void CreatePreviewImage(out Image elementImage, out Image highlightedElementImage, bool useScale = false)
        {
            if (!(Element is StiDataSource))
            {
                base.CreatePreviewImage(out elementImage, out highlightedElementImage, useScale);
                return;
            }

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
            using (var g = Graphics.FromImage(bmp))
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

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            if ((report.CalculationMode == StiCalculationMode.Interpretation) || string.IsNullOrWhiteSpace(report.ReportName)) return null;

            List<StiCheck> checks = null;

            StringComparison comparison = report.ScriptLanguage == StiReportLanguageType.VB ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

            foreach (StiDataSource dataSource in report.Dictionary.DataSources)
            {
                if (report.ReportName.Equals(dataSource.Name, comparison))
                {
                    StiDuplicatedReportNameCheck check = new StiDuplicatedReportNameCheck();
                    check.Element = dataSource;
                    check.IsDataSource = true;
                    //check.Actions.Add(new StiEditNameAction());
                    //check.Actions.Add(new StiGenerateNewNameDataSourceAction());
                    //check.Actions.Add(new StiDeleteDataSourceAction());

                    if (checks == null) checks = new List<StiCheck>();
                    checks.Add(check);
                }
            }

            StiComponentsCollection comps = report.GetComponents();
            foreach (StiComponent comp in comps)
            {
                if (report.ReportName.Equals(comp.Name, comparison))
                {
                    StiDuplicatedReportNameCheck check = new StiDuplicatedReportNameCheck();
                    check.Element = comp;
                    check.Actions.Add(new StiEditNameAction());
                    check.Actions.Add(new StiGenerateNewNameComponentAction());

                    if (checks == null) 
                        checks = new List<StiCheck>();

                    checks.Add(check);
                }
            }

            return checks;
        }
        #endregion
    }
}