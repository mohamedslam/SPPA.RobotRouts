#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report.Engine
{
    public partial class StiParser
    {
        #region Evaluate the value of the property.
        private object call_property(object name, ArrayList argsList)
        {
            object baseValue = argsList[0];

            if (name is string)
            {
                if (baseValue == null) return null;
                try
                {
                    PropertyInfo property = baseValue.GetType().GetProperty((string)name);
                    if (property != null)
                    {
                        object result = property.GetValue(baseValue, new object[0]);
                        return result;
                    }
                }
                catch
                {
                }
                return null;
            }

            if (baseValue is DateTime)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Year: return ((DateTime)baseValue).Year;
                    case StiPropertyType.Month: return ((DateTime)baseValue).Month;
                    case StiPropertyType.Day: return ((DateTime)baseValue).Day;
                    case StiPropertyType.Hour: return ((DateTime)baseValue).Hour;
                    case StiPropertyType.Minute: return ((DateTime)baseValue).Minute;
                    case StiPropertyType.Second: return ((DateTime)baseValue).Second;
                    case StiPropertyType.Date: return ((DateTime)baseValue).Date;
                    case StiPropertyType.DayOfWeek: return ((DateTime)baseValue).DayOfWeek;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue is TimeSpan)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Days: return ((TimeSpan)baseValue).Days;
                    case StiPropertyType.Hours: return ((TimeSpan)baseValue).Hours;
                    case StiPropertyType.Milliseconds: return ((TimeSpan)baseValue).Milliseconds;
                    case StiPropertyType.Minutes: return ((TimeSpan)baseValue).Minutes;
                    case StiPropertyType.Seconds: return ((TimeSpan)baseValue).Seconds;
                    case StiPropertyType.Ticks: return ((TimeSpan)baseValue).Ticks;
                    case StiPropertyType.TotalDays: return ((TimeSpan)baseValue).TotalDays;
                    case StiPropertyType.TotalHours: return ((TimeSpan)baseValue).TotalHours;
                    case StiPropertyType.TotalMinutes: return ((TimeSpan)baseValue).TotalMinutes;
                    case StiPropertyType.TotalSeconds: return ((TimeSpan)baseValue).TotalSeconds;
                    case StiPropertyType.TotalMilliseconds: return ((TimeSpan)baseValue).TotalMilliseconds;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue is String)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Length: return Convert.ToString(baseValue).Length;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue == null)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Length: return 0;
                }
            }

            if (baseValue is Range)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.From: return (baseValue as Range).FromObject;
                    case StiPropertyType.To: return (baseValue as Range).ToObject;
                    case StiPropertyType.FromDate: return (baseValue as DateTimeRange).FromDate;
                    case StiPropertyType.ToDate: return (baseValue as DateTimeRange).ToDate;
                    case StiPropertyType.FromTime: return (baseValue as TimeSpanRange).FromTime;
                    case StiPropertyType.ToTime: return (baseValue as TimeSpanRange).ToTime;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue is IStiList)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Count: return (baseValue as IStiList).Count;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue is StiDataBand)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Count: return (baseValue as StiDataBand).Count;
                    case StiPropertyType.SelectedLine: return (baseValue as StiDataBand).SelectedLine;
                    case StiPropertyType.Position: return (baseValue as StiDataBand).Position;
                    case StiPropertyType.Line: return (baseValue as StiDataBand).Line;
                    case StiPropertyType.Enabled: return (baseValue as StiDataBand).Enabled;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue is IStiButtonElement)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Checked: return (baseValue as IStiButtonElement).Checked;
                }
            }

            if (baseValue is IStiFilterElement filterElement)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.SelectedValue: return filterElement.SelectedValue;
                    case StiPropertyType.SelectedLabel: return filterElement.SelectedLabel;
                    case StiPropertyType.SelectedIndex: return filterElement.SelectedIndex;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue is StiDataSource)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Count: return (baseValue as StiDataSource).Count;
                    case StiPropertyType.Position: return (baseValue as StiDataSource).Position;
                    case StiPropertyType.Rows: return (baseValue as StiDataSource).Rows;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue is StiBusinessObject)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.BusinessObjectValue: return (baseValue as StiBusinessObject).BusinessObjectValue;
                    case StiPropertyType.Position: return (baseValue as StiBusinessObject).Position;
                }
                ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, GetTypeName(argsList[0]), Enum.GetName(typeof(StiPropertyType), name));
            }

            if (baseValue is StiPage)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Skip: return (baseValue as StiPage).Skip;
                }
            }

            if (baseValue is StiComponent)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Name: return (baseValue as StiComponent).Name;
                    case StiPropertyType.TagValue: return (baseValue as StiComponent).TagValue;
                    case StiPropertyType.Enabled: return (baseValue as StiComponent).Enabled;
                }
            }

            if (baseValue is StiText)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Text: return (baseValue as StiText).Text?.Value;
                }
            }

            if (baseValue is Dialogs.StiComboBoxControl)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.SelectedIndex: return (baseValue as Dialogs.StiComboBoxControl).SelectedIndex;
                    case StiPropertyType.SelectedItem: return (baseValue as Dialogs.StiComboBoxControl).SelectedItem;
                    case StiPropertyType.SelectedValue: return (baseValue as Dialogs.StiComboBoxControl).SelectedValue;
                    case StiPropertyType.Text: return (baseValue as Dialogs.StiComboBoxControl).Text;
                }
            }

            if (baseValue is Dialogs.StiDateTimePickerControl)
            {
                switch ((StiPropertyType)name)
                {
                    case StiPropertyType.Value: return (baseValue as Dialogs.StiDateTimePickerControl).Value;
                }
            }

            return null;
        }
        #endregion    
    }
}
