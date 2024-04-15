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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Units;
using System;

namespace Stimulsoft.Report.Labels
{
    public sealed class StiPaperLabelInfo : ICloneable
	{
		#region ICloneable
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion

		#region Properties
	    /// <summary>
		/// Gets or sets name of page.
		/// </summary>
		public string Name { get; }

	    /// <summary>
		/// Gets or sets orientation of page.
		/// </summary>
		public StiPageOrientation Orientation { get; }

	    /// <summary>
		/// Gets or sets width of label in centimeters.
		/// </summary>
		public double Width { get; }

	    /// <summary>
		/// Gets or sets height of label in centimeters.
		/// </summary>		
		public double Height { get; }

	    public StiUnit Unit { get; set; }
	    #endregion

		#region Methods
		private string ConvertFromCM(double value)
		{
			if (!(Unit is StiCentimetersUnit))
			{
				value = new StiCentimetersUnit().ConvertToHInches(value);
				value = Unit.ConvertFromHInches(value);
			}
			value = Math.Round(value, 2);
			return value.ToString();
		}

		public override string ToString()
		{			
			if (Width == 0 || Height == 0)return Name;

			var widthStr = ConvertFromCM(Width);
			var heightStr = ConvertFromCM(Height);

			var orientationStr = string.Empty;
			if (Orientation == StiPageOrientation.Landscape)
			{
				orientationStr = " Landscape";
				var temp = widthStr;
				widthStr = heightStr;
				heightStr = temp;
			}
			
			return $"{Name}{orientationStr} ({widthStr}x{heightStr})";
		}
		#endregion

		public StiPaperLabelInfo(
			string name, StiPageOrientation orientation,
			double width, double height)
		{
			this.Name = name;
			this.Orientation = orientation;

			this.Width = width;
			this.Height = height;
		}
	}
}
