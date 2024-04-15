#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Stimulsoft.Report.Export
{
    public partial class StiPdfExportService
    {

        internal StiPdfData RenderIndicators(StiPdfData pp)
        {
            StiText textComp = pp.Component as StiText;
            if (textComp == null || textComp.Indicator == null) return pp;

            RectangleD rect = new RectangleD(pp.X, pp.Y, pp.Width, pp.Height);

            #region IconSet
            var iconSetIndicator = textComp.Indicator as StiIconSetIndicator;
            if (iconSetIndicator != null && (iconSetIndicator.Icon != StiIcon.None || iconSetIndicator.CustomIcon != null))
            {
                StiImageData pd = (StiImageData)imageList[imagesCurrent];
                float imageWidth = (float)(pd.Width * hiToTwips);
                float imageHeight = (float)(pd.Height * hiToTwips);

                var iconRect = rect.ToRectangleF();
                float cf = (float)hiToTwips;
                iconRect.Inflate(-cf, -cf);

                #region Alignment
                switch (iconSetIndicator.Alignment)
                {
                    case ContentAlignment.TopLeft:
                        iconRect = new RectangleF(iconRect.X, iconRect.Bottom - imageHeight, imageWidth, imageHeight);
                        rect.X += iconRect.Width + 1;
                        rect.Width -= iconRect.Width + 1;
                        break;

                    case ContentAlignment.TopCenter:
                        iconRect = new RectangleF(iconRect.X + (iconRect.Width - imageWidth) / 2, iconRect.Bottom - imageHeight, imageWidth, imageHeight);
                        break;

                    case ContentAlignment.TopRight:
                        iconRect = new RectangleF(iconRect.Right - imageWidth, iconRect.Bottom - imageHeight, imageWidth, imageHeight);
                        rect.Width -= iconRect.Width + 1;
                        break;

                    case ContentAlignment.MiddleLeft:
                        iconRect = new RectangleF(iconRect.X, iconRect.Y + (iconRect.Height - imageHeight) / 2, imageWidth, imageHeight);
                        rect.X += iconRect.Width + 1;
                        rect.Width -= iconRect.Width + 1;
                        break;

                    case ContentAlignment.MiddleCenter:
                        iconRect = new RectangleF(iconRect.X + (iconRect.Width - imageWidth) / 2, iconRect.Y + (iconRect.Height - imageHeight) / 2, imageWidth, imageHeight);
                        break;

                    case ContentAlignment.MiddleRight:
                        iconRect = new RectangleF(iconRect.Right - imageWidth, iconRect.Y + (iconRect.Height - imageHeight) / 2, imageWidth, imageHeight);
                        rect.Width -= iconRect.Width + 1;
                        break;

                    case ContentAlignment.BottomLeft:
                        iconRect = new RectangleF(iconRect.X, iconRect.Y, imageWidth, imageHeight);
                        rect.X += iconRect.Width + 1;
                        rect.Width -= iconRect.Width + 1;
                        break;

                    case ContentAlignment.BottomCenter:
                        iconRect = new RectangleF(iconRect.X + (iconRect.Width - imageWidth) / 2, iconRect.Y, imageWidth, imageHeight);
                        break;

                    case ContentAlignment.BottomRight:
                        iconRect = new RectangleF(iconRect.Right - imageWidth, iconRect.Y, imageWidth, imageHeight);
                        rect.Width -= iconRect.Width + 1;
                        break;
                }
                #endregion

                StiPdfData pp2 = new StiPdfData();
                pp2.X = iconRect.X;
                pp2.Y = iconRect.Y;
                pp2.Width = iconRect.Width;
                pp2.Height = iconRect.Height;

                WriteImageInfo2(pp2, 1, 1);
            }
            #endregion

            #region DataBar
            var barIndicator = textComp.Indicator as StiDataBarIndicator;
            if (barIndicator != null && barIndicator.Value != 0)
            {
                var barRect = rect; //.ToRectangleF();
                float cf = (float)hiToTwips;
                barRect.Inflate(-2 * cf, -2 * cf);

                float totalWidth = barIndicator.Maximum + Math.Abs(barIndicator.Minimum);

                double minimumPart = barRect.Width * Math.Abs(barIndicator.Minimum) / totalWidth;
                double maximumPart = barRect.Width * barIndicator.Maximum / totalWidth;
                double valuePart = barRect.Width * Math.Abs(barIndicator.Value) / totalWidth;

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

                bool hasGradient = false;

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

                            var brush = new StiGradientBrush(startColor, endColor, angle);
                            FillRectBrush(brush, barRect);

                            StiShadingData ssd = (StiShadingData)shadingArray[shadingCurrent - 1];
                            ssd.Angle = angle;
                            ssd.X = fillRect.X;
                            ssd.Y = fillRect.Y;
                            ssd.Width = fillRect.Width;
                            ssd.Height = fillRect.Height;
                            ssd.FunctionIndex = GetShadingFunctionNumber(startColor, endColor, false);
                            shadingArray[shadingCurrent - 1] = ssd;

                            hasGradient = true;
                        }
                    }
                    #endregion

                    #region StiBrushType.Solid
                    else
                    {
                        var brush = new StiSolidBrush(barIndicator.Value < 0 ? barIndicator.NegativeColor : barIndicator.PositiveColor);
                        FillRectBrush(brush, barRect);
                    }
                    #endregion

                    #region ShowBorder
                    if (barIndicator.ShowBorder)
                    {
                        pageStream.WriteLine("q");
                        PushColorToStack();

                        SetStrokeColor(barIndicator.Value < 0 ? barIndicator.NegativeBorderColor : barIndicator.PositiveBorderColor);

                        pageStream.WriteLine("{0} {1} {2} {3} re S",
                            ConvertToString(barRect.X),
                            ConvertToString(barRect.Y),
                            ConvertToString(barRect.Width),
                            ConvertToString(barRect.Height));

                        pageStream.WriteLine("Q");
                        PopColorFromStack();
                    }
                    #endregion
                }

                if (!hasGradient && barIndicator.BrushType == Stimulsoft.Report.Components.StiBrushType.Gradient)
                {
                    shadingCurrent++;
                }
            }
            #endregion

            pp.X = rect.X;
            pp.Width = rect.Width;
            return pp;
        }

    }
}
