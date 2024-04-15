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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Surface.Editor;
using System.Collections.Generic;
using System.Reflection;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.App
{
    internal static class StiAppDesignAssembly
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
#if DEBUG
            if (isInited) return;

            lock (lockObject)
            {
                try
                {
                    Assembly = StiAssemblyFinder.GetAssembly($"Stimulsoft.App.Design, {StiVersion.VersionInfo}");
                }
                catch
                {
                }
                isInited = true;
            }
#endif
        }

        internal static IStiUnplacedElementsForm GetUnplacedComponentsUIFormFactory(List<IStiComponentUI> elements)
        {
            try
            {
                LoadAssembly();

                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.App.Design.Editors.StiUnplacedComponentsForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new[] { elements }) as IStiUnplacedElementsForm;

            }
            catch
            {
            }
            return null;
        }

        internal static Form GetWatermarkFormFactory(IStiWatermarkUI watermark)
        {
            try
            {
                LoadAssembly();

                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.App.Design.Editors.StiWatermarkEditorForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new object[2] { watermark.WatermarkUI, watermark }) as Form;

            }
            catch
            {
            }
            return null;
        }
        #endregion
    }
}
