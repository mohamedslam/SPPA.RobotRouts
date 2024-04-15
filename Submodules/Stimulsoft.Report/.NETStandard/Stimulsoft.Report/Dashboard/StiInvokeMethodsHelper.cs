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

using System;
using System.Reflection;

namespace Stimulsoft.Report.Dashboard
{
    internal class StiInvokeMethodsHelper
    {
        public static object InvokeStaticMethod(string assemblyName, string className, string methodName)
        {
            return InvokeStaticMethod(assemblyName, className, methodName, null, null);
        }

        public static object InvokeStaticMethod(string assemblyName, string className, string methodName, object[] parameters)
        {
            return InvokeStaticMethod(assemblyName, className, methodName, parameters, null);
        }

        public static object InvokeStaticMethod(string assemblyName, string className, string methodName, object[] parameters, Type[] parametersTypes)
        {
            var type = Type.GetType($"{assemblyName}.{className}, {assemblyName}, {StiVersion.VersionInfo}");
            if (type == null)
                return null;

            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            var method = parametersTypes != null
                ? type.GetMethod(methodName, bindingFlags, null, parametersTypes, null)
                : type.GetMethod(methodName, bindingFlags);

            if (method != null)
                return method.Invoke(null, parameters);

            return null;
        }

        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            if (obj == null) return;

            var property = obj.GetType().GetProperty(propertyName);
            property?.SetValue(obj, value, null);
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null) return null;
            var property = obj.GetType().GetProperty(propertyName);

            return property != null ? property.GetValue(obj, null) : null;
        }

        public static Type GetType(string assemblyName, string className)
        {
            return Type.GetType($"{assemblyName}.{className}, {assemblyName}, {StiVersion.VersionInfo}");
        }
    }
}
