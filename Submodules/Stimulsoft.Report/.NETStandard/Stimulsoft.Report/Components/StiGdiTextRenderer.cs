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
using System.Drawing;
using System.Drawing.Text;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Components
{
	public static class StiGdiTextRenderer
	{
        #region Methods
        public static void DrawString(Graphics g, RectangleD rect, string text, StiText textBox)
        {
            var fontSize = (float)(textBox.Font.Size * textBox.Page.Zoom);
            var factorX = g.DpiX / 100;
            var factorY = g.DpiX / 100;

            fontSize *= factorX;

            using (var font = StiFontUtils.ChangeFontSize(textBox.Font, fontSize))
            using (var sf = textBox.TextOptions.GetStringFormat())
            {
                sf.Alignment = StiTextDrawing.GetAlignment(textBox.HorAlignment);
                sf.LineAlignment = StiTextDrawing.GetAlignment(textBox.VertAlignment);

                var gdiTextRect = rect.ToRectangle();
                gdiTextRect.X += (int) Math.Round(g.Transform.OffsetX);
                gdiTextRect.Y += (int) Math.Round(g.Transform.OffsetY);

                if (textBox.IsPrinting)
                {
                    gdiTextRect.X = (int) (factorX * gdiTextRect.X);
                    gdiTextRect.Y = (int) (factorY * gdiTextRect.Y);
                    gdiTextRect.Width = (int) (factorX * gdiTextRect.Width);
                    gdiTextRect.Height = (int) (factorY * gdiTextRect.Height);
                }

                var flags = GetTextFormatFlagsFromStringFormat(sf);
                TextRenderer.DrawText(g, text, font, gdiTextRect, StiBrush.ToColor(textBox.TextBrush), flags);
            }
        }

		private static TextFormatFlags GetTextFormatFlagsFromStringFormat(StringFormat format)
		{
			var flags = TextFormatFlags.Default;
			if (format == null)return flags;

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
        #endregion
    }
}