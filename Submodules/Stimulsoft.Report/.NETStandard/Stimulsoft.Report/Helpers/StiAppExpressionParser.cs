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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using System;
using System.Drawing;
using System.Linq;

namespace Stimulsoft.Report.Helpers
{
    public static class StiAppExpressionParser
    {
        public static void ProcessExpressions(StiComponent component, bool allowDataLoading = false)
        {
            if (component == null)return;

            var appExpessions = component as IStiAppExpressionCollection;
            if (appExpessions?.Expressions == null || appExpessions.Expressions.Count == 0) return;

            var componentType = component.GetType();

            foreach (var expression in appExpessions.Expressions.ToList().Where(e => !e.IsEmpty))
            {
                try
                {
                    var property = componentType.GetProperty(expression.Name);
                    if (property == null) continue;
                    
                    if (property.PropertyType == typeof(bool))                    
                        property.SetValue(component, ParseBoolExpression(component, expression.Name, allowDataLoading));                    

                    if (property.PropertyType == typeof(string))
                        property.SetValue(component, ParseStringExpression(component, expression.Name, allowDataLoading));

                    if (property.PropertyType == typeof(Color))
                        property.SetValue(component, ParseColorExpression(component, expression.Name, allowDataLoading));

                    if (property.PropertyType == typeof(StiBrush))
                        property.SetValue(component, ParseBrushExpression(component, expression.Name, allowDataLoading));

                    if (property.PropertyType.IsEnum)
                    {
                        var value = ParseEnumExpression(component, expression.Name, property.PropertyType, allowDataLoading);

                        if (value != null)
                            property.SetValue(component, value);
                    }
                }
                catch
                {
                }
            }
        }
                
        public static bool ParseBoolExpression(StiComponent component, string propName, bool allowDataLoading = false)
        {
            var value = ParseExpression(component, StiAppExpressionHelper.GetExpression(component, propName), allowDataLoading);
            return StiValueHelper.TryToBool(value);
        }

        public static object ParseEnumExpression(StiComponent component, string propName, Type enumType, bool allowDataLoading = false)
        {
            var value = ParseExpression(component, StiAppExpressionHelper.GetExpression(component, propName), allowDataLoading);

            try
            {
                var strValue = value?.ToString();
                return Enum.Parse(enumType, strValue.Trim(), true);
            }
            catch
            {
                return null;
            }
        }

        public static string ParseStringExpression(StiComponent component, string propName, bool allowDataLoading = false)
        {
            var value = ParseExpression(component, StiAppExpressionHelper.GetExpression(component, propName), allowDataLoading);

            var strValue = value?.ToString();
            return strValue ?? "";
        }

        public static Color ParseColorExpression(StiComponent component, string propName, bool allowDataLoading = false)
        {
            var value = ParseExpression(component, StiAppExpressionHelper.GetExpression(component, propName), allowDataLoading);

            if (value is Color color)
                return color;

            var strValue = value?.ToString();
            return string.IsNullOrWhiteSpace(strValue) ? Color.Transparent : StiColor.Get(strValue);
        }

        public static StiBrush ParseBrushExpression(StiComponent component, string propName, bool allowDataLoading = false)
        {
            var value = ParseExpression(component, StiAppExpressionHelper.GetExpression(component, propName), allowDataLoading);

            if (value is Color color)
                return new StiSolidBrush(color);

            if (value is StiBrush brush)
                return brush;

            var strValue = value?.ToString();
            return new StiSolidBrush(string.IsNullOrWhiteSpace(strValue) ? Color.Transparent : StiColor.Get(strValue));
        }

        private static object ParseExpression(StiComponent component, StiAppExpression expression, bool allowDataLoading = false)
        {
            if (string.IsNullOrWhiteSpace(expression?.Expression))
                return null;

            return StiReportParser.ParseObject(expression.Expression, component, false, null, allowDataLoading, true);
        }
    }
}