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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Web
{
    internal class StiTableOfContentsHelper
    {
        private static List<string> GetIdents(object comp, StiTableOfContents tableOfContents)
        {
            var strs = new List<string>();

            var report = comp as StiReport;
            if (report != null)
            {
                if (!string.IsNullOrWhiteSpace(tableOfContents.ReportPointer))
                    strs.Add(tableOfContents.ReportPointer);

                if (!string.IsNullOrWhiteSpace(report.ReportAlias))
                    strs.Add(report.ReportAlias);

                if (!string.IsNullOrWhiteSpace(report.ReportName))
                    strs.Add(report.ReportName);
            }

            var page = comp as StiPage;
            if (page != null)
            {
                if (!string.IsNullOrWhiteSpace(page.Alias))
                    strs.Add(page.Alias);

                if (!string.IsNullOrWhiteSpace(page.Name))
                    strs.Add(page.Name);
            }

            var groupHeaderBand = comp as StiGroupHeaderBand;
            if (groupHeaderBand != null)
            {
                if (!string.IsNullOrWhiteSpace(groupHeaderBand.Condition.Value) && groupHeaderBand.Condition.Value.Contains("{"))
                    strs.Add(groupHeaderBand.Condition.Value);
            }

            var band = comp as StiBand;
            if (band != null)
            {
                if (!string.IsNullOrWhiteSpace(band.Bookmark.Value) && band.Bookmark.Value.Contains("{"))
                    strs.Add(band.Bookmark.Value);

                if (!string.IsNullOrWhiteSpace(band.Hyperlink.Value) && band.Hyperlink.Value.Contains("{"))
                    strs.Add(band.Hyperlink.Value);

                foreach (var element in band.Components.ToList().Where(c => c is StiText).Cast<StiText>())
                {
                    if (!string.IsNullOrWhiteSpace(element.Text.Value) && element.Text.Value.Contains("{"))
                        strs.Add(element.Text.Value);
                }
            }

            var text = comp as StiText;
            if (text != null)
            {
                if (!string.IsNullOrWhiteSpace(text.Bookmark.Value) && text.Bookmark.Value.Contains("{"))
                    strs.Add(text.Bookmark.Value);

                if (!string.IsNullOrWhiteSpace(text.Hyperlink.Value) && text.Hyperlink.Value.Contains("{"))
                    strs.Add(text.Hyperlink.Value);

                if (!string.IsNullOrWhiteSpace(text.Text.Value))
                {
                    if (text.Text.Value.Contains("{") || strs.Count == 0)
                        strs.Add(text.Text.Value);
                }
            }

            strs = strs.Distinct().ToList();

            if (strs.Count == 0)
                strs.Add("");

            return strs;
        }

        internal static void GetIdentsCollection(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var idents = new Hashtable();
            var tableOfContents = report.GetComponentByName(param["tableOfContentsName"] as string) as StiTableOfContents;
            if (tableOfContents != null)
            {
                var componentsNames = param["componentsNames"] as ArrayList;
                for (var i = 0; i < componentsNames.Count; i++)
                {
                    var component = report.GetComponentByName(componentsNames[i] as string);
                    if (component != null)
                    {
                        idents[component.Name] = GetIdents(component, tableOfContents);
                    }
                };
            }
            callbackResult["idents"] = idents;
        }

        internal static void UpdateComponentsPointerValues(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var changedComponents = param["changedComponents"] as Hashtable;
            var tableOfContents = report.GetComponentByName(param["tableOfContentsName"] as string) as StiTableOfContents;

            if (changedComponents != null && tableOfContents != null) 
            {
                foreach (DictionaryEntry changedComponent in changedComponents)
                {
                    var componentParams = changedComponent.Value as Hashtable;
                    var componentType = componentParams["componentType"] as string;
                    var componentName = componentParams["componentName"] as string;
                    var pointerValue = StiEncodingHelper.DecodeString(componentParams["pointerValue"] as string);

                    if (componentType == "StiReport") {
                        tableOfContents.ReportPointer = pointerValue;
                    }
                    else
                    {
                        var component = report.GetComponentByName(componentName);
                        if (component != null) component.Pointer.Value = pointerValue;
                    }
                }
            }
        }        
    }
}