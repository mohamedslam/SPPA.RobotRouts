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

using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiStyleCoreXF01 : StiStyleCoreXF
	{
		#region Properties.Localization
		/// <summary>
		/// Gets a localized name of style.
		/// </summary>
		public override string LocalizedName => $"{Loc.Get("Chart", "Style")}01";
        #endregion

        #region Properties
        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("C27535"),
            StiColor.Get("E78C41"),
            StiColor.Get("F8AA79"),
            StiColor.Get("FACBB4"),
            StiColor.Get("FDE6DC")
        };

        public override Color BasicStyleColor => Color.Wheat;

        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle01;

        public override bool SeriesShowBorder { get; } = true;
        #endregion

        #region Methods
        public override StiBrush GetColumnBrush(Color color)
		{
			return new StiGlareBrush(StiColorUtils.Dark(color, 50), color, 0);
		}
		#endregion
	}
}