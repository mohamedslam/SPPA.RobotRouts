#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
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

using System.Collections.Generic;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using System.Linq;
using System;
using Stimulsoft.Report.Helpers;
using System.Threading.Tasks;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiPictorialStackedSeriesCoreXF : StiSeriesCoreXF
    {
        #region Fields
        private object lockMeasureObj = new object(); 
        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "PictorialStacked");
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesCollection)
        {
            if (seriesCollection.Length == 0 || rect.Width <=0 || rect.Height<= 0) return;

            var pictoriaStackedlSeries = seriesCollection.Cast<IStiPictorialStackedSeries>().ToArray();

            double[] allTrueValues = GetAllTrueValues(pictoriaStackedlSeries);

            int globalIndex = 0;

            #region Calculate Colors
            int colorCount = 0;
            foreach (IStiPictorialStackedSeries ser in pictoriaStackedlSeries)
            {
                if (ser.Values != null)
                {
                    colorCount += ser.Values.Length;
                }
            }
            #endregion

            #region Calculate width Labels Width
            var labels = this.Series.Chart.SeriesLabels;
            var maxLabelWidth = labels.Width * context.Options.Zoom;
            var measureLabelWidth = 0;

            if (maxLabelWidth != 0)
            {
                measureLabelWidth = (int)maxLabelWidth;
            }

            else
            {
                foreach(var funnelSeries in seriesCollection)
                {
                    if (labels != null && labels.Visible)
                    {
                        for (int pointIndex = 0; pointIndex < funnelSeries.Values.Length; pointIndex++)
                        {
                            if (funnelSeries.Values.Length > pointIndex)
                            {
                                double value = funnelSeries.Values[pointIndex].GetValueOrDefault();

                                if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                                {
                                    var labelText = labels.Core.
                                        GetLabelText(funnelSeries, value, GetArgumentText(funnelSeries, pointIndex), funnelSeries.Core.GetTag(pointIndex), funnelSeries.CoreTitle);

                                    var font = StiFontGeom.ChangeFontSize(labels.Font, labels.Font.Size * context.Options.Zoom);
                                    var labelRect = context.MeasureString(labelText, font);

                                    lock (lockMeasureObj)
                                    {
                                        measureLabelWidth = (int)Math.Max(measureLabelWidth, Math.Ceiling(labelRect.Width));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            measureLabelWidth = measureLabelWidth + StiScale.I10;
            #endregion

            if (labels is StiOutsideLeftPictorialStackedLabels)
            {
                rect = new RectangleF(rect.X + measureLabelWidth, rect.Y, rect.Width - measureLabelWidth, rect.Height);
            }

            else if (labels is StiOutsideRightPictorialStackedLabels)
            {
                rect = new RectangleF(rect.X, rect.Y, rect.Width - measureLabelWidth, rect.Height);
            }

            var icon = ((IStiPictorialStackedSeries)seriesCollection[0]).Icon.GetValueOrDefault();
            var rectIcon = GetRectangle(context, rect, icon);
            var correctionPoint = CorrectionMainPoint(rectIcon, icon);
            rectIcon = new RectangleF(rectIcon.X + correctionPoint.X, rectIcon.Y + 0, rectIcon.Width, rectIcon.Height);

            var rectLabels = rect;

            var lineLength = 0f;

            rect = new RectangleF(rect.X + correctionPoint.X, rect.Y + 0, rect.Width, rect.Height);

            if (labels is StiOutsideLeftPictorialStackedLabels)
            {
                rectLabels = new RectangleF(rect.X - measureLabelWidth, rectIcon.Y, measureLabelWidth, rectIcon.Height);
                lineLength = rectIcon.Left - rectLabels.Right - measureLabelWidth;
            }

            else if (labels is StiOutsideRightPictorialStackedLabels)
            {
                rectLabels = new RectangleF(rectIcon.Right, rectIcon.Y, measureLabelWidth, rectIcon.Height);
                lineLength = rect.Right - rectIcon.Right;
            }

            float singleValueHeight = rectIcon.Height / GetSumValues(allTrueValues);

            var listLabelsGeom = new List<StiSeriesLabelsGeom>();

            var x = rect.X;
            var y = 0f;

            var firstitem = true;

            #region Render Items
            for (int indexSer = 0; indexSer < pictoriaStackedlSeries.Length; indexSer++)
            {
                var ser = pictoriaStackedlSeries[indexSer];
                for (int index = 0; index < ser.Values.Length; index++)
                {
                    double value = ser.Values[index].GetValueOrDefault();

                    if (value == 0)
                    {
                        globalIndex++;
                        continue;
                    }

                    var seriesBrush = ser.Core.GetSeriesBrush(globalIndex, colorCount);
                    seriesBrush = ser.ProcessSeriesBrushes(globalIndex, seriesBrush);
                    seriesBrush = new StiSolidBrush(StiBrush.ToColor(seriesBrush));

                    var height = singleValueHeight * Math.Abs((float)value);

                    if (firstitem)
                    {
                        firstitem = false;
                        height += rectIcon.Y;
                    }

                    if (index == pictoriaStackedlSeries.Length - 1 && index == ser.Values.Length - 1)
                        height = rect.Bottom - y;

                    var clipRect = new RectangleF(x, y, rect.Width, height);

                    var timeSpan = new TimeSpan(StiChartHelper.GlobalBeginTimeElement.Ticks / ser.Values.Length * index);
                    var pictorialGeom = new StiPictorialStackedSeriesElementGeom(geom, value, index, seriesBrush, ser, icon, rect, clipRect, null);

                    if (pictorialGeom != null)
                    {
                        if (ser.Core.Interaction != null)
                            pictorialGeom.Interaction = new StiSeriesInteractionData(geom.Area, ser, index);

                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(pictorialGeom);
                    }

                    y += height;
                    globalIndex++;
                }
            } 
            #endregion

            #region Render Labels
            x = rectIcon.X;
            y = rectIcon.Y;
            globalIndex = 0;

            for (int indexSer = 0; indexSer < pictoriaStackedlSeries.Length; indexSer++)
            {
                var ser = pictoriaStackedlSeries[indexSer];
                for (int index = 0; index < ser.Values.Length; index++)
                {
                    double value = ser.Values[index].GetValueOrDefault();

                    if (value == 0)
                    {
                        globalIndex++;
                        continue;
                    }

                    var height = singleValueHeight * Math.Abs((float)value);

                    #region Render Series Labels
                    if (labels != null && labels.Visible)
                    {
                        if (labels.Step == 0 || (index % labels.Step == 0))
                        {
                            if (labels.Core is StiPictorialStackedLabelsCoreXF pictorialStackedLabelsCoreXF)
                            {
                                var rectLabel = new RectangleF(rectLabels.X, y, rectLabels.Width, height);

                                var seriesLabelsGeom =
                                    pictorialStackedLabelsCoreXF.RenderLabel(ser, context, index, value,
                                    GetArgumentText(ser, index), ser.Core.GetTag(index), globalIndex, colorCount, lineLength, rectLabel);

                                if (seriesLabelsGeom != null)
                                    listLabelsGeom.Add(seriesLabelsGeom);
                            }
                        }
                    }
                    #endregion

                    y += height;
                    globalIndex++;
                }
            } 
            #endregion

            #region Add Labels on Area
            foreach (var label in listLabelsGeom)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(label);

                label.ClientRectangle = CheckLabelsRect(labels, geom, label.ClientRectangle);
            }
            #endregion
        }

        private PointF CorrectionMainPoint(RectangleF rect, StiFontIcons icon)
        {
            var paddingArray = StiFontIconsHelper.GetIconPadding(icon);

            var paddingPercentLeft = paddingArray[0];
            var paddingPercentTop = paddingArray[1];
            var paddingPercentRight = paddingArray[2];
            var paddingPercentBottom = paddingArray[3];

            return new PointF(
                (paddingPercentRight - paddingPercentLeft) / 100 * rect.Width / 2,
                (paddingPercentBottom - paddingPercentTop) / 100 * rect.Height / 2);
        }

        private string GetArgumentText(IStiSeries series, int index)
        {
            if (series.Arguments.Length > index && series.Arguments[index] != null)
            {
                return series.Arguments[index].ToString();
            }
            return string.Empty;
        }

        private RectangleF GetRectangle(StiContext context, RectangleF rect, StiFontIcons icon)
        {
            using (var fontFamilyIcons = StiFontIconsHelper.GetFontFamilyIcons())
            using (var font = new Font(fontFamilyIcons, 1))
            {
                var fontSizeDelta = MeasureFontSize(context, rect, font, icon);

                if (fontSizeDelta <= 0) return RectangleF.Empty;

                using (var fontNew = new Font(fontFamilyIcons, fontSizeDelta.GetValueOrDefault()))
                {
                    var fontGeom = new StiFontGeom(fontNew.FontFamily, fontNew.FontFamily.Name, fontNew.Size, fontNew.Style,
                                fontNew.Unit, fontNew.GdiCharSet, fontNew.GdiVerticalFont);

                    var textIcon = StiFontIconsHelper.GetContent(icon);

                    var sf = GetStringFormatGeom(context);
                    var realSizeIcon = context.MeasureRotatedString(textIcon, fontGeom, rect, sf, 0);

                    var paddingArray = StiFontIconsHelper.GetIconPadding(icon);

                    var paddingPercentLeft = paddingArray[0];
                    var paddingPercentTop = paddingArray[1];
                    var paddingPercentRight = paddingArray[2];
                    var paddingPercentBottom = paddingArray[3];

                    return new RectangleF(
                        rect.X + realSizeIcon.X + paddingPercentLeft / 100f * realSizeIcon.Width,
                        rect.Y + realSizeIcon.Y + paddingPercentTop / 100f * realSizeIcon.Height,
                        realSizeIcon.Width * (1 - (paddingPercentLeft + paddingPercentRight) / 100f),
                        realSizeIcon.Height * (1 - (paddingPercentTop + paddingPercentBottom) / 100f));
                }
            }
        }

        protected internal StiStringFormatGeom GetStringFormatGeom(StiContext context)
        {
            var sf = context.GetGenericStringFormat();
            sf.Trimming = StringTrimming.None;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            return sf;
        }

        private float? MeasureFontSize(StiContext context, RectangleF rect, Font font, StiFontIcons icon)
        {
            var textIcon = StiFontIconsHelper.GetContent(icon);

            var size = context.MeasureString(textIcon, new StiFontGeom(new Font(font.FontFamily.Name, 1, font.Style)));

            float? fontSize = null;

            if (size.Width == 0 || size.Height == 0)
                return 0;

            if (rect.Width > rect.Height)
            {
                var factorX = rect.Width / size.Width;
                var factorY = rect.Height / size.Height;
                var factor = factorX > factorY ? factorY : factorX;

                if (fontSize == null)
                    fontSize = factor;

                else if (fontSize > factor)
                    fontSize = factor;
            }

            else
            {
                var factor = rect.Width / size.Width * size.Width / size.Height;

                if (fontSize == null)
                    fontSize = factor;

                else if (fontSize > factor)
                    fontSize = factor;
            }

            return fontSize;
        }

        private float GetSumValues(double[] values)
        {
            double sumValues = 0;
            foreach (double value in values)
            {
                sumValues += Math.Abs(value);
            }
            return (float)sumValues;
        }

        private double[] GetAllTrueValues(IStiSeries[] funnelSeries)
        {
            var values = new List<double>();

            foreach (var series in funnelSeries)
            {
                foreach (double? value in series.Values)
                {
                    values.Add(value.GetValueOrDefault());
                }
            }

            return values.ToArray();
        } 
        #endregion

        public StiPictorialStackedSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
