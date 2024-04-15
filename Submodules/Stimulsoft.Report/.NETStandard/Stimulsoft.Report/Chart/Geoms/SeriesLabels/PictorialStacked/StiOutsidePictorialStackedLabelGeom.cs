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
    internal class StiOutsidePictorialStackedLabelGeom : StiSeriesLabelsGeom
    {
        #region Properties
        public StiBrush SeriesBrush { get; }

        public Color BorderColor { get; }

        public Color SeriesBorderColor { get; }

        public Color LabelLineColor { get; }

        public StiBrush LabelBrush { get; }

        public string Text { get; }

        public RectangleF LabelRect { get; }

        public RectangleF LineRect { get; }

        public StiAnimation Animation { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            base.Draw(context);

            var rectCurrent = Rectangle.Round(this.LabelRect);

            var chart = this.Series.Chart as StiChart;

            if (chart.IsAnimation && this.Animation == null)
                this.Animation = new StiOpacityAnimation(StiChartHelper.GlobalBeginTimeElement, StiChartHelper.GlobalDurationElement);

            DrawMarker(context, Rectangle.Round(LabelRect), SeriesBorderColor, SeriesBrush);
            if (chart.IsAnimation && StiOptions.Configuration.IsWPF)
            {
                DrawAnimationLabelWPF(context);
            }

            else
            {
                DrawLineLabel(context);

                int distX = rectCurrent.X + rectCurrent.Width / 2;
                int distY = rectCurrent.Y + rectCurrent.Height / 2;

                context.PushTranslateTransform(distX, distY);
                context.PushRotateTransform(SeriesLabels.Angle);

                Rectangle rect = rectCurrent;
                rect.X = -rect.Width / 2;
                rect.Y = -rect.Height / 2;

                DrawLabelArea(context, rect);
                DrawLabelText(context, rect);

                context.PopTransform();
                context.PopTransform();

                if (IsMouseOver)
                    context.FillRectangle(StiMouseOverHelper.GetLineMouseOverColor(this.SeriesBrush), LabelRect.X, LabelRect.Y, LabelRect.Width, LabelRect.Height, null);
            }
        }

        private void DrawAnimationLabelWPF(StiContext context)
        {
            if (this.Animation == null)
                Animation = new StiOpacityAnimation(Duration, BeginTime);

            var font = StiFontGeom.ChangeFontSize(SeriesLabels.Font, SeriesLabels.Font.Size * context.Options.Zoom);
            var borderPen = SeriesLabels.DrawBorder ? new StiPenGeom(BorderColor, 1) : null;
            var sf = SeriesLabels.Core.GetStringFormatGeom(context);

            context.DrawAnimationLines(new StiPenGeom(LabelLineColor, 1), new PointF[] { new PointF(LineRect.X, LineRect.Y + LineRect.Height / 2), new PointF(LineRect.Right, LineRect.Y + LineRect.Height / 2) }, this.Animation);
            context.DrawAnimationLabel(Text, font, LabelBrush, SeriesLabels.Brush, borderPen, Rectangle.Round(LabelRect), sf, StiRotationMode.CenterCenter, SeriesLabels.Angle, SeriesLabels.DrawBorder, Animation);
        }

        private void DrawLineLabel(StiContext context)
        {
            var chart = this.Series.Chart as StiChart;

            if (chart.IsAnimation)
            {
                context.DrawAnimationLines(new StiPenGeom(LabelLineColor, 1), new PointF[] { new PointF(LineRect.X, LineRect.Y + LineRect.Height / 2), new PointF(LineRect.Right, LineRect.Y + LineRect.Height / 2) }, this.Animation);
            }

            else
            {
                context.DrawLine(new StiPenGeom(LabelLineColor, 1), LineRect.X, LineRect.Y + LineRect.Height / 2, LineRect.Right, LineRect.Y + LineRect.Height / 2);
            }
        }

        private void DrawLabelArea(StiContext context, Rectangle rect)
        {
            if (rect.IsEmpty) return;

            var chart = this.Series.Chart as StiChart;
            var borderPen = SeriesLabels.DrawBorder ? new StiPenGeom(BorderColor) : null;

            if (chart.IsAnimation)
            {
                context.DrawAnimationRectangle(SeriesLabels.Brush, borderPen, rect, null, this.Animation);
            }

            else
            {
                context.FillRectangle(SeriesLabels.Brush, rect, null);
                if (SeriesLabels.DrawBorder)
                    context.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
            }

            if (IsMouseOver)
                context.FillRectangle(StiMouseOverHelper.GetLineMouseOverColor(this.SeriesBrush), rect.X, rect.Y, rect.Width, rect.Height, null);
        }

        private void DrawLabelText(StiContext context, Rectangle rect)
        {
            var chart = this.Series.Chart as StiChart;
            var font = StiFontGeom.ChangeFontSize(SeriesLabels.Font, SeriesLabels.Font.Size * context.Options.Zoom);
            var sf = SeriesLabels.Core.GetStringFormatGeom(context);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            if (chart.IsAnimation)
                context.DrawAnimationText(Text, font, LabelBrush, rect, sf, StiRotationMode.CenterCenter, 0, SeriesLabels.Antialiasing, rect.Width, this.Animation);

            else
                context.DrawRotatedString(Text, font, LabelBrush, rect, sf, StiRotationMode.CenterCenter, 0, SeriesLabels.Antialiasing, rect.Width);
        }
        #endregion

        public StiOutsidePictorialStackedLabelGeom(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value,
            RectangleF clientRectangle, string text,
            StiBrush seriesBrush, StiBrush labelBrush, Color labelLineColor, Color borderColor, Color seriesBorderColor, RectangleF labelRect, RectangleF lineRect, StiAnimation animation)
            : base(seriesLabels, series, index, value, clientRectangle)
        {
            this.Text = text;
            this.LabelBrush = labelBrush;
            this.LabelLineColor = labelLineColor;
            this.BorderColor = borderColor;
            this.SeriesBorderColor = seriesBorderColor;
            this.SeriesBrush = seriesBrush;
            this.Animation = animation;
            this.LineRect = lineRect;

            this.LabelRect = labelRect;
        }
    }
}
