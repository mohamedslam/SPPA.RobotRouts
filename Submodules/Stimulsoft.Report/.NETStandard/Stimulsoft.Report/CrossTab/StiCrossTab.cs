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
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using System.Linq;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.CrossTab
{
    /// <summary>
    /// Describes the class that realizes a StiCrossTab.
    /// </summary>
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiCrossTab.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.catCrossBands.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Design.CrossTab.StiCrossTabDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfCrossTabDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiCrossTabGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiCrossTabWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiCrossTabV1Builder))]
    [StiV2Builder(typeof(StiCrossTabV2Builder))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiContextTool(typeof(IStiDataSource))]
    [StiContextTool(typeof(IStiMasterComponent))]
    [StiContextTool(typeof(IStiDataRelation))]
    [StiContextTool(typeof(IStiSort))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiPrintIfEmpty))]
    [StiEngine(StiEngineVersion.All)]
    public class StiCrossTab :
        StiContainer,
        IStiDataSource,
        IStiFilter,
        IStiCrossTab,
        IStiSort,
        IStiDataRelation,
        IStiPrintIfEmpty,
        IStiBusinessObject
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyBool("CanGrow", CanGrow, true);

            // StiCrossTab
            jObject.AddPropertyBool("PrintIfEmpty", PrintIfEmpty, true);
            jObject.AddPropertyStringNullOrEmpty("DataRelationName", DataRelationName);
            jObject.AddPropertyStringNullOrEmpty("DataSourceName", DataSourceName);
            jObject.AddPropertyStringNullOrEmpty("BusinessObjectGuid", BusinessObjectGuid);
            jObject.AddPropertyStringArray("Sort", Sort);
            jObject.AddPropertyEnum("FilterEngine", FilterEngine, StiFilterEngine.ReportEngine);
            jObject.AddPropertyEnum("FilterMode", FilterMode, StiFilterMode.And);
            jObject.AddPropertyJObject("Filters", Filters.SaveToJsonObject(mode));
            jObject.AddPropertyBool("FilterOn", FilterOn, true);
            jObject.AddPropertyInt("CrossTabStyleIndex", CrossTabStyleIndex);
            jObject.AddPropertyEnum("HorAlignment", HorAlignment, StiCrossHorAlignment.None);
            jObject.AddPropertyEnum("SummaryDirection", SummaryDirection, StiSummaryDirection.UpToDown);
            jObject.AddPropertyBool("KeepCrossTabTogether", KeepCrossTabTogether);
            jObject.AddPropertyStringNullOrEmpty("EmptyValue", EmptyValue);
            jObject.AddPropertyBool("Wrap", Wrap);
            jObject.AddPropertyDouble("WrapGap", WrapGap);
            jObject.AddPropertyBool("RightToLeft", RightToLeft);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PrintIfEmpty":
                        this.PrintIfEmpty = property.DeserializeBool();
                        break;

                    case "DataRelationName":
                        this.DataRelationName = property.DeserializeString();
                        break;

                    case "DataSourceName":
                        this.dataSourceName = property.DeserializeString();
                        break;

                    case "BusinessObjectGuid":
                        this.businessObjectGuid = property.DeserializeString();
                        break;

                    case "Sort":
                        this.Sort = property.DeserializeStringArray();
                        break;

                    case "FilterEngine":
                        this.FilterEngine = property.DeserializeEnum<StiFilterEngine>();
                        break;

                    case "FilterMode":
                        this.FilterMode = property.DeserializeEnum<StiFilterMode>();
                        break;

                    case "Filters":
                        this.Filters.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case "FilterOn":
                        this.FilterOn = property.DeserializeBool();
                        break;

                    case "CrossTabStyleIndex":
                        this.crossTabStyleIndex = property.DeserializeInt();
                        break;

                    case "HorAlignment":
                        this.HorAlignment = property.DeserializeEnum<StiCrossHorAlignment>();
                        break;

                    case "SummaryDirection":
                        this.SummaryDirection = property.DeserializeEnum<StiSummaryDirection>();
                        break;

                    case "KeepCrossTabTogether":
                        this.KeepCrossTabTogether = property.DeserializeBool();
                        break;

                    case "EmptyValue":
                        this.EmptyValue = property.DeserializeString();
                        break;

                    case "Wrap":
                        this.Wrap = property.DeserializeBool();
                        break;

                    case "WrapGap":
                        this.WrapGap = property.DeserializeDouble();
                        break;

                    case "RightToLeft":
                        this.RightToLeft = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCrossTab;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.CrossTabEditor(),
            });

            if (this.Wrap)
            {
                collection.Add(StiPropertyCategories.CrossTab, new[]
                {
                    propHelper.EmptyValue(),
                    propHelper.HorAlignment(),
                    propHelper.PrintIfEmpty(),
                    propHelper.RightToLeft(),
                    propHelper.Wrap(),
                    propHelper.WrapGap()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.CrossTab, new[]
                {
                    propHelper.EmptyValue(),
                    propHelper.HorAlignment(),
                    propHelper.PrintIfEmpty(),
                    propHelper.RightToLeft(),
                    propHelper.Wrap()
                });
            }

            if (level == StiLevel.Professional)
            {
                collection.Add(StiPropertyCategories.Data, new[]
                {
                    propHelper.DataSource(),
                    propHelper.DataRelation(),
                    propHelper.BusinessObject(),
                    propHelper.FilterOn(),
                    propHelper.Filters(),
                    propHelper.FilterEngine(),
                    propHelper.FilterMode(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Data, new[]
                {
                    propHelper.DataSource(),
                });
            }
            
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
                    propHelper.Conditions(),
                    propHelper.ComponentStyle()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.UseParentStyles()
                });
            }

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.CanBreak(),
                    propHelper.KeepCrossTabTogether()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.KeepCrossTabTogether(),
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
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.KeepCrossTabTogether(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }

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
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/index.html?report_internals_crosstable.htm";
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
            this.WrapGap = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.WrapGap));
        }
        #endregion

        #region IStiPrintIfEmpty
        /// <summary>
        /// Gets or sets value indicates that the cross-tab is printed if data is not present.
        /// </summary>
        [DefaultValue(true)]
        [StiCategory("CrossTab")]
        [StiOrder(StiPropertyOrder.CrossTabPrintIfEmpty)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the cross-tab is printed if data is not present.")]
        public virtual bool PrintIfEmpty { get; set; } = true;
        #endregion

        #region IStiShift Off
        [StiNonSerialized]
        [Browsable(false)]
        public override StiShiftMode ShiftMode
        {
            get
            {
                return base.ShiftMode;
            }
            set
            {
                base.ShiftMode = value;
            }
        }
        #endregion

        #region IStiCanShrink Off
        /// <summary>
        /// Gets or sets value which indicates that this object can shrink.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorCanShrink)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that this object can shrink.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiPropertyLevel(StiLevel.Basic)]
        public override bool CanShrink
        {
            get
            {
                return base.CanShrink;
            }
            set
            {
                base.CanShrink = value;
            }
        }
        #endregion

        #region IStiCanGrow Off
        /// <summary>
        /// Gets or sets value indicates that this object can grow.
        /// </summary>
        [DefaultValue(true)]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorCanGrow)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that this object can grow.")]
        public override bool CanGrow
        {
            get
            {
                return base.CanGrow;
            }
            set
            {
                base.CanGrow = value;
            }
        }
        #endregion

        #region IStiBreakable
        public override bool CanBreak
        {
            get
            {
                var placedOnBand = StiSubReportsHelper.GetParentBand(this) != null;
                return placedOnBand || base.CanBreak;
            }
            set
            {
                base.CanBreak = value;
            }
        }
        #endregion

        #region IStiAnchor Off
        [StiNonSerialized]
        [Browsable(false)]
        public override StiAnchorMode Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
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
        public string DataRelationName { get; set; } = "";
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
        public bool IsDataSourceEmpty => string.IsNullOrEmpty(DataSourceName) || DataSource == null;
        #endregion

        #region IStiBusinessObject
        [Browsable(false)]
        public bool IsBusinessObjectEmpty => string.IsNullOrEmpty(BusinessObjectGuid) || BusinessObject == null;

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
        [Browsable(false)]
        public virtual string[] Sort { get; set; } = new string[0];

        private bool ShouldSerializeSort()
        {
            return Sort == null || Sort.Length > 0;
        }
        #endregion

        #region IStiEnumerator
        /// <summary>
        /// Sets the position at the beginning.
        /// </summary>
        public void First()
        {
            if (this.DataSource != null)
                this.DataSource.First();

            else if (this.BusinessObject != null)
                this.BusinessObject.First();
        }

        /// <summary>
        /// Move on the previous row.
        /// </summary>
        public void Prior()
        {
            if (this.DataSource != null)
                this.DataSource.Prior();

            else if (this.BusinessObject != null)
                this.BusinessObject.Prior();
        }

        /// <summary>
        /// Move on the next row.
        /// </summary>
        public void Next()
        {
            if (this.DataSource != null)
                this.DataSource.Next();

            else if (this.BusinessObject != null)
                this.BusinessObject.Next();
        }

        /// <summary>
        /// Move on the last row.
        /// </summary>
        public void Last()
        {
            if (this.DataSource != null)
                this.DataSource.Last();

            else if (this.BusinessObject != null)
                this.BusinessObject.Last();
        }

        /// <summary>
        /// Gets value indicates that the position indicates to the end of data.
        /// </summary>
        [Browsable(false)]
        public bool IsEof
        {
            get
            {
                if (DataSource != null)
                    return DataSource.IsEof;

                if (BusinessObject != null)
                    return BusinessObject.IsEof;

                return true;
            }
            set
            {
                if (DataSource != null)
                    DataSource.IsEof = value;

                else if (BusinessObject != null)
                    BusinessObject.IsEof = value;
            }
        }

        /// <summary>
        /// Gets value, indicates that the position indicates to the beginning of data.
        /// </summary>
        [Browsable(false)]
        public bool IsBof
        {
            get
            {
                if (DataSource != null)
                    return DataSource.IsBof;

                if (BusinessObject != null)
                    return BusinessObject.IsBof;

                return true;
            }
            set
            {
                if (DataSource != null)
                    DataSource.IsBof = value;

                else if (BusinessObject != null)
                    BusinessObject.IsBof = value;
            }
        }

        /// <summary>
        /// Gets value indicates that no data.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                if (DataSource != null)
                    return DataSource.IsEmpty;

                if (BusinessObject != null)
                    return BusinessObject.IsEmpty;

                return true;
            }
        }

        /// <summary>
        /// Gets the current position.
        /// </summary>
        [Browsable(false)]
        public virtual int Position
        {
            get
            {
                if (DataSource != null)
                    return DataSource.Position;

                if (BusinessObject != null)
                    return BusinessObject.Position;

                return 0;
            }
            set
            {
                if (DataSource != null)
                    DataSource.Position = value;

                else if (BusinessObject != null)
                    BusinessObject.Position = value;
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
                if (DataSource != null)
                    return DataSource.Count;

                if (BusinessObject != null)
                    return BusinessObject.Count;

                return 0;
            }
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var cross = (StiCrossTab)base.Clone();

            cross.CrossTabInfo = this.CrossTabInfo.Clone() as StiCrossTabInfo;
            cross.CrossTabInfoV1 = this.CrossTabInfoV1.Clone() as StiCrossTabInfoV1;

            cross.Filters = this.Filters != null
                ? this.Filters.Clone() as StiFiltersCollection
                : null;

            return cross;
        }
        #endregion

        #region StiService override
        public override void PackService()
        {
            base.PackService();
            Filters = null;
        }
        #endregion

        #region Render override
        [Browsable(false)]
        public StiCrossTabInfo CrossTabInfo { get; private set; } = new StiCrossTabInfo();

        [Browsable(false)]
        public StiCrossTabInfoV1 CrossTabInfoV1 { get; private set; } = new StiCrossTabInfoV1();
        #endregion

        #region IStiFilter
        /// <summary>
        /// Gets or sets how a filter will be applied to data - be means of the report generator or by means of changing the SQL query.
        /// </summary>
        [DefaultValue(StiFilterEngine.ReportEngine)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataFilterEngine)]
        [Description("Gets or sets how a filter will be applied to data - be means of the report generator or by means of changing the SQL query.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public StiFilterEngine FilterEngine { get; set; } = StiFilterEngine.ReportEngine;

        /// <summary>
        /// Gets or sets filter mode.
        /// </summary>
        [DefaultValue(StiFilterMode.And)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataFilterMode)]
        [Description("Gets or sets filter mode.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public StiFilterMode FilterMode { get; set; } = StiFilterMode.And;

        /// <summary>
        /// Gets or sets a method for filtration.
        /// </summary>
        [Browsable(false)]
        public StiFilterEventHandler FilterMethodHandler { get; set; }

        /// <summary>
        /// Gets or sets the collection of data filters.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [TypeConverter(typeof(StiFiltersCollectionConverter))]
        [StiCategory("Data")]
        [Editor("Stimulsoft.Report.Components.Design.StiFiltersCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets the collection of data filters.")]
        [StiPropertyLevel(StiLevel.Professional)]
        [StiOrder(StiPropertyOrder.DataFilters)]
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

        #region StiComponent override
        /// <summary>
        /// May this container be located in the specified component.
        /// </summary>
        /// <param name="component">Component for checking.</param>
        /// <returns>true, if this container may is located in the specified component.</returns>
        public override bool CanContainIn(StiComponent component)
        {
            if (component is StiCrossTab) return false;
            if (component is StiClone) return false;

            return component is StiContainer;
        }

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiCrossTab");

        public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.CrossTab;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Cross;

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.CrossTab;

        /// <summary>
        /// Gets or sets the default client area of a component.
        /// </summary>
        [Browsable(false)]
        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 300, 100);

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                if (Report != null && Report.EngineVersion == StiEngineVersion.EngineV2)
                    return StiComponentType.Simple;

                if (Parent is StiBand)
                    return StiComponentType.Simple;

                return StiComponentType.Master;
            }
        }
        #endregion

        #region CrossTab Style
        private int crossTabStyleIndex = 0;
        [DefaultValue(0)]
        [Browsable(false)]
        [StiSerializable]
        public int CrossTabStyleIndex
        {
            get
            {
                return crossTabStyleIndex;
            }
            set
            {
                if (value == crossTabStyleIndex) return;

                if (value != -1)
                {
                    base.ComponentStyle = "";
                    this.CrossTabStyleColor = null;
                }

                crossTabStyleIndex = value >= StiOptions.Designer.CrossTab.StyleColors.Length 
                    ? StiOptions.Designer.CrossTab.StyleColors.Length - 1 
                    : value;

                if (IsDesigning && Report != null && !Report.IsSerializing)
                {
                    foreach (StiCrossField field in Components)//reset to defaults
                    {
                        field.ComponentStyle = "CrossTab";
                        field.Font = new Font("Arial", 8);
                    }
                    UpdateStyles();
                }
                    
            }
        }

        [DefaultValue(null)]
        [Browsable(false)]
        public object CrossTabStyleColor { get; set; }

        [DefaultValue("")]
        [Browsable(false)]
        public string CrossTabStyle
        {
            get
            {
                return this.ComponentStyle;
            }
            set
            {
                this.ComponentStyle = value;
            }
        }

        [Editor("Stimulsoft.Report.Design.StiCrossTabStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public override string ComponentStyle
        {
            get
            {
                return base.ComponentStyle;
            }
            set
            {
                if (value == null)
                    value = "";

                if (base.ComponentStyle == value) return;

                base.ComponentStyle = value;

                this.CrossTabStyleColor = null;
                this.crossTabStyleIndex = -1;

                UpdateStyles();

                foreach (StiCrossField field in Components)
                {
                    field.ComponentStyle = ComponentStyle;
                }
            }
        }

        public void UpdateStyles(bool rewriteDefaultStyle = true)
        {
            foreach (StiCrossField field in Components)
            {
                ApplyFieldStyle(field, rewriteDefaultStyle);
            }
        }

        public Color GetCellColor()
        {
            if (CrossTabStyleColor is Color)
                return (Color)CrossTabStyleColor;

            if (!string.IsNullOrEmpty(CrossTabStyle) && Report != null)
            {
                var style = Report.Styles[this.CrossTabStyle] as StiCrossTabStyle;
                if (style != null)
                    return style.Color;
            }

            return CrossTabStyleIndex != -1 
                ? StiOptions.Designer.CrossTab.StyleColors[CrossTabStyleIndex] 
                : Color.White;
        }

        public void ApplyFieldStyle(StiCrossField field, bool rewriteDefaultStyle = true)
        {
            if (field == null) return;

            if (!string.IsNullOrEmpty(CrossTabStyle) && Report != null && Report.Styles[this.CrossTabStyle] as StiCrossTabStyle != null)
            {
                if (field.ComponentStyle != "")
                {
                    var style = Report.Styles[this.CrossTabStyle] as StiCrossTabStyle;

                    if (field is StiCrossColumn || field is StiCrossTitle || field is StiCrossColumnTotal || (field is StiCrossSummaryHeader && SummaryDirection == StiSummaryDirection.LeftToRight))
                    {
                        field.TextBrush = new StiSolidBrush(style.ColumnHeaderForeColor);
                        field.Brush = new StiSolidBrush(style.ColumnHeaderBackColor);
                    }
                    else if (field is StiCrossRow || field is StiCrossRowTotal || (field is StiCrossSummaryHeader && SummaryDirection == StiSummaryDirection.UpToDown))
                    {
                        field.TextBrush = new StiSolidBrush(style.RowHeaderForeColor);
                        field.Brush = new StiSolidBrush(style.RowHeaderBackColor);
                    }
                    else //if(field is StiCrossSummary)
                    {
                        field.TextBrush = new StiSolidBrush(style.CellForeColor);
                        field.Brush = new StiSolidBrush(style.CellBackColor);
                    }
                }                
            }
            else if (field.ComponentStyle != "CrossTab" && !string.IsNullOrEmpty(field.ComponentStyle) && Report != null && Report.Styles[field.ComponentStyle] != null)
            {
                Report.Styles[field.ComponentStyle].SetStyleToComponent(field);
            }
            else if (rewriteDefaultStyle)
            {
                field.Brush = new StiSolidBrush(Color.White);
                field.Border.Color = StiColorUtils.Dark(GetCellColor(), 100);
                field.TextBrush = new StiSolidBrush(Color.Black);

                if (field is StiCrossTitle ||
                    field is StiCrossRow ||
                    field is StiCrossColumn ||
                    field is StiCrossSummaryHeader)
                {
                    field.Brush = new StiSolidBrush(GetCellColor());
                    field.TextBrush = new StiSolidBrush(StiColorUtils.Dark(GetCellColor(), 150));
                }

                if (field is StiCrossTotal)
                    field.Brush = new StiSolidBrush(StiColorUtils.Light(GetCellColor(), 50));

                if (field is StiCrossSummary)
                    field.Brush = new StiSolidBrush(StiColorUtils.Light(GetCellColor(), 100));
            }   
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the horizontal alignment of an Cross-Tab.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiCrossHorAlignment.None)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets the horizontal alignment of an Cross-Tab.")]
        [StiCategory("CrossTab")]
        [StiOrder(StiPropertyOrder.CrossTabHorAlignment)]
        public StiCrossHorAlignment HorAlignment { get; set; } = StiCrossHorAlignment.None;

        [DefaultValue(true)]
        [StiCategory("CrossTab")]
        [StiShowInContextMenu]
        [StiOrder(StiPropertyOrder.CrossTabPrintTitleOnAllPages)]
        [StiSerializable]
        [StiEngine(StiEngineVersion.EngineV2)]
        [Browsable(false)]
        public bool PrintTitleOnAllPages { get; set; } = true;

        [DefaultValue(StiSummaryDirection.UpToDown)]
        [StiSerializable]
        [Browsable(false)]
        public StiSummaryDirection SummaryDirection { get; set; } = StiSummaryDirection.UpToDown;

        /// <summary>
        /// Gets or sets value which indicates that the CrossTab must to be kept together with DataBand on what it is placed.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorKeepCrossTabTogether)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that the CrossTab must to be kept together with DataBand on what it is placed.")]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiShowInContextMenu]
        public virtual bool KeepCrossTabTogether { get; set; }
        
        /// <summary>
        /// Gets or sets string value which is used to show the cross-tab in empty cells.
        /// </summary>
        [StiSerializable]
        [StiCategory("CrossTab")]
        [StiOrder(StiPropertyOrder.CrossTabEmptyValue)]
        [Description("Gets or sets string value which is used to show the cross-tab in empty cells.")]
        public string EmptyValue { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets value which indicates that the cross-tab is to be output in one column. 
        /// At the same time, everything that do not fit by the width is output below. For using 
        /// this property it is necessary to enable the CanBreak property of the band on which 
        /// the cross-tab is placed (if it is placed on the band). 
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("CrossTab")]
        [StiOrder(StiPropertyOrder.CrossTabWrap)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that the cross-tab is to be output in one column. At the same time, everything that do not fit by the width is output below. For using this property it is necessary to enable the CanBreak property of the band on which the cross-tab is placed (if it is placed on the band).")]
        public bool Wrap { get; set; }

        /// <summary>
        /// Gets or sets space between two parts of a wrapped cross-tab. The property is used jointly with the property Wrap. 
        /// </summary>
        [DefaultValue(0d)]
        [StiSerializable]
        [StiCategory("CrossTab")]
        [StiOrder(StiPropertyOrder.CrossTabWrapGap)]
        [Description("Gets or sets space between two parts of a wrapped cross-tab. The property is used jointly with the property Wrap.")]
        public double WrapGap { get; set; }

        private bool rightToLeft;
        /// <summary>
        /// Gets or sets horizontal CrossTab direction.
        /// </summary>
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets horizontal CrossTab direction.")]
        [StiOrder(StiPropertyOrder.CrossTabRightToLeft)]
        [StiSerializable]
        [StiCategory("CrossTab")]
        public virtual bool RightToLeft
        {
            get
            {
                return rightToLeft;
            }
            set
            {
                if (rightToLeft == value) return;

                CheckBlockedException("RightToLeft");
                rightToLeft = value;
            }
        }
        #endregion

        #region Properties.Override
        [StiNonSerialized]
        [Browsable(false)]
        public override bool Printable
        {
            get
            {
                return base.Printable;
            }
            set
            {
                base.Printable = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool GrowToHeight
        {
            get
            {
                return base.GrowToHeight;
            }
            set
            {
                base.GrowToHeight = value;
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossTab();
        }
        #endregion

        #region this
        /// <summary>
        /// Creates an object of the type StiCrossTab.
        /// </summary>
        public StiCrossTab() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        ///  Creates an object of the type StiCrossTab.
        /// </summary>
        /// <param name="rect">The rectangle decribes size and position of the component.</param>
        public StiCrossTab(RectangleD rect) : base(rect)
        {
            this.PlaceOnToolbox = false;
            this.CanGrow = true;
        }
        #endregion
    }
}