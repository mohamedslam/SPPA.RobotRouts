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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiStackedAreaSeriesCoreXF : StiStackedLineSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiStackedAreaSeries areaSeries = this.Series as IStiStackedAreaSeries;

            if (areaSeries.AllowApplyStyle)
            {
                areaSeries.Brush = style.Core.GetAreaBrush(color);
            }
        }
        #endregion

        #region Methods
        protected override void RenderAreas(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
            if (pointsInfo.PointsStart == null || pointsInfo.PointsStart.Length <= 1) return;
            if (pointsInfo.PointsEnd == null || pointsInfo.PointsEnd.Length <= 1) return;

            StiStackedAreaSeriesGeom seriesGeom = new StiStackedAreaSeriesGeom(geom, pointsInfo, this.Series);

            if (seriesGeom != null)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesGeom);
            }
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            IStiStackedAreaSeries areaSeries = this.Series as IStiStackedAreaSeries;

            StiBrush brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return areaSeries.Brush;
            return brush;
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
                return StiLocalization.Get("Chart", "StackedArea");
            }
        }
        #endregion        

        public StiStackedAreaSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
	}
}
