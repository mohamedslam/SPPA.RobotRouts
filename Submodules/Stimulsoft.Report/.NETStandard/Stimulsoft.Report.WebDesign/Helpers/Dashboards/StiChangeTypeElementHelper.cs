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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using System.Collections;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    public class StiChangeTypeElementHelper
    { 
        #region Methods
        public static void ChangeTypeElement(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var element = report.Pages.GetComponentByName(param["elementName"] as string);
            if (element != null)
            {
                var originalElement = element;

                if (param["originalElementContent"] != null && param["originalElementType"] != null)
                {
                    originalElement = CreateNewComponent(param["originalElementType"] as string, report);
                    var jObject = JsonConvert.DeserializeObject<JObject>(param["originalElementContent"] as string);
                    originalElement.LoadFromJsonObject(jObject);
                    originalElement.Page = element.Page;
                    originalElement.Report = element.Report;
                }
                else
                {
                    var json = element.SaveToJsonObject(StiJsonSaveMode.Report);
                    callbackResult["originalElementContent"] = json.ToString();
                }

                var newComponent = CreateNewComponent(param["newType"] as string, report);
                if (newComponent == null) return;
                newComponent.Page = element.Page;

                var isOriginal = newComponent.GetType() == originalElement.GetType();
                if (isOriginal)
                    newComponent = originalElement;

                var parent = element.Parent;
                var page = element.Page;
                var index = parent.Components.IndexOf(element);
                parent.Components.Remove(element);
                parent.Components.Insert(index, newComponent);
                newComponent.Parent = parent;
                newComponent.Page = page;

                if (!isOriginal)
                {
                    try
                    {
                        var json = originalElement.SaveToJsonObject(StiJsonSaveMode.Report);
                        newComponent.LoadFromJsonObject(json);
                    }
                    catch
                    {
                    }

                    try
                    {
                        (newComponent as IStiConvertibleElement)?.ConvertFrom(originalElement as IStiElement);
                    }
                    catch
                    {
                    }

                    var styleComponent = newComponent as IStiDashboardElementStyle;
                    if (styleComponent != null && styleComponent.Style == StiElementStyleIdent.Custom)
                        styleComponent.Style = StiElementStyleIdent.Auto;
                }

                var newComponentProps = new Hashtable();
                newComponentProps["name"] = newComponent.Name;
                newComponentProps["typeComponent"] = newComponent.GetType().Name;
                newComponentProps["componentRect"] = StiReportEdit.GetComponentRect(newComponent);
                newComponentProps["parentName"] = StiReportEdit.GetParentName(newComponent);
                newComponentProps["parentIndex"] = StiReportEdit.GetParentIndex(newComponent).ToString();
                newComponentProps["componentIndex"] = StiReportEdit.GetComponentIndex(newComponent).ToString();
                newComponentProps["childs"] = StiReportEdit.GetAllChildComponents(newComponent);
                newComponentProps["svgContent"] = StiReportEdit.GetSvgContent(newComponent, StiReportEdit.StrToDouble((string)param["zoom"]));
                newComponentProps["pageName"] = newComponent.Page.Name;
                newComponentProps["properties"] = StiReportEdit.GetAllProperties(newComponent);

                callbackResult["newComponentProps"] = newComponentProps;
            }
        }

        private static StiComponent CreateNewComponent(string elementType, StiReport report)
        {
            switch (elementType) {
                case "StiTableElement":
                    return StiDashboardCreator.CreateTableElement(report) as StiComponent;

                case "StiChartElement":
                    return StiDashboardCreator.CreateChartElement(report) as StiComponent;

                case "StiGaugeElement":
                    return StiDashboardCreator.CreateGaugeElement(report) as StiComponent;

                case "StiPivotTableElement":
                    return StiDashboardCreator.CreatePivotTableElement(report) as StiComponent;

                case "StiIndicatorElement":
                    return StiDashboardCreator.CreateIndicatorElement(report) as StiComponent;

                case "StiProgressElement":
                    return StiDashboardCreator.CreateProgressElement(report) as StiComponent;

                case "StiCardsElement":
                    return StiDashboardCreator.CreateCardsElement(report) as StiComponent;

                case "StiRegionMapElement":
                    return StiDashboardCreator.CreateRegionMapElement(report) as StiComponent;

                case "StiOnlineMapElement":
                    return StiDashboardCreator.CreateOnlineMapElement(report) as StiComponent;

                case "StiComboBoxElement":
                    return StiDashboardCreator.CreateComboBoxElement(report) as StiComponent;

                case "StiDatePickerElement":
                    return StiDashboardCreator.CreateDatePickerElement(report) as StiComponent;

                case "StiListBoxElement":
                    return StiDashboardCreator.CreateListBoxElement(report) as StiComponent;

                case "StiTreeViewBoxElement":
                    return StiDashboardCreator.CreateTreeViewBoxElement(report) as StiComponent;

                case "StiTreeViewElement":
                    return StiDashboardCreator.CreateTreeViewElement(report) as StiComponent;

                case "StiButtonElement":
                    return StiDashboardCreator.CreateButtonElement(report) as StiComponent;
            }

            return null;
        }
        #endregion
    }
}
