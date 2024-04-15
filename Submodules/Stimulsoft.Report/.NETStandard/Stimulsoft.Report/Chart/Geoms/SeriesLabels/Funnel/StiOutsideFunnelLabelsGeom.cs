#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideFunnelLabelsGeom : StiCenterFunnelLabelsGeom
    {
        #region Properties
        public PointF StartPointLine { get; }

        public PointF EndPointLine { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var borderPen = new StiPenGeom(BorderColor);
            var chart = this.Series.Chart as StiChart;

            if (chart.IsAnimation)
            {
                var animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, StiChartHelper.GlobalDurationElement);
                context.DrawAnimationLines(borderPen, new[] { StartPointLine, EndPointLine }, animation);
            }

            else
            {
                context.DrawLine(borderPen, StartPointLine.X, StartPointLine.Y, EndPointLine.X, EndPointLine.Y);
            }

            base.Draw(context);            
        }
        #endregion

        public StiOutsideFunnelLabelsGeom(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value,
            RectangleF clientRectangle, string text,
            StiBrush seriesBrush, StiBrush labelBrush, Color borderColor, Color seriesBorderColor, RectangleF labelRect,
            PointF startPointLine, PointF endPointLine)
            : base(seriesLabels, series, index, value, clientRectangle, text, seriesBrush, labelBrush, borderColor, seriesBorderColor, labelRect, null)
        {
            this.StartPointLine = startPointLine;
            this.EndPointLine = endPointLine;
        }    
    }
}
