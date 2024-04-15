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
using Stimulsoft.Base.Drawing;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiRadarSeriesCoreXF : StiSeriesCoreXF
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            IStiRadarSeries radarSeries = this.Series as IStiRadarSeries;

            if (radarSeries.AllowApplyStyle)
            {
                //radarSeries.CoreBrush = style.Core.GetColumnBrush(color);
                //radarSeries.CoreBorderColor = style.Core.GetColumnBorder(color);

                if (radarSeries.Marker != null)
                {
                    radarSeries.Marker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 100));
                    radarSeries.Marker.BorderColor = StiColorUtils.Dark(color, 100);
                }
            }
        }
        #endregion

        #region Methods

        private List<List<PointF?>> GetPointsList(double?[] values, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            var pointsList = new List<List<PointF?>>();

            var area = geom.Area as IStiRadarArea;
            var core = area.Core as StiRadarAreaCoreXF;

            foreach (IStiRadarSeries series in seriesArray)
            {
                var points = new List<PointF?>();
                pointsList.Add(points);

                int pointIndex = 0;
                foreach (PointF? curPoint in core.Points)
                {
                    double? value = pointIndex < series.Values.Length ? series.Values[pointIndex] : null;

                    if (value == null && series.ShowNulls)
                        value = 0d;

                    if (value == null)
                    {
                        points.Add(null);
                    }
                    else
                    {

                        if (value != null)
                            value = (value - area.YAxis.Info.Minimum) * area.YAxis.Info.Dpi;

                        var point = new PointF(
                            (float)(core.CenterPoint.X + value.GetValueOrDefault() * curPoint.Value.X),
                            (float)(core.CenterPoint.Y + value.GetValueOrDefault() * curPoint.Value.Y));

                        point.X -= geom.ClientRectangle.X;
                        point.Y -= geom.ClientRectangle.Y;

                        points.Add(point);
                    }
                    pointIndex++;
                }
            }

            return pointsList;
        }

        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            if (seriesArray == null || seriesArray.Length == 0 || this.Series.Chart == null) return;

            bool isAnimationChangingValues = ((StiChart)this.Series.Chart).IsAnimationChangingValues;

            var pointsList = GetPointsList(this.Series.Values, geom, seriesArray);
            var pointsListFrom = GetPointsList(this.Series.ValuesStart, geom, seriesArray);


            int seriesIndex = 0;
            foreach (IStiRadarSeries series in seriesArray)
            {
                var points = pointsList[seriesIndex];
                points.Add(points[0]);

                List<string> pointsIds;
                if (series.Arguments.Length < points.Count - 1) 
                    pointsIds = points.Select((p, i) => i.ToString()).ToList();
                else 
                    pointsIds = series.Arguments.Select(p => p == null ? "" : p.ToString()).ToList();
                
                pointsIds.Add(pointsIds[0] + "_e");

                var pointsFrom = pointsListFrom[seriesIndex];
                pointsFrom.Add(pointsFrom[0]);

                var centerPoint = new PointF(geom.ClientRectangle.Width / 2, geom.ClientRectangle.Height / 2);

                var pointsInfo = new StiSeriesPointsInfo()
                {
                    PointsFrom = pointsFrom.Select(p => p != null ? p : centerPoint).ToArray(),
                    Points = points.Select(p => p != null ? p : centerPoint).ToArray(),
                    PointsIds = pointsIds.ToArray()
                };

                RenderAreas(context, series, pointsInfo, geom);

                RenderLines(context, series, pointsInfo, geom);

                points.RemoveAt(points.Count - 1);

                RenderPoints(context, series, points, geom);

                points.RemoveAt(points.Count - 1);
                seriesIndex++;
            }
        }

        public virtual void RenderAreas(StiContext context, IStiRadarSeries series, StiSeriesPointsInfo pointsInfo, StiAreaGeom geom)
        {
        }

        public virtual void RenderLines(StiContext context, IStiRadarSeries series, StiSeriesPointsInfo pointsInfo, StiAreaGeom geom)
        {
        }

        public virtual void RenderPoints(StiContext context, IStiRadarSeries series, List<PointF?> points, StiAreaGeom geom)
        {
            int pointIndex = 0;
            foreach (PointF? point in points)
            {
                if (point != null)
                {
                    double? value = pointIndex < series.Values.Length ? series.Values[pointIndex] : null;

                    var radarGeom = new StiRadarPointSeriesElementGeom(geom, value.GetValueOrDefault(), pointIndex, series.Marker.Brush, series, point.Value, context.Options.Zoom);

                    if (geom != null)
                    {
                        geom.CreateChildGeoms();
                        geom.ChildGeoms.Add(radarGeom);
                    }

                    #region Draw Series Labels
                    var labels = series.Core.GetSeriesLabels();

                    if (labels != null && labels.Visible && value != null)
                    {
                        var startPoint = point.GetValueOrDefault();

                        if (labels.Step == 0 || (pointIndex % labels.Step == 0))
                        {
                            var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF)labels.Core).RenderLabel(series, context,
                                startPoint,
                                startPoint,
                                pointIndex, value, value,
                                GetArgument(series, pointIndex),
                                series.Core.GetTag(pointIndex),
                                0, 1, geom.ClientRectangle);

                            if (seriesLabelsGeom != null)
                            {
                                geom.CreateChildGeoms();
                                geom.ChildGeoms.Add(seriesLabelsGeom);
                                seriesLabelsGeom.ClientRectangle = CheckLabelsRect(labels, geom, seriesLabelsGeom.ClientRectangle);
                            }

                        }
                    }
                    #endregion                    
                }
                pointIndex++;
            }
        }

        private string GetArgument(IStiRadarSeries series, int pointIndex)
        {
            if (series.Arguments.Length > pointIndex && series.Arguments[pointIndex] != null)
                return series.Arguments[pointIndex].ToString();
            else
                return string.Empty;
        }

        public override StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            var brush = base.GetSeriesBrush(colorIndex, colorCount);
            if (brush == null) return new StiSolidBrush(StiColorUtils.Dark(Color.White, 20));
            return brush;
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            object color = base.GetSeriesBorderColor(colorIndex, colorCount);
            if (color == null) return Color.Black;
            return color;
        }
        #endregion

        public StiRadarSeriesCoreXF(IStiSeries series)
            : base(series)
        {
        }
    }
}
