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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiLocationOutsidePageCheck : StiComponentCheck
    {
        #region Properties
        public override bool PreviewVisible
        {
            get
            {
                return true;
            }
        }

        public override string ShortMessage
        {
            get
            {
                if (IsOutsidePage)
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiLocationOutsidePageCheckShort"), this.ElementName);
                else
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiLocationOutsidePrintableAreaCheckShort"), this.ElementName);
            }
        }

        public override string LongMessage
        {
            get
            {
                if (Element != null)
                {
                    if (IsOutsidePage)
                        return string.Format(StiLocalizationExt.Get("CheckComponent", "StiLocationOutsidePageCheckLong"), this.ElementName);
                    else
                        return string.Format(StiLocalizationExt.Get("CheckComponent", "StiLocationOutsidePrintableAreaCheckLong"), this.ElementName);
                }
                else
                {
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiLocationOutsidePageCheckLong"), this.ElementName);
                }
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Warning;
            }
        }

        public bool IsOutsidePage
        {
            get
            {
                StiComponent comp = Element as StiComponent;
                StiPage page = comp.Page;
                if (page == null) return false;

                RectangleD compRect = comp.GetPaintRectangle(false, false);
                if (comp is StiHorizontalLinePrimitive) compRect.Height = 0;
                if (comp is StiVerticalLinePrimitive) compRect.Width = 0;

                double pageHeight = (page.PageHeight - page.Margins.Top - page.Margins.Bottom) * page.SegmentPerHeight;
                pageHeight *= page.LargeHeightAutoFactor;
                RectangleD pageRect = new RectangleD(
                    -page.Margins.Left,
                    -page.Margins.Top,
                    page.Width + page.Margins.Left + page.Margins.Right,
                    pageHeight + page.Margins.Top + page.Margins.Bottom);

                if (page.IsDashboard)
                {
                    pageRect = new RectangleD(
                    -page.Margins.Left,
                    -page.Margins.Top,
                    page.ClientRectangle.Width + page.Margins.Left + page.Margins.Right,
                    page.ClientRectangle.Height + page.Margins.Top + page.Margins.Bottom);
                }

                if (((decimal)compRect.Left < (decimal)pageRect.Left) ||
                    ((decimal)compRect.Top < (decimal)pageRect.Top) ||
                    ((decimal)compRect.Right > (decimal)pageRect.Right))
                {
                    return true;
                }
                if ((decimal)compRect.Bottom > (decimal)pageRect.Bottom)
                {
                    if (comp is StiBand) return false;
                    if ((comp.Parent != comp.Page) && (comp.Bottom < pageHeight)) return false;
                    return true;
                }
                return false;
            }
        }

        public bool IsOutsidePrintableArea
        {
            get
            {
                StiComponent comp = Element as StiComponent;
                StiPage page = comp.Page;
                if (page == null) return false;

                RectangleD compRect = comp.GetPaintRectangle(false, false);
                if (comp is StiHorizontalLinePrimitive) compRect.Height = 0;
                if (comp is StiVerticalLinePrimitive) compRect.Width = 0;

                double pageHeight = (page.PageHeight - page.Margins.Top - page.Margins.Bottom) * page.SegmentPerHeight;
                pageHeight *= page.LargeHeightAutoFactor;
                RectangleD pageRect = new RectangleD(0, 0, page.Width, pageHeight);

                if (page.IsDashboard)
                {
                    pageRect = new RectangleD(0, 0, page.ClientRectangle.Width, page.ClientRectangle.Height);
                }

                if ((decimal)compRect.Left < (decimal)pageRect.Left ||
                    (decimal)compRect.Top < (decimal)pageRect.Top ||
                    (decimal)compRect.Right > (decimal)(pageRect.Right * 1.01))
                {
                    return true;
                }
                if ((decimal)compRect.Bottom > (decimal)(pageRect.Bottom * 1.01))
                {
                    if (comp is StiBand) return false;
                    if ((comp.Parent != comp.Page) && (comp.Bottom < pageHeight)) return false;
                    return true;
                }
                return false;
            }
        }

        private bool NotAllowToDelete
        {
            get
            {
                return Element is StiComponent && !(Element as StiComponent).AllowDelete;
            }
        }
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            try
            {

                bool failed = IsOutsidePage || IsOutsidePrintableArea;

                if (failed)
                {
                    StiLocationOutsidePageCheck check = new StiLocationOutsidePageCheck();
                    check.Element = obj;
                    if (IsOutsidePage) check.Actions.Add(new StiMoveComponentToPageAreaAction());
                    check.Actions.Add(new StiMoveComponentToPrintablePageAreaAction());
                    if (!NotAllowToDelete) check.Actions.Add(new StiDeleteComponentAction());
                    return check;
                }
                else return null;
            }
            finally
            {
                this.Element = null;
            }
        }
        #endregion
    }
}