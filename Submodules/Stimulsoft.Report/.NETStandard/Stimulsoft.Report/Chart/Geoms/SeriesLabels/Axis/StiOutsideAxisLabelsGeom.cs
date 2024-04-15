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
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiOutsideAxisLabelsGeom : StiSeriesLabelsGeom
    {
        #region Properties
        public Color LabelColor { get; }

        public Color LabelBorderColor { get; }

        public Color LabelLineColor { get; }

        public StiBrush SeriesBrush { get; }

        public Color SeriesBorderColor { get; }

        public StiFontGeom Font { get; }

        public string Text { get; }

        public PointF StartPoint { get; }

        public PointF EndPoint { get; }

        public StiAnimation Animation { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            base.Draw(context);

            var labelRect = Rectangle.Round(this.ClientRectangle);
            var chart = this.Series.Chart as StiChart;

            if (chart.IsAnimation && this.Animation == null)
                this.Animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, StiChartHelper.GlobalDurationElement);

            DrawMarker(context, labelRect, this.SeriesBorderColor, this.SeriesBrush);
            DrawLines(context, labelRect);
            if (chart.IsAnimation && StiOptions.Configuration.IsWPF)
            {
                DrawAnimationLabelWPF(context, labelRect);
            }

            else
            {
                context.PushTranslateTransform(labelRect.X + labelRect.Width / 2, labelRect.Y + labelRect.Height / 2);
                context.PushRotateTransform(SeriesLabels.Angle);

                Rectangle rect = labelRect;

                rect.X = -rect.Width / 2;
                rect.Y = -rect.Height / 2;

                DrawLabelArea(context, rect);
                DrawLabelText(context, rect);

                context.PopTransform();
                context.PopTransform();

                if (IsMouseOver)
                    context.FillRectangle(StiMouseOverHelper.GetLineMouseOverColor(this.SeriesBrush), labelRect, null);
            }
        }

        private void DrawAnimationLabelWPF(StiContext context, Rectangle rect)
        {
            if (this.Animation == null)
                Animation = new StiOpacityAnimation(Duration, BeginTime);

            var labelBrush = new StiSolidBrush(LabelColor);
            var font = StiFontGeom.ChangeFontSize(SeriesLabels.Font, SeriesLabels.Font.Size * context.Options.Zoom);
            var borderPen = SeriesLabels.DrawBorder ? new StiPenGeom(LabelBorderColor, 1) : null;
            var sf = SeriesLabels.Core.GetStringFormatGeom(context);

            context.DrawAnimationLabel(Text, font, labelBrush, SeriesLabels.Brush, borderPen, Rectangle.Round(rect), sf, StiRotationMode.CenterCenter, SeriesLabels.Angle, SeriesLabels.DrawBorder, Animation);
        }

        private void DrawLabelArea(StiContext context, Rectangle rect)
        {
            if (rect.IsEmpty) return;

            var chart = this.Series.Chart as StiChart;
            var borderPen = SeriesLabels.DrawBorder ? new StiPenGeom(LabelBorderColor, 1) : null;

            if (chart.IsAnimation)
                context.DrawAnimationRectangle(SeriesLabels.Brush, borderPen, rect, null, this.Animation);

            else
            {
                context.FillRectangle(SeriesLabels.Brush, rect, null);
                if (SeriesLabels.DrawBorder)
                    context.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
            }

            if (IsMouseOver)
                context.FillRectangle(StiMouseOverHelper.GetLineMouseOverColor(this.SeriesBrush), rect.X, rect.Y, rect.Width, rect.Height, null);
        }

        private void DrawLines(StiContext context, Rectangle rect)
        {
            var chart = this.Series.Chart as StiChart;
            var lineColor = new StiPenGeom(LabelLineColor);
            var borderPen = new StiPenGeom(LabelBorderColor);

            if (chart.IsAnimation)
                context.DrawAnimationLines(borderPen, new[] { EndPoint, StartPoint }, this.Animation);

            else
                context.DrawLine(lineColor, EndPoint.X, EndPoint.Y, StartPoint.X, rect.Y + rect.Height);
        }

        private void DrawLabelText(StiContext context, Rectangle rect)
        {
            var labelBrush = new StiSolidBrush(LabelColor);
            var chart = this.Series.Chart as StiChart;
            var font = StiFontGeom.ChangeFontSize(SeriesLabels.Font, SeriesLabels.Font.Size * context.Options.Zoom);
            var sf = SeriesLabels.Core.GetStringFormatGeom(context);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            if (chart.IsAnimation)
                context.DrawAnimationText(Text, font, labelBrush, rect, sf, StiRotationMode.CenterCenter, 0, SeriesLabels.Antialiasing, rect.Width, this.Animation);

            else
                context.DrawRotatedString(Text, font, labelBrush, rect, sf, StiRotationMode.CenterCenter, 0, SeriesLabels.Antialiasing, rect.Width);
        }
        #endregion

        public StiOutsideAxisLabelsGeom(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value,
            RectangleF clientRectangle, string text, Color labelColor, Color labelBorderColor, Color labelLineColor,
            StiBrush seriesBrush, Color seriesBorderColor, StiFontGeom font, PointF startPoint, PointF endPoint, StiAnimation animation)
            : base(seriesLabels, series, index, value, clientRectangle)
        {
            this.Text = text;
            this.LabelColor = labelColor;
            this.LabelLineColor = labelLineColor;
            this.LabelBorderColor = labelBorderColor;
            this.SeriesBrush = seriesBrush;
            this.SeriesBorderColor = seriesBorderColor;
            this.Font = font;
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.Animation = animation;
        }
    }
}
