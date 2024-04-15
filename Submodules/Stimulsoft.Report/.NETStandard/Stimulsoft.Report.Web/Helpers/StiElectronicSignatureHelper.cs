#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Import;
using Stimulsoft.Report.SignatureFonts;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Report.Web.Helpers.Dashboards;
using System;
using System.Collections;
using System.Drawing;
using System.Text;

#if STIDRAWING
using FontFamily = Stimulsoft.Drawing.FontFamily;
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiElectronicSignatureHelper
    {
        public static Hashtable GetSignatureData(StiReport report, StiRequestParams requestParams)
        {
            var getStyles = requestParams.GetBoolean("getStyles");
            var data = new Hashtable();

            data["signatures"] = GetSignatureComponents(report);

            if (getStyles)
            {
                data["styles"] = GetStylesForSignature();
            }

            return data;
        }

        private static ArrayList GetSignatureComponents(StiReport report)
        {
            var componets = new ArrayList();
            
            foreach (StiComponent component in report.GetRenderedComponents())
            {
                if (component is StiElectronicSignature signatureComp && component.Enabled)
                {
                    componets.Add(new Hashtable
                    {
                        ["name"] = signatureComp.Name,
                        ["signatureMode"] = signatureComp.Mode.ToString(),
                        ["typeFullName"] = signatureComp.Type.FullName,
                        ["typeInitials"] = signatureComp.Type.Initials,
                        ["typeStyle"] = signatureComp.Type.Style,
                        ["drawText"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(signatureComp.Text.Text)),
                        ["drawTextFont"] = StiDashboardElementViewHelper.GetFontJson(signatureComp.Text.Font),
                        ["drawTextColor"] = ColorToString(signatureComp.Text.Color),
                        ["drawTextHorAlignment"] = signatureComp.Text.HorAlignment.ToString(),
                        ["drawImage"] = signatureComp.Image.Image != null ? ImageToBase64(signatureComp.Image.Image) : string.Empty,
                        ["drawImageHorAlignment"] = signatureComp.Image.HorAlignment.ToString(),
                        ["drawImageVertAlignment"] = signatureComp.Image.VertAlignment.ToString(),
                        ["drawImageStretch"] = signatureComp.Image.Stretch,
                        ["drawImageAspectRatio"] = signatureComp.Image.AspectRatio
                    });
                }
            }

            return componets;
        }

        public static ArrayList GetStylesForSignature() 
        {
            var styles = new ArrayList()
            {
                new Hashtable() {
                    ["styleName"] = "Style1",
                    ["fontName"] = "Teddy Bear",
                    ["fontContent"] = GetSignatureFontContent("TeddyBear.ttf", StiResourceType.FontTtf)
                },
                new Hashtable() {
                    ["styleName"] = "Style2",
                    ["fontName"] = "MADE Likes Script",
                    ["fontContent"] = GetSignatureFontContent("MADELikesScript.otf", StiResourceType.FontOtf)
                },
                new Hashtable() {
                    ["styleName"] = "Style3",
                    ["fontName"] = "Denistina",
                    ["fontContent"] = GetSignatureFontContent("Denistina.ttf", StiResourceType.FontTtf)
                }
            };

            return styles;
        }

        public static string GetSignatureFontContent(string fileName, StiResourceType resourceType)
        {
            using (var fontStream = typeof(StiSignatureFontsHelper).Assembly.GetManifestResourceStream($"Stimulsoft.Base.SignatureFonts.{fileName}"))
            {
                if (null == fontStream)
                    return null;

                var fontStreamLength = (int)fontStream.Length;
                var fontData = new byte[fontStreamLength];
                fontStream.Read(fontData, 0, fontStreamLength);

                return StiReportResourceHelper.GetBase64DataFromFontResourceContent(resourceType, fontData);
            }
        }

        public static bool CheckSignedReport(StiReport report)
        {
            if ((report.PreviewSettings & (int)StiPreviewSettings.Signature) == 0)
                return false;

            foreach (StiComponent component in report.GetRenderedComponents())
            {
                if (component is StiElectronicSignature && component.Enabled)
                    return true;
            }

            return false;
        }

        private static string ColorToString(Color color)
        {
            if (color.A == 0)
                return "transparent";
            else
            {
                if (color.A == 255) 
                    return string.Format("{0},{1},{2}", color.R.ToString(), color.G.ToString(), color.B.ToString());
                else 
                    return string.Format("{0},{1},{2},{3}", color.A.ToString(), color.R.ToString(), color.G.ToString(), color.B.ToString());
            }
        }

        internal static Color StringToColor(string colorStr)
        {
            var newColor = Color.Transparent;

            if (!string.IsNullOrEmpty(colorStr) && colorStr != "transparent")
            {
                var colors = colorStr.Split(',');
                var isAlpha = colors.Length >= 4;

                var a = isAlpha ? Convert.ToInt32(colors[0]) : 255;
                var r = Convert.ToInt32(colors[isAlpha ? 1 : 0]);
                var g = Convert.ToInt32(colors[isAlpha ? 2 : 1]);
                var b = Convert.ToInt32(colors[isAlpha ? 3 : 2]);

                if (a == 0 && r == 255 && g == 255 && b == 255)
                    newColor = Color.Transparent;
                else
                    newColor = Color.FromArgb(a, r, g, b);
            }

            return newColor;
        }

        internal static Font JsonToFont(Hashtable fontJson)
        {
            var fontName = fontJson["name"] as string;
            var fontSize = Convert.ToDouble(fontJson["size"]);
            var fontStyle = (FontStyle)0;

            if (Convert.ToBoolean(fontJson["bold"]))
                fontStyle |= FontStyle.Bold;

            if (Convert.ToBoolean(fontJson["italic"]))
                fontStyle |= FontStyle.Italic;

            if (Convert.ToBoolean(fontJson["underline"]))
                fontStyle |= FontStyle.Underline;

            if (Convert.ToBoolean(fontJson["strikeout"]))
                fontStyle |= FontStyle.Strikeout;


            FontFamily fontFamily = StiFontCollection.IsCustomFont(fontName) ? StiFontCollection.GetFontFamily(fontName) : new FontFamily(fontName);
            try
            {
                return StiFontCollection.CreateFont(fontName, (float)fontSize, StiFontUtils.CorrectStyle(fontFamily.Name, fontStyle));
            }
            finally
            {
                if (!StiFontCollection.IsCustomFont(fontName)) fontFamily.Dispose();
            }
        }

        private static string ImageToBase64(byte[] bytes)
        {
            if (OleUnit.IsOleHeader(bytes))
            {
                //remove ole-link from array
                var objHeader = new OleUnit.ObjectHeader(bytes);
                var tempData = new byte[bytes.Length - objHeader.HeaderLen];
                Array.Copy(bytes, objHeader.HeaderLen, tempData, 0, bytes.Length - objHeader.HeaderLen);
                bytes = tempData;
            }

            var mimeType = "data:image;base64,";

            if (Report.Helpers.StiImageHelper.IsPng(bytes))
            {
                mimeType = "data:image/png;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsMetafile(bytes))
            {
                mimeType = "data:image/x-wmf;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsBmp(bytes))
            {
                mimeType = "data:image/bmp;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsJpeg(bytes))
            {
                mimeType = "data:image/jpeg;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsGif(bytes))
            {
                mimeType = "data:image/gif;base64,";
            }
            else if (Base.Helpers.StiSvgHelper.IsSvg(bytes))
            {
                mimeType = "data:image/svg+xml;base64,";
            }

            return mimeType + Convert.ToBase64String(bytes);
        }

        internal static byte[] Base64ToByteArray(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;
            return Convert.FromBase64String(base64String.IndexOf("base64,") >= 0 ? base64String.Substring(base64String.IndexOf("base64,") + 7) : base64String);
        }
    }
}