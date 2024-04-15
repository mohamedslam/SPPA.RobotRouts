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

using Stimulsoft.Base;
using Stimulsoft.Report.Dashboard;

namespace Stimulsoft.Report
{
    public static class StiVariableExpressionHelper
    {
        public static IStiAppVariable GetVariableSpecifiedAsExpression(IStiElement element, string expression)
        {
            expression = GetSimpleName(expression);

            return element?.GetReport()?.GetDictionary()?.GetVariableByName(expression);
        }

        public static bool IsVariableSpecifiedAsExpression(IStiElement element, string expression)
        {
            return GetVariableSpecifiedAsExpression(element, expression) != null;
        }

        public static string ExtractVariableName(string exp)
        {
            if (string.IsNullOrWhiteSpace(exp))
                return exp;

            exp = exp.Trim();

            if (exp.StartsWith("["))
                exp = exp.Substring(1);

            if (exp.EndsWith("]"))
                exp = exp.Substring(0, exp.Length - 1);

            return exp.Trim();
        }

        public static string GetSimpleName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            name = name.ToLowerInvariant().Trim();
            name = ExtractVariableName(name);

            return name.Trim();
        }
    }
}