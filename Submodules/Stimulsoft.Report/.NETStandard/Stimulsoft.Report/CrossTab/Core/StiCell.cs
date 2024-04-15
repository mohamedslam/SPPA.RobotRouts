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
using System.Collections.Generic;
using System.Diagnostics;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.CrossTab.Core
{
    public class StiCell : ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            var cell = this.MemberwiseClone() as StiCell;
            cell.ParentCell = cell;
            return cell;
        }
        #endregion

        #region Properties
        public SizeD Size { get; set; }

        public bool IsChangeWidthForRightToLeft { get; set; }

        public bool IsNumeric { get; set; }

        public bool IsNegativeColor { get; set; }

        public bool IsImage { get; set; }

        public bool IsCrossSummary { get; set; }

        public StiCrossField Field { get; set; }
        
        public string Text { get; set; }
        
        public object HyperlinkValue { get; set; }

        public object ToolTipValue { get; set; }

        public object TagValue { get; set; }

        public StiCell ParentCell { get; set; }
        
        public object Value { get; set; }
        
        public int Width { get; set; }

        public int Height { get; set; }

        public int SummaryIndex { get; set; } = -1;

        public int Level { get; set; } = -1;
        
        public Dictionary<string, object> DrillDownParameters { get; set; }

        public string ParentGuid { get; set; }

        public string Guid { get; set; }

        public bool KeepMergedCellsTogether { get; set; }

        public bool FieldCloned { get; set; } = false;

        public StiCellType CellType = StiCellType.Cell;
        #endregion

        #region Methods
        public string GetComponentPlacement()
        {
            string placement;

            if (Field is StiCrossColumn)
                placement = StiOptions.Export.OptimizeDataOnlyMode ? "d" : "h";

            else if (Field is StiCrossHeader)
                placement = StiOptions.Export.OptimizeDataOnlyMode ? "d" : "h";

            else if (Field is StiCrossTitle)
                placement = "h";

            else
                placement = "d";

            return placement.Length > 0 && Field != null && Field.Parent != null
                ? $"{placement}.{Field.Parent.Name}"
                : placement;
        }

        public override string ToString()
        {
            return Text + " type:" + CellType;
        }
        #endregion

        public StiCell() : this(string.Empty, 0m, null)
		{
		}

		public StiCell(string text, decimal value, StiCrossField field) : 
			this (text, value, 1, 1, field)
		{
			IsNumeric = true;
		}

		public StiCell(string text, decimal value, int width, int height, StiCrossField field)
		{
			this.Text = text;
			this.Value = value;
			this.Width = width;
			this.Height = height;
			this.Field = field;
		}
	}
}
