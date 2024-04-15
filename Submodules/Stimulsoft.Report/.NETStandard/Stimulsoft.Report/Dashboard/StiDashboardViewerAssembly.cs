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
using Stimulsoft.Base;
using System.Reflection;
using Stimulsoft.Report.Viewer;
using System.Collections.Generic;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dashboard
{
    internal static class StiDashboardViewerAssembly
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

            if (isInited) return;

            lock (lockObject)
            { 
                try
                {
                    Assembly = StiAssemblyFinder.GetAssembly($"Stimulsoft.Dashboard.Viewer, {StiVersion.VersionInfo}");
                }
                catch
                {
                }
                isInited = true;
            }
        }

        internal static void Show(StiReport report)
        {
            LoadAssembly();
            if (!IsAssemblyLoaded)
                throw new Exception("Assembly 'Stimulsoft.Dashboard.Viewer' is not found!");

            var type = Assembly.GetType("Stimulsoft.Dashboard.Viewer.StiDashboardViewerForm");
            if (type == null)
                throw new Exception("Type 'Stimulsoft.Dashboard.Viewer.StiDashboardViewerForm' is not found!");

            var viewer = StiActivator.CreateObject(type) as IStiViewerForm;
            viewer.Report = report;
            viewer.ShowViewer();
        }

        internal static UserControl GetViewerControl(StiReport report, int startPage = -1)
        {
            LoadAssembly();

            if (!IsAssemblyLoaded)
                throw new Exception("Assembly 'Stimulsoft.Dashboard.Viewer' is not found!");

            var typeControl = Assembly.GetType("Stimulsoft.Dashboard.Viewer.StiDashboardViewerControl");
            if (typeControl == null)
                throw new Exception("Type 'Stimulsoft.Dashboard.Viewer.StiDashboardViewerControl' is not found!");

            var type = Assembly.GetType("Stimulsoft.Dashboard.Viewer.StiDashboardViewerHelper");
            if (type == null)
                throw new Exception("Type 'Stimulsoft.Dashboard.Viewer.StiDashboardViewerHelper' is not found!");

            return type.GetMethod("CreateViewerControl")?.Invoke(null, new object[] { report, startPage }) as UserControl;
        }
        #endregion
    }
}
