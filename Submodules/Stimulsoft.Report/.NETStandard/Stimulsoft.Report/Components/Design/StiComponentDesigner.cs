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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Design;
using System;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Design
{
    /// <summary>
    /// Class describes base designer of components.
    /// </summary>
    public class StiComponentDesigner : IStiComponentDesigner
    {
        #region Handlers
        /// <summary>
        /// Occurs when user DoubleClick on a the component in the designer.
        /// </summary>
        /// <param name="sender">Component on what DoubleClick occured.</param>
        public virtual void OnDoubleClick(StiComponent sender)
        {
            if (StiOptions.Designer.IsComponentEditorEnabled)
                Design(sender);
        }
        #endregion

        #region Methods
        public virtual StiAction GetActionFromPoint(double x, double y, StiComponent component)
        {
            return GetActionFromPoint(x, y, component, false);
        }

        /// <summary>
		/// Returns StiAction for specified component from point.
		/// </summary>
		/// <param name="x">x coordinate.</param>
		/// <param name="y">y coordinate.</param>
		/// <param name="component">Component for checking.</param>
		/// <returns>Action for this point.</returns>
        public virtual StiAction GetActionFromPoint(double x, double y, StiComponent component, bool forGetDesigner = false)
        {
            //If you change this value from 4 you need also change method StiTableHelper.GetDistForResize
            var dist = component.Page.Unit.ConvertFromHInches(4d);
            var locked = component.Locked || component.Inherited || !StiRestrictionsHelper.IsAllowChangePosition(component);
            var action = StiActionUtils.PointInRect(dist, x, y, component.ClientRectangle, component.SelectRectangle, component.IsSelected, locked);

            #region Correct action depend on restrictions
            if (action == StiAction.Move)
            {
                if (!StiRestrictionsHelper.IsAllowMove(component) && !forGetDesigner)
                    return StiAction.None;

                return action;
            }

            if (action == StiAction.Select)
            {
                if (!StiRestrictionsHelper.IsAllowSelect(component))
                    return StiAction.None;

                return action;
            }

            if (!StiRestrictionsHelper.IsAllowResize(component))
                return StiAction.None;
            #endregion

            return action;
        }

        /// <summary>
        /// Returns the action fits to the position of a point in the specified rectangle.
        /// </summary>
        /// <param name="x">X point.</param>
        /// <param name="y">Y point.</param>
        /// <returns>Action.</returns>
        public virtual StiAction PointInRect(IStiDesignerBase designer, StiComponent comp, int x, int y)
        {
            //If you change this value from 4 you need also change method StiTableHelper.GetDistForResize
            var dist = comp.Page.Unit.ConvertFromHInches(4d);

            var action = StiActionUtils.PointInRect(
                dist,
                designer.XToPage(x), designer.YToPage(y),
                comp.ComponentToPage(comp.ClientRectangle),
                comp.ComponentToPage(comp.SelectRectangle),
                comp.IsSelected,
                !(comp is IStiTableCell) &&
                (comp.Locked ||
                comp.Inherited ||
                (!StiRestrictionsHelper.IsAllowChangePosition(comp))));

            #region Correct action depend on restrictions
            if (action == StiAction.Move)
            {
                if (!StiRestrictionsHelper.IsAllowMove(comp))
                    return StiAction.None;

                return action;
            }
            if (action == StiAction.Select)
            {
                if (!StiRestrictionsHelper.IsAllowSelect(comp))
                    return StiAction.None;

                return action;
            }
            if (!StiRestrictionsHelper.IsAllowResize(comp))
                return StiAction.None;
            #endregion

            return action;
        }

        /// <summary>
        /// Calls the designer of the component.
        /// </summary>
        /// <param name="component">Component for edition.</param>
        /// <returns>Result of showing the component designer.</returns>
        public virtual DialogResult Design(StiComponent component)
        {
            return DialogResult.None;
        }

        /// <summary>
		/// Creates a component of the specified type.
		/// </summary>
		/// <param name="componentType">Type of conmponent being ceated.</param>
		/// <param name="region">The rectangle describes the component size.</param>
		/// <returns>Created component.</returns>
		public virtual StiComponent CreateComponent(Type componentType, RectangleD region)
        {
            return StiActivator.CreateObject(componentType, new object[] { region }) as StiComponent;
        }

        /// <summary>
		/// Returns a designer of the component.
		/// </summary>
		/// <param name="designer">Report designer.</param>
		/// <param name="componentType">Component type.</param>
		/// <returns>Component designer.</returns>
        public static StiComponentDesigner GetComponentDesigner(IStiDesignerBase designer, Type componentType)
        {
            return GetComponentDesigner(designer, componentType, StiGuiMode.Gdi);
        }

        /// <summary>
        /// Returns a designer of the component.
        /// </summary>
        /// <param name="designer">Report designer.</param>
        /// <param name="componentType">Component type.</param>
        /// <returns>Component designer.</returns>
        public static StiComponentDesigner GetComponentDesigner(IStiDesignerBase designer, Type componentType, StiGuiMode guiMode)
        {
            switch (guiMode)
            {
                case StiGuiMode.Gdi:
                    {
                        var attrs = componentType.GetCustomAttributes(typeof(StiDesignerAttribute), true) as StiDesignerAttribute[];
                        if (attrs == null || attrs.Length <= 0) return null;

                        var designerTypeName = attrs[0].DesignerTypeName;
                        var designerType = GetTypeFromName(designerTypeName);
                        if (designerType == null) return null;

                        return StiActivator.CreateObject(designerType, new object[] { designer }) as StiComponentDesigner;
                    }

                case StiGuiMode.Wpf:
                    {
                        var attrs = componentType.GetCustomAttributes(typeof(StiWpfDesignerAttribute), true) as StiWpfDesignerAttribute[];
                        if (attrs == null || attrs.Length <= 0) return null;

                        var designerTypeName = attrs[0].DesignerTypeName;
                        var designerType = GetTypeFromName(designerTypeName);
                        if (designerType == null) return null;

                        return StiActivator.CreateObject(designerType, new object[] { designer }) as StiComponentDesigner;
                    }

                default:
                    {
                        var attrs = componentType.GetCustomAttributes(typeof(StiWpfDesignerAttribute), true) as StiWpfDesignerAttribute[];
                        if (attrs == null || attrs.Length <= 0) return null;

                        var designerTypeName = attrs[0].DesignerTypeName;
                        designerTypeName = designerTypeName.Replace("Stimulsoft.Report.WpfDesign", "Stimulsoft.Client.Designer");
                        designerTypeName = designerTypeName.Replace("StiWpf", "StiCloud");

                        var designerType = GetTypeFromName(designerTypeName);
                        if (designerType == null) return null;

                        return StiActivator.CreateObject(designerType, new object[] { designer }) as StiComponentDesigner;
                    }
            }
        }

        internal static Type GetTypeFromName(string typeName)
        {
            return string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName);
        }
        #endregion

        #region Properties
        public bool FirstRun { get; set; }

        /// <summary>
        /// Report designer.
        /// </summary>
        public IStiDesignerBase Designer { get; set; }
        #endregion

        /// <summary>
        /// Creates a new designer of the component.
        /// </summary>
        /// <param name="designer">Report designer.</param>
        public StiComponentDesigner(IStiDesignerBase designer)
        {
            this.Designer = designer;
        }
    }
}
