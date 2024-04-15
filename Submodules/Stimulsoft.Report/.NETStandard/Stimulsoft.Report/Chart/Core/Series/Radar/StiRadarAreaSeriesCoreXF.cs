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
using System.Collections.Generic;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiRadarAreaSeriesCoreXF : StiRadarSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiRadarAreaSeries radarSeries = this.Series as IStiRadarAreaSeries;

            if (radarSeries.AllowApplyStyle)
            {
                radarSeries.Brush = style.Core.GetAreaBrush(color);
                radarSeries.LineColor = color;
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
                return StiLocalization.Get("Chart", "RadarArea");
            }
        }
        #endregion  
      
        #region Methods
        public override void RenderLines(StiContext context, IStiRadarSeries series, StiSeriesPointsInfo pointsInfo, StiAreaGeom geom)
        {
            var radarGeom = new StiLineSeriesGeom(geom, pointsInfo, series);

            if (geom != null)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(radarGeom);
            }
        }

        public override void RenderAreas(StiContext context, IStiRadarSeries series, StiSeriesPointsInfo pointsInfo, StiAreaGeom geom)
        {
            var radarGeom = new StiRadarAreaSeriesGeom(series, pointsInfo);
            if (geom != null)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(radarGeom);
            }
        }
        #endregion

        public StiRadarAreaSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
	}
}
