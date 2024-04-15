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
using System.Xml;

using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export.Services.Helpers;
using System;
using Stimulsoft.Base.Localization;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Export
{
    internal static class StiMathFormulaSvgHelper
    {
        #region Methods
        internal static void WriteEmptyDataMessage(XmlTextWriter writer, StiSvgData svgData)
        {
            if (svgData.Width > 50)
            {
                writer.WriteStartElement("image");
                writer.WriteAttributeString("x", StiContextSvgHelper.DoubleToString(svgData.Width / 2 - 16));
                writer.WriteAttributeString("y", StiContextSvgHelper.DoubleToString(svgData.Height / 2 - 16));
                writer.WriteAttributeString("width", "32");
                writer.WriteAttributeString("height", "32");
                writer.WriteStartAttribute("href");
                writer.WriteString($"data:image/png;base64,{Convert.ToBase64String(GetEmptyDataImage())}");
                writer.WriteRaw("\r\n");
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }

            var font = new Font("Arial", 8);
            var text = Loc.Get("PropertyMain", "NoElements");
            var textSize = MeasureTextWidth(text, font);

            if (svgData.Width > textSize + 30)
            {
                writer.WriteStartElement("text");
                writer.WriteAttributeString("x", StiContextSvgHelper.DoubleToString(svgData.Width / 2 - textSize / 2));
                writer.WriteAttributeString("y", StiContextSvgHelper.DoubleToString(svgData.Height / 2 + 30));
                writer.WriteAttributeString("style", "font-size:8pt; font-family:'Arial'; fill: #a9a9a9;");
                writer.WriteRaw(text);
                writer.WriteEndElement();
            }
        }

        private static byte[] GetEmptyDataImage()
        {
            var assembly = typeof(StiChartSvgHelper).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"Stimulsoft.Report.Images.NotDefined.StiMathFormula_x2.png"))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        private static double MeasureTextWidth(string text, Font baseFont)
        {
            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            {
                return g.MeasureString(text, baseFont).Width;
            }
        } 
        #endregion
    }
}
