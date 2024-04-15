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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [StiServiceBitmap(typeof(StiChartStyle), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [StiServiceCategoryBitmap(typeof(StiArea), "Stimulsoft.Report.Images.Components.StiChart.png")]
    public abstract class StiSeries :
        StiService,
        IStiSeriesParent,
        IStiSeries,
        IStiPropertyGridObject,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyBool("AllowApplyStyle", AllowApplyStyle, true);
            jObject.AddPropertyStringNullOrEmpty("Format", Format);
            jObject.AddPropertyEnum("SortBy", SortBy, StiSeriesSortType.None);
            jObject.AddPropertyEnum("SortDirection", SortDirection, StiSeriesSortDirection.Ascending);
            jObject.AddPropertyBool("ShowInLegend", ShowInLegend, true);
            jObject.AddPropertyEnum("ShowSeriesLabels", showSeriesLabels, StiShowSeriesLabels.FromChart);
            jObject.AddPropertyBool("ShowShadow", ShowShadow, true);
            jObject.AddPropertyEnum("FilterMode", FilterMode, StiFilterMode.And);
            jObject.AddPropertyJObject("Filters", Filters.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Conditions", Conditions.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("TopN", TopN.SaveToJsonObject(mode));
            jObject.AddPropertyEnum("YAxis", YAxis, StiSeriesYAxis.LeftYAxis);
            jObject.AddPropertyJObject("SeriesLabels", seriesLabels.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("TrendLines", TrendLines.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("ValueDataColumn", ValueDataColumn);
            jObject.AddPropertyStringNullOrEmpty("ArgumentDataColumn", ArgumentDataColumn);
            jObject.AddPropertyStringNullOrEmpty("AutoSeriesTitleDataColumn", AutoSeriesTitleDataColumn);
            jObject.AddPropertyStringNullOrEmpty("AutoSeriesKeyDataColumn", AutoSeriesKeyDataColumn);
            jObject.AddPropertyStringNullOrEmpty("AutoSeriesColorDataColumn", AutoSeriesColorDataColumn);
            jObject.AddPropertyStringNullOrEmpty("ToolTipDataColumn", ToolTipDataColumn);
            jObject.AddPropertyStringNullOrEmpty("TagDataColumn", TagDataColumn);
            jObject.AddPropertyStringNullOrEmpty("HyperlinkDataColumn", HyperlinkDataColumn);
            jObject.AddPropertyBool("DrillDownEnabled", DrillDownEnabled);
            jObject.AddPropertyStringNullOrEmpty("DrillDownReport", DrillDownReport);
            jObject.AddPropertyStringNullOrEmpty("DrillDownPageGuid", DrillDownPageGuid);
            jObject.AddPropertyBool("AllowSeries", AllowSeries, true);
            jObject.AddPropertyBool("AllowSeriesElements", AllowSeriesElements, true);
            jObject.AddPropertyJObject("Interaction", Interaction.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("NewAutoSeriesEvent", NewAutoSeriesEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetValueEvent", GetValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfValuesEvent", GetListOfValuesEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetArgumentEvent", GetArgumentEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfArgumentsEvent", GetListOfArgumentsEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetTitleEvent", GetTitleEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetToolTipEvent", GetToolTipEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfToolTipsEvent", GetListOfToolTipsEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetTagEvent", GetTagEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfTagsEvent", GetListOfTagsEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetHyperlinkEvent", GetHyperlinkEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetListOfHyperlinksEvent", GetListOfHyperlinksEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Value", Value.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfValues", ListOfValues.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Argument", Argument.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfArguments", ListOfArguments.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Title", Title.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ToolTip", ToolTip.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfToolTips", ListOfToolTips.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Tag", Tag.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfTags", ListOfTags.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("Hyperlink", Hyperlink.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("ListOfHyperlinks", ListOfHyperlinks.SaveToJsonObject(mode));

            if (mode == StiJsonSaveMode.Document)
            {
                jObject.AddPropertyStringNullOrEmpty("ValuesString", ValuesString);
                jObject.AddPropertyStringNullOrEmpty("ArgumentsString", ArgumentsString);
                jObject.AddPropertyStringNullOrEmpty("ToolTipsString", ToolTipsString);
                jObject.AddPropertyStringNullOrEmpty("TagString", TagString);
                jObject.AddPropertyStringNullOrEmpty("HyperlinkString", HyperlinkString);
                jObject.AddPropertyStringNullOrEmpty("TitleValue", TitleValue);
            }

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "AllowApplyStyle":
                        this.AllowApplyStyle = property.DeserializeBool();
                        break;

                    case "Format":
                        this.Format = property.DeserializeString();
                        break;

                    case "SortBy":
                        this.SortBy = property.DeserializeEnum<StiSeriesSortType>();
                        break;

                    case "SortDirection":
                        this.SortDirection = property.DeserializeEnum<StiSeriesSortDirection>(); 
                        break;

                    case "ShowInLegend":
                        this.ShowInLegend = property.DeserializeBool();
                        break;

                    case "ShowSeriesLabels":
                        this.showSeriesLabels = property.DeserializeEnum<StiShowSeriesLabels>(); 
                        break;

                    case "ShowShadow":
                        this.ShowShadow = property.DeserializeBool();
                        break;

                    case "FilterMode":
                        this.FilterMode = property.DeserializeEnum<StiFilterMode>(); 
                        break;

                    case "Filters":
                        this.Filters.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "Conditions":
                        this.Conditions.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "TopN":
                        this.TopN.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "YAxis":
                        this.YAxis = property.DeserializeEnum<StiSeriesYAxis>(); 
                        break;

                    case "SeriesLabels":
                        this.seriesLabels = StiSeriesLabels.LoadFromJsonObjectInternal((JObject)property.Value, (StiChart)this.chart);
                        break;

                    case "TrendLine":
#pragma warning disable CS0618 // Type or member is obsolete
                        this.TrendLine = StiTrendLine.CreateFromJsonObject((JObject)property.Value);
#pragma warning restore CS0618 // Type or member is obsolete
                        break;

                    case "TrendLines":
                        this.TrendLines.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "ValueDataColumn":
                        this.ValueDataColumn = property.DeserializeString();
                        break;

                    case "ArgumentDataColumn":
                        this.ArgumentDataColumn = property.DeserializeString();
                        break;

                    case "AutoSeriesTitleDataColumn":
                        this.AutoSeriesTitleDataColumn = property.DeserializeString();
                        break;

                    case "AutoSeriesKeyDataColumn":
                        this.AutoSeriesKeyDataColumn = property.DeserializeString();
                        break;

                    case "AutoSeriesColorDataColumn":
                        this.AutoSeriesColorDataColumn = property.DeserializeString();
                        break;

                    case "ToolTipDataColumn":
                        this.ToolTipDataColumn = property.DeserializeString();
                        break;

                    case "TagDataColumn":
                        this.TagDataColumn = property.DeserializeString();
                        break;

                    case "HyperlinkDataColumn":
                        this.HyperlinkDataColumn = property.DeserializeString();
                        break;

                    case "DrillDownEnabled":
                        this.DrillDownEnabled = property.DeserializeBool();
                        break;

                    case "DrillDownReport":
                        this.DrillDownReport = property.DeserializeString();
                        break;

                    case "DrillDownPageGuid":
                        this.DrillDownPageGuid = property.DeserializeString();
                        break;

                    case "AllowSeries":
                        this.AllowSeries = property.DeserializeBool();
                        break;

                    case "AllowSeriesElements":
                        this.AllowSeriesElements = property.DeserializeBool();
                        break;

                    case "Interaction":
                        this.Interaction.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "NewAutoSeriesEvent":
                        {
                            var newAutoSeriesEvent = new StiNewAutoSeriesEvent();
                            newAutoSeriesEvent.LoadFromJsonObject((JObject)property.Value);
                            this.NewAutoSeriesEvent = newAutoSeriesEvent;
                        }
                        break;

                    case "GetValueEvent":
                        {
                            var getValueEvent = new StiGetValueEvent();
                            getValueEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetValueEvent = getValueEvent;
                        }
                        break;

                    case "GetListOfValuesEvent":
                        {
                            var getListOfValuesEvent = new StiGetListOfValuesEvent();
                            getListOfValuesEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetListOfValuesEvent = getListOfValuesEvent;
                        }
                        break;

                    case "GetArgumentEvent":
                        {
                            var getArgumentEvent = new StiGetArgumentEvent();
                            getArgumentEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetArgumentEvent = getArgumentEvent;
                        }
                        break;

                    case "GetListOfArgumentsEvent":
                        {
                            var getListOfArgumentsEvent = new StiGetListOfArgumentsEvent();
                            getListOfArgumentsEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetListOfArgumentsEvent = getListOfArgumentsEvent;
                        }
                        break;

                    case "GetTitleEvent":
                        {
                            var getTitleEvent = new StiGetTitleEvent();
                            getTitleEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetTitleEvent = getTitleEvent;
                        }
                        break;

                    case "GetToolTipEvent":
                        {
                            var getToolTipEvent = new StiGetToolTipEvent();
                            getToolTipEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetToolTipEvent = getToolTipEvent;
                        }
                        break;

                    case "GetListOfToolTipsEvent":
                        {
                            var getListOfToolTipsEvent = new StiGetListOfToolTipsEvent();
                            getListOfToolTipsEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetListOfToolTipsEvent = getListOfToolTipsEvent;
                        }
                        break;

                    case "GetTagEvent":
                        {
                            var getTagEvent = new StiGetTagEvent();
                            getTagEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetTagEvent = getTagEvent;
                        }
                        break;

                    case "GetListOfTagsEvent":
                        {
                            var getListOfTagsEvent = new StiGetListOfTagsEvent();
                            getListOfTagsEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetListOfTagsEvent = getListOfTagsEvent;
                        }
                        break;

                    case "GetHyperlinkEvent":
                        {
                            var getHyperlinkEvent = new StiGetHyperlinkEvent();
                            getHyperlinkEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetHyperlinkEvent = getHyperlinkEvent;
                        }
                        break;

                    case "GetListOfHyperlinksEvent":
                        {
                            var getListOfHyperlinksEvent = new StiGetListOfHyperlinksEvent();
                            getListOfHyperlinksEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetListOfHyperlinksEvent = getListOfHyperlinksEvent;
                        }
                        break;

                    case "Value":
                        {
                            var valueObj = new StiExpression();
                            valueObj.LoadFromJsonObject((JObject)property.Value);
                            this.Value = valueObj;
                        }
                        break;

                    case "ListOfValues":
                        {
                            var expression = new StiListOfValuesExpression();
                            expression.LoadFromJsonObject((JObject)property.Value);
                            this.ListOfValues = expression;
                        }
                        break;

                    case "Argument":
                        {
                            var argument = new StiArgumentExpression();
                            argument.LoadFromJsonObject((JObject)property.Value);
                            this.Argument = argument;
                        }
                        break;

                    case "ListOfArguments":
                        {
                            var listOfArguments = new StiListOfArgumentsExpression();
                            listOfArguments.LoadFromJsonObject((JObject)property.Value);
                            this.ListOfArguments = listOfArguments;
                        }
                        break;

                    case "Title":
                        {
                            var title = new StiTitleExpression();
                            title.LoadFromJsonObject((JObject)property.Value);
                            this.Title = title;
                            this.TitleValue = title.Value;
                        }
                        break;

                    case "ToolTip":
                        {
                            var toolTip = new StiToolTipExpression();
                            toolTip.LoadFromJsonObject((JObject)property.Value);
                            this.ToolTip = toolTip;
                        }
                        break;

                    case "ListOfToolTips":
                        {
                            var listOfToolTips = new StiListOfToolTipsExpression();
                            listOfToolTips.LoadFromJsonObject((JObject)property.Value);
                            this.ListOfToolTips = listOfToolTips;
                        }
                        break;

                    case "Tag":
                        {
                            var tag = new StiTagExpression();
                            tag.LoadFromJsonObject((JObject)property.Value);
                            this.Tag = tag;
                        }
                        break;

                    case "ListOfTags":
                        {
                            var listOfTags = new StiListOfTagsExpression();
                            listOfTags.LoadFromJsonObject((JObject)property.Value);
                            this.ListOfTags = listOfTags;
                        }
                        break;

                    case "Hyperlink":
                        {
                            var hyperlink = new StiHyperlinkExpression();
                            hyperlink.LoadFromJsonObject((JObject)property.Value);
                            this.Hyperlink = hyperlink;
                        }
                        break;

                    case "ListOfHyperlinks":
                        {
                            var expression = new StiListOfHyperlinksExpression();
                            expression.LoadFromJsonObject((JObject)property.Value);
                            this.ListOfHyperlinks = expression;
                        }
                        break;

                    case "ValuesString":
                        this.ValuesString = property.DeserializeString();
                        break;

                    case "ArgumentsString":
                        this.ArgumentsString = property.DeserializeString();
                        break;

                    case "ToolTipsString":
                        this.ToolTipsString = property.DeserializeString();
                        break;

                    case "TagString":
                        this.TagString = property.DeserializeString();
                        break;

                    case "HyperlinkString":
                        this.HyperlinkString = property.DeserializeString();
                        break;

                    case "TitleValue":
                        this.TitleValue = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public virtual StiComponentId ComponentId => StiComponentId.StiSeries;

        [Browsable(false)]
        public string PropName => string.Empty;

        public abstract StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level);

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            var objectHelper = new StiEventCollection();

            // RenderEventsCategory
            var list = new[]
            {
                StiPropertyEventId.NewAutoSeriesEvent
            };
            objectHelper.Add(StiPropertyCategories.RenderEvents, list);

            list = new[]
            {
                StiPropertyEventId.GetValueEvent,
                StiPropertyEventId.GetListOfValuesEvent,
                StiPropertyEventId.GetArgumentEvent,
                StiPropertyEventId.GetListOfArgumentsEvent,
                StiPropertyEventId.GetTitleEvent,
                StiPropertyEventId.GetToolTipEvent,
                StiPropertyEventId.GetListOfToolTipsEvent,
                StiPropertyEventId.GetTagEvent,
                StiPropertyEventId.GetListOfTagsEvent,
                StiPropertyEventId.GetHyperlinkEvent,
                StiPropertyEventId.GetListOfHyperlinksEvent
            };
            objectHelper.Add(StiPropertyCategories.ValueEvents, list);

            return objectHelper;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var series = base.Clone() as StiSeries;

            series.Title = this.Title.Clone() as StiTitleExpression;
            series.Values = this.Values.Clone() as double?[];
            series.Arguments = this.Arguments.Clone() as object[];
            series.Tags = this.Tags.Clone() as object[];
            series.ToolTips = this.ToolTips.Clone() as string[];
            series.Hyperlinks = this.Hyperlinks.Clone() as string[];
            series.TopN = this.TopN.Clone() as StiSeriesTopN;
            series.Interaction = this.Interaction.Clone() as IStiSeriesInteraction;
            series.SeriesLabels = this.SeriesLabels.Clone() as IStiSeriesLabels;

            if (this.Core != null)
            {
                series.Core = this.Core.Clone() as StiSeriesCoreXF;
                series.Core.Series = series;
            }

            return series;
        }
        #endregion

        #region IStiChartPainter
        protected virtual void BaseTransform(object context, float x, float y, float angle, float dx, float dy)
        {
        }
        #endregion

        #region IStiSeriesParent
        [Browsable(false)]
        public StiComponent Parent => Chart as StiComponent;
        #endregion

        #region StiService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => Core.LocalizedName;

        /// <summary>
        /// Gets a service category.
        /// </summary>
        [Browsable(false)]
        public sealed override string ServiceCategory => "Chart";

        /// <summary>
        /// Gets a service type.
        /// </summary>
        [Browsable(false)]
        public sealed override Type ServiceType => typeof(StiSeries);
        #endregion

        #region Properties
        [Browsable(false)]
        public StiSeriesCoreXF Core { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that chart style will be used.
        /// </summary>
        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that chart style will be used.")]
        [DefaultValue(true)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool AllowApplyStyle { get; set; } = true;

        /// <summary>
        /// Gets or sets string that is used to format series values.
        /// </summary>
        [DefaultValue("")]
        [StiCategory("Data")]
        [StiSerializable]
        [Editor("Stimulsoft.Report.Chart.Design.StiFormatEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets string that is used to format series values.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public string Format { get; set; } = "";

        /// <summary>
        /// Gets or sets mode of series values sorting.
        /// </summary>
        [DefaultValue(StiSeriesSortType.None)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [Description("Gets or sets mode of series values sorting.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiSeriesSortType SortBy { get; set; } = StiSeriesSortType.None;

        /// <summary>
        /// Gets or sets sort direction.
        /// </summary>
        [DefaultValue(StiSeriesSortDirection.Ascending)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [Description("Gets or sets sort direction.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiSeriesSortDirection SortDirection { get; set; } = StiSeriesSortDirection.Ascending;

        /// <summary>
        /// Gets or sets value which indicates that series must be shown in legend.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets value which indicates that series must be shown in legend.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public bool ShowInLegend { get; set; } = true;

        [DefaultValue(true)]
        [Browsable(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Obsolete("Please use ShowSeriesLabels property instead ShowLabels property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowLabels
        {
            get
            {
                return true;
            }
            set
            {
                if (value)
                    ShowSeriesLabels = StiShowSeriesLabels.FromChart;
            }
        }

        private StiShowSeriesLabels showSeriesLabels = StiShowSeriesLabels.FromChart;
        /// <summary>
        /// Gets or sets series labels output mode.
        /// </summary>
        [DefaultValue(StiShowSeriesLabels.FromChart)]
        [StiSerializable]
        [TypeConverter(typeof(Design.StiShowSeriesLabelsEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets series labels output mode.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiShowSeriesLabels ShowSeriesLabels
        {
            get
            {
                return showSeriesLabels;
            }
            set
            {
#pragma warning disable 612, 618
                showSeriesLabels = (value == StiShowSeriesLabels.None) ? StiShowSeriesLabels.FromChart : value;
#pragma warning restore 612, 618
            }
        }

        /// <summary>
        /// Gets or sets value which indicates draw shadow or no.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Appearance")]
        [Description("Gets or sets value which indicates draw shadow or no.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool ShowShadow { get; set; } = true;

        /// <summary>
        /// Gets or sets filter mode of series.
        /// </summary>
        [DefaultValue(StiFilterMode.And)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [Description("Gets or sets filter mode of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiFilterMode FilterMode { get; set; } = StiFilterMode.And;

        /// <summary>
        /// Gets or sets filters which used to filter series values.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Data")]
        [Description("Gets or sets filters which used to filter series values.")]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartFiltersEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiChartFiltersCollection Filters { get; set; } = new StiChartFiltersCollection();

        private bool ShouldSerializeFilters()
        {
            return Filters == null || Filters.Count > 0;
        }

        /// <summary>
        /// Gets or sets collection of conditions which can be used to change behavior of series.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Data")]
        [TypeConverter(typeof(Design.StiChartConditionsCollectionConverter))]
        [Description("Gets or sets collection of conditions which can be used to change behavior of series.")]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartConditionsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiChartConditionsCollection Conditions { get; set; } = new StiChartConditionsCollection();

        private bool ShouldSerializeConditions()
        {
            return Conditions == null || Conditions.Count > 0;
        }

        /// <summary>
        /// Gets or sets parameters of displaying top results.
        /// </summary>
        [StiCategory("Data")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(Design.StiSeriesTopNConverter))]
        [Description("Gets or sets parameters of displaying top results.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual IStiSeriesTopN TopN { get; set; } = new StiSeriesTopN();

        private bool ShouldSerializeTopN()
        {
            return TopN == null || !TopN.IsDefault;
        }

        [StiNonSerialized]
        [Browsable(false)]
        public virtual bool TopNAllowed => true;

        /// <summary>
        /// Gets or sets Y Axis for series on which will output string representation of arguments.
        /// </summary>
        [DefaultValue(StiSeriesYAxis.LeftYAxis)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Common")]
        [Description("Gets or sets Y Axis for series on which will output string representation of arguments.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiSeriesYAxis YAxis { get; set; } = StiSeriesYAxis.LeftYAxis;

        private IStiSeriesLabels seriesLabels;
        /// <summary>
        /// Gets or sets series labels settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiCategory("Common")]
        [Description("Gets or sets series labels settings.")]
        [TypeConverter(typeof(Design.StiSeriesLabelsConverter))]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartLabelsEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        public IStiSeriesLabels SeriesLabels
        {
            get
            {
                return seriesLabels;
            }
            set
            {
                seriesLabels = value;
                seriesLabels.Chart = this.Chart;
            }
        }

        /// <summary>
        /// Gets or sets trend line settings.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Common")]
        [Description("Gets or sets trend lines settings.")]
        [TypeConverter(typeof(Design.StiTrendLinesCollectionConverter))]
        [StiPropertyLevel(StiLevel.Standard)]
        [Editor("Stimulsoft.Report.Chart.Design.StiChartTrendLinesEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual StiTrendLinesCollection TrendLines { get; set; } = new StiTrendLinesCollection();

        private bool ShouldSerializeTrendLines()
        {
            return TrendLines == null || TrendLines.Count > 0;
        }

        /// <summary>
        /// Do not use this property.
        /// </summary>
        [Browsable(false)]
        [StiNonSerialized]
        [Obsolete("TrendLine property is obsolete. Please use TrendLines property.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual IStiTrendLine TrendLine {
            get
            {
                if (TrendLines != null && TrendLines.Count > 0)
                    return TrendLines[0];

                return new StiTrendLineNone();
            }
            set
            {
                TrendLines.Insert(0, value as StiTrendLine);
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public virtual bool TrendLineAllowed => true;

        /// <summary>
        /// Internal use only. Special for DBS.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        internal bool IsTotalLabel { get; set; }

        private IStiChart chart;
        [StiSerializable(StiSerializationVisibility.Reference)]
        [Browsable(false)]
        public IStiChart Chart
        {
            get
            {
                return chart;
            }
            set
            {
                if (chart != value)
                {
                    chart = value;

                    if (value != null)
                        this.SeriesLabels.Chart = value;
                }
            }
        }

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        internal Color? ColorLegend { get; set; }
        #endregion

        #region Properties.Data

        #region Values Start
        [Browsable(false)]
        public double?[] ValuesStart { get; set; } = { };
        #endregion

        #region Values
        private double?[] values = { 1, 3, 2 };
        [Browsable(false)]
        public double?[] Values
        {
            get
            {
                if (Chart == null || !Chart.IsDesigning || IsDashboard)
                    return values;

                double?[] defaultValues;
                var seriesIndex = Chart.Series.IndexOf(this);

                if (this is IStiWaterfallSeries)
                {
                    var waterfallSeries = this as IStiWaterfallSeries;
                    if (waterfallSeries.Total.Visible)
                        defaultValues = new double?[] { 3, -1, 2, 4 };
                    else
                        defaultValues = new double?[] { 3, -1, 2 };
                }

                else if (this is IStiBoxAndWhiskerSeries)
                    defaultValues = new double?[] { 8, 2, 3, 9, 10 };

                else if (this is IStiHistogramSeries)
                    defaultValues = new double?[] { 4, 2, 1 };

                else if (!string.IsNullOrEmpty(ListOfValues.Value) && !ListOfValues.Value.Contains("{"))
                    defaultValues = GetNullableValuesFromString(this, ListOfValues.Value);

                else if (this is IStiSteppedRangeSeries)
                    defaultValues = new double?[] { 1 + seriesIndex * 3, 3 + seriesIndex * 4, 2 + seriesIndex * 3, 3 + seriesIndex * 3, 3 + seriesIndex * 3 };

                else if (this is IStiFunnelSeries)
                    defaultValues = new double?[] { 3, 2, 1 };

                else if (this is IStiGanttSeries)
                    defaultValues = new double?[] { 1 + seriesIndex * 3, 3 + seriesIndex * 4, 7 + seriesIndex * 3 };

                else if (this is IStiScatterSeries)
                    defaultValues = new double?[] { 1 + seriesIndex, 6 + seriesIndex, 2 + seriesIndex };

                else if (this is IStiPieSeries)
                {
                    defaultValues = seriesIndex == 0
                        ? new double?[] { 1, 3, 2 }
                        : new double?[] { 1 + seriesIndex, 3 + seriesIndex, 4 + seriesIndex };
                }
                else
                {
                    if (this is StiRadarSeries)
                        defaultValues = new double?[] { 1 + seriesIndex, 2 + seriesIndex, 3 + seriesIndex, 4 + seriesIndex, 5 + seriesIndex };

                    else if (seriesIndex == 0)
                        defaultValues = values;

                    else
                        defaultValues = new double?[] { 1 + seriesIndex, 3 + seriesIndex, 2.5 + seriesIndex };
                }

                var offset = this.GetOffsetForValues();
                if (offset != 0)
                {
                    var correctionValues = new double?[defaultValues.Length];

                    for (int index = 0; index < defaultValues.Length; index++)
                    {
                        correctionValues[index] = defaultValues[index] + offset;
                    }

                    return correctionValues;
                }

                return defaultValues;
            }
            set
            {
                #region Prepare Values Start
                var preparedValues = new double?[value.Length];
                for (var index = 0; index < value.Length; index++)
                {
                    if (index < values.Length && ValuesStart?.Length > 0)
                        preparedValues[index] = values[index];
                    else
                        preparedValues[index] = 0;
                }
                #endregion

                this.ValuesStart = preparedValues;
                values = value;
            }
        }

        /// <summary>
        /// Gets or sets a name of the column that contains the point value.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.ValueValueDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Value")]
        [Description("Gets or sets a name of the column that contains the value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual string ValueDataColumn { get; set; } = string.Empty;

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ValuesString
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (double? value in values)
                {
                    if (first)
                        sb.AppendFormat("{0}", value == null ? 0d : value);
                    else
                        sb.AppendFormat(";{0}", value == null ? 0d : value);

                    first = false;
                }
                return sb.ToString();
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    values = new double?[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    values = new double?[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        values[index++] = double.Parse(str);
                    }
                }
            }
        }
        #endregion

        #region Arguments
        /// <summary>
        /// This array contains argument values without any formatting. Internal dashboards use only!
        /// </summary>
        [Browsable(false)]
        public object[] OriginalArguments { get; set; }

        private object[] arguments = new object[0];
        [Browsable(false)]
        public virtual object[] Arguments
        {
            get
            {
                if (Chart == null || !Chart.IsDesigning || IsDashboard)
                    return arguments;

                if (this is IStiWaterfallSeries)
                {
                    var waterfallSeries = this as IStiWaterfallSeries;
                    if (waterfallSeries.Total.Visible)
                        return new object[] { "A", "B", "C", waterfallSeries.Total.Text };
                    else
                        return new object[] { "A", "B", "C" };
                }

                if (this is IStiHistogramSeries)
                    return new object[] { "[1, 2]", "(2, 3]", "(3, 4]" };

                if (!string.IsNullOrEmpty(ListOfArguments.Value))
                    return GetArgumentsFromString(ListOfArguments.Value);

                if ((this is IStiTreemapSeries || this is IStiGanttSeries || this is IStiRangeBarSeries || this is IStiCandlestickSeries || this is StiSunburstSeries) && (arguments == null || arguments.Length == 0))
                    return new object[] { "A", "B", "C" };

                if ((this is IStiScatterSeries || this is IStiScatterLineSeries || this is IStiScatterSplineSeries) && (arguments == null || arguments.Length == 0))
                    return new object[] { "1", "5", "4" };

                return arguments;
            }
            set
            {
                arguments = value;
            }
        }

        /// <summary>
        /// Gets or sets a name of the column that contains the argument value.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.ArgumentArgumentDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Argument")]
        [DefaultValue("")]
        [Description("Gets or sets a name of the column that contains the argument value.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual string ArgumentDataColumn { get; set; } = string.Empty;

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ArgumentsString
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (object arg in arguments)
                {
                    if (arg != null)
                    {
                        if (first)
                            sb.AppendFormat("{0}", XmlConvert.EncodeName(arg.ToString()));
                        else
                            sb.AppendFormat(";{0}", XmlConvert.EncodeName(arg.ToString()));

                        first = false;
                    }
                }
                return sb.ToString();
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    arguments = new object[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    arguments = new object[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        arguments[index++] = XmlConvert.DecodeName(str);
                    }
                }
            }
        }
        #endregion

        #region AutoSeries
        /// <summary>
        /// Gets or sets a name of the column that contains the title of auto created series.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.DataAutoSeriesTitleDataColumn)]
        [DefaultValue("")]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Series")]
        [Description("Gets or sets a name of the column that contains the title of auto created series.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public string AutoSeriesTitleDataColumn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a name of the column that contains the key of auto created series.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Series")]
        [StiOrder(StiSeriesPropertyOrder.DataAutoSeriesKeyDataColumn)]
        [DefaultValue("")]
        [Description("Gets or sets a name of the column that contains the key of auto created series.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public string AutoSeriesKeyDataColumn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a name of the column that contains the color of auto created series. Color must be presented as string.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Series")]
        [StiOrder(StiSeriesPropertyOrder.DataAutoSeriesColorDataColumn)]
        [DefaultValue("")]
        [Description("Gets or sets a name of the column that contains the color of auto created series. Color must be presented as string.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public string AutoSeriesColorDataColumn { get; set; } = string.Empty;
        #endregion

        #region ToolTips
        private string[] toolTips = new string[0];
        [Browsable(false)]
        public virtual string[] ToolTips
        {
            get
            {
                if (Chart == null || !Chart.IsDesigning || IsDashboard)
                    return toolTips;

                if (!string.IsNullOrEmpty(ListOfToolTips.Value))
                    return GetStringsFromString(ListOfToolTips.Value);

                return toolTips;
            }
            set
            {
                toolTips = value;
            }
        }

        /// <summary>
        /// Gets or sets a name of the column that contains the tool tip value.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiOrder(StiSeriesPropertyOrder.ToolTipDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Argument")]
        [DefaultValue("")]
        [Description("Gets or sets a name of the column that contains the tool tip value.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public string ToolTipDataColumn { get; set; } = string.Empty;


        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ToolTipsString
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (object arg in toolTips)
                {
                    if (arg != null)
                    {
                        if (first)
                            sb.AppendFormat("{0}", XmlConvert.EncodeName(arg.ToString()));
                        else
                            sb.AppendFormat(";{0}", XmlConvert.EncodeName(arg.ToString()));

                        first = false;
                    }
                }
                return sb.ToString();
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    toolTips = new string[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    toolTips = new string[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        toolTips[index++] = XmlConvert.DecodeName(str);
                    }
                }
            }
        }
        #endregion

        #region Tags
        private object[] tags = new object[0];
        [Browsable(false)]
        public virtual object[] Tags
        {
            get
            {
                if (Chart == null || !Chart.IsDesigning || IsDashboard)
                    return tags;

                if (!string.IsNullOrEmpty(ListOfTags.Value))
                    return GetArgumentsFromString(ListOfTags.Value);

                return tags;
            }
            set
            {
                tags = value;
            }
        }

        /// <summary>
        /// Gets or sets a name of the column that contains the tag value.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiOrder(StiSeriesPropertyOrder.TagDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Argument")]
        [DefaultValue("")]
        [Description("Gets or sets a name of the column that contains the tag value.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public string TagDataColumn { get; set; } = string.Empty;

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string TagString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (object arg in tags)
                {
                    if (arg != null)
                    {
                        if (first)
                            sb.AppendFormat("{0}", XmlConvert.EncodeName(arg.ToString()));
                        else
                            sb.AppendFormat(";{0}", XmlConvert.EncodeName(arg.ToString()));

                        first = false;
                    }
                }
                return sb.ToString();
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    tags = new object[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    tags = new object[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        tags[index++] = XmlConvert.DecodeName(str);
                    }
                }
            }
        }
        #endregion

        #region Hyperlinks
        private string[] hyperlinks = new string[0];
        [Browsable(false)]
        public virtual string[] Hyperlinks
        {
            get
            {
                if (Chart == null || !Chart.IsDesigning || IsDashboard)
                    return hyperlinks;

                if (!string.IsNullOrEmpty(ListOfHyperlinks.Value))
                    return GetStringsFromString(ListOfHyperlinks.Value);

                return hyperlinks;
            }
            set
            {
                hyperlinks = value;
            }
        }

        /// <summary>
        /// Gets or sets a name of the column that contains the hyperlink value.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiOrder(StiSeriesPropertyOrder.HyperlinkDataColumn)]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Argument")]
        [DefaultValue("")]
        [Description("Gets or sets a name of the column that contains the hyperlink value.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public string HyperlinkDataColumn { get; set; } = string.Empty;


        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string HyperlinkString
        {
            get
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (object arg in hyperlinks)
                {
                    if (arg != null)
                    {
                        if (first)
                            sb.AppendFormat("{0}", XmlConvert.EncodeName(arg.ToString()));
                        else
                            sb.AppendFormat(";{0}", XmlConvert.EncodeName(arg.ToString()));

                        first = false;
                    }
                }
                return sb.ToString();
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    hyperlinks = new string[0];
                }
                else
                {
                    string[] strs = value.Split(new char[] { ';' });

                    hyperlinks = new string[strs.Length];

                    int index = 0;
                    foreach (string str in strs)
                    {
                        hyperlinks[index++] = XmlConvert.DecodeName(str);
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Properties.DrillDown
        /// <summary>
        /// Gets or sets value which indicates whether the Drill-Down operation can be executed.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether the Drill-Down operation can be executed.")]
        [StiOrder(StiSeriesPropertyOrder.DrillDownEnabled)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool DrillDownEnabled { get; set; }

        /// <summary>
        /// Gets or sets a path to a report for the Drill-Down operation.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [DefaultValue("")]
        [Description("Gets or sets a path to a report for the Drill-Down operation.")]
        [StiOrder(StiSeriesPropertyOrder.DrillDownReport)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual string DrillDownReport { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a page for the Drill-Down operation.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        [Editor("Stimulsoft.Report.Components.Design.StiInteractionDrillDownPageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [DefaultValue(null)]
        [Description("Gets or sets a page for the Drill-Down operation.")]
        [StiOrder(StiSeriesPropertyOrder.DrillDownPage)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiPage DrillDownPage
        {
            get
            {
                if (Chart == null || ((StiChart)Chart).Report == null)
                    return null;

                foreach (StiPage page in ((StiChart)Chart).Report.Pages)
                {
                    if (page.Guid == this.DrillDownPageGuid)
                        return page;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    this.DrillDownPageGuid = null;
                }
                else
                {
                    if (value.Guid == null)
                        value.Guid = StiGuidUtils.NewGuid();

                    this.DrillDownPageGuid = value.Guid;
                }
            }
        }

        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(null)]
        public string DrillDownPageGuid { get; set; }

        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(true)]
        public bool AllowSeries { get; set; } = true;

        [Browsable(false)]
        [StiSerializable]
        [DefaultValue(true)]
        public bool AllowSeriesElements { get; set; } = true;
        #endregion

        #region Properties.Core
        [Browsable(false)]
        public string CoreTitle
        {
            get
            {
                var value = Chart != null && Chart.IsDesigning ? Title.Value : TitleValue;

                if (value == null)
                    value = string.Empty;

                return value;
            }
            set
            {
                Title.Value = value;
                TitleValue = value;
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that this series is used in dashboards mode and IsDesign property will ingore.
        /// </summary>
        [Browsable(false)]
        internal bool IsDashboard { get; set; }

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public bool IsDateTimeValues
        {
            get
            {
                return this.Core.IsDateTimeValues;
            }
            set
            {
                this.Core.IsDateTimeValues = value;
            }
        }
        #endregion

        #region Interaction
        private IStiSeriesInteraction interaction;
        /// <summary>
        /// Gets interaction options of this series.
        /// </summary>
        [StiCategory("Common")]
        [Description("Gets interaction options of this series.")]
        [TypeConverter(typeof(Components.Design.StiSeriesInteractionConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public virtual IStiSeriesInteraction Interaction
        {
            get
            {
                return interaction;
            }
            set
            {
                if (this.interaction != value)
                {
                    interaction = value;

                    if (value != null)
                        ((StiSeriesInteraction)interaction).ParentSeries = this;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Used for design time.
        /// </summary>
        protected double GetOffsetForValues()
        {
            var offset = 0d;
            var area = this.Chart.Area as StiAxisArea;
            if (area != null && !area.YAxis.Range.Auto)
            {
                offset = area.YAxis.Range.Minimum;
            }
            return offset;
        }

        internal double?[] GetApproximationValuesStart()
        {
            if (this.Chart.IsDesigning)
                return this.ValuesStart;

            if (this.Values == null || this.ValuesStart == null)
                return this.ValuesStart;

            var maxValue = ((IStiAxisArea)this.chart.Area).YAxis.Info.Maximum;

            var minValueStart = 0d;
            foreach (StiSeries series in this.chart.Series)
                minValueStart = Math.Min(minValueStart, series.ValuesStart.ToList().Min().GetValueOrDefault());

            var maxValueStart = 0d;
            foreach (StiSeries series in this.chart.Series)
                maxValueStart = Math.Max(maxValueStart, series.ValuesStart.ToList().Max().GetValueOrDefault());

            ((IStiAxisArea)this.chart.Area).AxisCore.CalculateMinimumAndMaximumYAxis(ref minValueStart, ref maxValueStart);

            var factor = maxValue == 0 || maxValueStart == 0 ? 0 : maxValue / maxValueStart;

            var resultValues = new double?[this.ValuesStart.Length];

            for (var index = 0; index < this.ValuesStart.Length; index++)
            {
                var currentValue = this.ValuesStart[index];
                resultValues[index] = currentValue.GetValueOrDefault() * factor;
            }

            return resultValues;
        }

        public Color ProcessSeriesColors(int pointIndex, Color seriesColor)
        {
            foreach (StiChartCondition condition in this.Conditions)
            {
                if (GetConditionResult(pointIndex, condition))
                    return condition.Color;
            }
            return seriesColor;
        }

        public StiMarkerType ProcessSeriesMarkerType(int pointIndex, StiMarkerType markerType)
        {
            foreach (StiChartCondition condition in this.Conditions)
            {
                if (GetConditionResult(pointIndex, condition))
                    return condition.MarkerType;
            }
            return markerType;
        }

        public float ProcessSeriesMarkerAngle(int pointIndex, float markerAngle)
        {
            foreach (StiChartCondition condition in this.Conditions)
            {
                if (GetConditionResult(pointIndex, condition))
                    return condition.MarkerAngle;
            }
            return markerAngle;
        }

        public bool ProcessSeriesMarkerVisible(int pointIndex)
        {
            foreach (StiChartCondition condition in this.Conditions)
            {
                if (GetConditionResult(pointIndex, condition))
                    return true;
            }
            return false;
        }

        public StiBrush ProcessSeriesBrushes(int pointIndex, StiBrush seriesBrush)
        {
            foreach (StiChartCondition condition in this.Conditions)
            {
                if (GetConditionResult(pointIndex, condition))
                {
                    if (this.AllowApplyStyle && this.Chart.Style != null)
                        seriesBrush = this.Chart.Style.Core.GetColumnBrush(condition.Color);
                    else
                        seriesBrush = new StiSolidBrush(condition.Color);

                    if (this.Chart.Area is IStiClusteredBarArea)
                    {
                        if (seriesBrush is StiGradientBrush)
                            ((StiGradientBrush)seriesBrush).Angle += 90;

                        if (seriesBrush is StiGlareBrush)
                            ((StiGlareBrush)seriesBrush).Angle += 90;
                    }
                    return seriesBrush;
                }
            }
            return seriesBrush;
        }

        private bool GetConditionResult(int pointIndex, StiChartCondition condition)
        {
            if ((this.Values == null || this.Values.Length <= pointIndex) && !(this is IStiFinancialSeries))
                return false;

            object itemValue = null;
            object itemValueEnd = null;

            object itemTooltip = null;

            object itemValueOpen = null;
            object itemValueClose = null;
            object itemValueLow = null;
            object itemValueHigh = null;

            object itemArgument = null;

            if ((this.chart.Area is IStiAxisArea && ((IStiAxisArea)this.chart.Area).ReverseHor && !(this.Chart.Area is IStiClusteredBarArea)) ||
                (this.Chart.Area is IStiClusteredBarArea && ((IStiAxisArea)this.chart.Area).ReverseVert))
            {
                if (this.Values != null && pointIndex < this.Values.Length)
                    itemValue = this.Values[this.Values.Length - pointIndex - 1];

                if (this is IStiRangeSeries && pointIndex < ((IStiRangeSeries)this).ValuesEnd.Length)
                    itemValueEnd = ((IStiRangeSeries)this).ValuesEnd[((IStiRangeSeries)this).ValuesEnd.Length - pointIndex - 1];

                if (this is IStiFinancialSeries)
                {
                    if (pointIndex < ((IStiFinancialSeries)this).ValuesOpen.Length)
                        itemValueOpen = ((IStiFinancialSeries)this).ValuesOpen[((IStiFinancialSeries)this).ValuesOpen.Length - pointIndex - 1];

                    if (pointIndex < ((IStiFinancialSeries)this).ValuesClose.Length)
                        itemValueClose = ((IStiFinancialSeries)this).ValuesClose[((IStiFinancialSeries)this).ValuesClose.Length - pointIndex - 1];

                    if (pointIndex < ((IStiFinancialSeries)this).ValuesLow.Length)
                        itemValueLow = ((IStiFinancialSeries)this).ValuesLow[((IStiFinancialSeries)this).ValuesLow.Length - pointIndex - 1];

                    if (pointIndex < ((IStiFinancialSeries)this).ValuesHigh.Length)
                        itemValueHigh = ((IStiFinancialSeries)this).ValuesHigh[((IStiFinancialSeries)this).ValuesHigh.Length - pointIndex - 1];
                }

                if (this.Arguments != null && pointIndex < this.Arguments.Length)
                    itemArgument = this.Arguments[this.Arguments.Length - pointIndex - 1];

                if (this.ToolTips != null && pointIndex < this.ToolTips.Length)
                    itemTooltip = this.ToolTips[this.ToolTips.Length - pointIndex - 1];
            }
            else
            {
                if (this.Values != null && pointIndex < this.Values.Length)
                    itemValue = this.Values[pointIndex];

                if (this is IStiRangeSeries && pointIndex < ((IStiRangeSeries)this).ValuesEnd.Length)
                    itemValueEnd = ((IStiRangeSeries)this).ValuesEnd[pointIndex];

                if (this is IStiFinancialSeries)
                {
                    if (pointIndex < ((IStiFinancialSeries)this).ValuesOpen.Length)
                        itemValueOpen = ((IStiFinancialSeries)this).ValuesOpen[pointIndex];

                    if (pointIndex < ((IStiFinancialSeries)this).ValuesClose.Length)
                        itemValueClose = ((IStiFinancialSeries)this).ValuesClose[pointIndex];

                    if (pointIndex < ((IStiFinancialSeries)this).ValuesLow.Length)
                        itemValueLow = ((IStiFinancialSeries)this).ValuesLow[pointIndex];

                    if (pointIndex < ((IStiFinancialSeries)this).ValuesHigh.Length)
                        itemValueHigh = ((IStiFinancialSeries)this).ValuesHigh[pointIndex];
                }

                if (this.Arguments != null && pointIndex < this.Arguments.Length)
                    itemArgument = this.Arguments[pointIndex];

                if (this.ToolTips != null && pointIndex < this.ToolTips.Length)
                    itemTooltip = this.ToolTips[pointIndex];
            }
            object data = StiChartHelper.GetFilterData(null, condition, null);

            return StiChartHelper.GetFilterResult(condition, itemArgument, itemValue, itemValueEnd, itemValueOpen,
                                                  itemValueClose, itemValueLow, itemValueHigh, itemTooltip, data);
        }

        public override string ToString()
        {
            return ServiceName;
        }

        internal static bool TryParseValue(string value, string culture, out double result)
        {
            CultureInfo storedCulture = null;

            try
            {
                if (!string.IsNullOrEmpty(culture))
                {
                    storedCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                }

                return double.TryParse(value, out result);
            }
            finally
            {
                if (storedCulture != null)
                    Thread.CurrentThread.CurrentCulture = storedCulture;
            }
        }

        public static double?[] GetNullableValuesFromString(StiSeries series, string list)
        {
            var alValue = new List<double?>();

            var culture = ((StiChart)series.Chart).Report.GetParsedCulture();
            var points = list.Split(new char[] { ';' });
            foreach (string point in points)
            {
                if (point == null || point.Trim().Length == 0) continue;

                double? value = 0d;
                double result;
                                
                if (TryParseValue(point, culture, out result))
                {
                    value = result;
                }
                else
                {
                    DateTime resultDateTime;
                    if (!string.IsNullOrEmpty(culture))
                    {
                        #region Try Parse with report Culture
                        try
                        {
                            var cultureInfo = new CultureInfo(culture);

                            if (DateTime.TryParse(point, cultureInfo, DateTimeStyles.None, out resultDateTime))
                            {
                                series.Core.IsDateTimeValues = true;
                                value = resultDateTime.ToOADate();
                            }
                        }
                        catch
                        {
                        }
                        #endregion
                    }
                    else
                    {
                        if (DateTime.TryParse(point, out resultDateTime))
                        {
                            #region Try Parse with System Culture
                            series.Core.IsDateTimeValues = true;
                            value = resultDateTime.ToOADate();
                            #endregion
                        }
                        else
                        {
                            if (DateTime.TryParse(point, new CultureInfo("en-US", false), DateTimeStyles.None, out resultDateTime))
                            {
                                #region Try Parse with en-US Culture
                                series.Core.IsDateTimeValues = true;
                                value = resultDateTime.ToOADate();
                                #endregion
                            }
                        }
                    }
                }

                alValue.Add(value);
            }

            return alValue.ToArray();
        }

        public static double[] GetValuesFromString(string list)
        {
            var alValue = new List<double>();

            string[] points = list.Split(new char[] { ';' });
            foreach (string point in points)
            {
                if (point != null && point.Trim().Length > 0)
                {
                    double value = 0d;
                    try
                    {
                        value = (double)StiReport.ChangeType(point, typeof(double));
                    }
                    catch
                    {
                    }
                    alValue.Add(value);
                }
            }

            return alValue.ToArray();
        }

        public static string[] GetStringsFromString(string list)
        {
            var alValue = new List<string>();

            var points = list.Split(new char[] { ';' });
            foreach (string point in points)
            {
                if (point != null && point.Trim().Length > 0)
                {
                    alValue.Add(point);
                }
            }

            return alValue.ToArray();
        }

        public static object[] GetArgumentsFromString(string list)
        {
            return list.Split(new char[] { ';' });
        }
        #endregion

        #region Methods.virtual
        public virtual StiSeries CreateNew()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Methods.Types
        public abstract Type GetDefaultAreaType();
        #endregion

        #region Events
        #region NewAutoSeries
        /// <summary>
        /// Occurs when new auto series is created.
        /// </summary>
        public event StiNewAutoSeriesEventHandler NewAutoSeries;

        public void InvokeNewAutoSeries(StiNewAutoSeriesEventArgs e)
        {
            this.NewAutoSeries?.Invoke(e.Series, e);
        }

        /// <summary>
        /// Occurs when new auto series is created.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when new auto series is created.")]
        public StiNewAutoSeriesEvent NewAutoSeriesEvent { get; set; } = new StiNewAutoSeriesEvent();
        #endregion

        #region GetValue
        /// <summary>
        /// Occurs when getting the property Value.
        /// </summary>
        public event StiGetValueEventHandler GetValue;

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        protected virtual void OnGetValue(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        public virtual void InvokeGetValue(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetValue(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesValue**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(Value.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                this.GetValue?.Invoke(sender, e);

                StiBlocklyHelper.InvokeBlockly(((StiChart)this.Chart).Report, sender, GetValueEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetValue property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";
                
                StiLogService.Write(GetType(), str);
                StiLogService.Write(GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the property Value.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the property Value.")]
        public StiGetValueEvent GetValueEvent { get; set; } = new StiGetValueEvent();
        #endregion

        #region GetListOfValues
        /// <summary>
        /// Occurs when getting the list of values.
        /// </summary>
        public event StiGetValueEventHandler GetListOfValues;

        /// <summary>
        /// Raises the values event.
        /// </summary>
        protected virtual void OnGetListOfValues(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetListOfValues event.
        /// </summary>
        public void InvokeGetListOfValues(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfValues(e);

                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesListOfValues**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(ListOfValues, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                this.GetListOfValues?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetListOfValues property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(GetType(), str);
                StiLogService.Write(GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the list of values.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the list of values.")]
        public StiGetListOfValuesEvent GetListOfValuesEvent { get; set; } = new StiGetListOfValuesEvent();
        #endregion

        #region GetArgument
        /// <summary>
        /// Occurs when getting the property Argument.
        /// </summary>
        public event StiValueEventHandler GetArgument;

        /// <summary>
        /// Raises the GetArgument event.
        /// </summary>
        protected virtual void OnGetArgument(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetArgument event.
        /// </summary>
        public virtual void InvokeGetArgument(StiComponent sender, StiValueEventArgs e)
        {
            try
            {
                OnGetArgument(e);

                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesArgument**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(Argument.Value, tempText);
                    e.Value = string.IsNullOrEmpty(Argument.Value) ? null : sender.Report.ToString(parserResult);
                }
                this.GetArgument?.Invoke(sender, e);

                StiBlocklyHelper.InvokeBlockly(((StiChart)this.Chart).Report, this, GetArgumentEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetArgument property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(GetType(), str);
                StiLogService.Write(GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the property Argument.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the property Argument.")]
        public StiGetArgumentEvent GetArgumentEvent { get; set; } = new StiGetArgumentEvent();
        #endregion

        #region GetListOfArguments
        /// <summary>
        /// Occurs when getting the list of arguments.
        /// </summary>
        public event StiGetValueEventHandler GetListOfArguments;

        /// <summary>
        /// Raises the Arguments event.
        /// </summary>
        protected virtual void OnGetListOfArguments(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetListOfArguments event.
        /// </summary>
        public void InvokeGetListOfArguments(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfArguments(e);

                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesListOfArguments**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(ListOfArguments, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                this.GetListOfArguments?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetListOfArguments property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the list of arguments.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the list of arguments.")]
        public StiGetListOfArgumentsEvent GetListOfArgumentsEvent { get; set; } = new StiGetListOfArgumentsEvent();
        #endregion

        #region GetTitle
        /// <summary>
        /// Occurs when getting the property Title.
        /// </summary>
        public event StiGetTitleEventHandler GetTitle;

        /// <summary>
        /// Raises the GetTitle event.
        /// </summary>
        protected virtual void OnGetTitle(StiGetTitleEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetTitle event.
        /// </summary>
        public virtual void InvokeGetTitle(StiComponent sender, StiGetTitleEventArgs e)
        {
            try
            {
                OnGetTitle(e);

                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesTitle**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(Title.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                this.GetTitle?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in Title property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(GetType(), str);
                StiLogService.Write(GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the property Title.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the property Title.")]
        public StiGetTitleEvent GetTitleEvent { get; set; } = new StiGetTitleEvent();
        #endregion

        #region GetToolTip
        /// <summary>
        /// Occurs when getting the ToolTip for the series.
        /// </summary>
        public event StiValueEventHandler GetToolTip;

        /// <summary>
        /// Raises the GetToolTip event.
        /// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnGetToolTip(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetToolTip event.
        /// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public void InvokeGetToolTip(object sender, StiValueEventArgs e)
        {
            try
            {
                OnGetToolTip(e);

                GetToolTip?.Invoke(sender, e);

                StiBlocklyHelper.InvokeBlockly(((StiChart)this.Chart).Report, sender, GetToolTipEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetToolTip property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(GetType(), str);
                StiLogService.Write(GetType(), ex.Message);

                if (Chart != null && ((StiChart)Chart).Report != null)
                    ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the ToolTip for the series.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the ToolTip for the series.")]
        public virtual StiGetToolTipEvent GetToolTipEvent { get; set; } = new StiGetToolTipEvent();
        #endregion

        #region GetListOfToolTips
        /// <summary>
        /// Occurs when getting the list of tooltips.
        /// </summary>
        public event StiGetValueEventHandler GetListOfToolTips;

        /// <summary>
        /// Raises the GetListOfToolTips event.
        /// </summary>
        protected virtual void OnGetListOfToolTips(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetListOfToolTips event.
        /// </summary>
        public void InvokeGetListOfToolTips(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfToolTips(e);

                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesListOfToolTips**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(ListOfToolTips, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                this.GetListOfToolTips?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetListOfToolTips property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the list of ToolTips.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the list of ToolTips.")]
        public StiGetListOfToolTipsEvent GetListOfToolTipsEvent { get; set; } = new StiGetListOfToolTipsEvent();
        #endregion

        #region GetTag
        /// <summary>
        /// Occurs when getting the Tag for the series.
        /// </summary>
        public event StiValueEventHandler GetTag;

        /// <summary>
        /// Raises the GetTag event.
        /// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnGetTag(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetTag event.
        /// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public void InvokeGetTag(StiComponent sender, StiValueEventArgs e)
        {
            try
            {
                OnGetTag(e);

                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesTag**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(Tag.Value, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                GetTag?.Invoke(sender, e);

                StiBlocklyHelper.InvokeBlockly(((StiChart)this.Chart).Report, sender, GetTagEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetTag property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(GetType(), str);
                StiLogService.Write(GetType(), ex.Message);

                if (Chart != null && ((StiChart)Chart).Report != null)
                    ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the Tag for the series.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the Tag for the series.")]
        public virtual StiGetTagEvent GetTagEvent { get; set; } = new StiGetTagEvent();
        #endregion

        #region GetListOfTags
        /// <summary>
        /// Occurs when getting the list of Tags.
        /// </summary>
        public event StiGetValueEventHandler GetListOfTags;

        /// <summary>
        /// Raises the GetListOfTags event.
        /// </summary>
        protected virtual void OnGetListOfTags(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetListOfTags event.
        /// </summary>
        public void InvokeGetListOfTags(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfTags(e);
                if (sender.Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    var tempText = new StiText
                    {
                        Name = "**ChartSeriesListOfTag**",
                        Page = sender.Report.Pages[0]
                    };
                    var parserResult = Engine.StiParser.ParseTextValue(ListOfTags, tempText);
                    e.Value = sender.Report.ToString(parserResult);
                }
                this.GetListOfTags?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetListOfTags property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the list of Tags.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the list of Tags.")]
        public StiGetListOfTagsEvent GetListOfTagsEvent { get; set; } = new StiGetListOfTagsEvent();
        #endregion

        #region GetHyperlink
        /// <summary>
        /// Occurs when getting the Hyperlink for the series.
        /// </summary>
        public event StiValueEventHandler GetHyperlink;

        /// <summary>
        /// Raises the GetHyperlink event.
        /// </summary>
        /// <param name="e">A parameter which contains event data.</param>
        protected virtual void OnGetHyperlink(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetHyperlink event.
        /// </summary>
        /// <param name="sender">A sender which invokes an event.</param>
        /// <param name="e">A parameter which contains event data.</param>
        public void InvokeGetHyperlink(object sender, StiValueEventArgs e)
        {
            try
            {
                OnGetHyperlink(e);

                GetHyperlink?.Invoke(sender, e);

                StiBlocklyHelper.InvokeBlockly(((StiChart)this.Chart).Report, sender, GetHyperlinkEvent, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetHyperlink property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(GetType(), str);
                StiLogService.Write(GetType(), ex.Message);

                if (Chart != null && ((StiChart)Chart).Report != null)
                    ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the Hyperlink for the series.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the Hyperlink for the series.")]
        public virtual StiGetHyperlinkEvent GetHyperlinkEvent { get; set; } = new StiGetHyperlinkEvent();
        #endregion

        #region GetListOfHyperlinks
        /// <summary>
        /// Occurs when getting the list of Hyperlinks.
        /// </summary>
        public event StiGetValueEventHandler GetListOfHyperlinks;

        /// <summary>
        /// Raises the Hyperlinks event.
        /// </summary>
        protected virtual void OnGetListOfHyperlinks(StiGetValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetListOfHyperlinks event.
        /// </summary>
        public void InvokeGetListOfHyperlinks(StiComponent sender, StiGetValueEventArgs e)
        {
            try
            {
                OnGetListOfHyperlinks(e);
                this.GetListOfHyperlinks?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                var str = $"Expression in GetListOfHyperlinks property of '{ServiceName}' series from '{((StiChart)Chart).Name}' chart can't be evaluated!";

                StiLogService.Write(this.GetType(), str);
                StiLogService.Write(this.GetType(), ex.Message);

                ((StiChart)Chart).Report.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when getting the list of Hyperlinks.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when getting the list of Hyperlinks.")]
        public StiGetListOfHyperlinksEvent GetListOfHyperlinksEvent { get; set; } = new StiGetListOfHyperlinksEvent();
        #endregion
        #endregion

        #region Expressions
        #region Value
        /// <summary>
        /// Gets or sets point value expression. Example: {Order.Value}
        /// </summary>
        [StiCategory("Value")]
        [StiOrder(StiSeriesPropertyOrder.ValueValue)]
        [StiSerializable]
        [Description("Gets or sets point value expression. Example: {Order.Value}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiExpression Value { get; set; } = new StiExpression();
        #endregion

        #region ListOfValues
        /// <summary>
        /// Gets or sets the expression to fill a list of values.  Example: 1;2;3
        /// </summary>
        [StiCategory("Value")]
        [StiOrder(StiSeriesPropertyOrder.ValueListOfValues)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of values. Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfValuesExpression ListOfValues { get; set; } = new StiListOfValuesExpression();
        #endregion

        #region Argument
        [StiCategory("Argument")]
        [StiSerializable]
        [StiOrder(StiSeriesPropertyOrder.ArgumentArgument)]
        [Description("Gets or sets argument expression. Example: {Order.Argument}")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiArgumentExpression Argument { get; set; } = new StiArgumentExpression();
        #endregion

        #region ListOfArguments
        /// <summary>
        /// Gets or sets the expression to fill a list of arguments.  Example: 1;2;3
        /// </summary>
        [StiCategory("Argument")]
        [StiOrder(StiSeriesPropertyOrder.ArgumentListOfArguments)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of arguments.  Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfArgumentsExpression ListOfArguments { get; set; } = new StiListOfArgumentsExpression();
        #endregion

        #region Title
        /// <summary>
        /// Gets or sets title of series.
        /// </summary>
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        [Browsable(false)]
        public virtual string TitleValue { get; set; }

        /// <summary>
        /// Gets or sets title of series.
        /// </summary>
        [StiCategory("Common")]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets title of series.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiTitleExpression Title { get; set; } = new StiTitleExpression();
        #endregion

        #region ToolTip
        /// <summary>
        /// Gets or sets the expression to fill a series tooltip.
        /// </summary>
        [StiCategory("Navigation")]
        [Browsable(false)]
        [StiOrder(StiPropertyOrder.NavigationToolTip)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a series tooltip.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiToolTipExpression ToolTip { get; set; } = new StiToolTipExpression();
        #endregion

        #region ListOfToolTips
        /// <summary>
        /// Gets or sets the expression to fill a list of tool tips.  Example: 1;2;3
        /// </summary>
        [StiCategory("Value")]
        [Browsable(false)]
        [StiOrder(StiSeriesPropertyOrder.ListOfToolTips)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of tool tips. Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfToolTipsExpression ListOfToolTips { get; set; } = new StiListOfToolTipsExpression();
        #endregion

        #region Tag
        /// <summary>
        /// Gets or sets the expression to fill a series tag.
        /// </summary>
        [StiCategory("Navigation")]
        [Browsable(false)]
        [StiOrder(StiSeriesPropertyOrder.Tag)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a series tag.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiTagExpression Tag { get; set; } = new StiTagExpression();
        #endregion

        #region ListOfTags
        /// <summary>
        /// Gets or sets the expression to fill a list of tags.  Example: 1;2;3
        /// </summary>
        [StiCategory("Value")]
        [Browsable(false)]
        [StiOrder(StiSeriesPropertyOrder.ListOfTags)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of tags. Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfTagsExpression ListOfTags { get; set; } = new StiListOfTagsExpression();
        #endregion

        #region Hyperlink
        /// <summary>
        /// Gets or sets the expression to fill a series hyperlink.
        /// </summary>
        [StiCategory("Navigation")]
        [Browsable(false)]
        [StiOrder(StiSeriesPropertyOrder.Hyperlink)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a series hyperlink.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiHyperlinkExpression Hyperlink { get; set; } = new StiHyperlinkExpression();
        #endregion

        #region ListOfHyperlinks
        /// <summary>
        /// Gets or sets the expression to fill a list of hyperlinks.  Example: 1;2;3
        /// </summary>
        [Browsable(false)]
        [StiCategory("Value")]
        [StiOrder(StiSeriesPropertyOrder.ListOfHyperlinks)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets the expression to fill a list of hyperlinks. Example: 1;2;3")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiListOfHyperlinksExpression ListOfHyperlinks { get; set; } = new StiListOfHyperlinksExpression();
        #endregion
        #endregion

        public StiSeries()
        {
            this.SeriesLabels = new StiCenterAxisLabels();
            this.Interaction = new StiSeriesInteraction();
        }
    }
}