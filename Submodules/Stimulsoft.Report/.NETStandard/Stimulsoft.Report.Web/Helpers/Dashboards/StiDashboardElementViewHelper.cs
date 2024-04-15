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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Data.Engine;
using Stimulsoft.Data.Helpers;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Controls;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Styles;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Stimulsoft.Report.Chart;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    internal class StiDashboardElementViewHelper
    {
        #region Methods
        public static Hashtable GetElementContentAttributes(IStiElement element, double scaleX, double scaleY, StiRequestParams requestParams)
        {
            var content = new Hashtable();

            #region ListBoxElement
            if (element is IStiListBoxElement)
            {
                var listBoxElement = element as IStiListBoxElement;
                content["items"] = StiListBoxElementViewHelper.GetElementItems(listBoxElement);
                content["filters"] = StiDataFiltersHelper.GetElementFilters(listBoxElement);
                content["columnPath"] = StiListBoxElementViewHelper.GetColumnPath(listBoxElement);
                content["isStringColumnType"] = StiDataFiltersHelper.IsStringColumnType(listBoxElement);
                content["showAllValue"] = listBoxElement.ShowAllValue;
                content["selectionMode"] = listBoxElement.SelectionMode;
                content["orientation"] = listBoxElement.Orientation;
                content["settings"] = StiListBoxElementViewHelper.GetSettings(listBoxElement);
                content["title"] = GetTitle(listBoxElement);
            }
            #endregion

            #region ComboBoxElement
            else if (element is IStiComboBoxElement)
            {
                var comboBoxElement = element as IStiComboBoxElement;
                content["items"] = StiComboBoxElementViewHelper.GetElementItems(comboBoxElement);
                content["filters"] = StiDataFiltersHelper.GetElementFilters(comboBoxElement);
                content["columnPath"] = StiComboBoxElementViewHelper.GetColumnPath(comboBoxElement);
                content["isStringColumnType"] = StiDataFiltersHelper.IsStringColumnType(comboBoxElement);
                content["showAllValue"] = comboBoxElement.ShowAllValue;
                content["selectionMode"] = comboBoxElement.SelectionMode;
                content["settings"] = StiComboBoxElementViewHelper.GetSettings(comboBoxElement);
            }
            #endregion

            #region DatePickerElement
            else if (element is IStiDatePickerElement)
            {
                var datePickerElement = element as IStiDatePickerElement;
                content["filters"] = StiDataFiltersHelper.GetElementFilters(datePickerElement);
                content["columnPath"] = StiDatePickerElementViewHelper.GetColumnPath(datePickerElement);
                content["selectionMode"] = datePickerElement.SelectionMode;
                content["initialRangeSelection"] = datePickerElement.InitialRangeSelection;
                content["initialRangeSelectionSource"] = datePickerElement.InitialRangeSelectionSource;
                content["condition"] = datePickerElement.Condition;
                content["settings"] = StiDatePickerElementViewHelper.GetSettings(datePickerElement);

                var isVariablePresent = StiDatePickerElementViewHelper.IsVariablePresent(datePickerElement);
                var isRangeVariablePresent = StiDatePickerElementViewHelper.IsRangeVariablePresent(datePickerElement);

                content["isVariablePresent"] = isVariablePresent;
                content["isRangeVariablePresent"] = isRangeVariablePresent;

                if (datePickerElement.InitialRangeSelectionSource == StiInitialDateRangeSelectionSource.Variable)
                {
                    if (isRangeVariablePresent)
                        content["variableRangeValues"] = StiDatePickerElementViewHelper.GetVariableRangeValues(datePickerElement);
                    else if (isVariablePresent)
                        content["variableValue"] = StiDatePickerElementViewHelper.GetVariableValue(datePickerElement);
                }
                else if (datePickerElement.SelectionMode == StiDateSelectionMode.AutoRange)
                    content["autoRangeValues"] = StiDatePickerElementViewHelper.GetAutoRangeValues(datePickerElement);
            }
            #endregion

            #region TreeViewElement
            else if (element is IStiTreeViewElement)
            {
                var treeViewElement = element as IStiTreeViewElement;
                content["items"] = StiTreeViewElementViewHelper.GetElementItems(treeViewElement);
                content["filters"] = StiDataFiltersHelper.GetElementFilters(treeViewElement);
                content["columnPath"] = StiTreeViewElementViewHelper.GetColumnPath(treeViewElement);
                content["isStringColumnType"] = StiDataFiltersHelper.IsStringColumnType(treeViewElement);
                content["meterKey"] = StiTreeViewElementViewHelper.GetMeterKey(treeViewElement);
                content["showAllValue"] = treeViewElement.ShowAllValue;
                content["selectionMode"] = treeViewElement.SelectionMode;
                content["settings"] = StiTreeViewElementViewHelper.GetSettings(treeViewElement);
                content["title"] = GetTitle(treeViewElement);
            }
            #endregion

            #region TreeViewBoxElement
            else if (element is IStiTreeViewBoxElement)
            {
                var treeViewBoxElement = element as IStiTreeViewBoxElement;
                content["items"] = StiTreeViewBoxElementViewHelper.GetElementItems(treeViewBoxElement);
                content["filters"] = StiDataFiltersHelper.GetElementFilters(treeViewBoxElement);
                content["columnPath"] = StiTreeViewBoxElementViewHelper.GetColumnPath(treeViewBoxElement);
                content["isStringColumnType"] = StiDataFiltersHelper.IsStringColumnType(treeViewBoxElement);
                content["meterKey"] = StiTreeViewBoxElementViewHelper.GetMeterKey(treeViewBoxElement);
                content["showAllValue"] = treeViewBoxElement.ShowAllValue;
                content["selectionMode"] = treeViewBoxElement.SelectionMode;
                content["settings"] = StiTreeViewBoxElementViewHelper.GetSettings(treeViewBoxElement);
                content["title"] = GetTitle(treeViewBoxElement);
            }
            #endregion

            #region TableElement
            else if (element is IStiTableElement)
            {
                var tableElement = element as IStiTableElement;
                content["data"] = StiTableElementViewHelper.GetTableData(tableElement, scaleX);
                content["hiddenData"] = StiTableElementViewHelper.GetTableHiddenData(tableElement);
                content["filters"] = StiDataFiltersHelper.GetElementFilters(tableElement);
                content["sorts"] = StiDataSortsHelper.GetElementSorts(tableElement);
                content["settings"] = StiTableElementViewHelper.GetTableSettings(tableElement);
                content["title"] = GetTitle(tableElement);
                content["interaction"] = GetDashboardInteraction(tableElement);
                content["filtersString"] = StiDataFiltersHelper.GetDataTableFilterQueryStringRepresentation(tableElement);
            }
            #endregion

            #region ChartElement
            else if (element is IStiChartElement)
            {
                var chartElement = element as IStiChartElement;
                content["filters"] = StiDataFiltersHelper.GetElementFilters(chartElement);
                content["htmlContent"] = StiDashboardsSvgHelper.SaveElementToString(chartElement, scaleX, scaleY, false, StiExportFormat.Html5, requestParams);
                content["interaction"] = GetDashboardInteraction(chartElement);
                content["filtersString"] = StiDataFiltersHelper.GetDataTableFilterQueryStringRepresentation(chartElement);
                content["title"] = GetTitle(chartElement);
                content["sortItems"] = StiDataSortsHelper.GetSortMenuItems(chartElement);
                content["userViewStates"] = StiChartElementViewHelper.GetUserViewStates(chartElement);
                content["selectedViewStateKey"] = (chartElement as IStiUserViewStates)?.SelectedViewStateKey;
                content["dataMode"] = chartElement.DataMode;

                if (StiChartElementViewHelper.IsBubble(chartElement))
                {
                    content["isBubble"] = true;
                    content["bubleYColumnPath"] = StiChartElementViewHelper.GetBubleYColumnPath(chartElement);
                    content["bubleXColumnPath"] = StiChartElementViewHelper.GetBubleXColumnPath(chartElement);
                }
                else
                {
                    content["argumentColumnPath"] = StiChartElementViewHelper.GetArgumentColumnPath(chartElement);
                    content["seriesColumnPath"] = StiChartElementViewHelper.GetSeriesColumnPath(chartElement);
                }
            }
            #endregion

            #region RegionMapElement
            else if (element is IStiRegionMapElement)
            {
                var mapElement = element as IStiRegionMapElement;
                content["columnPath"] = StiRegionMapElementViewHelper.GetColumnPath(mapElement);
                content["filters"] = StiDataFiltersHelper.GetElementFilters(mapElement);
                content["htmlContent"] = StiDashboardsSvgHelper.SaveElementToString(mapElement, scaleX, scaleY, false, StiExportFormat.Html5);
                content["interaction"] = GetDashboardInteraction(mapElement);
                content["filtersString"] = StiDataFiltersHelper.GetDataTableFilterQueryStringRepresentation(mapElement);
                content["title"] = GetTitle(mapElement);
                content["isDark"] = StiDashboardStyleHelper.IsDarkStyle(mapElement);
                content["showZoomPanel"] = StiOptions.Viewer.Map.ShowZoomPanel;
            }
            #endregion

            #region PivotTable
            else if (element is IStiPivotTableElement)
            {
                var pivotElement = element as IStiPivotTableElement;
                content["data"] = StiPivotTableElementViewHelper.GetPivotTableData(pivotElement);
                content["settings"] = StiPivotTableElementViewHelper.GetPivotTableSettings(pivotElement);
                content["title"] = GetTitle(pivotElement);
            }
            #endregion

            #region OnlineMap
            else if (element is IStiOnlineMapElement)
            {
                content["htmlContent"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(StiDashboardOnlimeMapHelper.GetBingMapScript(element, true)));
            }
            #endregion

            #region Panel
            else if (element is IStiPanel)
            {
                content["dashboardWatermark"] = GetDashboardWatermark(element);
            }
            #endregion

            #region Indicator
            else if (element is IStiIndicatorElement)
            {
                var needToVertScroll = true;
                var needToHorScroll = false;

                content["sortItems"] = StiDataSortsHelper.GetSortMenuItems(element);
                content["interaction"] = GetDashboardInteraction(element);
                content["svgContent"] = StiDashboardsSvgHelper.SaveElementToString(element, ref needToVertScroll, ref needToHorScroll, scaleX, scaleY, false, StiExportFormat.ImageSvg);

                if (needToVertScroll)
                {
                    content["title"] = GetTitle(element);
                    content["svgContentIsVertScrollable"] = true;
                }
            }
            #endregion

            #region GaugeElement
            else if (element is IStiGaugeElement)
            {
                var needToVertScroll = true;
                var needToHorScroll = false;
                
                content["svgContent"] = StiDashboardsSvgHelper.SaveElementToString(element, ref needToVertScroll, ref needToHorScroll, scaleX, scaleY, false, StiExportFormat.ImageSvg, requestParams);
                content["sortItems"] = StiDataSortsHelper.GetSortMenuItems(element);

                if (needToVertScroll)
                {
                    content["title"] = GetTitle(element);
                    content["svgContentIsVertScrollable"] = true;
                }
            }
            #endregion

            #region ButtonElement
            else if (element is IStiButtonElement)
            {
                var buttonElement = element as IStiButtonElement;
                content["buttonText"] = !string.IsNullOrEmpty(buttonElement.Text) ? Convert.ToBase64String(Encoding.UTF8.GetBytes(buttonElement.Text)) : "";
                content["buttonType"] = buttonElement.Type;
                content["iconAlignment"] = buttonElement.IconAlignment;
                content["horAlignment"] = (buttonElement as IStiTextHorAlignment).HorAlignment;
                content["vertAlignment"] = (buttonElement as IStiVertAlignment).VertAlignment;
                content["wordWrap"] = (buttonElement as IStiWordWrap).WordWrap;
                content["buttonShapeType"] = buttonElement.ShapeType;
                content["brush"] = GetBrushStr((buttonElement as IStiBrush).Brush);
                content["textBrush"] = GetBrushStr((buttonElement as IStiTextBrush).TextBrush);
                content["iconBrush"] = GetBrushStr(buttonElement.IconBrush);
                content["font"] = GetFontJson(buttonElement.Font);
                content["buttonIconSet"] = GetButtonIconSetProperty(buttonElement.GetIconSet());
                content["buttonVisualStates"] = GetButtonVisualStatesProperty(buttonElement.GetVisualStates());
                content["styleColors"] = GetButtonStyleColors(buttonElement);
                content["checked"] = buttonElement.Checked;
            }
            #endregion

            #region ImageElement
            else if (element is IStiImageElement)
            {                
                var needToVertScroll = false;
                var needToHorScroll = false;

                var imageElement = element as IStiImageElement;
                content["interaction"] = GetDashboardInteraction(imageElement);
                content["aspectRatio"] = imageElement.AspectRatio;
                content["horAlignment"] = ((IStiHorAlignment)imageElement).HorAlignment;
                content["vertAlignment"] = ((IStiVertAlignment)imageElement).VertAlignment;
                content["svgContent"] = StiDashboardsSvgHelper.SaveElementToString(element, ref needToVertScroll, ref needToHorScroll, scaleX, scaleY, false, StiExportFormat.ImageSvg);
            }
            #endregion

            #region OthersElements
            else
            {
                if (element is IStiTextElement)
                {
                    content["interaction"] = GetDashboardInteraction(element);
                    content["plainText"] = ((IStiTextElement)element).GetSimpleText();
                    content["isTimeExpression"] = Data.Helpers.StiExpressionHelper.IsTimeExpression(((IStiTextElement)element).Text);
                }

                var needToVertScroll = true;
                var needToHorScroll = element is IStiCardsElement;
                
                content["svgContent"] = StiDashboardsSvgHelper.SaveElementToString(element, ref needToVertScroll, ref needToHorScroll, scaleX, scaleY, false, StiExportFormat.ImageSvg);

                if (element is IStiProgressElement || element is IStiIndicatorElement || element is IStiCardsElement)
                {
                    if (!(element is IStiCardsElement))
                    {
                        content["sortItems"] = StiDataSortsHelper.GetSortMenuItems(element);
                    }

                    if (needToVertScroll || needToHorScroll)
                    {
                        content["title"] = GetTitle(element);
                        if (needToVertScroll) content["svgContentIsVertScrollable"] = true;
                        if (needToHorScroll) content["svgContentIsHorScrollable"] = true;
                    }
                }
            }
            #endregion

            return content;
        }
        #endregion

        #region Methods.Helpers
        public static string GetForeColor(IStiElement element)
        {
            return StiReportHelper.GetHtmlColor(StiDashboardStyleHelper.GetForeColor(element));
        }

        public static string GetBackColor(IStiElement element)
        {
            return StiReportHelper.GetHtmlColor(StiDashboardStyleHelper.GetBackColor(element, null, true));
        }

        public static Hashtable GetBorder(IStiElement element)
        {
            if (element is IStiSimpleBorder)
                return GetBorderJson((element as IStiSimpleBorder).Border);
            else
                return null;
        }

        public static Hashtable GetBorderJson(StiSimpleBorder border)
        {
            if (border == null) return null;

            return new Hashtable()
            {
                ["left"] = border.IsLeftBorderSidePresent,
                ["top"] = border.IsTopBorderSidePresent,
                ["right"] = border.IsRightBorderSidePresent,
                ["bottom"] = border.IsBottomBorderSidePresent,
                ["size"] = border.Size,
                ["color"] = StiReportHelper.GetHtmlColor(border.Color),
                ["style"] = (int)border.Style
            };
        }

        public static Hashtable GetFont(IStiElement element)
        {
            if (element is IStiFont)
                return GetFontJson((element as IStiFont).Font);
            else
                return GetFontJson(new Font("Arial", 10));
        }

        public static Hashtable GetFontJson(Font font)
        {
            if (font == null) return null;

            return new Hashtable()
            {
                ["name"] = !String.IsNullOrEmpty(font.OriginalFontName) ? font.OriginalFontName : font.Name,
                ["size"] = font.Size.ToString(),
                ["bold"] = font.Bold,
                ["italic"] = font.Italic,
                ["underline"] = font.Underline,
                ["strikeout"] = font.Strikeout
            };
        }

        public static Hashtable GetTitle(IStiElement element)
        {
            var titleElement = element as IStiTitleElement;
            if (titleElement != null && titleElement.Title != null)
            {
                var title = titleElement.Title;
                var titleObj = new Hashtable();
                var titleBackColor = title.BackColor == Color.Transparent ? StiDashboardStyleHelper.GetBackColor(element, null, true) : title.BackColor;
                titleObj["backColor"] = titleBackColor.Equals(StiDashboardStyleHelper.GetBackColor(element, null, true)) ? "transparent" : StiReportHelper.GetHtmlColor(titleBackColor);
                titleObj["foreColor"] = StiReportHelper.GetHtmlColor(title.ForeColor == Color.Transparent ? StiDashboardStyleHelper.GetTitleForeColor(element) : title.ForeColor);
                titleObj["text"] = title.Text != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(StiReportParser.Parse(title.Text, element))) : string.Empty;
                titleObj["font"] = GetFontJson(title.Font);
                titleObj["horAlignment"] = title.HorAlignment;
                titleObj["visible"] = title.Visible;

                return titleObj;
            }

            return null;
        }

        private static string GetBrushStr(StiBrush brush)
        {
            string brushStr = "none";

            if (brush is StiStyleBrush)
            {
                return "isStyleBrush";
            }
            else if (brush is StiDefaultBrush)
            {
                return "isDefaultBrush";
            }
            else if (brush is StiEmptyBrush)
            {
                return "0";
            }
            else if (brush is StiSolidBrush)
            {
                var solid = brush as StiSolidBrush;
                return string.Format("1;{0}",
                    StiReportHelper.GetHtmlColor(solid.Color));
            }
            else if (brush is StiHatchBrush)
            {
                var hatch = brush as StiHatchBrush;
                return string.Format("2;{0};{1};{2}",
                    StiReportHelper.GetHtmlColor(hatch.ForeColor),
                    StiReportHelper.GetHtmlColor(hatch.BackColor),
                    ((int)hatch.Style).ToString());
            }
            else if (brush is StiGradientBrush)
            {
                var gradient = brush as StiGradientBrush;
                return string.Format("3;{0};{1};{2}",
                    StiReportHelper.GetHtmlColor(gradient.StartColor),
                    StiReportHelper.GetHtmlColor(gradient.EndColor),
                    gradient.Angle.ToString());
            }
            else if (brush is StiGlareBrush)
            {
                var glare = brush as StiGlareBrush;
                return string.Format("4;{0};{1};{2};{3};{4}",
                    StiReportHelper.GetHtmlColor(glare.StartColor),
                    StiReportHelper.GetHtmlColor(glare.EndColor),
                    glare.Angle.ToString(),
                    glare.Focus.ToString(),
                    glare.Scale.ToString());
            }
            else if (brush is StiGlassBrush)
            {
                var glass = brush as StiGlassBrush;
                return string.Format("5;{0};{1};{2}",
                    StiReportHelper.GetHtmlColor(glass.Color),
                    glass.Blend.ToString(),
                    glass.DrawHatch ? "1" : "0");
            }

            return brushStr;
        }

        #region Button IconSet
        private static Hashtable GetButtonIconSetProperty(IStiButtonElementIconSet iconSet)
        {
            if (iconSet != null)
            {
                return new Hashtable()
                {
                    ["icon"] = iconSet.Icon != null ? StiFontIconsHelper.GetContent(iconSet.Icon) : "",
                    ["checkedIcon"] = iconSet.CheckedIcon != null ? StiFontIconsHelper.GetContent(iconSet.CheckedIcon) : "",
                    ["uncheckedIcon"] = iconSet.UncheckedIcon != null ? StiFontIconsHelper.GetContent(iconSet.UncheckedIcon) : "",
                };
            }
            return null;
        }
        #endregion

        #region Button VisualStates
        private static Hashtable GetButtonVisualStatesProperty(IStiButtonVisualStates visualStates)
        {
            if (visualStates != null)
            {
                var result = new Hashtable();
                var states = new Hashtable()
                {
                    ["hover"] = visualStates.GetHoverState(),
                    ["pressed"] = visualStates.GetPressedState(),
                    ["check"] = visualStates.GetCheckedState()
                };

                foreach (DictionaryEntry state in states)
                {
                    var visualState = state.Value as IStiButtonVisualState;

                    result[state.Key] = new Hashtable()
                    {
                        ["border"] = GetBorderJson(visualState.Border),
                        ["brush"] = GetBrushStr(visualState.Brush),
                        ["font"] = GetFontJson(visualState.Font),
                        ["iconBrush"] = GetBrushStr(visualState.IconBrush),
                        ["textBrush"] = GetBrushStr(visualState.TextBrush),
                        ["iconSet"] = GetButtonIconSetProperty(visualState.GetIconSet())
                    };
                };

                return result;
            }
            return null;
        }
        #endregion

        #region Button Style Colors
        private static Hashtable GetButtonStyleColors(IStiButtonElement buttonElement)
        {
            var styleColors = new Hashtable();
            var controlElementStyle = StiDashboardStyleHelper.GetControlStyle(buttonElement);
            if (controlElementStyle != null)
            {
                styleColors["backColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.BackColor);
                styleColors["hoverBackColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.HotBackColor);
                styleColors["selectedBackColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.SelectedBackColor);

                styleColors["iconColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.GlyphColor);
                styleColors["hoverIconColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.HotGlyphColor);
                styleColors["selectedIconColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.SelectedGlyphColor);

                styleColors["textColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.ForeColor);
                styleColors["hoverTextColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.HotForeColor);
                styleColors["selectedTextColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.SelectedForeColor);
            }
            return styleColors;
        }
        #endregion

        public static Hashtable GetControlElementSettings(IStiElement element)
        {
            var settings = new Hashtable();
            settings["backColor"] = GetBackColor(element);
            settings["foreColor"] = GetForeColor(element);
            settings["isDarkStyle"] = StiDashboardStyleHelper.IsDarkStyle(element);

            var elementBackColor = StiDashboardStyleHelper.GetBackColor(element, null, true);
            if (elementBackColor.A != 255) settings["backColor"] = "transparent";

            var foreColorFormStyle = !(element is IStiForeColor && (element as IStiForeColor).ForeColor != null && (element as IStiForeColor).ForeColor != Color.Transparent);

            var controlElement = element as IStiControlElement;
            if (controlElement != null)
            {
                var controlElementStyle = StiDashboardStyleHelper.GetControlStyle(controlElement);
                if (controlElementStyle != null)
                {
                    settings["glyphColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.GlyphColor);
                    settings["hotBackColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.HotBackColor);
                    settings["hotForeColor"] = foreColorFormStyle ? StiReportHelper.GetHtmlColor(controlElementStyle.HotForeColor) : settings["foreColor"];
                    settings["hotGlyphColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.HotGlyphColor);
                    settings["hotSelectedBackColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.HotSelectedBackColor);
                    settings["hotSelectedForeColor"] = foreColorFormStyle ? StiReportHelper.GetHtmlColor(controlElementStyle.HotSelectedForeColor) : settings["foreColor"];
                    settings["hotSelectedGlyphColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.HotSelectedGlyphColor);
                    settings["selectedBackColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.SelectedBackColor);
                    settings["selectedForeColor"] = foreColorFormStyle ? StiReportHelper.GetHtmlColor(controlElementStyle.SelectedForeColor) : settings["foreColor"];
                    settings["selectedGlyphColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.SelectedGlyphColor);
                    settings["separatorColor"] = StiReportHelper.GetHtmlColor(controlElementStyle.SeparatorColor);
                }
            }

            return settings;
        }

        public static Hashtable GetLayout(IStiElement element)
        {
            var layoutObj = new Hashtable();
            var interactionLayout = (element as IStiElementInteraction)?.DashboardInteraction as IStiInteractionLayout;

            if (interactionLayout != null)
            {
                layoutObj["fullScreenButton"] = interactionLayout.ShowFullScreenButton;
                layoutObj["saveButton"] = interactionLayout.ShowSaveButton;
                layoutObj["viewDataButton"] = interactionLayout.ShowViewDataButton;
            }

            return layoutObj;
        }

        private static Color FixColor(Color color)
        {
            return color.A < 255
                ? Color.FromArgb(255, color.R, color.G, color.B)
                : color;
        }

        public static Hashtable GetActionColors(IStiElement element)
        {
            var colors = new Hashtable();
            var controlStyle = StiDashboardStyleHelper.GetControlStyle(element);
            var isDarkStyle = StiDashboardStyleHelper.IsDarkStyle(element);

            colors["foreColor"] = StiReportHelper.GetHtmlColor(StiDashboardStyleHelper.GetForeColor(element, controlStyle.ForeColor));

            if (element is IStiDashboard)
                colors["backColor"] = StiReportHelper.GetHtmlColor(FixColor(StiDashboardStyleHelper.GetDashboardBackColor(element as IStiDashboard, true)));
            else
                colors["backColor"] = StiReportHelper.GetHtmlColor(FixColor(StiDashboardStyleHelper.GetBackColor(element, controlStyle.BackColor)));

            colors["glyphColor"] = StiReportHelper.GetHtmlColor(controlStyle.GlyphColor);

            colors["selectedForeColor"] = StiReportHelper.GetHtmlColor(controlStyle.SelectedForeColor);
            colors["selectedBackColor"] = StiReportHelper.GetHtmlColor(controlStyle.SelectedBackColor);
            colors["selectedGlyphColor"] = StiReportHelper.GetHtmlColor(controlStyle.SelectedGlyphColor);

            colors["hotForeColor"] = StiReportHelper.GetHtmlColor(controlStyle.HotForeColor);
            colors["hotBackColor"] = StiReportHelper.GetHtmlColor(controlStyle.HotBackColor);
            colors["hotGlyphColor"] = StiReportHelper.GetHtmlColor(controlStyle.HotGlyphColor);

            colors["hotSelectedForeColor"] = StiReportHelper.GetHtmlColor(controlStyle.HotSelectedForeColor);
            colors["hotSelectedBackColor"] = StiReportHelper.GetHtmlColor(controlStyle.HotSelectedBackColor);
            colors["hotSelectedGlyphColor"] = StiReportHelper.GetHtmlColor(controlStyle.HotSelectedGlyphColor);

            if (element is IStiDashboard && isDarkStyle)
            {
                if (StiDashboardStyleHelper.GetStyle(element) == StiElementStyleIdent.DarkTurquoise)
                {
                    colors["selectedBackColor"] = StiReportHelper.GetHtmlColor(StiColor.Get("113344"));
                    colors["hotBackColor"] = StiReportHelper.GetHtmlColor(StiColor.Get("235e6d"));
                    colors["hotSelectedBackColor"] = StiReportHelper.GetHtmlColor(StiColor.Get("f0621e"));
                }
                else
                {
                    colors["selectedBackColor"] = StiReportHelper.GetHtmlColor(StiColor.Get("414141"));
                    colors["hotBackColor"] = StiReportHelper.GetHtmlColor(StiColor.Get("1f1f1f"));
                    colors["hotSelectedBackColor"] = StiReportHelper.GetHtmlColor(StiColor.Get("494949"));
                }
            }

            colors["separatorColor"] = StiReportHelper.GetHtmlColor(controlStyle.SeparatorColor);
            colors["isDarkStyle"] = isDarkStyle;
            colors["styleName"] = StiDashboardStyleHelper.GetStyle(element).ToString();

            return colors;
        }

        public static Hashtable GetDashboardInteraction(object element)
        {
            Hashtable interactionObject = new Hashtable();
            var dashboardInteraction = (element as IStiElementInteraction)?.DashboardInteraction;

            if (dashboardInteraction != null)
            {
                interactionObject["ident"] = dashboardInteraction.Ident;
                interactionObject["onHover"] = dashboardInteraction.OnHover;
                interactionObject["onClick"] = dashboardInteraction.OnClick;
                interactionObject["hyperlinkDestination"] = dashboardInteraction.HyperlinkDestination;
                interactionObject["toolTip"] = dashboardInteraction.ToolTip;
                interactionObject["toolTipStyles"] = GetToolTipStyles(element);
                interactionObject["hyperlink"] = GetHyperlinkText(dashboardInteraction.Hyperlink, element);
                interactionObject["drillDownPageKey"] = dashboardInteraction.DrillDownPageKey;

                var drillDownParameters = dashboardInteraction.GetDrillDownParameters();
                if (drillDownParameters != null)
                {
                    interactionObject["drillDownParameters"] = new ArrayList();
                    drillDownParameters.ForEach(p =>
                    {
                        var drillDownParameter = new Hashtable();
                        drillDownParameter["name"] = p.Name;
                        drillDownParameter["expression"] = p.Expression;
                        ((ArrayList)interactionObject["drillDownParameters"]).Add(drillDownParameter);
                    });
                }

                if (dashboardInteraction is IStiAllowUserColumnSelectionDashboardInteraction)
                {
                    interactionObject["allowUserColumnSelection"] = ((IStiAllowUserColumnSelectionDashboardInteraction)dashboardInteraction).AllowUserColumnSelection;
                }

                if (dashboardInteraction is IStiAllowUserSortingDashboardInteraction)
                {
                    interactionObject["allowUserSorting"] = ((IStiAllowUserSortingDashboardInteraction)dashboardInteraction).AllowUserSorting;
                }

                if (dashboardInteraction is IStiAllowUserFilteringDashboardInteraction)
                {
                    interactionObject["allowUserFiltering"] = ((IStiAllowUserFilteringDashboardInteraction)dashboardInteraction).AllowUserFiltering;
                }

                if (dashboardInteraction is IStiAllowUserDrillDownDashboardInteraction)
                {
                    interactionObject["allowUserDrillDown"] = ((IStiAllowUserDrillDownDashboardInteraction)dashboardInteraction).AllowUserDrillDown;
                }

                if (dashboardInteraction is IStiInteractionLayout)
                {
                    interactionObject["showFullScreenButton"] = ((IStiInteractionLayout)dashboardInteraction).ShowFullScreenButton;
                    interactionObject["showSaveButton"] = ((IStiInteractionLayout)dashboardInteraction).ShowSaveButton;
                    interactionObject["showViewDataButton"] = ((IStiInteractionLayout)dashboardInteraction).ShowViewDataButton;
                }

                if (dashboardInteraction is IStiTableDashboardInteraction)
                {
                    interactionObject["drillDownFiltered"] = ((IStiTableDashboardInteraction)dashboardInteraction).DrillDownFiltered;
                    interactionObject["fullRowSelect"] = ((IStiTableDashboardInteraction)dashboardInteraction).FullRowSelect;
                }

                if (dashboardInteraction is IStiChartDashboardInteraction)
                {
                    interactionObject["viewsState"] = ((IStiChartDashboardInteraction)dashboardInteraction).ViewsState;
                }

                var drillDownElement = element as IStiDrillDownElement;
                if (drillDownElement != null)
                {
                    interactionObject["drillDownCurrentLevel"] = drillDownElement.DrillDownCurrentLevel;
                    interactionObject["drillDownLevelCount"] = drillDownElement.DrillDownLevelCount;
                    interactionObject["drillDownFilters"] = StiDataFiltersHelper.GetDrillDownFilters(drillDownElement);
                    interactionObject["drillDownFiltersList"] = StiDataFiltersHelper.GetDrillDownFiltersList(drillDownElement);
                }
            }

            return interactionObject;
        }

        private static Hashtable GetToolTipStyles(object element)
        {
            if (element is IStiIndicatorElement)
            {
                var indicatorStyle = StiDashboardStyleHelper.GetIndicatorStyle(element as IStiIndicatorElement);
                return new Hashtable()
                {
                    ["border"] = GetBorderJson(indicatorStyle.ToolTipBorder),
                    ["brush"] = GetBrushStr(indicatorStyle.ToolTipBrush),
                    ["textBrush"] = GetBrushStr(indicatorStyle.ToolTipTextBrush),
                    ["cornerRadius"] = GetCornerRadius(indicatorStyle.ToolTipCornerRadius)
                };
            }
            else if (element is IStiChartElement)
            {
                var chartElement = element as IStiChartElement;
                var styleIdent = StiDashboardStyleHelper.GetStyle(chartElement);

                if (styleIdent == StiElementStyleIdent.Custom)
                {
                    var chartStyle = chartElement.Report.Styles.ToList().FirstOrDefault(s => s is StiChartStyle && s.Name?.ToLowerInvariant() == chartElement.CustomStyleName?.ToLowerInvariant()) as StiChartStyle;
                    if (chartStyle != null)
                    {
                        return new Hashtable()
                        {
                            ["border"] = GetBorderJson(chartStyle.ToolTipBorder),
                            ["brush"] = GetBrushStr(chartStyle.ToolTipBrush),
                            ["textBrush"] = GetBrushStr(chartStyle.ToolTipTextBrush),
                            ["cornerRadius"] = GetCornerRadius(chartStyle.ToolTipCornerRadius)
                        };
                    }
                }
                else
                {
                    var chartStyleCore = StiDashboardStyleHelper.GetChartStyle(element as IStiChartElement)?.Core;
                    if (chartStyleCore != null)
                    {
                        return new Hashtable()
                        {
                            ["border"] = GetBorderJson(chartStyleCore.ToolTipBorder),
                            ["brush"] = GetBrushStr(chartStyleCore.ToolTipBrush),
                            ["textBrush"] = GetBrushStr(chartStyleCore.ToolTipTextBrush),
                            ["cornerRadius"] = GetCornerRadius(chartStyleCore.ToolTipCornerRadius)
                        };
                    }
                }
            }
            else if (element is IStiRegionMapElement)
            {
                var mapStyle = StiDashboardStyleHelper.GetMapStyle(element as IStiRegionMapElement);
                return new Hashtable()
                {
                    ["border"] = GetBorderJson(mapStyle.ToolTipBorder),
                    ["brush"] = GetBrushStr(mapStyle.ToolTipBrush),
                    ["textBrush"] = GetBrushStr(mapStyle.ToolTipTextBrush),
                    ["cornerRadius"] = GetCornerRadius(mapStyle.ToolTipCornerRadius)
                };
            }

            return null;
        }

        public static Hashtable GetShadow(IStiElement element)
        {
            var shadow = (element as IStiSimpleShadow)?.Shadow;
            if (shadow != null)
            {
                var shadowObject = new Hashtable();
                shadowObject["visible"] = shadow.Visible;
                shadowObject["color"] = StiReportHelper.GetHtmlColor(shadow.Color);
                shadowObject["location"] = $"{shadow.Location.X};{shadow.Location.Y}";
                shadowObject["size"] = shadow.Size.ToString();

                return shadowObject;
            }
            return null;
        }

        public static Hashtable GetCornerRadius(StiCornerRadius cornerRadius)
        {
            if (cornerRadius != null)
            {
                var cornerRadiusObject = new Hashtable();
                cornerRadiusObject["topLeft"] = cornerRadius.TopLeft;
                cornerRadiusObject["topRight"] = cornerRadius.TopRight;
                cornerRadiusObject["bottomRight"] = cornerRadius.BottomRight;
                cornerRadiusObject["bottomLeft"] = cornerRadius.BottomLeft;

                return cornerRadiusObject;
            }

            return null;
        }

        private static string GetHyperlinkText(string hyperlink, object element)
        {
            try
            {
                if (!string.IsNullOrEmpty(hyperlink) && hyperlink.Contains("{") && hyperlink.Contains("}"))
                {
                    var newHyperlink = StiReportParser.Parse(hyperlink, element as StiComponent, false);

                    if (hyperlink == newHyperlink)
                        StiReportParser.CleanCache((element as StiComponent)?.Report.Key);

                    if (!string.IsNullOrEmpty(newHyperlink))
                        return newHyperlink;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return hyperlink;
        }

        public static string Format(IStiControlElement element, object value)
        {
            if (value != null && element != null)
            {
                if (value is DateTime && !(element.TextFormat is StiDateFormatService))
                    return ((DateTime)value).ToShortDateString();
                else
                    return element.TextFormat?.Format(value);
            }
            return string.Empty;
        }

        public static Dictionary<string, object> GetConstants(string value, ArrayList cells)
        {
            var constants = new Dictionary<string, object>();
            constants.Add("Value", value);
            if (cells != null)
            {
                foreach (Hashtable cell in cells)
                {
                    var dataColumnName = $"Row.{StiCodeDomSerializator.ReplaceSymbols(cell["owningColumnName"] != null ? cell["owningColumnName"] as string : "")}";
                    constants[dataColumnName] = cell["value"] as string;
                }
            }
            return constants;
        }

        internal static void ParseDashboardDrillDownParameters(ArrayList drillDownParameters, StiReport report)
        {
            foreach (Hashtable p1 in drillDownParameters)
            {
                if (p1 != null)
                {
                    var constants = GetConstants(p1["value"] as string, p1["rowCels"] as ArrayList);
                    IStiReportComponent element = report.Pages.ToList().Select(p => p.GetComponents().ToList().FirstOrDefault(c => c.Guid == p1["tableKey"] as string))?.FirstOrDefault();

                    var parameters = p1["parameters"] as ArrayList;
                    if (parameters != null && element != null)
                    {
                        foreach (Hashtable p2 in parameters)
                        {
                            p2["value"] = StiReportParser.Parse(p2["value"] as string, element, false, constants);
                        }
                    }
                }
            }
        }

        public static Hashtable GetDashboardWatermark(IStiElement element)
        {
            var watermark = (element as IStiDashboardWatermark)?.DashboardWatermark;
            if (watermark != null)
            {
                var jsWatermark = new Hashtable();
                if (watermark.TextEnabled)
                {
                    jsWatermark["textEnabled"] = watermark.TextEnabled;
                    jsWatermark["text"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(watermark.Text));
                    jsWatermark["textFont"] = GetFontJson(watermark.TextFont);
                    jsWatermark["textColor"] = StiReportHelper.GetHtmlColor(watermark.TextColor);
                    jsWatermark["textAngle"] = watermark.TextAngle;
                }

                if (watermark.ImageEnabled)
                {
                    jsWatermark["imageEnabled"] = watermark.ImageEnabled;
                    jsWatermark["image"] = watermark.Image != null ? ImageToBase64(StiImageConverter.ImageToBytes(watermark.Image)) : string.Empty;
                    jsWatermark["imageSize"] = watermark.Image != null ? $"{watermark.Image.Width};{watermark.Image.Height}" : "0;0";
                    jsWatermark["imageAlignment"] = watermark.ImageAlignment;
                    jsWatermark["imageTransparency"] = watermark.ImageTransparency;
                    jsWatermark["imageMultipleFactor"] = watermark.ImageMultipleFactor;
                    jsWatermark["imageAspectRatio"] = watermark.ImageAspectRatio;
                    jsWatermark["imageStretch"] = watermark.ImageStretch;
                    jsWatermark["imageTiling"] = watermark.ImageTiling;
                }
                if (watermark.WeaveEnabled)
                {
                    jsWatermark["weaveEnabled"] = watermark.WeaveEnabled;
                    jsWatermark["weaveMajorIcon"] = watermark.WeaveMajorIcon;
                    jsWatermark["weaveMajorColor"] = StiReportHelper.GetHtmlColor(watermark.WeaveMajorColor);
                    jsWatermark["weaveMajorSize"] = watermark.WeaveMajorSize;
                    jsWatermark["weaveMinorIcon"] = watermark.WeaveMinorIcon;
                    jsWatermark["weaveMinorColor"] = StiReportHelper.GetHtmlColor(watermark.WeaveMinorColor);
                    jsWatermark["weaveMinorSize"] = watermark.WeaveMinorSize;
                    jsWatermark["weaveDistance"] = watermark.WeaveDistance;
                    jsWatermark["weaveAngle"] = watermark.WeaveAngle;
                };

                if (watermark.WeaveEnabled && (watermark.WeaveMajorIcon != null || watermark.WeaveMinorIcon != null))
                {
                    GetWeaveWatermarkImages(watermark, jsWatermark);
                }
                return jsWatermark;
            }
            return null;
        }

        private static void GetWeaveWatermarkImages(StiAdvancedWatermark watermark, Hashtable jsWatermark)
        {
            var majorSize = 5 * watermark.WeaveMajorSize;
            var minorSize = 5 * watermark.WeaveMinorSize;

            if (watermark.WeaveMajorIcon != null)
            {
                var weaveMajorImage = StiFontIconsHelper.ConvertFontIconToImage(watermark.WeaveMajorIcon.Value, watermark.WeaveMajorColor, majorSize, majorSize);
                jsWatermark["weaveMajorImage"] = new Hashtable()
                {
                    ["width"] = weaveMajorImage.Width,
                    ["height"] = weaveMajorImage.Height,
                    ["text"] = StiFontIconsHelper.GetContent(watermark.WeaveMajorIcon),
                    ["size"] = watermark.WeaveMajorSize,
                    ["color"] = StiReportHelper.GetHtmlColor(watermark.WeaveMajorColor)
                };
            }

            if (watermark.WeaveMinorIcon != null)
            {
                var weaveMinorImage = StiFontIconsHelper.ConvertFontIconToImage(watermark.WeaveMinorIcon.Value, watermark.WeaveMinorColor, minorSize, minorSize);
                jsWatermark["weaveMinorImage"] = new Hashtable()
                {
                    ["width"] = weaveMinorImage.Width,
                    ["height"] = weaveMinorImage.Height,
                    ["text"] = StiFontIconsHelper.GetContent(watermark.WeaveMinorIcon),
                    ["size"] = watermark.WeaveMinorSize,
                    ["color"] = StiReportHelper.GetHtmlColor(watermark.WeaveMinorColor)
                };
            }
        }

        private static string ImageToBase64(byte[] image)
        {
            string mimeType = "data:image/png;base64,";

            if (Report.Helpers.StiImageHelper.IsMetafile(image))
            {
                mimeType = "data:image/x-wmf;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsBmp(image))
            {
                mimeType = "data:image/bmp;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsJpeg(image))
            {
                mimeType = "data:image/jpeg;base64,";
            }
            else if (Report.Helpers.StiImageHelper.IsGif(image))
            {
                mimeType = "data:image/gif;base64,";
            }
            else if (Base.Helpers.StiSvgHelper.IsSvg(image))
            {
                mimeType = "data:image/svg+xml;base64,";
            }

            return mimeType + Convert.ToBase64String(image);
        }
        #endregion
    }
}
