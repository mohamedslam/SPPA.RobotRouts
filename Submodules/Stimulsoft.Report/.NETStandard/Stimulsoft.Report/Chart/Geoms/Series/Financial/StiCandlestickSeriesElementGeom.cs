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
using System.Collections.Generic;
using System.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiCandlestickSeriesElementGeom : StiFinancialSeriesElementGeom
    {
        #region Properties
        private StiBrush brush;
        public StiBrush Brush
        {
            get
            {
                return brush;
            }
        }

        private Color borderColor;
        public Color BorderColor
        {
            get
            {
                return borderColor;
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
            var candlestickArea = this.AreaGeom.Area as IStiCandlestickArea;
            var candlestickSeries = this.Series as IStiCandlestickSeries;

            float borderWidht = candlestickSeries.BorderWidth * context.Options.Zoom;
            float width = (float)candlestickArea.XAxis.Info.Dpi / 2;

            float open = this.Open;
            float close = this.Close;
            float high = this.High;
            float low = this.Low;
            float positionX = this.PositionX;

            var pen = new StiPenGeom(borderColor, borderWidht);
            if (close == open) open -= 1;

            var chart = this.Series.Chart as StiChart;

            if (chart.IsAnimation)
            {
                TimeSpan duration = StiChartHelper.GlobalDurationElement;

                var pointsOpen = new PointF[] { new PointF(positionX, high), new PointF(positionX, open) };
                var pointsClose = new PointF[] { new PointF(positionX, close), new PointF(positionX, low) };
                
                var path = new List<StiSegmentGeom>();

                float x1 = positionX - width / 2;
                float y1 = open;
                float x2 = x1 + width;
                float y2 = y1 + close - open;

                path.Add(new StiLineSegmentGeom(x1, y1, x2, y1));
                path.Add(new StiLineSegmentGeom(x2, y1, x2, y2));
                path.Add(new StiLineSegmentGeom(x2, y2, x1, y2));
                path.Add(new StiLineSegmentGeom(x1, y2, x1, y1));
                path.Add(new StiCloseFigureSegmentGeom());

                var animation = new StiOpacityAnimation(duration, beginTime);

                if (candlestickSeries.ShowShadow)
                {
                    float borderWidhtShadow = borderWidht + 0.5f * context.Options.Zoom;
                    StiPenGeom penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), borderWidhtShadow);

                    var pointsOpenShadow = new PointF[] { new PointF(positionX + borderWidhtShadow / 2, high + borderWidhtShadow / 2), new PointF(positionX + borderWidhtShadow / 2, open + borderWidhtShadow / 2) };
                    var pointsCloseShadow = new PointF[] { new PointF(positionX + borderWidhtShadow / 2, close + borderWidhtShadow / 2), new PointF(positionX + borderWidhtShadow / 2, low + borderWidhtShadow / 2) };

                    context.DrawAnimationLines(penShadow, pointsOpenShadow, animation);
                    context.DrawAnimationLines(penShadow, pointsCloseShadow, animation);

                    var pathShadow = new List<StiSegmentGeom>();
                    pathShadow.Add(new StiLineSegmentGeom(x1 + borderWidhtShadow / 2, y1 + borderWidhtShadow / 2, x2 + borderWidhtShadow / 2, y1 + borderWidhtShadow / 2));
                    pathShadow.Add(new StiLineSegmentGeom(x2 + borderWidhtShadow / 2, y1 + borderWidhtShadow / 2, x2 + borderWidhtShadow / 2, y2 + borderWidhtShadow / 2));
                    pathShadow.Add(new StiLineSegmentGeom(x2 + borderWidhtShadow / 2, y2 + borderWidhtShadow / 2, x1 + borderWidhtShadow / 2, y2 + borderWidhtShadow / 2));
                    pathShadow.Add(new StiLineSegmentGeom(x1 + borderWidhtShadow / 2, y2 + borderWidhtShadow / 2, x1 + borderWidhtShadow / 2, y1 + borderWidhtShadow / 2));
                    pathShadow.Add(new StiCloseFigureSegmentGeom());

                    context.FillDrawAnimationPath(null, penShadow, pathShadow, new RectangleF(x1 + borderWidhtShadow / 2, y1 + borderWidhtShadow / 2, width, close - open), null, animation, null);
                }

                context.DrawAnimationLines(pen, pointsOpen, animation);
                context.DrawAnimationLines(pen, pointsClose, animation);

                context.FillDrawAnimationPath(brush, pen, path, new RectangleF(x1, y1, width, close - open), this, animation, null);
            }
            else
            {
                if (candlestickSeries.ShowShadow)
                {
                    float borderWidhtShadow = borderWidht + 0.5f * context.Options.Zoom;
                    StiPenGeom penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), borderWidhtShadow);

                    context.PushTranslateTransform(borderWidht, borderWidht);
                    context.DrawLine(penShadow, positionX, close + borderWidhtShadow / 2, positionX, low + borderWidhtShadow / 2);
                    context.DrawLine(penShadow, positionX, high, positionX, open - borderWidhtShadow / 2);
                    context.DrawRectangle(penShadow, positionX - width / 2, open, width, close - open);
                    context.PopTransform();
                }

                context.DrawLine(pen, positionX, high, positionX, open);
                context.DrawLine(pen, positionX, close, positionX, low);
                context.FillRectangle(brush, positionX - width / 2, open, width, close - open, null);
                context.DrawRectangle(pen, positionX - width / 2, open, width, close - open);
            }
        }
        #endregion

        public StiCandlestickSeriesElementGeom(StiAreaGeom areaGeom, IStiSeries series, RectangleF clientRectangle,
            float bodyStart, float bodyEnd, float high, float low, float positionX, int index, StiBrush brush, Color borderColor, TimeSpan beginTime)
            : base(areaGeom, series, clientRectangle, bodyStart, bodyEnd, high, low, positionX, index)
        {
            this.brush = brush;

            this.borderColor = borderColor;

            this.beginTime = beginTime;
        }
    }
}
