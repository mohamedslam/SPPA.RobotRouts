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
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Report.Units;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the class that realizes a Data Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiDataBand), "Stimulsoft.Report.Images.Components.StiDataBand.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiDataBandDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfDataBandDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiDataBandV1Builder))]
    [StiV2Builder(typeof(StiDataBandV2Builder))]
    [StiContextTool(typeof(IStiDataSource))]
    [StiContextTool(typeof(IStiMasterComponent))]
    [StiContextTool(typeof(IStiDataRelation))]
    [StiContextTool(typeof(IStiSort))]
    [StiContextTool(typeof(IStiKeepGroupTogether))]
    [StiContextTool(typeof(IStiKeepChildTogether))]
    [StiContextTool(typeof(IStiKeepHeaderTogether))]
    [StiContextTool(typeof(IStiKeepFooterTogether))]
    [StiContextTool(typeof(IStiPrintOnAllPages))]
    [StiContextTool(typeof(IStiPrintIfDetailEmpty))]
    [StiContextTool(typeof(IStiStartNewPage))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiBreakable))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiDataSourceQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 0)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiDataRelationQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 1)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiMasterComponentQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 2)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiBandQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 3)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 4)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfDataSourceQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 0)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfDataRelationQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 1)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfMasterComponentQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 2)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfBandQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 3)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 4)]
    public class StiDataBand :
        StiDynamicBand,
        IStiDataSource,
        IStiMasterComponent,
        IStiDataRelation,
        IStiOddEvenStyles,
        IStiSort,
        IStiFilter,
        IStiStartNewPage,
        IStiPrintOnAllPages,
        IStiPrintIfDetailEmpty,
        IStiKeepDetailsTogether,
        IStiKeepFooterTogether,
        IStiKeepGroupTogether,
        IStiKeepHeaderTogether,
        IStiSelectedLine,
        IStiRenderMaster,
        IStiBusinessObject
    {
        #region IStiJsonReportObject.override
        // Fields
        internal string jsonMasterComponentTemp;

        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDataBand
            jObject.AddPropertyBool("StartNewPage", StartNewPage);
            jObject.AddPropertyFloat("StartNewPageIfLessThan", StartNewPageIfLessThan, 100f);
            jObject.AddPropertyBool("KeepGroupTogether", KeepGroupTogether);
            jObject.AddPropertyBool("KeepHeaderTogether", KeepHeaderTogether, true);
            jObject.AddPropertyBool("KeepFooterTogether", KeepFooterTogether, true);
            jObject.AddPropertyBool("KeepChildTogether", KeepChildTogether);
            jObject.AddPropertyEnum("KeepDetails", KeepDetails, StiKeepDetails.None);
            jObject.AddPropertyBool("PrintOnAllPages", PrintOnAllPages);
            jObject.AddPropertyBool("PrintIfDetailEmpty", PrintIfDetailEmpty);
            jObject.AddPropertyStringNullOrEmpty("DataSourceName", DataSourceName);
            jObject.AddPropertyStringNullOrEmpty("BusinessObjectGuid", BusinessObjectGuid);
            jObject.AddPropertyStringNullOrEmpty("DataRelationName", DataRelationName);
            jObject.AddPropertyEnum("FilterMode", FilterMode, StiFilterMode.And);
            jObject.AddPropertyEnum("FilterEngine", FilterEngine, StiFilterEngine.ReportEngine);
            jObject.AddPropertyBool("FilterOn", FilterOn, true);
            jObject.AddPropertyStringNullOrEmpty("EvenStyle", EvenStyle);
            jObject.AddPropertyStringNullOrEmpty("OddStyle", OddStyle);
            jObject.AddPropertyJObject("BeginRenderEvent", BeginRenderEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("RenderingEvent", RenderingEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("EndRenderEvent", EndRenderEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetCollapsedEvent", GetCollapsedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyBool("RightToLeft", RightToLeft);
            jObject.AddPropertyDouble("ColumnGaps", ColumnGaps, 0d);
            jObject.AddPropertyDouble("ColumnWidth", ColumnWidth, 0d);
            jObject.AddPropertyInt("Columns", Columns);
            jObject.AddPropertyInt("MinRowsInColumn", MinRowsInColumn);
            jObject.AddPropertyEnum("ColumnDirection", ColumnDirection, StiColumnDirection.AcrossThenDown);
            jObject.AddPropertyBool("ResetDataSource", ResetDataSource);
            jObject.AddPropertyBool("CalcInvisible", CalcInvisible);
            jObject.AddPropertyInt("CountData", CountData);
            jObject.AddPropertyBool("KeepDetailsTogether", KeepDetailsTogether);
            jObject.AddPropertyStringNullOrEmpty("LimitRows", LimitRows);
            jObject.AddPropertyBool("MultipleInitialization", MultipleInitialization);

            if (MasterComponent != null)
                jObject.AddPropertyStringNullOrEmpty("MasterComponent", MasterComponent.Name);

            if (mode == StiJsonSaveMode.Report)
            {
                jObject.AddPropertyStringArray("Sort", Sort);
                jObject.AddPropertyJObject("Filters", Filters.SaveToJsonObject(mode));
                jObject.AddPropertyJObject("Collapsed", Collapsed.SaveToJsonObject(mode));
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
                    case "StartNewPage":
                        this.StartNewPage = property.DeserializeBool();
                        break;

                    case "StartNewPageIfLessThan":
                        this.StartNewPageIfLessThan = property.DeserializeFloat();
                        break;

                    case "KeepGroupTogether":
                        this.KeepGroupTogether = property.DeserializeBool();
                        break;

                    case "KeepHeaderTogether":
                        this.KeepHeaderTogether = property.DeserializeBool();
                        break;

                    case "KeepFooterTogether":
                        this.KeepFooterTogether = property.DeserializeBool();
                        break;

                    case "KeepChildTogether":
                        this.KeepChildTogether = property.DeserializeBool();
                        break;

                    case "MultipleInitialization":
                        this.MultipleInitialization = property.DeserializeBool();
                        break;

                    case "KeepDetails":
                        this.keepDetails = property.DeserializeEnum<StiKeepDetails>();
                        break;

                    case "Sort":
                        this.Sort = property.DeserializeStringArray();
                        break;

                    case "PrintOnAllPages":
                        this.PrintOnAllPages = property.DeserializeBool();
                        break;

                    case "PrintIfDetailEmpty":
                        this.PrintIfDetailEmpty = property.DeserializeBool();
                        break;

                    case "DataSourceName":
                        this.dataSourceName = property.DeserializeString();
                        break;

                    case "BusinessObjectGuid":
                        this.businessObjectGuid = property.DeserializeString();
                        break;

                    case "DataRelationName":
                        this.DataRelationName = property.DeserializeString();
                        break;

                    case "FilterMode":
                        this.FilterMode = property.DeserializeEnum<StiFilterMode>();
                        break;

                    case "FilterEngine":
                        this.FilterEngine = property.DeserializeEnum<StiFilterEngine>();
                        break;

                    case "Filters":
                        {
                            if (Filters == null)
                                Filters = new StiFiltersCollection();

                            this.Filters.LoadFromJsonObject((JObject)property.Value);
                        }
                        break;

                    case "FilterOn":
                        this.FilterOn = property.DeserializeBool();
                        break;

                    case "EvenStyle":
                        this.EvenStyle = property.DeserializeString();
                        break;

                    case "OddStyle":
                        this.OddStyle = property.DeserializeString();
                        break;

                    case "BeginRenderEvent":
                        {
                            var _event = new StiBeginRenderEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.BeginRenderEvent = _event;
                        }
                        break;

                    case "RenderingEvent":
                        {
                            var _event = new StiRenderingEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.RenderingEvent = _event;
                        }
                        break;

                    case "EndRenderEvent":
                        {
                            var _event = new StiEndRenderEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.EndRenderEvent = _event;
                        }
                        break;

                    case "GetCollapsedEvent":
                        {
                            var _event = new StiGetCollapsedEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetCollapsedEvent = _event;
                        }
                        break;

                    case "Collapsed":
                        {
                            var _expression = new StiCollapsedExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.Collapsed = _expression;
                        }
                        break;

                    case "RightToLeft":
                        this.rightToLeft = property.DeserializeBool();
                        break;

                    case "ColumnGaps":
                        this.columnGaps = property.DeserializeDouble();
                        break;

                    case "ColumnWidth":
                        this.columnWidth = property.DeserializeDouble();
                        break;

                    case "Columns":
                        this.columns = property.DeserializeInt();
                        break;

                    case "MinRowsInColumn":
                        this.minRowsInColumn = property.DeserializeInt();
                        break;

                    case "ColumnDirection":
                        this.columnDirection = property.DeserializeEnum<StiColumnDirection>();
                        break;

                    case "ResetDataSource":
                        this.ResetDataSource = property.DeserializeBool();
                        break;

                    case "CalcInvisible":
                        this.CalcInvisible = property.DeserializeBool();
                        break;

                    case "CountData":
                        this.countData = property.DeserializeInt();
                        break;

                    case "MasterComponent":
                        {
                            this.jsonMasterComponentTemp = property.DeserializeString();
                            this.Report.jsonLoaderHelper.MasterComponents.Add(this);
                        }
                        break;

                    case "LimitRows":
                        this.LimitRows = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiDataBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.DataBandEditor()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.PageColumnBreak, new[]
                {
                    propHelper.NewPageBefore(),
                    propHelper.NewPageAfter(),
                    propHelper.NewColumnBefore(),
                    propHelper.NewColumnAfter()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.PageColumnBreak, new[]
                {
                    propHelper.NewPageBefore(),
                    propHelper.NewPageAfter(),
                    propHelper.NewColumnBefore(),
                    propHelper.NewColumnAfter(),
                    propHelper.SkipFirst()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.PageColumnBreak, new[]
                {
                    propHelper.NewPageBefore(),
                    propHelper.NewPageAfter(),
                    propHelper.NewColumnBefore(),
                    propHelper.NewColumnAfter(),
                    propHelper.BreakIfLessThan(),
                    propHelper.SkipFirst(),
                    propHelper.LimitRows()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Columns, new[]
                {
                    propHelper.Columns(),
                    propHelper.ColumnWidth(),
                    propHelper.ColumnGaps(),
                    propHelper.ColumnDirection(),
                    propHelper.RightToLeft()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Columns, new[]
                {
                    propHelper.Columns(),
                    propHelper.ColumnWidth(),
                    propHelper.ColumnGaps(),
                    propHelper.ColumnDirection(),
                    propHelper.MinRowsInColumn(),
                    propHelper.RightToLeft()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Height()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Height(),
                    propHelper.MaxHeight(),
                    propHelper.MinHeight()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.OddStyle(),
                    propHelper.EvenStyle()
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
                    propHelper.OddStyle(),
                    propHelper.EvenStyle(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.CalcInvisible(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.KeepDetailsTogether(),
                    propHelper.PrintAtBottom(),
                    propHelper.PrintIfDetailEmpty(),
                    propHelper.PrintOn(),
                    propHelper.PrintOnAllPages(),
                    propHelper.ResetPageNumber()
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
                    StiPropertyCategories.RenderEvents,
                    new[]
                    {
                        StiPropertyEventId.BeginRenderEvent,
                        StiPropertyEventId.EndRenderEvent,
                        StiPropertyEventId.RenderingEvent,
                    }
                },
                {
                    StiPropertyCategories.ValueEvents,
                    new[]
                    {
                        StiPropertyEventId.GetCollapsedEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "User-Manual/report_internals_creating_lists_data_band.htm";
        #endregion

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public override void SaveState(string stateName)
        {
            if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV2)
            {
                States.PushInt(stateName, this, "positionValue", positionValue);
                States.PushBool(stateName, this, "isEofValue", isEofValue);
                States.PushBool(stateName, this, "isBofValue", isBofValue);

                if (DataSource != null)
                    this.DataSource.SaveState(stateName);

                if (!string.IsNullOrEmpty(BusinessObjectGuid))
                    this.BusinessObject.SaveState(stateName);
            }
            else
            {
                base.SaveState(stateName);

                States.PushInt(stateName, this, "startLine", this.DataBandInfoV1.StartLine);
                States.PushInt(stateName, this, "positionValue", positionValue);
                States.PushBool(stateName, this, "isEofValue", isEofValue);
                States.PushBool(stateName, this, "isBofValue", isBofValue);
                States.PushInt(stateName, this, "runtimeLine", this.DataBandInfoV1.RuntimeLine);
                States.PushDouble(stateName, this, "freeSpace", this.DataBandInfoV1.FreeSpace);
                States.PushInt(stateName, this, "lineThrough", LineThrough);
                States.PushBool(stateName, this, "firstCall", this.DataBandInfoV1.FirstCall);
                States.PushBool(stateName, this, "firstGroupOnPass", this.DataBandInfoV1.FirstGroupOnPass);
                States.Push(stateName, this, "CurrentDetailComponent", this.DataBandInfoV1.CurrentDetailComponent);
                States.PushInt(stateName, this, "lastPositionRendering", this.DataBandInfoV1.LastPositionRendering);

                if (DataSource != null)
                    this.DataSource.SaveState(stateName);

                if (!string.IsNullOrEmpty(BusinessObjectGuid))
                    this.BusinessObject.SaveState(stateName);
            }
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public override void RestoreState(string stateName)
        {
            if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV2)
            {
                isBofValue = States.PopBool(stateName, this, "isBofValue");
                isEofValue = States.PopBool(stateName, this, "isEofValue");
                positionValue = States.PopInt(stateName, this, "positionValue");

                if (DataSource != null)
                    this.DataSource.RestoreState(stateName);

                if (!string.IsNullOrEmpty(BusinessObjectGuid))
                    this.BusinessObject.RestoreState(stateName);

                StiDataBandV2Builder.PrepareGroupResults(this);
            }
            else
            {
                base.RestoreState(stateName);
                if (States.IsExist(stateName, this))
                {

                    DataBandInfoV1.FirstCall = States.PopBool(stateName, this, "firstCall");
                    LineThrough = States.PopInt(stateName, this, "lineThrough");
                    DataBandInfoV1.RuntimeLine = States.PopInt(stateName, this, "runtimeLine");
                    isBofValue = States.PopBool(stateName, this, "isBofValue");
                    isEofValue = States.PopBool(stateName, this, "isEofValue");
                    positionValue = States.PopInt(stateName, this, "positionValue");
                    DataBandInfoV1.StartLine = States.PopInt(stateName, this, "startLine");
                    DataBandInfoV1.FreeSpace = States.PopDouble(stateName, this, "freeSpace");
                    DataBandInfoV1.FirstGroupOnPass = States.PopBool(stateName, this, "firstGroupOnPass");
                    DataBandInfoV1.LastPositionRendering = States.PopInt(stateName, this, "lastPositionRendering");
                }

                if (DataSource != null)
                    this.DataSource.RestoreState(stateName);

                if (!string.IsNullOrEmpty(BusinessObjectGuid))
                    this.BusinessObject.RestoreState(stateName);

                if (Page.PageInfoV1.RestoreCurrentDetailComponent && States.IsExist(stateName, this))
                    this.DataBandInfoV1.CurrentDetailComponent = States.Pop(stateName, this, "currentDetailComponent") as StiComponent;
            }
        }
        #endregion

        #region IStiResetPageNumber
        /// <summary>
        /// Allows to reset page number on this component.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorResetPageNumber)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Allows to reset page number on this component.")]
        [StiEngine(StiEngineVersion.All)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public override bool ResetPageNumber
        {
            get
            {
                return base.ResetPageNumber;
            }
            set
            {
                base.ResetPageNumber = value;
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

        #region IStiUnitConvert
        /// <summary>
        /// Converts a component out of one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
            base.Convert(oldUnit, newUnit, isReportSnapshot);

            this.ColumnGaps = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.ColumnGaps));
            this.ColumnWidth = newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.ColumnWidth));
        }
        #endregion

        #region IStiStartNewPage EngineV1 Only
        /// <summary>
        /// Gets or sets value indicates that it is necessary to print every new string on a new page.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorStartNewPage)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that it is necessary to print every new string on a new page.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool StartNewPage { get; set; }

        /// <summary>
		/// Gets or sets value which indicates how much free space on a page (in percentage terms) should be reserved for formation of a new page.
		/// The value should be set in the range from 0 to 100. If the value is 100 then, in 
		/// any case, a new page will be formed. This property is used together with the StartNewPage property.
		/// </summary>
		[DefaultValue(100f)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorStartNewPageIfLessThan)]
        [Description("Gets or sets value which indicates how much free space on a page (in percentage terms) " +
             "should be reserved for formation of a new page. The value should be set in the range from 0 to 100. " +
             "If the value is 100 then, in any case, a new page will be formed. This property is used together with the StartNewPage property.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual float StartNewPageIfLessThan
        {
            get
            {
                return BreakIfLessThan;
            }
            set
            {
                BreakIfLessThan = value;
            }
        }
        #endregion

        #region IStiKeepGroupTogether EngineV1 Only
        /// <summary>
        /// Gets or sets value indicates that group is to be kept together.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorKeepGroupTogether)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that group is to be kept together.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepGroupTogether { get; set; }
        #endregion

        #region IStiKeepHeaderTogether EngineV1 Only
        /// <summary>
        /// Gets or sets value indicates that header is printed with data together.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorKeepHeaderTogether)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that header is printed with data together.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepHeaderTogether { get; set; } = true;
        #endregion

        #region IStiKeepFooterTogether EngineV1 Only
        /// <summary>
        /// Gets or sets value indicates that the footer is printed with data.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorKeepFooterTogether)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the footer is printed with data.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepFooterTogether { get; set; } = true;
        #endregion

        #region IStiKeepChildTogether EngineV1 Only
        /// <summary>
        /// Gets or sets value indicates that childs are to be kept together.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorKeepChildTogether)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that childs are to be kept together.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepChildTogether
        {
            get
            {
                return this.KeepDetailsTogether;
            }
            set
            {
                this.KeepDetailsTogether = value;
            }
        }
        #endregion

        #region IStiKeepDetailsTogether EngineV2 Only
        /// <summary>
        /// Gets or sets value indicates that details are to be kept together with this DataBand.
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets value indicates that details are to be kept together with this DataBand.")]
        [Browsable(false)]
        public virtual bool KeepDetailsTogether
        {
            get
            {
                return keepDetails == StiKeepDetails.KeepDetailsTogether;
            }
            set
            {
                var tempValue = value ? StiKeepDetails.KeepDetailsTogether : StiKeepDetails.None;
                if (keepDetails != tempValue)
                {
                    CheckBlockedException("KeepDetailsTogether");
                    keepDetails = tempValue;
                }
            }
        }
        #endregion

        #region IStiKeepDetails EngineV2 Only
        private StiKeepDetails keepDetails = StiKeepDetails.None;
        /// <summary>
        /// Gets or sets value indicates how details are to be kept together with this DataBand.
        /// </summary>
        [DefaultValue(StiKeepDetails.None)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorKeepDetailsTogether)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates how details are to be kept together with this DataBand.")]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiKeepDetails KeepDetails
        {
            get
            {
                return keepDetails;
            }
            set
            {
                if (keepDetails != value)
                {
                    CheckBlockedException("KeepDetails");
                    keepDetails = value;
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
        public virtual string[] Sort { get; set; } = new string[0];

        private bool ShouldSerializeSort()
        {
            return Sort == null || Sort.Length > 0;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties, bool cloneComponents)
        {
            var band = (StiDataBand)base.Clone(cloneProperties, cloneComponents);

            band.dataBandInfoV1 = this.DataBandInfoV1.Clone() as StiDataBandInfoV1;
            band.dataBandInfoV2 = this.DataBandInfoV2.Clone() as StiDataBandInfoV2;

            band.MasterComponent = null;

            if (this.Sort != null)
                band.Sort = (string[])this.Sort.Clone();
            else
                band.Sort = null;

            if (this.Filters != null)
                band.Filters = this.Filters.Clone() as StiFiltersCollection;
            else
                band.Filters = null;

            return band;
        }
        #endregion

        #region IStiPrintOnAllPages
        /// <summary>
        /// Gets or sets value indicates that the component is printed on all pages.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintOnAllPages)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the component is printed on all pages.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintOnAllPages { get; set; }
        #endregion

        #region IStiPrintIfDetailEmpty
        /// <summary>
        /// Gets or sets value indicates that if detail is empty 
        /// then the master data must be printed anyway.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintIfDetailEmpty)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that if detail is empty " +
             "then the master data must be printed anyway.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintIfDetailEmpty { get; set; }
        #endregion

        #region IStiDataSource
        [Browsable(false)]
        public bool IsDataSourceEmpty => string.IsNullOrEmpty(DataSourceName) || DataSource == null;

        /// <summary>
		/// Get data source that is used for getting data.
		/// </summary>
		[TypeConverter(typeof(StiDataSourceConverter))]
        [RefreshProperties(RefreshProperties.All)]
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
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue("")]
        public string DataSourceName
        {
            get
            {
                return dataSourceName;
            }
            set
            {
                if (dataSourceName == value) return;

                dataSourceName = value;

                if (!string.IsNullOrEmpty(value))
                    this.BusinessObjectGuid = null;

                if (!string.IsNullOrEmpty(value))
                    CountData = 0;

                StiOptions.Engine.GlobalEvents.InvokeDataSourceAssigned(this, EventArgs.Empty);
            }
        }
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
                if (businessObjectGuid == value) return;

                businessObjectGuid = value;

                if (!string.IsNullOrEmpty(value))
                    this.DataSourceName = null;

                StiOptions.Engine.GlobalEvents.InvokeBusinessObjectAssigned(this, EventArgs.Empty);
            }
        }
        #endregion

        #region IStiSelectedLine
        /// <summary>
        /// Gets or sets the selected line in rows which is output by this data band.
        /// </summary>
        [Browsable(false)]
        public int SelectedLine { get; set; } = -1;
        #endregion

        #region IStiEnumerator
        /// <summary>
		/// Sets the position at the beginning.
		/// </summary>
		public virtual void First()
        {
            if (!this.IsBusinessObjectEmpty)
                this.BusinessObject.First();

            else if (!this.IsDataSourceEmpty)
                this.DataSource.First();

            else
            {
                this.isEofValue = false;
                this.isBofValue = true;
                this.positionValue = 0;
            }

            if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1)
                StiDataBandV1Builder.SetDetails(this);

            else
                StiDataBandV2Builder.SetDetails(this);
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

            if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1)
                StiDataBandV1Builder.SetDetails(this);

            else
                StiDataBandV2Builder.SetDetails(this);
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

                if (this.positionValue >= this.countData - 1)
                    this.isEofValue = true;
                else
                    this.positionValue++;
            }

            if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1)
                StiDataBandV1Builder.SetDetails(this);
            else
                StiDataBandV2Builder.SetDetails(this);
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
                this.positionValue = this.countData - 1;
            }

            if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1)
                StiDataBandV1Builder.SetDetails(this);
            else
                StiDataBandV2Builder.SetDetails(this);
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
                if (!this.IsBusinessObjectEmpty)
                    return BusinessObject.Count;

                if (!this.IsDataSourceEmpty)
                    return DataSource.Count;

                return countData;
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
        [DefaultValue("")]
        public string DataRelationName { get; set; } = "";
        #endregion

        #region IStiFilter
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
        public virtual StiFilterMode FilterMode { get; set; } = StiFilterMode.And;

        /// <summary>
        /// Gets or sets how a filter will be applied to data - be means of the report generator or by means of changing the SQL query.
        /// </summary>
        [DefaultValue(StiFilterEngine.ReportEngine)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataFilterEngine)]
        [Browsable(true)]
        [Description("Gets or sets how a filter will be applied to data - be means of the report generator or by means of changing the SQL query.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual StiFilterEngine FilterEngine { get; set; } = StiFilterEngine.ReportEngine;

        /// <summary>
		/// Gets or sets a method for filtration.
		/// </summary>
		[Browsable(false)]
        public StiFilterEventHandler FilterMethodHandler { get; set; }

        /// <summary>
		/// Gets or sets the collection of data filters.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
        [StiOrder(StiPropertyOrder.DataFilters)]
        [TypeConverter(typeof(StiFiltersCollectionConverter))]
        [StiCategory("Data")]
        [Editor("Stimulsoft.Report.Components.Design.StiFiltersCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets the collection of data filters.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual StiFiltersCollection Filters { get; set; } = new StiFiltersCollection();

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
        [StiPropertyLevel(StiLevel.Basic)]
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

        #region IStiOddEvenStyles
        public static object PropertyEvenStyle = new object();
        /// <summary>
        /// Gets or sets value, indicates style of even lines.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceEvenStyle)]
        [Description("Gets or sets value, indicates style of even lines.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Editor("Stimulsoft.Report.Design.StiStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiExpressionStyleConverter))]
        [StiExpressionAllowed]
        public virtual string EvenStyle
        {
            get
            {
                return Properties.Get(PropertyEvenStyle, string.Empty) as string;
            }
            set
            {
                Properties.Set(PropertyEvenStyle, value, string.Empty);
            }
        }

        public static object PropertyOddStyle = new object();
        /// <summary>
        /// Gets or sets value, indicates style of odd lines.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceOddStyle)]
        [Description("Gets or sets value, indicates style of odd lines.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [Editor("Stimulsoft.Report.Design.StiStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiExpressionStyleConverter))]
        [StiExpressionAllowed]
        public virtual string OddStyle
        {
            get
            {
                return Properties.Get(PropertyOddStyle, string.Empty) as string;
            }
            set
            {
                Properties.Set(PropertyOddStyle, value, string.Empty);
            }
        }
        #endregion

        #region StiService override
        public override void PackService()
        {
            base.PackService();
            Filters = null;
        }
        #endregion

        #region StiBand override
        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(90, 147, 204);

        /// <summary>
		/// Gets header end color.
		/// </summary>
		[Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(84, 175, 220);

        /// <summary>
        /// Returns the band header text.
        /// </summary>
        /// <returns>Band header text.</returns>
        public override string GetHeaderText()
        {
            var businessObject = BusinessObject;
            if (businessObject != null)
                return $"{ToString()}; {Loc.Get("PropertyMain", "BusinessObject")}: {businessObject.GetFullName()}";

            var text2 = $"{ToString()}; {Loc.Get("PropertyMain", "DataSource")}: ";
            if (DataSource != null)
                return $"{text2}{DataSource}";

            if (CountData > 0)
                return $"{text2}{CountData}";

            else
                return $"{text2}{Loc.Get("Report", "NotAssigned")}";
        }
        #endregion

        #region Render.Main.Methods
        public override bool DoBookmark()
        {
            if (Report != null && Report.EngineVersion == StiEngineVersion.EngineV2)
                return base.DoBookmark();
            return false;
        }

        public override void DoPointer(bool createNewGuid = true)
        {
            if (Report != null && Report.EngineVersion == StiEngineVersion.EngineV2)
                base.DoPointer(createNewGuid);
        }

        public void InvokeGroupRendering()
        {
            if (this.Report == null) return;

            if (this.Report.EngineVersion == StiEngineVersion.EngineV1)
            {
                var comps = this.DataBandInfoV1.GroupHeaderComponents;
                if (this is StiCrossDataBand && ((StiCrossDataBand)this).ColumnMode)
                    comps = ((StiDataBand)Parent).DataBandInfoV1.GroupHeaderComponents;

                if (comps != null)
                {
                    foreach (StiGroupHeaderBand header in comps)
                    {
                        header.InvokeRendering(Position);
                    }
                }
            }
            else
            {
                if (DataBandInfoV2.GroupHeaders != null)
                {
                    foreach (StiGroupHeaderBand header in DataBandInfoV2.GroupHeaders)
                    {
                        header.InvokeRendering();
                    }
                }
            }
        }
        #endregion

        #region Render
        private StiDataBandInfoV1 dataBandInfoV1;
        [Browsable(false)]
        public StiDataBandInfoV1 DataBandInfoV1
        {
            get
            {
                return dataBandInfoV1 ?? (dataBandInfoV1 = new StiDataBandInfoV1());
            }
        }

        private StiDataBandInfoV2 dataBandInfoV2;
        [Browsable(false)]
        public StiDataBandInfoV2 DataBandInfoV2
        {
            get
            {
                return dataBandInfoV2 ?? (dataBandInfoV2 = new StiDataBandInfoV2());
            }
        }

        [StiEngine(StiEngineVersion.EngineV2)]
        public override StiComponent Render()
        {
            return null;
        }
        #endregion

        #region Render.IStiRenderMaster
        [StiEngine(StiEngineVersion.EngineV2)]
        public virtual void RenderMaster()
        {
            var builder = StiV2Builder.GetBuilder(this.GetType()) as StiDataBandV2Builder;
            builder.RenderMaster(this);
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Return events collection of this component.
        /// </summary>
        public override StiEventsCollection GetEvents()
        {
            var events = base.GetEvents();

            if (BeginRenderEvent != null) 
                events.Add(BeginRenderEvent);

            if (RenderingEvent != null) 
                events.Add(RenderingEvent);

            if (EndRenderEvent != null) 
                events.Add(EndRenderEvent);

            if (GetCollapsedEvent != null)
                events.Add(GetCollapsedEvent);

            return events;
        }

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.DataBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
		/// Gets a component priority.
		/// </summary>
		public override int Priority => (int)StiComponentPriority.DataBand;

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                if (MasterComponent == null)
                    return StiComponentType.Master;

                if (MasterComponent != null)
                {
                    if (Page.Skip && MasterComponent.Page != Page)
                        return StiComponentType.Master;

                    if (Parent != MasterComponent.Parent)
                        return StiComponentType.Master;
                }

                return StiComponentType.Detail;
            }
        }

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiDataBand");
        #endregion

        #region Events
        #region OnBeginRender
        private static readonly object EventBeginRender = new object();

        /// <summary>
        /// Occurs when band begin render.
        /// </summary>
        public event EventHandler BeginRender
        {
            add
            {
                Events.AddHandler(EventBeginRender, value);
            }
            remove
            {
                Events.RemoveHandler(EventBeginRender, value);
            }
        }

        /// <summary>
        /// Raises the BeginRender event for this component.
        /// </summary>
        protected virtual void OnBeginRender(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the BeginRender event for this component.
        /// </summary>
        public void InvokeBeginRender()
        {
            try
            {
                #region EngineV1 Only
                this.DataBandInfoV1.LastPositionRendering = -1;
                this.DataBandInfoV1.RuntimeLine = 0;
                #endregion

                OnBeginRender(EventArgs.Empty);
                var handler = Events[EventBeginRender] as EventHandler;
                if (handler != null)
                    handler(this, EventArgs.Empty);

                StiBlocklyHelper.InvokeBlockly(this.Report, this, BeginRenderEvent);
            }
            catch (Exception ex)
            {
                string atCustomFunction = StiCustomFunctionHelper.CheckExceptionForCustomFunction(ex, this.Report, false);
                string str = $"Error in the 'BeginRender' event of '{Name}' component{atCustomFunction}: {ex.Message}";
                StiLogService.Write(this.GetType(), str);
                Report?.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when band begin render.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when band begin render.")]
        public virtual StiBeginRenderEvent BeginRenderEvent
        {
            get
            {
                return new StiBeginRenderEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion

        #region OnRendering
        private static readonly object EventRendering = new object();

        /// <summary>
        /// Occurs when a data row rendering.
        /// </summary>
        public event EventHandler Rendering
        {
            add
            {
                Events.AddHandler(EventRendering, value);
            }
            remove
            {
                Events.RemoveHandler(EventRendering, value);
            }
        }

        /// <summary>
        /// Raises the Rendering event for this component.
        /// </summary>
        protected virtual void OnRendering(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the Rendering event for this component.
        /// </summary>
		public void InvokeRendering()
        {
            try
            {
                if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV2)
                {
                    OnRendering(EventArgs.Empty);

                    var handler = Events[EventRendering] as EventHandler;
                    if (handler != null)
                        handler(this, EventArgs.Empty);
                }
                else
                {
                    if (this.DataBandInfoV1.LastPositionRendering != Position)
                    {
                        if (ResetPageNumber)
                            Report.EngineV1.ProcessResetPageNumber(Report);

                        this.DataBandInfoV1.RuntimeLine++;
                        LineThrough++;
                        this.DataBandInfoV1.LastPositionRendering = Position;

                        OnRendering(EventArgs.Empty);

                        var handler = Events[EventRendering] as EventHandler;
                        if (handler != null)
                            handler(this, EventArgs.Empty);
                    }
                }

                StiBlocklyHelper.InvokeBlockly(this.Report, this, RenderingEvent);
            }
            catch (Exception ex)
            {
                string atCustomFunction = StiCustomFunctionHelper.CheckExceptionForCustomFunction(ex, this.Report, false);
                string str = $"Error in the 'Rendering' event of '{Name}' component{atCustomFunction}: {ex.Message}";
                StiLogService.Write(this.GetType(), str);
                Report?.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
        /// Occurs when a data row rendering.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when a data row rendering.")]
        public virtual StiRenderingEvent RenderingEvent
        {
            get
            {
                return new StiRenderingEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion

        #region OnEndRender
        private static readonly object EventEndRender = new object();

        /// <summary>
        /// Occurs when band rendering is finished.
        /// </summary>
        public event EventHandler EndRender
        {
            add
            {
                Events.AddHandler(EventEndRender, value);
            }
            remove
            {
                Events.RemoveHandler(EventEndRender, value);
            }
        }

        /// <summary>
        /// Raises the EndRender event for this component.
        /// </summary>
        protected virtual void OnEndRender(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the EndRender event for this component.
        /// </summary>
        public void InvokeEndRender()
        {
            try
            {
                OnEndRender(EventArgs.Empty);

                var handler = Events[EventEndRender] as EventHandler;
                if (handler != null)
                    handler(this, EventArgs.Empty);

                StiBlocklyHelper.InvokeBlockly(this.Report, this, EndRenderEvent);
            }
            catch (Exception ex)
            {
                string atCustomFunction = StiCustomFunctionHelper.CheckExceptionForCustomFunction(ex, this.Report, false);
                string str = $"Error in the 'EndRender' event of '{Name}' component{atCustomFunction}: {ex.Message}";
                StiLogService.Write(this.GetType(), str);
                Report?.WriteToReportRenderingMessages(str);
            }
        }

        /// <summary>
		/// Occurs when band rendering is finished.
		/// </summary>
		[StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when band rendering is finished.")]
        public virtual StiEndRenderEvent EndRenderEvent
        {
            get
            {
                return new StiEndRenderEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion

        #region GetCollapsed
        private static readonly object EventGetCollapsed = new object();

        public event StiValueEventHandler GetCollapsed
        {
            add
            {
                Events.AddHandler(EventGetCollapsed, value);
            }
            remove
            {
                Events.RemoveHandler(EventGetCollapsed, value);
            }
        }

        /// <summary>
        /// Raises the GetCollapsed event for this component.
        /// </summary>
        protected virtual void OnGetCollapsed(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetCollapsed event for this component.
        /// </summary>
        public void InvokeGetCollapsed(StiValueEventArgs e)
        {
            try
            {
                OnGetCollapsed(e);

                if (Report != null && Report.CalculationMode == StiCalculationMode.Interpretation && this.Collapsed.Value.Length > 0)
                    e.Value = StiParser.ParseTextValue(this.Collapsed.Value, this);

                var handler = Events[EventGetCollapsed] as StiValueEventHandler;
                if (handler != null)
                    handler(this, e);

                StiBlocklyHelper.InvokeBlockly(this.Report, this, GetCollapsedEvent, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "DoGetCollapsed...ERROR");
                StiLogService.Write(this.GetType(), ex.Message);

                Report?.WriteToReportRenderingMessages($"{Name} {ex.Message}");
            }
        }

        /// <summary>
        /// Occurs when the Collapsed value is calculated.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when the Collapsed value is calculated.")]
        public virtual StiGetCollapsedEvent GetCollapsedEvent
        {
            get
            {
                return new StiGetCollapsedEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion
        #endregion

        #region Expressions
        #region Interaction.Collapsed
        /// <summary>
        /// Gets or sets an expression to fill a collapsed value.
        /// </summary>
        [StiCategory("Behavior")]
        [Browsable(false)]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [Description("Gets or sets an expression to fill a collapsed value.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiCollapsedExpression Collapsed
        {
            get
            {
                return new StiCollapsedExpression(this, "Collapsed");
            }
            set
            {
                if (value != null)
                    value.Set(this, "Collapsed", value.Value);
            }
        }
        #endregion
        #endregion

        #region Columns.Properties
        private bool rightToLeft;
        /// <summary>
        /// Gets or sets horizontal column direction.
        /// </summary>
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets horizontal column direction.")]
        [StiOrder(StiPropertyOrder.ColumnsRightToLeft)]
        [StiSerializable]
        [StiCategory("Columns")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual bool RightToLeft
        {
            get
            {
                return rightToLeft;
            }
            set
            {
                if (rightToLeft != value)
                {
                    CheckBlockedException("RightToLeft");
                    rightToLeft = value;
                }
            }
        }

        public double GetColumnWidth()
        {
            var bandColumnWidth = this.ColumnWidth;
            if (bandColumnWidth != 0)
                return bandColumnWidth;

            if (Columns == 0)
                return this.Width;

            return Width / Columns - ColumnGaps;
        }

        private double columnGaps;
        /// <summary>
        /// Gets or sets distance between two columns.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0d)]
        [StiCategory("Columns")]
        [StiOrder(StiPropertyOrder.ColumnsColumnGaps)]
        [Description("Gets or sets distance between two columns.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual double ColumnGaps
        {
            get
            {
                return columnGaps;
            }
            set
            {
                if (columnGaps == value) return;

                CheckBlockedException("ColumnGaps");

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "ColumnGaps",
                        $"Value of '{value}' is not valid for 'ColumnGaps'. 'ColumnGaps' must be greater than or equal to 0.");
                }
                columnGaps = Math.Round(value, 2);
            }
        }

        private double columnWidth;
        /// <summary>
        /// Gets or sets width of column.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0d)]
        [StiCategory("Columns")]
        [StiOrder(StiPropertyOrder.ColumnsColumnWidth)]
        [Description("Gets or sets width of column.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual double ColumnWidth
        {
            get
            {
                return columnWidth;
            }
            set
            {
                if (columnWidth == value) return;

                CheckBlockedException("ColumnWidth");
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "ColumnWidth",
                        $"Value of '{value}' is not valid for 'ColumnWidth'. 'ColumnWidth\' must be greater than or equal to 0.");
                }
                columnWidth = Math.Round(value, 2);
            }
        }

        private int columns;
        /// <summary>
        /// Gets or sets columns count.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0)]
        [StiCategory("Columns")]
        [StiOrder(StiPropertyOrder.ColumnsColumns)]
        [Description("Gets or sets columns count.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual int Columns
        {
            get
            {
                return columns;
            }
            set
            {
                if (columns == value) return;

                CheckBlockedException("Columns");
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "Columns",
                        $"Value of '{value}' is not valid for 'Columns'. 'Columns' must be greater than or equal to 0.");
                }
                columns = value;
            }
        }

        private int minRowsInColumn;
        /// <summary>
        /// Gets or sets minimum count of the rows in one column.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0)]
        [StiCategory("Columns")]
        [StiOrder(StiPropertyOrder.ColumnsMinRowsInColumn)]
        [Description("Gets or sets minimum count of the rows in one column.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual int MinRowsInColumn
        {
            get
            {
                return minRowsInColumn;
            }
            set
            {
                if (minRowsInColumn == value) return;
                CheckBlockedException("MinRowsInColumn");

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "MinRowsInColumn",
                        $"Value of '{value}' is not valid for 'MinRowsInColumn'. 'MinRowsInColumn' must be greater than or equal to 0.");
                }
                minRowsInColumn = value;
            }
        }

        private StiColumnDirection columnDirection = StiColumnDirection.AcrossThenDown;
        /// <summary>
        /// Gets or sets direction of the rendeting columns.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiColumnDirection.AcrossThenDown)]
        [StiCategory("Columns")]
        [StiOrder(StiPropertyOrder.ColumnsColumnDirection)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets direction of the rendeting columns.")]
        public virtual StiColumnDirection ColumnDirection
        {
            get
            {
                return columnDirection;
            }
            set
            {
                if (columnDirection != value)
                {
                    CheckBlockedException("ColumnDirection");
                    columnDirection = value;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of line from the beginning of the report formation.
        /// </summary>
        [Browsable(false)]
        public int LineThrough { get; set; } = 1;

        private int line = 1;
        /// <summary>
        /// Gets the number of line from the beginning of the group formation.
        /// </summary>
        [Browsable(false)]
        public int Line
        {
            get
            {
                if (this.Report == null)
                    return line;

                if (this.Report.EngineVersion == StiEngineVersion.EngineV1)
                    return this.DataBandInfoV1.RuntimeLine - DataBandInfoV1.StartLine;

                else
                    return line;
            }
            set
            {
                line = value;
            }
        }

        /// <summary>
		/// Gets or sets value, indicates to reset Data Source postion to begin when preparation for rendering.
		/// </summary>
		[StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataResetDataSource)]
        [Description("Gets or sets value, indicates to reset Data Source postion to begin when preparation for rendering.")]
        [StiEngine(StiEngineVersion.EngineV1)]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool ResetDataSource { get; set; }

        /// <summary>
        /// Initialize the data source for each container and detail section. For example, Filters will be applied for each detail section even if Relation is not assigned.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataMultipleInitialization)]
        [Description("Initialize the data source for each container and detail section. For example, Filters will be applied for each detail section even if Relation is not assigned.")]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool MultipleInitialization { get; set; }

        /// <summary>
		/// Gets or sets value which indicates that, 
		/// when aggregate functions calculation, it is nesessary to take into consideration invisible data bands.
		/// </summary>
		[StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.DataCalcInvisible)]
        [Description("Gets or sets value which indicates " +
             "that, when aggregate functions calculation, it is nesessary to take into consideration invisible data bands.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual bool CalcInvisible { get; set; }

        private int countData;
        /// <summary>
        /// Gets or sets the count of rows for virtual data.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.All)]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataCountData)]
        [Description("Gets or sets the count of rows for virtual data.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual int CountData
        {
            get
            {
                return countData;
            }
            set
            {
                if (countData != value)
                {
                    countData = value;

                    if (value > 0)
                        DataSourceName = string.Empty;
                }
            }
        }


        /// <summary>
        /// Gets or sets the limit of rows for break page or column.
        /// </summary>
        [Editor("Stimulsoft.Report.Components.Design.StiExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("PageColumnBreak")]
        [StiOrder(StiPropertyOrder.PageColumnBreakLimitRows)]
        [Description("Gets or sets the limit of rows for break.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual string LimitRows { get; set; } = string.Empty;
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiDataBand();
        }
        #endregion

        /// <summary>
		/// Creates an object of the type StiDataBand.
		/// </summary>
		public StiDataBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        ///  Creates an object of the type StiDataBand.
        /// </summary>1
        /// <param name="rect">The rectangle decribes size and position of the component.</param>
        public StiDataBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;

            PrintIfDetailEmpty = StiOptions.Engine.PrintIfDetailEmptyDefaultValue;
        }
    }
}