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
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Context;
using System.Threading;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiXRadarAxisCoreXF :
        StiRadarAxisCoreXF
    {
        #region IStiApplyStyle
        public override void ApplyStyle(IStiChartStyle style)
        {
            if (this.Axis.AllowApplyStyle)
            {
                ((IStiXRadarAxis)this.Axis).Labels.Core.ApplyStyle(style);
            }
        }
        #endregion

        #region Methods
        public virtual StiXRadarAxisLabelGeom RenderLabel(StiContext context, IStiSeries series, PointF point, object argument, float angle, int colorIndex, int colorCount)
        {
            string argumentText = GetLabelText(argument);

            var rect = GetLabelRect(context, point, argumentText, 0);
            rect.X = -rect.Width / 2;
            rect.Y = -rect.Height / 2;

            RectangleF labelRect = GetLabelRect(context, point, argumentText, angle);
            Color borderColor = ((IStiXRadarAxis)Axis).Labels.BorderColor;
            StiBrush labelBrush = new StiSolidBrush(((IStiXRadarAxis)Axis).Labels.Color);

            //Red line
            //context.DrawRectangle(new StiPenGeom(Color.Red), labelRect.X, labelRect.Y, labelRect.Width, labelRect.Height);

            return new StiXRadarAxisLabelGeom(this.Axis as IStiXRadarAxis, argumentText, labelBrush, borderColor, angle, rect, labelRect, point);
        }

        internal string GetLabelText(object value)
        {
            try
            {
                var xAxis = this.Axis as IStiXRadarAxis;
                if (xAxis.Labels.Format != null && xAxis.Labels.Format.Trim().Length != 0)
                {
                    try
                    {
                        #region If value is string try to convert it to decimal value
                        if (value is string)
                        {
                            decimal result;
                            if (decimal.TryParse(value.ToString().Replace(".", ",").Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator), out result))
                            {
                                value = result;
                            }
                        }
                        #endregion

                        if (!StiChartOptions.OldChartPercentMode && xAxis.Labels.Format.StartsWith("P", StringComparison.InvariantCulture))
                        {
                            int signs = 0;
                            if (xAxis.Labels.Format.Length > 1)
                            {
                                int.TryParse(xAxis.Labels.Format.Remove(0, 1), out signs);
                            }

                            return string.Format("{0}{1:N" + signs.ToString() + "}{2}{3}", xAxis.Labels.TextBefore, value, "%", xAxis.Labels.TextAfter);
                        }
                        else return string.Format("{0}{1:" + xAxis.Labels.Format + "}{2}", xAxis.Labels.TextBefore, value, xAxis.Labels.TextAfter);
                    }
                    catch
                    {
                    }
                }
                return string.Format("{0}{1}{2}", xAxis.Labels.TextBefore, value, xAxis.Labels.TextAfter);
            }
            catch
            {
            }
            return value.ToString();
        }

        public virtual RectangleF GetLabelRect(StiContext context, PointF point, string text, float angle)
        {
            StiFontGeom font = StiFontGeom.ChangeFontSize(
                ((IStiXRadarAxis)this.Axis).Labels.Font, 
                ((IStiXRadarAxis)this.Axis).Labels.Font.Size * context.Options.Zoom);
            StiStringFormatGeom sf = context.GetGenericStringFormat();

            sf.Trimming = StringTrimming.None;
            if (!((IStiXRadarAxis)this.Axis).Labels.WordWrap)
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;

            return context.MeasureRotatedString(text, font, point, sf, StiRotationMode.CenterBottom, angle, (int)(((IStiXRadarAxis)this.Axis).Labels.Width * context.Options.Zoom));
        }
        #endregion        

        #region Properties
        public IStiXRadarAxis XAxis
        {
            get
            {
                return Axis as IStiXRadarAxis;
            }
        }
        #endregion

        public StiXRadarAxisCoreXF(IStiRadarAxis axis) : base(axis)
        {
        }
    }
}
