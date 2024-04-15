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
using System.Collections.Generic;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using System.Drawing;

namespace Stimulsoft.Report.CrossTab.Core
{
	public class StiGrid
	{
	    #region Properties
        public decimal MaxWidth { get; set; } = 10m;

	    public decimal MaxHeight { get; set; } = 10m;
	    
        private double GridSize => Report.Info.GridSize;

        public StiReport Report { get; set; }

	    public Hashtable Fields { get; } = new Hashtable();
        
	    public bool DesignTime { get; set; }
        
	    public decimal[] Widths { get; set; }
        
	    public decimal[] Heights { get; set; }
        
	    public decimal[] CoordX { get; private set; }
        
	    public decimal[] CoordY { get; private set; }

	    public StiCell[][] Cells { get; set; }

		public bool AlignToGrid = true;

        
	    public int RowCount
		{
			get
			{
				return Heights.Length;
			}
			set
			{
				Init(Widths.Length, value);
			}
		}		


		public int ColCount
		{
			get
			{
				return Widths.Length;
			}
			set
			{
				Init(value, Heights.Length);
			}
		}
        #endregion

		#region Methods
		public void SetTextOfCell(int x, int y, string value)
		{
            Cells[x][y].Text = value;
		}

		private decimal Align(decimal value)
		{
			return StiAlignValue.AlignToMaxGrid(value, (decimal)GridSize, AlignToGrid);
		}
		
		private decimal GetCellTotalWidth(int cellX, int width)
		{
			var totalWidth = 0m;
			for (int x = cellX; x < cellX + width; x ++)
			{
                if (x < Widths.Length)
    				totalWidth += Widths[x];
			}
			return totalWidth;
		}

		private decimal GetCellTotalHeight(int cellY, int height)
		{
			var totalHeight = 0m;
			for (int y = cellY; y < cellY + height; y ++)
			{
				totalHeight += Heights[y];
			}
			return totalHeight;
		}

	    public void DoAutoSize()
	    {
	        #region Calculate size
	        for (int colIndex = 0; colIndex < ColCount; colIndex++)
	        {
	            for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
	            {
	                StiCell cell = Cells[colIndex][rowIndex];
	                if (cell.Field != null && cell.ParentCell == cell)
	                {
	                    var strMaxWidth = 10000d;
	                    if (cell.Field.MaxSize.Width != 0)
	                    {
                            strMaxWidth = GetFieldWidth(cell.Field, cell.Field.Report.Unit.ConvertToHInches(cell.Field.MaxSize.Width));
                        }

                        if (cell.Field.MaxSize.Height != 0 && (cell.Field.Angle == 90f || cell.Field.Angle == 270f))
                        {
                            strMaxWidth = GetFieldHeight(cell.Field, cell.Field.Report.Unit.ConvertToHInches(cell.Field.MaxSize.Height));
                        }

	                    StiExpression storedText = cell.Field.Text;
	                    cell.Field.Text = cell.Text;

	                    SizeD size = SizeD.Empty;
	                    if (cell.Field.Report != null && cell.Field.Report.IsWpf)
	                    {
                            size = StiWpfTextRender.MeasureString(strMaxWidth, cell.Field);

	                        if (cell.Field.CheckAllowHtmlTags())
	                        {
	                            size.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorWpfWithHtmlInCrossTab;
	                        }
                        }
	                    else if (cell.Field.TextQuality == StiTextQuality.Wysiwyg || cell.Field.CheckAllowHtmlTags())
	                    {
	                        size = StiWysiwygTextRender.MeasureString(strMaxWidth, cell.Field.Font, cell.Field);
	                    }
	                    else if (cell.Field.TextQuality == StiTextQuality.Typographic)
	                    {
	                        size = StiTypographicTextRender.MeasureString(strMaxWidth, cell.Field.Font, cell.Field);
	                    }
	                    else
	                    {
	                        size = StiStandardTextRenderer.MeasureString(strMaxWidth, cell.Field.Font, cell.Field);
	                    }

	                    cell.Field.Text = storedText;

	                    size.Width += cell.Field.Margins.Left + cell.Field.Margins.Right;
	                    size.Height += cell.Field.Margins.Top + cell.Field.Margins.Bottom;

                        var indicator = cell.Field.Indicator as StiIconSetIndicator;
                        if (indicator?.CustomIcon != null && indicator?.CustomIconSize != null)
                        {
                            size.Height = Math.Max(size.Height, indicator.CustomIconSize.Value.Height);
                            if (indicator.Alignment != ContentAlignment.BottomCenter && indicator.Alignment != ContentAlignment.MiddleCenter && indicator.Alignment != ContentAlignment.TopCenter)
                            {
                                size.Width += indicator.CustomIconSize.Value.Width;
                            }
                        }

	                    size.Width = Report.Unit.ConvertFromHInches(size.Width);
	                    size.Height = Report.Unit.ConvertFromHInches(size.Height);

	                    if (cell.Field.MaxSize.Width != 0d && cell.Field.MaxSize.Width < size.Width)
	                        size.Width = cell.Field.MaxSize.Width;

	                    if (cell.Field.MinSize.Width != 0d && cell.Field.MinSize.Width > size.Width)
	                        size.Width = cell.Field.MinSize.Width;

	                    cell.Size = size;
	                }
	                else
	                {
	                    cell.Size = SizeD.Empty;
	                    if (colIndex > 0 && cell.ParentCell == null && Cells[colIndex - 1][rowIndex].Width == 1)
	                        Cells[colIndex - 1][rowIndex].Width++;
	                }
	            }
	        }
	        #endregion

	        #region Measure cols
	        for (int colIndex = 0; colIndex < ColCount; colIndex++)
	        {
	            var width = (decimal)Report.Unit.ConvertFromHInches(StiOptions.Engine.CrossTab.DefaultWidth);

	            for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
	            {
	                StiCell cell = Cells[colIndex][rowIndex];
	                if (cell.Width > 1) continue;
	                var cellWidth = (decimal)cell.Size.Width;

	                if (cell.Field != null)
	                {
	                    if (cell.Field.MinSize.Width != 0d) cellWidth = Math.Max(cellWidth, (decimal)cell.Field.MinSize.Width);
	                    if (cell.Field.MaxSize.Width != 0d) cellWidth = Math.Min(cellWidth, (decimal)cell.Field.MaxSize.Width);
	                }

	                width = Math.Max(width, cellWidth);
	            }

	            width = Math.Min(width, MaxWidth);
	            width = Align(width);
	            Widths[colIndex] = width;
	        }
	        #endregion

	        #region Measure rows
	        for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
	        {
	            var height = (decimal)Report.Unit.ConvertFromHInches(StiOptions.Engine.CrossTab.DefaultHeight);

	            for (int colIndex = 0; colIndex < ColCount; colIndex++)
	            {
	                StiCell cell = Cells[colIndex][rowIndex];
	                if (cell.Height > 1) continue;

	                var cellHeight = (decimal)cell.Size.Height;

	                if (cell.Field != null)
	                {
	                    if (cell.Field.MinSize.Height != 0d) cellHeight = Math.Max(cellHeight, (decimal)cell.Field.MinSize.Height);
	                    if (cell.Field.MaxSize.Height != 0d) cellHeight = Math.Min(cellHeight, (decimal)cell.Field.MaxSize.Height);
	                }

	                height = Math.Max(height, cellHeight);
	            }

	            height = Math.Min(height, MaxHeight);
	            height = Align(height);

	            Heights[rowIndex] = height;
	        }
	        #endregion

	        #region Calculate size
	        for (int colIndex = 0; colIndex < ColCount; colIndex++)
	        {
	            for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
	            {
	                StiCell cell = Cells[colIndex][rowIndex];
	                if (cell.Field != null && cell.ParentCell == cell && (cell.Width > 1 || cell.Height > 1))
	                {
	                    var totalWidth = GetCellTotalWidth(colIndex, cell.Width);
	                    var totalHeight = GetCellTotalHeight(rowIndex, cell.Height);

	                    if ((decimal)cell.Size.Width > totalWidth && totalWidth > 0)
	                    {
	                        var factor = (decimal)cell.Size.Width / totalWidth;

	                        for (int index = colIndex; index < colIndex + cell.Width; index++)
	                        {
	                            if (index < Widths.Length) Widths[index] *= factor;
	                        }
	                    }

	                    if ((decimal)cell.Size.Height > totalHeight && totalHeight > 0)
	                    {
	                        var factor = (decimal) cell.Size.Height / totalHeight;

	                        for (int index = rowIndex; index < rowIndex + cell.Height; index++)
	                        {
	                            if (index < Heights.Length) Heights[index] *= factor;
	                        }
	                    }
	                }
	            }
	        }
	        #endregion

	        #region StiAlignValue.AlignToMaxGrid
	        for (int colIndex = 0; colIndex < ColCount; colIndex++)
	        {
	            Widths[colIndex] = StiAlignValue.AlignToMaxGrid(Widths[colIndex], (decimal)Report.Info.GridSize, AlignToGrid);
	        }

	        for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
	        {
	            Heights[rowIndex] = StiAlignValue.AlignToMaxGrid(Heights[rowIndex], (decimal)Report.Info.GridSize, AlignToGrid);
	        }
	        #endregion

	        #region Calculate x positions
	        var posX = 0m;
	        for (int colIndex = 0; colIndex < ColCount; colIndex++)
	        {
	            CoordX[colIndex] = posX;
	            posX += Widths[colIndex];
	        }
	        #endregion

	        #region Calculate y positions
	        var posY = 0m;
	        for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
	        {
	            CoordY[rowIndex] = posY;
	            posY += Heights[rowIndex];
	        }
	        #endregion
	    }

        private double GetFieldWidth(StiCrossField field, double value)
        {
            var rect = new RectangleD(0, 0, value, field.Height);
            rect = field.ConvertTextMargins(rect, false);
            rect = field.ConvertTextBorders(rect, false);
            return rect.Width;
        }

        private double GetFieldHeight(StiCrossField field, double value)
        {
            var rect = new RectangleD(0, 0, field.Height, value);
            rect = field.ConvertTextMargins(rect, false);
            rect = field.ConvertTextBorders(rect, false);
            return rect.Height;
        }

	    public StiCell SetCell(int cellX, int cellY, int cellWidth, int cellHeight, object text, object value,
            StiCrossField field, bool isNumeric, object hyperlink, object toolTip, object tag, Dictionary<string, object> drillDownParameters, StiCellType type = StiCellType.Cell)
        {
            return SetCell(cellX, cellY, cellWidth, cellHeight, 0, text, value, field, isNumeric, hyperlink, toolTip, tag, drillDownParameters, -1, null, null, false, type);
        }

		public StiCell SetCell(int cellX, int cellY, int cellWidth, int cellHeight, int index, object text, object value, 
			StiCrossField field, bool isNumeric, object hyperlink, object toolTip, object tag, Dictionary<string, object> drillDownParameters, int level, String parentGuid, String guid, bool keepMergedCellsTogether, StiCellType type = StiCellType.Cell)
		{
			StiCell parent = Cells[cellX][cellY];
            var originalField = field;
            if (!DesignTime)field = field.Clone() as StiCrossField;

			#region Process Conditions
			object resTag = field.TagValue;
			object resToolTip = field.ToolTipValue;
			object resHyperlinkValue = field.HyperlinkValue;
			string resTextValue = field.TextValue;

			StiValueEventArgs e = new StiValueEventArgs(value);
			field.TagValue = tag;
			field.ToolTipValue = toolTip;
			field.HyperlinkValue = hyperlink;

		    var crossSummary = field as StiCrossSummary;
            if (crossSummary != null)
            {
                if (CellExists(0, cellY))
                    crossSummary.CrossRowValue = Cells[0][cellY].Value as string;

                if (CellExists(cellX, 1))
                    crossSummary.CrossColumnValue = Cells[cellX][1].Value as string;
            }

			field.InvokeTextProcess(field, e);

			var crossHeader = field as StiCrossHeader;
			if (!string.IsNullOrEmpty(crossHeader?.ExpandExpression))
            {
				Report.Engine.LastInvokeTextProcessValueEventArgsValue = e.Value;
				Report.Engine.LastInvokeTextProcessIndexEventArgsValue = index + 1;
				try
				{
					var exp = crossHeader.ExpandExpression;
					exp = exp != null ? (!(exp.StartsWith("{") || exp.EndsWith("}")) ? $"{{{exp}}}" : exp) : "";
					var result = StiParser.ParseTextValue(exp, field);
					crossHeader.IsExpanded = true.Equals(result);
				}
				catch { }
			}

			field.TagValue = resTag;
			field.ToolTipValue = resToolTip;
			field.HyperlinkValue = resHyperlinkValue;
            field.OriginalValue = value;

            if (crossSummary != null)
            {
                crossSummary.CrossRowValue = null;
                crossSummary.CrossColumnValue = null;
            }
            if (field.TextValue != resTextValue)
                text = field.TextValue;
            #endregion

            if (!StiOptions.Engine.CrossTab.MemoryOptimization || (text != null && text.ToString() != ""))
                originalField = field;

			StiCellType mainType = type;
			switch (type)
            {
				case StiCellType.HeaderCol:
					mainType = StiCellType.HeaderColMain;
					break;

				case StiCellType.HeaderColTotal:
					mainType = StiCellType.HeaderColTotalMain;
					break;

				case StiCellType.HeaderRow:
					mainType = StiCellType.HeaderRowMain;
					break;

				case StiCellType.HeaderRowTotal:
					mainType = StiCellType.HeaderRowTotalMain;
					break;

				case StiCellType.CornerCol:
					mainType = StiCellType.CornerColMain;
			        break;

				case StiCellType.CornerRow:
					mainType = StiCellType.CornerRowMain;
					break;

				case StiCellType.LeftTopLine:
					mainType = StiCellType.LeftTopLineMain;
					break;

				case StiCellType.RightTopLine:
					mainType = StiCellType.RightTopLineMain;
					break;
			}

            #region Set cell formats for cells rectangle
            for (int x = cellX; x < (cellX + cellWidth); x++)
		    for (int y = cellY; y < (cellY + cellHeight); y++)
		    {
		        if (text != null) Cells[x][y].Text = text.ToString();
		        Cells[x][y].ParentCell = parent;
		        Cells[x][y].Value = value;
		        Cells[x][y].Width = cellWidth - (x - cellX);
		        Cells[x][y].Height = cellHeight - (y - cellY);
		        Cells[x][y].Field = originalField;
		        Cells[x][y].IsNumeric = isNumeric;
		        Cells[x][y].HyperlinkValue = hyperlink;
		        Cells[x][y].ToolTipValue = toolTip;
		        Cells[x][y].TagValue = tag;
		        Cells[x][y].DrillDownParameters = drillDownParameters;
		        Cells[x][y].Level = level;
                Cells[x][y].ParentGuid = parentGuid;
                Cells[x][y].Guid = guid;
                Cells[x][y].KeepMergedCellsTogether = keepMergedCellsTogether;
				Cells[x][y].CellType = type;
			}
			#endregion

			Cells[cellX][cellY].CellType = mainType;

		    if (DesignTime && Fields[field] == null)
                Fields[field] = new Point(cellX, cellY);

		    return Cells[cellX][cellY];
		}

	    private bool CellExists(int cellX, int cellY)
	    {
	        return Cells.GetLength(0) > cellX && Cells[cellX].GetLength(0) > cellY;
	    }

		public void SetCellField(int cellX, int cellY, StiCrossField field)
		{
		    if (Cells[cellX][cellY] != null)
		        Cells[cellX][cellY].Field = field;
		}

		public void Init(int colCount, int rowCount)
		{
            if (StiOptions.Engine.CrossTab.AllowGCCollect && (colCount > 1000 || rowCount > 1000))
            {
                GC.Collect();

                if (StiOptions.Engine.AllowWaitForPendingFinalizers)
                    GC.WaitForPendingFinalizers();

                GC.Collect();
            }

			Widths = new decimal[colCount];
            Heights = new decimal[rowCount];
            CoordX = new decimal[colCount];
            CoordY = new decimal[rowCount];
			
			for (int index = 0; index < colCount; index ++)Widths[index] = 10;
			for (int index = 0; index < rowCount; index ++)Heights[index] = 10;

			Cells = new StiCell[colCount][];
			for (int colIndex = 0; colIndex < colCount; colIndex++)			
			{
				Cells[colIndex] = new StiCell[rowCount];

				for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
				{
					Cells[colIndex][rowIndex] = new StiCell();
				}
			}
		}
		#endregion
	}
}
