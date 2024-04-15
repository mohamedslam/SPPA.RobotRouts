#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Data.Engine;
using Stimulsoft.Report.App;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dashboard;

namespace Stimulsoft.Report.Design
{
    public class StiInplaceEditor
    {
        #region Consts
        public static int ButtonSize = 30;
        public static int ButtonMargin = 4;
        #endregion

        #region Properties
        public static bool IsEditorActivated { get; set; }
        #endregion

        #region Methods
        public static bool IsEditorAllowed(object comp)
        {
            return IsFieldsEditorAllowed(comp) || IsSortAndFilterEditorAllowed(comp);
        }

        public static bool IsFieldsEditorAllowed(object comp)
        {
            return !(comp is StiPanel || comp is IStiButtonElement || comp is IStiComponentUI);
        }        

        public static bool IsSortAndFilterEditorAllowed(object comp)
        {
            return IsFilterEditorAllowed(comp) ||
                IsSortEditorAllowed(comp) ||
                IsDataTopNEditorAllowed(comp) ||
                IsDataTransformationEditorAllowed(comp);
        }

        public static bool IsFilterEditorAllowed(object comp)
        {
            return comp is IStiDataFilters;
        }

        public static bool IsSortEditorAllowed(object comp)
        {
            return comp is IStiUserSorts;
        }

        public static bool IsTopNEditorAllowed(object comp)
        {
            if (comp is IStiChartElement chartElement)
                return !chartElement.IsScatterChart;

            return comp is IStiDataTopN;
        }

        public static bool IsDataTransformationEditorAllowed(object comp)
        {
            return comp is IStiDataTransformationElement;
        }

        public static bool IsInteractionEditorAllowed(object comp)
        {
            return comp is IStiElementInteraction;
        }

        public static bool IsDataTopNEditorAllowed(object comp)
        {
            return comp is IStiDataTopN;
        }

        public static bool IsTypeChangingAllowed(object comp)
        {
            return 
                comp is IStiTableElement ||
                comp is IStiCardsElement ||
                comp is IStiChartElement ||
                comp is IStiGaugeElement ||
                comp is IStiPivotTableElement ||
                comp is IStiIndicatorElement ||
                comp is IStiProgressElement ||
                comp is IStiRegionMapElement ||
                comp is IStiOnlineMapElement ||
                comp is IStiComboBoxElement ||
                comp is IStiDatePickerElement ||
                comp is IStiListBoxElement ||
                comp is IStiTreeViewBoxElement ||
                comp is IStiTreeViewElement;
        }

        public static bool IsLeftLocation(StiComponent component)
        {
            if (IsRightLocation(component))
                return false;

            return component.Left - ButtonMargin - ButtonSize > 0;
        }

        public static bool IsRightLocation(StiComponent component)
        {
            if (component.Page == null)
                return true;

            var pos = component.Right + ButtonMargin + ButtonSize;
            return pos < component.Page.Width;
        }

        public static void OnFieldsEditorClick(StiComponent component)
        {
            if (!IsFieldsEditorAllowed(component)) return;

            var componentDesigner = StiComponentDesigner.GetComponentDesigner(component.Report.Designer, component.GetType());
            if (componentDesigner == null) return;

            if (!StiRestrictionsHelper.IsAllowChange(component)) return;

            component.Report.Designer.UndoRedoSave(component.LocalizedName, true);
            componentDesigner.OnDoubleClick(component);
        }

        public static void OnFilterEditorClick(StiComponent component)
        {
            if (!IsFilterEditorAllowed(component)) return;

            var designer = StiDataFiltersDesigner.GetDataFiltersDesigner(component.Report.Designer, component.GetType());
            if (designer == null || !StiRestrictionsHelper.IsAllowChange(component)) return;

            component.Report.Designer.UndoRedoSave(component.LocalizedName, true);
            designer.OnDoubleClick(component);
        }

        public static void OnSortEditorClick(StiComponent component)
        {
            if (!IsSortEditorAllowed(component)) return;

            var designer = StiDataSortDesigner.GetDataSortDesigner(component.Report.Designer, component.GetType());
            if (designer == null || !StiRestrictionsHelper.IsAllowChange(component)) return;

            component.Report.Designer.UndoRedoSave(component.LocalizedName, true);
            designer.OnDoubleClick(component);
        }

        public static void OnDataTopNEditorClick(StiComponent component)
        {
            if (!IsTopNEditorAllowed(component)) return;

            var designer = StiDataTopNDesigner.GetDesigner(component.Report.Designer, component.GetType());
            if (designer == null || !StiRestrictionsHelper.IsAllowChange(component)) return;

            component.Report.Designer.UndoRedoSave(component.LocalizedName, true);
            designer.OnDoubleClick(component);
        }

        public static void OnDataTransformationEditorClick(StiComponent component)
        {
            if (!IsDataTransformationEditorAllowed(component)) return;

            var designer = StiDataTransformationDesigner.GetTransformationDesigner(component.Report.Designer, component.GetType());
            if (designer == null || !StiRestrictionsHelper.IsAllowChange(component)) return;

            component.Report.Designer.UndoRedoSave(component.LocalizedName, true);
            designer.OnDoubleClick(component);
        }

        public static void OnInteractionEditorClick(StiComponent component)
        {
            if (!IsInteractionEditorAllowed(component)) return;

            var designer = StiElementInteractionDesigner.GetDesigner(component.Report.Designer, component.GetType());
            if (designer == null || !StiRestrictionsHelper.IsAllowChange(component)) return;

            component.Report.Designer.UndoRedoSave(component.LocalizedName, true);
            designer.OnDoubleClick(component);
        }
        #endregion
    }
}