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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report.Engine
{
    /// <summary>
    /// Class helps work with chart interactions.
    /// </summary>
    public static class StiChartInteractionHelper
    {
        #region Methods.Interaction Imitation
        public static void ProcessChart(StiChart masterChart, StiChart childChart)
        {
            if (!StiOptions.Configuration.IsWeb) return;

            if (StiOptions.Engine.AllowInteractionInChartWithComponents)
            {
                var interactionPresent = false;
                foreach (IStiSeries series in childChart.Series)
                {
                    if ((series.Hyperlinks != null && series.Hyperlinks.Length > 0) ||
                        (series.Tags != null && series.Tags.Length > 0) ||
                        (series.ToolTips != null && series.ToolTips.Length > 0) ||
                        (series.Core.Interaction != null && ((StiSeriesInteraction)series.Core.Interaction).DrillDownEnabled))
                    {
                        interactionPresent = true;
                        break;
                    }
                }

                if (interactionPresent)
                {
                    var painter = new StiGdiContextPainter(StiReport.GlobalMeasureGraphics);
                    var context = new StiContext(painter, true, false, false, 1);

                    var rect = masterChart.Report.Unit.ConvertToHInches(masterChart.ClientRectangle);

                    var chartGeom = childChart.Core.Render(context, new RectangleF(0, 0, (float)rect.Width, (float)rect.Height), true);
                    var seriesGeomItems = chartGeom.GetSeriesGeoms();

                    if (seriesGeomItems.Count != 0)
                    {
                        var interactiveComps = new List<StiText>();

                        foreach (var seriesGeomItem in seriesGeomItems)
                        {
                            IStiSeries series = null;

                            #region SeriesElementGeom
                            var seriesElementGeom = seriesGeomItem as StiSeriesElementGeom;
                            if (seriesElementGeom != null)
                                series = seriesElementGeom.Series;

                            if (series != null && seriesElementGeom.Interaction != null)
                            {
                                var rectF = chartGeom.GetRect(seriesGeomItem);
                                if (seriesElementGeom is StiPieSeriesElementGeom)
                                {
                                    var pieSegment = seriesElementGeom as StiPieSeriesElementGeom;

                                    var angleRad = (float)(Math.PI * pieSegment.EndAngle / 180);

                                    var pieCenter = new PointF(rectF.X + rectF.Width / 2, rectF.Y + rectF.Height / 2);
                                    
                                    var centerPoint2 = new PointF(
                                        pieCenter.X + (float)Math.Cos(angleRad) * pieSegment.Radius / 2,
                                        pieCenter.Y + (float)Math.Sin(angleRad) * pieSegment.Radius / 2);

                                    rectF = new RectangleF(centerPoint2.X - pieSegment.Radius / 4, centerPoint2.Y - pieSegment.Radius / 4, pieSegment.Radius / 2, pieSegment.Radius / 2);
                                }

                                var rectD = new RectangleD(rectF.X, rectF.Y, rectF.Width, rectF.Height);
                                rectD = masterChart.Report.Unit.ConvertFromHInches(rectD);

                                #region Create Interaction Text Components
                                var text = CreateInteractionText(masterChart, childChart, series.Core, seriesElementGeom.Interaction, ref rectD);
                                #endregion

                                interactiveComps.Add(text);
                            }
                            #endregion

                            #region StiSeriesGeom
                            var seriesGeom = seriesGeomItem as StiSeriesGeom;
                            if (seriesGeom != null)
                                series = seriesGeom.Series;

                            if (series != null && seriesGeom != null && seriesGeom.Interactions != null)
                            {
                                var step = seriesGeom.Interactions[seriesGeom.Interactions.Count - 1].Point.GetValueOrDefault().X - seriesGeom.Interactions[0].Point.GetValueOrDefault().X;

                                step = Math.Min(step, 30);

                                var startPoint = PointF.Empty;
                                if (seriesGeom.AreaGeom != null)
                                    startPoint = chartGeom.GetRect(seriesGeom.AreaGeom).Location;

                                foreach (var data in seriesGeom.Interactions)
                                {
                                    var rectD = new RectangleD(startPoint.X + data.Point.GetValueOrDefault().X - step / 2, startPoint.Y + data.Point.GetValueOrDefault().Y - step / 2, 
                                        step, step);
                                    rectD = masterChart.Report.Unit.ConvertFromHInches(rectD);

                                    #region Create Interaction Text Components
                                    var text = CreateInteractionText(masterChart, childChart, series.Core, data, ref rectD);
                                    #endregion

                                    interactiveComps.Add(text);
                                }
                            }
                            #endregion
                        }

                        if (interactiveComps.Count > 0)
                            childChart.ChartInfoV2.InteractiveComps = interactiveComps;
                    }
                }
            }
        }

        private static StiText CreateInteractionText(StiChart masterChart, StiChart childChart, StiSeriesCoreXF series, 
            StiSeriesInteractionData interactionData, ref RectangleD rectD)
        {
            var text = new StiText(rectD)
            {
                Name = childChart.Name + "Interaction#FX%",
                ToolTipValue = interactionData.Tooltip,
                HyperlinkValue = interactionData.Hyperlink,
                TagValue = interactionData.Tag
            };

            if (series.Interaction != null && ((StiSeriesInteraction)series.Interaction).DrillDownEnabled)
            {
                text.Page = masterChart.Page;
                text.Interaction = new StiInteraction
                {
                    DrillDownEnabled = ((StiSeriesInteraction) series.Interaction).DrillDownEnabled,
                    DrillDownPageGuid = ((StiSeriesInteraction) series.Interaction).DrillDownPageGuid,
                    DrillDownReport = ((StiSeriesInteraction) series.Interaction).DrillDownReport
                };

                text.DrillDownParameters = new Dictionary<string, object>();
                text.DrillDownParameters.Add("Series", series);
                text.DrillDownParameters.Add("SeriesIndex", series.Series.Chart.Series.IndexOf(series.Series));
                text.DrillDownParameters.Add("SeriesArgument", interactionData.Argument);
                text.DrillDownParameters.Add("SeriesValue", interactionData.Value);
                text.DrillDownParameters.Add("SeriesEndValue", interactionData.EndValue);
                text.DrillDownParameters.Add("SeriesPointIndex", interactionData.PointIndex);
                text.DrillDownParameters.Add("SeriesTag", interactionData.Tag);
                text.DrillDownParameters.Add("SeriesHyperlink", interactionData.Hyperlink);
                text.DrillDownParameters.Add("SeriesTooltip", interactionData.Tooltip);
            }
            return text;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns geom cell object for specified chart component at specified x and y location.
        /// </summary>
        private static StiCellGeom GetGeomAt(StiChart chart, RectangleD chartRect, float x, float y, out StiCellGeom chartGeom, float zoom, Type typeNotUse = null)
        {
            chartGeom = null;

            x -= (float)chartRect.X;
            y -= (float)chartRect.Y;

            #region Chart Geom is Cached
            if (cachedChart == chart && cachedChartGeom != null && cachedChartRect.Equals(chartRect) && typeNotUse == null)//Not Find if typeNotUse == null, because need find on another layers
                chartGeom = cachedChartGeom;

            else
            {
                var painter = new StiGdiContextPainter(StiReport.GlobalMeasureGraphics);
                var context = new StiContext(painter, true, false, false, zoom);
                chartGeom = chart.Core.Render(context, new RectangleF(0, 0, (float) chartRect.Width, (float) chartRect.Height), true);

                cachedChart = chart;
                cachedChartGeom = chartGeom;
                cachedChartRect = chartRect;
            }
            #endregion

            return chartGeom.GetGeomAt(chartGeom, x, y, typeNotUse);
        }

        /// <summary>
        /// Returns IStiGeomInteraction object for specified chart at specified x and y location.
        /// </summary>
        public static IStiGeomInteraction GetInteraction(StiChart chart, RectangleD chartRect, float x, float y, out StiCellGeom chartGeom, float zoom, Type typeNotUse = null)
        {
            return GetGeomAt(chart, chartRect, x, y, out chartGeom, zoom, typeNotUse);
        }

        /// <summary>
        /// Resets all chart geom cache settings.
        /// </summary>
        public static void ResetChartGeomCache()
        {
            if (cachedChartGeom != null)
            {
                cachedChartGeom.Dispose();
                cachedChartGeom = null;
            }

            cachedChart = null;
        }
        #endregion

        #region Fields
        private static RectangleD cachedChartRect = RectangleD.Empty;
        private static StiChart cachedChart;
        private static StiCellGeom cachedChartGeom;
        #endregion
    }    
}
