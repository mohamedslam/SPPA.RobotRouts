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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.App;
using Stimulsoft.Report.Units;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Design
{
    public class StiTableHelper
    {
        #region Methods
        public static bool IsAllowUseInTableMode(StiComponent comp)
        {
            if (comp is StiPage)
                return false;

            if (comp.Parent is IStiCrossTab)
                return false;

            return StiRestrictionsHelper.IsAllowChangePosition(comp);
        }

        public static int GetSelectedCount(StiComponentsCollection comps)
        {
            if (comps == null)
                return 0;

            var count = 0;

            foreach (StiComponent comp in comps)
            {
                if (IsAllowUseInTableMode(comp))
                    count++;
            }
            return count;
        }

        public static void GetSelectedLines(IStiDesignerBase designer, ref Hashtable xx, ref Hashtable yy, bool isPaint)
        {
            GetSelectedLines(designer, ref xx, ref yy, isPaint, isPaint);
        }

        public static void GetSelectedLines(IStiDesignerBase designer,
            ref Hashtable xx, ref Hashtable yy, bool isPaint, bool useZoom)
        {
            var rect = GetSelectedRectangle(designer, isPaint, useZoom);

            xx[rect.X] = rect.X;
            yy[rect.Y] = rect.Y;
            xx[rect.Right] = rect.Right;
            yy[rect.Bottom] = rect.Bottom;

            foreach (StiComponent comp in designer.SelectedComponentsOnPage)
            {
                if (IsAllowUseInTableMode(comp))
                {
                    var compRect = comp.GetPaintRectangle(isPaint, useZoom);

                    xx[compRect.X] = compRect.X;
                    yy[compRect.Y] = compRect.Y;
                    xx[compRect.Right] = compRect.Right;
                    yy[compRect.Bottom] = compRect.Bottom;
                }
            }
        }

        public static bool IsTableMode(StiComponent component)
        {
            return IsTableMode(component.Page.Report.Designer);
        }

        public static bool IsTableMode(IStiDesignerBase designer)
        {
            if (designer == null)
                return false;

            return GetSelectedCount(designer.SelectedComponentsOnPage) > 1;
        }

        public static RectangleD GetSelectedRectangle(IStiDesignerBase designer)
        {
            return GetSelectedRectangle(designer, false);
        }

        public static RectangleD GetSelectedRectangle(IStiDesignerBase designer, bool isPaintRect)
        {
            return GetSelectedRectangle(designer, isPaintRect, designer.SelectedComponentsOnPage);
        }

        public static RectangleD GetSelectedRectangle(IStiDesignerBase designer, bool isPaintRect, bool useZoom)
        {
            return GetSelectedRectangle(designer, isPaintRect, useZoom, designer.SelectedComponentsOnPage);
        }

        public static RectangleD GetSelectedRectangle(IStiDesignerBase designer, bool isPaintRect, StiComponentsCollection comps)
        {
            return GetSelectedRectangle(designer, isPaintRect, isPaintRect, comps);
        }

        public static RectangleD GetSelectedRectangle(IStiDesignerBase designer, bool isPaintRect, bool useZoom, StiComponentsCollection comps)
        {
            var rect = RectangleD.Empty;

            foreach (StiComponent comp in comps)
            {
                if (IsAllowUseInTableMode(comp))
                {
                    var compRect = comp.GetPaintRectangle(isPaintRect, useZoom);

                    rect = rect.IsEmpty ? compRect : rect.FitToRectangle(compRect);
                }
            }
            return rect;
        }

        public static double GetDistForResize(StiComponent comp)
        {
            return comp.Page.Unit.ConvertFromHInches(4d);
        }

        private RectangleD GetRect(StiComponent comp)
        {
            return comp.GetPaintRectangle(false, false, false);
        }

        private void SetRect(StiComponent comp, RectangleD rect)
        {
            comp.SetPaintRectangle(rect);
        }

        private double ConvertValue(double value)
        {
            value = designer.Report.GetCurrentPage().Unit.ConvertFromHInches(
                value / designer.Report.GetCurrentPage().Zoom);

            return value;
        }

        private double Align(double value)
        {
            if (altKeyMode)
                return value;

            return StiAlignValue.AlignToGrid(value,
                designer.Report.GetCurrentPage().GridSize,
                designer.Report.Info.AlignToGrid);
        }

        private decimal Round(double value)
        {
            if (designer.Report.GetCurrentPage() is IStiForm)
                return Math.Round((decimal)value, 0);

            if (designer.Report.GetCurrentPage() is IStiScreen)
                return Math.Round((decimal)value, 0);

            if (designer.Report.GetCurrentPage() is IStiDashboard)
                return Math.Round((decimal)value, 0);

            var unit = designer.Report.GetCurrentPage().Unit;

            if (unit is StiCentimetersUnit || unit is StiInchesUnit)
                return Math.Round((decimal)value, 1);

            return Math.Round((decimal)value, 0);
        }

        private bool IsContain(double value1, double value2, double dist)
        {
            return (value1 > (value2 - dist)) && (value1 < (value2 + dist));
        }

        public void ProcessResizeColumns()
        {
            var currentPos = Cursor.Position;
            var currentX = ConvertValue(currentPos.X);
            var storedX = ConvertValue(storedPosition.X);
            var offset = Align((currentX - storedX) * StiScale.System);
            var resizeType = IsControlKeyPressed ? StiResizeType.TwoResize : StiResizeType.Resize;
            var objs = designer.Report.GetSelected();

            if (!designer.Report.IsPageDesigner && objs.Any(obj => obj is IStiTableCell))
                resizeType = StiResizeType.TwoResize;

            ResizeColumns(mouseX, offset, resizeType);
        }

        public void ResizeColumns(double lineX, double offset, StiResizeType resizeType)
        {
            var dist = GetDistForResize(designer.Report.GetCurrentPage());

            #region Check range
            foreach (StiComponent component in designer.SelectedComponentsOnPage)
            {
                if (!IsAllowUseInTableMode(component)) continue;

                var clientRect = component.ClientRectangle;
                RestoreComponentLocation(component);

                try
                {
                    var rect = GetRect(component);

                    if (IsContain(rect.Right, lineX, dist))
                    {
                        if ((component.Width + offset) < 0)
                            offset = -component.Width;

                        continue;
                    }

                    if (rect.Left < lineX && rect.Right > lineX && resizeType == StiResizeType.Resize)
                    {
                        if ((component.Width + offset) < 0)
                            offset = -component.Width;

                        continue;
                    }

                    if (IsContain(rect.Left, lineX, dist) && resizeType == StiResizeType.TwoResize)
                    {
                        if ((component.Width - offset) < 0)
                            offset = component.Width;

                        continue;
                    }
                }
                finally
                {
                    component.ClientRectangle = clientRect;
                }
            }

            #endregion

            RestoreComponentsLocation();

            #region Change Position
            foreach (StiComponent component in designer.SelectedComponentsOnPage)
            {
                if (!IsAllowUseInTableMode(component)) continue;

                var rect = GetRect(component);

                if (IsContain(rect.Right, lineX, dist))
                {
                    if (component is IStiTableCell)
                    {
                        ResizeTableComponentCell(component, offset, 0, StiAction.SizeRight);
                        continue;
                    }
                    component.Width += offset;
                    component.Width = Align(component.Width);
                    continue;
                }

                if (rect.Left >= (lineX - dist) && resizeType == StiResizeType.Resize)
                {
                    component.Left += offset;
                    component.Left = Align(component.Left);
                    continue;
                }

                if (rect.Left < lineX && rect.Right > lineX && resizeType == StiResizeType.Resize)
                {
                    component.Width += offset;
                    component.Width = Align(component.Width);
                    continue;
                }

                if (IsContain(rect.Left, lineX, dist) && resizeType == StiResizeType.TwoResize)
                {
                    if (component is IStiTableCell)
                    {
                        ResizeTableComponentCell(component, -offset, 0, StiAction.SizeLeft);
                        continue;
                    }
                    component.Left += offset;
                    component.Width -= offset;
                    component.Left = Align(component.Left);
                    component.Width = Align(component.Width);
                    continue;
                }
            }
            #endregion
        }

        public void ProcessResizeRows()
        {
            var currentPos = Cursor.Position;
            var currentY = ConvertValue(currentPos.Y);
            var storedY = ConvertValue(storedPosition.Y);
            var offset = Align((currentY - storedY) * StiScale.System);
            var resizeType = IsControlKeyPressed ? StiResizeType.TwoResize : StiResizeType.Resize;
            var objs = designer.Report.GetSelected();

            if (!designer.Report.IsPageDesigner && objs.Any(obj => obj is IStiTableCell))
                resizeType = StiResizeType.TwoResize;

            ResizeRows(mouseY, offset, resizeType);
        }

        public void ResizeRows(double lineY, double offset, StiResizeType resizeType)
        {
            var dist = GetDistForResize(designer.Report.GetCurrentPage());

            #region Check range
            foreach (StiComponent component in designer.SelectedComponentsOnPage)
            {
                if (!IsAllowUseInTableMode(component)) continue;

                var clientRect = component.ClientRectangle;
                RestoreComponentLocation(component);

                try
                {
                    var rect = GetRect(component);

                    if (IsContain(rect.Bottom, lineY, dist))
                    {
                        if ((component.Height + offset) < 0) offset = -component.Height;
                        continue;
                    }

                    if (rect.Top < lineY && rect.Bottom > lineY && resizeType == StiResizeType.Resize)
                    {
                        if ((component.Height + offset) < 0) offset = -component.Height;
                        continue;
                    }

                    if (IsContain(rect.Top, lineY, dist) && resizeType == StiResizeType.TwoResize)
                    {
                        if ((component.Height - offset) < 0) offset = component.Height;
                        continue;
                    }
                }
                finally
                {
                    component.ClientRectangle = clientRect;
                }
            }

            #endregion

            RestoreComponentsLocation();

            #region Change position
            foreach (StiComponent component in designer.SelectedComponentsOnPage)
            {
                if (!IsAllowUseInTableMode(component)) continue;

                var rect = GetRect(component);

                if (IsContain(rect.Bottom, lineY, dist))
                {
                    if (component is IStiTableCell)
                    {
                        ResizeTableComponentCell(component, 0, offset, StiAction.SizeBottom);
                        continue;
                    }
                    component.Height += offset;
                    component.Height = Align(component.Height);
                    continue;
                }

                if (rect.Top >= (lineY - dist) && resizeType == StiResizeType.Resize)
                {
                    component.Top += offset;
                    component.Top = Align(component.Top);
                    continue;
                }

                if (rect.Top < lineY && rect.Bottom > lineY && resizeType == StiResizeType.Resize)
                {
                    component.Height += offset;
                    component.Height = Align(component.Height);
                    continue;
                }

                if (IsContain(rect.Top, lineY, dist) && resizeType == StiResizeType.TwoResize)
                {
                    if (component is IStiTableCell)
                    {
                        ResizeTableComponentCell(component, 0, -offset, StiAction.SizeTop);
                        continue;
                    }
                    component.Top += offset;
                    component.Height -= offset;
                    component.Top = Align(component.Top);
                    component.Height = Align(component.Height);
                    continue;
                }
            }
            #endregion
        }

        public void ProcessResizeTable(bool resizeHorizontally, bool resizeVertically)
        {
            var selectedRect = GetSelectedRectangle(designer);
            var currentPos = Cursor.Position;

            #region Check inversion
            var invertX = false;
            var invertY = false;

            switch (designer.Report.Info.CurrentAction)
            {
                case StiAction.SizeLeft:
                case StiAction.SizeLeftTop:
                case StiAction.SizeLeftBottom:
                    invertX = true;
                    break;
            }

            switch (designer.Report.Info.CurrentAction)
            {
                case StiAction.SizeLeftTop:
                case StiAction.SizeTop:
                case StiAction.SizeRightTop:
                    invertY = true;
                    break;
            }
            #endregion

            var currentX = ConvertValue(currentPos.X);
            var currentY = ConvertValue(currentPos.Y);
            var storedX = ConvertValue(storedPosition.X);
            var storedY = ConvertValue(storedPosition.Y);

            double offsetX;
            double offsetY;

            double startX;
            double startY;

            if (invertX)
            {
                offsetX = storedX - currentX;
                startX = selectedRect.Right;
            }
            else
            {
                offsetX = currentX - storedX;
                startX = selectedRect.X;
            }

            if (invertY)
            {
                offsetY = storedY - currentY;
                startY = selectedRect.Bottom;
            }
            else
            {
                offsetY = currentY - storedY;
                startY = selectedRect.Y;
            }

            var scale = StiScale.System;
            offsetX *= scale;
            offsetY *= scale;

            var dpiX = ((storedSelectedRect.Width + offsetX) / storedSelectedRect.Width);
            var dpiY = ((storedSelectedRect.Height + offsetY) / storedSelectedRect.Height);

            if ((dpiX < 0) || double.IsNaN(dpiX) || double.IsInfinity(dpiX)) return;
            if ((dpiY < 0) || double.IsNaN(dpiY) || double.IsInfinity(dpiY)) return;

            RestoreComponentsLocation();

            //fix: if first component is not aligned to grid, then exception occurs after resize due
            //to the nature of the calculations

            selectedRect = GetSelectedRectangle(designer);
            if (invertY)
            {
                if (startY < selectedRect.Bottom)
                    startY = selectedRect.Bottom;
            }
            else
            {
                if (startY > selectedRect.Top)
                    startY = selectedRect.Top;
            }

            //the same for horizontal coordinates
            if (invertX)
            {
                if (startX < selectedRect.Right)
                    startX = selectedRect.Right;
            }
            else
            {
                if (startX > selectedRect.Left)
                    startX = selectedRect.Left;
            }

            ResizeTable(designer.SelectedComponentsOnPage, startX, startY, dpiX, dpiY, resizeHorizontally, resizeVertically, invertX, invertY);
        }

        public void ResizeTable(StiComponentsCollection components, double startX, double startY, double dpiX, double dpiY,
            bool resizeHorizontally, bool resizeVertically, bool invertX, bool invertY, bool isTableComponent = false)
        {
            var x = new Hashtable();
            var y = new Hashtable();

            foreach (StiComponent component in components)
            {
                if (!IsAllowUseInTableMode(component)) continue;

                var rect = GetRect(component);
                var originalRect = rect;

                rect.Width *= dpiX;
                rect.Height *= dpiY;

                rect.X = invertX
                    ? startX - (startX - rect.X) * dpiX
                    : startX + (rect.X - startX) * dpiX;

                rect.Y = invertY
                    ? (startY - (startY - rect.Y) * dpiY)
                    : (startY + (rect.Y - startY) * dpiY);

                rect = rect.Normalize();
                if (isTableComponent == false) rect = new RectangleD(Align(rect.X), Align(rect.Y), Align(rect.Width), Align(rect.Height));

                x[Round(originalRect.Left)] = rect.X;
                y[Round(originalRect.Top)] = rect.Y;
                x[Round(originalRect.Right)] = rect.Right;
                y[Round(originalRect.Bottom)] = rect.Bottom;
            }

            foreach (StiComponent component in components)
            {
                if (!IsAllowUseInTableMode(component)) continue;

                var originalRect = GetRect(component);

                var leftObject = x[Round(originalRect.Left)];
                var topObject = y[Round(originalRect.Top)];
                var rightObject = x[Round(originalRect.Right)];
                var bottomObject = y[Round(originalRect.Bottom)];

                if (leftObject != null && topObject != null && rightObject != null && bottomObject != null)
                {
                    var left = (double)leftObject;
                    var top = (double)topObject;
                    var right = (double)rightObject;
                    var bottom = (double)bottomObject;

                    if (!resizeHorizontally)
                    {
                        left = originalRect.Left;
                        right = originalRect.Right;
                    }

                    if (!resizeVertically)
                    {
                        top = originalRect.Top;
                        bottom = originalRect.Bottom;
                    }
                    if (isTableComponent)
                    {
                        SaveComponentLocation(component);
                        component.SetPaintRectangle(new RectangleD(left, top, right - left, bottom - top));
                    }
                    else if (component is IStiTableCell)
                    {
                        var offsetX = 0d;
                        var offsetY = 0d;

                        switch (designer.Report.Info.CurrentAction)
                        {
                            case StiAction.SizeRight:
                            case StiAction.SizeRightTop:
                            case StiAction.SizeRightBottom:
                                offsetX = right - originalRect.Right;
                                break;

                            case StiAction.SizeLeft:
                            case StiAction.SizeLeftTop:
                            case StiAction.SizeLeftBottom:
                                offsetX = originalRect.Left - left;
                                break;
                        }

                        switch (designer.Report.Info.CurrentAction)
                        {
                            case StiAction.SizeTop:
                            case StiAction.SizeRightTop:
                            case StiAction.SizeLeftTop:
                                offsetY = originalRect.Top - top;
                                break;

                            case StiAction.SizeBottom:
                            case StiAction.SizeRightBottom:
                            case StiAction.SizeLeftBottom:
                                offsetY = bottom - originalRect.Bottom;
                                break;
                        }

                        ResizeTableComponentCell(component, offsetX, offsetY, designer.Report.Info.CurrentAction);
                    }
                    else if (component is StiTable table)
                    {
                        foreach (StiComponent comp in table.Components)
                        {
                            RestoreComponentLocation(comp);
                        }
                        var rect = new RectangleD(left, top, right - left, bottom - top);
                        SetRect(component, rect);
                        ResizeTable(table.Components, startX, startY, rect.Width / originalRect.Width, rect.Height / originalRect.Height, resizeHorizontally, resizeVertically, invertX, invertY, true);
                    }
                    else
                    {
                        SetRect(component, new RectangleD(left, top, right - left, bottom - top));
                    }
                }
            }
        }

        public void SaveSelectedRectangle()
        {
            storedSelectedRect = GetSelectedRectangle(designer);
        }

        public void SaveComponentsLocation()
        {
            if (storedLocations == null)
                storedLocations = new Hashtable();
            else
                storedLocations.Clear();

            foreach (StiComponent component in designer.SelectedComponentsOnPage)
            {
                SaveComponentLocation(component);
            }
        }

        public void SaveComponentLocation(StiComponent component)
        {
            if (!IsAllowUseInTableMode(component)) return;

            storedLocations[component] = component.ClientRectangle;
        }

        public void RestoreComponentsLocation()
        {
            if (storedLocations == null) return;

            foreach (StiComponent comp in designer.SelectedComponentsOnPage)
            {
                if (!IsAllowUseInTableMode(comp)) continue;

                RestoreComponentLocation(comp);
            }
        }

        public void RestoreComponentLocation(StiComponent comp)
        {
            if (storedLocations[comp] is RectangleD)
                comp.ClientRectangle = (RectangleD)storedLocations[comp];
        }

        public void SaveCursorLocation(int eX, int eY)
        {
            storedPosition = Cursor.Position;

            mouseX = designer.XToPage(eX);
            mouseY = designer.YToPage(eY);
        }

        public void Dispose()
        {
            storedLocations?.Clear();
            storedLocations = null;
        }
        #endregion

        #region Table Compoent Methods
        private void ResizeTableComponentCell(StiComponent tableCell, double offsetX, double offsetY, StiAction action)
        {
            var dist = GetDistForResize(designer.Report.GetCurrentPage());

            var table = tableCell.Parent as StiTable;
            var changedComponentColumnList = new Dictionary<StiComponent, RectangleD>();
            var changedComponentRowList = new Dictionary<StiComponent, RectangleD>();

            RestoreComponentLocation(table);

            var tableRect = GetRect(table);
            var cellRect = GetRect(tableCell);
            var originalRect = cellRect;

            var isFirstColumn = Math.Round(tableRect.Left) == Math.Round(cellRect.Left);
            var isFirstRow = Math.Round(tableRect.Top) == Math.Round(cellRect.Top);
            var isLastColumn = Math.Round(tableRect.Right) == Math.Round(cellRect.Right);
            var isLastRow = Math.Round(tableRect.Bottom) == Math.Round(cellRect.Bottom);

            foreach (StiComponent comp in table.Components)
            {
                RestoreComponentLocation(comp);
            }

            switch (action)
            {
                case StiAction.SizeLeft:
                case StiAction.SizeLeftTop:
                case StiAction.SizeLeftBottom:
                    if (!isFirstColumn)
                    {
                        if (cellRect.X - offsetX >= cellRect.Right) offsetX = -cellRect.Width + dist;
                        if (cellRect.X - offsetX <= 0) offsetX = cellRect.X - dist;

                        var errorRect = new RectangleD();

                        if (!ResizeTableComponentColumns(tableCell, originalRect.X + dist, offsetX, StiAction.SizeLeft, ref changedComponentColumnList, ref errorRect))
                        {
                            changedComponentColumnList.Clear();
                            offsetX = -errorRect.Width + dist;
                            if (!ResizeTableComponentColumns(tableCell, originalRect.X + dist, offsetX, StiAction.SizeLeft, ref changedComponentColumnList, ref errorRect)) return;
                        }
                        if (!ResizeTableComponentColumns(tableCell, originalRect.X - dist, -offsetX, StiAction.SizeRight, ref changedComponentColumnList, ref errorRect))
                        {
                            changedComponentColumnList.Clear();
                            offsetX = errorRect.Width - dist;
                            if (!ResizeTableComponentColumns(tableCell, originalRect.X + dist, offsetX, StiAction.SizeLeft, ref changedComponentColumnList, ref errorRect)) return;
                            if (!ResizeTableComponentColumns(tableCell, originalRect.X - dist, -offsetX, StiAction.SizeRight, ref changedComponentColumnList, ref errorRect)) return;
                        }

                        cellRect.X -= offsetX;
                        cellRect.Width += offsetX;
                    }
                    break;

                case StiAction.SizeRight:
                case StiAction.SizeRightTop:
                case StiAction.SizeRightBottom:
                    if (!isLastColumn)
                    {
                        if (cellRect.Width + offsetX <= 0) offsetX = -cellRect.Width + dist;

                        var errorRect = new RectangleD();

                        if (!ResizeTableComponentColumns(tableCell, originalRect.Right - dist, offsetX, StiAction.SizeRight, ref changedComponentColumnList, ref errorRect))
                        {
                            changedComponentColumnList.Clear();
                            offsetX = -errorRect.Width + dist;
                            if (!ResizeTableComponentColumns(tableCell, originalRect.Right - dist, offsetX, StiAction.SizeRight, ref changedComponentColumnList, ref errorRect)) return;
                        }
                        if (!ResizeTableComponentColumns(tableCell, originalRect.Right + dist, -offsetX, StiAction.SizeLeft, ref changedComponentColumnList, ref errorRect))
                        {
                            changedComponentColumnList.Clear();
                            offsetX = errorRect.Width - dist;
                            if (!ResizeTableComponentColumns(tableCell, originalRect.Right - dist, offsetX, StiAction.SizeRight, ref changedComponentColumnList, ref errorRect)) return;
                            if (!ResizeTableComponentColumns(tableCell, originalRect.Right + dist, -offsetX, StiAction.SizeLeft, ref changedComponentColumnList, ref errorRect)) return;
                        }

                        cellRect.Width += offsetX;
                    }
                    break;

            }

            switch (action)
            {
                case StiAction.SizeTop:
                case StiAction.SizeLeftTop:
                case StiAction.SizeRightTop:
                    if (!isFirstRow)
                    {
                        if (cellRect.Y - offsetY >= cellRect.Bottom) offsetY = -cellRect.Height + dist;
                        if (cellRect.Y - offsetY <= 0) offsetY = cellRect.Y - dist;

                        var errorRect = new RectangleD();

                        if (!ResizeTableComponentRows(tableCell, originalRect.Y + dist, offsetY, StiAction.SizeTop, ref changedComponentRowList, ref errorRect))
                        {
                            changedComponentRowList.Clear();
                            offsetY = -errorRect.Height + dist;
                            if (!ResizeTableComponentRows(tableCell, originalRect.Y + dist, offsetY, StiAction.SizeTop, ref changedComponentRowList, ref errorRect)) return;
                        }
                        if (!ResizeTableComponentRows(tableCell, originalRect.Y - dist, -offsetY, StiAction.SizeBottom, ref changedComponentRowList, ref errorRect))
                        {
                            changedComponentRowList.Clear();
                            offsetY = errorRect.Height - dist;
                            if (!ResizeTableComponentRows(tableCell, originalRect.Y + dist, offsetY, StiAction.SizeTop, ref changedComponentRowList, ref errorRect)) return;
                            if (!ResizeTableComponentRows(tableCell, originalRect.Y - dist, -offsetY, StiAction.SizeBottom, ref changedComponentRowList, ref errorRect)) return;
                        }

                        cellRect.Y -= offsetY;
                        cellRect.Height += offsetY;
                    }
                    break;

                case StiAction.SizeBottom:
                case StiAction.SizeLeftBottom:
                case StiAction.SizeRightBottom:
                    {
                        if (cellRect.Height + offsetY <= 0) offsetY = -cellRect.Height + dist;

                        var errorRect = new RectangleD();

                        if (!ResizeTableComponentRows(tableCell, originalRect.Bottom - dist, offsetY, StiAction.SizeBottom, ref changedComponentRowList, ref errorRect))
                        {
                            changedComponentRowList.Clear();
                            offsetY = -errorRect.Height + dist;
                            if (!ResizeTableComponentRows(tableCell, originalRect.Bottom - dist, offsetY, StiAction.SizeBottom, ref changedComponentRowList, ref errorRect)) return;
                        }
                        if (!ResizeTableComponentRows(tableCell, originalRect.Bottom + dist, -offsetY, StiAction.SizeTop, ref changedComponentRowList, ref errorRect))
                        {
                            changedComponentRowList.Clear();
                            offsetY = errorRect.Height - dist;
                            if (!ResizeTableComponentRows(tableCell, originalRect.Bottom - dist, offsetY, StiAction.SizeBottom, ref changedComponentRowList, ref errorRect)) return;
                            if (!ResizeTableComponentRows(tableCell, originalRect.Bottom + dist, -offsetY, StiAction.SizeTop, ref changedComponentRowList, ref errorRect)) return;
                        }

                        cellRect.Height += offsetY;

                        if (isLastRow)
                        {
                            SaveComponentLocation(table);
                            tableRect.Height += offsetY;
                            SetRect(table, tableRect);
                        }
                    }
                    break;

            }

            var changedComponentList = changedComponentColumnList;
            foreach (var item in changedComponentRowList)
            {
                if (changedComponentList.ContainsKey(item.Key))
                    changedComponentList[item.Key] = new RectangleD(changedComponentList[item.Key].X, item.Value.Y, changedComponentList[item.Key].Width, item.Value.Height);
                else
                    changedComponentList[item.Key] = item.Value;
            }
            SetPaintRectangle(changedComponentList);

            tableCell.SetPaintRectangle(cellRect);
        }

        private void SetPaintRectangle(Dictionary<StiComponent, RectangleD> list)
        {
            foreach (var item in list)
            {
                SaveComponentLocation(item.Key);
                item.Key.SetPaintRectangle(item.Value);
            }
        }

        private bool ResizeTableComponentColumns(StiComponent baseCell, double lineX, double offset, StiAction action, ref Dictionary<StiComponent, RectangleD> changedComponentList, ref RectangleD cellRect)
        {
            if (offset == 0) return true;

            var table = baseCell.Parent as StiTable;

            var dist = GetDistForResize(designer.Report.GetCurrentPage()) * 1.1;

            foreach (StiComponent tableCell in table.Components)
            {
                if (changedComponentList.Keys.Contains(tableCell))
                    continue;
                if (baseCell == tableCell)
                    continue;

                cellRect = GetRect(tableCell);

                if (cellRect.Left < lineX && cellRect.Left + dist >= lineX ||
                    cellRect.Right > lineX && cellRect.Right - dist <= lineX)
                {
                    if (!TryResizeTableComponentCell(ref cellRect, offset, 0, action))
                        return false;

                    changedComponentList.Add(tableCell, cellRect);
                }
            }

            return true;
        }

        private bool ResizeTableComponentRows(StiComponent baseCell, double lineY, double offset, StiAction action, ref Dictionary<StiComponent, RectangleD> changedComponentList, ref RectangleD cellRect)
        {
            if (offset == 0) return true;

            var table = baseCell.Parent as StiTable;

            var dist = GetDistForResize(designer.Report.GetCurrentPage()) * 1.1;

            foreach (StiComponent tableCell in table.Components)
            {
                if (changedComponentList.Keys.Contains(tableCell))
                    continue;
                if (baseCell == tableCell)
                    continue;

                cellRect = GetRect(tableCell);

                if (cellRect.Top < lineY && cellRect.Top + dist >= lineY ||
                    cellRect.Bottom > lineY && cellRect.Bottom - dist <= lineY)
                {
                    if (!TryResizeTableComponentCell(ref cellRect, 0, offset, action))
                        return false;

                    changedComponentList.Add(tableCell, cellRect);
                }
            }

            return true;
        }

        private bool TryResizeTableComponentCell(ref RectangleD cellRect, double offsetX, double offsetY, StiAction action)
        {
            var dist = GetDistForResize(designer.Report.GetCurrentPage());

            switch (action)
            {
                case StiAction.SizeLeft:

                    if (cellRect.X - offsetX > cellRect.Right - dist) return false;

                    cellRect.X -= offsetX;
                    cellRect.Width += offsetX;
                    break;

                case StiAction.SizeLeftTop:

                    if (cellRect.X - offsetX > cellRect.Right - dist || cellRect.Y - offsetY > cellRect.Bottom - dist) return false;

                    cellRect.X -= offsetX;
                    cellRect.Width += offsetX;
                    cellRect.Y -= offsetY;
                    cellRect.Height += offsetY;
                    break;

                case StiAction.SizeTop:

                    if (cellRect.Y - offsetY > cellRect.Bottom - dist) return false;

                    cellRect.Y -= offsetY;
                    cellRect.Height += offsetY;
                    break;

                case StiAction.SizeRightTop:

                    if (cellRect.Y - offsetY > cellRect.Bottom - dist || cellRect.Width + offsetX <= dist) return false;

                    cellRect.Width += offsetX;
                    cellRect.Y -= offsetY;
                    cellRect.Height += offsetY;
                    break;

                case StiAction.SizeRight:

                    if (cellRect.Width + offsetX < dist) return false;

                    cellRect.Width += offsetX;
                    break;

                case StiAction.SizeRightBottom:

                    if (cellRect.Width + offsetX < dist || cellRect.Height + offsetY < dist) return false;

                    cellRect.Width += offsetX;
                    cellRect.Height += offsetY;
                    break;

                case StiAction.SizeBottom:

                    if (cellRect.Height + offsetY < dist) return false;

                    cellRect.Height += offsetY;
                    break;

                case StiAction.SizeLeftBottom:

                    if (cellRect.X - offsetX > cellRect.Right - dist || cellRect.Height + offsetY < dist) return false;

                    cellRect.X -= offsetX;
                    cellRect.Width += offsetX;
                    cellRect.Height += offsetY;
                    break;
            }

            return true;
        }
        #endregion

        #region Properties
        public bool IsControlKeyPressed => (Control.ModifierKeys & Keys.Control) > 0;
        #endregion

        #region Fields
        private IStiDesignerBase designer = null;
        private bool altKeyMode = false;
        private Hashtable storedLocations = null;
        private Point storedPosition = Point.Empty;
        private RectangleD storedSelectedRect = RectangleD.Empty;
        private double mouseX = 0;
        private double mouseY = 0;
        #endregion

        public StiTableHelper(IStiDesignerBase designer, bool altKeyMode)
        {
            this.designer = designer;
            this.altKeyMode = altKeyMode;
        }
    }
}
