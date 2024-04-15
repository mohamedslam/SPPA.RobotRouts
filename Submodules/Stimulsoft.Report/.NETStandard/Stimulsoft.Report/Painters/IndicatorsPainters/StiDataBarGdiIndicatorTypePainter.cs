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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using LinearGradientBrush = Stimulsoft.Drawing.Drawing2D.LinearGradientBrush;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiDataBarGdiIndicatorTypePainter : StiGdiIndicatorTypePainter
    {
        #region Methods.Painter
        public override RectangleD Paint(object context, StiComponent component, RectangleD rect)
        {
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = (Graphics)context;
            var textComp = (StiText)component;

            float zoom = (float)component.Report.Info.Zoom;
            var rectComp = rect;

            if (zoom > 1)
                rectComp.Inflate(-2 * zoom, -2 * zoom);
            else
                rectComp.Inflate(-2, -2);

            var barIndicator = textComp.Indicator as StiDataBarIndicator;
            if (barIndicator == null || barIndicator.Value == 0f)            
                return rect;            

            float totalWidth = barIndicator.Maximum + Math.Abs(barIndicator.Minimum);

            var barRect = rectComp.ToRectangleF();

            float minimumPart = barRect.Width * Math.Abs(barIndicator.Minimum) / totalWidth;
            float maximumPart = barRect.Width * barIndicator.Maximum / totalWidth;
            float valuePart = barRect.Width * Math.Abs(barIndicator.Value) / totalWidth;

            #region Process Direction
            var direction = barIndicator.Direction;
            if (direction == StiDataBarDirection.Default)
            {
                if (textComp.TextOptions != null && textComp.TextOptions.RightToLeft)
                    direction = StiDataBarDirection.RighToLeft;
                else
                    direction = StiDataBarDirection.LeftToRight;
            }
            #endregion

            float angle = 0f;

            #region StiDataBarDirection.LeftToRight
            if (direction == StiDataBarDirection.LeftToRight)
            {
                if (barIndicator.Value < 0)
                    barRect.X += minimumPart - valuePart;
                else
                    barRect.X += minimumPart;

                barRect.Width = valuePart;
            }
            #endregion

            #region StiDataBarDirection.RightToLeft
            else
            {
                angle = 180f;

                if (barIndicator.Value < 0)
                    barRect.X = barRect.Right - minimumPart;
                else
                    barRect.X = barRect.Right - minimumPart - valuePart;

                barRect.Width = valuePart;
            }
            #endregion

            if (barIndicator.Value < 0)
            {
                angle += 180f;
            }

            if (barRect.Width > 0 && barRect.Height > 0)
            {
                #region StiBrushType.Gradient
                if (barIndicator.BrushType == Stimulsoft.Report.Components.StiBrushType.Gradient)
                {
                    RectangleD fillRect;
                    if (direction == StiDataBarDirection.LeftToRight)
                    {
                        if (barIndicator.Value > 0)
                            fillRect = new RectangleD(rect.Left + minimumPart, rect.Top, maximumPart, rect.Height);
                        else
                            fillRect = new RectangleD(rect.Left, rect.Top, minimumPart, rect.Height);
                    }
                    else
                    {
                        if (barIndicator.Value < 0)
                            fillRect = new RectangleD(rect.Left + maximumPart, rect.Top, minimumPart, rect.Height);
                        else
                            fillRect = new RectangleD(rect.Left, rect.Top, maximumPart, rect.Height);
                    }

                    if (barRect.Width > 0 && barRect.Width < 1)
                        barRect.Width = 1;

                    if (fillRect.Width > 0 && fillRect.Width < 1)
                        fillRect.Width = 1;

                    if (fillRect.Width > 0 && barRect.Width > 0)
                    {
                        Color startColor = barIndicator.Value < 0 ? barIndicator.NegativeColor : barIndicator.PositiveColor;
                        Color endColor = StiColorUtils.Light(startColor, 200);
                        fillRect.X -= fillRect.Width * .1;
                        fillRect.Width += fillRect.Width * .2;
                        using (var brush = new LinearGradientBrush(fillRect.ToRectangleF(), startColor, endColor, angle))
                        {
                            g.FillRectangle(brush, barRect);
                        }
                    }
                }
                #endregion

                #region StiBrushType.Solid
                else
                {
                    using (var brush = new SolidBrush(barIndicator.Value < 0 ? barIndicator.NegativeColor : barIndicator.PositiveColor))
                    {
                        g.FillRectangle(brush, barRect);
                    }
                }
                #endregion

                #region ShowBorder
                if (barIndicator.ShowBorder)
                {
                    using (var pen = new Pen(barIndicator.Value < 0 ? barIndicator.NegativeBorderColor : barIndicator.PositiveBorderColor))
                    {
                        if (zoom > 1)
                            pen.Width *= zoom;

                        g.DrawRectangle(pen, barRect.X, barRect.Y, barRect.Width, barRect.Height);
                    }
                }
                #endregion
            }

            return rect;
        }
        #endregion
    }
}
