#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.IO;
using System.Reflection;

namespace Stimulsoft.Base.TaskScheduler
{
    public static class StiTaskSchedulerSettings
    {
        #region Fields
        private static MethodInfo runMethod;
        #endregion

        #region Properties
        public static Type WinFormsDbsHtmlEditor { get; set; }

        public static IStiTaskSchedulerBridge Current { get; set; }

        public static bool AllowHtmlEditor { get; private set; }
        #endregion

        #region Methods
        public static bool TryToLoadTaskShedulerAssembly()
        {
            try
            {
                var assemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Stimulsoft.TaskScheduler.dll");
                if (!File.Exists(assemblyPath))
                    return false;

                var assembly = Assembly.LoadFile(assemblyPath);
                if (assembly == null)
                    return false;

                var type = assembly.GetType("Stimulsoft.TaskScheduler.StiSchedulerBridge");
                if (type == null)
                    return false;

                var method = type.GetMethod("Init", BindingFlags.Public | BindingFlags.Static);
                method?.Invoke(null, null);
            }
            catch
            {
                return false;
            }

            try
            {
                WinFormsDbsHtmlEditor = Type.GetType("Stimulsoft.Dashboard.Design.Editors.StiHtmlEditorForm, Stimulsoft.Dashboard.Design");
                if (WinFormsDbsHtmlEditor != null)
                {
                    AllowHtmlEditor = true;
                    return true;
                }
            }
            catch
            {

            }

            return true;
        }

        public static void Run(string key)
        {
            try
            {
                if (runMethod == null)
                {
                    var type = Type.GetType("Stimulsoft.TaskScheduler.StiSchedulerRunner, Stimulsoft.TaskScheduler");
                    if (type == null) return;

                    var method = type.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
                    if (method == null) return;

                    runMethod = method;
                }

                runMethod.Invoke(null, new object[] { key });
            }
            catch
            {

            }
        }

        public static void RunLogViewer(string logFileName)
        {
            try
            {
                if (runMethod == null)
                {
                    var type = Type.GetType("Stimulsoft.TaskScheduler.StiSchedulerRunner, Stimulsoft.TaskScheduler");
                    if (type == null) return;

                    var method = type.GetMethod("RunLogViewer", BindingFlags.Public | BindingFlags.Static);
                    if (method == null) return;

                    runMethod = method;
                }

                runMethod.Invoke(null, new object[] { logFileName });
            }
            catch
            {

            }
        }        
        #endregion
    }
}