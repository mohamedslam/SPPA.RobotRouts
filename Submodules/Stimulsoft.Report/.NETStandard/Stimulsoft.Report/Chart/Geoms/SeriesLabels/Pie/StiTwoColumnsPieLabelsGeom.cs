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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using static Stimulsoft.Base.Dashboard.StiElementConsts;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiTwoColumnsPieLabelsGeom : StiSeriesLabelsGeom
    {
        #region Properties
        public StiBrush SeriesBrush { get; }

        public Color BorderColor { get; }

        public Color SeriesBorderColor { get; }

        public StiBrush LabelBrush { get; }

        public StiBrush SeriesLabelsBrush { get; }

        public string Text { get; }

        public RectangleF LabelRect { get; }

        public Color LineColor { get; }
        
        public PointF StartPoint { get; }

        public PointF EndPoint { get; set; }

        public PointF ArcPoint { get; }

        public PointF CenterPie { get; }

        public StiAnimation Animation { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
{
            base.Draw(context);

            var chart = this.Series.Chart as StiChart;

            if (chart.IsAnimation && this.Animation == null)
                this.Animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, StiChartHelper.GlobalDurationElement);

            var labelRect = Rectangle.Round(this.ClientRectangle);                 

            DrawLines(context);
            DrawMarker(context, Rectangle.Round(labelRect), SeriesBorderColor, SeriesBrush);
            DrawLabelArea(context, labelRect);
            DrawLabelText(context, labelRect);

            if (IsMouseOver)
                context.FillRectangle(StiMouseOverHelper.GetLineMouseOverColor(this.SeriesBrush), labelRect);
        }

        private void DrawLines(StiContext context)
        {
            if (SeriesLabels.DrawBorder)
            {
                PointF? endPoint0 = null;

                var linePen = new StiPenGeom(LineColor, 1);

                if (CenterPie.Y > StartPoint.Y && EndPoint.Y > StartPoint.Y || CenterPie.Y < StartPoint.Y && EndPoint.Y < StartPoint.Y)
                {
                    endPoint0 = CenterPie.X > EndPoint.X
                        ? new PointF(EndPoint.X + 13, StartPoint.Y)
                        : new PointF(EndPoint.X - 13, StartPoint.Y);
                }

                context.PushSmoothingModeToAntiAlias();

                var points = endPoint0 != null
                    ? new[] { EndPoint, endPoint0.GetValueOrDefault(), StartPoint, ArcPoint }
                    : new[] { EndPoint, StartPoint, ArcPoint };

                var chart = this.Series.Chart as StiChart;

                if (chart.IsAnimation)
                    context.DrawAnimationLines(linePen, points, this.Animation);

                else
                    context.DrawLines(linePen, points);

                context.PopSmoothingMode();
            }
        }

        private void DrawLabelArea(StiContext context, Rectangle rect)
        {
            if (float.IsNaN(rect.X) || float.IsNaN(rect.Y) || float.IsNaN(rect.Width) || float.IsNaN(rect.Height)) return;

            var chart = this.Series.Chart as StiChart;
            var borderPen = SeriesLabels.DrawBorder ? new StiPenGeom(BorderColor, 1) : null;

            if (chart.IsAnimation)
                context.DrawAnimationRectangle(SeriesLabelsBrush, borderPen, rect, null, this.Animation);
            else
            {
                context.FillRectangle(SeriesLabelsBrush, rect, null);
                if (SeriesLabels.DrawBorder)
                    context.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        private void DrawLabelText(StiContext context, Rectangle rect)
        {
            var chart = this.Series.Chart as StiChart;
            var font = StiFontGeom.ChangeFontSize(SeriesLabels.Font, SeriesLabels.Font.Size * context.Options.Zoom);
            var sf = SeriesLabels.Core.GetStringFormatGeom(context);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            if (chart.IsAnimation)
                context.DrawAnimationText(Text, font, LabelBrush, rect, sf, StiRotationMode.CenterCenter, 0f,
                    SeriesLabels.Antialiasing, rect.Width, this.Animation);

            else
                context.DrawRotatedString(Text, font, LabelBrush, rect, sf, StiRotationMode.CenterCenter, 0f,
                SeriesLabels.Antialiasing, rect.Width);
        }

        protected override void DrawMarker(StiContext context, Rectangle itemRect, object markerColor, StiBrush markerBrush)
        {
            if (!SeriesLabels.MarkerVisible)return;

            var markerRect = Rectangle.Empty;
            var rightPosition = (int)(itemRect.Right + 2 * context.Options.Zoom);
            var leftPosition = (int)(itemRect.Left - (2 + SeriesLabels.MarkerSize.Width) * context.Options.Zoom);

            if (StiOptions.Engine.AllowFixPieChartMarkerAlignment)
            {
                if (SeriesLabels.MarkerAlignment == StiMarkerAlignment.Center)
                    markerRect.X = ClientRectangle.X < CenterPie.X ? rightPosition : leftPosition;

                else if (SeriesLabels.MarkerAlignment == StiMarkerAlignment.Right)
                    markerRect.X = ClientRectangle.X < CenterPie.X ? rightPosition : leftPosition;

                else
                    markerRect.X = ClientRectangle.X > CenterPie.X ? rightPosition : leftPosition;
            }
            else
            {
                if (SeriesLabels.MarkerAlignment == StiMarkerAlignment.Center)
                    markerRect.X = ClientRectangle.X < CenterPie.X ? rightPosition : leftPosition;

                else
                    markerRect.X = SeriesLabels.MarkerAlignment == StiMarkerAlignment.Right ? rightPosition : leftPosition;
            }

            markerRect.Y = (int)(itemRect.Y + (itemRect.Height - SeriesLabels.MarkerSize.Height * context.Options.Zoom) / 2);
            markerRect.Width = (int)(SeriesLabels.MarkerSize.Width * context.Options.Zoom);
            markerRect.Height = (int)(SeriesLabels.MarkerSize.Height * context.Options.Zoom);

            var color = markerColor is Color ? (Color)markerColor : Color.Black;
            var pen = new StiPenGeom(color, 1);

            context.FillRectangle(markerBrush, markerRect.X, markerRect.Y, markerRect.Width, markerRect.Height, null);
            context.DrawRectangle(pen, markerRect.X, markerRect.Y, markerRect.Width, markerRect.Height);
        }
        #endregion

        public StiTwoColumnsPieLabelsGeom(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value,
            RectangleF clientRectangle, string text,
            StiBrush seriesBrush, StiBrush labelBrush, StiBrush seriesLabelsBrush, Color borderColor, Color seriesBorderColor,
            RectangleF labelRect, Color lineColor,
            PointF startPoint, PointF endPoint, PointF arcPoint, PointF centerPie, StiAnimation animation)
            : base(seriesLabels, series, index, value, clientRectangle)
        {
            this.Text = text;
            this.SeriesLabelsBrush = seriesLabelsBrush;
            this.LabelBrush = labelBrush;
            this.LineColor = lineColor;
            this.BorderColor = borderColor;
            this.SeriesBorderColor = seriesBorderColor;
            this.SeriesBrush = seriesBrush;
            this.LabelRect = labelRect;
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.ArcPoint = arcPoint;
            this.CenterPie = centerPie;
            this.Animation = animation;
        }
    }
}
