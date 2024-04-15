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
using Stimulsoft.Report.Dashboard.Editor;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Surface.Editor;
using System.Collections.Generic;
using System.Reflection;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dashboard
{
    internal static class StiDashboardDesignAssembly
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
                    Assembly = StiAssemblyFinder.GetAssembly($"Stimulsoft.Dashboard.Design, {StiVersion.VersionInfo}");
                }
                catch
                {
                }
                isInited = true;
            }
        }

        internal static IStiHtmlTextEditorControl GetHtmlTextEditor()
        {
            try
            {
                LoadAssembly();
                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Dashboard.Design.Controls.StiHtmlTextEditorControl");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type) as IStiHtmlTextEditorControl;

            }
            catch
            {
            }
            return null;
        }

        internal static IStiElementConditionsEditorForm GetElementConditionsEditorFormFactory(IStiElement element)
        {
            IStiElementConditionsEditorForm form = null;
            if (element is IStiChartElement)
                form = GetChartElementConditionsEditorForm((IStiChartElement)element);

            if (element is IStiIndicatorElement)
                form = GetIndicatorElementConditionsEditorForm((IStiIndicatorElement)element);

            if (element is IStiPivotTableElement)
                form = GetPivotTableElementConditionsEditorForm((IStiPivotTableElement)element);

            if (element is IStiProgressElement)
                form = GetProgressElementConditionsEditorForm((IStiProgressElement)element);

            if (element is IStiTableElement)
                form = GetTableElementConditionsEditorForm((IStiTableElement)element);

            return form;
        }

        internal static IStiUnplacedElementsForm GetUnplacedElementsFormFactory(List<IStiElement> elements)
        {
            try
            {
                LoadAssembly();

                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Dashboard.Design.Editors.StiUnplacedElementsForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new[] { elements }) as IStiUnplacedElementsForm;

            }
            catch
            {
            }
            return null;
        }

        internal static IStiElementConditionsEditorForm GetIndicatorElementConditionsEditorForm(IStiIndicatorElement indicatorElement)
        {
            try
            {
                LoadAssembly();
                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Dashboard.Design.Editors.Indicator.StiIndicatorElementConditionsEditorForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new[] { indicatorElement }) as IStiElementConditionsEditorForm;

            }
            catch
            {
            }
            return null;
        }

        internal static IStiElementConditionsEditorForm GetChartElementConditionsEditorForm(IStiChartElement chartElement)
        {
            try
            {
                LoadAssembly();
                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Dashboard.Design.Editors.Chart.StiChartElementConditionsEditorForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new[] { chartElement }) as IStiElementConditionsEditorForm;

            }
            catch
            {
            }
            return null;
        }

        internal static IStiElementConditionsEditorForm GetPivotTableElementConditionsEditorForm(IStiPivotTableElement pivotTableElement)
        {
            try
            {
                LoadAssembly();
                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Dashboard.Design.Editors.PivotTable.StiPivotTableElementConditionsEditorForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new[] { pivotTableElement }) as IStiElementConditionsEditorForm;
            }
            catch
            {
            }
            return null;
        }


        internal static IStiElementConditionsEditorForm GetProgressElementConditionsEditorForm(IStiProgressElement progressElement)
        {
            try
            {
                LoadAssembly();
                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Dashboard.Design.Editors.Progress.StiProgressElementConditionsEditorForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new[] { progressElement }) as IStiElementConditionsEditorForm;
            }
            catch
            {
            }
            return null;
        }

        private static IStiElementConditionsEditorForm GetTableElementConditionsEditorForm(IStiTableElement element)
        {
            try
            {
                LoadAssembly();
                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Dashboard.Design.Editors.Table.StiTableElementConditionsEditorForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new[] { element }) as IStiElementConditionsEditorForm;
            }
            catch
            {
            }
            return null;
        }

        internal static Form GetWatermarkFormFactory(IStiDashboardWatermark watermark)
        {
            try
            {
                LoadAssembly();

                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Dashboard.Design.Editors.StiWatermarkEditorForm");
                if (type == null)
                    return null;

                return StiActivator.CreateObject(type, new object[2] { watermark.DashboardWatermark, watermark }) as Form;

            }
            catch
            {
            }
            return null;
        }
        #endregion
    }
}
