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
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiDatePickerElementViewHelper
    {
        #region Methods
        public static Hashtable GetAutoRangeValues(IStiDatePickerElement datePickerElement)
        {
            if (datePickerElement.SelectionMode == StiDateSelectionMode.AutoRange)
            {
                var dataTable = StiElementDataCache.GetOrCreate(datePickerElement);
                if (dataTable == null || dataTable.Rows.Count == 0) return null;

                var rangeValues = new Hashtable();

                var dates = dataTable.Rows.Select(r => r[0]).Where(d => d is DateTime);
                if (dates.Any())
                {
                    var selectionStart = dates.Where(d => d is DateTime).Cast<DateTime>().Min();
                    var selectionEnd = dates.Where(d => d is DateTime).Cast<DateTime>().Max();

                    rangeValues["selectionStart"] = selectionStart.ToString("MM'/'dd'/'yyyy");
                    rangeValues["selectionEnd"] = selectionEnd.ToString("MM'/'dd'/'yyyy");

                    return rangeValues;
                }
            }

            return null;
        }

        public static Hashtable GetVariableRangeValues(IStiDatePickerElement datePickerElement)
        {
            var variable = StiVariableExpressionHelper.GetVariableSpecifiedAsExpression(datePickerElement, datePickerElement.GetValueMeter()?.Expression) as StiVariable;
            if (variable != null)
            {
                var value = (datePickerElement.Report.Dictionary as IStiAppDictionary).GetVariableValueByName(variable.Name);

                if (variable.InitBy == StiVariableInitBy.Expression && !string.IsNullOrEmpty(value as string))
                    value = StiReportParser.Parse($"{{{value}}}", datePickerElement as StiComponent);

                if (value is DateTimeRange)
                {
                    var rangeValues = new Hashtable();
                    rangeValues["selectionStart"] = StiValueHelper.TryToDateTime(((DateTimeRange)value).FromObject).ToString("MM'/'dd'/'yyyy");
                    rangeValues["selectionEnd"] = StiValueHelper.TryToDateTime(((DateTimeRange)value).ToObject).ToString("MM'/'dd'/'yyyy");

                    return rangeValues;
                }
            }
            return null;
        }

        public static string GetVariableValue(IStiDatePickerElement datePickerElement)
        {
            var variable = StiVariableExpressionHelper.GetVariableSpecifiedAsExpression(datePickerElement, datePickerElement.GetValueMeter()?.Expression) as StiVariable;
            if (variable != null)
            {
                var value = (datePickerElement.Report.Dictionary as IStiAppDictionary).GetVariableValueByName(variable.Name);

                if (variable.InitBy == StiVariableInitBy.Expression && !string.IsNullOrEmpty(value as string))
                    value = StiReportParser.Parse($"{{{value}}}", datePickerElement as StiComponent);

                return StiValueHelper.TryToDateTime(value).ToString("MM'/'dd'/'yyyy");
            }
            return DateTime.Now.ToString("MM'/'dd'/'yyyy");
        }

        public static bool IsVariablePresent(IStiDatePickerElement datePickerElement)
        {
            var valueMeter = datePickerElement.GetValueMeter();
            return valueMeter != null && StiVariableExpressionHelper.IsVariableSpecifiedAsExpression(datePickerElement, valueMeter?.Expression);
        }

        public static bool IsRangeVariablePresent(IStiDatePickerElement datePickerElement)
        {
            var valueMeter = datePickerElement.GetValueMeter();
            return valueMeter != null && IsVariablePresent(datePickerElement) &&
                Range.IsRangeType((StiVariableExpressionHelper.GetVariableSpecifiedAsExpression(datePickerElement, valueMeter?.Expression) as StiVariable)?.Type);
        }

        public static Hashtable GetFormattedValues(StiReport report, StiRequestParams requestParams)
        {   
            var dateValues = requestParams.GetHashtable("dateValues");
            var datePickerElementName = requestParams.GetString("datePickerElementName");
            var datePickerElement = report != null && datePickerElementName != null ? report.Pages.GetComponentByName(datePickerElementName) as IStiDatePickerElement : null;
            
            if (dateValues != null && datePickerElement != null)
            {
                CultureInfo storedCulture = null;
                try
                {
                    var formattedValues = new Hashtable();

                    var culture = report.GetParsedCulture();
                    if (!string.IsNullOrWhiteSpace(culture))
                    {
                        storedCulture = Thread.CurrentThread.CurrentCulture;
                        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                    }
                    if (dateValues["value1"] != null)
                    {
                        formattedValues["value1"] = StiDashboardElementViewHelper.Format(datePickerElement, StiDataFiltersHelper.TryToDateTime((string)dateValues["value1"]));
                    }
                    if (dateValues["value2"] != null)
                    {
                        formattedValues["value2"] = StiDashboardElementViewHelper.Format(datePickerElement, StiDataFiltersHelper.TryToDateTime((string)dateValues["value2"]));
                    }
                    if (storedCulture != null)
                        Thread.CurrentThread.CurrentCulture = storedCulture;

                    return formattedValues;
                }
                catch
                {
                    return dateValues;
                }
            }
            return dateValues;
        }

        public static string GetColumnPath(IStiDatePickerElement datePickerElement)
        {
            var valueMeter = datePickerElement.GetValueMeter();

            return valueMeter != null ? valueMeter.Expression : null;
        }

        public static Hashtable GetSettings(IStiDatePickerElement datePickerElement)
        {
            var settings = StiDashboardElementViewHelper.GetControlElementSettings(datePickerElement);
            settings["itemHeight"] = StiElementConsts.ComboBox.ItemHeight;

            return settings;
        }
        #endregion
    }
}
