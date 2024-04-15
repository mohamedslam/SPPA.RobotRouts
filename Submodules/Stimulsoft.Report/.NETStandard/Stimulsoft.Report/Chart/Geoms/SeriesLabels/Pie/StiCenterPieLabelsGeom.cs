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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiCenterPieLabelsGeom : StiSeriesLabelsGeom
    {
        #region Properties
        public StiBrush SeriesBrush { get; }

        public Color BorderColor { get; }

        public Color SeriesBorderColor { get; }

        public StiBrush SeriesLabelsBrush { get; }

        public StiBrush LabelBrush { get; }

        public string Text { get; }

        public StiRotationMode RotationMode { get; }

        public RectangleF LabelRect { get; set; }

        public float AngleToUse { get; }

        public StiAnimation Animation { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            base.Draw(context);

            var rect = Rectangle.Round(this.ClientRectangle);

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
                //Округляем для того чтобы убрать смазывание бордюра labels
                var distX = (int)Math.Round(LabelRect.X + LabelRect.Width / 2, 0);
                var distY = (int)Math.Round(LabelRect.Y + LabelRect.Height / 2, 0);

                context.PushTranslateTransform(distX, distY);
                context.PushRotateTransform(AngleToUse);

                DrawLabelArea(context, rect);
                DrawLabelText(context, rect);

                context.PopTransform();
                context.PopTransform();
            }
        }

        private void DrawAnimationLabelWPF(StiContext context)
        {
            if (this.Animation == null)
                Animation = new StiOpacityAnimation(Duration, BeginTime);

            var font = StiFontGeom.ChangeFontSize(SeriesLabels.Font, SeriesLabels.Font.Size * context.Options.Zoom);
            var borderPen = SeriesLabels.DrawBorder ? new StiPenGeom(BorderColor, 1) : null;
            var sf = SeriesLabels.Core.GetStringFormatGeom(context);

            context.DrawAnimationLabel(Text, font, LabelBrush, SeriesLabelsBrush, borderPen, Rectangle.Round(LabelRect), sf, RotationMode, AngleToUse, SeriesLabels.DrawBorder, Animation);
        }

        private void DrawLabelArea(StiContext context, Rectangle rect)
        {
            if (rect.IsEmpty) return;

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

        public StiCenterPieLabelsGeom(IStiSeriesLabels seriesLabels, IStiSeries series, int index, double value,
            RectangleF clientRectangle, string text,
            StiBrush seriesBrush, StiBrush labelBrush, StiBrush seriesLabelsBrush, Color borderColor, Color seriesBorderColor,
            StiRotationMode rotationMode, RectangleF labelRect, float angleToUse, StiAnimation animation)
            : base(seriesLabels, series, index, value, clientRectangle)
        {
            this.Text = text;
            this.LabelBrush = labelBrush;
            this.BorderColor = borderColor;
            this.SeriesBorderColor = seriesBorderColor;
            this.SeriesLabelsBrush = seriesLabelsBrush;
            this.SeriesBrush = seriesBrush;
            this.RotationMode = rotationMode;
            this.LabelRect = labelRect;
            this.AngleToUse = angleToUse;
            this.Animation = animation;
        }
    }
}
