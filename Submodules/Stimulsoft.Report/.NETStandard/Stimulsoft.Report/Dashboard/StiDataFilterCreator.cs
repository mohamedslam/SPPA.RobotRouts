#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System;

namespace Stimulsoft.Report.Dashboard
{
    public static class StiDataFilterCreator
    {
        public static StiDataFilterRule CreateEqualBasedOnValue(object value, string columnName, StiComponent component)
        {
            var dataColumn = StiDataColumn.GetDataColumnFromColumnName(component.Report.Dictionary, columnName);
            if (dataColumn == null && (columnName.Contains("[") || columnName.Contains("]")))
            {
                columnName = columnName.Replace("[", "").Replace("]", "");
                dataColumn = StiDataColumn.GetDataColumnFromColumnName(component.Report.Dictionary, columnName);
            }

            var type = dataColumn?.Type;

            if (type == typeof(string) && ((value is string && string.IsNullOrEmpty(value as string)) || value == null || value == DBNull.Value))
                return new StiDataFilterRule(columnName, StiDataFilterCondition.IsBlankOrNull, value?.ToString());

            else if (value == null || value == DBNull.Value)
                return new StiDataFilterRule(columnName, StiDataFilterCondition.IsNull);

            else
                return new StiDataFilterRule(columnName, StiDataFilterCondition.EqualTo, value?.ToString());
        }
    }
}