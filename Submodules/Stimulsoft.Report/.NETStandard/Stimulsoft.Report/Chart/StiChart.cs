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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.Units;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Chart
{
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.catInfographics.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Chart.Design.StiChartDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfChartDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiChartWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiChartGdiPainter))]
    [StiV1Builder(typeof(StiChartV1Builder))]
    [StiV2Builder(typeof(StiChartV2Builder))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    public partial class StiChart :
        StiComponent,
        IStiBorder,
        IStiBusinessObject,
        IStiBrush,
        IStiDataSource,
        IStiDataRelation,
        IStiMasterComponent,
        IStiSort,
        IStiFilter,
        IStiExportImageExtended,
        IStiIgnoryStyle,
        IStiGlobalizationProvider,
        IStiProcessAtEnd,
        IStiChartComponent,
        IStiChart
    {
        #region IStiJsonReportObject.override
        // Fields
        internal string jsonMasterComponentTemp;

        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // NonSerialized
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");

            // StiChart
            jObject.AddPropertyBool("AllowApplyStyle", AllowApplyStyle, true);
            jObject.AddPropertyEnum("FilterMode", FilterMode, StiFilterMode.And);
            jObject.AddPropertyBool("FilterOn", FilterOn, true);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyStringNullOrEmpty("DataSourceName", DataSourceName);
            jObject.AddPropertyStringNullOrEmpty("BusinessObjectGuid", BusinessObjectGuid);
            jObject.AddPropertyInt("CountData", CountData);
            jObject.AddPropertyStringNullOrEmpty("DataRelationName", DataRelationName);
            jObject.AddPropertyBool("ProcessAtEnd", ProcessAtEnd);
            jObject.AddPropertyJObject("ProcessChartEvent", ProcessChartEvent.SaveToJsonObject(mode));
            jObject.AddPropertyEnum("Rotation", Rotation, StiImageRotation.None);
            jObject.AddPropertyEnum("EditorType", EditorType, StiChartEditorType.Advanced);
            jObject.AddPropertyStringNullOrEmpty("CustomStyleName", CustomStyleName);
            jObject.AddPropertyInt("HorSpacing", HorSpacing, 10);
            jObject.AddPropertyInt("VertSpacing", VertSpacing, 10);

            if (MasterComponent != null)
                jObject.AddPropertyStringNullOrEmpty("MasterComponent", MasterComponent.Name);

            jObject.AddPropertyJObject("Series", series.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Area", area.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Table", Table.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("SeriesLabels", SeriesLabels.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Legend", Legend.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Title", title.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Strips", Strips.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ConstantLines", ConstantLines.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Style", Style.SaveToJsonObject(mode));

            if (mode == StiJsonSaveMode.Report)
            {
                jObject.AddPropertyJObject("Filters", Filters.SaveToJsonObject(mode));
                jObject.AddPropertyStringArray("Sort", Sort);
                jObject.AddPropertyJObject("SeriesLabelsConditions", SeriesLabelsConditions.SaveToJsonObject(mode));
            }

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "FilterMode":
                        this.FilterMode = property.DeserializeEnum<StiFilterMode>();
                        break;

                    case "Sort":
                        this.Sort = property.DeserializeStringArray();
                        break;

                    case "Filters":
                        this.Filters.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "FilterOn":
                        this.FilterOn = property.DeserializeBool();
                        break;

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "DataSourceName":
                        this.dataSourceName = property.DeserializeString();
                        break;

                    case "BusinessObjectGuid":
                        this.businessObjectGuid = property.DeserializeString();
                        break;

                    case "CountData":
                        this.CountData = property.DeserializeInt();
                        break;

                    case "DataRelationName":
                        this.DataRelationName = property.DeserializeString();
                        break;

                    case "ProcessAtEnd":
                        this.ProcessAtEnd = property.DeserializeBool();
                        break;

                    case "ProcessChartEvent":
                        {
                            var _event = new StiProcessChartEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.ProcessChartEvent = _event;
                        }
                        break;

                    case "SeriesLabelsConditions":
                        this.SeriesLabelsConditions.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Rotation":
                        this.Rotation = property.DeserializeEnum<StiImageRotation>();
                        break;

                    case "EditorType":
                        this.EditorType = property.DeserializeEnum<StiChartEditorType>(); 
                        break;

                    case "AllowApplyStyle":
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case "CustomStyleName":
                        this.CustomStyleName = property.DeserializeString();
                        break;

                    case "HorSpacing":
                        this.horSpacing = property.DeserializeInt();
                        break;

                    case "VertSpacing":
                        this.vertSpacing = property.DeserializeInt();
                        break;

                    case "MasterComponent":
                        {
                            this.jsonMasterComponentTemp = property.DeserializeString();
                            this.Report.jsonLoaderHelper.MasterComponents.Add(this);
                        }
                        break;

                    case "Style":
                        this.Style = StiChartStyle.CreateFromJsonObject((JObject)property.Value);
                        break;

                    case "ConstantLines":
                        this.ConstantLines.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Strips":
                        this.Strips.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Title":
                        this.Title.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Legend":
                        this.Legend.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "SeriesLabels":
                        this.SeriesLabels = StiSeriesLabels.LoadFromJsonObjectInternal((JObject)property.Value, this);
                        break;

                    case "Table":
                        this.table.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Area":
                        this.area = StiArea.CreateFromJsonObject((JObject)property.Value);
                        this.area.Chart = this;
                        break;

                    case "Series":
                        this.series.LoadFromJsonObject((JObject)property.Value);
                        break;
                }
            }

            this.Core.ApplyStyle(Style);
        }

        public string SaveToString(StiJsonSaveMode mode)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                return SaveToJsonObject(mode).ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        internal void LoadFromString(string text)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                var jObject = (JObject)JsonConvert.DeserializeObject(text);
                LoadFromJsonObject(jObject);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiChart;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var collection = new StiPropertyCollection();
            var propHelper = propertyGrid.PropertiesHelper;

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.ChartEditor(),
                propHelper.DataSourceEditor()
            });

            collection.Add(StiPropertyCategories.Chart, new[]
            {
                propHelper.AllowApplyStyle()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                });
            }

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.UseParentStyles()
                });
            }

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.GrowToHeight(),
                    propHelper.Enabled()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.GrowToHeight(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.GrowToHeight(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }

            // DesignCategory
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                });
            }

            return collection;
        }

        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return new StiEventCollection
            {
                {
                    StiPropertyCategories.MouseEvents,
                    new[]
                    {
                        StiPropertyEventId.ClickEvent,
                        StiPropertyEventId.DoubleClickEvent,
                        StiPropertyEventId.MouseEnterEvent,
                        StiPropertyEventId.MouseLeaveEvent
                    }
                },
                {
                    StiPropertyCategories.NavigationEvents,
                    new[]
                    {
                        StiPropertyEventId.GetBookmarkEvent,
                        StiPropertyEventId.GetDrillDownReportEvent,
                        StiPropertyEventId.GetHyperlinkEvent,
                        StiPropertyEventId.GetPointerEvent,
                    }
                },
                {
                    StiPropertyCategories.PrintEvents,
                    new[]
                    {
                        StiPropertyEventId.AfterPrintEvent,
                        StiPropertyEventId.BeforePrintEvent,
                    }
                },
                {
                    StiPropertyCategories.ValueEvents,
                    new[]
                    {
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                        StiPropertyEventId.ProcessChartEvent
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/index.html?report_internals_charts.htm";
        #endregion

        #region IStiUnitConvert
        /// <summary>
        /// Converts a component out of one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
            base.Convert(oldUnit, newUnit, isReportSnapshot);
            this.Legend.Size = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.Legend.Size));
        }
        #endregion

        #region IStiChartXF
        public double ConvertToHInches(double value)
        {
            if (this.Report != null)
                return this.Report.Unit.ConvertToHInches(value);
            else
                return value;
        }
        #endregion

        #region IStiGlobalizationProvider
        /// <summary>
        /// Sets localized string to specified property name.
        /// </summary>
        void IStiGlobalizationProvider.SetString(string propertyName, string value)
        {
            switch (propertyName)
            {
                case "Title.Text":
                    Title.Text = value;
                    break;

                case "Legend.Title":
                    Legend.Title = value;
                    break;

                case "SeriesLabels.TextBefore":
                    SeriesLabels.TextBefore = value;
                    break;

                case "SeriesLabels.TextAfter":
                    SeriesLabels.TextAfter = value;
                    break;
            }

            var axisArea = area as IStiAxisArea;
            if (axisArea != null)
            {
                switch (propertyName)
                {

                    case "Area.XAxis.Labels.TextBefore":
                        axisArea.XAxis.Labels.TextBefore = value;
                        break;

                    case "Area.XAxis.Labels.TextAfter":
                        axisArea.XAxis.Labels.TextAfter = value;
                        break;

                    case "Area.XAxis.Title.Text":
                        axisArea.XAxis.Title.Text = value;
                        break;

                    case "Area.YAxis.Labels.TextBefore":
                        axisArea.YAxis.Labels.TextBefore = value;
                        break;

                    case "Area.YAxis.Labels.TextAfter":
                        axisArea.YAxis.Labels.TextAfter = value;
                        break;

                    case "Area.YAxis.Title.Text":
                        axisArea.YAxis.Title.Text = value;
                        break;

                    case "Area.XTopAxis.Labels.TextBefore":
                        axisArea.XTopAxis.Labels.TextBefore = value;
                        break;

                    case "Area.XTopAxis.Labels.TextAfter":
                        axisArea.XTopAxis.Labels.TextAfter = value;
                        break;

                    case "Area.XTopAxis.Title.Text":
                        axisArea.XTopAxis.Title.Text = value;
                        break;

                    case "Area.YRightAxis.Labels.TextBefore":
                        axisArea.YRightAxis.Labels.TextBefore = value;
                        break;

                    case "Area.YRightAxis.Labels.TextAfter":
                        axisArea.YRightAxis.Labels.TextAfter = value;
                        break;

                    case "Area.YRightAxis.Title.Text":
                        axisArea.YRightAxis.Title.Text = value;
                        break;
                }
            }

            var seriesIndex = 0;
            foreach (IStiSeries series in this.Series)
            {
                var seriesTitle = $"Series{seriesIndex}.Title";
                if (propertyName == seriesTitle && series.CoreTitle != value)
                    series.CoreTitle = value;

                var seriesTopNOther = $"Series{seriesIndex}.TopN.OtherText";
                if (propertyName == seriesTopNOther)
                    series.TopN.OthersText = value;

                seriesIndex++;
            }
        }

        /// <summary>
        /// Gets localized string from specified property name.
        /// </summary>
        string IStiGlobalizationProvider.GetString(string propertyName)
        {
            switch (propertyName)
            {
                case "Title.Text":
                    return Title.Text;

                case "Legend.Title":
                    return Legend.Title;

                case "SeriesLabels.TextBefore":
                    return SeriesLabels.TextBefore;

                case "SeriesLabels.TextAfter":
                    return SeriesLabels.TextAfter;
            }

            var axisArea = area as IStiAxisArea;
            if (axisArea != null)
            {
                switch (propertyName)
                {
                    case "Area.XAxis.Labels.TextBefore":
                        return axisArea.XAxis.Labels.TextBefore;

                    case "Area.XAxis.Labels.TextAfter":
                        return axisArea.XAxis.Labels.TextAfter;

                    case "Area.XAxis.Title.Text":
                        return axisArea.XAxis.Title.Text;

                    case "Area.YAxis.Labels.TextBefore":
                        return axisArea.YAxis.Labels.TextBefore;

                    case "Area.YAxis.Labels.TextAfter":
                        return axisArea.YAxis.Labels.TextAfter;

                    case "Area.YAxis.Title.Text":
                        return axisArea.YAxis.Title.Text;

                    case "Area.XTopAxis.Labels.TextBefore":
                        return axisArea.XTopAxis.Labels.TextBefore;

                    case "Area.XTopAxis.Labels.TextAfter":
                        return axisArea.XTopAxis.Labels.TextAfter;

                    case "Area.XTopAxis.Title.Text":
                        return axisArea.XTopAxis.Title.Text;

                    case "Area.YRightAxis.Labels.TextBefore":
                        return axisArea.YRightAxis.Labels.TextBefore;

                    case "Area.YRightAxis.Labels.TextAfter":
                        return axisArea.YRightAxis.Labels.TextAfter;

                    case "Area.YRightAxis.Title.Text":
                        return axisArea.YRightAxis.Title.Text;
                }
            }

            int seriesIndex = 0;
            foreach (IStiSeries series in this.Series)
            {
                var seriesTitle = $"Series{seriesIndex}.Title";
                if (propertyName == seriesTitle)
                    return series.CoreTitle;

                var seriesTopNOther = $"Series{seriesIndex}.TopN.OtherText";
                if (propertyName == seriesTopNOther)
                    return series.TopN.OthersText;

                seriesIndex++;
            }

            throw new ArgumentException($"Property with name {propertyName}");
        }

        /// <summary>
        /// Returns array of the property names which can be localized.
        /// </summary>
        string[] IStiGlobalizationProvider.GetAllStrings()
        {
            var list = new List<string>
            {
                "Title.Text",
                "Legend.Title",
                "SeriesLabels.TextBefore",
                "SeriesLabels.TextAfter"
            };

            if (Area is IStiAxisArea)
            {
                list.Add("Area.XAxis.Labels.TextBefore");
                list.Add("Area.XAxis.Labels.TextAfter");
                list.Add("Area.XAxis.Title.Text");

                list.Add("Area.YAxis.Labels.TextBefore");
                list.Add("Area.YAxis.Labels.TextAfter");
                list.Add("Area.YAxis.Title.Text");

                list.Add("Area.XTopAxis.Labels.TextBefore");
                list.Add("Area.XTopAxis.Labels.TextAfter");
                list.Add("Area.XTopAxis.Title.Text");

                list.Add("Area.YRightAxis.Labels.TextBefore");
                list.Add("Area.YRightAxis.Labels.TextAfter");
                list.Add("Area.YRightAxis.Title.Text");
            }

            for (int index = 0; index < series.Count; index++)
            {
                list.Add($"Series{index}.Title");
                list.Add($"Series{index}.TopN.OtherText");
            }

            return list.ToArray();
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            StiChart chart = base.Clone() as StiChart;

            chart = AdditionalClone(chart);

            return chart;
        }

        public override object Clone(bool cloneProperties)
        {
            var chart = (StiChart)base.Clone(cloneProperties);

            chart = AdditionalClone(chart);

            return chart;
        }

        private StiChart AdditionalClone(StiChart chart)
        {
            chart.ChartInfoV1 = this.ChartInfoV1.Clone() as StiChartInfoV1;
            chart.ChartInfoV2 = this.ChartInfoV2.Clone() as StiChartInfoV2;

            chart.Brush = this.Brush.Clone() as StiBrush;
            chart.Border = this.Border.Clone() as StiBorder;

            if (this.Sort != null)
                chart.Sort = (string[])this.Sort.Clone();
            else
                chart.Sort = null;

            if (this.Filters != null)
                chart.Filters = this.Filters.Clone() as StiFiltersCollection;
            else
                chart.Filters = null;

            if (chart.Title != null)
                chart.Title = this.Title.Clone() as IStiChartTitle;

            if (chart.Area != null)
                chart.Area = this.Area.Clone() as IStiArea;

            if (chart.SeriesLabels != null)
                chart.SeriesLabels = this.SeriesLabels.Clone() as IStiSeriesLabels;

            if (chart.Legend != null)
                chart.Legend = this.Legend.Clone() as IStiLegend;

            if (chart.Table != null)
                chart.Table = this.Table.Clone() as IStiChartTable;

            chart.Series = new StiSeriesCollection();
            chart.Series.SeriesAdded += Series_SeriesAdded;
            chart.Series.SeriesRemoved += Series_SeriesRemoved;

            foreach (IStiSeries series in this.series)
            {
                IStiSeries newSeries = (IStiSeries)series.Clone();
                string oldTitle = (newSeries as StiSeries).Title.Value;
                chart.Series.Add(newSeries);
                ((StiSeries)newSeries).Title.Value = oldTitle;
            }

            chart.ConstantLines = new StiConstantLinesCollection();
            foreach (IStiConstantLines line in this.constantLines) chart.ConstantLines.Add((IStiConstantLines)line.Clone());

            chart.Strips = new StiStripsCollection();
            foreach (IStiStrips strip in this.strips) chart.Strips.Add((IStiStrips)strip.Clone());

            if (chart.Core != null)
            {
                chart.Core = this.Core.Clone() as StiChartCoreXF;
                chart.Core.Chart = chart;
            }

            return chart;
        }
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public override void SaveState(string stateName)
        {
            base.SaveState(stateName);

            States.PushInt(stateName, this, "positionValue", positionValue);
            States.PushBool(stateName, this, "isEofValue", isEofValue);
            States.PushBool(stateName, this, "isBofValue", isBofValue);

            if (DataSource != null)
                this.DataSource.SaveState(stateName);
            if (BusinessObject != null)
            {
                this.BusinessObject.SaveState(stateName);
                this.BusinessObject.CreateEnumerator();
                this.BusinessObject.specTotalsCalculation = true;
            }
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public override void RestoreState(string stateName)
        {
            base.RestoreState(stateName);

            if (States.IsExist(stateName, this))
            {
                isBofValue = States.PopBool(stateName, this, "isBofValue");
                isEofValue = States.PopBool(stateName, this, "isEofValue");
                positionValue = States.PopInt(stateName, this, "positionValue");
            }

            if (DataSource != null)
                this.DataSource.RestoreState(stateName);
            if (BusinessObject != null)
                this.BusinessObject.RestoreState(stateName);
        }
        #endregion

        #region IStiExportImageExtended
        public virtual Image GetImage(ref float zoom)
        {
            return GetImage(ref zoom, StiExportFormat.None);
        }

        public virtual Image GetImage(ref float zoom, StiExportFormat format)
        {
            StiPainter painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            return painter.GetImage(this, ref zoom, format);
        }

        [Browsable(false)]
        public override bool IsExportAsImage(StiExportFormat format)
        {
            //if (StiFontIconsHelper.NeedToUseStimulsoftFont(this.Report)) return true;

            if (format == StiExportFormat.Pdf) return false;
            if (format == StiExportFormat.ImageSvg) return false;

            return true;
        }
        #endregion

        #region IStiFilter
        /// <summary>
        /// Gets or sets a method for filtration.
        /// </summary>
        [Browsable(false)]
        public StiFilterEventHandler FilterMethodHandler { get; set; }

        /// <summary>
        /// Gets or sets filter mode.
        /// </summary>
        [DefaultValue(StiFilterMode.And)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Browsable(false)]
        public StiFilterMode FilterMode { get; set; } = StiFilterMode.And;

        /// <summary>
        /// Gets or sets the collection of data filters.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [TypeConverter(typeof(StiFiltersCollectionConverter))]
        [StiOrder(StiPropertyOrder.DataFilters)]
        [StiCategory("Data")]
        [Editor("Stimulsoft.Report.Components.Design.StiFiltersCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets the collection of data filters.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public StiFiltersCollection Filters { get; set; } = new StiFiltersCollection();

        private bool ShouldSerializeFilters()
        {
            return Filters == null || Filters.Count > 0;
        }

        /// <summary>
        /// Do not use this property.
        /// </summary>
        [Browsable(false)]
        [StiNonSerialized]
        [Obsolete("Filter property is obsolete. Please use Filters property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual StiExpression Filter
        {
            get
            {
                if (Filters.Count == 0)
                    Filters.Add(new StiFilter());

                Filters[0].Item = StiFilterItem.Expression;
                return Filters[0].Expression;
            }
            set
            {
                if (Filters.Count == 0)
                    Filters.Add(new StiFilter());

                Filters[0].Item = StiFilterItem.Expression;
                Filters[0].Expression = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicates, that filter is turn on.
        /// </summary>
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiSerializable]
        [DefaultValue(true)]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataFilterOn)]
        [Description("Gets or sets value indicates, that filter is turn on.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool FilterOn { get; set; } = true;
        #endregion

        #region IStiBorder
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        public StiBorder Border { get; set; } = new StiBorder();

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
        }
        #endregion

        #region IStiBrush
        /// <summary>
        /// The brush, which is used to draw background.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiSerializable]
        [Description("The brush, which is used to draw background.")]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);
        #endregion

        #region IStiSort
        /// <summary>
        /// Gets or sets the array of strings that describes rules of sorting.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [TypeConverter(typeof(StiSortConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiSortEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataSort)]
        [Description("Gets or sets the array of strings that describes rules of sorting.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual string[] Sort { get; set; } = new string[0];

        private bool ShouldSerializeSort()
        {
            return Sort == null || Sort.Length > 0;
        }
        #endregion

        #region IStiDataSource
        /// <summary>
        /// Get data source that is used for getting data.
        /// </summary>
        [TypeConverter(typeof(StiDataSourceConverter))]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataDataSource)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataSourceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Get data source that is used for getting data.")]
        public virtual StiDataSource DataSource
        {
            get
            {
                if (isCacheValues)
                    return cachedDataSource;

                if (Page == null ||
                    Report == null ||
                    Report.DataSources == null ||
                    DataSourceName == null ||
                    DataSourceName.Length == 0) return null;

                return Report.DataSources[DataSourceName];
            }
        }

        private string dataSourceName = string.Empty;

        /// <summary>
        /// Gets or sets name of the Data Source.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue("")]
        public string DataSourceName
        {
            get
            {
                return dataSourceName;
            }
            set
            {
                if (dataSourceName != value)
                {
                    dataSourceName = value;
                    StiOptions.Engine.GlobalEvents.InvokeDataSourceAssigned(this, EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        public bool IsDataSourceEmpty
        {
            get
            {
                if (isCacheValues)
                    return cachedIsDataSourceEmpty;

                return string.IsNullOrEmpty(DataSourceName) || DataSource == null;
            }
        }
        #endregion

        #region IStiBusinessObject
        [Browsable(false)]
        public bool IsBusinessObjectEmpty
        {
            get
            {
                if (isCacheValues)
                    return cachedIsBusinessObjectEmpty;

                return string.IsNullOrEmpty(BusinessObjectGuid) || BusinessObject == null;
            }
        }

        /// <summary>
        /// Get business object that is used for getting data.
        /// </summary>
        [TypeConverter(typeof(StiBusinessObjectConverter))]
        [Description("Get Business Object that is used for getting data.")]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataBusinessObject)]
        [RefreshProperties(RefreshProperties.All)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Professional)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataSourceEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual StiBusinessObject BusinessObject
        {
            get
            {
                if (isCacheValues)
                    return cachedBusinessObject;

                if (Page == null ||
                    Report == null ||
                    BusinessObjectGuid == null ||
                    BusinessObjectGuid.Length == 0) return null;

                return StiBusinessObjectHelper.GetBusinessObjectFromGuid(this.Report, this.BusinessObjectGuid);
            }
        }

        private string businessObjectGuid = string.Empty;

        /// <summary>
        /// Gets or sets guid of the Business Object.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue("")]
        public string BusinessObjectGuid
        {
            get
            {
                return businessObjectGuid;
            }
            set
            {
                if (businessObjectGuid != value)
                {
                    businessObjectGuid = value;

                    if (!string.IsNullOrEmpty(value))
                        this.DataSourceName = null;

                    StiOptions.Engine.GlobalEvents.InvokeBusinessObjectAssigned(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region IStiMasterComponent
        /// <summary>
        /// Gets or sets the master-component.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Reference,
            StiSerializeTypes.SerializeToCode |
            StiSerializeTypes.SerializeToSaveLoad |
            StiSerializeTypes.SerializeToDocument)]
        [TypeConverter(typeof(StiMasterComponentConverter))]
        [DefaultValue(null)]
        [Description("Gets or sets the master-component.")]
        [StiOrder(StiPropertyOrder.DataMasterComponent)]
        [StiPropertyLevel(StiLevel.Professional)]
        [StiCategory("Data")]
        [Editor("Stimulsoft.Report.Components.Design.StiMasterComponentEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual StiComponent MasterComponent { get; set; }
        #endregion

        #region IStiEnumerator
        /// <summary>
        /// Gets or sets the count of rows for virtual data.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0)]
        [StiCategory("Data")]
        [StiPropertyLevel(StiLevel.Professional)]
        [StiOrder(StiPropertyOrder.DataCountData)]
        [Description("Gets or sets the count of rows for virtual data.")]
        public virtual int CountData { get; set; }

        /// <summary>
        /// Sets the position at the beginning.
        /// </summary>
        public virtual void First()
        {
            if (!this.IsBusinessObjectEmpty)
                this.BusinessObject.First();

            else if (!IsDataSourceEmpty)
                this.DataSource.First();

            else
            {
                this.isEofValue = false;
                this.isBofValue = true;
                this.positionValue = 0;
            }
        }

        /// <summary>
        /// Move on the previous row.
        /// </summary>
        public virtual void Prior()
        {
            if (!this.IsBusinessObjectEmpty)
                this.BusinessObject.Prior();

            else if (!this.IsDataSourceEmpty)
                this.DataSource.Prior();

            else
            {
                this.isBofValue = false;
                this.isEofValue = false;

                if (this.positionValue <= 0)
                    this.isBofValue = true;
                else
                    this.positionValue--;
            }
        }

        /// <summary>
        /// Move on the next row.
        /// </summary>
        public virtual void Next()
        {
            if (!this.IsBusinessObjectEmpty)
                this.BusinessObject.Next();

            else if (!this.IsDataSourceEmpty)
                this.DataSource.Next();

            else
            {
                this.isBofValue = false;
                this.isEofValue = false;

                if (this.positionValue >= this.CountData - 1)
                    this.isEofValue = true;
                else
                    this.positionValue++;
            }
        }

        /// <summary>
        /// Move on the last row.
        /// </summary>
        public virtual void Last()
        {
            if (!this.IsBusinessObjectEmpty)
                this.BusinessObject.Last();

            else if (!this.IsDataSourceEmpty)
                this.DataSource.Last();

            else
            {
                this.isEofValue = true;
                this.isBofValue = false;
                this.positionValue = this.CountData - 1;
            }
        }

        internal bool isEofValue;

        /// <summary>
        /// Gets value indicates that the position indicates to the end of data.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsEof
        {
            get
            {
                if (!this.IsBusinessObjectEmpty)
                    return BusinessObject.IsEof;

                if (!this.IsDataSourceEmpty)
                    return DataSource.IsEof;

                return isEofValue;
            }
            set
            {
                if (!this.IsBusinessObjectEmpty)
                    BusinessObject.IsEof = value;

                else if (!this.IsDataSourceEmpty)
                    DataSource.IsEof = value;

                else
                    isEofValue = value;
            }
        }

        internal bool isBofValue;

        /// <summary>
        /// Gets value, indicates that the position indicates to the beginning of data.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsBof
        {
            get
            {
                if (!this.IsBusinessObjectEmpty)
                    return BusinessObject.IsBof;

                if (!this.IsDataSourceEmpty)
                    return DataSource.IsBof;

                return isBofValue;
            }
            set
            {
                if (!this.IsBusinessObjectEmpty)
                    BusinessObject.IsBof = value;

                else if (!this.IsDataSourceEmpty)
                    DataSource.IsBof = value;

                else
                    isBofValue = value;
            }
        }

        /// <summary>
        /// Gets value indicates that no data.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsEmpty
        {
            get
            {
                if (!this.IsBusinessObjectEmpty)
                    return BusinessObject.IsEmpty;

                if (!this.IsDataSourceEmpty)
                    return DataSource.IsEmpty;

                return CountData == 0;
            }
        }

        internal int positionValue;

        /// <summary>
        /// Gets the current position.
        /// </summary>
        [Browsable(false)]
        public virtual int Position
        {
            get
            {
                if (!this.IsBusinessObjectEmpty)
                    return BusinessObject.Position;

                if (!this.IsDataSourceEmpty)
                    return DataSource.Position;

                return positionValue;
            }
            set
            {
                if (!this.IsBusinessObjectEmpty)
                    BusinessObject.Position = value;
                else if (!this.IsDataSourceEmpty)

                    DataSource.Position = value;

                else
                    positionValue = value;
            }
        }

        /// <summary>
        /// Gets count of rows.
        /// </summary>
        [Browsable(false)]
        public virtual int Count
        {
            get
            {
                if (isCacheValues)
                    return cachedCount;

                if (!this.IsBusinessObjectEmpty)
                    return BusinessObject.Count;

                if (!this.IsDataSourceEmpty)
                    return DataSource.Count;

                return CountData;
            }
        }

        private bool isCacheValues;
        private int cachedCount = 0;
        private bool cachedIsBusinessObjectEmpty;
        private bool cachedIsDataSourceEmpty;
        private StiDataSource cachedDataSource;
        private StiBusinessObject cachedBusinessObject;

        internal void CacheValues(bool cache)
        {
            if (cache)
            {
                cachedCount = Count;
                cachedIsBusinessObjectEmpty = IsBusinessObjectEmpty;
                cachedIsDataSourceEmpty = IsDataSourceEmpty;
                cachedDataSource = DataSource;
                cachedBusinessObject = BusinessObject;
            }
            else
            {
                cachedDataSource = null;
                cachedBusinessObject = null;
            }

            isCacheValues = cache;
        }
        #endregion

        #region IStiDataRelation
        /// <summary>
        /// Get link that is used for master-detail reports rendering.
        /// </summary>
        [TypeConverter(typeof(StiDataRelationConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiDataRelationEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataDataRelation)]
        [Description("Get link that is used for master-detail reports rendering.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual StiDataRelation DataRelation
        {
            get
            {
                if (Page == null ||
                    Report == null ||
                    Report.Dictionary == null ||
                    Report.Dictionary.Relations == null ||
                    DataRelationName == null ||
                    DataRelationName.Length == 0) return null;
                return Report.Dictionary.Relations[DataRelationName];
            }
        }

        /// <summary>
        /// Gets or sets relation name.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue("")]
        public string DataRelationName { get; set; } = "";
        #endregion

        #region IStiCanShrink override
        [Browsable(false)]
        [StiNonSerialized]
        public override bool CanShrink
        {
            get
            {
                return base.CanShrink;
            }
            set
            {
            }
        }
        #endregion

        #region IStiCanGrow override
        [Browsable(false)]
        [StiNonSerialized]
        public override bool CanGrow
        {
            get
            {
                return base.CanGrow;
            }
            set
            {
            }
        }
        #endregion

        #region IStiProcessAtEnd
        /// <summary>
        /// Gets or sets value indicates that a chart is processed at the end of the report execution.
        /// </summary>
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiSerializable]
        [StiCategory("ChartAdditional")]
        [StiOrder(StiPropertyOrder.ChartProcessAtEnd)]
        [Description("Gets or sets value indicates that a chart is processed at the end of the report execution.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool ProcessAtEnd { get; set; }
        #endregion

        #region IStiGetFonts
        public override List<StiFont> GetFonts()
        {
            var result = base.GetFonts();
            result.Add(new StiFont(Title.Font));
            result.Add(new StiFont(Legend.Font));
            result.Add(new StiFont(Table.DataCells.Font));
            result.Add(new StiFont(Table.Header.Font));
            foreach (var line in ConstantLines)
            {
                result.Add(new StiFont((line as IStiConstantLines).Font));
            }

            foreach (var line in Strips)
            {
                result.Add(new StiFont((line as IStiStrips).Font));
            }

            if (Area is StiAxisArea area)
            {
                result.Add(new StiFont(area.XAxis.Labels.Font));
                result.Add(new StiFont(area.XAxis.Title.Font));
                result.Add(new StiFont(area.YAxis.Labels.Font));
                result.Add(new StiFont(area.YAxis.Title.Font));
                result.Add(new StiFont(area.XTopAxis.Labels.Font));
                result.Add(new StiFont(area.XTopAxis.Title.Font));
                result.Add(new StiFont(area.YRightAxis.Labels.Font));
                result.Add(new StiFont(area.YRightAxis.Title.Font));
            }

            if (Area is StiRadarArea area2)
            {
                result.Add(new StiFont(area2.XAxis.Labels.Font));
                result.Add(new StiFont(area2.YAxis.Labels.Font));
            }

            foreach (var ser in Series)
            {
                if (ser is StiSeries serie)
                {
                    foreach (StiTrendLine line in serie.TrendLines)
                        result.Add(new StiFont(line.Font));

                    if (serie.SeriesLabels != null)
                        result.Add(new StiFont(serie.SeriesLabels.Font));
                }

                if (ser is StiPictorialSeries)
                {
                    result.Add(StiFontIconsHelper.GetFont());
                }
            }

            return result.Distinct().ToList();
        }
        #endregion

        #region StiComponent override
        /// <summary>
		/// Returns events collection of this component.
		/// </summary>
		public override StiEventsCollection GetEvents()
        {
            var events = base.GetEvents();

            if (ProcessChartEvent != null)
                events.Add(ProcessChartEvent);

            return events;
        }

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.Component;

        public override string LocalizedCategory => StiLocalization.Get("Components", "StiChart");

        /// <summary>
        /// Gets or sets the default client area of a component.
        /// </summary>
        [Browsable(false)]
        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 200, 200);

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType => StiComponentType.Simple;

        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Chart;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiChart");
        #endregion

        #region Events
        /// <summary>
        /// Invokes all events for this components.
        /// </summary>
        public override void InvokeEvents()
        {
            try
            {
                base.InvokeEvents();

                #region ProcessChart
                InvokeProcessChart(this, EventArgs.Empty);
                #endregion
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "DoEvents...ERROR");
                StiLogService.Write(this.GetType(), e);

                if (Report != null)
                    Report.WriteToReportRenderingMessages(this.Name + " " + e.Message);
            }
        }

        #region ProcessChart
        /// <summary>
        /// Occurs when getting the ProcessChart.
        /// </summary>
        public event EventHandler ProcessChart;

        /// <summary>
        /// Raises the ProcessChart event.
        /// </summary>
        protected virtual void OnProcessChart(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the ProcessChart event.
        /// </summary>
        public void InvokeProcessChart(object sender, EventArgs e)
        {
            try
            {
                OnProcessChart(e);
                this.ProcessChart?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "InvokeProcessChart...Warning");
                StiLogService.Write(this.GetType(), ex.Message);

                if (Report != null)
                    Report.WriteToReportRenderingMessages(this.Name + " " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when getting the ProcessChart.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the ProcessChart.")]
        public virtual StiProcessChartEvent ProcessChartEvent { get; set; } = new StiProcessChartEvent();
        #endregion
        #endregion

        #region Handlers
        private void Series_SeriesAdded(object sender, EventArgs e)
        {
            IStiSeries series = sender as IStiSeries;
            var chart = series.Chart as StiChart;
            if (chart.Area == null || !chart.Area.Core.IsAcceptableSeries(series.GetType()))
            {
                chart.Area = StiActivator.CreateObject(series.GetDefaultAreaType()) as IStiArea;
                if (chart.SeriesLabels == null || (!chart.Area.Core.IsAcceptableSeriesLabels(chart.SeriesLabels.GetType())))
                    chart.SeriesLabels = StiActivator.CreateObject(Area.GetDefaultSeriesLabelsType()) as IStiSeriesLabels;
            }

            if (chart.Series.Count == 1 && chart.Series[0] is StiTreemapSeries)
                chart.Legend.VertAlignment = StiLegendVertAlignment.TopOutside;

            else if (chart.Series.Count == 1 && chart.Series[0] is StiPictorialSeries)
            {
                chart.Legend.VertAlignment = StiLegendVertAlignment.TopOutside;
                chart.Legend.HorAlignment = StiLegendHorAlignment.Center;
                chart.Legend.Columns = 1;
            }

            chart.Core.ApplyStyle(chart.Style);
        }

        private void Series_SeriesRemoved(object sender, EventArgs e)
        {
            IStiSeries seriesTemp = sender as IStiSeries;
            var chart = seriesTemp.Chart as StiChart;

            if (chart.Series.Count == 0)
            {
                if (!(chart.Area is IStiClusteredColumnArea))
                {
                    chart.Area = new StiClusteredColumnArea();
                    chart.Area.Core.ApplyStyle(this.Style);
                }

                return;
            }

            if (chart.Area != null)
            {
                Type areaType = chart.Area.GetType();
                foreach (IStiSeries series in chart.Series)
                {
                    Type defaultAreaType = series.GetDefaultAreaType();
                    if (defaultAreaType == areaType)
                        return;
                }
            }

            chart.Area = StiActivator.CreateObject(((StiSeries)chart.Series[0]).GetDefaultAreaType()) as IStiArea;
            if (AllowApplyStyle)
                this.Core.ApplyStyle(chart.Style);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets collection of conditions which can be used to change behavior of series labels.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [Description("Gets or sets collection of conditions which can be used to change behavior of series labels.")]
        [Browsable(false)]
        public virtual StiChartConditionsCollection SeriesLabelsConditions { get; set; } = new StiChartConditionsCollection();

        /// <summary>
        /// 'ChartType' property is obsolete. Now chart type detected from first series in series collection.
        /// </summary>
        [StiNonSerialized]
        [Description("'ChartType' property is obsolete. Now chart type detected from first series in series collection.")]
        [StiCategory("Chart")]
        [StiOrder(StiPropertyOrder.ChartChartType)]
        [TypeConverter(typeof(Design.StiAreaConverter))]
        [Browsable(false)]
        [Obsolete("'ChartType' property is obsolete. Now chart type detected from first series in series collection.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IStiArea ChartType
        {
            get
            {
                return Area;
            }
            set
            {
                Area = value;
            }
        }

        [Browsable(false)]
        public override string ComponentStyle
        {
            get
            {
                return base.ComponentStyle;
            }
            set
            {
                base.ComponentStyle = value;
            }
        }

        [Browsable(false)]
        public bool IsDashboard { get; set; }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiChart();
        }
        #endregion

        #region Methods
        public void ApplyStyle()
        {
            if (AllowApplyStyle)
                Core.ApplyStyle(Style);
        }

        public void SimplifyValues()
        {
            int seriesIndex = 0;
            while (seriesIndex < this.Series.Count)
            {
                var series = this.Series[seriesIndex] as StiSeries;

                if (series.Values.Length > 800)
                {
                    var shorterListPoints = StiChartHelper.GetShorterListPoints(series);

                    series.Values = new double?[shorterListPoints.Count];
                    series.Arguments = new object[shorterListPoints.Count];
                    for (int index = 0; index < shorterListPoints.Count; index++)
                    {
                        series.Values[index] = shorterListPoints[index].X;
                        series.Arguments[index] = shorterListPoints[index].Y;
                    }
                }

                seriesIndex++;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new StiChart.
        /// </summary>
        public StiChart() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new StiChart.
        /// </summary>
        /// <param name="rect">The rectangle describes sizes and position of the component.</param>
        public StiChart(RectangleD rect) : base(rect)
        {
            this.allowApplyStyle = StiOptions.Engine.DefaultValueOfAllowApplyStyleProperty;

            PlaceOnToolbox = true;

            Legend = new StiLegend();
            Series = new StiSeriesCollection();
            Series.SeriesAdded += Series_SeriesAdded;
            Series.SeriesRemoved += Series_SeriesRemoved;
            Area = new StiClusteredColumnArea();
            SeriesLabels = new StiCenterAxisLabels();
            ConstantLines = new StiConstantLinesCollection();
            Strips = new StiStripsCollection();
            Title = new StiChartTitle();
            Table = new StiChartTable();
            PreviousAnimations = new List<StiAnimation>();

            this.Core = new StiChartCoreXF(this);
            Core.ApplyStyle(Style);
        }
    }
}