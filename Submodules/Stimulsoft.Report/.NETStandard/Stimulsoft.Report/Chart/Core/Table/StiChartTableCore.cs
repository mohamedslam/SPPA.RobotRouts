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
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiChartTableCore : ICloneable, IStiApplyStyle
    {
        public StiChartTableCore(IStiChartTable table)
        {
            this.ChartTable = table;
        }

        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (this.ChartTable.AllowApplyStyle)
            {
                this.ChartTable.GridLineColor = style.Core.LegendBorderColor;
                this.ChartTable.TextColor = style.Core.LegendLabelsColor;
            }
        }
        #endregion	

        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Properties
        public IStiChartTable ChartTable { get; set; }
        #endregion

        #region Constants
        internal const float WidthSpace = 2f;
        #endregion

        #region Methods

        public bool ShowTable()
        {
            if ((ChartTable.Chart.Area is IStiClusteredColumnArea || ChartTable.Chart.Area is IStiStackedColumnArea) &&
                !(ChartTable.Chart.Area is IStiCandlestickArea) &&
                !(ChartTable.Chart.Area is IStiRangeArea) &&
                !(ChartTable.Chart.Area is IStiScatterArea) &&
                !(ChartTable.Chart.Area is IStiStockArea) &&
                ChartTable.Visible)
            {
                return true;
            }
            return false;
        }

        public float GetHeightTable(StiContext context, float widthTable)
        {
            var font = StiFontGeom.ChangeFontSize(ChartTable.DataCells.Font, ChartTable.DataCells.Font.Size * context.Options.Zoom);
            var size = context.MeasureString("HeightText", font);

            return size.Height * (ChartTable.Chart.Series.Count) + GetHeightHeaderTable(context, widthTable);
        }

        public float GetHeightHeaderTable(StiContext context, float widthTable)
        {
            float height = 0;

            var area = ChartTable.Chart.Area as StiAxisArea;
            var startFromZero = area.XAxis.StartFromZero;
            var listArgument = GetArguments();

            var font = StiFontGeom.ChangeFontSize(ChartTable.Header.Font, ChartTable.Header.Font.Size * context.Options.Zoom);

            #region StringFormat
            var sf = context.GetGenericStringFormat();
            sf.Trimming = StringTrimming.None;
            if (!this.ChartTable.Header.WordWrap)
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            #endregion

            int count = area.XAxis.StartFromZero ? listArgument.Count + 1 : listArgument.Count - 1;

            float cellWidth = (widthTable - GetWidthCellLegend(context)) / count;

            for (int indexColumn = 0; indexColumn < listArgument.Count; indexColumn++)
            {
                float deltaWidth = 0;
                if (indexColumn == 0 || indexColumn == listArgument.Count - 1)
                {
                    deltaWidth = startFromZero ? cellWidth / 2 : -cellWidth / 2;
                }

                var curWidth = cellWidth + deltaWidth - 2 * WidthSpace * context.Options.Zoom;
                if (curWidth < 0) curWidth = 0;

                float measureHeight = context.MeasureRotatedString(listArgument[indexColumn], font, new PointF(0, 0), sf, StiRotationMode.CenterCenter, 0, (int)curWidth).Height;

                height = Math.Max(height, measureHeight);
            }

            return height;
        }

        public float GetWidthCellLegend(StiContext context)
        {
            var font = StiFontGeom.ChangeFontSize(ChartTable.DataCells.Font, ChartTable.DataCells.Font.Size * context.Options.Zoom);
            var sf = context.GetGenericStringFormat();
            sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            float widthCell = 0;

            var rect = new RectangleF();

            foreach (StiSeries series in ChartTable.Chart.Series)
            {
                rect = context.MeasureRotatedString(series.CoreTitle, font, new PointF(0, 0), sf, StiRotationMode.CenterCenter, 0);
                widthCell = Math.Max(rect.Width, widthCell);
            }

            if (ChartTable.MarkerVisible)
                widthCell += rect.Height;

            return (float)Math.Ceiling(widthCell);
        }

        public StiCellGeom Render(StiContext context, RectangleF rect)
        {
            return new StiChartTableGeom(rect, GetTableValues(), GetWidthCellLegend(context), GetHeightHeaderTable(context, rect.Width), WidthSpace * context.Options.Zoom, ChartTable);
        }

        private int GetMaxCountValues(StiSeriesCollection series)
        {
            int result = 0;
            foreach (StiSeries stiSeries in series)
            {
                result = Math.Max(result, stiSeries.Values.Length);
            }
            return result;
        }

        private List<string> GetArguments()
        {
            var series = this.ChartTable.Chart.Series;

            var result = new List<string>();

            int countValues = GetMaxCountValues(series);

            for (int index = 0; index < countValues; index++)
            {
                var value = series[0].Arguments.Length > index
                    ? series[0].Arguments[index].ToString()
                    : (index + 1).ToString();

                var valueText = GetLabelText(value, series[0]);

                result.Add(valueText);
            }
            return result;
        }

        internal string GetLabelText(object objectValue, IStiSeries series)
        {
            CultureInfo storedCulture = null;

            try
            {
                var culture =  ((StiChart)this.ChartTable.Chart)?.Report?.GetParsedCulture();
                if (!string.IsNullOrEmpty(culture))
                {
                    storedCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                }

                string format = null;
                if (series != null) format = series.Format;
                if (format == null || format.Trim().Length == 0) format = this.ChartTable.Header.Format;

                if (format != null && format.Trim().Length != 0)
                {
                    #region If value is string try to convert it to decimal value
                    if (objectValue is string)
                    {
                        string strValue = objectValue.ToString().Replace(".", ",").Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        decimal result;
                        if (decimal.TryParse(strValue, out result))
                        {
                            objectValue = result;
                        }
                        else
                        {
                            DateTime resultDateTime;
                            var cultureName = string.IsNullOrEmpty(culture) ? "en-US" : culture;
                            if (DateTime.TryParse(objectValue.ToString(), new CultureInfo(cultureName, true), DateTimeStyles.None, out resultDateTime))
                            {
                                objectValue = resultDateTime;
                            }
                            else
                            {
                                if (DateTime.TryParse(objectValue.ToString(), out resultDateTime))
                                {
                                    objectValue = resultDateTime;
                                }
                            }
                        }
                    }
                    #endregion

                    else if (objectValue == null)
                        return string.Empty;

                    if (!StiChartOptions.OldChartPercentMode && format.StartsWith("P", StringComparison.InvariantCulture))
                    {
                        int signs = 0;
                        if (format.Length > 1)
                        {
                            int.TryParse(format.Remove(0, 1), out signs);
                        }

                        return string.Format("{0:N" + signs.ToString() + "}{1}{2}", objectValue, "%", this.ChartTable.Header.TextAfter);
                    }
                    else
                    {
                        var errorEntity = new StiErrorFormatEntity()
                        {
                            Format = format,
                            Value = objectValue
                        };

                        try
                        {
                            if (!StiErrorFormatEntity.Hash.Contains(errorEntity))
                                return string.Format("{0:" + format + "}{1}", objectValue, this.ChartTable.Header.TextAfter);
                        }
                        catch
                        {
                            StiErrorFormatEntity.Hash.Add(errorEntity);
                        }
                    }
                }
                return string.Format("{0}{1}", objectValue, this.ChartTable.Header.TextAfter);
            }
            catch
            {
            }
            finally
            {
                if (storedCulture != null)
                    Thread.CurrentThread.CurrentCulture = storedCulture;
            }
            return objectValue.ToString();
        }

        private string[,] GetTableValues()
        {
            var series = this.ChartTable.Chart.Series;
            var list = GetArguments();

            var table = new string[series.Count + 1, list.Count + 1];

            for (int index = 0; index < list.Count; index++)
            {
                table[0, index + 1] = list[index];
            }

            var area = this.ChartTable.Chart.Area as StiAxisArea;

            int indexRow = 1;
            foreach (StiSeries serie in series)
            {
                table[indexRow, 0] = serie.CoreTitle;
                int indexColumn = 1;
                for (int index = 0; index < serie.Values.Length; index++)
                {
                    double? val = area.ReverseHor ? serie.Values[serie.Values.Length - index - 1] : serie.Values[index];
                    string format = this.ChartTable.Chart.Table.Format;

                    if (format != null && format.Trim().Length != 0 && val != null)
                    {
                        if (format.StartsWith("P", StringComparison.InvariantCulture) && !StiChartOptions.OldChartPercentMode)
                        {
                            int decimals = 0;
                            double percentValue;
                            try
                            {
                                if (format.Length > 1) decimals = Convert.ToInt32(format.Remove(0, 1));
                                percentValue = serie.SeriesLabels.Core.RecalcValue(val.Value, decimals);
                            }
                            catch
                            {
                                percentValue = serie.SeriesLabels.Core.RecalcValue(val.Value, 2);
                            }

                            table[indexRow, indexColumn] = string.Format("{0:N" + decimals.ToString() + "}{1}", percentValue, "%");
                        }
                        else
                        {
                            object objectValue = val;
                            if (serie.Core.IsDateTimeValues) objectValue = DateTime.FromOADate(val.Value);

                            table[indexRow, indexColumn] = string.Format("{0:" + format + "}", objectValue);
                        }
                    }
                    else
                    {
                        table[indexRow, indexColumn] = val.ToString();
                    }

                    indexColumn++;
                }
                indexRow++;
            }

            return table;
        }
        #endregion
    }
}
