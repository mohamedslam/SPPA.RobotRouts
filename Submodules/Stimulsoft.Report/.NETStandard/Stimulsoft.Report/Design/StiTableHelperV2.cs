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
using Stimulsoft.Report.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Design
{
    public class StiTableHelperV2
    {
        #region Fields
        private IStiDesignerBase designer = null;
        private bool altKeyMode = false;
        private Hashtable storedLocations = null;
        private Point storedPosition = Point.Empty;
        private double mouseX;
        private double mouseY;
        private RectangleD storedSelectedRect = RectangleD.Empty;
        #endregion

        #region Properties
        public IStiTableCell SelectedCell { get; set; }

        public StiTable SelectedTable { get; set; }

        public bool IsSelectedCell => SelectedCell != null;

        public bool IsSelectedTable => SelectedTable != null;

        public bool IsAnySelected => SelectedTable != null || SelectedCell != null;
        #endregion

        #region Methods
        public void ProcessResizeCell()
        {
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

            if (invertX)
                offsetX = storedX - currentX;

            else
                offsetX = currentX - storedX;

            if (invertY)
                offsetY = storedY - currentY;

            else
                offsetY = currentY - storedY;

            var scale = StiScale.System;
            offsetX *= scale;
            offsetY *= scale;

            ResizeCell(offsetX, offsetY);
        }

        public void ResizeCell(double offsetX, double offsetY)
        {
            var tableCell = designer.SelectedComponentsOnPage.ToList().FirstOrDefault(c => c is IStiTableCell);
            if (tableCell != null && tableCell.Parent is StiTable table)
            {
                RestoreComponentsLocation(table);
                ResizeBasicCell(tableCell, offsetX, offsetY, designer.Report.Info.CurrentAction);
            }
        }

        private void ResizeBasicCell(StiComponent tableCell, double offsetX, double offsetY, StiAction action)
        {
            var dist = GetDistForResize(designer.Report.GetCurrentPage());

            var table = tableCell.Parent as StiTable;
            var changedComponentColumnList = new Dictionary<StiComponent, RectangleD>();
            var changedComponentRowList = new Dictionary<StiComponent, RectangleD>();

            var tableRect = GetRect(table);
            var cellRect = GetRect(tableCell);
            var originalRect = cellRect;

            var lockSizeLeft = (decimal)tableRect.Left == (decimal)cellRect.Left;
            var lockSizeTop = (decimal)tableRect.Top == (decimal)cellRect.Top;
            var lockSizeRight = (decimal)tableRect.Right == (decimal)cellRect.Right;
            var lockSizeBottom = (decimal)tableRect.Bottom == (decimal)cellRect.Bottom;

            switch (action)
            {
                case StiAction.SizeLeft:
                    if (cellRect.X - offsetX > cellRect.Right) return;

                    if (!lockSizeLeft)
                    {
                        cellRect.X -= offsetX;
                        cellRect.Width += offsetX;

                        if (!ResizeColumns(tableCell, originalRect.X + dist, offsetX, StiAction.SizeLeft, ref changedComponentColumnList)) return;

                        if (!ResizeColumns(tableCell, originalRect.X - dist, -offsetX, StiAction.SizeRight, ref changedComponentColumnList)) return;

                        SetPaintRectangle(changedComponentColumnList);
                    }
                    break;

                case StiAction.SizeLeftTop:

                    if (cellRect.X - offsetX > cellRect.Right || cellRect.Y - offsetY > cellRect.Bottom) return;

                    if (!lockSizeLeft)
                    {
                        cellRect.X -= offsetX;
                        cellRect.Width += offsetX;

                        if (!ResizeColumns(tableCell, originalRect.X - dist, -offsetX, StiAction.SizeRight, ref changedComponentColumnList)) return;

                        if (!ResizeColumns(tableCell, originalRect.X + dist, offsetX, StiAction.SizeLeft, ref changedComponentColumnList)) return;

                        SetPaintRectangle(changedComponentColumnList);
                    }

                    if (!lockSizeTop)
                    {
                        cellRect.Y -= offsetY;
                        cellRect.Height += offsetY;

                        if (!ResizeRows(tableCell, originalRect.Y - dist, -offsetY, StiAction.SizeBottom, ref changedComponentRowList)) return;

                        if (!ResizeRows(tableCell, originalRect.Y + dist, offsetY, StiAction.SizeTop, ref changedComponentRowList)) return;

                        SetPaintRectangle(changedComponentRowList);
                    }
                    break;

                case StiAction.SizeTop:

                    if (cellRect.Y - offsetY > cellRect.Bottom) return;

                    if (!lockSizeTop)
                    {
                        cellRect.Y -= offsetY;
                        cellRect.Height += offsetY;

                        if (!ResizeRows(tableCell, originalRect.Y - dist, -offsetY, StiAction.SizeBottom, ref changedComponentRowList)) return;

                        if (!ResizeRows(tableCell, originalRect.Y + dist, offsetY, StiAction.SizeTop, ref changedComponentRowList)) return;

                        SetPaintRectangle(changedComponentRowList);
                    }
                    break;

                case StiAction.SizeRightTop:

                    if (cellRect.Y - offsetY > cellRect.Bottom || cellRect.Width + offsetX <= 0) return;

                    if (!lockSizeTop)
                    {
                        cellRect.Y -= offsetY;
                        cellRect.Height += offsetY;

                        if (!ResizeRows(tableCell, originalRect.Y - dist, -offsetY, StiAction.SizeBottom, ref changedComponentRowList)) return;

                        if (!ResizeRows(tableCell, originalRect.Y + dist, offsetY, StiAction.SizeTop, ref changedComponentRowList)) return;

                        SetPaintRectangle(changedComponentRowList);
                    }

                    if (!lockSizeRight)
                    {
                        cellRect.Width += offsetX;

                        if (!ResizeColumns(tableCell, originalRect.Right - dist, offsetX, StiAction.SizeRight, ref changedComponentColumnList)) return;

                        if (!ResizeColumns(tableCell, originalRect.Right + dist, -offsetX, StiAction.SizeLeft, ref changedComponentColumnList)) return;

                        SetPaintRectangle(changedComponentColumnList);
                    }
                    break;

                case StiAction.SizeRight:

                    if (cellRect.Width + offsetX <= 0) return;

                    if (!lockSizeRight)
                    {
                        cellRect.Width += offsetX;

                        if (!ResizeColumns(tableCell, originalRect.Right - dist, offsetX, StiAction.SizeRight, ref changedComponentColumnList)) return;

                        if (!ResizeColumns(tableCell, originalRect.Right + dist, -offsetX, StiAction.SizeLeft, ref changedComponentColumnList)) return;

                        SetPaintRectangle(changedComponentColumnList);
                    }
                    break;

                case StiAction.SizeRightBottom:

                    if (cellRect.Width + offsetX <= 0 || cellRect.Height + offsetY <= 0) return;

                    if (!lockSizeRight)
                    {
                        cellRect.Width += offsetX;

                        if (!ResizeColumns(tableCell, originalRect.Right - dist, offsetX, StiAction.SizeRight, ref changedComponentColumnList)) return;

                        if (!ResizeColumns(tableCell, originalRect.Right + dist, -offsetX, StiAction.SizeLeft, ref changedComponentColumnList)) return;

                        SetPaintRectangle(changedComponentColumnList);
                    }

                    if (!lockSizeBottom)
                    {
                        cellRect.Height += offsetY;

                        if (!ResizeRows(tableCell, originalRect.Bottom - dist, offsetY, StiAction.SizeBottom, ref changedComponentRowList)) return;

                        if (!ResizeRows(tableCell, originalRect.Bottom + dist, -offsetY, StiAction.SizeTop, ref changedComponentRowList)) return;

                        SetPaintRectangle(changedComponentRowList);
                    }
                    break;

                case StiAction.SizeBottom:

                    if (cellRect.Height + offsetY <= 0) return;

                    if (!lockSizeBottom)
                    {
                        cellRect.Height += offsetY;

                        if (!ResizeRows(tableCell, originalRect.Bottom - dist, offsetY, StiAction.SizeBottom, ref changedComponentRowList)) return;

                        if (!ResizeRows(tableCell, originalRect.Bottom + dist, -offsetY, StiAction.SizeTop, ref changedComponentRowList)) return;

                        SetPaintRectangle(changedComponentRowList);
                    }
                    break;

                case StiAction.SizeLeftBottom:

                    if (cellRect.X - offsetX > cellRect.Right || cellRect.Height + offsetY <= 0) return;

                    if (!lockSizeLeft)
                    {
                        cellRect.X -= offsetX;
                        cellRect.Width += offsetX;

                        if (!ResizeColumns(tableCell, originalRect.X - dist, -offsetX, StiAction.SizeRight, ref changedComponentColumnList)) return;

                        if (!ResizeColumns(tableCell, originalRect.X + dist, offsetX, StiAction.SizeLeft, ref changedComponentColumnList)) return;

                        SetPaintRectangle(changedComponentColumnList);
                    }

                    if (!lockSizeBottom)
                    {
                        cellRect.Height += offsetY;

                        if (!ResizeRows(tableCell, originalRect.Bottom - dist, offsetY, StiAction.SizeBottom, ref changedComponentRowList)) return;

                        if (!ResizeRows(tableCell, originalRect.Bottom + dist, -offsetY, StiAction.SizeTop, ref changedComponentRowList)) return;

                        SetPaintRectangle(changedComponentRowList);
                    }
                    break;
            }

            tableCell.SetPaintRectangle(cellRect);
        }

        private void SetPaintRectangle(Dictionary<StiComponent, RectangleD> list)
        {
            foreach (var comp in list.Keys)
                comp.SetPaintRectangle(list[comp]);
        }

        private bool TryResizeCell(StiComponent tableCell, double offsetX, double offsetY, StiAction action, ref RectangleD rectangle)
        {
            return ResizeCell(tableCell, offsetX, offsetY, action, ref rectangle, false);
        }

        private bool ResizeCell(StiComponent tableCell, double offsetX, double offsetY, StiAction action, ref RectangleD rectangle, bool applyResize = true)
        {
            var cellRect = GetRect(tableCell);

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

                    if (cellRect.Width + offsetX <= dist) return false;

                    cellRect.Width += offsetX;
                    break;

                case StiAction.SizeRightBottom:

                    if (cellRect.Width + offsetX <= dist || cellRect.Height + offsetY <= dist) return false;

                    cellRect.Width += offsetX;
                    cellRect.Height += offsetY;
                    break;

                case StiAction.SizeBottom:

                    if (cellRect.Height + offsetY <= dist) return false;

                    cellRect.Height += offsetY;
                    break;

                case StiAction.SizeLeftBottom:

                    if (cellRect.X - offsetX > cellRect.Right - dist || cellRect.Height + offsetY <= dist) return false;

                    cellRect.X -= offsetX;
                    cellRect.Width += offsetX;
                    cellRect.Height += offsetY;
                    break;
            }

            rectangle = cellRect;

            if (applyResize)
                tableCell.SetPaintRectangle(cellRect);

            return true;
        }

        public bool ResizeColumns(StiComponent baseCell, double lineX, double offset, StiAction action, ref Dictionary<StiComponent, RectangleD> changedComponentList)
        {
            if (offset != 0)
            {
                var table = baseCell.Parent as StiTable;

                var dist = GetDistForResize(designer.Report.GetCurrentPage()) * 1.1;

                foreach (StiComponent tableCell in table.Components)
                {
                    if (changedComponentList.Keys.Contains(tableCell))
                        continue;
                    if (baseCell == tableCell)
                        continue;

                    var rect = GetRect(tableCell);

                    if (rect.Left < lineX && rect.Left + dist >= lineX ||
                        rect.Right > lineX && rect.Right - dist <= lineX)
                    {
                        var rectCell = new RectangleD();

                        if (!TryResizeCell(tableCell, offset, 0, action, ref rectCell))
                            return false;

                        changedComponentList.Add(tableCell, rectCell);
                    }
                }
            }

            return true;
        }

        public bool ResizeRows(StiComponent baseCell, double lineY, double offset, StiAction action, ref Dictionary<StiComponent, RectangleD> changedComponentList)
        {
            if (offset != 0)
            {
                var table = baseCell.Parent as StiTable;

                var dist = GetDistForResize(designer.Report.GetCurrentPage()) * 1.1;

                #region First Passage
                foreach (StiComponent tableCell in table.Components)
                {
                    if (changedComponentList.Keys.Contains(tableCell) || baseCell == tableCell) continue;

                    var rect = GetRect(tableCell);

                    if (rect.Top < lineY && rect.Top + dist >= lineY ||
                        rect.Bottom > lineY && rect.Bottom - dist <= lineY)
                    {
                        var rectCell = new RectangleD();

                        if (!TryResizeCell(tableCell, 0, offset, action, ref rectCell))
                            return false;

                        changedComponentList.Add(tableCell, rectCell);
                    }
                }
                #endregion
            }

            return true;
        }

        public static double GetDistForResize(StiComponent comp)
        {
            return comp.Page.Unit.ConvertFromHInches(4d);
        }

        private RectangleD GetRect(StiComponent comp)
        {
            return comp.GetPaintRectangle(false, false, false);
        }

        public void ProcessResizeTable(bool resizeHorizontally, bool resizeVertically)
        {
            var table = SelectedTable;
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

            RestoreComponentsLocation(table);

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

            ResizeTable(table, startX, startY, dpiX, dpiY, resizeHorizontally, resizeVertically, invertX, invertY);
        }

        public void ResizeTable(StiTable table, double startX, double startY, double dpiX, double dpiY,
            bool resizeHorizontally, bool resizeVertically, bool invertX, bool invertY)
        {
            var x = new Hashtable();
            var y = new Hashtable();

            foreach (StiComponent component in table.Components)
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
                    ? startY - (startY - rect.Y) * dpiY
                    : startY + (rect.Y - startY) * dpiY;

                rect = rect.Normalize();
                //rect = new RectangleD(Align(rect.X), Align(rect.Y), Align(rect.Width), Align(rect.Height));

                x[Round(originalRect.Left)] = rect.X;
                y[Round(originalRect.Top)] = rect.Y;
                x[Round(originalRect.Right)] = rect.Right;
                y[Round(originalRect.Bottom)] = rect.Bottom;
            }

            foreach (StiComponent component in table.Components)
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

                    component.SetPaintRectangle(new RectangleD(left, top, right - left, bottom - top));
                }
            }
        }

        public void RestoreComponentsLocation()
        {
            if (this.SelectedCell != null)
            {
                var table = ((StiComponent)this.SelectedCell).Parent as StiTable;
                this.RestoreComponentsLocation(table);
            }
        }

        public void RestoreComponentsLocation(StiTable table)
        {
            if (storedLocations == null) return;

            foreach (StiComponent comp in table.Components)
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

        private double ConvertValue(double value)
        {
            value = designer.Report.GetCurrentPage().Unit.ConvertFromHInches(
                value / designer.Report.GetCurrentPage().Zoom);

            value = StiAlignValue.AlignToGrid(value,
                designer.Report.GetCurrentPage().GridSize,
                designer.Report.Info.AlignToGrid && (!altKeyMode));

            return value;
        }

        public static bool IsAllowUseInTableMode(StiComponent comp)
        {
            if (comp is StiPage)
                return false;

            if (comp.Parent is IStiCrossTab)
                return false;

            return StiRestrictionsHelper.IsAllowChangePosition(comp);
        }

        public void SaveComponentsLocation(StiTable table)
        {
            if (table == null) return;

            if (storedLocations == null)
                storedLocations = new Hashtable();
            else
                storedLocations.Clear();

            foreach (StiComponent comp in table.Components)
            {
                if (!IsAllowUseInTableMode(comp)) continue;

                storedLocations[comp] = comp.ClientRectangle;
            }
        }

        public void SaveCursorLocation(int eX, int eY)
        {
            storedPosition = Cursor.Position;

            mouseX = designer.XToPage(eX);
            mouseY = designer.YToPage(eY);
        }

        public void SaveSelectedRectangle()
        {
            storedSelectedRect = GetSelectedRectangle(designer);
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

            if (designer.Report.GetCurrentPage() is IStiDashboard)
                return Math.Round((decimal)value, 0);

            var unit = designer.Report.GetCurrentPage().Unit;

            if (unit is StiCentimetersUnit || unit is StiInchesUnit)
                return Math.Round((decimal)value, 1);

            return Math.Round((decimal)value, 0);
        }

        public void Dispose()
        {
            SelectedCell = null;
            storedLocations?.Clear();
            storedLocations = null;
        }
        #endregion

        public StiTableHelperV2(IStiDesignerBase designer, bool altKeyMode)
        {
            this.designer = designer;
            this.altKeyMode = altKeyMode;
        }
    }
}