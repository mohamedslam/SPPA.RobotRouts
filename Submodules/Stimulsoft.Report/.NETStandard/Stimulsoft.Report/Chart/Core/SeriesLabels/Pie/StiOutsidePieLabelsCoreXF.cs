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
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsidePieLabelsCoreXF : StiCenterPieLabelsCoreXF
	{
        #region IStiApplyStyle
        public override void ApplyStyle(IStiChartStyle style)
        {
            base.ApplyStyle(style);

            if (this.SeriesLabels.AllowApplyStyle)
            {
                ((IStiOutsidePieLabels)this.SeriesLabels).LineColor = style.Core.SeriesLabelsLineColor;
            }
        }
        #endregion        

        #region Properties
        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.OutsidePie;
            }
        }
        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "LabelsOutside");
            }
        }
        #endregion

        #region Methods
        public Color GetLineColor(IStiSeries series, int colorIndex, int colorCount)
        {
            if (this.SeriesLabels.UseSeriesColor) return GetSeriesLabelColor(series, colorIndex, colorCount);
            return ((IStiOutsidePieLabels)this.SeriesLabels).LineColor;
        }
        #endregion

        public StiOutsidePieLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
  	}
}
