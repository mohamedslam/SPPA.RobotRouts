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
using Stimulsoft.Base.Localization;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiLineSeriesCoreXF3D : StiSeriesCoreXF3D
    {
        #region IStiApplyStyleSeries
        public override void ApplyStyle(IStiChartStyle style, Color color)
        {
            base.ApplyStyle(style, color);

            var columnSeries = this.Series as IStiBaseLineSeries3D;

            if (columnSeries.AllowApplyStyle)
            {
                columnSeries.LineColor = color;

                //columnSeries.Lighting = style.Core.SeriesLighting;

                //if (columnSeries.Marker != null)
                //{
                //    var marker = columnSeries.Marker as StiMarker;
                //    if (marker != null && marker.ExtendedVisible != StiExtendedStyleBool.FromStyle)
                //        columnSeries.Marker.Visible = marker.ExtendedVisible == StiExtendedStyleBool.True;
                //    else
                //        columnSeries.Marker.Visible = style.Core.MarkerVisible;

                //    columnSeries.Marker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 100));
                //    columnSeries.Marker.BorderColor = StiColorUtils.Dark(color, 100);
                //}

                //if (columnSeries.LineMarker != null)
                //{
                //    columnSeries.LineMarker.Brush = new StiSolidBrush(StiColorUtils.Light(color, 50));
                //    columnSeries.LineMarker.BorderColor = StiColorUtils.Dark(color, 150);
                //}
            }
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
                return $"3D {StiLocalization.Get("Chart", "Line")}";
            }
        }
        #endregion

        #region Methods
        public override void RenderSeries3D(StiRender3D render3D, StiContext context, StiRectangle3D rect, StiAreaGeom geom, int seriesIndex, IStiSeries[] series)
        {
            var area = geom.Area;
            if (series == null || series.Length == 0 || this.Series.Chart == null) return;

            var axisArea = area as IStiAxisArea3D;

            int colorIndex = 0;
            int colorCount = axisArea.AxisCore.ValuesCount * series.Length;

            var currentSeries = series[seriesIndex] as StiLineSeries3D;

            if (currentSeries == null) return;

            //var seriesLength = axisArea.XAxis.Info.Step * currentSeries.Length;
            var seriesWidth = axisArea.XAxis.Info.Step * currentSeries.Width;

            var seriesZPos = MeasureZPosition(axisArea, series.Length - seriesIndex - 1) + axisArea.ZAxis.Info.Step * (1 - currentSeries.Width) / 2;

            for (int pointIndex = 0; pointIndex < axisArea.AxisCore.ValuesCount; pointIndex++)
            {

                if (pointIndex < currentSeries.Values.Length - 1)
                {
                    #region Draw Line
                    var posY = axisArea.AxisCore.GetDividerY();

                    var seriesColor = currentSeries.ProcessSeriesColors(colorIndex, currentSeries.LineColor);
                    var seriesBorderColor = (Color)currentSeries.Core.GetSeriesBorderColor(colorIndex, colorCount);

                    var value = currentSeries.Values[pointIndex];
                    var valueNext = currentSeries.Values[pointIndex + 1];

                    var seriesXPos = MeasureXPosition(axisArea, pointIndex);
                    var seriesHeight = (float)(value.GetValueOrDefault() * axisArea.YAxis.Info.Dpi);

                    var sPoint1 = new StiPoint3D(seriesXPos, posY + seriesHeight, seriesZPos);
                    var sPoint2 = new StiPoint3D(seriesXPos, posY + seriesHeight, seriesZPos - seriesWidth);


                    seriesXPos = MeasureXPosition(axisArea, pointIndex + 1);
                    seriesHeight = (float)(valueNext.GetValueOrDefault() * axisArea.YAxis.Info.Dpi);

                    var ePoint1 = new StiPoint3D(seriesXPos, posY + seriesHeight, seriesZPos - seriesWidth);
                    var ePoint2 = new StiPoint3D(seriesXPos, posY + seriesHeight, seriesZPos);

                    var seriesColumnGeom = new StiLineSeriesGeom3D(sPoint1, sPoint2, ePoint1, ePoint2, seriesColor, seriesBorderColor, render3D);

                    geom.CreateChildGeoms();
                    geom.ChildGeoms.Add(seriesColumnGeom);
                    #endregion

                    //#region Render Series Labels
                    //var labels = currentSeries.Core.GetSeriesLabels();

                    //if (labels != null && labels.Visible)
                    //{
                    //    var seriesLabelsGeom = ((StiAxisSeriesLabelsCoreXF3D)labels.Core).RenderLabel3D(render3D, Series, context, columnRect,
                    //        pointIndex, value, value,
                    //        "",
                    //        currentSeries.Core.GetTag(pointIndex), 0,
                    //        colorIndex, colorCount);
                    //    if (seriesLabelsGeom != null)
                    //    {
                    //        geom.CreateChildGeoms();
                    //        geom.ChildGeoms.Add(seriesLabelsGeom);
                    //    }
                    //}
                    //#endregion
                }
            }

        }

        private double MeasureZPosition(IStiAxisArea3D axisArea, int seriesIndex)
        {
            return axisArea.ZAxis.Info.StripPositions[seriesIndex];
        }

        private double MeasureXPosition(IStiAxisArea3D axisArea, int pointIndex)
        {
            return axisArea.XAxis.Info.StripPositions[pointIndex + 1];
        }

        public override object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            var lineSeries = this.Series as IStiBaseLineSeries3D;

            object color = base.GetSeriesBorderColor(colorIndex, colorCount);

            if (color == null)
            {
                //if (this.Series is StiBubbleSeries bubbleSeries)
                //    return bubbleSeries.BorderColor;

                return lineSeries.LineColor;
            }

            return color;
        }

        public override Color GetSeriesColor(int colorIndex, int colorCount, Color color)
        {
            if (this.Series.Chart == null || this.Series.Chart.Area == null)
                return color;

            if (this.Series.Chart.Area.ColorEach && string.IsNullOrEmpty(((StiSeries)this.Series).AutoSeriesColorDataColumn))
            {
                var styleCore = Series.Chart.Style != null ? Series.Chart.Style.Core : new StiStyleCoreXF29();

                return styleCore.GetColorByIndex(colorIndex, colorCount, SeriesColors);
            }
            return color;
        } 
        #endregion

        public StiLineSeriesCoreXF3D(IStiSeries series)
            : base(series)
        {
        }
    }
}
