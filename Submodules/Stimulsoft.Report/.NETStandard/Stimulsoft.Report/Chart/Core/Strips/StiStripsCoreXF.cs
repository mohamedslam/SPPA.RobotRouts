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
using Stimulsoft.Report.Dashboard;
using System;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Report.Chart
{
    public class StiStripsCoreXF :
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
            if (this.Strips.AllowApplyStyle)
            {
                this.Strips.TitleColor = style.Core.StyleColors[style.Core.StyleColors.Length - 1];
                this.Strips.StripBrush = new StiSolidBrush(Color.FromArgb(150, style.Core.StyleColors[style.Core.StyleColors.Length - 1]));
            }
        }
        #endregion

        #region Methods.Render
        public void RenderXStrips(StiContext context, StiAxisAreaGeom geom, RectangleF rect)
        {
            var area = Strips.Chart.Area as IStiAxisArea;
            if (area == null) return;
            
            #region Calculate coodrs
            
            float minLeft = CalculateXValue(Strips.MinValue, area, rect);
            float maxLeft = CalculateXValue(Strips.MaxValue, area, rect);

            if (area.ReverseHor)
            {
                minLeft = rect.Width - minLeft;
                maxLeft = rect.Width - maxLeft;
            }

            if (minLeft > maxLeft)
            {
                float tmpTop = minLeft;
                minLeft = maxLeft;
                maxLeft = tmpTop;
            }
                        
            var stripRect = new RectangleF(minLeft, 0, maxLeft - minLeft, rect.Height);
            #endregion

            StiStripsXGeom stripGeom = new StiStripsXGeom(this.Strips, stripRect);

            geom.CreateChildGeoms();
            geom.ChildGeoms.Add(stripGeom);
        }

        private float CalculateXValue(string xValue, IStiAxisArea area, RectangleF rect)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            xValue = StiReportParser.Parse(xValue, area.Chart as StiChart);
            float value = 0f;
            float.TryParse(xValue.Replace(",", "."), out value);

            Thread.CurrentThread.CurrentCulture = currentCulture;

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
            }

            return left;
        }

        public void RenderYStrips(StiContext context, StiAxisAreaGeom geom, RectangleF rect)
        {
            IStiAxisArea area = Strips.Chart.Area as IStiAxisArea;
            if (area == null) return;

            #region Calculate coodrs

            var minTop = CalculateYValue(Strips.MinValue, area, rect);
            var maxTop = CalculateYValue(Strips.MaxValue, area, rect);
            
            if (minTop > maxTop)
            {
                float tmpTop = minTop;
                minTop = maxTop;
                maxTop = tmpTop;
            }

            RectangleF stripRect = new RectangleF(0, minTop, rect.Width, maxTop - minTop);
            #endregion

            StiStripsYGeom stripGeom = new StiStripsYGeom(this.Strips, stripRect);

            geom.CreateChildGeoms();
            geom.ChildGeoms.Add(stripGeom);
        }

        public void Render(StiContext context, StiAxisAreaGeom geom, RectangleF rect)
        {
            if (!Strips.Visible) return;

            if (Strips.Orientation == StiStrips.StiOrientation.Vertical)
                RenderXStrips(context, geom, rect);

            if (Strips.Orientation == StiStrips.StiOrientation.Horizontal ||
                Strips.Orientation == StiStrips.StiOrientation.HorizontalRight)
                RenderYStrips(context, geom, rect);
        }

        private float CalculateYValue(string yValue, IStiAxisArea area, RectangleF rect)
        {

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            yValue = StiReportParser.Parse(yValue, area.Chart as StiChart);
            float value = 0f;
            float.TryParse(yValue.Replace(",", "."), out value);

            Thread.CurrentThread.CurrentCulture = currentCulture;

            if (area.ReverseVert) value = -value;

            float top;
            if ((!area.YAxis.LogarithmicScale && Strips.Orientation == StiStrips.StiOrientation.Horizontal) ||
                (!area.YRightAxis.LogarithmicScale && Strips.Orientation == StiStrips.StiOrientation.HorizontalRight))
            {
                if (Strips.Orientation == StiStrips.StiOrientation.Horizontal)
                    top = area.AxisCore.GetDividerY() - value * (float)area.YAxis.Info.Dpi;
                else
                    top = area.AxisCore.GetDividerRightY() - value * (float)area.YRightAxis.Info.Dpi;                
            }
            else
            {
                var yAxis = Strips.Orientation == StiStrips.StiOrientation.Horizontal
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

            return top;
        }
        #endregion

        #region Properties
        private IStiStrips strips;
        public IStiStrips Strips
        {
            get
            {
                return strips;
            }
            set
            {
                strips = value;
            }
        }
        #endregion

        public StiStripsCoreXF(IStiStrips strips)
        {
            this.strips = strips;
        }
	}
}
