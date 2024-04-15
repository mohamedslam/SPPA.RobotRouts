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

using Stimulsoft.Base;
using Stimulsoft.Report.Design.Forms.Toolbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Stimulsoft.Report.Design.Forms
{
    internal static class StiFormWpfAssembly
    {
        #region Fields
        private static object lockObject = new object();
        private static bool isInited;
        private const string AssemblyName = "Stimulsoft.Form.Wpf";
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
#if DEBUG
            if (isInited) return;

            lock (lockObject)
            {
                try
                {
                    Assembly = StiAssemblyFinder.GetAssembly(AssemblyName);
                }
                catch
                {
                }
                isInited = true;
            }
#endif
        }

        private static object CreateObject(string typeName, object[] arg)
        {
            var type = Type.GetType($"{typeName}, {AssemblyName}");
            return StiActivator.CreateObject(type, arg);
        }

        internal static string GetTypeName(string name)
        {
            var type = Assembly.GetType("Stimulsoft.Form.Wpf.Designer.StiNameTypeProvider");

            return type.GetProperty(name).GetValue(null, null).ToString();
        }
        #endregion

        #region Methods.Toolbox
        internal static object GetStiToolbox()
        {
            try
            {
                var typeName = GetTypeName("StiToolbox");
                return CreateObject(typeName, new object[] { });
            }
            catch
            {
                return null;
            }
        }

        public static List<StiFormsToolboxItemRefl> GetToolboxItems()
        {
            try
            {
                var toolbox = GetStiToolbox();

                var type = toolbox.GetType();
                var info = type.GetProperty("Items");
                var baseList = info.GetValue(toolbox);
                var valueList = info.GetValue(toolbox) as ICollection;

                var list = new List<StiFormsToolboxItemRefl>();

                foreach (var item in valueList)
                    list.Add(new StiFormsToolboxItemRefl(item));

                return list;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Methods.Control
        internal static object GetStiPropertyGridPanel()
        {
            try
            {
                var typeName = GetTypeName("StiPropertyGridPanel");
                return CreateObject(typeName, new object[] { });
            }
            catch
            {
                return null;
            }
        }

        internal static object GetStiFormPreviewControl(string formJson)
        {
            try
            {
                var typeName = GetTypeName("StiFormPreviewControl");
                return CreateObject(typeName, new object[] { formJson });
            }
            catch
            {
                return null;
            }
        }

        internal static object GetStiFormDesignSurfaceControl(string formJson)
        {
            try
            {
                var typeName = GetTypeName("StiFormDesignSurfaceControl");
                return CreateObject(typeName, new object[] { formJson, true });
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
