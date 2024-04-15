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
    public class StiSteppedAreaSeriesCoreXF : StiSteppedLineSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiSteppedAreaSeries areaSeries = this.Series as IStiSteppedAreaSeries;

            if (areaSeries.AllowApplyStyle)
            {
                areaSeries.Brush = style.Core.GetAreaBrush(color);
            }
        }
        #endregion

        #region Methods
        protected override void RenderAreas(StiContext context, StiAreaGeom geom, StiSeriesPointsInfo pointsInfo)
        {
            IStiSteppedLineSeries lineSeries = this.Series as IStiSteppedLineSeries;
            if (pointsInfo.Points == null || pointsInfo.Points.Length == 0 || (pointsInfo.Points.Length == 1 && !lineSeries.PointAtCenter)) return;

            var seriesGeom = new StiSteppedAreaSeriesGeom(geom, pointsInfo, this.Series);

            if (seriesGeom != null)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesGeom);
            }
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            IStiSteppedAreaSeries areaSeries = this.Series as IStiSteppedAreaSeries;

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
                return StiLocalization.Get("Chart", "SteppedArea");
            }
        }
        #endregion        

        public StiSteppedAreaSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
	}
}
