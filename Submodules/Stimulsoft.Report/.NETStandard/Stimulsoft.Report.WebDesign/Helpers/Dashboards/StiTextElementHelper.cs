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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Export;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiTextElementHelper
    {
        #region Fields
        private IStiTextElement textElement;
        #endregion

        #region Helper Methods
        private Hashtable GetTextElementJSProperties()
        {
            Hashtable properties = StiReportEdit.GetAllProperties(textElement as StiComponent);
            properties["svgContent"] = StiEncodingHelper.Encode(StiDashboardsSvgHelper.SaveElementToString(textElement, 1, 1, true));

            return properties;
        }
        #endregion

        #region Methods
        public void ExecuteJSCommand(Hashtable parameters, Hashtable callbackResult)
        {
            switch ((string)parameters["command"])
            {
                case "SetProperty":
                    {
                        SetProperty(parameters, callbackResult);
                        break;
                    }
            }

            callbackResult["elementProperties"] = GetTextElementJSProperties();
        }

        private void SetProperty(Hashtable parameters, Hashtable callbackResult)
        {
            switch (parameters["propertyName"] as string)
            {
                case "text":
                    {
                        textElement.Text = StiEncodingHelper.DecodeString(parameters["propertyValue"] as string);
                        CheckFontProperties(textElement);
                        break;
                    }
                case "fontProperties":
                    {
                        var fontProperties = parameters["propertyValue"] as Hashtable;
                        fontProperties["foreColors"] = GetForeColorsProperty(textElement);
                        SetFontProperties(textElement, parameters["propertyValue"] as Hashtable);
                        break;
                    }
                case "clearAllFormatting":
                    {
                        ((IStiForeColor)textElement).ForeColor = Color.Transparent;
                        
                        var text = textElement.Text;
                        text = RemoveTagsFromText(text, "b");
                        text = RemoveTagsFromText(text, "i");
                        text = RemoveTagsFromText(text, "u");
                        text = RemoveTagsFromText(text, "em");
                        text = RemoveTagsFromText(text, "strong");
                        text = RemoveTagsFromText(text, "font-color");
                        text = RemoveTagsFromText(text, "text-align");
                        text = RemoveTagsFromText(text, "font");
                        text = text.Replace("color=", "");
                        text = text.Replace("face=", "");
                        textElement.Text = text;
                        break;
                    }
            }
        }

        private static string RemoveTagsFromText(string text, string tagName)
        {
            var startSymbols = new string[] { "<", "</" };
            var finishSymbols = new string[] { ">", "=", " " };

            foreach (string startSymbol in startSymbols)
            {
                while (text.Contains($"{startSymbol}{tagName}>") || text.Contains($"{startSymbol}{tagName}=") || text.Contains($"{startSymbol}{tagName} "))
                {
                    var startIndex = -1;
                    foreach (string finishSymbol in finishSymbols) 
                    {
                        var startIndexAlt = text.IndexOf($"{startSymbol}{tagName}{finishSymbol}");
                        if (startIndexAlt >= 0 && (startIndexAlt < startIndex || startIndex == -1))
                            startIndex = startIndexAlt;
                    }

                    if (startIndex >= 0)
                    {
                        var endIndex = text.IndexOf(">", startIndex, text.Length - startIndex);
                        if (endIndex >= 0)
                            text = text.Substring(0, startIndex) + (endIndex < text.Length - 1 ? text.Substring(endIndex + 1) : string.Empty);
                        else
                            break;
                    }
                }
            }

            return text;
        }

        public static void CheckFontProperties(IStiTextElement textElement)
        {
            var text = textElement.Text;
            if (text.IndexOf("<text-align") < 0)
            {
                var align = GetHorAlignmentProperty(textElement);
                text = $"<text-align=\"{align}\">{text}</text-align>";
            }
            if (text.IndexOf("<font") < 0)
            {
                var font = GetFontProperty(textElement);
                text = $"<font face=\"{font.Name}\" size=\"{font.Size}\">{text}</font>";
            }
            else
            {
                if (text.IndexOf("size=") < 0)
                {
                    var font = GetFontProperty(textElement);
                    text = text.Insert(text.IndexOf("<font") + 6, $" size=\"{font.Size}\" ");
                }
                if (text.IndexOf("face=") < 0)
                {
                    var font = GetFontProperty(textElement);
                    text = text.Insert(text.IndexOf("<font") + 6, $" face=\"{font.Name}\" ");
                }
            }
            textElement.Text = text;
        }

        public static void SetFontProperties(IStiTextElement textElement, Hashtable fontAttrs)
        {
            var text = textElement.Text;
            text = RemoveTagsFromText(text, "font");
            text = RemoveTagsFromText(text, "text-align");
            text = RemoveTagsFromText(text, "font-color");

            var openTags = string.Empty;
            var closeTags = string.Empty;

            if (fontAttrs["fontBold"] != null && fontAttrs["fontItalic"] != null && fontAttrs["fontUnderline"] != null)
            {
                text = RemoveTagsFromText(text, "b");
                text = RemoveTagsFromText(text, "i");
                text = RemoveTagsFromText(text, "u");
                text = RemoveTagsFromText(text, "strong");

                if (Convert.ToBoolean(fontAttrs["fontBold"]))
                {
                    openTags += "<b>";
                    closeTags = "</b>" + closeTags;
                }
                if (Convert.ToBoolean(fontAttrs["fontItalic"]))
                {
                    openTags += "<i>";
                    closeTags = "</i>" + closeTags;
                }
                if (Convert.ToBoolean(fontAttrs["fontUnderline"]))
                {
                    openTags += "<u>";
                    closeTags = "</u>" + closeTags;
                }
            }

            var foreColors = fontAttrs["foreColors"] as ArrayList;

            if (foreColors != null && foreColors.Count > 0)
            {
                if (foreColors.Count == 1)
                {
                    text = $"<font-color=\"{(foreColors[0] as Hashtable)["color"]}\">{text}</font-color>";
                }
                else {
                    foreColors.Cast<Hashtable>().ToList().ForEach(colorBlock =>
                    {
                        text = text.Replace(colorBlock["text"] as string, $"<font-color=\"{ colorBlock["color"]}\">{colorBlock["text"]}</font-color>");
                    });
                }
            }

            textElement.Text = $"<font face=\"{fontAttrs["fontName"]}\" size=\"{fontAttrs["fontSize"]}\"><text-align=\"{fontAttrs["horAlignment"]}\">{openTags}{text}{closeTags}</text-align></font>";
        }

        public static Font GetFontProperty(IStiTextElement textElement)
        {
            var text = textElement.Text;
            var fontName = "Arial";
            var fontSize = 12f;
            var style = FontStyle.Regular;

            if (!string.IsNullOrEmpty(text))
            {
                var sizeIndex = text.IndexOf("size=\"");
                if (sizeIndex >= 0)
                {
                    var sizeText = text.Substring(sizeIndex + 6);
                    var size = (float)StiValueHelper.TryToDouble(sizeText.Substring(0, sizeText.IndexOf("\"")));
                    if (size > 0) fontSize = size;
                }
                var faceIndex = text.IndexOf("face=\"");
                if (faceIndex >= 0)
                {
                    var faceText = text.Substring(faceIndex + 6);
                    var name = faceText.Substring(0, faceText.IndexOf("\""));
                    if (name.Length > 0) fontName = name;
                }                
                if (text.Contains("<b>") || text.Contains("<b style")) style |= FontStyle.Bold;
                if (text.Contains("<i>") || text.Contains("<i style")) style |= FontStyle.Italic;
                if (text.Contains("<u>") || text.Contains("<u style")) style |= FontStyle.Underline;
            }
            return new Font(fontName, fontSize, style);
        }

        public static StiTextHorAlignment GetHorAlignmentProperty(IStiTextElement textElement)
        {
            var horAlignment = StiTextHorAlignment.Center;
            var text = textElement.Text;            
            if (!string.IsNullOrEmpty(text))
            {
                var alignIndex = text.IndexOf("text-align=\"");
                if (alignIndex >= 0)
                {
                    var alignText = text.Substring(alignIndex + 12);
                    var align = alignText.Substring(0, alignText.IndexOf("\""));
                    if (align.Length > 0)
                        Enum.TryParse(StiReportEdit.UpperFirstChar(align), out horAlignment);
                }
            }
            return horAlignment;
        }

        private static string GetColorFromText(string text)
        {
            var colorAttr = "color=\"";

            if (!string.IsNullOrEmpty(text))
            {
                var colorIndex = text.IndexOf(colorAttr);
                if (colorIndex >= 0)
                {
                    var colorText = text.Substring(colorIndex + colorAttr.Length);
                    colorText = colorText.Substring(0, colorText.IndexOf("\""));
                    
                    if (colorText.Length > 0)
                        return colorText;
                }
            }

            return string.Empty;
        }

        public static ArrayList GetForeColorsProperty(IStiTextElement textElement)
        {
            var colors = new ArrayList();
            var text = textElement.Text;

            if (!string.IsNullOrEmpty(text))
            {
                var colorsBlocks = text.Split(new string[] { "color=\"", "</font>", "</font-color>" }, StringSplitOptions.RemoveEmptyEntries);
                if (colorsBlocks.Length > 1)
                {
                    foreach (string colorBlock in colorsBlocks)
                    {
                        if (colorBlock.StartsWith("#")) 
                        {
                            var colorEndIndex = colorBlock.IndexOf("\"");
                            var color = colorBlock.Substring(0, colorEndIndex);
                            var textBlock = colorBlock.Substring(colorEndIndex + 2);

                            textBlock = RemoveTagsFromText(textBlock, "b");
                            textBlock = RemoveTagsFromText(textBlock, "i");
                            textBlock = RemoveTagsFromText(textBlock, "u");
                            textBlock = RemoveTagsFromText(textBlock, "strong");

                            if (textBlock.Length > 0 && color.Length > 0)
                            {
                                colors.Add(new Hashtable
                                {
                                    ["text"] = textBlock,
                                    ["color"] = color
                                });
                            }
                        }
                    }
                }
                else if (colorsBlocks.Length > 0)
                {
                    var color = GetColorFromText(text);
                    if (!string.IsNullOrEmpty(color))
                    {
                        colors.Add(new Hashtable
                        {
                            ["text"] = "",
                            ["color"] = color
                        });
                    }
                }
            }
            return colors;
        }

        public static void CreateTextElementFromDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.IsModified = true;
            string currentPageName = (string)param["pageName"];
            Hashtable point = (Hashtable)param["point"];
            Hashtable itemObject = (Hashtable)param["itemObject"];
            StiPage currentPage = report.Pages[currentPageName];

            var newComponent = StiDashboardHelper.CreateDashboardElement(report, "StiTextElement");
            double compWidth = newComponent.DefaultClientRectangle.Size.Width;
            double compHeight = newComponent.DefaultClientRectangle.Size.Height;
            double compXPos = StiReportEdit.StrToDouble((string)point["x"]);
            double compYPos = StiReportEdit.StrToDouble((string)point["y"]);

            if (compXPos + currentPage.Margins.Left + compWidth * 2 > currentPage.Width)
            {
                compXPos = currentPage.Width - currentPage.Margins.Left - compWidth * 2;
            }

            StiReportEdit.AddComponentToPage(newComponent, currentPage);
            RectangleD compRect = new RectangleD(new PointD(compXPos, compYPos), new SizeD(compWidth, compHeight));
            StiReportEdit.SetComponentRect(newComponent, compRect);
            ((IStiTextElement)newComponent).Text = StiEncodingHelper.DecodeString((string)itemObject["fullName"]);

            Hashtable componentProps = new Hashtable();
            componentProps["name"] = newComponent.Name;
            componentProps["typeComponent"] = newComponent.GetType().Name;
            componentProps["componentRect"] = StiReportEdit.GetComponentRect(newComponent);
            componentProps["parentName"] = StiReportEdit.GetParentName(newComponent);
            componentProps["parentIndex"] = StiReportEdit.GetParentIndex(newComponent).ToString();
            componentProps["componentIndex"] = StiReportEdit.GetComponentIndex(newComponent).ToString();
            componentProps["childs"] = StiReportEdit.GetAllChildComponents(newComponent);
            componentProps["svgContent"] = StiReportEdit.GetSvgContent(newComponent);
            componentProps["pageName"] = currentPage.Name;
            componentProps["properties"] = StiReportEdit.GetAllProperties(newComponent);
            componentProps["rebuildProps"] = StiReportEdit.GetPropsRebuildPage(report, currentPage);

            callbackResult["newComponent"] = componentProps;
        }
        #endregion

        #region Constructor
        public StiTextElementHelper(IStiTextElement textElement)
        {
            this.textElement = textElement;
        }
        #endregion   
    }
}