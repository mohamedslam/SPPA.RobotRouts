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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Units;
using System.Drawing;
using System.Drawing.Text;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
    public static class StiTypographicTextRender
    {
        public static void DrawString(Graphics g, RectangleD rect, string text, StiComponent component,
            Font textFont, StiBrush textBrush, StiTextOptions textOptions, StiTextHorAlignment textHorAlignment, StiVertAlignment textVertAlignment)
        {
            var textRect = rect;

            var zoom = component.Page.Zoom;
            if (StiScale.IsNoScaling)
            {
                if (!component.IsPrinting && StiDpiHelper.NeedDeviceCapsScale)
                    zoom *= StiDpiHelper.DeviceCapsScale;
            }

            var fontSize = (float)(textFont.Size * zoom);

            if (component.Report.IsPrinting)
            {
                if (textFont.Unit != GraphicsUnit.Pixel && textFont.Unit != GraphicsUnit.World)
                    fontSize *= StiOptions.Engine.TypographicTextQualityScale / 100f;
            }
            else
            {
                if (textFont.Unit != GraphicsUnit.Pixel && textFont.Unit != GraphicsUnit.World)
                    fontSize *= StiOptions.Engine.StandardTextQualityScale / 96f;
            }

            using (var font = StiFontUtils.ChangeFontSize(textFont, fontSize))
            using (var brush = StiBrush.GetBrush(textBrush, textRect.ToRectangleF()))
            {
                var defaultHint = g.TextRenderingHint;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                StiTextDrawing.DrawString(g, text, font, brush, textRect, textOptions, textHorAlignment, textVertAlignment, true, (float)component.Page.Zoom, (component as StiText).LineSpacing * StiOptions.Engine.TextLineSpacingScale);

                g.TextRenderingHint = defaultHint;
            }
        }

        public static void DrawString(Graphics g, RectangleD rect, string text, StiText textBox)
        {
            DrawString(g, rect, text, textBox, textBox.Font, textBox.TextBrush, textBox.TextOptions, textBox.HorAlignment, textBox.VertAlignment);
        }

        public static SizeD MeasureString(double width, Font font, StiText textBox)
        {
            var fontSize = (float)(font.Size * StiDpiHelper.DeviceCapsScale);

            if (font.Unit != GraphicsUnit.Pixel && font.Unit != GraphicsUnit.World)
                fontSize *= StiOptions.Engine.StandardTextQualityScale / 96f;

            TextRenderingHint defaultHint;
            lock (textBox.GetMeasureGraphics())
            {
                defaultHint = textBox.GetMeasureGraphics().TextRenderingHint;
                textBox.GetMeasureGraphics().TextRenderingHint = TextRenderingHint.AntiAlias;
            }

            SizeD actualSize;

            using (var newFont = StiFontUtils.ChangeFontSize(font, fontSize))
            {
                lock (textBox.GetMeasureGraphics())
                {
                    actualSize = StiTextDrawing.MeasureString(textBox.GetMeasureGraphics(), textBox.Text,
                        newFont, width, textBox.TextOptions, textBox.HorAlignment, textBox.VertAlignment, true, textBox.LineSpacing * StiOptions.Engine.TextLineSpacingScale);
                }
            }

            lock (textBox.GetMeasureGraphics())
            {
                textBox.GetMeasureGraphics().TextRenderingHint = defaultHint;
            }

            //correction of the width to prevent rounding error
            if (textBox.Report != null && textBox.Report.ReportUnit != StiReportUnitType.HundredthsOfInch)
            {
                var unit = textBox.Report.Unit;
                var fullWidth = actualSize.Width + textBox.Margins.Left + textBox.Margins.Right + textBox.Border.Size;

                if (unit.ConvertToHInches(Math.Round(unit.ConvertFromHInches(fullWidth), 2)) < fullWidth)
                    actualSize.Width += unit.ConvertToHInches(0.01);
            }

            if (textBox.Angle == 90 || textBox.Angle == 180)
                actualSize.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorTypographic;

            else
                actualSize.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorTypographic;

            return actualSize;
        }
    }
}
