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

using System.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Extensions;

namespace Stimulsoft.Report.Dictionary
{
    public static class StiDataColumnExt
    {
        #region Methods
        public static StiDataSource GetDataSourceByColumnKey(StiReport report, string columnKey)
        {
            if (StiKeyHelper.IsEmptyKey(columnKey)) return null;
            foreach (StiDataSource dataSource in report.Dictionary.DataSources)
            {
                var column = dataSource.Columns.ToList().FirstOrDefault(c => c.Key == columnKey);
                if (column != null)
                    return dataSource;
            }
            return null;
        }

        public static StiDataColumn GetColumnByKey(StiReport report, string columnKey)
        {
            if (StiKeyHelper.IsEmptyKey(columnKey)) return null;
            foreach (StiDataSource dataSource in report.Dictionary.DataSources)
            {
                var column = dataSource.Columns.ToList().FirstOrDefault(c => c.Key == columnKey);
                if (column != null)
                    return column;
            }
            return null;
        }

        public static bool IsNumericType(this StiDataColumn column)
        {
            return column != null && column.Type != null && column.Type.IsNumericType();
        }

        public static bool IsDateType(this StiDataColumn column)
        {
            return column != null && column.Type != null && column.Type.IsDateType();
        }

        public static bool IsIntegerType(this StiDataColumn column)
        {
            return column != null && column.Type != null && column.Type.IsIntegerType();
        }

        public static bool IsArray(this StiDataColumn column)
        {            
            return column != null && column.Type != null && column.Type.IsArray;
        }
        #endregion
    }
}