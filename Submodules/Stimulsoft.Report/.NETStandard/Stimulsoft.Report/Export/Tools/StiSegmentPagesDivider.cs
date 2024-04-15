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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using System.Collections.Generic;

namespace Stimulsoft.Report.Export
{
    public sealed class StiSegmentPagesDivider
    {
        public static StiPagesCollection Divide(StiPagesCollection pages)
        {
            return Divide(pages, null);
        }
         
        public static StiPagesCollection Divide(StiPagesCollection pages, StiExportService service)
        {
            var finded = false;
            foreach (StiPage page in pages)
            {
                if (page.SegmentPerWidth > 1 || page.SegmentPerHeight > 1)
                {
                    finded = true;
                    break;
                }
            }
            if (!finded)
                return pages;

            if (service != null)
                service.StatusString = StiLocalization.Get("Report", "PreparingReport");

            var newPages = new StiPagesCollection(pages.Report, pages);
            newPages.CacheMode = pages.CacheMode;

            if (newPages.CacheMode)
                newPages.CanUseCacheMode = true;

            foreach (StiPage page in pages)
            {
                pages.GetPage(page);    //fix 2012.05.31

                if (service != null)
                    service.InvokeExporting(page, pages, service.CurrentPassNumber, service.MaximumPassNumber);

                if (page.SegmentPerWidth > 1 || page.SegmentPerHeight > 1)
                {
                    #region Prepare ExceedMargins arrays
                    var ems = new List<StiComponent>[page.SegmentPerHeight, page.SegmentPerWidth];
                    for (var y = 0; y < page.SegmentPerHeight; y++)
                    {
                        for (var x = 0; x < page.SegmentPerWidth; x++)
                        {
                            ems[y, x] = new List<StiComponent>();
                        }
                    }
                    #endregion

                    //prepare list of components by segments
                    var segments = new List<int>[page.SegmentPerHeight];
                    for (int index = 0; index < page.SegmentPerHeight; index++)
                    {
                        segments[index] = new List<int>();
                    }
                    decimal pageWidth = (decimal)(page.PageWidth - page.Margins.Left - page.Margins.Right);
                    decimal pageHeight = (decimal)(page.PageHeight - page.Margins.Top - page.Margins.Bottom);
                    for (int index = 0; index < page.Components.Count; index++)
                    {
                        StiComponent comp = page.Components[index];
                        if (!comp.Enabled) continue;
                        int index1 = (int)((decimal)comp.Top / pageHeight);
                        int index2 = (int)((decimal)comp.Bottom / pageHeight);
                        if (index2 > segments.Length - 1) index2 = segments.Length - 1;
                        for (int indexS = index1; indexS <= index2; indexS++)
                        {
                            segments[indexS].Add(index);
                        }

                        #region Check for ExceedMargins
                        var stiText = comp as StiText;
                        if ((stiText == null) || (stiText.ExceedMargins == StiExceedMargins.None)) continue;
                        int x = (int)((decimal)comp.Left / pageWidth);
                        int x1 = x;
                        int x2 = x;
                        int y1 = index1;
                        int y2 = index1;
                        if ((stiText.ExceedMargins & StiExceedMargins.Left) > 0) x1 = 0;
                        if ((stiText.ExceedMargins & StiExceedMargins.Right) > 0) x2 = page.SegmentPerWidth - 1;
                        if ((stiText.ExceedMargins & StiExceedMargins.Top) > 0) y1 = 0;
                        if ((stiText.ExceedMargins & StiExceedMargins.Bottom) > 0) y2 = page.SegmentPerHeight - 1;
                        if (x1 != x2 || y1 != y2)
                        {
                            for (int yy = y1; yy <= y2; yy++)
                            {
                                for (int xx = x1; xx <= x2; xx++)
                                {
                                    if (yy == index1 && xx == x) continue;

                                    StiText txt2 = stiText.Clone() as StiText;
                                    txt2.Text = string.Empty;
                                    txt2.Border.Side = StiBorderSides.None;
                                    while ((decimal)txt2.Left > pageWidth) txt2.Left -= (double)pageWidth;
                                    while ((decimal)txt2.Top > pageHeight) txt2.Top -= (double)pageHeight;

                                    if (xx != x) txt2.ExceedMargins |= StiExceedMargins.Left | StiExceedMargins.Right;
                                    if (yy != index1) txt2.ExceedMargins |= StiExceedMargins.Top | StiExceedMargins.Bottom;

                                    ems[yy, xx].Add(txt2);
                                }
                            }
                        }
                        #endregion
                    }

                    var comps = new StiComponentsCollection();
                    comps.AddRange(page.Components);

                    for (var y = 0; y < page.SegmentPerHeight; y++)
                    {
                        for (var x = 0; x < page.SegmentPerWidth; x++)
                        {
                            var newPage = page.Clone(false, false) as StiPage;
                            newPage.CacheGuid = StiGuidUtils.NewGuid();
                            newPage.SegmentPerWidth = 1;
                            newPage.SegmentPerHeight = 1;

                            if (service is StiPdfExportService)
                            {
                                StiBorderSides sides = StiBorderSides.None;
                                if (x > 0) sides |= StiBorderSides.Left;
                                if (x < page.SegmentPerWidth - 1) sides |= StiBorderSides.Right;
                                if (y > 0) sides |= StiBorderSides.Top;
                                if (y < page.SegmentPerHeight - 1) sides |= StiBorderSides.Bottom;
                                newPage.TagValue = $"Segments:{(int)sides}";
                            }

                            var rectM = new RectangleM(
                                (decimal)(x * newPage.Width),
                                (decimal)(y * newPage.Height),
                                (decimal)newPage.Width,
                                (decimal)newPage.Height);

                            var rect = rectM.ToRectangleD();

                            newPage.Components.AddRange(ems[y, x].ToArray());

                            var listCompIndexes = segments[y];
                            foreach (int index in listCompIndexes)
                            {
                                StiComponent comp = comps[index];
                                if (comp.Enabled)
                                {
                                    //left corner fit to the page
                                    if (rectM.Left <= (decimal)comp.Left && (decimal)comp.Left < rectM.Right && rectM.Top <= (decimal)comp.Top && (decimal)comp.Top < rectM.Bottom)
                                    {
                                        var needClip = comp.Right > (x + 1.5) * newPage.Width;

                                        // earlier there was a check - the components from the zero segment were not cloned.
                                        // but sometimes this causes a problem, because the page of the component changes, and it is not assigned back.
                                        // so now the components are always cloned
                                        //
                                        //if (x == 0 && y == 0 && !needClip)
                                        //    newPage.Components.Add(comp);

                                        //else
                                        //{
                                            var newComp = comp.Clone() as StiComponent;
                                            newComp.Left -= rect.Left;
                                            newComp.Top -= rect.Top;

                                            if (needClip)
                                                newComp.Width = newPage.Width * 1.5 - newComp.Left;

                                            newPage.Components.Add(newComp);
                                        //}

                                        continue;
                                    }

                                    //right corner fit to the page
                                    if ((decimal)comp.Left < rectM.Right && (decimal)comp.Right > rectM.Left && (decimal)comp.Top < rectM.Bottom && (decimal)comp.Bottom > rectM.Top)
                                    {
                                        var needClip = !(service is StiPdfExportService);

                                        #region Make new component
                                        StiComponent newComp = null;
                                        if (comp is StiContainer) newComp = comp.Clone() as StiContainer;
                                        if (comp is StiText)
                                        {
                                            var txtComp = comp.Clone() as StiText;
                                            if (needClip)
                                                txtComp.Text = string.Empty;

                                            newComp = txtComp;
                                        }
                                        if (comp is StiImage)
                                        {
                                            var cont = new StiContainer
                                            {
                                                Border = (comp as StiImage).Border,
                                                Brush = (comp as StiImage).Brush
                                            };
                                            newComp = cont;
                                        }
                                        if (comp is StiCrossLinePrimitive || comp is StiHorizontalLinePrimitive)
                                            newComp = comp.Clone() as StiComponent;
                                        #endregion

                                        if (newComp != null)
                                        {
                                            var border = new StiBorder();
                                            if (newComp is IStiBorder)
                                                border = ((IStiBorder)newComp).Border;

                                            #region Clipping
                                            if ((decimal)comp.Left < rectM.Left && needClip)
                                            {
                                                newComp.Left = 0;
                                                border.Side &= StiBorderSides.All ^ StiBorderSides.Left;
                                            }
                                            else
                                                newComp.Left -= rect.Left;

                                            if ((decimal)comp.Right > rectM.Right && needClip)
                                            {
                                                newComp.Width = newPage.Width - newComp.Left;
                                                border.Side &= StiBorderSides.All ^ StiBorderSides.Right;
                                            }
                                            else
                                                newComp.Width = comp.Right - (rect.Left + newComp.Left);

                                            if ((decimal)comp.Top < rectM.Top && needClip)
                                            {
                                                newComp.Top = 0;
                                                border.Side &= StiBorderSides.All ^ StiBorderSides.Top;
                                            }
                                            else
                                                newComp.Top -= rect.Top;

                                            if ((decimal)comp.Bottom > rectM.Bottom && needClip)
                                            {
                                                newComp.Height = newPage.Height - newComp.Top;
                                                border.Side &= StiBorderSides.All ^ StiBorderSides.Bottom;
                                            }
                                            else
                                                newComp.Height = comp.Bottom - (rect.Top + newComp.Top);
                                            #endregion

                                            newPage.Components.Add(newComp);
                                        }
                                    }
                                }
                            }

                            newPages.AddV2Internal(newPage);
                        }
                    }
                }
                else
                {
                    newPages.CanUseCacheMode = false;
                    newPages.AddV2Internal(page);
                    newPages.CanUseCacheMode = newPages.CacheMode;
                }
            }
            newPages.CanUseCacheMode = false;

            return newPages;
        }
    }
}
