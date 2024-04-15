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
    public class StiCenterAxisLabelsGeom : StiSeriesLabelsGeom
    {
        #region Properties
        public Color LabelColor { get; }

        public Color LabelBorderColor { get; }

        public StiBrush SeriesBrush { get; }

        public StiBrush SeriesLabelsBrush { get; }

        public Color SeriesBorderColor { get; }

        public StiFontGeom Font { get; }

        public string Text { get; }

        public StiAnimation Animation { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            base.Draw(context);

            var labelRect = Rectangle.Round(this.ClientRectangle);

            var sf = SeriesLabels.Core.GetStringFormatGeom(context);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var chart = this.Series.Chart as StiChart;

            if (chart.IsAnimation && this.Animation == null)
                Animation = new StiOpacityAnimation(Duration, BeginTime);

            DrawMarker(context, labelRect, this.SeriesBorderColor, this.SeriesBrush);
            if (chart.IsAnimation && StiOptions.Configuration.IsWPF)
            {
                DrawAnimationLabelWPF(context, labelRect);
            }
            else
            {
                var distX = labelRect.X + labelRect.Width / 2;
                var distY = labelRect.Y + labelRect.Height / 2;

                context.PushTranslateTransform(distX, distY);
                context.PushRotateTransform(SeriesLabels.Angle);

                var rect = labelRect;
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

            context.DrawAnimationLabel(Text, font, labelBrush, SeriesLabelsBrush, borderPen, Rectangle.Round(rect), sf, StiRotationMode.CenterCenter, SeriesLabels.Angle, SeriesLabels.DrawBorder, Animation);
        }

        private void DrawLabelArea(StiContext context, Rectangle rect)
        {
            if (rect.IsEmpty) return;

            var chart = this.Series.Chart as StiChart;
            var borderPen = SeriesLabels.DrawBorder ? new StiPenGeom(LabelBorderColor, 1) : null;

            if (chart.IsAnimation)
                context.DrawAnimationRectangle(SeriesLabelsBrush, borderPen, rect, null, this.Animation);

            else
            {
                context.FillRectangle(SeriesLabelsBrush, rect, null);
                if (SeriesLabels.DrawBorder)
                    context.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
            }

            if (IsMouseOver)
                context.FillRectangle(StiMouseOverHelper.GetLineMouseOverColor(this.SeriesBrush), rect.X, rect.Y, rect.Width, rect.Height, null);
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

        public StiCenterAxisLabelsGeom(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value,
            RectangleF clientRectangle, string text, Color labelColor, Color labelBorderColor,
            StiBrush seriesBrush, StiBrush seriesLabelsBrush, Color seriesBorderColor, StiFontGeom font, StiAnimation animation)
            : base(seriesLabels, series, index, value, clientRectangle)
        {
            this.Text = text;
            this.LabelColor = labelColor;
            this.LabelBorderColor = labelBorderColor;
            this.SeriesBrush = seriesBrush;
            this.SeriesLabelsBrush = seriesLabelsBrush;
            this.SeriesBorderColor = seriesBorderColor;
            this.Font = font;
            this.Animation = animation;
        }
    }
}
