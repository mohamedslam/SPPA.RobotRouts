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

using System;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;

namespace Stimulsoft.Report
{
	/// <summary>
	/// The class contains methods to work with style condition.
	/// </summary>
    internal static class StiStyleConditionHelper
    {
        public static bool IsAllowStyle(StiComponent component, StiBaseStyle style)
        {
            foreach (StiStyleCondition condition in style.Conditions)
            {
                var resultComponentName = true;
                var resultComponentType = true;
                var resultPlacement = true;
                var resultLocation = true;

                #region ComponentName
                if ((condition.Type & StiStyleConditionType.ComponentName) > 0)
                {
                    switch (condition.OperationComponentName)
                    {
                        case StiStyleConditionOperation.EqualTo:
                            resultComponentName = component.Name == condition.ComponentName;
                            break;

                        case StiStyleConditionOperation.NotEqualTo:
                            resultComponentName = component.Name != condition.ComponentName;
                            break;

                        case StiStyleConditionOperation.BeginningWith:
                            resultComponentName = component.Name != null && component.Name.StartsWith(condition.ComponentName);
                            break;

                        case StiStyleConditionOperation.EndingWith:
                            resultComponentName = component.Name != null && component.Name.EndsWith(condition.ComponentName);
                            break;

                        case StiStyleConditionOperation.Containing:
                            resultComponentName = component.Name != null && component.Name.Contains(condition.ComponentName);
                            break;

                        case StiStyleConditionOperation.NotContaining:
                            resultComponentName = component.Name != null && !component.Name.Contains(condition.ComponentName);
                            break;
                    }
                }
                #endregion

                #region ComponentType
                if ((condition.Type & StiStyleConditionType.ComponentType) > 0)
                {
                    resultComponentType = false;

                    if ((condition.ComponentType & StiStyleComponentType.Chart) > 0 && component is Stimulsoft.Report.Chart.StiChart) resultComponentType = true;
                    if ((condition.ComponentType & StiStyleComponentType.CrossTab) > 0 && (component is Stimulsoft.Report.CrossTab.StiCrossTab || component is IStiCrossTabField)) resultComponentType = true;
                    if ((condition.ComponentType & StiStyleComponentType.Image) > 0 && component is StiImage) resultComponentType = true;
                    if ((condition.ComponentType & StiStyleComponentType.Text) > 0 && component is StiSimpleText) resultComponentType = true;
                    if ((condition.ComponentType & StiStyleComponentType.Primitive) > 0 && (component is StiShape || component is StiPrimitive)) resultComponentType = true;
                    if ((condition.ComponentType & StiStyleComponentType.CheckBox) > 0 && component is StiCheckBox) resultComponentType = true;

                    if (condition.OperationComponentType == StiStyleConditionOperation.NotEqualTo)
                        resultComponentType = !resultComponentType;
                }
                #endregion

                #region Location
                if ((condition.Type & StiStyleConditionType.Location) > 0 && component.Page != null && component.Parent != null)
                {
                    var compLeft = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Left) / 10, 0);
                    var compTop = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Top) / 10, 0);
                    var compWidth = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Width) / 10, 0);
                    var compHeight = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Height) / 10, 0);
                    var parentWidth = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Parent.Width) / 10, 0);
                    var parentHeight = Math.Round((decimal)component.Page.Unit.ConvertToHInches(component.Parent.Height) / 10, 0);
                    var compRight = compLeft + compWidth;
                    var compBottom = compTop + compHeight;

                    resultLocation = false;

                    if ((condition.Location & StiStyleLocation.Left) > 0 && compTop <= 0 && compLeft <= 0 && compHeight == parentHeight) resultLocation = true;
                    if ((condition.Location & StiStyleLocation.Right) > 0 && compTop <= 0 && compRight >= parentWidth && compHeight == parentHeight) resultLocation = true;

                    if ((condition.Location & StiStyleLocation.Top) > 0 && compLeft <= 0 && compTop <= 0 && compWidth == parentWidth) resultLocation = true;
                    if ((condition.Location & StiStyleLocation.Bottom) > 0 && compLeft <= 0 && compBottom >= parentHeight && compWidth == parentWidth) resultLocation = true;

                    if ((condition.Location & StiStyleLocation.CenterHorizontal) > 0 && compTop <= 0 && compLeft > 0 && compRight < parentWidth && compHeight == parentHeight) resultLocation = true;
                    if ((condition.Location & StiStyleLocation.CenterVertical) > 0 && compLeft <= 0 && compTop > 0 && compBottom < parentHeight && compWidth == parentWidth) resultLocation = true;

                    if (!resultLocation)
                    {
                        if ((condition.Location & StiStyleLocation.TopLeft) > 0 && compTop <= 0 && compLeft <= 0) resultLocation = true;
                        if ((condition.Location & StiStyleLocation.TopCenter) > 0 && compTop <= 0 && compLeft > 0 && compRight < parentWidth) resultLocation = true;
                        if ((condition.Location & StiStyleLocation.TopRight) > 0 && compTop <= 0 && compRight >= parentWidth) resultLocation = true;
                        if ((condition.Location & StiStyleLocation.MiddleLeft) > 0 && compTop > 0 && compBottom < parentHeight && compLeft <= 0) resultLocation = true;
                        if ((condition.Location & StiStyleLocation.MiddleCenter) > 0 && compTop > 0 && compBottom < parentHeight && compLeft > 0 && compRight < parentWidth) resultLocation = true;
                        if ((condition.Location & StiStyleLocation.MiddleRight) > 0 && compTop > 0 && compBottom < parentHeight && compRight >= parentWidth) resultLocation = true;
                        if ((condition.Location & StiStyleLocation.BottomLeft) > 0 && compBottom >= parentHeight && compLeft <= 0) resultLocation = true;
                        if ((condition.Location & StiStyleLocation.BottomCenter) > 0 && compBottom >= parentHeight && compLeft > 0 && compRight < parentWidth) resultLocation = true;
                        if ((condition.Location & StiStyleLocation.BottomRight) > 0 && compBottom >= parentHeight && compRight >= parentWidth) resultLocation = true;
                    }
                                        
                    if (condition.OperationPlacement == StiStyleConditionOperation.NotEqualTo)
                        resultLocation = !resultLocation;
                }
                #endregion

                #region Placement
                if ((condition.Type & StiStyleConditionType.Placement) > 0)
                {
                    resultPlacement = false;

                    if ((condition.Placement & StiStyleComponentPlacement.ReportTitle) > 0 && component.Parent is StiReportTitleBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.ReportSummary) > 0 && component.Parent is StiReportSummaryBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.PageHeader) > 0 && component.Parent is StiPageHeaderBand) resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.PageFooter) > 0 && component.Parent is StiPageFooterBand) resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.GroupHeader) > 0 && component.Parent is StiGroupHeaderBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.GroupFooter) > 0 && component.Parent is StiGroupFooterBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Header) > 0 && component.Parent is StiHeaderBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Footer) > 0 && component.Parent is StiFooterBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.ColumnHeader) > 0  && component.Parent is StiColumnHeaderBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.ColumnFooter) > 0  && component.Parent is StiColumnFooterBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Data) > 0 && component.Parent is StiDataBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.DataEvenStyle) > 0 && component is StiDataBand) resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.DataOddStyle) > 0 && component is StiDataBand) resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Table) > 0 && component.Parent is StiTable)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Hierarchical) > 0 && component.Parent is StiHierarchicalBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Child) > 0 && component.Parent is StiChildBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Empty) > 0 && component.Parent is StiEmptyBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Overlay) > 0 && component.Parent is StiOverlayBand)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Panel) > 0 && component.Parent is StiPanel)resultPlacement = true;
                    if ((condition.Placement & StiStyleComponentPlacement.Page) > 0 && component.Parent is StiPage)resultPlacement = true;

                    #region Nested Level
                    if (resultPlacement && (condition.Type & StiStyleConditionType.PlacementNestedLevel) > 0)
                    {
                        var level = 1;
                        StiComponent parent = component.Parent;
                        if ((condition.Placement & StiStyleComponentPlacement.DataEvenStyle) > 0 ||
                            (condition.Placement & StiStyleComponentPlacement.DataOddStyle) > 0)
                            parent = component;

                        var band = parent as StiBand;
                        if (band != null)
                            level = band.NestedLevel;

                        if (condition.OperationPlacementNestedLevel == StiStyleConditionOperation.EqualTo)
                            resultPlacement = level == condition.PlacementNestedLevel;

                        else if (condition.OperationPlacementNestedLevel == StiStyleConditionOperation.NotEqualTo)
                            resultPlacement = level != condition.PlacementNestedLevel;

                        else if (condition.OperationPlacementNestedLevel == StiStyleConditionOperation.GreaterThan)
                            resultPlacement = level > condition.PlacementNestedLevel;

                        else if (condition.OperationPlacementNestedLevel == StiStyleConditionOperation.GreaterThanOrEqualTo)
                            resultPlacement = level >= condition.PlacementNestedLevel;

                        else if (condition.OperationPlacementNestedLevel == StiStyleConditionOperation.LessThan)
                            resultPlacement = level < condition.PlacementNestedLevel;

                        else if (condition.OperationPlacementNestedLevel == StiStyleConditionOperation.LessThanOrEqualTo)
                            resultPlacement = level <= condition.PlacementNestedLevel;
                    }
                    #endregion

                    if (condition.OperationPlacement == StiStyleConditionOperation.NotEqualTo)
                        resultPlacement = !resultPlacement;
                }
                #endregion

                if (!resultComponentName || !resultComponentType || !resultPlacement || !resultLocation)
                    return false;
            }

            return true;
        }
    }
}
