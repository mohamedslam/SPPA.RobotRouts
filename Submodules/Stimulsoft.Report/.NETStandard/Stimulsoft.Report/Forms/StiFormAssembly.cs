#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Form											}
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
using System;
using System.Reflection;

namespace Stimulsoft.Report.Design.Forms
{
    internal static class StiFormAssembly
    {
        #region Fields
        private static object lockObject = new object();
        private static bool isInited;

        private const string assemblyName = "Stimulsoft.Form.dll";
        private const string formInteropProviderTypeName = "Stimulsoft.Form.DesignerInterop.StiFormInteropProvider";
        #endregion

        #region Properties
        internal static bool IsAssemblyLoaded => Assembly != null;

        private static Assembly assembly;
        internal static Assembly Assembly
        {
            get
            {
                LoadAssembly();

                return assembly;
            }
            set
            {
                assembly = value;
            }
        }
        #endregion

        #region Methods
        internal static void LoadAssembly()
        {
            if (StiOptions.Engine.SkipLoadingFormAssembly) return;

            if (isInited) return;

            lock (lockObject)
            {
                try
                {
                    Assembly = StiAssemblyFinder.GetAssembly(assemblyName);
                }
                catch
                {
                }
                isInited = true;
            }
        }

        internal static object LoadForm(string content)
        {
            try
            {
                LoadAssembly();

                if (!IsAssemblyLoaded)
                    return null;

                var interopProviderType = Assembly.GetType(formInteropProviderTypeName);
                if (interopProviderType == null)
                    return null;

                var method = interopProviderType.GetMethod("LoadForm", new[] { typeof(string) });
                if (method == null)
                    return null;

                return method.Invoke(null, new object[] { content });
            }
            catch
            {
            }
            return null;
        }

        internal static string SaveForm(object form)
        {
            try
            {
                LoadAssembly();
                if (!IsAssemblyLoaded)
                    return null;

                var interopProviderType = Assembly.GetType(formInteropProviderTypeName);
                if (interopProviderType == null)
                    return null;

                var method = interopProviderType.GetMethod("SaveFormToString", new[] { typeof(object) });
                if (method == null)
                    return null;

                return method.Invoke(null, new object[] { form }) as string;
            }
            catch
            {
            }
            return null;
        }

        internal static object NewStiFormElement()
        {
            try
            {
                LoadAssembly();
                if (!IsAssemblyLoaded)
                    return null;

                var interopProviderType = Assembly.GetType(formInteropProviderTypeName);
                if (interopProviderType == null)
                    return null;

                var method = interopProviderType.GetMethod("GetEmptyForm", new Type[] { });
                if (method == null)
                    return null;

                return method.Invoke(null, null);
            }
            catch
            {
            }
            return null;
        }
        #endregion
    }
}
