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
using System.Collections;
using System.Web;

#if NETSTANDARD
using Stimulsoft.System.Web;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiDesignerOptionsHelper
    {
        public static Hashtable GetDefaultDesignerOptions()
        {
            Hashtable defaultOptions = new Hashtable();
            defaultOptions["showHeaders"] = StiSettings.GetBool("StiDesigner", "ShowHeaders", true);
            defaultOptions["showRulers"] = StiSettings.GetBool("StiDesigner", "ShowRulers", true);
            defaultOptions["showOrder"] = StiSettings.GetBool("StiDesigner", "ShowOrder", false);
            defaultOptions["runDesignerAfterInsert"] = StiSettings.GetBool("StiDesigner", "RunDesignerAfterInsert", true);
            defaultOptions["useLastFormat"] = StiSettings.GetBool("StiDesigner", "UseLastFormat", true);
            defaultOptions["showDimensionLines"] = StiSettings.GetBool("StiDesigner", "ShowDimensionLines", true);
            defaultOptions["generateLocalizedName"] = StiSettings.GetBool("StiDesigner", "GenerateLocalizedName", false);
            defaultOptions["alignToGrid"] = StiSettings.GetBool("StiDesigner", "AlignToGrid", true);
            defaultOptions["showGrid"] = StiSettings.GetBool("StiDesigner", "ShowGrid", true);
            defaultOptions["gridMode"] = StiSettings.Get("StiDesigner", "GridMode", StiGridMode.Lines).ToString();
            defaultOptions["gridSizeInch"] = StiSettings.GetDouble("StiDesigner", "GridSizeInch", 0.1).ToString();
            defaultOptions["gridSizeHundredthsOfInch"] = StiSettings.GetDouble("StiDesigner", "GridSizeHundredthsOfInch", 10).ToString();
            defaultOptions["gridSizeCentimetres"] = StiSettings.GetDouble("StiDesigner", "GridSizeCentimetres", 0.2).ToString();
            defaultOptions["gridSizeMillimeters"] = StiSettings.GetDouble("StiDesigner", "GridSizeMillimeters", 2).ToString();
            defaultOptions["gridSizePixels"] = StiSettings.GetDouble("StiDesigner", "GridSizePixels", 8).ToString();
            defaultOptions["quickInfoType"] = StiSettings.Get("StiDesigner", "QuickInfoType", StiQuickInfoType.None).ToString();
            defaultOptions["quickInfoOverlay"] = StiSettings.GetBool("StiDesigner", "QuickInfoOverlay", true);
            defaultOptions["autoSaveInterval"] = StiSettings.GetInt("StiDesigner", "AutoSaveInterval", 15).ToString();
            defaultOptions["enableAutoSaveMode"] = StiSettings.GetBool("StiDesigner", "EnableAutoSaveMode", false);
            defaultOptions["startScreen"] = StiSettings.GetStr("StiDesigner", "StartScreen", "Welcome");

            return defaultOptions;
        }

        public static Hashtable GetDesignerOptions(StiRequestParams requestParams)
        {
            var designerOptions = requestParams.GetString("designerOptions");
            if (!string.IsNullOrEmpty(designerOptions))
            {
                designerOptions = StiEncodingHelper.DecodeString(designerOptions);
                return (Hashtable)JSON.Decode(designerOptions);
            }
            else
            {
                return GetDefaultDesignerOptions();
            }
        }
        
        public static void ApplyDesignerOptionsToReport(Hashtable designerOptions, StiReport report)
        {
            if (designerOptions == null || report == null) return;

            foreach (DictionaryEntry property in designerOptions)
            {
                StiReportEdit.SetPropertyValue(report, StiReportEdit.UpperFirstChar((string)property.Key), report.Info, property.Value);
            }

            if (designerOptions["startScreen"] != null)
                StiSettings.Set("StiDesigner", "StartScreen", designerOptions["startScreen"] as string);
        }        
    }
}
