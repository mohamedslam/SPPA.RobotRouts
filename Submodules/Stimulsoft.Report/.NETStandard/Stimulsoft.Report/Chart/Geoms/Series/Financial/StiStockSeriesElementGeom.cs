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
using Stimulsoft.Base;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiStockSeriesElementGeom : StiFinancialSeriesElementGeom
    {
        #region Properties
        private Color color;
        public Color Color
        {
            get
            {
                return color;
            }
        }

        private TimeSpan beginTime;
        public TimeSpan BeginTime
        {
            get
            {
                return beginTime;
            }
        }
        #endregion

        #region Methods
        public override void Draw(StiContext context)
        {
            var stockArea = this.AreaGeom.Area as IStiStockArea;
            var stockSeries = this.Series as IStiStockSeries;
            
            float lineWidht = stockSeries.LineWidth * context.Options.Zoom;
            float width = (float)stockArea.XAxis.Info.Dpi / 3;

            var pen = new StiPenGeom(color, lineWidht)
            {
                PenStyle = stockSeries.LineStyle
            };

            var chart = this.Series.Chart as StiChart;

            if (chart.IsAnimation)
            {
                var animationOpacity = new StiOpacityAnimation(StiChartHelper.GlobalDurationElement, beginTime);

                if (stockSeries.ShowShadow)
                {
                    float lineWidgtShadow = lineWidht + 0.5f * context.Options.Zoom;
                    var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), lineWidgtShadow)
                    {
                        PenStyle = stockSeries.LineStyle
                    };

                    var pointsShadow1 = new PointF[] { new PointF(PositionX, High), new PointF(PositionX, Low) };
                    var pointsShadow2 = new PointF[] { new PointF(PositionX - width, Open), new PointF(PositionX - lineWidgtShadow / 2, Open) };
                    var pointsShadow3 = new PointF[] { new PointF(PositionX + width, Close), new PointF(PositionX + lineWidgtShadow / 2, Close) };

                    context.DrawAnimationLines(penShadow, pointsShadow1, animationOpacity);
                    context.DrawAnimationLines(penShadow, pointsShadow2, animationOpacity);
                    context.DrawAnimationLines(penShadow, pointsShadow3, animationOpacity);
                }

                var points1 = new PointF[] { new PointF(PositionX, High), new PointF(PositionX, Low) };
                var points2 = new PointF[] { new PointF(PositionX - width, Open), new PointF(PositionX, Open) };
                var points3 = new PointF[] { new PointF(PositionX + width, Close), new PointF(PositionX, Close) };

                context.DrawAnimationLines(pen, points1, animationOpacity);
                context.DrawAnimationLines(pen, points2, animationOpacity);
                context.DrawAnimationLines(pen, points3, animationOpacity);
            }
            else
            {
                if (stockSeries.ShowShadow)
                {
                    float lineWidgtShadow = lineWidht + 0.5f * context.Options.Zoom;
                    var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), lineWidgtShadow)
                    {
                        PenStyle = stockSeries.LineStyle
                    };

                    context.PushTranslateTransform(lineWidht, lineWidht);
                    context.DrawLine(penShadow, PositionX, High, PositionX, Low);
                    context.DrawLine(penShadow, PositionX - width, Open, PositionX - lineWidgtShadow / 2, Open);
                    context.DrawLine(penShadow, PositionX + width, Close, PositionX + lineWidgtShadow / 2, Close);
                    context.PopTransform();
                }

                context.DrawLine(pen, PositionX, High, PositionX, Low);
                context.DrawLine(pen, PositionX - width, Open, PositionX, Open);
                context.DrawLine(pen, PositionX + width, Close, PositionX, Close);
            }
        }
        #endregion

        public StiStockSeriesElementGeom(StiAreaGeom areaGeom, IStiSeries series, RectangleF clientRectangle,
            float open, float close, float high, float low, float positionX, int index, Color color, TimeSpan beginTime)
            : base(areaGeom, series, clientRectangle, open, close, high, low, positionX, index)
        {
            this.color = color;

            this.beginTime = beginTime;
        }
    }
}
