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

using System.Collections;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;

namespace Stimulsoft.Report.Design
{
    public class StiInplaceEditorTitleElement
    {
        #region Consts
        public static int ButtonSize = 20;
        public static int ButtonMargin = 4;
        #endregion

        #region Fields
        private static Hashtable isTitleFocusedHash = new Hashtable();
        private static Hashtable isSortFocusedHash = new Hashtable();
        private static Hashtable isViewStateFocusedHash = new Hashtable();
        private static Hashtable isComponentFocusedHash = new Hashtable();
        #endregion

        #region Methods
        private static void CheckComponentGuid(StiComponent component)
        {
            component.Guid = StiKeyHelper.GetOrGeneratedKey(component.Guid);
        }

        public static bool IsComponentFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            return isComponentFocusedHash[component.Guid] != null;
        }
        
        public static void SetComponentFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            isComponentFocusedHash[component.Guid] = component.Guid;
        }

        public static void ResetComponentFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            if (isComponentFocusedHash[component.Guid] != null)
                isComponentFocusedHash.Remove(component.Guid);
        }

        public static bool IsTitleFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            return isTitleFocusedHash[component.Guid] != null;
        }

        public static void SetTitleFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            isTitleFocusedHash[component.Guid] = component.Guid;
        }

        public static void ResetTitleFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            if (isTitleFocusedHash[component.Guid] != null)
                isTitleFocusedHash.Remove(component.Guid);
        }

        public static bool IsSortFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            return isSortFocusedHash[component.Guid] != null;
        }

        public static void SetSortFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            isSortFocusedHash[component.Guid] = component.Guid;
        }

        public static void ResetSortFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            if (isSortFocusedHash[component.Guid] != null)
                isSortFocusedHash.Remove(component.Guid);
        }

        public static bool IsViewStateFocused(StiComponent component, int index)
        {
            CheckComponentGuid(component);

            return isViewStateFocusedHash[$"{component.Guid}.{index}"] != null;
        }

        public static void SetViewStateFocused(StiComponent component, int index)
        {
            CheckComponentGuid(component);

            isViewStateFocusedHash[$"{component.Guid}.{index}"] = component.Guid;
        }

        public static void ResetViewStateFocused(StiComponent component)
        {
            CheckComponentGuid(component);

            for (var index = 0; index < 5; index++)
            {
                var guid = $"{component.Guid}.{index}";
                if (isViewStateFocusedHash[guid] != null)
                    isViewStateFocusedHash.Remove(guid);
            }
        }

        public static RectangleD GetTitleRect(RectangleD rect, StiComponent comp)
        {
            rect = StiBorderElementHelper.GetBorderContentRect(rect, comp as IStiElement);
            return GetSimpleTitleRect(rect, comp);
        }

        public static RectangleD GetSimpleTitleRect(RectangleD rect, StiComponent comp)
        {
            var buttonSize = StiScale.XXI(ButtonSize);
            var buttonMargin = StiScale.XXI(ButtonMargin);
            var scale = comp.Report.Info.Zoom;

            var titleRect = new RectangleD(
                rect.Right - buttonMargin - buttonSize, 
                rect.Top + buttonMargin,
                buttonSize, buttonSize);

            var cornerRadius = (comp as IStiCornerRadius)?.CornerRadius;
            if (cornerRadius != null && cornerRadius.TopRight > 0)
                titleRect.X -= StiScale.I(cornerRadius.TopRight * scale);

            if (titleRect.X < rect.X)
                return RectangleD.Empty;
                
            return titleRect;
        }

        public static RectangleD GetSortRect(RectangleD rect, StiComponent comp)
        {
            rect = StiBorderElementHelper.GetBorderContentRect(rect, comp as IStiElement);
            return GetSimpleSortRect(rect, comp);
        }

        public static RectangleD GetSimpleSortRect(RectangleD rect, StiComponent comp)
        {
            var sortRect = GetSimpleTitleRect(rect, comp);
            if (sortRect.IsEmpty) 
                return RectangleD.Empty;

            sortRect.X -= sortRect.Width + StiScale.I4;

            if (sortRect.X < rect.X)
                return RectangleD.Empty;

            return sortRect;
        }

        public static RectangleD GetViewStateRect(RectangleD rect, StiComponent comp, int index)
        {
            var allowUserSorting = StiSortMenuHelper.IsAllowUserSorting(comp as IStiElement);
            rect = StiBorderElementHelper.GetBorderContentRect(rect, comp as IStiElement);
            return GetSimpleViewStateRect(rect, comp, allowUserSorting, index);
        }

        public static RectangleD GetSimpleViewStateRect(RectangleD rect, StiComponent comp, bool allowUserSorting, int index)
        {
            var viewRect = allowUserSorting ? GetSimpleSortRect(rect, comp) : GetSimpleTitleRect(rect, comp);
            if (viewRect.IsEmpty)
                return RectangleD.Empty;

            viewRect.X -= (viewRect.Width + StiScale.I4) * (index + 1);

            if (viewRect.X < rect.X)
                return RectangleD.Empty;

            return viewRect;
        }

        public static void InvokeTitleClick(StiComponent component)
        {
            var titleElement = component as IStiTitleElement;
            if (titleElement == null) return;

            if (StiRestrictionsHelper.IsAllowChange(component))
                titleElement.Title.Visible = !titleElement.Title.Visible;
        }
        #endregion
    }
}
