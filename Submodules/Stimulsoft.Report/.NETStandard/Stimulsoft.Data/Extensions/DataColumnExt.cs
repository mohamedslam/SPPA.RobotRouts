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

using System.Data;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Helpers;

namespace Stimulsoft.Data.Extensions
{
    public static class DataColumnExt
    {
        #region Methods
        public static bool IsNumericType(this DataColumn column)
        {
            return column?.DataType != null && column.DataType.IsNumericType();
        }

        public static bool IsDateType(this DataColumn column)
        {
            return column?.DataType != null && column.DataType.IsDateType();
        }

        public static bool IsIntegerType(this DataColumn column)
        {
            return column?.DataType != null && column.DataType.IsIntegerType();
        }

        public static bool IsMoneyName(this DataColumn column)
        {
            return StiMoneyNameHelper.IsMoneyName(column?.ColumnName);
        }

        public static string GetHumanReadableName(this DataColumn column)
        {
            return StiHumanReadableHelper.GetHumanReadableName(column.ColumnName);
        }
        #endregion
    }
}