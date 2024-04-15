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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiTrendLineGeom : StiCellGeom
    {
        #region Fields
        private IStiTrendLine trendLine;

        private PointF pointStart;

        private PointF pointEnd;        
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
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
                StiNullableDrawing.DrawLines(context, penShadow, GetArray(pointStart, pointEnd));
                context.PopTransform();
            }
            #endregion

            #region Draw Lines
            var pen = new StiPenGeom(lineColor, lineWidth * context.Options.Zoom);
            pen.PenStyle = style;

            context.DrawLine(pen, pointStart.X, pointStart.Y, pointEnd.X, pointEnd.Y);
            #endregion

            #region Draw Text
            if (trendLine.TitleVisible)
            {
                var brush = new StiSolidBrush(trendLine.LineColor);
                var font = StiFontGeom.ChangeFontSize(trendLine.Font, (float)(trendLine.Font.Size * context.Options.Zoom));

                var sf = context.GetGenericStringFormat();
                var point = PointF.Empty;
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

        private static PointF?[] GetArray(PointF pointStart, PointF pointEnd)
        {
            PointF?[] points = {pointStart, pointEnd};
            return points;
        }
        #endregion

        public StiTrendLineGeom(PointF pointStart, PointF pointEnd, IStiTrendLine trendLine)
            :
            base(StiBaseLineSeriesGeom.GetClientRectangle(StiTrendLineGeom.GetArray(pointStart, pointEnd), trendLine.LineWidth))
        {
            this.pointStart = pointStart;
            this.pointEnd = pointEnd;
            this.trendLine = trendLine;
        }
    }
}
