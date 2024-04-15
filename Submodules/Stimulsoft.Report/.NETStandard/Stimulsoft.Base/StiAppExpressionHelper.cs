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
using System.ComponentModel;

namespace Stimulsoft.Base
{
    public static class StiAppExpressionHelper
    {
        public static bool IsExpressionSpecified(object component, string propName)
        {
            var prop = GetExpression(component, propName);

            return !string.IsNullOrWhiteSpace(prop?.Expression);
        }

        public static StiAppExpression GetExpression(object component, string propName)
        {
            var appExpessions = component as IStiAppExpressionCollection;
            if (appExpessions?.Expressions == null)
                return null;

            return appExpessions?.Expressions[propName];
        }

        public static string GetExpressionValue(object component, string propName)
        {
            return GetExpression(component, propName)?.Expression;
        }

        public static void SetExpression(object component, string propName, string expression)
        {
            var appExpessions = component as IStiAppExpressionCollection;
            if (appExpessions == null) return;

            if (appExpessions.Expressions == null)
                appExpessions.Expressions = new StiAppExpressionCollection();

            appExpessions.Expressions[propName] = new StiAppExpression(propName, expression);
        }

        public static void RemoveExpression(object component, string propName)
        {
            var appExpessions = component as IStiAppExpressionCollection;
            if (appExpessions == null) return;

            appExpessions.Expressions?.Remove(propName);
        }

        /// <summary>
        /// Returns an expression if specified from the context object.
        /// Returns null if the expression is not specified.
        /// </summary>
        public static string GetExpressionFromInstance(ITypeDescriptorContext context)
        {
            return GetExpressionFromInstance(context?.Instance, context?.PropertyDescriptor?.Name);
        }

        /// <summary>
        /// Returns and expression if specified from the specified instance and the property name. 
        /// Returns null if the expression is not specified.
        /// </summary>
        public static string GetExpressionFromInstance(object instance, string propertyName)
        {
            if (instance == null || string.IsNullOrEmpty(propertyName))
                return null;

            var expressions = instance as IStiAppExpressionCollection;
            if (expressions?.Expressions == null || expressions.Expressions[propertyName] == null)
                return null;

            var prop = expressions.Expressions[propertyName];
            if (string.IsNullOrWhiteSpace(prop?.Expression))
                return null;

            return prop.Expression;
        }
    }
}