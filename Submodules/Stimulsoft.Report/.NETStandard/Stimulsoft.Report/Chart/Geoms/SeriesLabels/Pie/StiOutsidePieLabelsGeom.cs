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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsidePieLabelsGeom : StiCenterPieLabelsGeom
    {
        #region Properties
        public Color LineColor { get; }

        public PointF LabelPoint { get; }

        public PointF StartPoint { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            base.Draw(context);

            var outsidePieLabels = SeriesLabels as IStiOutsidePieLabels;
            if (!outsidePieLabels.DrawBorder)return;

            var borderPen = new StiPenGeom(LineColor);
            var chart = this.Series.Chart as StiChart;

            context.PushSmoothingModeToAntiAlias();

            if (chart.IsAnimation)
            {
                var animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, StiChartHelper.GlobalDurationElement);
                context.DrawAnimationLines(borderPen, new[] { LabelPoint, StartPoint }, animation);
            }

            else
            {
                context.DrawLine(borderPen, LabelPoint.X, LabelPoint.Y, StartPoint.X, StartPoint.Y);
            }

            context.PopSmoothingMode();
        }
        #endregion

        public StiOutsidePieLabelsGeom(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value, RectangleF clientRectangle, string text,
            StiBrush seriesBrush, StiBrush labelBrush, StiBrush seriesLabelsBrush, Color borderColor, Color seriesBorderColor,
            StiRotationMode rotationMode, RectangleF labelRect, float angleToUse, Color lineColor, 
            PointF labelPoint, PointF startPoint)
            : base(seriesLabels, series, index, value, clientRectangle, text,
                seriesBrush, labelBrush, seriesLabelsBrush, borderColor, seriesBorderColor, 
                rotationMode, labelRect, angleToUse, null)
        {
            this.LineColor = lineColor;
            this.LabelPoint = labelPoint;
            this.StartPoint = startPoint;
        }
    }
}
