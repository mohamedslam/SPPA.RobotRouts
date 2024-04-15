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
using System.Drawing;
using System.Collections.Generic;

using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiGanttSeriesCoreXF : StiClusteredBarSeriesCoreXF
    {
        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            List<StiSeriesLabelsGeom> seriesLabelsList = new List<StiSeriesLabelsGeom>();

            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            var ganttArea = geom.Area as IStiGanttArea;

            double posX = ganttArea.AxisCore.GetDividerXD();
            float posY = ganttArea.AxisCore.GetDividerY();

            bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;

            foreach (IStiGanttSeries currSeries in series)
            {
                int valuesCount = currSeries.Values.Length;
                if (currSeries.ValuesEnd.Length < valuesCount) valuesCount = currSeries.ValuesEnd.Length;

                int argumentsCount = currSeries.Arguments.Length;

                int count = Math.Min(valuesCount, argumentsCount);
                int colorIndex = argumentsCount - 1;

                int pointIndex = 0;
                while (pointIndex < count)
                {
                    double? nullableValue = currSeries.Values[pointIndex];
                    double? nullableValueEnd = currSeries.ValuesEnd[pointIndex];
                    if (nullableValue == null) nullableValue = 0d;
                    if (nullableValueEnd == null) nullableValueEnd = 0d;

                    double seriesWidth = Math.Abs(nullableValue.Value - nullableValueEnd.Value) * ganttArea.XAxis.Info.Dpi;
                    double seriesLeftPos = posX;

                    double value = Math.Min(nullableValue.Value, nullableValueEnd.Value);

                    if (ganttArea.ReverseHor) seriesLeftPos -= value * ganttArea.XAxis.Info.Dpi + seriesWidth;
                    else seriesLeftPos += value * ganttArea.XAxis.Info.Dpi;

                    int argumentIndex = ganttArea.YAxis.Info.StripLines.Count - 1;
                    foreach (StiStripLineXF line in ganttArea.YAxis.Info.StripLines)
                    {
                        if (line.ValueObject != null && currSeries.Arguments[pointIndex].ToString() == line.ValueObject.ToString()) break;
                        argumentIndex--;
                    }

                    if (ganttArea.ReverseVert)
                        argumentIndex = ganttArea.YAxis.Info.StripLines.Count - 1 - argumentIndex;

                    float seriesHeight = ganttArea.YAxis.Info.Step / 2;
                    float seriesTopPos = posY - seriesHeight / 2;

                    if (ganttArea.ReverseVert) seriesTopPos += ganttArea.YAxis.Info.Step * argumentIndex;
                    else seriesTopPos -= ganttArea.YAxis.Info.Step * argumentIndex;

                    var seriesRect = new RectangleF((float)seriesLeftPos, seriesTopPos, (float)seriesWidth, seriesHeight);
                    var seriesRectFrom = RectangleF.FromLTRB(seriesRect.Left, seriesRect.Top, seriesRect.Left, seriesRect.Bottom);

                    var seriesBrush = currSeries.Core.GetSeriesBrush(colorIndex, count);
                    var brush = currSeries.ProcessSeriesBrushes(colorIndex, seriesBrush);

                    var seriesGeom = new StiGanttSeriesElementGeom(geom, value, pointIndex, currSeries, seriesRect, seriesRectFrom, brush);

                    if (currSeries.Core.Interaction != null)
                        seriesGeom.Interaction = new StiSeriesInteractionData(ganttArea, currSeries, pointIndex);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(seriesGeom);

                    #region Render Series Labels
                    IStiAxisSeriesLabels labels = currSeries.Core.GetSeriesLabels();

                    if (labels != null && labels.Visible)
                    {
                        if (labels is StiValueAxisLabels)
                        {
                            PointF pointValueEndLabels = new PointF((float)seriesLeftPos, seriesTopPos + seriesHeight / 2);
                            PointF pointValueLabels = new PointF((float)seriesLeftPos + (float)seriesWidth, seriesTopPos + seriesHeight / 2);

                            if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                            {
                                if (nullableValueEnd < nullableValue)
                                {
                                    double? temp = nullableValue;
                                    nullableValue = nullableValueEnd;
                                    nullableValueEnd = temp;
                                }

                                StiSeriesLabelsGeom seriesValueLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currSeries, context,
                                    pointValueLabels,
                                    pointValueLabels,
                                    pointIndex, nullableValueEnd, nullableValueEnd,
                                    currSeries.Arguments[pointIndex].ToString(),
                                    currSeries.Core.GetTag(pointIndex),
                                    0, 1, rect);

                                StiSeriesLabelsGeom seriesValueEndLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currSeries, context,
                                    pointValueEndLabels,
                                    pointValueEndLabels,
                                    pointIndex, nullableValue, nullableValue,
                                    currSeries.Arguments[pointIndex].ToString(),
                                    currSeries.Core.GetTag(pointIndex),
                                    0, 1, rect);

                                if (seriesValueEndLabelsGeom != null)
                                {
                                    seriesLabelsList.Add(seriesValueEndLabelsGeom);
                                    seriesValueEndLabelsGeom.ClientRectangle = CheckLabelsRect(labels, geom, seriesValueEndLabelsGeom.ClientRectangle);
                                }
                                if (seriesValueLabelsGeom != null)
                                {
                                    seriesLabelsList.Add(seriesValueLabelsGeom);
                                    seriesValueLabelsGeom.ClientRectangle = CheckLabelsRect(labels, geom, seriesValueLabelsGeom.ClientRectangle);
                                }
                            }
                        }
                        else if (!(labels is StiNoneLabels))
                        {
                            PointF pointValueEndLabels;
                            PointF pointValueLabels;

                            if (labels is StiCenterAxisLabels)
                            {
                                pointValueEndLabels = new PointF((float)seriesLeftPos, seriesTopPos + seriesHeight / 2);
                                pointValueLabels = new PointF((float)seriesLeftPos + (float)seriesWidth, seriesTopPos + seriesHeight / 2);
                            }
                            else
                            {
                                pointValueEndLabels = new PointF((float)seriesLeftPos + (float)seriesWidth / 2, seriesTopPos);
                                pointValueLabels = new PointF((float)seriesLeftPos + (float)seriesWidth / 2, seriesTopPos);
                            }

                            double? seriesValue = value;

                            if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                            {
                                StiSeriesLabelsGeom seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currSeries, context,
                                    pointValueLabels,
                                    pointValueEndLabels,
                                    pointIndex, value, seriesValue,
                                    currSeries.Arguments[pointIndex]?.ToString(),
                                    currSeries.Core.GetTag(pointIndex),
                                    0, 1, rect);

                                if (seriesLabelsGeom != null)
                                {
                                    seriesLabelsList.Add(seriesLabelsGeom);
                                    seriesLabelsGeom.ClientRectangle = CheckLabelsRect(labels, geom, seriesLabelsGeom.ClientRectangle);
                                }
                            }
                        }
                    }
                    #endregion

                    colorIndex = colorIndex - 1;

                    pointIndex++;
                }
            }
            #region Draw Series Labels in second path
            foreach (StiSeriesLabelsGeom seriesLabelsGeom in seriesLabelsList)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesLabelsGeom);
            }
            #endregion
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
                return StiLocalization.Get("Chart", "Gantt");
            }
        }
        #endregion

        public StiGanttSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
