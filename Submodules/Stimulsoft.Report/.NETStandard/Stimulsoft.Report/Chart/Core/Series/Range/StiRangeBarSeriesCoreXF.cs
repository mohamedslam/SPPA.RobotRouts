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
    public class StiRangeBarSeriesCoreXF : StiClusteredColumnSeriesCoreXF
    {
        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] series)
        {
            var seriesLabelsList = new List<StiSeriesLabelsGeom>();

            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            var rangeArea = geom.Area as IStiRangeBarArea;

            float posX = rangeArea.AxisCore.GetDividerX();
            float posY = rangeArea.AxisCore.GetDividerY();

            foreach (IStiRangeBarSeries currSeries in series)
            {
                int valuesCount = currSeries.Values.Length;
                if (currSeries.ValuesEnd.Length < valuesCount) valuesCount = currSeries.ValuesEnd.Length;

                int argumentsCount = currSeries.Arguments.Length;
                int count = Math.Min(valuesCount, argumentsCount);
                int colorIndex = rangeArea.ReverseHor ? count - 1 : 0;

                int index = 0;

                while (index < count)
                {
                    double? nullableValue = currSeries.Values[index];
                    double? nullableValueEnd = currSeries.ValuesEnd[index];
                    if (nullableValue == null) nullableValue = 0d;
                    if (nullableValueEnd == null) nullableValueEnd = 0d;

                    double seriesHeight = Math.Abs(nullableValue.Value - nullableValueEnd.Value) * rangeArea.YAxis.Info.Dpi;
                    double seriesTopPos = posY;

                    double value = Math.Max(nullableValue.Value, nullableValueEnd.Value);

                    if (rangeArea.ReverseVert)
                        seriesTopPos += Math.Min(nullableValue.Value, nullableValueEnd.Value) * rangeArea.YAxis.Info.Dpi;
                    else
                        seriesTopPos -= value * rangeArea.YAxis.Info.Dpi;

                    int argumentIndex = rangeArea.XAxis.Info.StripLines.Count - 1;
                    foreach (StiStripLineXF line in rangeArea.XAxis.Info.StripLines)
                    {
                        if (currSeries.Arguments[index].ToString() == line.ValueObject?.ToString()) break;
                        argumentIndex--;
                    }

                    argumentIndex = rangeArea.XAxis.Info.StripLines.Count - 1 - argumentIndex;

                    float seriesWidth = (rangeArea.XAxis.Info.Step - rangeArea.XAxis.Info.Step * (1f - currSeries.Width));
                    float seriesLeftPos = posX - seriesWidth / 2;

                    seriesLeftPos += rangeArea.XAxis.Info.Step * argumentIndex;

                    var seriesRect = new RectangleF(seriesLeftPos, (float)seriesTopPos, seriesWidth, (float)seriesHeight);
                    var seriesRectFrom = RectangleF.FromLTRB(seriesRect.Left, seriesRect.Bottom, seriesRect.Right, seriesRect.Bottom);

                    var seriesBrush = currSeries.Core.GetSeriesBrush(colorIndex, count);
                    var brush = currSeries.ProcessSeriesBrushes(colorIndex, seriesBrush);

                    var seriesGeom = new StiRangeBarElementGeom(geom, value, index, currSeries, brush, seriesRect, seriesRectFrom);

                    if (currSeries.Core.Interaction != null)
                        seriesGeom.Interaction = new StiSeriesInteractionData(rangeArea, currSeries, index);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(seriesGeom);

                    #region Render Series Labels
                    var labels = currSeries.Core.GetSeriesLabels();

                    if (labels != null && labels.Visible)
                    {
                        PointF pointValueEndLabels = new PointF(seriesLeftPos + seriesWidth / 2, (float)seriesTopPos);
                        PointF pointValueLabels = new PointF(seriesLeftPos + seriesWidth / 2, (float)seriesTopPos + (float)seriesHeight);

                        if (labels.Step == 0 || (index % labels.Step == 0))
                        {
                            if (nullableValueEnd < nullableValue)
                            {
                                double? temp = nullableValue;
                                nullableValue = nullableValueEnd;
                                nullableValueEnd = temp;
                            }

                            var seriesValueLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currSeries, context,
                                pointValueLabels,
                                pointValueLabels,
                                index, nullableValue, nullableValue,
                                currSeries.Arguments[index].ToString(),
                                currSeries.Core.GetTag(index),
                                0, 1, rect);

                            var seriesValueEndLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(currSeries, context,
                                pointValueEndLabels,
                                pointValueEndLabels,
                                index, nullableValueEnd, nullableValueEnd,
                                currSeries.Arguments[index].ToString(),
                                currSeries.Core.GetTag(index),
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
                    #endregion

                    colorIndex = rangeArea.ReverseHor ? colorIndex - 1 : colorIndex + 1;

                    index++;
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
                return StiLocalization.Get("Chart", "RangeBar");
            }
        }
        #endregion        

        public StiRangeBarSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
