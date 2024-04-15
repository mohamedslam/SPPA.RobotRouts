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

using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.GaugeGeoms;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Gauge.Primitives;
using Stimulsoft.Report.Painters;
using System;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    internal sealed class StiLinearBarGeometry : IStiScaleBarGeometry
    {
        #region Fields
        private readonly StiLinearScale scale;
        #endregion

        #region Properties
        private SizeF size;
        public SizeF Size => size;

        private RectangleF rectGeometry;
        public RectangleF RectGeometry => rectGeometry;

        public float Radius => 0f;

        public float Diameter => 0f;

        private PointF center;
        public PointF Center => this.center;
        #endregion

        #region Methods
        public void CheckRectGeometry(RectangleF rect)
        {
            this.size = rect.Size;

            CheckMinMaxWidth(out var startWidth, out var endWidth);

            this.rectGeometry = GetRectGeometry(rect, (startWidth > endWidth) ? startWidth : endWidth);
            this.center = new PointF(this.rectGeometry.Left + this.rectGeometry.Width / 2, this.rectGeometry.Top + this.rectGeometry.Height / 2);
        }

        private RectangleF GetRectGeometry(RectangleF rect, float value)
        {
            float width;
            float height;

            if (this.scale.Orientation == Orientation.Horizontal)
            {
                width = this.size.Width * this.scale.RelativeHeight;
                height = this.size.Height * value;
            }
            else
            {
                height = this.size.Height * this.scale.RelativeHeight;
                width = this.size.Width * value;
            }

            return new RectangleF(rect.X + (this.size.Width - width) / 2, rect.Y + (this.size.Height - height) / 2, width, height);
        }

        public float GetRestToLenght()
        {
            float value = Math.Abs(scale.StartWidth - scale.EndWidth);

            if (scale.Orientation == Orientation.Horizontal)
            {
                return (this.size.Height * value) / 2;
            }
            else
            {
                return (this.size.Width * value) / 2;
            }
        }

        private void CheckMinMaxWidth(out float startWidth, out float endWidth)
        {
            if (this.scale.IsReversed)
            {
                startWidth = this.scale.EndWidth;
                endWidth = this.scale.StartWidth;
            }
            else
            {
                startWidth = this.scale.StartWidth;
                endWidth = this.scale.EndWidth;
            }
        }

        public void DrawScaleGeometry(StiGaugeContextPainter context)
        {
            CheckMinMaxWidth(out var startValue, out var endValue);
            var points = new PointF[4];

            if (this.scale.Orientation == Orientation.Horizontal)
            {
                float offsetY = this.size.Height;
                offsetY *= (startValue < endValue) ? startValue : endValue;
                offsetY = (rectGeometry.Height - offsetY) / 2;

                if (startValue > endValue)
                {
                    points[0] = new PointF(rectGeometry.Left, rectGeometry.Top);
                    points[1] = new PointF(rectGeometry.Right, rectGeometry.Top + offsetY);
                    points[2] = new PointF(rectGeometry.Right, rectGeometry.Bottom - offsetY);
                    points[3] = new PointF(rectGeometry.Left, rectGeometry.Bottom);
                }
                else
                {
                    points[0] = new PointF(rectGeometry.Left, rectGeometry.Top + offsetY);
                    points[1] = new PointF(rectGeometry.Left, rectGeometry.Bottom - offsetY);
                    points[2] = new PointF(rectGeometry.Right, rectGeometry.Bottom);
                    points[3] = new PointF(rectGeometry.Right, rectGeometry.Top);
                }
            }
            else
            {
                float offsetX = this.size.Width;
                offsetX *= (startValue < endValue) ? startValue : endValue;
                offsetX = (rectGeometry.Width - offsetX) / 2;

                if (startValue > endValue)
                {
                    points[0] = new PointF(rectGeometry.Left + offsetX, rectGeometry.Top);
                    points[1] = new PointF(rectGeometry.Left, rectGeometry.Bottom);
                    points[2] = new PointF(rectGeometry.Right, rectGeometry.Bottom);
                    points[3] = new PointF(rectGeometry.Right - offsetX, rectGeometry.Top);
                }
                else
                {
                    points[0] = new PointF(rectGeometry.Left, rectGeometry.Top);
                    points[1] = new PointF(rectGeometry.Left + offsetX, rectGeometry.Bottom);
                    points[2] = new PointF(rectGeometry.Right - offsetX, rectGeometry.Bottom);
                    points[3] = new PointF(rectGeometry.Right, rectGeometry.Top);
                }
            }

            var pathGeom = new StiGraphicsPathGaugeGeom(rectGeometry, points[0], this.scale.Brush, this.scale.BorderBrush, 1f);
            pathGeom.AddGraphicsPathLinesGaugeGeom(points);
            pathGeom.AddGraphicsPathCloseFigureGaugeGeom();

            context.AddGraphicsPathGaugeGeom(pathGeom);
        }
        #endregion

        #region Methods.GetGeometry
        public StiGraphicsPathLinesGaugeGeom DrawGeometry(StiGaugeContextPainter context, float StartValue, float EndValue, float StartWidth, float EndWidth,
            float Offset, StiPlacement Placement, ref RectangleF rect, bool returnOnlyRect)
        {
            // Получаем область вывода Range
            var rectGeometry = this.RectGeometry;
            rect = rectGeometry;
            // Если длинна или высота = 0, то прерываем
            if (rect.Width == 0 || rect.Height == 0) return null;

            float startValue = StartValue;
            float endValue = EndValue;
            // Если значения равны, то отрисовывать нечего - прерываем
            if (startValue == endValue) return null;

            float minimum = this.scale.ScaleHelper.ActualMinimum;
            float maximum = this.scale.ScaleHelper.ActualMaximum;
            float length = this.scale.ScaleHelper.TotalLength;

            if ((startValue <= minimum && endValue <= minimum) || (startValue >= maximum && endValue >= maximum))
                return null;

            #region Проверяем чтобы значения startValue/endValue небыли больше или меньше минимум/максимума
            float length1, length2;
            if (startValue < endValue)
            {
                if (startValue < minimum) startValue = minimum;
                if (endValue > maximum) endValue = maximum;

                length1 = StiMathHelper.Length(minimum, startValue);
                length2 = StiMathHelper.Length(startValue, endValue);
            }
            else
            {
                if (startValue < minimum) startValue = minimum;
                if (endValue > maximum) endValue = maximum;

                length1 = StiMathHelper.Length(minimum, endValue);
                length2 = StiMathHelper.Length(endValue, startValue);
            }
            #endregion

            #region Обрезаем область в зависимости от заданных StartAscent, EndAscent
            if (scale.Orientation == Orientation.Horizontal)
            {
                float step = rect.Width / length;
                float offset1 = length1 * step;
                float offset2 = length2 * step;

                if (scale.IsReversed)
                {
                    rect.X = rect.Right - (offset1 + offset2);
                    rect.Width = offset2;
                }
                else
                {
                    rect.X += offset1;
                    rect.Width = offset2;
                }
            }
            else
            {
                float step = rect.Height / length;
                float offset1 = length1 * step;
                float offset2 = length2 * step;

                if (scale.IsReversed)
                {
                    rect.Y += offset1;
                    rect.Height = offset2;
                }
                else
                {
                    rect.Y += rect.Height - (offset1 + offset2);
                    rect.Height = offset2;
                }
            }
            #endregion

            #region Изменяем размер в зависимости от значений StartWidth и EndWidth
            bool isStartGreaterEnd = scale.StartWidth > scale.EndWidth;
            float restToLength = scale.barGeometry.GetRestToLenght();

            float offsetMax = length1 / length;
            float offsetMin = (length1 + length2) / length;
            if (!isStartGreaterEnd)
            {
                offsetMax = 1 - offsetMax;
                offsetMin = 1 - offsetMin;
            }

            offsetMax *= restToLength;
            offsetMin *= restToLength;

            float offsetRest = isStartGreaterEnd ? (offsetMin - offsetMax) : (offsetMax - offsetMin);
            float actualMinAscent;
            float actualMaxAscent = 0f;

            if (scale.Orientation == Orientation.Horizontal)
            #region Horizontal
            {
                actualMinAscent = this.size.Height * Math.Min(StartWidth, EndWidth);
                actualMaxAscent = this.size.Height * Math.Max(StartWidth, EndWidth);
                float offsetY = this.size.Height * Offset;

                if (isStartGreaterEnd)
                {
                    switch (Placement)
                    {
                        case StiPlacement.Outside:
                            rect.Y -= actualMaxAscent - offsetMin + offsetRest;
                            rect.Height = actualMaxAscent + offsetRest;
                            rect.Y -= offsetY;
                            break;

                        case StiPlacement.Overlay:
                            rect.Y = StiRectangleHelper.CenterY(rect) - actualMaxAscent / 2;
                            rect.Height = actualMaxAscent;
                            rect.Y += offsetY;
                            break;

                        case StiPlacement.Inside:
                            rect.Y += rect.Height - offsetMin;
                            rect.Height = actualMaxAscent + offsetRest;
                            rect.Y += offsetY;
                            break;
                    }
                }
                else
                {
                    switch (Placement)
                    {
                        case StiPlacement.Outside:
                            rect.Y -= actualMaxAscent - offsetMax + offsetRest;
                            rect.Height = actualMaxAscent + offsetRest;
                            rect.Y -= offsetY;
                            break;

                        case StiPlacement.Overlay:
                            rect.Y = StiRectangleHelper.CenterY(rect) - actualMaxAscent / 2;
                            rect.Height = actualMaxAscent;
                            rect.Y += offsetY;
                            break;

                        case StiPlacement.Inside:
                            rect.Y += rect.Height - offsetMax;
                            rect.Height = actualMaxAscent + offsetRest;
                            rect.Y += offsetY;
                            break;
                    }
                }
            }
            #endregion
            else
            #region Vertical
            {
                actualMinAscent = this.size.Width * Math.Min(StartWidth, EndWidth);
                actualMaxAscent = this.size.Width * Math.Max(StartWidth, EndWidth);
                float offsetX = this.size.Width * Offset;

                if (isStartGreaterEnd)
                {
                    switch (Placement)
                    {
                        case StiPlacement.Outside:
                            rect.X -= actualMaxAscent - offsetMin + offsetRest;
                            rect.Width = actualMaxAscent + offsetRest;
                            rect.X -= offsetX;
                            break;

                        case StiPlacement.Overlay:
                            rect.X = StiRectangleHelper.CenterX(rect) - actualMaxAscent / 2;
                            rect.Width = actualMaxAscent;
                            rect.X += offsetX;
                            break;

                        case StiPlacement.Inside:
                            rect.X += rect.Width - offsetMin;
                            rect.Width = actualMaxAscent + offsetRest;
                            rect.X += offsetX;
                            break;
                    }
                }
                else
                {
                    switch (Placement)
                    {
                        case StiPlacement.Outside:
                            rect.X -= actualMaxAscent - offsetMax + offsetRest;
                            rect.Width = actualMaxAscent + offsetRest;
                            rect.X -= offsetX;
                            break;

                        case StiPlacement.Overlay:
                            rect.X = StiRectangleHelper.CenterX(rect) - actualMaxAscent / 2;
                            rect.Width = actualMaxAscent;
                            rect.X += offsetX;
                            break;

                        case StiPlacement.Inside:
                            rect.X += rect.Width - offsetMax;
                            rect.Width = actualMaxAscent + offsetRest;
                            rect.X += offsetX;
                            break;
                    }
                }
            }
            #endregion
            #endregion

            return returnOnlyRect ? null : DrawPrimitiveGeometry(context, rect, actualMinAscent, actualMaxAscent,
                StartWidth, EndWidth, Placement, offsetRest, isStartGreaterEnd);
        }

        public StiGraphicsPathLinesGaugeGeom DrawPrimitiveGeometry(StiGaugeContextPainter context, RectangleF rect, float minAscent, float maxAscent,
            float StartWidth, float EndWidth, StiPlacement Placement, float restOffset, bool isStartGreaterEnd)
        {
            var rect1 = rect;
            rect.X = 0;
            rect.Y = 0;
            var points = new PointF[4];

            if (scale.StartWidth == scale.EndWidth && minAscent == maxAscent)
            {
                points[0] = new PointF(rect.Left, rect.Top);
                points[1] = new PointF(rect.Right, rect.Top);
                points[2] = new PointF(rect.Right, rect.Bottom);
                points[3] = new PointF(rect.Left, rect.Bottom);
            }
            else
            {
                bool isBarUp = (scale.StartWidth < scale.EndWidth);
                bool isRangeUp = (StartWidth <= EndWidth);
                if (scale.IsReversed)
                {
                    isBarUp = !isBarUp;
                    isRangeUp = !isRangeUp;
                }

                if (scale.Orientation == Orientation.Horizontal)
                #region Horizontal
                {
                    if (isBarUp)
                    {
                        if (isRangeUp)
                        {
                            switch (Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, rect.Height - minAscent);
                                    points[1] = new PointF(rect.Width, rect.Height - restOffset - maxAscent);
                                    points[2] = new PointF(rect.Width, rect.Height - restOffset);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, StiRectangleHelper.CenterY(rect) - minAscent / 2);
                                    points[1] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) - maxAscent / 2);
                                    points[2] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) + maxAscent / 2);
                                    points[3] = new PointF(0, StiRectangleHelper.CenterY(rect) + minAscent / 2);
                                    break;

                                default:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, restOffset);
                                    points[2] = new PointF(rect.Width, restOffset + maxAscent);
                                    points[3] = new PointF(0, minAscent);
                                    break;
                            }
                        }
                        else
                        {
                            switch (Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, rect.Height - maxAscent);
                                    points[1] = new PointF(rect.Width, rect.Height - restOffset - minAscent);
                                    points[2] = new PointF(rect.Width, rect.Height - restOffset);
                                    points[3] = new PointF(0, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, StiRectangleHelper.CenterY(rect) - maxAscent / 2);
                                    points[1] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) - minAscent / 2);
                                    points[2] = new PointF(rect.Width, StiRectangleHelper.CenterY(rect) + minAscent / 2);
                                    points[3] = new PointF(0, StiRectangleHelper.CenterY(rect) + maxAscent / 2);
                                    break;

                                default:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(rect.Width, restOffset);
                                    points[2] = new PointF(rect.Width, restOffset + minAscent);
                                    points[3] = new PointF(0, maxAscent);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (isRangeUp)
                        {
                            switch (Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, rect.Height - restOffset - minAscent);
                                    points[1] = new PointF(rect.Width, rect.Height - maxAscent);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(0, rect.Height - restOffset);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, StiRectangleHelper.CenterY(rect) - minAscent / 2);
                                    points[1] = new PointF(rect.Right, StiRectangleHelper.CenterY(rect) - maxAscent / 2);
                                    points[2] = new PointF(rect.Right, StiRectangleHelper.CenterY(rect) + maxAscent / 2);
                                    points[3] = new PointF(0, StiRectangleHelper.CenterY(rect) + minAscent / 2);
                                    break;

                                default:
                                    points[0] = new PointF(0, restOffset);
                                    points[1] = new PointF(rect.Right, 0);
                                    points[2] = new PointF(rect.Right, maxAscent);
                                    points[3] = new PointF(0, restOffset + minAscent);
                                    break;
                            }
                        }
                        else
                        {
                            switch (Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(0, rect.Height - restOffset - maxAscent);
                                    points[1] = new PointF(rect.Right, rect.Height - minAscent);
                                    points[2] = new PointF(rect.Right, rect.Height);
                                    points[3] = new PointF(0, rect.Height - restOffset);
                                    break;


                                case StiPlacement.Overlay:
                                    points[0] = new PointF(0, StiRectangleHelper.CenterY(rect) - maxAscent / 2);
                                    points[1] = new PointF(rect.Right, StiRectangleHelper.CenterY(rect) - minAscent / 2);
                                    points[2] = new PointF(rect.Right, StiRectangleHelper.CenterY(rect) + minAscent / 2);
                                    points[3] = new PointF(0, StiRectangleHelper.CenterY(rect) + maxAscent / 2);
                                    break;

                                default:
                                    points[0] = new PointF(0, restOffset);
                                    points[1] = new PointF(rect.Right, 0);
                                    points[2] = new PointF(rect.Right, minAscent);
                                    points[3] = new PointF(0, restOffset + maxAscent);
                                    break;
                            }
                        }
                    }
                }
                #endregion
                else
                #region Vertical
                {
                    if (isBarUp)
                    #region Bar - Up
                    {
                        if (isRangeUp)
                        {
                            switch (Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(rect.Width - restOffset - maxAscent, 0);
                                    points[1] = new PointF(rect.Width - restOffset, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(rect.Width - minAscent, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(StiRectangleHelper.CenterX(rect) - maxAscent / 2, 0);
                                    points[1] = new PointF(StiRectangleHelper.CenterX(rect) + maxAscent / 2, 0);
                                    points[2] = new PointF(StiRectangleHelper.CenterX(rect) + minAscent / 2, rect.Height);
                                    points[3] = new PointF(StiRectangleHelper.CenterX(rect) - minAscent / 2, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(restOffset, 0);
                                    points[1] = new PointF(restOffset + maxAscent, 0);
                                    points[2] = new PointF(minAscent, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;
                            }
                        }
                        else
                        {
                            switch (Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(rect.Width - restOffset - minAscent, 0);
                                    points[1] = new PointF(rect.Width - restOffset, 0);
                                    points[2] = new PointF(rect.Width, rect.Height);
                                    points[3] = new PointF(rect.Width - maxAscent, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(StiRectangleHelper.CenterX(rect) - minAscent / 2, 0);
                                    points[1] = new PointF(StiRectangleHelper.CenterX(rect) + minAscent / 2, 0);
                                    points[2] = new PointF(StiRectangleHelper.CenterX(rect) + maxAscent / 2, rect.Height);
                                    points[3] = new PointF(StiRectangleHelper.CenterX(rect) - maxAscent / 2, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(restOffset, 0);
                                    points[1] = new PointF(restOffset + minAscent, 0);
                                    points[2] = new PointF(maxAscent, rect.Height);
                                    points[3] = new PointF(0, rect.Height);
                                    break;
                            }
                        }
                    }
                    #endregion
                    else
                    #region Bar - Down
                    {
                        if (isRangeUp)
                        {
                            switch (Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(rect.Width - maxAscent, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width - restOffset, rect.Height);
                                    points[3] = new PointF(rect.Width - restOffset - minAscent, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(StiRectangleHelper.CenterX(rect) - maxAscent / 2, 0);
                                    points[1] = new PointF(StiRectangleHelper.CenterX(rect) + maxAscent / 2, 0);
                                    points[2] = new PointF(StiRectangleHelper.CenterX(rect) + minAscent / 2, rect.Height);
                                    points[3] = new PointF(StiRectangleHelper.CenterX(rect) - minAscent / 2, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(rect.Left, 0);
                                    points[1] = new PointF(maxAscent, 0);
                                    points[2] = new PointF(restOffset + minAscent, rect.Height);
                                    points[3] = new PointF(restOffset, rect.Height);
                                    break;
                            }
                        }
                        else
                        {
                            switch (Placement)
                            {
                                case StiPlacement.Outside:
                                    points[0] = new PointF(rect.Width - minAscent, 0);
                                    points[1] = new PointF(rect.Width, 0);
                                    points[2] = new PointF(rect.Width - restOffset, rect.Height);
                                    points[3] = new PointF(rect.Width - restOffset - maxAscent, rect.Height);
                                    break;

                                case StiPlacement.Overlay:
                                    points[0] = new PointF(StiRectangleHelper.CenterX(rect) - minAscent / 2, 0);
                                    points[1] = new PointF(StiRectangleHelper.CenterX(rect) + minAscent / 2, 0);
                                    points[2] = new PointF(StiRectangleHelper.CenterX(rect) + maxAscent / 2, rect.Height);
                                    points[3] = new PointF(StiRectangleHelper.CenterX(rect) - maxAscent / 2, rect.Height);
                                    break;

                                default:
                                    points[0] = new PointF(0, 0);
                                    points[1] = new PointF(minAscent, 0);
                                    points[2] = new PointF(restOffset + maxAscent, rect.Height);
                                    points[3] = new PointF(restOffset, rect.Height);
                                    break;
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }

            points[0].X += rect1.Left;
            points[0].Y += rect1.Top;
            points[1].X += rect1.Left;
            points[1].Y += rect1.Top;
            points[2].X += rect1.Left;
            points[2].Y += rect1.Top;
            points[3].X += rect1.Left;
            points[3].Y += rect1.Top;

            return new StiGraphicsPathLinesGaugeGeom(points);
        }
        #endregion

        public StiLinearBarGeometry(StiLinearScale scale)
        {
            this.size = new SizeF(0, 0);
            this.rectGeometry = RectangleF.Empty;
            this.center = new PointF(0, 0);
            this.scale = scale;
        }
    }
}