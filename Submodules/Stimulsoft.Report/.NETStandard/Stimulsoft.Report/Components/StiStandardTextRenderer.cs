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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using System;
using System.Drawing;
using System.Drawing.Text;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Components
{
    public static class StiStandardTextRenderer
	{
		public static void DrawString(Graphics g, RectangleD rect, string text, StiText textBox)
		{
            DrawString(g, rect, text, textBox, (float)textBox.Page.Zoom);
		}

        public static void DrawString(Graphics g, RectangleD rect, string text, StiText textBox, float zoom)
        {
            if (StiScale.IsNoScaling)
            {
                if (!textBox.IsPrinting && StiDpiHelper.NeedDeviceCapsScale)
                    zoom = (float)(zoom * StiDpiHelper.DeviceCapsScale);
            }

            if (StiOptions.Engine.UseGdi32ForTextRendering && textBox.TextOptions.Angle == 0)
                DrawStringGdi(g, rect, text, textBox, zoom);

            else
            {
                var textRect = rect;
                var fontSize = textBox.Font.Size * zoom;

                if (textBox.Report.IsPrinting)
                {
                    if (textBox.Font.Unit != GraphicsUnit.Pixel && textBox.Font.Unit != GraphicsUnit.World)
                        fontSize *= StiOptions.Engine.StandardTextQualityScale / 100f;
                }
                else
                {
                    if (textBox.Font.Unit != GraphicsUnit.Pixel && textBox.Font.Unit != GraphicsUnit.World)
                        fontSize *= StiOptions.Engine.StandardTextQualityScale / 96f;
                }

                #region Check fit text, part1
                StiViewerFitTextHelper.StiFitTextInfo fitInfo = null;
                object hashObj = null;
                string hashSt = null;
                if (textBox.WordWrap && fontSize >= 3)
                {
                    fitInfo = StiViewerFitTextHelper.GetReportInfo(textBox.Report);
                    if (fitInfo != null)
                        hashObj = fitInfo.GetFontSizeObject(textBox, rect, text, ref fontSize, ref hashSt);
                }
                #endregion

                using (var font = StiFontUtils.ChangeFontSize(textBox.Font, fontSize))
                {
                    #region Check fit text, part2
                    Font newFont = null;
                    if (fitInfo != null && hashObj == null)
                    {
                        var sized = StiTextDrawing.MeasureString(g, text, font, textRect.Width, textBox.TextOptions, textBox.HorAlignment, textBox.VertAlignment, false);
                        var ep = font.Size * 0.2f;
                        if (sized.Height > rect.Height + ep)
                        {
                            while (sized.Height > rect.Height + ep)
                            {
                                fontSize -= 0.5f;
                                if (newFont != null)
                                    newFont.Dispose();

                                newFont = StiFontUtils.ChangeFontSize(font, fontSize);
                                sized = StiTextDrawing.MeasureString(g, text, newFont, textRect.Width, textBox.TextOptions, textBox.HorAlignment, textBox.VertAlignment, false);
                            }

                            fitInfo.HashText[hashSt] = fontSize;
                            fitInfo.HashComponent[textBox] = fontSize;
                        }
                        else
                        {
                            fitInfo.HashText[hashSt] = false;
                            fitInfo.HashComponent[textBox] = false;
                        }
                    }
                    #endregion

                    using (var brush = StiBrush.GetBrush(textBox.TextBrush, textRect.ToRectangleF()))
                    {
                        StiTextDrawing.DrawString(g, text, newFont ?? font, brush, textRect,
                            textBox.TextOptions, textBox.HorAlignment, textBox.VertAlignment,
                            false, (float)textBox.Page.Zoom, textBox.LineSpacing * StiOptions.Engine.TextLineSpacingScale);
                    }

                    if (newFont != null)
                        newFont.Dispose();
                }
            }
        }

        public static void DrawStringGdi(Graphics g, RectangleD rect, string text, StiText textBox, float zoom)
		{
			var textRect = rect;
           
            var factorX = 1f;
            var factorY = 1f;
				
			var fontSize = textBox.Font.Size * zoom;
			factorX *= g.DpiX / 100;
            factorY *= g.DpiX / 100;

            fontSize *= factorX;

			using (var font = StiFontUtils.ChangeFontSize(textBox.Font, fontSize))
            using (var sf = textBox.TextOptions.GetStringFormat())
            {
                sf.Alignment = StiTextDrawing.GetAlignment(textBox.HorAlignment);
                sf.LineAlignment = StiTextDrawing.GetAlignment(textBox.VertAlignment);
				
				var gdiTextRect = textRect.ToRectangle();
                gdiTextRect.X += (int)Math.Round(g.Transform.OffsetX);
                gdiTextRect.Y += (int)Math.Round(g.Transform.OffsetY);

                if (textBox.IsPrinting)
                {
					gdiTextRect.X = (int)(factorX * gdiTextRect.X);
                    gdiTextRect.Y = (int)(factorY * gdiTextRect.Y);
                    gdiTextRect.Width = (int)(factorX * gdiTextRect.Width);
                    gdiTextRect.Height = (int)(factorY * gdiTextRect.Height);
                }

                var flags = GetTextFormatFlagsFromStringFormat(sf);
                TextRenderer.DrawText(g, text, font, gdiTextRect, StiBrush.ToColor(textBox.TextBrush), flags);
			}
		}		

		public static SizeD MeasureString(double maxWidth, Font font, StiText textBox)
		{
            var graphics = textBox.GetMeasureGraphics();
            lock (graphics)
			{
                var fontSize = font.Size * StiDpiHelper.DeviceCapsScale;
			    if (font.Unit != GraphicsUnit.Pixel && font.Unit != GraphicsUnit.World)
			        fontSize *= StiOptions.Engine.StandardTextQualityScale / 96f;

			    var tempFont = font;
			    if (StiDpiHelper.NeedDeviceCapsScale || StiOptions.Engine.StandardTextQualityScale != 96)
			        tempFont = StiFontUtils.ChangeFontSize(font, (float) fontSize);

			    var size = StiTextDrawing.MeasureString(graphics, textBox.Text,
					tempFont, maxWidth, textBox.TextOptions, 
					textBox.HorAlignment, textBox.VertAlignment, false, textBox.LineSpacing * StiOptions.Engine.TextLineSpacingScale);

                //correction of the width to prevent rounding error
                var report = textBox.Report;
                if (report != null && report.ReportUnit != StiReportUnitType.HundredthsOfInch)
                {
                    var unit = report.Unit;
                    var margins = textBox.Margins;
                    var fullWidth = size.Width + margins.Left + margins.Right + textBox.Border.Size;

                    if (unit.ConvertToHInches(Math.Round(unit.ConvertFromHInches(fullWidth), 2)) < fullWidth)
                        size.Width += unit.ConvertToHInches(0.01);
                }

				if (textBox.Angle == 90 || textBox.Angle == 180)
					size.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorStandard;

				else
					size.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorStandard;

				return size;
			}
		}
		
        private static TextFormatFlags GetTextFormatFlagsFromStringFormat(StringFormat format)
        {
            var flags = TextFormatFlags.Default;

            if (format == null)
                return flags;

            float dummy;
            if (format.GetTabStops(out dummy) != null)
                flags |= TextFormatFlags.ExpandTabs;

            if ((format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0)
                flags |= TextFormatFlags.RightToLeft;

            if ((format.FormatFlags & StringFormatFlags.FitBlackBox) != 0)
                flags |= TextFormatFlags.NoPadding;

            if ((format.FormatFlags & StringFormatFlags.NoClip) != 0)
                flags |= TextFormatFlags.NoClipping;

            flags |= TextFormatFlags.WordBreak;

            if ((format.FormatFlags & StringFormatFlags.LineLimit) != 0)
                flags |= TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;

            switch (format.Alignment)
            {
                case StringAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;

                case StringAlignment.Far:
                    flags |= TextFormatFlags.Right;
                    break;

                default:
                    flags |= TextFormatFlags.Left;
                    break;
            }

            switch (format.LineAlignment)
            {
                case StringAlignment.Center:
                    flags |= TextFormatFlags.VerticalCenter;
                    break;

                case StringAlignment.Far:
                    flags |= TextFormatFlags.Bottom;
                    break;

                default:
                    flags |= TextFormatFlags.Top;
                    break;
            }

            switch (format.Trimming)
            {
                case StringTrimming.EllipsisCharacter:
                    flags |= TextFormatFlags.EndEllipsis;
                    break;

                case StringTrimming.EllipsisPath:
                    flags |= TextFormatFlags.PathEllipsis;
                    break;

                case StringTrimming.EllipsisWord:
                    flags |= TextFormatFlags.WordEllipsis;
                    break;
            }

            switch (format.HotkeyPrefix)
            {
                case HotkeyPrefix.Hide:
                    flags |= TextFormatFlags.HidePrefix;
                    break;

                case HotkeyPrefix.None:
                    flags |= TextFormatFlags.NoPrefix;
                    break;
            }

            return flags;
        }
	}
}
