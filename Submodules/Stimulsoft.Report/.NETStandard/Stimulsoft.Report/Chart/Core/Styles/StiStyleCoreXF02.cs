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
    public class StiStyleCoreXF02 : StiStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
		/// </summary>
        public override string LocalizedName => $"{Loc.Get("Chart", "Style")}02";
        #endregion

        #region Properties
        public override Color BasicStyleColor => Color.WhiteSmoke;

        public override Color[] StyleColors { get; } = 
        {
            StiColor.Get("8091A5"),
            StiColor.Get("aB7B7A"),
            StiColor.Get("9BA781"),
            StiColor.Get("90859D"),
            StiColor.Get("79A1AD"),
            StiColor.Get("BD987A"),
            StiColor.Get("8BA4C2"),
            StiColor.Get("CA8482"),
            StiColor.Get("B3C68D"),
            StiColor.Get("A292B6")
        };
        
        public override StiChartStyleId StyleId => StiChartStyleId.StiStyle02;
        #endregion

		#region Methods
		public override StiBrush GetColumnBrush(Color color)
		{
			return new StiGlareBrush(color, StiColorUtils.Light(color, 50), 0);
		}
		#endregion
	}
}