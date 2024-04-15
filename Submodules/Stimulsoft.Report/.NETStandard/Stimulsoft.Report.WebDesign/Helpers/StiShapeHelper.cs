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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using System.Collections;
using System.Drawing.Imaging;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiShapeHelper
    {
        public static Hashtable GetShapeJSObject(StiShape shape)
        {
            Hashtable properties = new Hashtable();
            properties["name"] = shape.Name;
            //properties["shapeType"] = StiReportEdit.GetShapeTypeProperty(shape);
            properties["properties"] = GetShapeProperties(shape);
            properties["svgContent"] = StiReportEdit.GetSvgContent(shape);

            return properties;
        }

        private static Hashtable GetShapeProperties(StiShape shape)
        {
            var properties = new Hashtable();

            properties["shapeType"] = StiReportEdit.GetShapeTypeProperty(shape);
            properties["brush"] = StiReportEdit.BrushToStr(shape.Brush);
            properties["shapeBorderStyle"] = ((int)shape.Style).ToString();
            properties["size"] = StiReportEdit.DoubleToStr(shape.Size);
            properties["shapeBorderColor"] = StiReportEdit.GetStringFromColor(shape.BorderColor);
            properties["shapeText"] = shape.Text != null ? StiEncodingHelper.Encode(shape.Text) : "";
            properties["foreColor"] = StiReportEdit.GetStringFromColor(shape.ForeColor);
            properties["backgroundColor"] = StiReportEdit.GetStringFromColor(shape.BackgroundColor);
            properties["font"] = StiReportEdit.FontToStr(shape.Font);

            return properties;
        }

        public static void ApplyShapeProperty(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var shape = report.Pages.GetComponentByName((string)param["componentName"]) as StiShape;
            if (shape != null)
            {
                var property = param["property"] as Hashtable;
                var propertyName = property["name"] as string;

                if (propertyName == "shapeType")
                    StiReportEdit.SetShapeTypeProperty(shape, property["value"]);
                
                else if (propertyName == "shapeBorderStyle")
                    StiReportEdit.SetPropertyValue(report, "Style", shape, property["value"]);
                
                else if (propertyName == "shapeBorderColor")
                    StiReportEdit.SetPropertyValue(report, "BorderColor", shape, property["value"]);
                
                else if (propertyName == "shapeText")
                    StiReportEdit.SetPropertyValue(report, "Text", shape, StiEncodingHelper.DecodeString(property["value"] as string));
                
                else
                    StiReportEdit.SetPropertyValue(report, StiReportEdit.UpperFirstChar(propertyName), shape, property["value"]);

                callbackResult["shape"] = GetShapeJSObject(shape);
            }
        }
    }
}