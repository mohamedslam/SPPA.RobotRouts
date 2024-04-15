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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiTextHelper
    {
        public static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />

            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;

            //Decode html specific characters
            text = WebUtility.HtmlDecode(text);

            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");

            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);

            //Strip formatting
            return stripFormattingRegex.Replace(text, string.Empty);
        }

        public static string CheckHyperlinkFromHtmlText(StiComponent comp, RectangleD rect, Point point)
        {
            var textComp = comp as StiText;
            if ((textComp == null) || !textComp.CheckAllowHtmlTags() || string.IsNullOrWhiteSpace(textComp.Text)) 
                return null;

            try
            {
                #region Get runs info
                var rectComp = textComp.Page.Unit.ConvertToHInches(textComp.ComponentToPage(textComp.ClientRectangle));
                var rectText = textComp.ConvertTextMargins(rectComp, false);
                rectText = textComp.ConvertTextBorders(rectText, false);

                var textOptions = (textComp as IStiTextOptions).TextOptions;

                Graphics g;
                if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
                {
                    g = Graphics.FromHwnd(IntPtr.Zero);
                }
                else
                {
                    var imageForGraphicsForTextRenderer = new Bitmap(1, 1);
                    g = Graphics.FromImage(imageForGraphicsForTextRenderer);
                }
                var outRunsList = new List<StiTextRenderer.RunInfo>();
                var outFontsList = new List<StiTextRenderer.StiFontState>();

                StiTextRenderer.DrawTextForOutput(
                    g,
                    textComp.Text,
                    textComp.Font,
                    rectText,
                    StiBrush.ToColor(textComp.TextBrush),
                    StiBrush.ToColor(textComp.Brush),
                    textComp.LineSpacing * StiOptions.Engine.TextLineSpacingScale,
                    textComp.HorAlignment,
                    textComp.VertAlignment,
                    textOptions.WordWrap,
                    textOptions.RightToLeft,
                    1,
                    textOptions.Angle,
                    textOptions.Trimming,
                    textOptions.LineLimit,
                    textComp.CheckAllowHtmlTags(),
                    outRunsList,
                    outFontsList,
                    textOptions,
                    1);
                #endregion

                var scale = rectComp.Width / rect.Width;
                var pt = new PointD((point.X - rect.X) * scale, (point.Y - rect.Y) * scale);

                foreach (var run in outRunsList)
                {
                    if ((run.Href != null) && (pt.X >= run.XPos) && (pt.Y > run.YPos) && (pt.Y < run.YPos + outFontsList[run.FontIndex].LineHeight))
                    {
                        var sumW = run.Widths.Sum();
                        
                        if (pt.X < run.XPos + sumW)                        
                            return run.Href;
                        
                    }
                }
            }
            catch 
            {
            }

            return null;
        }
    }
}
