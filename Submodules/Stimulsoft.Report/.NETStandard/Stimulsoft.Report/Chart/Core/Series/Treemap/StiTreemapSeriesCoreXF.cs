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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public class StiTreemapSeriesCoreXF : StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var treemapSeries = this.Series as IStiTreemapSeries;

            if (treemapSeries.AllowApplyStyle)
            {
                treemapSeries.Brush = style.Core.GetColumnBrush(color);
                treemapSeries.BorderColor = style.Core.GetColumnBorder(color);
                treemapSeries.BorderThickness = style.Core.SeriesBorderThickness;
                treemapSeries.CornerRadius = style.Core.SeriesCornerRadius;
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            var duration = StiChartHelper.GlobalDurationElement;
            var beginTime = StiChartHelper.GlobalBeginTimeElement;

            var listValues = new List<double>();
            foreach (var value in this.Series.Values.ToList())
                listValues.Add(value.GetValueOrDefault());

            rect.Inflate(-1, -1);

            var normalizeData = ((StiTreemapAreaCoreXF)this.Series.Chart.Area.Core).NormalizeDataForArea(listValues, rect.Width * rect.Height);

            var rects = ((StiTreemapAreaCoreXF)this.Series.Chart.Area.Core).Squarify(normalizeData, new List<double?>(), rect, new List<RectangleF>());

            var indexColor = 0;

            foreach (var ser in seriesArray)
            {
                if (ser == this.Series) break;

                indexColor += ser.Values.Length;
            }

            var indexColorCount = seriesArray.Sum(x => x.Values.Length);

            var listLabels = new List<StiSeriesLabelsGeom>();

            var rectIndex = -1;
            for (int index = 0; index < listValues.Count; index++)
            {
                if (listValues[index] == 0)
                    continue;

                rectIndex++;

                #region Draw Box
                var seriesBrush = this.Series.Core.GetSeriesBrush(indexColor, indexColorCount);
                seriesBrush = this.Series.ProcessSeriesBrushes(indexColor, seriesBrush);

                var seriesBorderColor = (Color)this.Series.Core.GetSeriesBorderColor(indexColor, indexColorCount);

                if (seriesBorderColor.A == 0 && !this.Series.Chart.Area.ColorEach)
                {
                    seriesBorderColor = Color.White;
                }

                var beginTimeAnimationOpacity = new TimeSpan(beginTime.Ticks / rects.Count * rectIndex);
                var animation = new StiOpacityAnimation(duration, beginTimeAnimationOpacity);

                var seriesTreemapElementGeom = new StiTreemapSeriesElementGeom(geom, this.Series.Values[index].GetValueOrDefault(), index, 
                    seriesBrush, seriesBorderColor, this.Series, rects[rectIndex], animation);

                if (this.Series.Core.Interaction != null)
                    seriesTreemapElementGeom.Interaction = new StiSeriesInteractionData(geom.Area, this.Series, rectIndex);

                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(seriesTreemapElementGeom);
                #endregion

                #region Render Series Label
                var labels = this.Series.Chart.SeriesLabels as StiCenterTreemapLabels;
                if (labels != null && labels.Visible)
                {
                    var seriesLabelsGeom = ((StiCenterTreemapLabelsCoreXF)labels.Core).RenderLabel(this.Series, context,
                                        rectIndex,
                                        this.Series.Values[index].GetValueOrDefault(),
                                        GetArgumentText(this.Series, index), this.Series.Core.GetTag(index),
                                        indexColor, indexColorCount, rects[rectIndex], null);

                    if (seriesLabelsGeom != null)
                        listLabels.Add(seriesLabelsGeom);

                }
                #endregion

                indexColor++;
            }

            foreach (var labelGeom in listLabels)
            {
                geom.CreateChildGeoms();
                geom.ChildGeoms.Add(labelGeom);
            }

            if (geom.Area.Chart.SeriesLabels.PreventIntersection)
            {
                this.CheckIntersectionLabels(geom);
            }
        }

        private string GetArgumentText(IStiSeries series, int index)
        {
            if (series.Arguments.Length > index && series.Arguments[index] != null)
            {
                return series.Arguments[index].ToString();
            }
            return string.Empty;
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            StiBrush brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return ((IStiTreemapSeries)this.Series).Brush;
            return brush;
        }


        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return ((IStiTreemapSeries)this.Series).BorderColor;
            return color;
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
                return StiLocalization.Get("Chart", "Treemap");
            }
        }
        #endregion

        public StiTreemapSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
