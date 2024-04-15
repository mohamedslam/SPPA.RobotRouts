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

using System;
using System.ComponentModel;
using System.Globalization;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Threading;
using Stimulsoft.Report.Components.TextFormats;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiSeriesLabelsCoreXF : 
        ICloneable,
        IStiApplyStyle
	{
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (this.SeriesLabels.AllowApplyStyle)
            {
                this.SeriesLabels.LabelColor = style.Core.SeriesLabelsColor;
                this.SeriesLabels.BorderColor = style.Core.SeriesLabelsBorderColor;
                this.SeriesLabels.Brush = style.Core.SeriesLabelsBrush;
                this.SeriesLabels.Font = style.Core.SeriesLabelsFont;
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public abstract int Position
        {
            get;
        }

        [Browsable(false)]
        public abstract StiSeriesLabelsType SeriesLabelsType
        {
            get;
        }

        private IStiSeriesLabels seriesLabels;
        public IStiSeriesLabels SeriesLabels
        {
            get
            {
                return seriesLabels;
            }
            set
            {
                seriesLabels = value;
            }
        }
        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public abstract string LocalizedName
        {
            get;
        }
        #endregion

        #region Methods
        public StiBrush ProcessSeriesColors(int pointIndex, StiBrush brush, IStiSeries series)
        {
            StiBrush seriesBrush = brush;
            if (this.SeriesLabels == null) return seriesBrush;

            foreach (StiChartCondition condition in this.SeriesLabels.Chart.SeriesLabelsConditions)
            {
                if (series.Values == null || series.Values.Length <= pointIndex) continue;
                object itemValue = series.Values[pointIndex];

                object itemValueEnd = null;
                if (series is IStiRangeSeries && pointIndex < ((IStiRangeSeries)series).ValuesEnd.Length)
                    itemValueEnd = ((IStiRangeSeries)series).ValuesEnd[pointIndex];

                object itemArgument = null;
                if (series.Arguments != null && pointIndex < series.Arguments.Length)
                    itemArgument = series.Arguments[pointIndex];

                object itemTooltip = null;
                if (series.ToolTips != null && pointIndex < series.ToolTips.Length)
                    itemTooltip = series.ToolTips[pointIndex];

                object itemValueOpen = null;
                object itemValueClose = null;
                object itemValueLow = null;
                object itemValueHigh = null;
                if (series is IStiFinancialSeries)
                {
                    if (pointIndex < ((IStiFinancialSeries)series).ValuesOpen.Length)
                        itemValueOpen = ((IStiFinancialSeries)series).ValuesOpen[pointIndex];

                    if (pointIndex < ((IStiFinancialSeries)series).ValuesClose.Length)
                        itemValueClose = ((IStiFinancialSeries)series).ValuesClose[pointIndex];

                    if (pointIndex < ((IStiFinancialSeries)series).ValuesLow.Length)
                        itemValueLow = ((IStiFinancialSeries)series).ValuesLow[pointIndex];

                    if (pointIndex < ((IStiFinancialSeries)series).ValuesHigh.Length)
                        itemValueHigh = ((IStiFinancialSeries)series).ValuesHigh[pointIndex];
                }

                object data = StiChartHelper.GetFilterData(null, condition, null);
                if (StiChartHelper.GetFilterResult(condition, itemArgument, itemValue, itemValueEnd, itemValueOpen, itemValueClose, itemValueLow, itemValueHigh, itemTooltip, data))
                {
                    seriesBrush = this.SeriesLabels.Chart.Style.Core.GetColumnBrush(condition.Color);
                    if (this.SeriesLabels.Chart.Area is IStiClusteredBarArea)
                    {
                        if (seriesBrush is StiGradientBrush) ((StiGradientBrush)seriesBrush).Angle += 90;
                        if (seriesBrush is StiGlareBrush) ((StiGlareBrush)seriesBrush).Angle += 90;
                    }
                    return seriesBrush;
                }
            }
            return seriesBrush;
        }

        internal Color GetSeriesLabelColor(IStiSeries series, int colorIndex, int colorCount)
        {
            var color = (Color)series.Core.GetSeriesBorderColor(colorIndex, colorCount);

            if (color == Color.Transparent)
                color = StiBrush.ToColor(series.Core.GetSeriesBrush(colorIndex, colorCount));

            return color;
        }

        protected Color GetBorderColor(IStiSeries series, int colorIndex, int colorCount)
        {
            if (this.SeriesLabels.UseSeriesColor) return GetSeriesLabelColor(series, colorIndex, colorCount);
            return this.SeriesLabels.BorderColor;
        }


        protected Color GetLabelColor(IStiSeries series, int colorIndex, int colorCount)
        {
            if (this.SeriesLabels.UseSeriesColor) return GetSeriesLabelColor(series, colorIndex, colorCount);
            return this.SeriesLabels.LabelColor;
        }


        internal virtual double RecalcValue(double value, int signs)
        {
            return value;
        }


        internal string GetLabelText(IStiSeries series, double? value, string argument, string tag, string seriesName)
        {
            return GetLabelText(series, value, argument, tag, seriesName, false);
        }

        internal string GetLabelText(IStiSeries series, double? value, string argument, string tag, string seriesName, bool useLegendValueType)
        {
            return GetLabelText(series, value, argument, tag, seriesName, 0, useLegendValueType);
        }

        internal string GetLabelText(IStiSeries series, double? value, string argument, string tag, string seriesName, double weight, bool useLegendValueType)
        {
            string text = null;
            StiSeriesLabelsValueType type = this.SeriesLabels.ValueType;
            if (useLegendValueType) type = this.SeriesLabels.LegendValueType;

            switch (type)
            {
                case StiSeriesLabelsValueType.Argument:
                    text = GetArgument(series, argument);
                    break;

                case StiSeriesLabelsValueType.Value:
                    text = GetFormattedValue(series, value);
                    break;

                case StiSeriesLabelsValueType.SeriesTitle:
                    text = seriesName;
                    break;

                case StiSeriesLabelsValueType.Tag:
                    text = tag;
                    break;

                case StiSeriesLabelsValueType.Weight:
                    text = weight.ToString();
                    break;

                case StiSeriesLabelsValueType.ValueArgument:
                    {
                        var arg3 = GetArgument(series, argument);
                        text = (string.IsNullOrEmpty(arg3))
                            ? GetFormattedValue(series, value)
                            : string.Format("{0} {1} {2}", GetFormattedValue(series, value), this.SeriesLabels.ValueTypeSeparator, arg3);
                    }
                    break;

                case StiSeriesLabelsValueType.ArgumentValue:
                    {
                        var arg3 = GetFormattedValue(series, value);
                        text = (string.IsNullOrEmpty(arg3))
                            ? GetArgument(series, argument)
                            : string.Format("{0} {1} {2}", GetArgument(series, argument), this.SeriesLabels.ValueTypeSeparator, arg3);
                    }
                    break;

                case StiSeriesLabelsValueType.SeriesTitleArgument:
                    {
                        var arg3 = GetArgument(series, argument);
                        text = (string.IsNullOrEmpty(arg3))
                            ? seriesName
                            : string.Format("{0} {1} {2}", seriesName, this.SeriesLabels.ValueTypeSeparator, arg3);
                    }
                    break;

                case StiSeriesLabelsValueType.SeriesTitleValue:
                    {
                        var arg3 = GetFormattedValue(series, value);
                        text = (string.IsNullOrEmpty(arg3))
                            ? seriesName
                            : string.Format("{0} {1} {2}", seriesName, this.SeriesLabels.ValueTypeSeparator, arg3);
                    }
                    break;
            }

            if (useLegendValueType) return text;
            return string.Format("{0}{1}{2}", this.SeriesLabels.TextBefore, text, this.SeriesLabels.TextAfter);
        }

        private string GetArgument(IStiSeries series, string argument)
        {
            string text = argument;
            if (series.Core.IsDateTimeArguments)
            {
                DateTime dateArgument;
                if (DateTime.TryParse(argument, out dateArgument))
                {
                    text = GetFormatted(series, dateArgument.ToOADate(), true);
                }
            }


            return text;
        }

        private string GetFormatted(IStiSeries series, double? value, bool isDateTime)
        {
            CultureInfo storedCulture = null;

            try
            {
                object objectValue = value;
                if (isDateTime) objectValue = DateTime.FromOADate(value.Value);

                #region DBS

                var pieLabels = this.SeriesLabels as StiPieSeriesLabels;
                var axisLabels = this.SeriesLabels as StiAxisSeriesLabels;

                if (this.SeriesLabels.FormatService != null && 
                    !(this.SeriesLabels.FormatService is StiGeneralFormatService) && 
                    !(pieLabels != null && pieLabels.ShowInPercent) &&
                    !(axisLabels != null && axisLabels.ShowInPercent))
                {
                    return this.SeriesLabels.FormatService.Format(objectValue);
                }
                #endregion

                var culture = ((StiChart) this.SeriesLabels.Chart)?.Report?.GetParsedCulture();
                if (!string.IsNullOrEmpty(culture))
                {
                    storedCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                }

                string format = null;
                if (series != null) format = series.Format;
                if (format == null || format.Trim().Length == 0) format = this.SeriesLabels.Format;

                if (format != null && format.Trim().Length != 0)
                {
                    if (format.StartsWith("P", StringComparison.InvariantCulture) && this.SeriesLabels.Chart != null &&
                        !StiChartOptions.OldChartPercentMode)
                    {
                        int decimals = 0;
                        double percentValue;
                        try
                        {
                            if (format.Length > 1) decimals = Convert.ToInt32(format.Remove(0, 1));
                            percentValue = RecalcValue(value.Value, decimals);
                        }
                        catch
                        {
                            percentValue = RecalcValue(value.Value, 2);
                        }

                        return string.Format("{0:N" + decimals.ToString() + "}{1}", percentValue, "%");
                    }
                    else
                    {
                        String returnText = string.Format("{0:" + format + "}", objectValue);

                        if (!isDateTime && returnText == format) return objectValue.ToString();
                        return returnText;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (storedCulture != null)
                    Thread.CurrentThread.CurrentCulture = storedCulture;
            }
            return RecalcValue(value.Value, 2).ToString();
        }

        protected virtual string GetFormattedValue(IStiSeries series, double? value)
        {
            if (value == null)
                return string.Empty;

            return GetFormatted(series, value, series.Core.IsDateTimeValues);
        }

        protected internal StiStringFormatGeom GetStringFormatGeom(StiContext context)
        {
            StiStringFormatGeom sf = context.GetGenericStringFormat();
            sf.Trimming = StringTrimming.None;
            if (!this.SeriesLabels.WordWrap)
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            return sf;
        }
        
        #endregion

        public StiSeriesLabelsCoreXF(IStiSeriesLabels seriesLabels)
        {
            this.seriesLabels = seriesLabels;
        }
    }
}
