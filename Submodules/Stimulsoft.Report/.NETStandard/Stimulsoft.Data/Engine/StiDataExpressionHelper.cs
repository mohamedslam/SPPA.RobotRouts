#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Data.Functions;
using Stimulsoft.Data.Helpers;

namespace Stimulsoft.Data.Engine
{
    public class StiDataExpressionHelper
    {
        #region Methods
        public static IStiAppDataColumn GetDataColumnFromExpression(IStiQueryObject query, string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return null;

            expression = StiExpressionHelper.RemoveFunction(expression);
            var args = StiExpressionHelper.GetArguments(expression);
            if (args == null || args.Count != 1)
                return null;

            var arg = args.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(arg))
                return null;

            arg = Funcs.ToDataName(arg);

            lock (query)
            {
                var dataSources = query?.GetDataSources(new[] { arg }).ToList();
                if (dataSources == null || !dataSources.Any())
                    return null;

                var dataSource = dataSources.First();
                return dataSource.FetchColumns().FirstOrDefault(c => Funcs.IsDataEqual(dataSource, c.GetName(), arg));
            }
        }

        public static bool IsDateDataColumnInExpression(IStiQueryObject query, string expression)
        {
            var column = GetDataColumnFromExpression(query, expression);
            return column?.GetDataType() != null && column.GetDataType().IsDateType();
        }

        public static bool IsNumericDataColumnInExpression(IStiQueryObject query, string expression)
        {
            var column = GetDataColumnFromExpression(query, expression);
            return column?.GetDataType() != null && column.GetDataType().IsNumericType();
        }
        #endregion
    }
}