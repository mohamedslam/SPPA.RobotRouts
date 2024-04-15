#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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

using Stimulsoft.Report.Units;
using System;

namespace Stimulsoft.Report.Labels
{
    public sealed class StiLabelInfo : ICloneable
	{
		#region ICloneable
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion

		#region Properties
	    /// <summary>
		/// Gets or sets name of manufacturer.
		/// </summary>
		public string Manufacturer { get; }

	    /// <summary>
		/// Gets or sets name of label.
		/// </summary>
		public string LabelName { get; }

	    /// <summary>
		/// Gets or sets width of label in centimeters.
		/// </summary>
		public double LabelWidth { get; }

	    /// <summary>
		/// Gets or sets height of label in centimeters.
		/// </summary>		
		public double LabelHeight { get; }

	    /// <summary>
		/// Gets or sets horizontal gap of label in centimeters.
		/// </summary>
		public double HorizontalGap { get; }

	    /// <summary>
		/// Gets or sets vertical gap of label in centimeters.
		/// </summary>
		public double VerticalGap { get; }

	    /// <summary>
		/// Gets or sets left margin of label page.
		/// </summary>
		public double LeftMargin { get; }

	    /// <summary>
		/// Gets or sets top margin of label page.
		/// </summary>
		public double TopMargin { get; }

	    /// <summary>
		/// Gets or sets page width.
		/// </summary>
		public double PaperWidth { get; }

	    /// <summary>
		/// Gets or sets page height.
		/// </summary>
		public double PaperHeight { get; }

	    /// <summary>
		/// Gets or sets number of rows.
		/// </summary>
		public int NumberOfRows { get; }

	    /// <summary>
		/// Gets or sets number of columns.
		/// </summary>
		public int NumberOfColumns { get; }


	    public StiUnit Unit { get; set; }
	    #endregion

		#region Methods
		private string ConvertFromInch(double value)
		{
			if (!(Unit is StiInchesUnit))
			{
				value = (new StiInchesUnit()).ConvertToHInches(value);
				value = Unit.ConvertFromHInches(value);
			}
			value = Math.Round(value, 2);
			return value.ToString();
		}

		public override string ToString()
		{
			var widthStr = ConvertFromInch(LabelWidth);
			var heightStr = ConvertFromInch(LabelHeight);

			return $"{LabelName} ({widthStr}x{heightStr})";
		}
		#endregion

		public StiLabelInfo(
			string manufacturer, string labelName, 
			double labelWidth, double labelHeight, 
			double horizontalGap, double verticalGap,
			double leftMargin, double topMargin,
			double paperWidth, double paperHeight,
			int numberOfRows, int numberOfColumns)
		{
			this.Manufacturer = manufacturer;
			this.LabelName = labelName;
			this.LabelWidth = labelWidth;
			this.LabelHeight = labelHeight;			
			this.HorizontalGap = horizontalGap;
			this.VerticalGap = verticalGap;
			this.LeftMargin = leftMargin;
			this.TopMargin = topMargin;
			this.PaperWidth = paperWidth;
			this.PaperHeight = paperHeight;
			this.NumberOfColumns = numberOfColumns;
			this.NumberOfRows = numberOfRows;
		}
	}
}
