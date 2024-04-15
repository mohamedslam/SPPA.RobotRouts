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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Stimulsoft.Base.Drawing.StiFontReader;

namespace Stimulsoft.Report.Dashboard
{
    public static class StiReportParser
    {
        #region Fields
        private static Dictionary<string, object> cache = new Dictionary<string, object>();
        private static Dictionary<string, object> wrongCache = new Dictionary<string, object>();
        public static object LockObject = new object();
        #endregion

        #region Methods
        public static string Parse(string expression, IStiReport report, bool allowCache = true,
            Dictionary<string, object> constants = null, bool allowDataLoading = true, bool onlyExpression = false)
        {
            return Parse(expression, report.FetchPages().FirstOrDefault(), allowCache, constants, allowDataLoading, onlyExpression);
        }

        public static string Parse(string expression, IStiReportComponent component, bool allowCache = true,
            Dictionary<string, object> constants = null, bool allowDataLoading = true, bool onlyExpression = false)
        {
            var value = ParseObject(expression, component, allowCache, constants, allowDataLoading, onlyExpression);
            return value != null ? value.ToString() : "";
        }

        public static object ParseObject(string expression, IStiReportComponent component, bool allowCache = true,
            Dictionary<string, object> constants = null, bool allowDataLoading = true, bool onlyExpression = false)
        {
            if (component == null)
                return expression;

            lock (LockObject)
            {
                if (onlyExpression && expression != null)
                    expression = PrepareExpression(expression);

                //Expression contains '{'. If not than that text doesn't contain expression.
                if (expression != null && !expression.Contains("{"))
                    return expression;

                var result = GetFromWrongCache(expression, component);
                if (result != null)
                    return result;

                result = allowCache && constants != null ? GetFromCache(expression, component) : null;
                if (result != null)
                    return result;

                result = ParseOrDefault(expression, component, constants, allowDataLoading);

                if ((result as string) == expression && expression != null && expression.Contains("{"))
                {
                    AddToWrongCache(expression, result, component);
                    return result;
                }

                if (allowCache && constants != null)
                    AddToCache(expression, result, component);

                return result;
            }
        }

        private static object ParseOrDefault(string expression, IStiReportComponent component,
            Dictionary<string, object> constants = null, bool allowDataLoading = true)
        {
            if (!TryParse(expression, out object value, component, constants, allowDataLoading))
                value = expression;

            return value;
        }

        private static bool TryParse(string expression, out object result, IStiReportComponent component, 
            Dictionary<string, object> constants = null, bool allowDataLoading = true)
        {
            lock (LockObject)
            {
                try
                {
                    if (expression == null || !expression.Contains("{"))
                    {
                        result = expression;
                        return true;
                    }

                    result = StiParser.ParseTextValue(expression, component as StiComponent, new StiParserParameters
                    {
                        SyntaxCaseSensitive = false,
                        Constants = PrepareConstants(constants),
                        GetDataFieldValue = allowDataLoading ? GetDataFieldValueProcessor : (StiParser.StiParserGetDataFieldValueDelegate)null
                    });
                                        
                    return true;
                }
                catch (global::System.Exception ex)
                {
                    #region Add RenderingMessage
                    var comp = component as StiComponent;
                    if (comp?.Report != null && comp.Report.IsRendering)
                    {
                        //find property name; possible incorrect if multiple properties have the same expression.
                        string propertyName = null;
                        for (int index = 0; index < comp.Expressions.Count; index++)
                        {
                            if (expression == "{" + comp.Expressions[index].Expression + "}")
                            {
                                propertyName = comp.Expressions[index].Name;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(propertyName))
                        {
                            comp.Report.WriteToReportRenderingMessages($"Expression in the {propertyName} property of the {comp.Name} component. " + ex.Message);
                        }
                        else
                        {
                            comp.Report.WriteToReportRenderingMessages($"PropertyExpression of the {comp.Name} component. " + ex.Message);
                        }
                    }
                    #endregion

                    result = null;
                    return false;
                }
            }
        }

        private static Hashtable PrepareConstants(Dictionary<string, object> constants)
        {
            Hashtable constantsHash = null;
            if (constants != null)
            {
                constantsHash = new Hashtable();
                constants.Keys
                    .Where(k => k != null)
                    .ToList()
                    .ForEach(k => constantsHash[k] = constants[k]);
            }

            return constantsHash;
        }

        public static void GetDataFieldValueProcessor(object sender, StiParser.StiParserGetDataFieldValueEventArgs e)
        {
            var report = sender as StiReport;

            var dataTable = StiDataPicker.Fetch(report, e.DataSourceName);
            if (dataTable == null)return;

            var dataName = $"{e.DataSourceName}.{e.DataColumnName}";
            var dataRow = dataTable.Rows.Cast<DataRow>().FirstOrDefault();
            if (dataRow == null) return;

            var dataColumn = dataTable.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName == dataName || c.ColumnName.Replace(" ", "") == dataName.Replace(" ", ""))
                .FirstOrDefault();

            if (dataColumn == null) return;

            var dataIndex = dataTable.Columns.IndexOf(dataColumn);
            if (!(dataIndex >= 0 && dataIndex < dataRow.ItemArray.Length)) return;
            
            e.Value = dataRow[dataName];
            e.Processed = true;
        }

        private static string PrepareExpression(string expression)
        {
            expression = expression.Trim();

            if (!expression.Contains("{"))
                expression = "{" + expression;

            if (!expression.Contains("}"))
                expression += "}";

            return expression;
        }
        #endregion

        #region Methods.Cache
        private static string GetCacheKey(string expression, IStiReportComponent component)
        {
            var appKey = StiAppKey.GetOrGeneratedKey(component) ?? string.Empty;
            var expressionKey = expression ?? string.Empty;

            return $"{appKey}.{expressionKey}";
        }
        
        internal static void AddToCache(string expression, object result, IStiReportComponent component)
        {
            lock (cache)
            {
                if (result == null)
                    result = string.Empty;

                var key = GetCacheKey(expression, component);

                cache[key] = result;
            }
        }

        internal static void AddToWrongCache(string expression, object result, IStiReportComponent component)
        {
            lock (cache)
            {
                if (result == null)
                    result = string.Empty;

                var key = GetCacheKey(expression, component);

                wrongCache[key] = result;
            }
        }

        internal static object GetFromCache(string expression, IStiReportComponent component)
        {
            lock (cache)
            {
                if (string.IsNullOrEmpty(expression))
                    return string.Empty;

                if (string.IsNullOrWhiteSpace(expression))
                    return expression;

                var key = GetCacheKey(expression, component);

                return cache.ContainsKey(key) ? cache[key] : null;
            }
        }

        internal static object GetFromWrongCache(string expression, IStiReportComponent component)
        {
            lock (wrongCache)
            {
                if (string.IsNullOrEmpty(expression))
                    return string.Empty;

                if (string.IsNullOrWhiteSpace(expression))
                    return expression;

                var key = GetCacheKey(expression, component);

                return wrongCache.ContainsKey(key) ? wrongCache[key] : null;
            }
        }

        public static void CleanCache(string reportKey)
        {
            lock (cache)
            {
                if (reportKey == null)
                {
                    cache.Clear();
                }
                else
                {
                    cache.Keys
                        .Where(k => k.StartsWith(reportKey))
                        .ToList()
                        .ForEach(k => cache.Remove(k));
                }
            }

            lock (wrongCache)
            {
                if (reportKey == null)
                {
                    wrongCache.Clear();
                }
                else
                {
                    wrongCache.Keys
                        .Where(k => k.StartsWith(reportKey))
                        .ToList()
                        .ForEach(k => wrongCache.Remove(k));
                }
            }
        }
        #endregion
    }
}