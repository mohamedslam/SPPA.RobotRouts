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
    public class StiTrendCurveGeom : StiCellGeom
    {
        #region Properties
        private PointF?[] points;
        public PointF?[] Points
        {
            get
            {
                return points;
            }
        }

        private IStiTrendLine trendLine;
        public IStiTrendLine TrendLine
        {
            get
            {
                return trendLine;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var pointsNull = new PointF[points.Length];
            for (int index = 0; index < points.Length; index++)
            {
                pointsNull[index] = points[index].Value;
            }

            var lineColor = this.trendLine.LineColor;
            float lineWidth = this.trendLine.LineWidth;
            var style = this.trendLine.LineStyle;
            bool showShadow = this.trendLine.ShowShadow;

            float scaledLineWidth = lineWidth * context.Options.Zoom;

            context.PushSmoothingModeToAntiAlias();

            #region showShadow
            if (showShadow)
            {
                var penShadow = new StiPenGeom(Color.FromArgb(50, 0, 0, 0), scaledLineWidth + 0.5f * context.Options.Zoom);
                penShadow.PenStyle = style;

                context.PushTranslateTransform(scaledLineWidth, scaledLineWidth);
                StiNullableDrawing.DrawCurve(context, penShadow, this.points, 0);
                context.PopTransform();
            }
            #endregion

            #region Draw Lines
            var pen = new StiPenGeom(lineColor, lineWidth * context.Options.Zoom);
            pen.PenStyle = style;
            context.DrawCurve(pen, pointsNull, 0);
            #endregion

            #region Draw Text
            if (trendLine.TitleVisible && pointsNull.Length > 0)
            {
                var brush = new StiSolidBrush(trendLine.LineColor);
                var font = StiFontGeom.ChangeFontSize(trendLine.Font, (float)(trendLine.Font.Size * context.Options.Zoom));

                var sf = context.GetGenericStringFormat();
                var point = PointF.Empty;
                var pointStart = pointsNull[0];
                var pointEnd = pointsNull[pointsNull.Length - 1];
                var mode = StiRotationMode.CenterCenter;

                switch (trendLine.Position)
                {
                    case StiTrendLine.StiTextPosition.LeftBottom:
                        point = pointStart;
                        mode = StiRotationMode.LeftTop;
                        break;

                    case StiTrendLine.StiTextPosition.LeftTop:
                        point = pointStart;
                        mode = StiRotationMode.LeftBottom;
                        break;

                    case StiTrendLine.StiTextPosition.RightBottom:
                        point = pointEnd;
                        mode = StiRotationMode.RightTop;
                        break;

                    case StiTrendLine.StiTextPosition.RightTop:
                        point = pointEnd;
                        mode = StiRotationMode.RightBottom;
                        break;
                }

                context.DrawRotatedString(trendLine.Text, font, brush, point, sf, mode, 0, true, 0);
            }
            #endregion

            context.PopSmoothingMode();
        }
        #endregion

        public StiTrendCurveGeom(PointF?[] points, IStiTrendLine trendLine)
            :
            base(StiBaseLineSeriesGeom.GetClientRectangle(points, trendLine.LineWidth))
        {
            this.points = points;
            this.trendLine = trendLine;
        }
    }
}