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
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiFontIconsExHelper
    {
        #region Methods
        public static bool NeedToUseStimulsoftFont(StiReport report)
        {
            if (report != null)
            {
                foreach (StiComponent comp in report.GetComponents())
                {
                    if (comp.Enabled)
                    {
                        if ((comp as StiImage)?.Icon != null || (comp as IStiImageElement)?.Icon != null || comp is IStiIndicatorElement)
                            return true;

                        var chart = comp as StiChart;
                        if (chart != null && chart.Series.Count > 0 && (chart.Series[0] is StiPictorialSeries || chart.Series[0] is IStiFontIconsSeries))
                            return true;

                        var chartElement = comp as IStiChartElement;
                        if (chartElement != null && chartElement.IsPictorialChart)
                            return true;
                    }
                }
            }

            return false;
        }

        internal static float GetIconFontSize(StiContext context, SizeF size, string text)
        {
            var fontFamilyIcons = StiFontIconsHelper.GetFontFamilyIcons();

            var measureSize = context.MeasureString(text, new StiFontGeom(new Font(fontFamilyIcons, 1)));

            if (measureSize.Width == 0 || measureSize.Height == 0)
                return 0;

            var factorX = size.Width / measureSize.Width;
            var factorY = size.Height / measureSize.Height;
            var factor = factorX > factorY ? factorY : factorX;

            return factor;
        }
        #endregion

        #region Methods.Draw
        internal static void DrawFillIcons(StiContext context, object brush, RectangleF rect, SizeF singleSize, StiFontIcons icon, string toolTip)
        {
            var textIcon = StiFontIconsHelper.GetContent(icon);

            var fontFamilyIcons = StiFontIconsHelper.GetFontFamilyIcons();
            var fonSize = GetIconFontSize(context, singleSize, textIcon);
            var font = new Font(fontFamilyIcons, fonSize);
            var fontGeom = new StiFontGeom(font.FontFamily, font.FontFamily.Name, font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
            
            var y = rect.Y;
            var x = rect.X;
            while (y < rect.Bottom)
            {
                while (x < rect.Right)
                {
                    var drawElementRect = new RectangleF(x, y, singleSize.Width, singleSize.Height);
                    context.DrawString(textIcon, fontGeom, brush, drawElementRect, GetStringFormatGeom(context), true, toolTip);
                    x += singleSize.Width;
                }
                x = rect.X;
                y += singleSize.Height;
            }
        }

        internal static void DrawDirectionIcons(StiContext context, object brush, RectangleF rect, SizeF singleSize, StiFontIcons icon, string toolTip, bool verticalDirection, bool roundValues = false)
        {
            var textIcon = StiFontIconsHelper.GetContent(icon);

            var fontFamilyIcons = StiFontIconsHelper.GetFontFamilyIcons();
            var fonSize = GetIconFontSize(context, singleSize, textIcon);

            if (fonSize <= 0) return;

            var font = new Font(fontFamilyIcons, fonSize);
            var fontGeom = new StiFontGeom(font.FontFamily, font.FontFamily.Name, font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);

            if (!roundValues)
                context.PushClip(rect);

            if (verticalDirection)
            {
                var y = rect.Bottom;
                var x = rect.X;

                var firstDrawIcon = true;

                while (y > rect.Top)
                {
                    var drawElementRect = new RectangleF(x, y - singleSize.Height, singleSize.Width, singleSize.Height);

                    if (firstDrawIcon && roundValues)
                    {
                        firstDrawIcon = false;
                        context.DrawString(textIcon, fontGeom, brush, drawElementRect, GetStringFormatGeom(context), true, toolTip);
                    }

                    else if (!roundValues || roundValues && y - rect.Top > singleSize.Height / 2f)
                    {
                        context.DrawString(textIcon, fontGeom, brush, drawElementRect, GetStringFormatGeom(context), true, toolTip);
                    }

                    y -= singleSize.Height;
                }
            }
            else
            {
                var y = rect.Y;
                var x = rect.X;

                while (x < rect.Right)
                {
                    var drawElementRect = new RectangleF(x, y, singleSize.Width, singleSize.Height);
                                        
                    var firstDrawIcon = true;

                    if (firstDrawIcon && roundValues)
                    {
                        firstDrawIcon = false;
                        context.DrawString(textIcon, fontGeom, brush, drawElementRect, GetStringFormatGeom(context), true, toolTip);
                    }

                    else if (!roundValues || roundValues && rect.Right - x > singleSize.Width / 2f)
                    {
                        context.DrawString(textIcon, fontGeom, brush, drawElementRect, GetStringFormatGeom(context), true, toolTip);
                    }

                    x += singleSize.Width;
                }
            }

            if (!roundValues)
                context.PopClip();
        }

        private static StiStringFormatGeom GetStringFormatGeom(StiContext context)
        {
            var sf = context.GetDefaultStringFormat();
            sf.Trimming = StringTrimming.None;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            return sf;
        }
        #endregion
    }
}
