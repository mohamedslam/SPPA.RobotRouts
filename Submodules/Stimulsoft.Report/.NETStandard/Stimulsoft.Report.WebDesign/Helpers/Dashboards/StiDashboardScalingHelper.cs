#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dashboard.Helpers;
using Stimulsoft.Report.Web.Helpers.Dashboards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Stimulsoft.Report.Web
{
    internal class StiDashboardScalingHelper
    {
        public static void ApplyScalingToDashboard(StiPage dashboard, double prevWidth, double prevHeight)
        {
            var components = dashboard.GetComponents().ToList().Where(c => c is IStiElement);
            var factorX = dashboard.Width / prevWidth;

            var rects = components
                .ToDictionary(e => e as StiComponent, e => Round(e.GetPaintRectangle(false, false)));

            //Adds all top and bottom coordinates of the elements to a list
            var yy = GetAllLocations(components, rects, prevHeight);

            //Creates a list of the all element positions
            var bands = new List<StiRangeBand>();
            for (var index = 0; index < yy.Count - 1; index++)
            {
                bands.Add(new StiRangeBand(yy[index], yy[index + 1]));
            }

            //Detects all fixed bands
            foreach (var component in components.Where(e => (e is IStiFixedHeightElement) && (e as IStiFixedHeightElement).IsFixedHeight))
            {
                bands.Where(b => b.Intersect(rects[component])).ToList().ForEach(c => c.IsFixed = true);
            }

            var totalFixedHeight = bands.Where(b => b.IsFixed).Sum(b => b.Height);
            var factorY = (dashboard.Height - totalFixedHeight) / (prevHeight - totalFixedHeight);

            //Calculates a new position for each bands
            var posY = 0;
            foreach (var band in bands)
            {
                band.Top = posY;

                var bandHeight = Round(band.IsFixed ? band.OriginalHeight : band.OriginalHeight * factorY);
                band.Bottom = band.Top + bandHeight;

                posY += bandHeight;
            }

            //Top-up filter elements
            components = TopUpFilterElements(components);

            foreach (var component in components)
            {
                if (component.Width == 0 || component.Height == 0) continue;

                var left = Round(rects[component].Left * factorX);
                var right = Round(rects[component].Right * factorX);
                var top = bands.FirstOrDefault(b => b.OriginalTop == rects[component].Top).Top;
                var bottom = bands.FirstOrDefault(b => b.OriginalBottom == rects[component].Bottom).Bottom;

                component.ClientRectangle = new RectangleD(left, top, right - left, bottom - top);
            }
        }

        private static int GetLevel(IStiFilterElement element, List<IStiFilterElement> elements)
        {
            if (StiCrossLinkedFilterHelper.IsCrossLinkedFilter(element))
                return 0;

            var level = 0;
            while (true)
            {
                if (string.IsNullOrWhiteSpace(element.GetParentKey()))
                    return level;

                element = elements.FirstOrDefault(e => e.GetKey() == element.GetParentKey());
                if (element == null) break;
                level++;
            }
            return level;
        }

        private static int Round(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private static Rectangle Round(RectangleD rect)
        {
            return new Rectangle(Round(rect.Left), Round(rect.Top), Round(rect.Width), Round(rect.Height));
        }

        private static List<int> GetAllLocations(IEnumerable<StiComponent> components, Dictionary<StiComponent, Rectangle> rects, double height)
        {
            var yy = new List<int>();
            foreach (var component in components)
            {
                yy.Add(Round(rects[component].Top));
                yy.Add(Round(rects[component].Bottom));
            }

            yy.Add(0);
            yy.Add(Round(height));

            return yy.Distinct().OrderBy(y => y).ToList();
        }

        private static IEnumerable<StiComponent> TopUpFilterElements(IEnumerable<StiComponent> elements)
        {
            var filterElements = elements
                .Where(e => e is IStiFilterElement)
                .Cast<IStiFilterElement>()
                .ToList();

            filterElements = filterElements
                .OrderBy(f => GetLevel(f, filterElements))
                .ToList();

            var otherElements = elements.Where(e => !(e is IStiFilterElement)).ToList();

            var list = new List<StiComponent>();

            list.AddRange(filterElements.Cast<StiComponent>().ToList());
            list.AddRange(otherElements);

            return list;
        }

        internal static double MakeHorizontalSpacingEqual(StiPage dashboard)
        {
            var rects = dashboard.Components.ToList().Select(c => Round(c.GetPaintRectangle(false, false)));

            var leftMargin = rects.Min(r => r.Left);
            var rightMargin = rects.Max(r => r.Right);

            if (leftMargin < 0)
                leftMargin = 0;

            rightMargin += leftMargin;

            return StiAlignHelper.AlignToGrid(rightMargin, dashboard.Report.Info.GridSize, dashboard.Report.Info.AlignToGrid);
        }

        internal static double MakeVerticalSpacingEqual(StiPage dashboard)
        {
            var rects = dashboard.Components.ToList().Select(c => Round(c.GetPaintRectangle(false, false)));

            var topMargin = rects.Min(r => r.Top);
            var bottomMargin = rects.Max(r => r.Bottom);

            if (topMargin < 0)
                topMargin = 0;

            bottomMargin += topMargin;

            return StiAlignHelper.AlignToGrid(bottomMargin, dashboard.Report.Info.GridSize, dashboard.Report.Info.AlignToGrid);
        }

        internal static void ChangeDashboardSettingsValue(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var commandName = param["commandName"] as string;
            var width = StiReportEdit.StrToDouble(param["width"] as string);
            var height = StiReportEdit.StrToDouble(param["height"] as string);
            var scaleContent = Convert.ToBoolean(param["scaleContent"]);
            var contentAlignment = (StiDashboardContentAlignment)Enum.Parse(typeof(StiDashboardContentAlignment), param["contentAlignment"] as string);
            var page = report.Pages[param["dashboardName"] as string];
            var dashboard = page as IStiDashboard;

            if (dashboard != null)
            {
                var prevWidth = page.Width;
                var prevHeight = page.Height;

                switch (commandName)
                {
                    case "ChangeWidth":
                        dashboard.Width = width;
                        ResizeDashboard(page, true, prevWidth, prevHeight, width, height, scaleContent, callbackResult);
                        break;

                    case "ChangeHeight":
                        dashboard.Height = height;
                        ResizeDashboard(page, true, prevWidth, prevHeight, width, height, scaleContent, callbackResult);
                        break;

                    case "MakeHorSpacingEqual":
                        if (page.Components.Count > 0)
                        {
                            width = MakeHorizontalSpacingEqual(page);
                            callbackResult["newWidth"] = StiReportEdit.DoubleToStr(width);
                        }
                        ResizeDashboard(page, false, prevWidth, prevHeight, width, height, scaleContent, callbackResult);
                        break;

                    case "MakeVertSpacingEqual":
                        if (page.Components.Count > 0)
                        {
                            height = MakeVerticalSpacingEqual(page);
                            callbackResult["newHeight"] = StiReportEdit.DoubleToStr(height);
                        }
                        ResizeDashboard(page, false, prevWidth, prevHeight, width, height, scaleContent, callbackResult);
                        break;

                    case "ChangeContentAlignment":
                        dashboard.ContentAlignment = contentAlignment;
                        callbackResult["properties"] = StiReportEdit.GetAllProperties(page);
                        break;
                }
            }
        }

        private static void ResizeDashboard(StiPage dashboard, bool allowRescale, double prevWidth, double prevHeight, double width, double height, bool scaleContent, Hashtable callbackResult)
        {
            var isChanged = false;

            if (prevWidth != width)
            {
                width = Math.Max(50, width);
                width = Math.Min(10000, width);

                dashboard.Width = width;
                isChanged = true;

                callbackResult["newWidth"] = StiReportEdit.DoubleToStr(width);
            }

            if (prevHeight != height)
            {
                height = Math.Max(50, height);
                height = Math.Min(10000, height);

                dashboard.Height = height;
                isChanged = true;

                callbackResult["newHeight"] = StiReportEdit.DoubleToStr(height);
            }

            if (isChanged)
            {
                if (scaleContent && allowRescale)
                {
                    ApplyScalingToDashboard(dashboard, prevWidth, prevHeight);
                }
                
                var rebuildProps = StiReportEdit.GetPropsRebuildPage(dashboard.Report, dashboard);
                AddSvgContents(dashboard, rebuildProps);

                callbackResult["properties"] = StiReportEdit.GetAllProperties(dashboard);
                callbackResult["rebuildProps"] = rebuildProps;
            }
        }

        private static void AddSvgContents(StiPage page, Hashtable rebuildProps)
        {
            page.GetComponents().ToList().ForEach(comp =>
            {
                if (rebuildProps[comp.Name] != null)
                    ((Hashtable)rebuildProps[comp.Name])["svgContent"] = StiReportEdit.GetSvgContent(comp);
            });
        }
    }
}