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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Meters;
using Stimulsoft.Report;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Surface;
using System.Drawing;
using Stimulsoft.Report.Images;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
using Stimulsoft.System.Windows.Forms.PropertyGridInternal;
#else
using System.Windows.Forms;
using System.Windows.Forms.PropertyGridInternal;
#endif

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Base.Design
{
    /// <summary>
    /// Class describes the panel of properties StiPropertyGrid.
    /// </summary>
    public class StiPropertiesTab : PropertiesTab
    {
        #region class OrderData
        private class OrderData : IComparable
        {
            #region IComparable
            public int CompareTo(object obj)
            {
                var secondValue = obj as OrderData;

                if (secondValue.CategoryPosition == this.CategoryPosition)
                {
                    if (secondValue.position == this.position)
                        return string.CompareOrdinal(this.Name, secondValue.Name);
                    else
                        return -secondValue.position.CompareTo(this.position);
                }
                return -secondValue.CategoryPosition.CompareTo(this.CategoryPosition);
            }
            #endregion

            #region Fields
            private int position;
            #endregion

            #region Properties
            public int CategoryPosition { get; set; }

            public string Name { get; }

            public string CategoryName { get; }
            #endregion

            public OrderData(string categoryName, string name, int position, int categoryPosition)
            {
                this.position = position;
                this.CategoryPosition = categoryPosition;
                this.Name = name;
                this.CategoryName = categoryName;
            }
        }
        #endregion

        #region class CategoryData
        private class CategoryData : IComparable
        {
            #region IComparable
            public int CompareTo(object obj)
            {
                var secondValue = obj as CategoryData;

                if (secondValue.position == this.position)
                    return string.CompareOrdinal(this.CategoryName, secondValue.CategoryName);
                else
                    return -secondValue.position.CompareTo(this.position);
            }
            #endregion

            #region Fields
            private int position;
            #endregion

            #region Properties
            public string CategoryName { get; }
            #endregion

            public CategoryData(string categoryName, int position)
            {
                this.CategoryName = categoryName;
                this.position = position;
            }
        }
        #endregion

        #region Properties
        internal PropertyGrid PropertyGrid { get; set; }

        /// <summary>
        /// This property let to show in WPF designer properties of components, which unavailable in WPF.
        /// </summary>            
        public static bool ShowAllPropertiesInWpf { get; set; }

        public override Bitmap Bitmap => StiReportImages.PropertyGrid.Properties() as Bitmap;

        public override string TabName => "Properties Tab";
        #endregion

        #region Methods
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            var pdc = GetProperties(component, attributes);

            if (this.PropertyGrid.SelectedObjects != null &&
                this.PropertyGrid.SelectedObjects.Length == 1)
            {
                return pdc;
            }

            // Only MultiSelected Components
            var orderedProperties = new List<Tuple<string, string>>();

            foreach (PropertyDescriptor propertyDescriptor in pdc)
            {
                var descriptor = propertyDescriptor as StiPropertyDescriptor;

                if (descriptor != null)
                    orderedProperties.Add(new Tuple<string, string>(descriptor.LocalizedName, descriptor.Name));

                else
                    orderedProperties.Add(new Tuple<string, string>(propertyDescriptor.Name, propertyDescriptor.Name));
            }

            orderedProperties.Sort();

            var propertyNames = new List<string>();
            foreach (var property in orderedProperties)
            {
                propertyNames.Add(property.Item2);
            }

            return pdc.Sort(propertyNames.ToArray());
        }

        public override bool CanExtend(object extendee)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            return GetPropertiesFromComponent(component, this.PropertyGrid.SelectedObjects, this.PropertyGrid.PropertySort, StiGuiMode.Gdi);
        }

        public static PropertyDescriptorCollection GetPropertiesFromComponent(object component, object[] selectedObjects, PropertySort propertySort, StiGuiMode guiMode)
        {
            var properties = GetPropertiesFromComponentInternal(component, selectedObjects, propertySort, guiMode);

            StiOptions.Engine.GlobalEvents.InvokeGetProperties(component, new Report.Events.StiGetPropertiesEventArgs(selectedObjects, properties, propertySort, guiMode));

            return properties;
        }

        private static PropertyDescriptorCollection GetPropertiesFromComponentInternal(object component, object[] selectedObjects, PropertySort propertySort, StiGuiMode guiMode)
        {
            if (component is StiDataBindingsCollection)
                return ProcessDataBindingsProperties(component);

            /*if (component is IStiChartElement)
            {
                var dynMethod = component.GetType().GetMethod("CheckBrowsableProperties", BindingFlags.Public | BindingFlags.Instance);
                dynMethod.Invoke(component, new object[] { });
            }*/

            var properties = new List<StiPropertyDescriptor>();
            var props = TypeDescriptor.GetProperties(component);

            #region Fill PropertyDescriptor collection
            foreach (PropertyDescriptor prop in props)
            {
                if (CheckAllowedProperty(prop, component)) continue;
                if (CheckPropertyLevel(prop)) continue;
                if (CheckEngineVersion(prop, component)) continue;
                if (CheckGuiMode(prop, guiMode)) continue;

                if (StiOptions.Designer.UseGlobalizationManager)
                {
                    if (prop.Name == "Text")
                    {
                        var text = component as StiText;
                        if (text != null && !string.IsNullOrWhiteSpace(text.GlobalizedName) && text.Report != null &&
                            text.Report.GlobalizationManager != null && !(text.Report.GlobalizationManager is StiNullGlobalizationManager))
                        {
                            try
                            {
                                var strObject = text.Report.GlobalizationManager.GetString(text.GlobalizedName);
                                if (strObject != null) continue;
                            }
                            catch
                            {
                            }
                        }
                    }


                    if (prop.Name == "Image")
                    {
                        var image = component as StiImage;
                        if (image != null && !string.IsNullOrWhiteSpace(image.GlobalizedName) && image.Report != null &&
                            image.Report.GlobalizationManager != null && !(image.Report.GlobalizationManager is StiNullGlobalizationManager))
                        {
                            try
                            {
                                var imageObject = image.Report.GlobalizationManager.GetObject(image.GlobalizedName);
                                if (imageObject != null) continue;
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                ProcessBrowsableAttribute(component, prop, properties);
            }
            #endregion

            properties = ProcessSignatureProperties(component, properties);
            properties = ProcessAllowApplyStyle(component, properties);
            properties = ProcessReportProperties(component, properties);
            properties = ProcessStyleProperties(component, properties);
            properties = ProcessCrossProperties(component, properties);
            properties = ProcessDashboardElementProperties(selectedObjects, component, properties);

            if (selectedObjects != null && selectedObjects.Length > 1)
            {
                properties = ProcessMultiProperties(component, properties);
                return new PropertyDescriptorCollection(properties.OrderBy(x => x.LocalizedName).ToArray());
            }

            else
                return SortProperties(component, propertySort, new PropertyDescriptorCollection(properties.ToArray()));
        }

        private static List<StiPropertyDescriptor> ProcessMultiProperties(object component, List<StiPropertyDescriptor> properties)
        {
            if (component is StiChart)
            {
                RemoveProperty("Series", properties);
                RemoveProperty("Labels", properties);
            }

            return properties;
        }

        private static List<StiPropertyDescriptor> ProcessAllowApplyStyle(object component,
        List<StiPropertyDescriptor> properties)
        {
            #region StiGauge
            var gauge = component as StiGauge;
            if (gauge != null && gauge.AllowApplyStyle)
            {
                RemoveProperty("Brush", properties);
                RemoveProperty("Border", properties);
            }
            #endregion

            return properties;
        }

        private static List<StiPropertyDescriptor> ProcessSignatureProperties(object component,
            List<StiPropertyDescriptor> properties)
        {
            var signature = component as StiElectronicSignature;
            if (signature != null)
            {
                switch (signature.Mode)
                {
                    case StiSignatureMode.Type:
                        RemoveProperty("Image", properties);
                        RemoveProperty("Text", properties);
                        RemoveProperty("Draw", properties);
                        break;

                    case StiSignatureMode.Draw:
                        RemoveProperty("Type", properties);
                        break;
                }
            }

            return properties;
        }

        private static List<StiPropertyDescriptor> ProcessReportProperties(object component,
            List<StiPropertyDescriptor> properties)
        {
            #region StiReport
            var report = component as StiReport;
            if (report != null && report.Pages.ContainsOnlyDashboards)
            {
                RemoveProperty("CacheAllData", properties);
                RemoveProperty("ConvertNulls", properties);
                RemoveProperty("ReportCacheMode", properties);
                RemoveProperty("RetrieveOnlyUsedData", properties);
                RemoveProperty("CachedTotals", properties);
                RemoveProperty("Collate", properties);
                RemoveProperty("EngineVersion", properties);
                RemoveProperty("NumberOfPass", properties);
                RemoveProperty("ReportUnit", properties);
                RemoveProperty("ScriptLanguage", properties);
                RemoveProperty("StopBeforePage", properties);
                RemoveProperty("PreviewMode", properties);
                RemoveProperty("PrinterSettings", properties);
            }
            #endregion

            return properties;
        }

        private static List<StiPropertyDescriptor> ProcessCrossProperties(object component,
            List<StiPropertyDescriptor> properties)
        {
            #region StiCrossSummary
            var crossSummary = component as StiCrossSummary;
            if (crossSummary != null)
            {
                if (crossSummary.Summary == StiSummaryType.Image)
                {
                    RemoveProperty("AllowHtmlTags", properties);
                    RemoveProperty("Angle", properties);
                    RemoveProperty("Font", properties);
                    RemoveProperty("HideZeros", properties);
                    RemoveProperty("HorAlignment", properties);
                    RemoveProperty("VertAlignment", properties);
                    RemoveProperty("Margins", properties);
                    RemoveProperty("TextFormat", properties);
                    RemoveProperty("TextQuality", properties);
                    RemoveProperty("TextBrush", properties);
                    RemoveProperty("TextOptions", properties);
                    RemoveProperty("WordWrap", properties);
                }
                else
                {
                    RemoveProperty("AspectRatio", properties);
                    RemoveProperty("ImageHorAlignment", properties);
                    RemoveProperty("ImageVertAlignment", properties);
                    RemoveProperty("Stretch", properties);
                }
            }
            #endregion

            #region StiCrossTab
            var crossTab = component as StiCrossTab;
            if (crossTab != null)
            {
                if (!crossTab.Wrap)
                    RemoveProperty("WrapGap", properties);

                return properties;
            }
            #endregion

            return properties;
        }

        #region DBS
        private static List<StiPropertyDescriptor> ProcessDashboardElementProperties(object[] selectedObjects, object component,
            List<StiPropertyDescriptor> properties)
        {
            if (component is IStiGaugeElement)
            {
                if (((IStiGaugeElement)component).CalculationMode == StiGaugeCalculationMode.Auto)
                {
                    RemoveProperty("Minimum", properties);
                    RemoveProperty("Maximum", properties);
                }
            }
            else if (component is IStiButtonElement buttonElement)
            {
                if (buttonElement.Type == StiButtonType.Button)
                {
                    RemoveProperty("Group", properties);
                    RemoveProperty("Checked", properties);
                }
                else if (buttonElement.Type == StiButtonType.CheckBox)
                {
                    RemoveProperty("Group", properties);
                }
            }
            else if (component is IStiIndicatorElement)
            {
                bool showIconAlignment = true;
                bool showTargetMode = false;

                var indicator = (IStiIndicatorElement)component;

                if (indicator.GetSeries() != null || indicator.DataMode == StiDataMode.ManuallyEnteringData)
                    showIconAlignment = false;

                if (indicator.GetTarget() != null || indicator.DataMode == StiDataMode.ManuallyEnteringData)
                    showTargetMode = true;

                if (!showIconAlignment)
                    RemoveProperty("IconAlignment", properties);

                if (!showTargetMode)
                    RemoveProperty("TargetMode", properties);
            }
            else if (component is StiChart)
            {
                var chart = component as StiChart;

                if (!(chart.Area is IStiAxisArea))
                {
                    RemoveProperty("XAxis", properties);
                    RemoveProperty("XTopAxis", properties);
                    RemoveProperty("YAxis", properties);
                    RemoveProperty("YRightAxis", properties);
                }

                if (!(chart.Area is StiAxisArea))
                    RemoveProperty("ConstantLines", properties);
            }
            else if (component is IStiChartElement)
            {
                var chartElement = component as IStiChartElement;

                if (!chartElement.IsAxisAreaChart || chartElement.IsRibbonChart)
                {
                    RemoveProperty("ConstantLines", properties);
                    RemoveProperty("TrendLines", properties);
                }

                if (!chartElement.IsClusteredColumnChart3D)
                    RemoveProperty("SideBySide", properties);

                if (!(chartElement.IsLinesChart))
                    RemoveProperty("Marker", properties);

                if (!(chartElement.IsParetoChart))
                    RemoveProperty("ParetoSeriesColors", properties);

                if (!(chartElement.IsPie3dChart))
                    RemoveProperty("Options3D", properties);

                RemoveProperty("EndValues", properties);
                RemoveProperty("CloseValues", properties);
                RemoveProperty("LowValues", properties);
                RemoveProperty("HighValues", properties);
            }
            else if (component is IStiChartArea)
            {
                var chart = selectedObjects?.Where(o => o is IStiChartElement).FirstOrDefault();

                if (chart != null && !(chart as IStiChartElement).IsAxisAreaChart)
                {
                    RemoveProperty("ReverseHor", properties);
                    RemoveProperty("ReverseVert", properties);
                    RemoveProperty("GridLinesHor", properties);
                    RemoveProperty("GridLinesVert", properties);
                    RemoveProperty("InterlacingHor", properties);
                    RemoveProperty("InterlacingVert", properties);
                    RemoveProperty("XTopAxis", properties);
                    RemoveProperty("YRightAxis", properties);
                }

                if (chart != null && !((IStiChartElement)chart).IsAxisAreaChart && !((IStiChartElement)chart).IsRadarChart && !((IStiChartElement)chart).IsAxisAreaChart3D)
                {
                    RemoveProperty("XAxis", properties);
                    RemoveProperty("YAxis", properties);
                }
            }
            else if (component is IStiYChartAxis)
            {
                var chart = selectedObjects?.Where(o => o is IStiChartElement).FirstOrDefault();

                if ((chart as IStiChartElement).IsRadarChart)
                {
                    RemoveProperty("Title", properties);
                    RemoveProperty("StartFromZero", properties);
                }

                if ((chart as IStiChartElement).IsAxisAreaChart3D)
                {
                    RemoveProperty("Range", properties);
                    RemoveProperty("StartFromZero", properties);
                    RemoveProperty("Title", properties);
                }
            }
            else if (component is IStiXChartAxis)
            {
                var chart = selectedObjects?.Where(o => o is IStiChartElement).FirstOrDefault();

                if ((chart as IStiChartElement).IsRadarChart)
                {
                    RemoveProperty("Range", properties);
                    RemoveProperty("ShowEdgeValues", properties);
                    RemoveProperty("StartFromZero", properties);
                    RemoveProperty("Title", properties);
                }

                if ((chart as IStiChartElement).IsAxisAreaChart3D)
                {
                    RemoveProperty("Range", properties);
                    RemoveProperty("StartFromZero", properties);
                    RemoveProperty("Title", properties);
                }
            }
            else if (component is IStiChartAxisLabels)//XAxis Label
            {
                var chart = selectedObjects?.Where(o => o is IStiChartElement).FirstOrDefault();

                if ((chart as IStiChartElement).IsRadarChart)
                {
                    RemoveProperty("Angle", properties);
                    RemoveProperty("Placement", properties);
                    RemoveProperty("TextAlignment", properties);
                }

                if ((chart as IStiChartElement).IsAxisAreaChart3D)
                {
                    RemoveProperty("Angle", properties);
                    RemoveProperty("Step", properties);
                    RemoveProperty("Placement", properties);
                    RemoveProperty("TextAlignment", properties);
                }
            }
            else if (component is IStiChartLabels)
            {
                var chartLabels = component as IStiChartLabels;
                if (chartLabels.Position == StiChartLabelsPosition.TwoColumns)
                    RemoveProperty("AutoRotate", properties);
            }
            else if (component is IStiChartLegendLabels)
            {
                var IsPieOrDoughnutChart = selectedObjects?.All(c =>
                    c is IStiChartElement &&
                    (((IStiChartElement)c).IsPieChart || ((IStiChartElement)c).IsPie3dChart || ((IStiChartElement)c).IsDoughnutChart));

                if (!IsPieOrDoughnutChart.GetValueOrDefault(false))
                    RemoveProperty("ValueType", properties);
            }
            else if (component is IStiRegionMapElement)
            {
                var map = (IStiRegionMapElement)component;
                if (map.DataFrom == Report.Maps.StiMapSource.Manual)
                {
                    RemoveProperty("KeyMeter", properties);
                    RemoveProperty("NameMeter", properties);
                    RemoveProperty("ValueMeter", properties);
                    RemoveProperty("GroupMeter", properties);
                    RemoveProperty("ColorMeter", properties);
                }
            }
            else if (component is IStiSurface)
            {
                var dashboard = (IStiSurface)component;
                if (dashboard.IsDesktopSurfaceSelected)
                    RemoveProperty("DeviceWidth", properties);
            }
            else if (component is IStiPivotColumn pivotColumn && pivotColumn.ColumnsCount < 2)
            {
                RemoveProperty("Expand", properties);
            }
            else if (component is IStiPivotRow pivotRow && pivotRow.RowsCount < 2)
            {
                RemoveProperty("Expand", properties);
            }
            else if (component is StiAzureBlobStorageDatabase)
            {
                var databaseComponent = component as StiAzureBlobStorageDatabase;

                if (!databaseComponent.BlobContentType.Equals("CSV"))
                {
                    RemoveProperty(nameof(databaseComponent.CodePage), properties);
                    RemoveProperty(nameof(databaseComponent.Delimiter), properties);
                }

                if (!databaseComponent.BlobContentType.Equals("Excel"))
                    RemoveProperty(nameof(databaseComponent.FirstRowIsHeader), properties);
            }

            return properties;
        }
        #endregion

        private static List<StiPropertyDescriptor> ProcessStyleProperties(object component,
            List<StiPropertyDescriptor> properties)
        {
            if (StiOptions.Designer.PropertyGrid.ShowPropertiesWhichUsedFromStyles)
                return properties;

            if (component == null)
                return properties;

            #region StiChart
            var chart = component as StiChart;
            if (chart != null)
            {
                if (chart.AllowApplyStyle)
                    RemoveProperty("Brush", properties);
            }
            #endregion

            #region StiTable
            if (component is StiTable table)
            {
                if (!string.IsNullOrEmpty(table.ComponentStyle))
                {
                    RemoveProperty("Brush", properties);
                }

                if (table.TableStyleFX != null)
                {
                    RemoveProperty("Border", properties);
                    RemoveProperty("Brush", properties);
                    RemoveProperty("TextBrush", properties);
                }
            }
            #endregion

            #region StiTable Cells
            if (component is StiTableCell ||
                component is StiTableCellCheckBox ||
                component is StiTableCellImage ||
                component is StiTableCellRichText)
            {
                var parentTable = ((StiComponent)component).Parent as StiTable;
                if (parentTable != null && !string.IsNullOrEmpty(parentTable.ComponentStyle))
                {
                    RemoveProperty("Border", properties);
                    RemoveProperty("Brush", properties);
                    RemoveProperty("TextBrush", properties);
                }
            }
            #endregion

            #region StiComponent
            if (component is StiComponent)
            {
                var style = ((StiComponent)component).GetComponentStyle() as StiStyle;
                if (style == null)
                    return properties;

                if (style.AllowUseBorderFormatting && style.AllowUseBorderSides)
                    RemoveProperty("Border", properties);

                if (style.AllowUseBrush)
                    RemoveProperty("Brush", properties);

                if (style.AllowUseFont)
                    RemoveProperty("Font", properties);

                if (style.AllowUseHorAlignment)
                    RemoveProperty("HorAlignment", properties);

                if (style.AllowUseVertAlignment)
                    RemoveProperty("VertAlignment", properties);

                if (style.AllowUseTextBrush)
                    RemoveProperty("TextBrush", properties);

                if (style.AllowUseImage)
                    RemoveProperty("Image", properties);

                return properties;
            }
            #endregion

            #region StiSeries
            var series = component as StiSeries;

            var pieSeries = series as StiPieSeries;
            if (pieSeries != null)
            {
                if (series.AllowApplyStyle)
                {
                    RemoveProperty("BorderThickness", properties);
                    RemoveProperty("ShowShadow", properties);
                    RemoveProperty("Lighting", properties);
                }

                if (pieSeries.AllowApplyBorderColor)
                    RemoveProperty("BorderColor", properties);

                if (pieSeries.AllowApplyBrush)
                    RemoveProperty("Brush", properties);

                return properties;
            }

            if (series != null)
            {
                if (series.AllowApplyStyle)
                {
                    RemoveProperty("Brush", properties);
                    RemoveProperty("BorderColor", properties);
                    RemoveProperty("BorderThickness", properties);
                    RemoveProperty("CornerRadius", properties);
                    RemoveProperty("ShowShadow", properties);
                    RemoveProperty("Lighting", properties);

                    if (!(series is IStiParetoSeries))
                        RemoveProperty("LineColor", properties);
                }

                if (series is IStiAllowApplyBrushNegative && !((IStiAllowApplyBrushNegative)series).AllowApplyBrushNegative)
                    RemoveProperty("BrushNegative", properties);

                if (series is IStiAllowApplyColorNegative && !((IStiAllowApplyColorNegative)series).AllowApplyColorNegative)
                    RemoveProperty("LineColorNegative", properties);

                if (series is IStiParetoSeries && !((IStiParetoSeries)series).AllowApplyLineColor)
                    RemoveProperty("LineColor", properties);

                return properties;
            }
            #endregion

            #region StiTrendLine
            var trendLine = component as StiTrendLine;
            if (trendLine != null && trendLine.AllowApplyStyle)
            {
                RemoveProperty("LineColor", properties);
                RemoveProperty("ShowShadow", properties);

                return properties;
            }
            #endregion

            #region StiSeriesLabels
            var labels = component as StiSeriesLabels;
            if (labels != null && labels.AllowApplyStyle)
            {
                RemoveProperty("LineColor", properties);
                RemoveProperty("Antialiasing", properties);
                RemoveProperty("LabelColor", properties);
                RemoveProperty("BorderColor", properties);
                RemoveProperty("Brush", properties);
                RemoveProperty("Font", properties);

                return properties;
            }
            #endregion

            #region StiChartTable
            var chartTable = component as StiChartTable;
            if (chartTable != null && chartTable.AllowApplyStyle)
            {
                RemoveProperty("TextColor", properties);
                RemoveProperty("GridLineColor", properties);
                RemoveProperty("Font", properties);
            }
            #endregion

            #region StiConstantLines
            var constantLines = component as StiConstantLines;
            if (constantLines != null && constantLines.AllowApplyStyle)
            {
                RemoveProperty("Antialiasing", properties);
                RemoveProperty("Font", properties);
                RemoveProperty("LineColor", properties);

                return properties;
            }
            #endregion

            #region StiStrips
            var strips = component as StiStrips;
            if (strips != null && strips.AllowApplyStyle)
            {
                RemoveProperty("StripBrush", properties);
                RemoveProperty("Antialiasing", properties);
                RemoveProperty("Font", properties);
                RemoveProperty("TitleColor", properties);

                return properties;
            }
            #endregion

            #region StiAxis
            var axis = component as StiAxis;
            if (axis != null && axis.AllowApplyStyle)
            {
                RemoveProperty("LineColor", properties);
                RemoveProperty("MinorColor", properties);
                RemoveProperty("Style", properties);
                RemoveProperty("MinorStyle", properties);

                return properties;
            }
            #endregion

            #region StiRadarAxis
            var radarAxis = component as StiRadarAxis;
            if (radarAxis != null && radarAxis.AllowApplyStyle)
            {
                RemoveProperty("LineColor", properties);

                return properties;
            }
            #endregion

            #region StiInterlacing
            var interlacing = component as StiInterlacing;
            if (interlacing != null && interlacing.AllowApplyStyle)
            {
                RemoveProperty("InterlacedBrush", properties);
                RemoveProperty("Visible", properties);

                return properties;
            }
            #endregion

            #region StiGridLines
            var gridLines = component as StiGridLines;
            if (gridLines != null && gridLines.AllowApplyStyle)
            {
                RemoveProperty("Color", properties);
                RemoveProperty("MinorColor", properties);
                RemoveProperty("Style", properties);
                RemoveProperty("MinorStyle", properties);

                return properties;
            }
            #endregion

            #region StiRadarGridLines
            var radarGridLines = component as StiRadarGridLines;
            if (radarGridLines != null && radarGridLines.AllowApplyStyle)
            {
                RemoveProperty("Color", properties);

                return properties;
            }
            #endregion

            #region StiLegend
            var legend = component as StiLegend;
            if (legend != null && legend.AllowApplyStyle)
            {
                RemoveProperty("ShowShadow", properties);
                RemoveProperty("BorderColor", properties);
                RemoveProperty("Brush", properties);
                RemoveProperty("TitleColor", properties);
                RemoveProperty("LabelsColor", properties);
                RemoveProperty("TitleFont", properties);
                RemoveProperty("Font", properties);

                return properties;
            }
            #endregion

            #region StiArea
            var area = component as StiArea;
            if (area != null && area.AllowApplyStyle)
            {
                RemoveProperty("ShowShadow", properties);
                RemoveProperty("BorderColor", properties);
                RemoveProperty("BorderThickness", properties);
                RemoveProperty("Brush", properties);

                return properties;
            }
            #endregion

            #region StiChartTitle
            var chartTitle = component as StiChartTitle;
            if (chartTitle != null && chartTitle.AllowApplyStyle)
            {
                RemoveProperty("Antialiasing", properties);
                RemoveProperty("Brush", properties);
                RemoveProperty("Font", properties);

                return properties;
            }
            #endregion

            return properties;
        }

        private static void RemoveProperty(string name, List<StiPropertyDescriptor> props)
        {
            for (var index = 0; index < props.Count; index++)
            {
                var prop = props[index];
                if (prop.Name == name)
                {
                    props.RemoveAt(index);
                    return;
                }
            }
        }

        private static PropertyDescriptorCollection SortProperties(object component, PropertySort propertySort, PropertyDescriptorCollection properties)
        {
            if (propertySort != PropertySort.CategorizedAlphabetical && propertySort != PropertySort.Categorized)
                return properties;

            var orderedProperties = new List<OrderData>();
            var hashKeys = new Hashtable();

            foreach (PropertyDescriptor property in properties)
            {
                if (!StiPropertyGrid.IsAllowedProperty(component.GetType(), property.Name)) continue;

                var order = property.Attributes[typeof(StiOrderAttribute)] as StiOrderAttribute;
                var category = property.Attributes[typeof(StiCategoryAttribute)] as StiCategoryAttribute;

                var categoryName = category != null ? category.Category : "Misc";
                var categoryPosition = StiCategoryIndexHelper.GetCategoryIndex(categoryName);

                if (categoryName == "Design" && component is StiReport)
                    categoryPosition = 0;

                var position = order != null ? order.Position : 0;

                if (hashKeys[categoryName] == null && categoryPosition != int.MaxValue)
                    hashKeys[categoryName] = categoryPosition;

                orderedProperties.Add(new OrderData(categoryName, property.Name, position, categoryPosition));
            }

            #region Sort
            var keys = new string[hashKeys.Count];
            hashKeys.Keys.CopyTo(keys, 0);
            var list = new List<CategoryData>();

            foreach (var key in keys)
            {
                list.Add(new CategoryData(key, (int)hashKeys[key]));
            }
            list.Sort();
            #endregion

            #region Set index to each element from list
            var index = 1;
            foreach (var data in list)
            {
                hashKeys[data.CategoryName] = index++;
            }
            #endregion

            #region Set level of each property
            #region Calculate maxLevel
            var maxLevel = index;
            foreach (var orderData in orderedProperties)
            {
                var position = index;
                if (hashKeys[orderData.CategoryName] != null)
                {
                    position = (int)hashKeys[orderData.CategoryName];
                }
                else
                {
                    hashKeys[orderData.CategoryName] = index;
                    index++;
                }

                maxLevel = Math.Max(position, maxLevel);
            }
            #endregion

            foreach (var orderData in orderedProperties)
            {
                var position = index;
                if (hashKeys[orderData.CategoryName] != null)
                {
                    position = (int)hashKeys[orderData.CategoryName];
                }
                else
                {
                    hashKeys[orderData.CategoryName] = index;
                    index++;
                }

                ((StiPropertyDescriptor)properties[orderData.Name]).Level = position;
                ((StiPropertyDescriptor)properties[orderData.Name]).MaxLevel = maxLevel;

                orderData.CategoryPosition = position;
            }
            #endregion

            orderedProperties.Sort();

            if (component is Font)
            {
                properties = properties.Sort(new[]
                {
                    "Name", "Size", "Bold", "Italic", "Underline", "Strikeout"
                });
            }
            else
            {
                var propertyNames = orderedProperties.Select(orderData => orderData.Name).ToArray();
                properties = properties.Sort(propertyNames);
            }
            return properties;
        }

        private static PropertyDescriptorCollection ProcessDataBindingsProperties(object component)
        {
            var properties = new PropertyDescriptorCollection(null);
            var collection = component as StiDataBindingsCollection;
            var props = TypeDescriptor.GetProperties(collection.Control);

            foreach (PropertyDescriptor prop in props)
            {
                if (!StiPropertyGrid.IsAllowedProperty(component.GetType(), prop.Name)) continue;

                var bindable = prop.Attributes[typeof(BindableAttribute)] as BindableAttribute;

                if (bindable == null || bindable.Bindable)
                    properties.Add(new StiDataBindingPropertyDescriptor(prop));
            }
            return properties;
        }

        private static void ProcessBrowsableAttribute(object component, PropertyDescriptor prop, List<StiPropertyDescriptor> properties)
        {
            if (component is IStiReport)
            {
                var browsable = prop.Attributes[typeof(StiBrowsableAttribute)] as StiBrowsableAttribute;

                if (browsable == null || browsable.Browsable)
                    properties.Add(new StiPropertyDescriptor(prop));
            }
            else
            {
                var browsable = prop.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;

                if (browsable == null || browsable.Browsable)
                    properties.Add(new StiPropertyDescriptor(prop));
            }
        }

        private static bool CheckGuiMode(PropertyDescriptor prop, StiGuiMode guiMode)
        {
            var guiModeAttr = prop.Attributes[typeof(StiGuiModeAttribute)] as StiGuiModeAttribute;

            return !(guiModeAttr == null || guiModeAttr.Mode == guiMode || ShowAllPropertiesInWpf);
        }

        private static bool CheckEngineVersion(PropertyDescriptor prop, object component)
        {
            if (component is IStiEngineVersionProperty || component is IStiReportProperty)
            {
                var engineVersion = component as IStiEngineVersionProperty;

                if (component is IStiReportProperty)
                    engineVersion = ((IStiReportProperty)component).GetReport() as IStiEngineVersionProperty;

                if (engineVersion == null) return true;

                var engine = prop.Attributes[typeof(StiEngineAttribute)] as StiEngineAttribute;
                return !(engine == null || engine.Version == engineVersion.EngineVersion || engine.Version == StiEngineVersion.All);
            }
            return false;
        }

        private static bool CheckAllowedProperty(PropertyDescriptor prop, object component)
        {
            return !StiPropertyGrid.IsAllowedProperty(component.GetType(), prop.Name);
        }

        private static bool CheckPropertyLevel(PropertyDescriptor prop)
        {
            if (StiPropertyGridOptions.PropertyLevel != StiLevel.Basic && StiPropertyGridOptions.PropertyLevel != StiLevel.Standard)
                return false;

            var levelAttribute = prop.Attributes[typeof(StiPropertyLevelAttribute)] as StiPropertyLevelAttribute;
            if (levelAttribute == null)
                return false;

            if (StiPropertyGridOptions.PropertyLevel == StiLevel.Basic)
            {
                if (levelAttribute.Level == StiLevel.Professional || levelAttribute.Level == StiLevel.Standard)
                    return true;
            }
            else if (StiPropertyGridOptions.PropertyLevel == StiLevel.Standard)
            {
                if (levelAttribute.Level == StiLevel.Professional)
                    return true;
            }

            return false;
        }
        #endregion
    }
}