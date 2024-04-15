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

namespace Stimulsoft.Report.Chart
{
    public class StiScatterSplineSeriesGeom : StiBaseLineSeriesGeom
    {
        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            for (int pointIndex = 0; pointIndex < (this.Points.Length - 1); pointIndex++)
            {
                var point1 = this.Points[pointIndex];
                var point2 = this.Points[pointIndex + 1];

                if (point1 == null || point2 == null)
                    continue;

                bool result = StiPointHelper.IsLineContainsPoint(point1.Value, point2.Value, StiMouseOverHelper.MouseOverLineDistance, new PointF(x, y));
                if (result)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var splineSeries = this.Series as IStiScatterSplineSeries;

            context.PushSmoothingModeToAntiAlias();

            var dashStyle = splineSeries.LineStyle;
            float scaledLineWidth = splineSeries.LineWidth * context.Options.Zoom;
            var animation = GetAnimation();

            var pen = new StiPenGeom(splineSeries.LineColor, splineSeries.LineWidth)
            {
                PenStyle = dashStyle
            };

            if (splineSeries.ShowShadow)
            {
                var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), scaledLineWidth + 0.5f * context.Options.Zoom);
                penShadow.PenStyle = dashStyle;

                context.PushTranslateTransform(scaledLineWidth, scaledLineWidth);
                StiNullableDrawing.DrawCurve(context, penShadow, Points, splineSeries.Tension, animation);
                context.PopTransform();
            }

            #region IsMouseOver
            if (Series.Core.IsMouseOver)
            {
                float lineWidth = splineSeries.LineWidth;
                float zoom = context.Options.Zoom;
                float pointSize = 11 + lineWidth;
                foreach (PointF? point in Points)
                {
                    if (point == null) continue;

                    var pointRect = new RectangleF(point.Value.X - pointSize / 2 * zoom, point.Value.Y - pointSize / 2 * zoom, pointSize * zoom, pointSize * zoom);
                    context.FillEllipse(StiMouseOverHelper.GetLineMouseOverColor(splineSeries.LineColor), pointRect, null);
                }

                var penMouseOver = new StiPenGeom(StiMouseOverHelper.GetLineMouseOverColor(splineSeries.LineColor), (4 + lineWidth) * context.Options.Zoom);
                penMouseOver.StartCap = StiPenLineCap.Round;
                penMouseOver.EndCap = StiPenLineCap.Round;
                StiNullableDrawing.DrawCurve(context, penMouseOver, Points, splineSeries.Tension, animation);
            }
            #endregion

            StiNullableDrawing.DrawCurve(context, pen, Points, splineSeries.Tension, animation);

            #region Draw Light
            if (scaledLineWidth >= 2 && splineSeries.Lighting)
            {
                float step = 0.5f * context.Options.Zoom;
                context.PushTranslateTransform(-step, -step);
                var penLight = new StiPenGeom(StiColorUtils.Light(splineSeries.LineColor, 70), 1);
                penLight.PenStyle = dashStyle;
                StiNullableDrawing.DrawCurve(context, penLight, Points, splineSeries.Tension, animation);

                context.PopTransform();
            }
            #endregion
            
            context.PopSmoothingMode();
        }
        #endregion

        public StiScatterSplineSeriesGeom(StiAreaGeom areaGeom, StiSeriesPointsInfo pointsInfo, IStiSeries series)
            : base(areaGeom, pointsInfo, series)
        {
        }
    }
}
