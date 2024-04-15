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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System.Globalization;
using System.Threading;
using Stimulsoft.Report.Dashboard;

namespace Stimulsoft.Report.Chart
{
    public class StiConstantLinesCoreXF :
        IStiApplyStyle,
        ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiApplyStyle
        /// <summary>
        /// Applying specified style to this area.
        /// </summary>
        /// <param name="style"></param>
        public void ApplyStyle(IStiChartStyle style)
        {
            if (this.ConstantLines.AllowApplyStyle)
            {
                this.ConstantLines.LineColor = style.Core.SeriesLabelsColor;
            }
        }
        #endregion

        #region Methods
        public void RenderXConstantLines(StiContext context, StiAxisAreaGeom geom, RectangleF rect)
        {
            var area = geom.Area as IStiAxisArea;
            if (area == null) return;

            #region Calculate coodrs
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            var strValue = StiReportParser.Parse(ConstantLines.AxisValue, area.Chart as StiChart);
            
            float value = 0f;

            if (geom.Area.Chart.Series.Count > 0 &&
                geom.Area.Chart.Series[0].Core.IsDateTimeArguments && 
                DateTime.TryParse(strValue, out DateTime resultDateTime))
            {
                value = (float)resultDateTime.ToOADate();
            }

            else
            {
                float.TryParse(strValue.Replace(",", "."), out value);
            }

            Thread.CurrentThread.CurrentCulture = currentCulture;

            if (area.ReverseVert) 
                value = -value;

            float left = 0;
            if (area.XAxis.LogarithmicScale && area.XAxis.Info.StripLines.Count > 0)
            {
                int countStrip = area.XAxis.Info.StripLines.Count;

                int startPoint = area.ReverseHor ? countStrip - 1 : 0;
                int endPoint = area.ReverseHor ? 0 : countStrip - 1;

                double startValue = area.XAxis.Info.StripLines[startPoint].Value;
                double endValue = area.XAxis.Info.StripLines[endPoint].Value;

                double decadeX = Math.Abs(rect.Width / (Math.Log10(endValue) - Math.Log10(startValue)));

                left = (float)Math.Abs(Math.Log10(value) * decadeX - Math.Log10(area.XAxis.Info.StripLines[startPoint].Value) * decadeX);
            }
            else
            {
                left = area.AxisCore.GetDividerX() + value * (float)area.XAxis.Info.Dpi;

                foreach (StiStripLineXF tick in area.XAxis.Info.StripLines)
                {
                    if (tick.ValueObject != null && tick.ValueObject.ToString() == ConstantLines.AxisValue)
                    {
                        value = (float)tick.Value;
                        if (area.ReverseHor) value = -value;
                        left = (float)(value * area.XAxis.Info.Dpi) + area.AxisCore.GetDividerX();
                    }
                }
            }
            #endregion

            #region Render Title
            var point = new PointF(0, 0);
            var mode = StiRotationMode.LeftTop;

            switch (ConstantLines.Position)
            {
                case StiConstantLines.StiTextPosition.LeftTop:
                    mode = StiRotationMode.LeftBottom;
                    point = new PointF(left + this.ConstantLines.LineWidth / 2, 0);
                    break;

                case StiConstantLines.StiTextPosition.LeftBottom:
                    mode = StiRotationMode.LeftTop;
                    point = new PointF(left - this.ConstantLines.LineWidth / 2, 0);
                    break;

                case StiConstantLines.StiTextPosition.CenterTop:
                    mode = StiRotationMode.CenterBottom;
                    point = new PointF(left + this.ConstantLines.LineWidth / 2, rect.Height / 2);
                    break;

                case StiConstantLines.StiTextPosition.CenterBottom:
                    mode = StiRotationMode.CenterTop;
                    point = new PointF(left - this.ConstantLines.LineWidth / 2, rect.Height / 2);
                    break;

                case StiConstantLines.StiTextPosition.RightTop:
                    mode = StiRotationMode.RightBottom;
                    point = new PointF(left + this.ConstantLines.LineWidth / 2, rect.Height);
                    break;

                case StiConstantLines.StiTextPosition.RightBottom:
                    mode = StiRotationMode.RightTop;
                    point = new PointF(left - this.ConstantLines.LineWidth / 2, rect.Height);
                    break;
            }
            #endregion
                        
            var lineGeom = new StiConstantLinesVerticalGeom(this.ConstantLines,
                new RectangleF(left, 0, left, rect.Height), point, mode);

            geom.CreateChildGeoms();
            geom.ChildGeoms.Add(lineGeom);
        }

        public void RenderYConstantLines(StiContext context, StiAxisAreaGeom geom, RectangleF rect)
        {
            var area = geom.Area as IStiAxisArea;
            if (area == null) return;

            #region Calculate coodrs
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            var strValue = StiReportParser.Parse(ConstantLines.AxisValue, area.Chart as StiChart);

            float value = 0f;
            float.TryParse(strValue.Replace(",", "."), out value);

            Thread.CurrentThread.CurrentCulture = currentCulture;

            if (area.ReverseVert) 
                value = -value;

            float top = 0;
            if ((!area.YAxis.LogarithmicScale && ConstantLines.Orientation == StiConstantLines.StiOrientation.Horizontal) ||
                (!area.YRightAxis.LogarithmicScale && ConstantLines.Orientation == StiConstantLines.StiOrientation.HorizontalRight))
            {
                if (ConstantLines.Orientation == StiConstantLines.StiOrientation.Horizontal)
                    top = area.AxisCore.GetDividerY() - value * (float)area.YAxis.Info.Dpi;
                else
                    top = area.AxisCore.GetDividerRightY() - value * (float)area.YRightAxis.Info.Dpi;                
            }
            else
            {
                var yAxis = ConstantLines.Orientation == StiConstantLines.StiOrientation.Horizontal
                    ? area.YAxis
                    : area.YRightAxis;

                int countStrip = yAxis.Info.StripLines.Count;

                int startPoint = 0;
                int endPoint = countStrip - 1;

                double startValue = yAxis.Info.StripLines[startPoint].Value;
                double endValue = yAxis.Info.StripLines[endPoint].Value;

                double decadeY = Math.Abs(rect.Height / (Math.Log10(endValue) - Math.Log10(startValue)));

                top = (float)Math.Abs(Math.Log10(yAxis.Info.StripLines[startPoint].Value) * decadeY - Math.Log10(value) * decadeY);
            }
            #endregion

            #region Draw Title
            var point = new PointF(0, 0);
            var mode = StiRotationMode.LeftTop;

            switch (ConstantLines.Position)
            {
                case StiConstantLines.StiTextPosition.LeftTop:
                    mode = StiRotationMode.LeftBottom;
                    point = new PointF(-rect.X, top - this.ConstantLines.LineWidth / 2);
                    break;

                case StiConstantLines.StiTextPosition.LeftBottom:
                    mode = StiRotationMode.LeftTop;
                    point = new PointF(-rect.X, top + this.ConstantLines.LineWidth / 2);
                    break;

                case StiConstantLines.StiTextPosition.CenterTop:
                    mode = StiRotationMode.CenterBottom;
                    point = new PointF(rect.Width / 2, top - this.ConstantLines.LineWidth / 2);
                    break;

                case StiConstantLines.StiTextPosition.CenterBottom:
                    mode = StiRotationMode.CenterTop;
                    point = new PointF(rect.Width / 2, top + this.ConstantLines.LineWidth / 2);
                    break;

                case StiConstantLines.StiTextPosition.RightTop:
                    mode = StiRotationMode.RightBottom;
                    point = new PointF(rect.Width, top - this.ConstantLines.LineWidth / 2);
                    break;

                case StiConstantLines.StiTextPosition.RightBottom:
                    mode = StiRotationMode.RightTop;
                    point = new PointF(rect.Width, top + this.ConstantLines.LineWidth / 2);
                    break;
            }
            #endregion

            var lineGeom = new StiConstantLinesYGeom(this.ConstantLines,
                new RectangleF(0, top, rect.Width, top), point, mode);

            geom.CreateChildGeoms();
            geom.ChildGeoms.Add(lineGeom);
        }

        public void Render(StiContext context, StiAxisAreaGeom geom, RectangleF rect)
        {
            if (!ConstantLines.Visible) return;

            if (ConstantLines.Orientation == StiConstantLines.StiOrientation.Vertical)
                RenderXConstantLines(context, geom, rect);

            if (ConstantLines.Orientation == StiConstantLines.StiOrientation.Horizontal ||
                ConstantLines.Orientation == StiConstantLines.StiOrientation.HorizontalRight)
                RenderYConstantLines(context, geom, rect);
        }
        #endregion

        #region Properties
        public IStiConstantLines ConstantLines { get; set; }
        #endregion

        public StiConstantLinesCoreXF(IStiConstantLines constantLines)
        {
            this.ConstantLines = constantLines;
        }
    }
}
