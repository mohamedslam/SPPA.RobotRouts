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

using System;
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiBoxAndWhiskerSeriesElementGeom : StiCellGeom
    {
        #region Properties
        public StiBrush Brush { get; }

        public Color BorderColor { get; }

        public TimeSpan BeginTime { get; }

        public StiAreaGeom AreaGeom { get; }

        public double? Mean { get; }

        public float Maximum { get; }

        public float Minimum { get; }

        public float Median { get; }

        public float FirstQuartile { get; }

        public float ThirdQuartile { get; }

        public float PositionX { get; }

        public double[] Values { get; }

        public IStiSeries Series { get; }
        #endregion

        #region Methods
        public override void Draw(StiContext context)
        {
            var chart = this.Series.Chart as StiChart;
            var area = this.AreaGeom.Area as IStiBoxAndWhiskerArea;
            var pen = new StiPenGeom(BorderColor, GetSeriesBorderThickness());

            float width = (float)area.XAxis.Info.Dpi / 2;
            var widthWhisker = width / 3;
            var widthMarker = widthWhisker / 6;

            context.PushSmoothingModeToAntiAlias();

            if (chart.IsAnimation)
            {
                var animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, this.BeginTime);

                context.DrawAnimationLines(pen, new PointF[] { new PointF(this.PositionX - widthWhisker / 2, this.Maximum), new PointF(this.PositionX + widthWhisker / 2, this.Maximum) }, animation);
                context.DrawAnimationLines(pen, new PointF[] { new PointF(this.PositionX - widthWhisker / 2, this.Minimum), new PointF(this.PositionX + widthWhisker / 2, this.Minimum) }, animation);
                context.DrawAnimationLines(pen, new PointF[] { new PointF(this.PositionX, this.Maximum), new PointF(this.PositionX, this.ThirdQuartile) }, animation);
                context.DrawAnimationLines(pen, new PointF[] { new PointF(this.PositionX, this.FirstQuartile), new PointF(this.PositionX, this.Minimum) }, animation);

                context.DrawAnimationRectangle(Brush, pen, new RectangleF(PositionX - width / 2, this.ThirdQuartile, width, this.FirstQuartile - this.ThirdQuartile), null, animation);

                context.DrawAnimationLines(pen, new PointF[] { new PointF(this.PositionX - width / 2, this.Median), new PointF(this.PositionX + width / 2, this.Median) }, animation);

                if (Values != null)
                {
                    var index = 1;
                    foreach (var valuePoint in Values)
                    {
                        var animationPoint = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, this.BeginTime + new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / Values.Length * index));
                        context.FillDrawAnimationEllipse(null, pen, PositionX - widthMarker / 2, (float)valuePoint - widthMarker / 2, widthMarker, widthMarker, null, null, animationPoint);
                        index++;
                    }
                }

                if (this.Mean != null)
                {
                    var meanValue = (float)this.Mean.GetValueOrDefault();
                    context.DrawAnimationLines(pen, new PointF[] { new PointF(this.PositionX - widthMarker, meanValue - widthMarker), new PointF(this.PositionX + widthMarker, meanValue + widthMarker) }, animation);
                    context.DrawAnimationLines(pen, new PointF[] { new PointF(this.PositionX + widthMarker, meanValue - widthMarker), new PointF(this.PositionX - widthMarker, meanValue + widthMarker) }, animation);
                }
            }
            else
            {
                context.DrawLine(pen, this.PositionX - widthWhisker / 2, this.Maximum, this.PositionX + widthWhisker / 2, this.Maximum);
                context.DrawLine(pen, this.PositionX - widthWhisker / 2, this.Minimum, this.PositionX + widthWhisker / 2, this.Minimum);
                context.DrawLine(pen, this.PositionX, this.Maximum, this.PositionX, this.ThirdQuartile);
                context.DrawLine(pen, this.PositionX, this.FirstQuartile, this.PositionX, this.Minimum);

                context.FillRectangle(Brush, this.PositionX - width / 2, this.ThirdQuartile, width, this.FirstQuartile - this.ThirdQuartile, null);
                context.DrawRectangle(pen, this.PositionX - width / 2, this.ThirdQuartile, width, this.FirstQuartile - this.ThirdQuartile);

                context.DrawLine(pen, this.PositionX - width / 2, this.Median, this.PositionX + width / 2, this.Median);

                if (Values != null)
                {
                    foreach (var valuePoint in Values)
                    {                        
                        context.DrawEllipse(pen, new RectangleF(PositionX - widthMarker / 2, (float)valuePoint - widthMarker / 2, widthMarker, widthMarker));
                    }
                }

                if (this.Mean != null)
                {
                    var meanValue = (float)this.Mean.GetValueOrDefault();
                    context.DrawLine(pen, this.PositionX - widthMarker, meanValue - widthMarker, this.PositionX + widthMarker, meanValue + widthMarker);
                    context.DrawLine(pen, this.PositionX + widthMarker, meanValue - widthMarker, this.PositionX - widthMarker, meanValue + widthMarker);
                }
            }
                
            context.PopSmoothingMode();
        }

        private int GetSeriesBorderThickness()
        {
            if (Series is IStiSeriesBorderThickness seriesBorderThickness)
                return seriesBorderThickness.BorderThickness;

            return 1;
        }
        #endregion

        public StiBoxAndWhiskerSeriesElementGeom(StiAreaGeom areaGeom, IStiSeries series,
            float positionX, float minimum, float maximim, float firstQuartile, float thirdQuartile, float median,
            double[] values, double? mean, RectangleF clientRectangle, StiBrush brush, Color borderColor, TimeSpan beginTime)
            : base(clientRectangle)
        {
            this.Series = series;
            this.AreaGeom = areaGeom;

            this.PositionX = positionX;

            this.Minimum = minimum;
            this.Maximum = maximim;
            this.FirstQuartile = firstQuartile;
            this.ThirdQuartile = thirdQuartile;
            this.Median = median;
            this.Mean = mean;

            this.Values = values;

            this.Brush = brush;
            this.BorderColor = borderColor;

            this.BeginTime = beginTime;
        }
    }
}
