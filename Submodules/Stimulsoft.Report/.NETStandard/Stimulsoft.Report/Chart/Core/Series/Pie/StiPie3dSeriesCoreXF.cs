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
using Stimulsoft.Report.Chart.Geoms.Series.Pie;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Stimulsoft.Report.Chart
{
    public class StiPie3dSeriesCoreXF : StiPieSeriesCoreXF
    {

        #region Fields
        /// <summary>
        ///   Array of ordered pie slices constituting the chart, starting from 
        ///   270 degrees axis.
        /// </summary>
        private StiPie3dSlice[] mPieSlices;
        private float pieHeight; 
        #endregion

        #region Methods
        public override void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
            InitializePieSlices(geom.Area, rect, seriesArray, context.Options.Zoom);

            if (mPieSlices == null || mPieSlices.Length == 0)
                InitializeEmptyPieSlices(geom.Area, rect, seriesArray, context.Options.Zoom);

            DrawBottoms(geom);

            if (pieHeight > 0)
                DrawSliceSides(geom);

            DrawTops(geom);

            DrawLabels(geom, context);
        }

        private void DrawLabels(StiAreaGeom areaGeom, StiContext context)
        {
            if (mPieSlices == null) return;

            for (var index = 0; index < mPieSlices.Length; index++)
            {
                var pieSlice = mPieSlices[index];
                pieSlice.DrawLabels(areaGeom, context);
            }
        }

        private void DrawTops(StiAreaGeom areaGeom)
        {
            if (mPieSlices == null) return;
            for (var index = 0; index < mPieSlices.Length; index++)
            {
                var pieSlice = mPieSlices[index];
                pieSlice.DrawTopPieSliceGeom(areaGeom);
            }
        }

        private void DrawSliceSides(StiAreaGeom areaGeom)
        {
            if (mPieSlices == null) return;

            var pieSlicesList = mPieSlices.ToList();

            if (pieSlicesList.Count == 0) return;

            StiPie3dSlice pieSlice = null;
            // if the first pie slice (crossing 270 i.e. back) is crossing 90 
            // (front) axis too, we have to split it
            
             if ((mPieSlices[0].StartAngle > 90) && (mPieSlices[0].StartAngle <= 270) && (mPieSlices[0].StartAngle + mPieSlices[0].SweepAngle > 450))
            {
                pieSlice = pieSlicesList[0];
                pieSlice.InitTextPosition(areaGeom);

                // this one is split at 0 deg to avoid line of split to be
                // visible on the periphery
                var splitSlices = pieSlice.Split(0F);
                pieSlicesList[0] = splitSlices[0];

                if (Math.Abs(splitSlices[1].SweepAngle) > 0F)
                {
                    pieSlicesList.Insert(1, splitSlices[1]);
                }
            }
            else if (((mPieSlices[0].StartAngle > 270) && (mPieSlices[0].StartAngle + mPieSlices[0].SweepAngle > 450)) || ((mPieSlices[0].StartAngle < 90) && (mPieSlices[0].StartAngle + mPieSlices[0].SweepAngle > 270)))
            {
                pieSlice = pieSlicesList[0];
                pieSlice.InitTextPosition(areaGeom);
                // this one is split at 180 deg to avoid line of split to be
                // visible on the periphery
                var splitSlices = pieSlice.Split(180F);

                pieSlicesList[0] = splitSlices[1];
                if (Math.Abs(splitSlices[1].SweepAngle) > 0F)
                {
                    pieSlicesList.Add(splitSlices[0]);
                }
            }

            // first draw the backmost pie slice
            pieSlice = pieSlicesList[0];
            pieSlice.DrawSides(areaGeom);
            // draw pie slices from the backmost to forward
            int incrementIndex = 1;
            int decrementIndex = pieSlicesList.Count - 1;

            while (incrementIndex < decrementIndex)
            {
                var sliceLeft = pieSlicesList[decrementIndex];
                float angle1 = sliceLeft.StartAngle - 90;
                
                if (angle1 > 180 || angle1 < 0)
                    angle1 = 0;

                var sliceRight = pieSlicesList[incrementIndex];
                float angle2 = (450 - sliceRight.EndAngle) % 360;

                if (angle2 > 180 || angle2 < 0)
                    angle2 = 0;
                                
                if (angle2 >= angle1)
                {
                    sliceRight.DrawSides(areaGeom);
                    ++incrementIndex;
                }

                else if (angle2 < angle1)
                {
                    sliceLeft.DrawSides(areaGeom);
                    --decrementIndex;
                }
            }

            pieSlice = pieSlicesList[decrementIndex];
            pieSlice.DrawSides(areaGeom);
        }

        private void DrawBottoms(StiAreaGeom areaGeom)
        {
            if (mPieSlices == null) return;

            for (var index = 0; index < mPieSlices.Length; index++)
            {
                var pieSlice = mPieSlices[index];
                pieSlice.DrawBottomPieSliceGeom(areaGeom);
            }
        }

        protected virtual void InitializePieSlices(IStiArea area, RectangleF mainRect, IStiSeries[] seriesArray, float zoom)
        {
            var boundingRect = MeasureBoundingRect(mainRect);

            var listPieSlices = new List<StiPie3dSlice>();

            float gradPerValue = GetGradPerValue(seriesArray);

            var currentSeries = seriesArray[0] as IStiPie3dSeries;
            var startAngle = currentSeries.StartAngle % 360;

            var colorCount = GetColorCount(seriesArray);

            pieHeight = currentSeries.Options3D.Height * zoom;

            var largestDisplacement = GetLargestDisplacement(zoom);
            var topEllipeSize = GetTopEllipseSize(boundingRect, zoom);

            int colorIndex = 0;
            int backPieIndex = -1;

            foreach (IStiPie3dSeries ser in seriesArray)
            {
                for (var index = 0; index < ser.Values.Length; index++)
                {
                    var value = ser.Values[index].GetValueOrDefault();

                    if (value != 0)
                    {
                        float sweepAngle = (float)(gradPerValue * Math.Abs(value));

                        var seriesBrush = ser.Brush;
                        if (ser.AllowApplyBrush)
                        {
                            seriesBrush = ser.Core.GetSeriesBrush(colorIndex, colorCount);
                            seriesBrush = ser.ProcessSeriesBrushes(index, seriesBrush);
                        }

                        if (ser.Options3D.Opacity >= 0 && ser.Options3D.Opacity < 1)
                        {
                            var color = StiBrush.ToColor(seriesBrush);
                            color = Color.FromArgb((int)(255 * ser.Options3D.Opacity), color.R, color.G, color.B);

                            seriesBrush = new StiSolidBrush(color);
                        }

                        var borderColor = ser.BorderColor;
                        if (ser.AllowApplyBorderColor)
                            borderColor = (Color)ser.Core.GetSeriesBorderColor(colorIndex, colorCount);

                        float xDisplacement = GetLargestDisplacement(zoom);
                        float yDisplacement = GetLargestDisplacement(zoom);

                        if (xDisplacement > 0F)
                        {
                            var pieDisplacement =
                                GetSliceDisplacement((float)(startAngle + sweepAngle / 2), xDisplacement, yDisplacement);
                            xDisplacement = pieDisplacement.Width;
                            yDisplacement = pieDisplacement.Height;
                        }

                        var rectPieSlilce = new RectangleF(
                            boundingRect.X + 2 * largestDisplacement + xDisplacement,
                            boundingRect.Y + 2 * largestDisplacement + yDisplacement,
                            topEllipeSize.Width,
                            topEllipeSize.Height);

                        var slice =
                            new StiPie3dSlice(area, value, GetArgumentText(ser, index), ser.Core.GetTag(index),
                            index, ser, rectPieSlilce, pieHeight, startAngle % 360, sweepAngle, seriesBrush, borderColor,
                            colorIndex, colorCount);

                        if (backPieIndex > -1 || ((startAngle <= 270) && (startAngle + sweepAngle > 270)) || ((startAngle >= 270) && (startAngle + sweepAngle > 630)))
                        {
                            ++backPieIndex;
                            listPieSlices.Insert(backPieIndex, slice);
                            //m_pieSlicesMapping.Insert(backPieIndex, i);
                        }
                        else
                        {
                            listPieSlices.Add(slice);
                            //m_pieSlicesMapping.Add(i);
                        }

                        startAngle += sweepAngle;
                    }
                    colorIndex++;


                    if (startAngle > 360)
                        startAngle -= 360;
                }
            }

            mPieSlices = listPieSlices.ToArray();
        }

        private RectangleF MeasureBoundingRect(RectangleF mainRect)
        {
            var factor = 1.61f;
            var boundingRect = RectangleF.Empty;

            if (mainRect.Width / mainRect.Height > factor)
            {
                var width = mainRect.Height * factor;
                boundingRect = new RectangleF((mainRect.Width - width) / 2, 0, width, mainRect.Height);
            }

            else
            {
                var height = mainRect.Width / factor;
                boundingRect = new RectangleF(0, (mainRect.Height - height) / 2, mainRect.Width, height);
            }

            boundingRect = new RectangleF(
                boundingRect.X + boundingRect.Width * 0.025f,
                boundingRect.Y + boundingRect.Height * 0.025f,
                boundingRect.Width * 0.95f,
                boundingRect.Height * 0.95f);

            return boundingRect;
        }

        protected virtual void InitializeEmptyPieSlices(IStiArea area, RectangleF mainRect, IStiSeries[] seriesArray, float zoom)
        {
            var boundingRect = MeasureBoundingRect(mainRect);

            var listPieSlices = new List<StiPie3dSlice>();

            var emptyChart = new StiChart();
            var emptySeries = new StiPie3dSeries()
            {
                Chart = emptyChart
            };
            var emptyValues = new double[2] { 1, 3 };
            float gradPerValue = 360 / 4;
            var startAngle = 0f;
            pieHeight = 20 * zoom;

            var largestDisplacement = GetLargestDisplacement(zoom);
            var topEllipeSize = GetTopEllipseSize(boundingRect, zoom);

            int colorIndex = 0;
            int backPieIndex = -1;

            for (var index = 0; index < emptyValues.Length; index++)
            {
                var value = emptyValues[index];

                if (value != 0)
                {
                    float sweepAngle = (float)(gradPerValue * Math.Abs(value));

                    var seriesBrush = new StiSolidBrush(Color.FromArgb(50, Color.LightGray));
                    var borderColor = Color.LightGray;                    

                    float xDisplacement = GetLargestDisplacement(zoom);
                    float yDisplacement = GetLargestDisplacement(zoom);

                    if (xDisplacement > 0F)
                    {
                        var pieDisplacement =
                            GetSliceDisplacement((float)(startAngle + sweepAngle / 2), xDisplacement, yDisplacement);
                        xDisplacement = pieDisplacement.Width;
                        yDisplacement = pieDisplacement.Height;
                    }

                    var rectPieSlilce = new RectangleF(
                        boundingRect.X + 2 * largestDisplacement + xDisplacement,
                        boundingRect.Y + 2 * largestDisplacement + yDisplacement,
                        topEllipeSize.Width,
                        topEllipeSize.Height);

                    var slice =
                        new StiPie3dSlice(area, value, "", null,
                        index, emptySeries, rectPieSlilce, pieHeight, startAngle % 360, sweepAngle, seriesBrush, borderColor,
                        0, 0);

                    if (backPieIndex > -1 || ((startAngle <= 270) && (startAngle + sweepAngle > 270)) || ((startAngle >= 270) && (startAngle + sweepAngle > 630)))
                    {
                        ++backPieIndex;
                        listPieSlices.Insert(backPieIndex, slice);
                    }
                    else
                    {
                        listPieSlices.Add(slice);
                    }

                    startAngle += sweepAngle;
                }
                colorIndex++;


                if (startAngle > 360)
                    startAngle -= 360;
            }

            mPieSlices = listPieSlices.ToArray();
        }

        private SizeF GetTopEllipseSize(RectangleF boundingRect, float zoom)
{
            var distance = GetLargestDisplacement(zoom);
            float widthTopEllipse = boundingRect.Width - 4 *distance;
            float heightTopEllipse = boundingRect.Height - 4 * distance - this.pieHeight;

            if (widthTopEllipse < 1)
                widthTopEllipse = 1;

            if (heightTopEllipse < 1)
                heightTopEllipse = 1;

            return new SizeF(widthTopEllipse, heightTopEllipse);
        }

        private SizeF GetLargestDisplacementEllipseSize(float zoom)
        {
            var distance = GetLargestDisplacement(zoom);
            return new SizeF(distance, distance);
        }

        protected SizeF GetSliceDisplacement(float angle, float xDisplacement, float yDisplacement)
        {
            if (xDisplacement == 0F)
                return SizeF.Empty;

            float xDisplacementCal = (float)(xDisplacement * Math.Cos(angle * Math.PI / 180));
            float yDisplacementCal = (float)(yDisplacement * Math.Sin(angle * Math.PI / 180));
            return new SizeF(xDisplacementCal, yDisplacementCal);
        }

        private float GetLargestDisplacement(float zoom)
        {
            return ((IStiPie3dSeries)this.Series).Options3D.Distance * zoom;
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
                return $"3D {StiLocalization.Get("Chart", "Pie")}";
            }
        }
        #endregion

        public StiPie3dSeriesCoreXF(IStiSeries series)
               : base(series)
        {
        }
    }
}
