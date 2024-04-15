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

using Stimulsoft.Report.BarCodes;
using System.Collections;

namespace Stimulsoft.Report.Web
{
    internal class StiBarCodeHelper
    {
        public static Hashtable GetBarCodeJSObject(StiBarCode barCode)
        {
            Hashtable properties = new Hashtable();
            properties["name"] = barCode.Name;
            properties["codeType"] = barCode.BarCodeType.GetType().Name;
            properties["properties"] = GetBarCodeProperties(barCode);
            properties["svgContent"] = StiReportEdit.GetSvgContent(barCode);

            return properties;
        }

        public static Hashtable GetBarCodeProperties(StiBarCode barCode)
        {
            Hashtable properties = new Hashtable();

            #region Common
            Hashtable propertiesCommon = new Hashtable();
            string[] propNamesCommon = { "Code", "Angle", "AutoScale", "ForeColor", "BackColor", "Font", "HorAlignment", "VertAlignment",
                "ShowLabelText", "ShowQuietZones" };
            foreach (string propName in propNamesCommon)
            {
                var value = StiReportEdit.GetPropertyValue(propName, barCode, true);
                if (value != null) propertiesCommon[StiReportEdit.LowerFirstChar(propName)] = value;
            }

            properties["common"] = propertiesCommon;
            #endregion

            #region Additional
            Hashtable propertiesAdditional = new Hashtable();
            string[] propNamesAdditional = { "Module", "Checksum", "CheckSum", "CheckSum1", "CheckSum2", "Height", "Ratio", "EncodingType", "MatrixSize",
                "UseRectangularSymbols", "SupplementCode", "ShowQuietZoneIndicator", "SupplementType", "AddClearZone", "PrintVerticalBars", "AspectRatio",
                "AutoDataColumns", "AutoDataRows", "DataColumns", "DataRows", "EncodingMode", "ErrorsCorrectionLevel", "RatioY", "Space", "ErrorCorrectionLevel",
                "CompanyPrefix", "ExtensionDigit", "SerialNumber", "Mode", "ProcessTilde", "StructuredAppendPosition", "StructuredAppendTotal", "TrimExcessData",
                "Image", "ImageMultipleFactor", "BodyBrush", "BodyShape", "EyeBallBrush", "EyeBallShape", "EyeFrameBrush", "EyeFrameShape"
            };

            foreach (string propName in propNamesAdditional)
            {
                var value = StiReportEdit.GetPropertyValue(propName, barCode.BarCodeType, true);
                if (value != null) propertiesAdditional[StiReportEdit.LowerFirstChar(propName)] = value;
            }

            properties["additional"] = propertiesAdditional;
            #endregion

            return properties;
        }

        public static void ApplyBarCodeProperty(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var barCode = report.Pages.GetComponentByName((string)param["componentName"]) as StiBarCode;
            if (barCode != null)
            {
                var property = param["property"] as Hashtable;
                var propertyName = property["name"] as string;

                if (propertyName == "codeType")
                    StiReportEdit.SetBarCodeTypeProperty(barCode, property["value"]);
                else
                    StiReportEdit.SetPropertyValue(report, StiReportEdit.UpperFirstChar(propertyName), barCode, property["value"]);

                callbackResult["barCode"] = GetBarCodeJSObject(barCode);
            }
        }
    }
}