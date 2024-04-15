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
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Stimulsoft.Report.Chart
{
    public class StiStyleCoreXF18 : StiStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
		/// </summary>
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}18";
        #endregion

		#region Methods
		public override StiBrush GetColumnBrush(Color color)
		{
			var style = HatchStyle.Cross;

			if (color == Color.White)
			{
				style = HatchStyle.BackwardDiagonal;
			}
			else if (color == Color.Black)
			{
				style = HatchStyle.DottedGrid;
			}
			else if (color == Color.Silver)
			{
				style = HatchStyle.Horizontal;
			}
			else if (color == Color.Red)
			{
				style = HatchStyle.Percent25;
			}
			else if (color == Color.Green)
			{
				style = HatchStyle.OutlinedDiamond;
			}
			else if (color == Color.Blue)
			{
				style = HatchStyle.ForwardDiagonal;
			}

			return new StiHatchBrush(style, Color.Black, Color.White);
		}
		#endregion

		#region Properties
        public override Color[] StyleColors { get; } = 
        {
            Color.White,
            Color.Black,
            Color.Silver,
            Color.Red,
            Color.Green,
            Color.Blue
        };

        public override Color BasicStyleColor => Color.White;

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle18;
        #endregion
	}
}