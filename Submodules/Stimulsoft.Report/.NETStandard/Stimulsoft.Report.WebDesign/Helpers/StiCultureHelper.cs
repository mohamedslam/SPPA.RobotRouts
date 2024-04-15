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

using System;
using System.Globalization;
using System.Collections;

namespace Stimulsoft.Report.Web
{
    internal class StiCultureHelper
    {
        public static ArrayList GetItems(CultureTypes cultureType)
        {
            ArrayList result = new ArrayList();
            var cultures = CultureInfo.GetCultures(cultureType);
            
            foreach (CultureInfo cul in cultures)
            {
                if (cul.Name == String.Empty) continue;

                Hashtable cultureProps = new Hashtable();
                cultureProps["name"] = cul.Name;
                cultureProps["displayName"] = cul.DisplayName + "  (" + cul.Name + ")";
                result.Add(cultureProps);
            }

            result.Sort(new MyComparer("displayName"));

            return result;
        }

        private static Hashtable GetGlobalizationContainerObject(StiGlobalizationContainer globContainer, StiReport report)
        {
            Hashtable containerObject = new Hashtable();
            containerObject["cultureName"] = globContainer.CultureName;
            Hashtable items = new Hashtable();
            foreach (StiGlobalizationItem item in globContainer.Items)
            {
                items[item.PropertyName] = item.Text;
            }
            containerObject["items"] = items;
            containerObject["reportItems"] = globContainer.GetAllStringsForReport(report);

            return containerObject;
        }

        public static ArrayList GetReportGlobalizationStrings(StiReport report)
        {
            report.GlobalizationStrings.FillItemsFromReport();

            ArrayList globStrings = new ArrayList();
            foreach (StiGlobalizationContainer globContainer in report.GlobalizationStrings)
            {
                globStrings.Add(GetGlobalizationContainerObject(globContainer, report));
            }

            return globStrings;
        }

        public static void AddReportGlobalizationStrings(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            string cultureName = param["cultureName"] as string;
            ArrayList globStrings = new ArrayList();

            var container = new StiGlobalizationContainer(param["cultureName"] as string);            
            container.FillItemsFromReport(report);
            report.GlobalizationStrings.Add(container);

            Hashtable containerObject = GetGlobalizationContainerObject(container, report);
            callbackResult["globalizationStrings"] = containerObject;
        }

        public static void EditGlobalizationStrings(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            int index = Convert.ToInt32(param["index"]);
            if (report.GlobalizationStrings.Count > index)
            {
                report.GlobalizationStrings[index].CultureName = param["newName"] as string;
                callbackResult["success"] = true;
            }
        }

        public static void RemoveReportGlobalizationStrings(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            int index = Convert.ToInt32(param["index"]);
            if (report.GlobalizationStrings.Count > index)
            {
                report.GlobalizationStrings.RemoveAt(index);
                callbackResult["success"] = true;
            }            
        }

        public static void GetCultureSettingsFromReport(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            int index = Convert.ToInt32(param["index"]);
            if (report.GlobalizationStrings.Count > index)
            {
                var container = report.GlobalizationStrings[index];
                container.Items.Clear();
                container.FillItemsFromReport(report);
                Hashtable containerObject = GetGlobalizationContainerObject(container, report);
                callbackResult["globalizationStrings"] = containerObject;
            }
        }

        public static void SetCultureSettingsToReport(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.LocalizeReport(param["cultureName"] as string);
            var container = new StiGlobalizationContainer(param["cultureName"] as string);
            callbackResult["reportItems"] = container.GetAllStringsForReport(report);

            report.Info.Zoom = StiReportEdit.StrToDouble((string)param["zoom"]);
            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(report);
            callbackResult["reportGuid"] = param["reportGuid"];
            callbackResult["selectedObjectName"] = (string)param["selectedObjectName"];
        }

        public static void ApplyGlobalizationStrings(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            int index = Convert.ToInt32(param["index"]);
            if (report.GlobalizationStrings.Count > index)
            {
                var container = report.GlobalizationStrings[index];
                bool haveItem = false;

                foreach (StiGlobalizationItem item in container.Items)
                {
                    if (item.PropertyName == param["propertyName"] as string)
                    {
                        item.Text = param["propertyValue"] as string;
                        haveItem = true;
                        break;
                    }
                }

                if (!haveItem)
                {
                    container.Items.Add(new StiGlobalizationItem(param["propertyName"] as string, param["propertyValue"] as string));
                }
            }
        }

        public static void RemoveUnlocalizedGlobalizationStrings(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.GlobalizationStrings.RemoveUnlocalizedItemsFromReport();
        }

        class MyComparer : IComparer
        {
            string key;
            public MyComparer(string key)
            {
                this.key = key;
            }
            public int Compare(object x, object y)
            {
                return ((string)((Hashtable)x)[key]).CompareTo((string)((Hashtable)y)[key]);
            }
        }
    }
}