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

using Stimulsoft.Base;
using Stimulsoft.Base.Dashboard;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiTableElementViewHelper
    {
        #region Methods
        public static List<object> GetTableData(IStiTableElement tableElement, double scaleX = 1)
        {
            var resultData = new List<object>();
            var dataTable = StiElementDataCache.GetOrCreate(tableElement);

            if (dataTable != null)
            {
                var headerCells = new List<object>();
                var meters = dataTable.Meters.AsEnumerable().Where(m => m is IStiTableColumn).ToList();
                var columnIndex = 0;

                foreach (var meter in meters)
                {
                    var columnVisible = Convert.ToBoolean(StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Components.Table.StiTableColumnVisibilityHelper", "GetVisible", new object[] { meter as IStiTableColumn, tableElement.Report as IStiReport }));

                    if (columnVisible)
                    {
                        var dataColumn = (StiDataColumn)StiDataExpressionHelper.GetDataColumnFromExpression((IStiDashboard)tableElement.Page, meter.Expression);
                        var columnObj = new Hashtable();

                        columnObj["columnIndex"] = columnIndex;
                        columnObj["key"] = meter.Key;
                        columnObj["label"] = StiLabelHelper.GetLabel(meter);
                        columnObj["labelCorrect"] = StiNameValidator.CorrectName(StiLabelHelper.GetLabel(meter));
                        columnObj["path"] = dataColumn != null ? dataColumn.GetColumnPath() : string.Empty;
                        columnObj["type"] = meter.GetType().Name;
                        columnObj["dataType"] = StiDataFiltersHelper.TypeToString(dataColumn != null ? dataColumn.Type : typeof(string));
                        columnObj["align"] = GetCellAlignment(meter);
                        columnObj["sortLabel"] = GetSortLabel(tableElement, meter.Key);
                        columnObj["filterLabel"] = GetFilterLabel(tableElement, meter.Key, dataColumn != null ? dataColumn.GetColumnPath() : string.Empty);
                        columnObj["cellForeColor"] = StiReportHelper.GetHtmlColor(GetCellForeColor(tableElement, meter as IStiTableColumn));
                        columnObj["cellForeColorInterlaced"] = StiReportHelper.GetHtmlColor(GetCellForeColor(tableElement, meter as IStiTableColumn, true));
                        columnObj["interaction"] = StiDashboardElementViewHelper.GetDashboardInteraction(meter);
                        columnObj["showTotalSummary"] = (meter as IStiTableColumn).ShowTotalSummary;
                        columnObj["summaryType"] = (meter as IStiTableColumn).SummaryType;
                        columnObj["summaryAlignment"] = (meter as IStiTableColumn).SummaryAlignment;
                        columnObj["headerAlignment"] = (meter as IStiTableColumn).HeaderAlignment;

                        if (meter is IStiDimensionColumn)
                        {
                            columnObj["showHyperlink"] = ((IStiDimensionColumn)meter).ShowHyperlink;
                            columnObj["hyperlinkPattern"] = ((IStiDimensionColumn)meter).HyperlinkPattern;
                        }

                        if (meter is IStiTableColumnSize)
                        {
                            var size = ((IStiTableColumnSize)meter).Size;
                            columnObj["sizeWidth"] = size.Width;
                            columnObj["sizeMaxWidth"] = size.MaxWidth;
                            columnObj["sizeMinWidth"] = size.MinWidth;
                            columnObj["sizeWordWrap"] = size.WordWrap;
                        }

                        headerCells.Add(columnObj);
                    }
                    columnIndex++;
                }

                resultData.Add(headerCells);

                var dataCells = StiInvokeMethodsHelper.InvokeStaticMethod(
                    "Stimulsoft.Dashboard.Export", "Tools.StiTableElementExportTool", "RenderCellsForViewer", new object[] { tableElement, ((StiComponent)tableElement).Width * scaleX }) as List<object>;

                if (dataCells != null)
                    resultData.AddRange(dataCells);
            }

            return resultData;
        }

        public static List<object> GetTableHiddenData(IStiTableElement tableElement)
        {
            var resultData = new List<object>();
            var dataTable = StiElementDataCache.GetOrCreate(tableElement);

            if (dataTable != null)
            {
                var headerCells = new List<object>();
                var dataCells = new List<object>();
                var meters = dataTable.Meters.AsEnumerable().Where(m => m is IStiTableColumn).ToList();
                var columnIndex = 0;

                foreach (var meter in meters)
                {
                    var columnVisible = Convert.ToBoolean(StiInvokeMethodsHelper.InvokeStaticMethod("Stimulsoft.Dashboard", "Components.Table.StiTableColumnVisibilityHelper", "GetVisible", new object[] { meter as IStiTableColumn, tableElement.Report as IStiReport }));
                    
                    if (!columnVisible)
                    {
                        var columnObj = new Hashtable();

                        columnObj["columnIndex"] = columnIndex;
                        columnObj["label"] = StiLabelHelper.GetLabel(meter);
                        columnObj["labelCorrect"] = StiNameValidator.CorrectName(StiLabelHelper.GetLabel(meter));
                        columnObj["key"] = meter.Key;

                        headerCells.Add(columnObj);

                        var rowIndex = 0;
                        foreach (var row in dataTable.Rows)
                        {
                            if (columnIndex < row.Length)
                            {
                                if (dataCells.Count < dataTable.Rows.Count)
                                    dataCells.Add(new List<object>());

                                var cellObject = new Hashtable();
                                cellObject["value"] = row[columnIndex]?.ToString();

                                ((List<object>)dataCells[rowIndex]).Add(cellObject);
                            }

                            rowIndex++;
                        }
                    }
                    columnIndex++;
                }

                resultData.Add(headerCells);
                resultData.AddRange(dataCells);
            }

            return resultData;
        }

        public static Hashtable GetTableSettings(IStiTableElement tableElement)
        {
            var settings = new Hashtable();
            var style = StiDashboardStyleHelper.GetTableStyle(tableElement);

            if (style != null)
            {
                settings["tableBorderColor"] = StiReportHelper.GetHtmlColor(style.LineColor);

                settings["headerForeColor"] = StiReportHelper.GetHtmlColor(GetHeaderForeColor(tableElement));
                settings["headerFont"] = StiDashboardElementViewHelper.GetFontJson(tableElement.HeaderFont);
                settings["headerBackground"] = StiReportHelper.GetHtmlColor(style.HeaderBackColor);
                settings["headerMouseOverBackground"] = StiReportHelper.GetHtmlColor(style.HotHeaderBackColor);
                settings["headerSelectedBackground"] = StiReportHelper.GetHtmlColor(style.HotHeaderBackColor);
                settings["headerHeight"] = StiElementConsts.Table.GetHeight(tableElement.HeaderFont);

                settings["cellBackColor"] = StiReportHelper.GetHtmlColor(style.CellBackColor);
                settings["cellBackColorInterlaced"] = StiReportHelper.GetHtmlColor(style.AlternatingCellBackColor);
                settings["cellSelectedBackColor"] = StiReportHelper.GetHtmlColor(style.SelectedCellBackColor);
                settings["cellSelectedForeColor"] = StiReportHelper.GetHtmlColor(style.SelectedCellForeColor);
                settings["cellHeight"] = StiElementConsts.Table.GetHeight(tableElement.Font);

                settings["footerForeColor"] = StiReportHelper.GetHtmlColor(GetFooterForeColor(tableElement));
                settings["footerFont"] = StiDashboardElementViewHelper.GetFontJson(tableElement.FooterFont);
                settings["footerBackground"] = StiReportHelper.GetHtmlColor(style.FooterBackColor);
                settings["footerHeight"] = StiElementConsts.Table.GetHeight(tableElement.FooterFont);
            }

            settings["sizeMode"] = tableElement.SizeMode;
            settings["frozenColumns"] = tableElement.FrozenColumns;

            return settings;
        }

        private static Color GetCellForeColor(IStiTableElement tableElement, IStiTableColumn column, bool isInterlacedForeColor = false)
        {            
            if (column != null && (column as IStiForeColor) != null && (column as IStiForeColor).ForeColor != Color.Transparent)
                return (column as IStiForeColor).ForeColor;

            if (tableElement != null && (tableElement as IStiForeColor) != null) 
            {
                var expForeColor = StiDashboardExpressionHelper.GetForeColor(tableElement, (tableElement as IStiForeColor).ForeColor);
                if (expForeColor != Color.Transparent)
                    return expForeColor;
            }

            var style = StiDashboardStyleHelper.GetTableStyle(tableElement);

            return (style != null ? (isInterlacedForeColor ? style.AlternatingCellForeColor : style.CellForeColor) : StiElementConsts.Table.Font.Color);
        }

        private static Color GetHeaderForeColor(IStiTableElement tableElement)
        {
            if (tableElement.HeaderForeColor != Color.Transparent)
                return tableElement.HeaderForeColor;

            return StiDashboardStyleHelper.GetTableStyle(tableElement).HeaderForeColor;
        }

        private static Color GetFooterForeColor(IStiTableElement tableElement)
        {
            if (tableElement.FooterForeColor != Color.Transparent)
                return tableElement.FooterForeColor;

            return StiDashboardStyleHelper.GetTableStyle(tableElement).FooterForeColor;
        }

        private static string GetCellAlignment(object element)
        {
            var elementAlign = element as IStiHorAlignment;

            if (elementAlign != null)
            {
                switch (elementAlign.HorAlignment)
                {
                    case StiHorAlignment.Left:
                        return "left";

                    case StiHorAlignment.Center:
                        return "center";

                    case StiHorAlignment.Right:
                        return "right";
                }
            }

            return "center";
        }

        private static Hashtable GetSortLabel(IStiTableElement tableElement, string meterKey)
        {
            var sortIndex = 1;

            foreach (var sortRule in tableElement.UserSorts)
            {
                if (sortRule.Key == meterKey) {
                    var sortLabel = new Hashtable();
                    sortLabel["direction"] = sortRule.Direction;
                    sortLabel["sortIndex"] = sortIndex;

                    return sortLabel;
                }
                sortIndex++;
            }

            return null;
        }

        private static Hashtable GetFilterLabel(IStiTableElement tableElement, string meterKey, string columnPath)
        {
            foreach (var filterRule in tableElement.UserFilters)
            {
                if (filterRule.Key != null ? filterRule.Key == meterKey : filterRule.Path == columnPath) {
                    var filterLabel = new Hashtable();
                    filterLabel["condition"] = filterRule.Condition;

                    return filterLabel;
                }
            }

            return null;
        }

        internal static Hashtable ChangeTableElementSelectColumns(StiReport report, StiRequestParams requestParams)
        {
            var elementName = requestParams.GetString("tableElementName");
            var hiddenColumns = requestParams.GetHashtable("tableElementHiddenColumns");

            var tableElement = report != null && elementName != null ? report.Pages.GetComponentByName(elementName) as IStiTableElement : null;
            if (tableElement != null)
            {
                var meters = tableElement.GetMeters();
                meters.ForEach(meter =>
                {
                    if (hiddenColumns[tableElement.Key + meter.Key] != null)
                    {
                        if (Convert.ToBoolean(hiddenColumns[tableElement.Key + meter.Key]))
                            ((IStiTableColumn)meter).Visibility = StiTableColumnVisibility.False;
                        else
                            ((IStiTableColumn)meter).Visibility = !string.IsNullOrEmpty(((IStiTableColumn)meter).VisibilityExpression) ? StiTableColumnVisibility.Expression : StiTableColumnVisibility.True;
                    }
                });

                var elements = ((IStiDashboard)tableElement.Page).GetElements(true).Where(e => e.IsEnabled).ToList();
                var totalFixedHeight = 0;
                List<StiRangeBand> bands = null;

                StiReportHelper.CalculatePositionForEachBand(requestParams, elements, tableElement.Page, out bands, out totalFixedHeight, tableElement as StiComponent);

                return StiReportHelper.GetElementAttributes(tableElement.Page, tableElement, false, requestParams, bands, totalFixedHeight);
            }

            return null;
        }
        #endregion
    }
}
