#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Collections;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Viewer;

namespace Stimulsoft.Report.Web
{
    internal class StiEditableFieldsHelper
    {
        public static bool CheckEditableReport(StiReport report)
        {
            if ((report.PreviewSettings & (int)StiPreviewSettings.Editor) == 0) 
                return false;

            foreach (StiComponent component in report.GetRenderedComponents())
            {
                if (component is StiText && ((StiText)component).Editable) return true;
                else if (component is StiCheckBox && ((StiCheckBox)component).Editable) return true;
                else if (component is StiRichText && ((StiRichText)component).Editable) return true;
            }

            return false;
        }        

        public static void ApplyEditableFieldsToReport(StiReport report, object parameters)
        {
            if (parameters == null) return;
            try
            {
                var allPagesParams = (Hashtable)parameters;
                foreach (DictionaryEntry pageParams in allPagesParams)
                {
                    int pageIndex = Convert.ToInt32(pageParams.Key);
                    Hashtable allComponetsParams = (Hashtable)pageParams.Value;

                    foreach (DictionaryEntry compParamsObject in allComponetsParams)
                    {
                        var compIndex = Convert.ToInt32(compParamsObject.Key);
                        var compParams = (Hashtable)compParamsObject.Value;

                        if (pageIndex < report.RenderedPages.Count)
                        {
                            StiPage page = report.RenderedPages[pageIndex];
                            if (compIndex < page.Components.Count)
                            {
                                StiComponent component = page.Components[compIndex];
                                if ((string)compParams["type"] == "CheckBox" && component is StiCheckBox)
                                {
                                    ((StiCheckBox)component).CheckedValue = (bool)compParams["checked"] ? "true" : "false";
                                }
                                else if ((string)compParams["type"] == "Text" && component is StiText)
                                {
                                    var text = (string)compParams["text"];
                                    if (((StiText)component).AllowHtmlTags) text = text.Replace("\n", "<br>");
                                    ((StiText)component).Text.Value = text;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        } 
    }
}
