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

using System;
using System.Collections;
using System.Linq;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.CrossTab.Core;

namespace Stimulsoft.Report.Engine
{
    public class StiCrossTabV2Builder : StiContainerV2Builder
    {
        #region Methods.Helpers
        public static string GetCollapsingName(StiCrossHeader header)
        {
            return GetCollapsingName(header.Name, header.HeaderLevel, header.HeaderValue);
        }

        public static string GetCollapsingName(string componentName, int level, string value)
        {
            return $"{componentName}.{level}.{value}";
        }

        public static bool IsCollapsed(StiCrossHeader masterHeader)
        {
            return IsCollapsed(masterHeader, masterHeader.HeaderLevel, masterHeader.HeaderValue);
        }

        public static bool IsCollapsed(StiCrossHeader masterHeader, int level, string textValue)
        {
            if (StiOptions.Engine.ForceDisableCollapsing) return false;

            if (masterHeader.Interaction == null ||
                (masterHeader.Interaction is StiCrossHeaderInteraction && !((StiCrossHeaderInteraction)masterHeader.Interaction).CollapsingEnabled)) return false;

            if (masterHeader.Report.InteractionCollapsingStates == null)
                return true;

            var value = masterHeader.Report.InteractionCollapsingStates[GetCollapsingName(masterHeader.Name, level, textValue)];
            return !(value is bool) || (bool)value;
        }

        public static void SetCollapsed(StiCrossHeader masterHeader, bool isCollapsed)
        {
            if (StiOptions.Engine.ForceDisableCollapsing) return;

            if (masterHeader.Interaction == null ||
                (masterHeader.Interaction is StiCrossHeaderInteraction &&
                (!((StiCrossHeaderInteraction)masterHeader.Interaction).CollapsingEnabled))) return;

            if (masterHeader.Report.InteractionCollapsingStates == null)
                masterHeader.Report.InteractionCollapsingStates = new Hashtable();

            masterHeader.Report.InteractionCollapsingStates[GetCollapsingName(masterHeader)] = isCollapsed;
        }

        internal static void MakeHorAlignment(StiCrossTab master, StiContainer destination, int startIndex, double parentWidth, int segmentPerWidth)
        {
            if (master.Wrap) return;

            if (master.HorAlignment == StiCrossHorAlignment.Width)
            {
                destination.Left = 0;
                destination.Width = parentWidth;

                StiCrossTabHelper.MakeHorAlignmentByWidth(destination, startIndex);
            }
            else if (master.HorAlignment != StiCrossHorAlignment.None)
            {
                destination.Left = 0;
                destination.Width = parentWidth * (segmentPerWidth > 1 ? segmentPerWidth : 1);

                double maxLeft = 0;
                double maxRight = 0;
                StiCrossTabHelper.CalculateMaxAndMin(destination, ref maxLeft, ref maxRight, startIndex);

                for (var index = startIndex; index < destination.Components.Count; index++)
                {
                    var comp = destination.Components[index];

                    switch (master.HorAlignment)
                    {
                        case StiCrossHorAlignment.Left:
                            comp.Left -= maxLeft;
                            break;

                        case StiCrossHorAlignment.Right:
                            comp.Left += destination.Width - maxRight;
                            break;

                        case StiCrossHorAlignment.Center:
                            var newLeft = (destination.Width - (maxRight - maxLeft)) / 2;
                            comp.Left -= (maxLeft - newLeft);
                            break;
                    }
                }
            }
        }

        private static void FinalizeCross(StiContainer renderedComponent)
        {
            renderedComponent.CanGrow = false;
            renderedComponent.CanShrink = true;

            var size = renderedComponent.GetActualSize();
            renderedComponent.Width = size.Width;
            renderedComponent.Height = size.Height;
            renderedComponent.CanGrow = true;
        }
        #endregion

        #region Methods.Render.CrossTab
        public StiComponent RenderCrossTab(StiCrossTabParams pars, StiCrossTab master)
        {
            var destination = pars.DestinationContainer;
            if (destination == null)
            {
                destination = base.InternalRender(master) as StiContainer;
                destination.Components.Clear();
            }

            destination.DockStyle = StiDockStyle.None;

            if (master.Page.UnlimitedHeight && !master.Page.UnlimitedBreakable)
            {
                if (master.Page.UnlimitedHeight && master.Page.UnlimitedBreakable)
                    destination.CanBreak = false;
            }
            else
            {
                destination.CanBreak = true;
            }

            if (!pars.AllowRendering)
                return destination;

            #region PrintIfEmpty
            if ((master.DataSource == null || master.DataSource.IsEmpty) && !master.PrintIfEmpty)
            {
                pars.RenderingIsFinished = true;
                return destination;
            }
            #endregion

            var rect = pars.DestinationRectangle;
            rect.X = 0;
            rect.Y = 0;

            var placedOnBand = StiSubReportsHelper.GetParentBand(master) != null;
            if (placedOnBand)
            {
                var page = master.Page;
                if (!master.RightToLeft)
                {
                    RenderCrossTabOnDataBand(pars, master, destination, rect);
                }

                else if (!page.UnlimitedBreakable)
                {
                    var right = master.CrossTabInfo.Cross.Widths.Sum();
                    var onePageWidth = (decimal)(page.PageWidth - page.Margins.Left - page.Margins.Right);
                    var pagesCount = (int)(right / onePageWidth) + 1;

                    pars.ShiftX = pagesCount * onePageWidth - right;
                    RenderCrossTabOnDataBand(pars, master, destination, rect);
                }

                else
                {
                    master.RightToLeft = false;
                    RenderCrossTabOnDataBand(pars, master, destination, rect);

                    var right = (decimal)destination.Components.ToList().Max(c => c.Right);
                    var onePageWidth = (decimal)(page.PageWidth - page.Margins.Left - page.Margins.Right);
                    var pagesCount = (int)(right / onePageWidth) + 1;

                    pars.ShiftX = pagesCount * onePageWidth - right - (decimal)destination.Left;
                    destination.Components.Clear();

                    master.RightToLeft = true;
                    StiCrossTabHelper.MakeRightToLeft(master);
                    master.CrossTabInfo.Cross.DoAutoSize();
                    RenderCrossTabOnDataBand(pars, master, destination, rect);
                }
            }

            else if (master.Wrap)
            {
                RenderCrossTabWrapped(pars, master, destination, rect);
            }

            else if (master.Page.UnlimitedHeight)
            {
                rect.Width = 100000000;
                rect.Height = 100000000;

                if (master.Page.UnlimitedBreakable)
                    RenderCrossTabUnlimitedBreakable(pars, master, destination, rect);
                else
                    RenderCrossTabUnlimited(pars, master, destination, rect);
            }
            else
            {
                int endCol;
                int endRow;
                RenderCrossTabOnPage(pars, master, destination, rect, out endCol, out endRow);
            }

            return destination;
        }

        public StiComponent RenderCrossTabOnDataBand(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, RectangleM rect)
        {
            var page = master.Page;
            var cross = master.CrossTabInfo.Cross;
            var engine = master.Report.Engine;

            var onePageWidth = (decimal)(page.PageWidth - page.Margins.Left - page.Margins.Right);
            var segmentWidth = page.UnlimitedBreakable ? onePageWidth : 100000;

            var startIndex = destination.Components.Count;
            pars.StartRow = 0;

            var skip = false;
            var wrapFinished = true;
            var posY = 0m;
            var previousSegmentHeight = 0m;

            //get summary height of all BandsOnAllPages, without rendering, so may be incorrect
            var heightBandsOnAllPages = 0m;
            var listBandsOnAllPages = engine.BandsOnAllPages.GetBandsList();
            foreach(var band in listBandsOnAllPages)
            {
                heightBandsOnAllPages += (decimal)band.Height;
            }

            //additional check if PageHeader/Footer PrintOn==ExceptFirstPage
            foreach (StiComponent comp in page.Components)
            {
                if (comp is StiPageHeaderBand || comp is StiPageFooterBand)
                {
                    if ((comp.PrintOn & StiPrintOnType.ExceptFirstPage) > 0)
                    {
                        heightBandsOnAllPages += (decimal)comp.Height;
                    }
                    if ((comp.PrintOn & StiPrintOnType.OnlyFirstPage) > 0)
                    {
                        heightBandsOnAllPages -= (decimal)comp.Height;
                    }
                }
            }

            while (pars.StartRow < cross.RowCount)
            {
                pars.StartColumn = wrapFinished ? 0 : pars.StartColumn;

                bool needKeepTogether = master.KeepCrossTabTogether && (master.Parent != null && !master.Parent.CanBreak);
                var segmentHeight = pars.StartRow == 0 && !skip && !needKeepTogether
                    ? (decimal)engine.FreeSpace - (decimal)master.Top
                    : (decimal)engine.StaticBands.ReservedFreeSpace - heightBandsOnAllPages; //(decimal)engine.PositionY;

                if (page.UnlimitedHeight && !page.UnlimitedBreakable) 
                    segmentHeight = 100000000;

                if (segmentHeight < 0)
                    segmentHeight = (decimal)engine.StaticBands.ReservedFreeSpace;

                #region First Headers and FreeSpace checking
                //If we starts crosstab rendering, then we need check that on current page only headers will be placed and in result headers will be moved
                //on next page, where headers will be rendered again. So, if we cannot place headers with one cells line on current page, then we increase 
                //posY on freespace value and output will be starts again on next page.
                if (pars.StartRow == 0 || !wrapFinished)
                {
                    var rectMeasure = rect;
                    if (RenderColHeaders(pars, master, destination, ref rectMeasure, true, true) > segmentHeight)
                    {
                        if (!skip)
                        {
                            previousSegmentHeight = segmentHeight;
                            posY += segmentHeight;
                            skip = true;
                            continue;
                        }
                        else
                        {
                            //if impossible to fit columns headers - show as is
                            posY -= previousSegmentHeight;
                            segmentHeight = previousSegmentHeight;
                        }
                    }
                    else if (master.Parent != null && master.Parent is IStiBreakable && !((IStiBreakable)master.Parent).CanBreak)
                    {
                        posY -= previousSegmentHeight;
                    }
                }
                #endregion

                var posX = pars.ShiftX;
                var endRow = 0;
                var firstPage = true;
                while (pars.StartColumn < cross.ColCount)
                {
                    var rectSegment = new RectangleM(posX, posY, segmentWidth - (decimal)destination.Left - (firstPage ? pars.ShiftX : 0), segmentHeight);

                    int endCol;
                    RenderCrossTabSegment(pars, master, destination, rectSegment, out endCol, out endRow);
                    pars.StartColumn = endCol;
                    if (master.Wrap)
                    {
                        wrapFinished = pars.StartColumn >= cross.ColCount;
                        break;
                    }

                    if (firstPage)
                        posX -= pars.ShiftX;

                    posX += segmentWidth;
                    firstPage = false;
                }

                if (wrapFinished)
                {
                    posY += segmentHeight;
                    pars.StartRow = endRow;
                }

                //if destination container have elements with bottom > posY then we need set bottom to posY
                //to avoid incorrect component position.
                if (destination.Components.Count > 0)
                    posY = Math.Max(posY, (decimal)destination.Components.Cast<StiComponent>().Max(c => c.Bottom));

                if (!wrapFinished)
                    posY += (decimal)master.WrapGap;
            }

            #region Increase Segments at Page Width
            if (destination.Components.Count > 0 && !master.Wrap && master.HorAlignment != StiCrossHorAlignment.Width)
            {
                var segmentPerWidth = (decimal)(destination.Components.ToList().Max(c => c.Right) + master.Left) / onePageWidth;
                if (segmentPerWidth > 1)
                {
                    while (segmentPerWidth > destination.ContainerInfoV2.SetSegmentPerWidth)
                        destination.ContainerInfoV2.SetSegmentPerWidth++;
                }
            }
            #endregion

            MakeHorAlignment(master, destination, startIndex, master.Parent.Width, destination.ContainerInfoV2.SetSegmentPerWidth);
            FinalizeCross(destination);

            return destination;
        }

        private void RenderCrossTabWrapped(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, RectangleM rect)
        {
            var cross = master.CrossTabInfo.Cross;
            var rectWidth = rect.Width;
            while (pars.StartColumn < cross.ColCount)
            {
                RenderRowHeaders(pars, master, destination, ref rect);
                var endCol = StiCrossTabHelper.GetEndCol(master, pars.StartColumn, rect);
                var endRow = StiCrossTabHelper.GetEndRow(master, pars.StartRow, rect);

                endRow = StiCrossTabHelper.CheckMergedRowCells(master, pars.StartRow, endRow, pars.StartColumn, endCol);

                StiCrossTabHelper.RenderCells(master, destination, pars.StartColumn, pars.StartRow, endCol, endRow, rect);

                var cellsRect = StiCrossTabHelper.GetCellsRect(master, pars.StartColumn, pars.StartRow, endCol, endRow);
                pars.RenderingIsFinished = (endCol == cross.ColCount) && (endRow == cross.RowCount);

                if (pars.RenderingIsFinished) return;

                if (endCol == cross.ColCount)
                {
                    pars.StartColumn = 0;
                    pars.StartRow = endRow;
                }
                else
                {
                    pars.StartColumn = endCol;
                }

                if (rect.Height - (decimal)master.WrapGap - cellsRect.Height > cellsRect.Height)
                {
                    rect.Y += (cellsRect.Height + (decimal)master.WrapGap);
                    rect.Height -= cellsRect.Height + (decimal)master.WrapGap;
                    rect.X = 0;
                    rect.Width = rectWidth;
                }
                else
                {
                    pars.RenderingIsFinished = false;
                    return;
                }
            }

            pars.RenderingIsFinished = true;
        }

        public void RenderCrossTabUnlimitedBreakable(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, RectangleM rect)
        {
            var page = GetPageForCrossTab(destination) ?? master.Report.RenderedPages.ToList().LastOrDefault();

            var endCol = StiCrossTabHelper.GetEndCol(master, pars.StartColumn, rect, pars.ForceNoBreak);
            var endRow = StiCrossTabHelper.GetEndRow(master, pars.StartRow, rect, pars.ForceNoBreak);

            var pageWidth = page.Width;
            var pageHeight = page.Height;

            var segmentHeightIndex = 0;
            var currentRow = 0;

            while (currentRow < endRow)
            {
                page.SegmentPerHeight = Math.Max(page.SegmentPerHeight, segmentHeightIndex + 1);

                var segmentWidthIndex = 0;
                var currentCol = 0;
                var reachedRow = 0;

                while (currentCol < endCol)
                {
                    page.SegmentPerWidth = Math.Max(page.SegmentPerWidth, segmentWidthIndex + 1);

                    var rectSegment = new RectangleM(
                        rect.Left + (decimal)(segmentWidthIndex * pageWidth),
                        rect.Top + (decimal)(segmentHeightIndex * pageHeight),
                        pars.DestinationRectangle.Width,
                        pars.DestinationRectangle.Height);

                    pars.StartColumn = currentCol;
                    pars.StartRow = currentRow;

                    RenderCrossTabOnPage(pars, master, destination, rectSegment, out currentCol, out reachedRow);

                    segmentWidthIndex++;
                }
                currentRow = reachedRow;

                segmentHeightIndex++;
            }
        }

        private void RenderCrossTabUnlimited(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, RectangleM rect)
        {
            var cross = master.CrossTabInfo.Cross;
            var page = GetPageForCrossTab(destination) ?? master.Report.RenderedPages.ToList().LastOrDefault();

            int endCol;
            int endRow;

            RenderCrossTabOnPage(pars, master, destination, rect, out endCol, out endRow);

            #region Increase Segments at Page Width
            var totalWidth = cross.Widths.Sum() + (decimal)master.Left;
            var segmentPerWidth = totalWidth / (pars.DestinationRectangle.Width + pars.DestinationRectangle.X);

            while (segmentPerWidth > page.SegmentPerWidth)
            {
                page.SegmentPerWidth++;
            }
            #endregion

            #region Increase Segments at Page Height
            var totalHeight = cross.Heights.Sum() + (decimal)master.Top;
            var segmentPerHeight = totalHeight / (pars.DestinationRectangle.Height + pars.DestinationRectangle.Y);

            while (segmentPerHeight > page.SegmentPerHeight)
            {
                page.SegmentPerHeight++;
            }
            #endregion
        }

        private void RenderCrossTabOnPage(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, RectangleM rect, out int endCol, out int endRow)
        {
            RenderCrossTabSegment(pars, master, destination, rect, out endCol, out endRow);

            MakeHorAlignment(master, destination, 0, pars.DestinationContainer != null ? pars.DestinationContainer.Width : master.Parent.Width, 1);
            FinalizeCross(destination);
        }

        private void RenderCrossTabSegment(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, RectangleM rect, out int endCol, out int endRow)
        {
            var cross = master.CrossTabInfo.Cross;

            RenderRowHeaders(pars, master, destination, ref rect);
            RenderColHeaders(pars, master, destination, ref rect);

            endRow = StiCrossTabHelper.GetEndRow(master, pars.StartRow, rect, pars.ForceNoBreak);
            endCol = StiCrossTabHelper.GetEndCol(master, pars.StartColumn, rect, pars.ForceNoBreak);

            endRow = StiCrossTabHelper.CheckMergedRowCells(master, pars.StartRow, endRow, pars.StartColumn, endCol);

            if (pars.StartRow < endRow)
                StiCrossTabHelper.RenderCells(master, destination, pars.StartColumn, pars.StartRow, endCol, endRow, rect);

            pars.RenderingIsFinished = (endCol == cross.ColCount) && (endRow == cross.RowCount);
            pars.StartColumn = endCol;

            if (endCol == cross.ColCount)
            {
                pars.StartColumn = 0;
                pars.StartRow = endRow;
            }
        }

        private decimal RenderColHeaders(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, ref RectangleM rect, bool measure = false, bool skipStartRowCheck = false)
        {
            if (pars.StartRow == 0 && !skipStartRowCheck) 
                return 0;

            if (!measure)
            {
                var measureRect = rect;
                measureRect.Height = 100000;

                var height = RenderColHeaders(pars, master, destination, ref measureRect, true, skipStartRowCheck);
                if (height > rect.Height) 
                    return 0;
            }

            var cross = master.CrossTabInfo.Cross;
            var endCol = GetEndColumn(pars, rect, cross, master);

            if (!measure)
            {
                var rectMeasure = rect;
                if (RenderColHeaders(pars, master, destination, ref rectMeasure, true) > rect.Height) 
                    return 0;
            }

            var totalHeight = 0m;
            var index = 0;
            var totalHeader = StiCrossTabHelper.IsColFieldsEmpty(master) 
                && master.SummaryDirection == StiSummaryDirection.LeftToRight 
                && cross.SumFields.Count > 1 ? 1 : 0;// if totals have subheader & coulums.count == 0;    
            
            if (cross.IsTopCrossTitleVisible)
            {
                if (cross.IsCrossTitlePrintOnAllPages)
                {
                    if (!measure)
                        StiCrossTabHelper.RenderCells(master, destination, pars.StartColumn, 0, endCol, 1 + totalHeader, rect);

                    for (var i = 0; i < 1 + totalHeader; i++)
                    {
                        rect.Y += cross.Heights[i];
                        rect.Height -= cross.Heights[i];
                        totalHeight += cross.Heights[i];
                    }
                }
                index++;
            }

            if (StiCrossTabHelper.IsColFieldsEmpty(master))
                return totalHeight;

            var sumHeader = master.SummaryDirection == StiSummaryDirection.LeftToRight && cross.SumFields.Count > 1 ? 1 : 0;
            if (!measure && AllColFieldsPresentOnAllPages(cross))//Render headers as single block to merge column header with size more then 1
                StiCrossTabHelper.RenderCells(master, destination, pars.StartColumn, index, endCol, index + cross.ColFields.Count + sumHeader, rect);

            for (var i = 0; i < cross.ColFields.Count + sumHeader; i++)
            {
                if ((i < cross.ColFields.Count && (cross.ColFields[i] as StiCrossHeader).PrintOnAllPages) || i >= cross.ColFields.Count)
                {
                    if (!measure && !AllColFieldsPresentOnAllPages(cross))
                        StiCrossTabHelper.RenderCells(master, destination, pars.StartColumn, index, endCol, index + 1, rect);

                    var height = cross.Heights.Length > index ? cross.Heights[index] : 0;

                    rect.Y += height;
                    rect.Height -= height;
                    totalHeight += height;
                }

                index++;
            }
            return totalHeight;
        }

        private decimal RenderRowHeaders(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, ref RectangleM rect, bool measure = false)
        {
            var cross = master.CrossTabInfo.Cross;

            if (!master.RightToLeft && pars.StartColumn == 0)
                return 0;

            if (master.RightToLeft && !measure)
            {
                var endCol = StiCrossTabHelper.GetEndCol(master, pars.StartColumn, rect);
                if (endCol == cross.Cells.Length)
                {
                    if (master.RightToLeft && rect.Width != 100000 - pars.ShiftX - (decimal)destination.Left)
                    {
                        decimal colsWidth = 0;
                        for (var i = pars.StartColumn; i < endCol; i++)
                        {
                            colsWidth += cross.Widths[i];
                        }
                        rect.X += (rect.Width - colsWidth);
                    }

                    return 0;
                }
            }

            decimal headersWidth = 0;
            if (!measure)
            {
                var measureRect = rect;
                measureRect.Width = 100000;
                headersWidth = RenderRowHeaders(pars, master, destination, ref measureRect, true);
                
                if (headersWidth > rect.Width) 
                    return 0;

                if (master.RightToLeft)
                {
                    measureRect = rect;
                    measureRect.Width -= headersWidth;

                    var endCol = StiCrossTabHelper.GetEndCol(master, pars.StartColumn, measureRect);
                    decimal nonHeaderWidth = 0;
                    for (var i = pars.StartColumn; i < endCol; i++)
                    {
                        nonHeaderWidth += cross.Widths[i];
                    }

                    if (rect.Width != 100000)
                        rect.X += rect.Width - (nonHeaderWidth + headersWidth);

                    rect.Width = nonHeaderWidth + headersWidth;
                }

            }

            var resHeight = rect.Height;
            var resY = rect.Y;
            var resX = rect.X;

            var summaryRow = 0;
            var colCorrection = 0;
            if (cross.SumFields.Count > 1 && master.SummaryDirection == StiSummaryDirection.UpToDown)
            {
                summaryRow = 1;
                colCorrection = cross.RowFields.Count;
                master.CrossTabInfo.HidedCells.Clear();
            }

            RenderCorner(pars, master, destination, ref rect, cross, measure, colCorrection);

            if (!measure)
            {
                var rectMeasure = rect;
                if (RenderRowHeaders(pars, master, destination, ref rectMeasure, true) > rect.Width) 
                    return 0;
            }

            var totalWidth = 0m;
            var endRow = GetEndRow(pars, rect, cross);
            var index = 0;

            if (cross.IsLeftCrossTitleVisible || (cross.IsTopCrossTitleVisible && cross.IsColsEmpty))
            {
                if (cross.IsCrossTitlePrintOnAllPages)
                {
                    if (!measure)
                        StiCrossTabHelper.RenderCells(master, destination, 0, pars.StartRow, 1, endRow, rect);

                    rect.X += cross.Widths[0];
                    rect.Width -= cross.Widths[0];
                    totalWidth += cross.Widths[0];
                }

                index++;
            }

            if (StiCrossTabHelper.IsRowFieldsEmpty(master)) 
                return totalWidth;

            int inc = 1;
            if (master.RightToLeft)
            {
                index = cross.Cells.Length - 1 - index;
                inc = -1;
            }

            var startIndex = index;
            decimal sumWidth = 0;

            
            for (var i = 0; i < (cross.RowFields.Count + summaryRow); i++)
            {
                StiCrossHeader field = i < cross.RowFields.Count ? (StiCrossHeader)cross.RowFields[i] : null;
                var width = cross.Widths.Length > index ? cross.Widths[index] : 0;
                if (field == null || field.PrintOnAllPages)
                {
                    sumWidth += width;
                    totalWidth += width;
                }

                if ((field == null || !field.PrintOnAllPages || field == cross.RowFields[cross.RowFields.Count - 1]) && sumWidth > 0)
                {
                    if (master.RightToLeft)
                        rect.X = resX + rect.Width - sumWidth;

                    if (!measure)
                        StiCrossTabHelper.RenderCells(master, destination, startIndex, pars.StartRow, index + 1, endRow, rect, colCorrection);

                    if (!master.RightToLeft)
                        rect.X += sumWidth;

                    rect.Width -= sumWidth;
                    sumWidth = 0;
                    startIndex = index + inc;
                }

                if (!master.RightToLeft) 
                    index++;

                else 
                    startIndex--;
            }

            if (master.RightToLeft)
                rect.X = resX;

            rect.Y = resY;
            rect.Height = resHeight;

            return totalWidth;
        }

        private decimal RenderCorner(StiCrossTabParams pars, StiCrossTab master, StiContainer destination, ref RectangleM rect, StiCross cross, bool measure = false, int colCorrection = 0)
        {
            if (pars.StartRow == 0 || StiCrossTabHelper.IsColFieldsEmpty(master)) 
                return 0;

            if (!measure)
            {
                var rectMeasure = rect;
                if (RenderCorner(pars, master, destination, ref rectMeasure, cross, true, colCorrection) > rect.Height) 
                    return 0;
            }

            var totalHeight = 0m;
            var index = 0;
            var rowCount = cross.RowFields.Cast<StiCrossHeader>().Count(r => r.PrintOnAllPages);
            if (cross.IsTopCrossTitleVisible)
            {
                if (cross.IsCrossTitlePrintOnAllPages)
                {
                    if (!measure)
                        StiCrossTabHelper.RenderCells(master, destination, 0, 0, rowCount, 1, rect, colCorrection);

                    rect.Y += cross.Heights[0];
                    rect.Height -= cross.Heights[0];
                    totalHeight += cross.Heights[0];
                }
                index++;
            }

            if (!measure && AllColFieldsPresentOnAllPages(cross))//Render headers as single block to merge column header with size more then 1
                StiCrossTabHelper.RenderCells(master, destination, 0, index, rowCount, index + cross.ColFields.Count, rect, colCorrection);

            foreach (StiCrossHeader field in cross.ColFields)
            {
                if (field.PrintOnAllPages)
                {
                    if (!measure && !AllColFieldsPresentOnAllPages(cross))
                        StiCrossTabHelper.RenderCells(master, destination, 0, index, rowCount, index + 1, rect);

                    var height = cross.Heights.Length > index ? cross.Heights[index] : 0;

                    rect.Y += height;
                    rect.Height -= height;
                    totalHeight += height;
                }

                index++;
            }
            return totalHeight;
        }

        private int GetEndColumn(StiCrossTabParams pars, RectangleM rect, StiCross cross, StiCrossTab master)
        {
            var width = rect.Width;
            var index = pars.StartColumn;
            while (index < cross.ColCount)
            {
                width -= cross.Widths[index];

                if (width < 0 && master.HorAlignment != StiCrossHorAlignment.Width) break;
                index++;
            }

            if (index == pars.StartColumn && (index + 1) <= cross.ColCount) 
                index++;

            return index;
        }

        private int GetEndRow(StiCrossTabParams pars, RectangleM rect, StiCross cross)
        {
            var height = rect.Height;
            var index = pars.StartRow;
            while (index < cross.RowCount)
            {
                height -= cross.Heights[index];

                if (height < 0) break;
                index++;
            }

            if (index == pars.StartRow && (index + 1) <= cross.RowCount) 
                index++;

            return index;
        }

        private static StiPage GetPageForCrossTab(StiContainer destination)
        {
            StiPage page = null;
            var parent = destination;

            while (page == null && parent != null)
            {
                page = parent.Page;
                parent = parent.Parent;
            }
            return page;
        }

        private bool AllColFieldsPresentOnAllPages(StiCross cross)
        {
            return cross.ColFields.Cast<StiCrossHeader>().All(c => c.PrintOnAllPages);
        }
        #endregion

        #region Methods.Render
        public override void Prepare(StiComponent masterComp)
        {
            base.Prepare(masterComp);

            StiFilterHelper.SetFilter(masterComp);
        }

        public override void UnPrepare(StiComponent masterComp)
        {
            base.UnPrepare(masterComp);

            StiCrossTabHelper.ClearCross(masterComp as StiCrossTab);
        }

        public override StiComponent InternalRender(StiComponent masterComp)
        {
            var masterCrossTab = masterComp as StiCrossTab;
            var pars = new StiCrossTabParams();

            var placedOnBand = StiSubReportsHelper.GetParentBand(masterCrossTab) != null;
            pars.AllowRendering = placedOnBand;

            var band = masterComp.Parent as StiDataBand;

            if (band != null)
                pars.DestinationRectangle = band.Parent.ClientRectangle.ToRectangleM();
            else
                pars.DestinationRectangle = masterCrossTab.ClientRectangle.ToRectangleM();

            if (placedOnBand)
            {
                StiCrossTabHelper.CreateCross(masterCrossTab);
                return RenderCrossTab(pars, masterCrossTab);
            }

            var cont = base.InternalRender(masterComp) as StiContainer;
            if (cont != null)
            {
                cont.Width = masterComp.Width;
                cont.Height = masterComp.Height;
                cont.Components.Clear();
            }

            return cont;
        }
        #endregion
    }
}
