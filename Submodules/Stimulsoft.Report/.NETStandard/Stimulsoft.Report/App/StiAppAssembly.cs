#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Apps 												}
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
using System.Reflection;
using Stimulsoft.Report.Components;
using System.Collections.Generic;

namespace Stimulsoft.Report.App
{
    internal static class StiAppAssembly
    {
        #region Fields
        private static object lockObject = new object();
        private static bool isInited;
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
#if DEBUG && !SBI//ENABLEAPPS
            if (StiOptions.Engine.SkipLoadingAppAssembly) return;

            if (isInited) return;

            lock (lockObject)
            { 
                try
                {
                    Assembly = StiAssemblyFinder.GetAssembly($"Stimulsoft.App, {StiVersion.VersionInfo}");
                }
                catch
                {
                }
                isInited = true;
            }
#endif
        }

        internal static List<StiComponent> LoadAppComponents()
        {
            try
            {
                LoadAssembly();

                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.App.Options.StiAppComponentsLoader");
                if (type == null)
                    return null;

                var methodFetchAll = type.GetMethod("FetchAll");
                if (methodFetchAll == null)
                    return null;

                return methodFetchAll.Invoke(null, new object[0]) as List<StiComponent>;
            }
            catch
            {
            }
            return null;
        }
        #endregion
    }
}