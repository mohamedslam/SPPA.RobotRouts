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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiSeriesCoreXF : 
        ICloneable,
        IStiApplyStyleSeries
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiApplyStyleSeries
        public virtual void ApplyStyle(IStiChartStyle style, Color color)
        {
            if (this.Series.AllowApplyStyle)
            {
                this.Series.ShowShadow = style.Core.SeriesShowShadow;

                if (this.Series.SeriesLabels != null)
                    this.Series.SeriesLabels.Core.ApplyStyle(style);
            }

            foreach (StiTrendLine line in this.series.TrendLines)
                line.Core.ApplyStyle(style);
        }
        #endregion

        #region Methods
        protected RectangleF CheckLabelsRect(IStiSeriesLabels labels, StiAreaGeom geom, RectangleF labelsRect)
        {
            return CheckLabelsRect(labels, geom.ClientRectangle, labelsRect) ;
        }

        protected RectangleF CheckLabelsRect(IStiSeriesLabels labels, RectangleF rect, RectangleF labelsRect)
        {
            if (labels != null && labels.PreventIntersection)
            {
                if (labelsRect.X < 0) labelsRect.X = 0;
                if (labelsRect.Y < 0) labelsRect.Y = 0;
                if (labelsRect.Right > rect.Width) labelsRect.X = rect.Width - labelsRect.Width;
                if (labelsRect.Bottom > rect.Height) labelsRect.Y = rect.Height - labelsRect.Height;

                var rectangle = GetDrawRectangle(labelsRect, labels.Angle);
                if (rectangle.Y < 0)
                {
                    labelsRect.Y = 25;
                    return labelsRect;
                }
                if (rectangle.Y + rectangle.Height > rect.Height)
                {
                    labelsRect.Y -= rectangle.Height / 2;
                }
            }
            return labelsRect;
        }

        private RectangleF GetDrawRectangle(RectangleF labelsRect, double angle)
        {
            var point1 = new PointF(labelsRect.Left, labelsRect.Top);
            var point2 = new PointF(labelsRect.Right, labelsRect.Top);
            var point3 = new PointF(labelsRect.Right, labelsRect.Bottom);
            var point4 = new PointF(labelsRect.Left, labelsRect.Bottom);

            var pointCenter = new PointF((labelsRect.Left + labelsRect.Right) / 2, (labelsRect.Top + labelsRect.Bottom) / 2);

            var pointRotate1 = RotatePoint(point1, pointCenter, angle);
            var pointRotate2 = RotatePoint(point2, pointCenter, angle);
            var pointRotate3 = RotatePoint(point3, pointCenter, angle);
            var pointRotate4 = RotatePoint(point4, pointCenter, angle);

            var minY = Math.Min(Math.Min(pointRotate1.Y, pointRotate2.Y), Math.Min(pointRotate3.Y, pointRotate4.Y));
            var maxY = Math.Max(Math.Max(pointRotate1.Y, pointRotate2.Y), Math.Max(pointRotate3.Y, pointRotate4.Y));
            var minX = Math.Min(Math.Min(pointRotate1.X, pointRotate2.X), Math.Min(pointRotate3.X, pointRotate4.X));
            var maxX = Math.Max(Math.Max(pointRotate1.X, pointRotate2.X), Math.Max(pointRotate3.X, pointRotate4.X));

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        private PointF RotatePoint(PointF pointToRotate, PointF centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new PointF
            {
                X =(float)(cosTheta * (pointToRotate.X - centerPoint.X) -sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =(float)(sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        protected virtual void CheckIntersectionLabels(StiAreaGeom geom)
        {
            List<StiCellGeom> childGeoms = geom.ChildGeoms;
            List<StiSeriesLabelsGeom> labelGeoms = new List<StiSeriesLabelsGeom>();
            if (childGeoms != null)
            {
                foreach (StiCellGeom cellGeom in childGeoms)
                {
                    if (cellGeom is StiSeriesLabelsGeom)
                        labelGeoms.Add((StiSeriesLabelsGeom)cellGeom);
                }
            }           

            int count = labelGeoms.Count;

            bool intersection = true;
            int indexCheck = 0;

            while (intersection && indexCheck < 29)
            {
                indexCheck++;

                for (int index1 = 0; index1 < count; index1++)
                {
                    for (int index2 = 0; index2 < count; index2++)
                    {
                        if (index2 == index1) continue;

                        var rect1 = this.GetDrawRectangle(labelGeoms[index1].ClientRectangle, labelGeoms[index1].SeriesLabels.Angle);
                        var rect2 = this.GetDrawRectangle(labelGeoms[index2].ClientRectangle, labelGeoms[index2].SeriesLabels.Angle);

                        var isChanged1 = false;
                        var isChanged2 = false;

                        float overlay = rect1.Height - Math.Abs(rect2.Y - rect1.Y) + 2;
                        if (rect1.IntersectsWith(rect2))
                        {
                            if (rect1.Y > rect2.Y)
                            {
                                rect1.Y += overlay / 2;
                                rect2.Y -= overlay / 2;

                                isChanged1 = true;
                                isChanged2 = true;
                            }
                            else
                            {
                                rect1.Y -= overlay / 2;
                                rect2.Y += overlay / 2;

                                isChanged1 = true;
                                isChanged2 = true;
                            }
                        }

                        if (rect1.Y < 0)
                        {
                            rect1.Y = 2;
                            isChanged1 = true;
                        }

                        if (rect2.Y < 0)
                        {
                            rect2.Y = 2;
                            isChanged2 = true;
                        }

                        if ((rect1.Y + rect1.Height) > geom.ClientRectangle.Height)
                        {
                            rect1.Y = geom.ClientRectangle.Height - rect1.Height - 2;
                            isChanged1 = true;
                        }

                        if ((rect2.Y + overlay / 2 + rect2.Height) > geom.ClientRectangle.Height)
                        {
                            rect2.Y = geom.ClientRectangle.Height - rect2.Height - 2;
                            isChanged2 = true;
                        }

                        if (isChanged1)
                            labelGeoms[index1].ClientRectangle = this.GetDrawRectangle(rect1, -labelGeoms[index1].SeriesLabels.Angle);

                        if (isChanged2)
                            labelGeoms[index2].ClientRectangle = this.GetDrawRectangle(rect2, -labelGeoms[index2].SeriesLabels.Angle);
                    }
                }
            }
        }

        private RectangleF GetLabelRectangle(float angle, RectangleF rect)
        {
            var hypotenuse = Math.Pow(Math.Pow(rect.Width, 2) + Math.Pow(rect.Height, 2), 0.5);
            var angleDelt = Math.Atan(rect.Height / rect.Width) / Math.PI * 180;
            angle += (float)angleDelt;
            return new RectangleF(rect.X, rect.Y, (float)(hypotenuse * Math.Cos(angle * Math.PI / 180)), (float)(hypotenuse * (float)Math.Sin(angle * Math.PI / 180)));
        }

        public virtual void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, IStiSeries[] seriesArray)
        {
        }        

        public virtual StiBrush GetSeriesBrush(int colorIndex, int colorCount)
        {
            if (this.Series.Chart == null || this.Series.Chart.Area == null)
                return null;

            if ((this.Series.Chart.Area.ColorEach || this is StiBoxAndWhiskerSeriesCoreXF || this is StiDoughnutSeriesCoreXF || this is StiSunburstSeriesCoreXF ||
                 this is StiPictorialSeriesCoreXF || this is StiPictorialStackedSeriesCoreXF || this is StiWaterfallSeriesCoreXF) && string.IsNullOrEmpty(((StiSeries) series).AutoSeriesColorDataColumn))
            {
                var styleCore = Series.Chart.Style != null ? Series.Chart.Style.Core : new StiStyleCoreXF29();

                var color = styleCore.GetColorByIndex(colorIndex, colorCount, SeriesColors);
                var seriesBrush = styleCore.GetColumnBrush(color);

                if (this.Series.Chart.Area is IStiClusteredBarArea)
                {
                    if (seriesBrush is StiGradientBrush)
                        ((StiGradientBrush)seriesBrush).Angle += 90;

                    if (seriesBrush is StiGlareBrush)
                        ((StiGlareBrush)seriesBrush).Angle += 90;
                }

                return seriesBrush;
            }
            return null;
        }


        public virtual object GetSeriesBorderColor(int colorIndex, int colorCount)
        {
            if (this.Series.Chart == null || this.Series.Chart.Area == null) return null;

            if (this.Series.Chart.Area.ColorEach && this.Series.AllowApplyStyle || this is StiBoxAndWhiskerSeriesCoreXF || this is StiDoughnutSeriesCoreXF || this is StiSunburstSeriesCoreXF || this is StiPictorialStackedSeriesCoreXF)
            {
                var styleCore = this.Series.Chart.Style != null ? this.Series.Chart.Style.Core : new StiStyleCoreXF29();

                var color = styleCore.GetColorByIndex(colorIndex, colorCount, SeriesColors);

                return styleCore.GetColumnBorder(color);
            }
            return null;
        }

        
        public IStiAxisSeriesLabels GetSeriesLabels()
        {
            if (this.Series.ShowSeriesLabels == StiShowSeriesLabels.FromChart)
                return this.Series.Chart.SeriesLabels as IStiAxisSeriesLabels;

            if (this.Series.ShowSeriesLabels == StiShowSeriesLabels.FromSeries)
                return this.Series.SeriesLabels as IStiAxisSeriesLabels;

            return null;
        }


        public string GetTag(int tagIndex)
        {
            if (series.Tags != null && tagIndex < series.Tags.Length && series.Tags[tagIndex] != null)
                return series.Tags[tagIndex].ToString();
            else
                return string.Empty;
        }
        #endregion

        #region Interaction
        //private static object FalseObject = new object();
        //private static object TrueObject = new object();

        #region MouseOver
        private Dictionary<int, bool> isMouseOverSeriesElementHashtable = null;

        public bool GetIsMouseOverSeriesElement(int seriesIndex)
        {
            if (isMouseOverSeriesElementHashtable == null || !isMouseOverSeriesElementHashtable.ContainsKey(seriesIndex))
                return false;

            return isMouseOverSeriesElementHashtable[seriesIndex];
        }

        public void SetIsMouseOverSeriesElement(int seriesIndex, bool value)
        {
            if (isMouseOverSeriesElementHashtable == null)
                isMouseOverSeriesElementHashtable = new Dictionary<int, bool>();

            isMouseOverSeriesElementHashtable[seriesIndex] = value;
        }

        public bool IsMouseOver { get; set; }
        #endregion
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

        #region Properties
        [Browsable(false)]
        [StiNonSerialized]
        internal Color[] SeriesColors { get; set; }

        private bool isDateTimeValues = false;
        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public bool IsDateTimeValues
        {
            get
            {
                return isDateTimeValues;
            }
            set
            {
                isDateTimeValues = value;
            }
        }

        private bool isDateTimeArguments = false;
        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public bool IsDateTimeArguments
        {
            get
            {
                return isDateTimeArguments;
            }
            set
            {
                isDateTimeArguments = value;
            }
        }

        private IStiSeries series;
        public IStiSeries Series
        {
            get
            {
                return series;
            }
            set
            {
                series = value;
            }
        }
        #endregion

        #region Properties.Core.Interaction
        public IStiSeriesInteraction Interaction
        {
            get
            {
                return Series.Interaction;
            }
            set
            {
                Series.Interaction = value;
            }
        }
        #endregion

        public StiSeriesCoreXF(IStiSeries series)
        {
            this.series = series;
        }
	}
}

