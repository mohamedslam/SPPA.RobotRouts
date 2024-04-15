#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using System.Drawing;
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using System.Data;
using Stimulsoft.Base.Helpers;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Chart
{
    public class StiSunburstSeriesCoreXF : StiSeriesCoreXF
    {
        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesCollection)
        {
            var dataTable = this.GetDataTable(seriesCollection);

            if (dataTable.Rows.Count < 1)
            {
                var radiusEmpty = GetRadius(context, rect);
                var pointCenterEmpty = GetPointCenter(rect);

                var rectEmpty = new RectangleF(pointCenterEmpty.X - radiusEmpty, pointCenterEmpty.Y - radiusEmpty, 2 * radiusEmpty, 2 * radiusEmpty);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(new StiSunburstEmptySeriesElementGeom(rectEmpty));
                return;
            }

            var indexValueColumn = dataTable.Columns.Count - 1;
            for (var indexRow = 0; indexRow < dataTable.Rows.Count; indexRow++)
            {
                // Set Arguments
                for (var indexSeries = 0; indexSeries < seriesCollection.Length; indexSeries++)
                {
                    if (seriesCollection.Length > 0 && seriesCollection[indexSeries].Arguments.Length > indexRow)
                        dataTable.Rows[indexRow][indexSeries + 1] = seriesCollection[indexSeries].Arguments[indexRow];
                    else
                        dataTable.Rows[indexRow][indexSeries + 1] = DBNull.Value;
                }

                // Set Values
                if (seriesCollection.Length > 0 && seriesCollection[0].Values.Length > indexRow && seriesCollection[0].Values[indexRow] != null)
                    dataTable.Rows[indexRow][indexValueColumn] = seriesCollection[0].Values[indexRow];

                else
                    dataTable.Rows[indexRow][indexValueColumn] = DBNull.Value;
            }

            var hashTables = new Dictionary<string, DataTable>();

            foreach (DataRow row in dataTable.Rows)
            {
                var argumentKey = row[1].ToString();

                if (hashTables.ContainsKey(argumentKey))
                {
                    var tableSeries = hashTables[argumentKey] as DataTable;
                    tableSeries.Rows.Add(row.ItemArray);
                }
                else
                {
                    var table = dataTable.Clone() as DataTable;
                    table.Rows.Add(row.ItemArray);
                    hashTables.Add(argumentKey, table);
                }
            }

            var radius = GetRadius(context, rect);
            var pointCenter = GetPointCenter(rect);
            var gradPerValue = GetGradPerValue(dataTable);

            if (float.IsInfinity(gradPerValue))
                return;

            var starAngle = 0f;
            var colorIndex = 0;

            var labelList = new List<StiSeriesLabelsGeom>();

            foreach (DataTable seriesTable in hashTables.Values)
            {
                var sumCurrentSeriesTable = GetSumColumn(seriesTable, "Value");
                var arcWidth = (float)(gradPerValue * sumCurrentSeriesTable);

                RenderComputeSeries(context, geom, seriesTable, pointCenter, radius, starAngle, gradPerValue, colorIndex, hashTables.Values.Count, ref labelList);

                starAngle += arcWidth;
                colorIndex++;
            }

            foreach (var label in labelList)
            {
                if (label != null)
                {
                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(label);
                }
            }
        }

        private void RenderComputeSeries(StiContext context, StiAreaGeom geom, DataTable seriesTable, PointF center,
            float radius, float startAngle, float gradPerValue, int colorIndex, int colorCount, ref List<StiSeriesLabelsGeom> labelGeoms)
        {
            var level = 1;
            float stepRadius = radius / (seriesTable.Columns.Count - 1);

            for (var indexArgumentColumn = 1; indexArgumentColumn < seriesTable.Columns.Count - 1; indexArgumentColumn++)
            {
                var argumentValueHash = new Dictionary<object, double?>();
                foreach (DataRow row in seriesTable.Rows)
                {
                    var argument = row[indexArgumentColumn];
                    var value = StiValueHelper.TryToNullableDouble(row[seriesTable.Columns.Count - 1]);

                    if (argumentValueHash.ContainsKey(argument) && value != null)
                    {
                        var valueOld = StiValueHelper.TryToNullableDouble(argumentValueHash[argument]).GetValueOrDefault();
                        argumentValueHash[argument] = Math.Abs(valueOld) + Math.Abs(value.GetValueOrDefault());
                    }

                    else if (argument is DBNull)
                    {
                        argumentValueHash[$"*StiGuid:{StiGuidUtils.NewGuid()}"] = value;
                    }

                    else
                    {
                        argumentValueHash[argument] = value;
                    }
                }

                var arguments = new object[argumentValueHash.Keys.Count];
                argumentValueHash.Keys.CopyTo(arguments, 0);

                var values = new double?[argumentValueHash.Values.Count];
                argumentValueHash.Values.CopyTo(values, 0);

                RenderLevelSeries(context, geom, arguments, values, center, stepRadius * level, stepRadius * (level + 1), startAngle, gradPerValue, colorIndex, colorCount, level);
                var labels = RenderLevelSeriesLebels(context, arguments, values, center, stepRadius * level, stepRadius * (level + 1), startAngle, gradPerValue, level);

                labelGeoms.AddRange(labels);

                level++;
            }
        }

        private void RenderLevelSeries(StiContext context, StiAreaGeom geom, object[] arguments, double?[] values,
            PointF center, float radius, float radiusDt, float startAngle, float gradPerValue, int colorIndex, int colorCount, int level)
        {
            var borderColor = (Color)this.Series.Core.GetSeriesBorderColor(colorIndex, colorCount);
            var seriesBrush = this.Series.Core.GetSeriesBrush(colorIndex, colorCount);
            var numberValues = values.Select(v => v);

            if (borderColor.A == 0)
            {
                borderColor = Color.White;
            }

            var duration = StiChartHelper.GlobalDurationElement;

            for (var index = 0; index < numberValues.Count(); index++)
            {
                var value = numberValues.ToList()[index];
                var argument = arguments[index];

                float arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                if (!string.IsNullOrEmpty(argument.ToString()) && !argument.ToString().StartsWith("*StiGuid:"))
                {
                    var doughnutElementGeom =
                        RenderSunburstElement(center, radius, radiusDt, borderColor, seriesBrush, startAngle, arcWidth, value, level, colorIndex, index, geom, new TimeSpan(duration.Ticks / (colorCount + numberValues.Count()) * (index + colorIndex)));

                    if (doughnutElementGeom != null)
                    {
                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(doughnutElementGeom);
                    }
                }

                startAngle += arcWidth;
            }
        }

        private List<StiSeriesLabelsGeom> RenderLevelSeriesLebels(StiContext context, object[] arguments, double?[] values,
            PointF center, float radius, float radiusDt, float startAngle, float gradPerValue, int level)
        {
            var resultList = new List<StiSeriesLabelsGeom>();
            var numberValues = values.Select(v => v);

            for (var index = 0; index < numberValues.Count(); index++)
            {
                var value = numberValues.ToList()[index];
                var argument = arguments[index];

                float arcWidth = (float)(gradPerValue * Math.Abs(value.GetValueOrDefault()));

                if (!string.IsNullOrEmpty(argument.ToString()) && !argument.ToString().StartsWith("*StiGuid:"))
                {
                    IStiSeriesLabels seriesLabels = null;

                    var currentSeries = level - 1 < this.Series.Chart.Series.Count
                        ? this.Series.Chart.Series[level - 1]
                        : this.Series;

                    if (currentSeries.ShowSeriesLabels == StiShowSeriesLabels.FromChart)
                        seriesLabels = this.Series.Chart.SeriesLabels;
                    if (currentSeries.ShowSeriesLabels == StiShowSeriesLabels.FromSeries)
                        seriesLabels = currentSeries.SeriesLabels;


                    if (this.Series.Chart != null && seriesLabels != null && seriesLabels.Visible)
                    {
                        var labels = seriesLabels as IStiPieSeriesLabels;

                        if (labels != null && labels.Visible && (labels.Step == 0 || (index % labels.Step == 0)))
                        {
                            RectangleF measureRect;

                            var seriesLabelsGeom =
                                ((StiPieSeriesLabelsCoreXF)labels.Core).RenderLabel(this.Series, context, center, radius, radiusDt,
                                    startAngle + arcWidth / 2,
                                    index, Math.Abs(value.GetValueOrDefault()), value,
                                    argument.ToString(),
                                    GetTag(index), false,
                                    0, 1, gradPerValue, out measureRect, false, 0);

                            resultList.Add(seriesLabelsGeom);
                        }
                    }
                }

                startAngle += arcWidth;
            }

            return resultList;
        }

        private StiSunburstSeriesElementGeom RenderSunburstElement(PointF center, float radius, float radiusDt,
            Color borderColor, StiBrush brush, float start, float angle, double? value, int index1, int index2, int index3, StiAreaGeom areaGeom, TimeSpan beginTime)
        {
            if (angle == 0 || float.IsNaN(angle)) return null;

            var rectSunburst = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            var rectSunbutsyDt = new RectangleF(center.X - radiusDt, center.Y - radiusDt, radiusDt * 2, radiusDt * 2);

            if (rectSunburst.Width <= 0 && rectSunburst.Height <= 0) return null;

            var path = new List<StiSegmentGeom>();
            path.Add(new StiArcSegmentGeom(rectSunburst, start, angle));
            path.Add(new StiLineSegmentGeom(GetPoint(center, radius, start + angle), GetPoint(center, radiusDt, start + angle)));
            path.Add(new StiArcSegmentGeom(rectSunbutsyDt, start + angle, -angle));
            path.Add(new StiLineSegmentGeom(GetPoint(center, radiusDt, start), GetPoint(center, radius, start)));
            path.Add(new StiCloseFigureSegmentGeom());

            var seriesGeom = new StiSunburstSeriesElementGeom(areaGeom, value.GetValueOrDefault(), index1, index2, index3, this.Series, rectSunburst, rectSunbutsyDt, path,
                    borderColor, brush, start, start + angle, radius, radiusDt, beginTime);

            return seriesGeom;
        }

        protected PointF GetPoint(PointF centerPie, float radius, float angle)
        {
            float angleRad = (float)(Math.PI * angle / 180);
            return new PointF(
                centerPie.X + (float)Math.Cos(angleRad) * radius,
                centerPie.Y + (float)Math.Sin(angleRad) * radius);
        }

        private DataTable GetDataTable(IStiSeries[] seriesCollection)
        {
            var table = new DataTable();

            var idColumn = new DataColumn("Id", Type.GetType("System.Int32"));
            idColumn.Unique = true;
            idColumn.AllowDBNull = false;
            idColumn.AutoIncrement = true;
            idColumn.AutoIncrementSeed = 0;
            idColumn.AutoIncrementStep = 1;
            table.Columns.Add(idColumn);

            for (var index = 0; index < seriesCollection.Length; index++)
            {
                var argumentColumn = new DataColumn($"Argument-{index}", typeof(string));
                argumentColumn.AllowDBNull = true;
                table.Columns.Add(argumentColumn);
            }

            var valueColumn = new DataColumn("Value", typeof(double));
            valueColumn.AllowDBNull = true;
            table.Columns.Add(valueColumn);


            var rowCount = GetCountRow(seriesCollection);
            for (var index = 0; index < rowCount; index++)
                table.Rows.Add();

            return table;
        }

        private int GetCountRow(IStiSeries[] seriesCollection)
        {
            var count = 0;
            foreach (var series in seriesCollection)
            {
                count = Math.Max(count, series.Arguments.Length);
            }

            return count;
        }

        private float GetGradPerValue(DataTable dataTable)
        {
            var totals = GetSumColumn(dataTable, "Value");// (double)dataTable.Compute("Sum(Value)", string.Empty);
            return (float)(360 / totals);
        }

        protected float GetRadius(StiContext context, RectangleF rect)
        {
            return Math.Min(rect.Width / 2, rect.Height / 2) * 0.95f;
        }

        protected PointF GetPointCenter(RectangleF rect)
        {
            return new PointF(rect.Width / 2, rect.Height / 2);
        }

        private double GetSumColumn(DataTable dataTable, string columnName)
        {
            return dataTable.AsEnumerable().Sum(row => Math.Abs(row.Field<double?>(columnName).GetValueOrDefault()));
        }
        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Sunburst");
            }
        }
        #endregion

        public StiSunburstSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
