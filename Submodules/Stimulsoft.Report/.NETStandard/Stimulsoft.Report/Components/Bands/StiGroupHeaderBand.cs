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
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
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
    /// The class describes the Group Header Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiGroupHeaderBand), "Stimulsoft.Report.Images.Components.StiGroupHeaderBand.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiGroupHeaderBandDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfGroupHeaderBandDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiGroupHeaderBandV1Builder))]
    [StiV2Builder(typeof(StiGroupHeaderBandV2Builder))]
    [StiContextTool(typeof(IStiGroup))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiBreakable))]
    [StiContextTool(typeof(IStiPrintOnAllPages))]
    [StiContextTool(typeof(IStiKeepHeaderTogether))]
    [StiContextTool(typeof(IStiStartNewPage))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiContextTool(typeof(IStiKeepGroupTogether))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiGroupHeaderBand :
        StiDynamicBand,
        IStiGroup,
        IStiPrintOnAllPages,
        IStiKeepGroupTogether,
        IStiStartNewPage
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyBool("CanBreak", CanBreak);

            // StiGroupHeaderBand
            jObject.AddPropertyBool("KeepGroupHeaderTogether", KeepGroupHeaderTogether, true);
            jObject.AddPropertyBool("KeepGroupTogether", KeepGroupTogether);
            jObject.AddPropertyBool("StartNewPage", StartNewPage);
            jObject.AddPropertyFloat("StartNewPageIfLessThan", StartNewPageIfLessThan, 100f);
            jObject.AddPropertyEnum("SortDirection", SortDirection, StiGroupSortDirection.Ascending);
            jObject.AddPropertyEnum("SummarySortDirection", SummarySortDirection, StiGroupSortDirection.None);
            jObject.AddPropertyEnum("SummaryType", SummaryType, StiGroupSummaryType.Sum);
            jObject.AddPropertyBool("PrintOnAllPages", PrintOnAllPages);
            jObject.AddPropertyJObject("Condition", Condition.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("SummaryExpression", SummaryExpression.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetSummaryExpressionEvent", GetSummaryExpressionEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetValueEvent", GetValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("GetCollapsedEvent", GetCollapsedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("BeginRenderEvent", BeginRenderEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("RenderingEvent", RenderingEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("EndRenderEvent", EndRenderEvent.SaveToJsonObject(mode));

            if (mode == StiJsonSaveMode.Report)
            {
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
                    case "KeepGroupHeaderTogether":
                        this.keepGroupHeaderTogether = property.DeserializeBool();
                        break;

                    case "KeepGroupTogether":
                        this.KeepGroupTogether = property.DeserializeBool();
                        break;

                    case "StartNewPage":
                        this.StartNewPage = property.DeserializeBool();
                        break;

                    case "StartNewPageIfLessThan":
                        this.StartNewPageIfLessThan = property.DeserializeFloat();
                        break;

                    case "SortDirection":
                        this.SortDirection = property.DeserializeEnum<StiGroupSortDirection>();
                        break;

                    case "SummarySortDirection":
                        this.summarySortDirection = property.DeserializeEnum<StiGroupSortDirection>();
                        break;

                    case "SummaryType":
                        this.SummaryType = property.DeserializeEnum<StiGroupSummaryType>();
                        break;

                    case "PrintOnAllPages":
                        this.PrintOnAllPages = property.DeserializeBool();
                        break;

                    case "Condition":
                        {
                            var _expression = new StiGroupConditionExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.Condition = _expression;
                        }
                        break;

                    case "SummaryExpression":
                        {
                            var _expression = new StiGroupSummaryExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.SummaryExpression = _expression;
                        }
                        break;

                    case "Collapsed":
                        {
                            var _expression = new StiCollapsedExpression();
                            _expression.LoadFromJsonObject((JObject)property.Value);
                            this.Collapsed = _expression;
                        }
                        break;

                    case "GetSummaryExpressionEvent":
                        {
                            var _event = new StiGetSummaryExpressionEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetSummaryExpressionEvent = _event;
                        }
                        break;

                    case "GetValueEvent":
                        {
                            var _event = new StiGetGroupConditionEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetValueEvent = _event;
                        }
                        break;

                    case "GetCollapsedEvent":
                        {
                            var _event = new StiGetCollapsedEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.GetCollapsedEvent = _event;
                        }
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
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiGroupHeaderBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.GroupHeaderEditor(),
            });

            // DataCategory
            if (level != StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Data, new[]
                {
                    propHelper.GroupHeaderExpression(),
                    propHelper.SortDirection(),
                    propHelper.SummarySortDirection(),
                    propHelper.SummaryExpression(),
                    propHelper.SummaryType()
                });
            }

            // PageColumnBreakCategory
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
                    propHelper.SkipFirst()
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
                    propHelper.MinHeight(),
                    propHelper.MaxHeight()
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
                    propHelper.CanBreak(),
                    propHelper.Enabled()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.KeepGroupHeaderTogether(),
                    propHelper.KeepGroupTogether(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.PrintAtBottom(),
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
                        StiPropertyEventId.BeforePrintEvent,
                        StiPropertyEventId.AfterPrintEvent
                    }
                },
                {
                    StiPropertyCategories.RenderEvents,
                    new[]
                    {
                        StiPropertyEventId.BeginRenderEvent,
                        StiPropertyEventId.RenderingEvent,
                        StiPropertyEventId.EndRenderEvent
                    }
                },
                {
                    StiPropertyCategories.ValueEvents,
                    new[]
                    {
                        StiPropertyEventId.GetCollapsedEvent,
                        StiPropertyEventId.GetSummaryExpressionEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                        StiPropertyEventId.GetValueEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/report_internals_groups_groupheaderband.htm";
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties, bool cloneComponents)
        {
            var band = base.Clone(cloneProperties, cloneComponents) as StiGroupHeaderBand;

            band.groupHeaderBandInfoV1 = this.GroupHeaderBandInfoV1.Clone() as StiGroupHeaderBandInfoV1;
            band.groupHeaderBandInfoV2 = this.GroupHeaderBandInfoV2.Clone() as StiGroupHeaderBandInfoV2;

            return band;
        }
        #endregion

        #region IStiBreakable override
        /// <summary>
        /// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorCanBreak)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether the component can or cannot break its contents on several pages.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
        public override bool CanBreak
        {
            get
            {
                return base.CanBreak;
            }
            set
            {
                base.CanBreak = value;
                this.GroupHeaderBandInfoV1.ForceCanBreak = value;
            }
        }
        #endregion

        #region IStiResetPageNumber
        /// <summary>
        /// Allows to reset page number on this component.
        /// </summary>
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

        #region IStiKeepGroupHeaderTogether
        private bool keepGroupHeaderTogether = true;
        /// <summary>
        /// Gets or sets value indicates that group header is printed with data together.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that group header is printed with data together.")]
        [StiShowInContextMenu]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepGroupHeaderTogether
        {
            get
            {
                return keepGroupHeaderTogether;
            }
            set
            {
                if (keepGroupHeaderTogether != value)
                {
                    CheckBlockedException("KeepGroupHeaderTogether");
                    keepGroupHeaderTogether = value;
                }
            }
        }
        #endregion

        #region IStiKeepGroupTogether
        /// <summary>
        /// Gets or sets value indicates that group is to be kept together.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that group is to be kept together.")]
        [StiShowInContextMenu]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepGroupTogether { get; set; }
        #endregion

        #region IStiStartNewPage
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

        #region IStiStateSaveRestore
        /// <summary>
        /// Saves the current state of an object.
        /// </summary>
        /// <param name="stateName">A name of the state being saved.</param>
        public override void SaveState(string stateName)
        {
            base.SaveState(stateName);

            States.PushInt(stateName, this, "lastPositionRendering", this.GroupHeaderBandInfoV1.LastPositionRendering);
            States.PushInt(stateName, this, "lastPositionLineRendering", this.GroupHeaderBandInfoV1.LastPositionLineRendering);
            States.PushInt(stateName, this, "line", Line);
        }

        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public override void RestoreState(string stateName)
        {
            if (States.IsExist(stateName, this))
            {
                GroupHeaderBandInfoV1.LastPositionRendering = States.PopInt(stateName, this, "lastPositionRendering");
                GroupHeaderBandInfoV1.LastPositionLineRendering = States.PopInt(stateName, this, "lastPositionLineRendering");
                Line = States.PopInt(stateName, this, "line");
            }

            base.RestoreState(stateName);
        }
        #endregion

        #region IStiGroup
        /// <summary>
        /// Gets or sets sorting direction of group by totals
        /// </summary>
        [DefaultValue(StiGroupSortDirection.Ascending)]
        [StiSerializable]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataSortDirection)]
        [Description("Gets or sets sorting direction of group by totals")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiGroupSortDirection SortDirection { get; set; } = StiGroupSortDirection.Ascending;

        private StiGroupSortDirection summarySortDirection = StiGroupSortDirection.None;
        /// <summary>
        /// Gets or sets function of calculating group totals for its sorting by totals.
        /// </summary>
        [DefaultValue(StiGroupSortDirection.None)]
        [StiSerializable]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataSummarySortDirection)]
        [Description("Gets or sets function of calculating group totals for its sorting by totals.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiEngine(StiEngineVersion.EngineV2)]
        public StiGroupSortDirection SummarySortDirection
        {
            get
            {
                return Report == null || Report.EngineVersion == StiEngineVersion.EngineV1
                    ? StiGroupSortDirection.None
                    : summarySortDirection;
            }
            set
            {
                summarySortDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets the sorting direction of grouped data.
        /// </summary>
        [DefaultValue(StiGroupSummaryType.Sum)]
        [StiSerializable]
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataSummaryType)]
        [Description("Gets or sets the sorting direction of grouped data.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiEngine(StiEngineVersion.EngineV2)]
        public StiGroupSummaryType SummaryType { get; set; } = StiGroupSummaryType.Sum;
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

        #region Render
        private StiGroupHeaderBandInfoV1 groupHeaderBandInfoV1;
        [Browsable(false)]
        public StiGroupHeaderBandInfoV1 GroupHeaderBandInfoV1
        {
            get
            {
                return groupHeaderBandInfoV1 ?? (groupHeaderBandInfoV1 = new StiGroupHeaderBandInfoV1());
            }
        }

        private StiGroupHeaderBandInfoV2 groupHeaderBandInfoV2;
        [Browsable(false)]
        public StiGroupHeaderBandInfoV2 GroupHeaderBandInfoV2
        {
            get
            {
                return groupHeaderBandInfoV2 ?? (groupHeaderBandInfoV2 = new StiGroupHeaderBandInfoV2());
            }
        }

        /// <summary>
        /// Returns the DataBand in which the component is located.
        /// Returns null, if nothing is located. 
        /// </summary>
        /// <returns>A DataBand in which the component is located.</returns>
        public override StiDataBand GetDataBand()
        {
            var obj = this.Parent;
            if (obj == null) return null;

            var groupIndex = this.Parent.Components.IndexOf(this) + 1;

            for (var index = groupIndex; index < this.Parent.Components.Count; index++)
            {
                if (this.Parent.Components[index] is StiDataBand)
                    return this.Parent.Components[index] as StiDataBand;
            }

            return null;
        }

        [Browsable(false)]
        public int Line { get; set; }
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

            if (GetValueEvent != null)
                events.Add(GetValueEvent);

            if (GetCollapsedEvent != null)
                events.Add(GetCollapsedEvent);

            if (GetSummaryExpressionEvent != null)
                events.Add(GetSummaryExpressionEvent);

            return events;
        }

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.GroupHeaderBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType => StiComponentType.Detail;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiGroupHeaderBand");

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.GroupHeaderBand;
        #endregion

        #region Expressions
        #region Condition
        /// <summary>
        /// Gets or sets grouping condition.
        /// </summary>
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataCondition)]
        [StiSerializable]
        [Description("Gets or sets grouping condition.")]
        [Editor("Stimulsoft.Report.Components.Design.StiGroupConditionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiGroupConditionExpression Condition
        {
            get
            {
                return new StiGroupConditionExpression(this, "Condition");
            }
            set
            {
                if (value != null)
                    value.Set(this, "Condition", value.Value);
            }
        }
        #endregion

        #region GroupSummary
        /// <summary>
        /// Gets or sets summary expression which is used for group summary totals calculation.
        /// </summary>
        [StiCategory("Data")]
        [StiOrder(StiPropertyOrder.DataSummaryExpression)]
        [StiSerializable]
        [Description("Gets or sets summary expression which is used for group summary totals calculation.")]
        [Editor("Stimulsoft.Report.Components.Design.StiGroupSummaryEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiEngine(StiEngineVersion.EngineV2)]
        public StiGroupSummaryExpression SummaryExpression
        {
            get
            {
                return new StiGroupSummaryExpression(this, "SummaryExpression");
            }
            set
            {
                if (value != null)
                    value.Set(this, "SummaryExpression", value.Value);
            }
        }
        #endregion

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
        [StiPropertyLevel(StiLevel.Basic)]
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

        #region Events
        #region GetGroupSummary
        private static readonly object EventGetSummaryExpression = new object();

        /// <summary>
        /// Occurs when when group summary expression is calculated.
        /// </summary>
        public event StiValueEventHandler GetSummaryExpression
        {
            add
            {
                Events.AddHandler(EventGetSummaryExpression, value);
            }
            remove
            {
                Events.RemoveHandler(EventGetSummaryExpression, value);
            }
        }

        /// <summary>
        /// Raises the GetSummaryExpression event for this component.
        /// </summary>
        protected virtual void OnGetSummaryExpression(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetSummaryExpression event for this component.
        /// </summary>
        public void InvokeGetSummaryExpression(StiValueEventArgs e)
        {
            try
            {
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    OnGetSummaryExpression(e);

                    var handler = Events[EventGetSummaryExpression] as StiValueEventHandler;
                    if (handler != null)
                        handler(this, e);
                }
                else
                {
                    OnGetSummaryExpression(e);

                    e.Value = StiParser.ParseTextValue(this.SummaryExpression.Value, this);

                    var handler = Events[EventGetSummaryExpression] as StiValueEventHandler;
                    if (handler != null)
                        handler(this, e);
                }


                StiBlocklyHelper.InvokeBlockly(this.Report, this, GetSummaryExpressionEvent, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "DoGetSummaryExpression...ERROR");
                StiLogService.Write(this.GetType(), ex.Message);

                if (Report != null)
                    Report.WriteToReportRenderingMessages($"{Name} {ex.Message}");
            }
        }

        /// <summary>
        /// Occurs when when group summary expression is calculated.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when when group summary expression is calculated.")]
        public StiGetSummaryExpressionEvent GetSummaryExpressionEvent
        {
            get
            {
                return new StiGetSummaryExpressionEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion

        #region GetValue
        private static readonly object EventGetValue = new object();

        /// <summary>
        /// Occurs when when is checked condition of the group.
        /// </summary>
        public event StiValueEventHandler GetValue
        {
            add
            {
                Events.AddHandler(EventGetValue, value);
            }
            remove
            {
                Events.RemoveHandler(EventGetValue, value);
            }
        }

        /// <summary>
        /// Raises the GetValue event for this component.
        /// </summary>
        protected virtual void OnGetValue(StiValueEventArgs e)
        {
        }

        /// <summary>
        /// Raises the GetValue event for this component.
        /// </summary>
        public void InvokeGetValue(StiValueEventArgs e)
        {
            try
            {
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    OnGetValue(e);

                    var handler = Events[EventGetValue] as StiValueEventHandler;
                    if (handler != null)
                        handler(this, e);
                }
                else
                {
                    OnGetValue(e);

                    e.Value = StiParser.ParseTextValue(this.Condition.Value, this);

                    var handler = Events[EventGetValue] as StiValueEventHandler;
                    if (handler != null)
                        handler(this, e);
                }

                StiBlocklyHelper.InvokeBlockly(this.Report, this, GetValueEvent, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "DoGetValue...ERROR");
                StiLogService.Write(this.GetType(), ex.Message);

                if (Report != null)
                    Report.WriteToReportRenderingMessages($"{Name} {ex.Message}");
            }
        }

        /// <summary>
        /// Occurs when when is checked condition of the group.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when when is checked condition of the group.")]
        public StiGetGroupConditionEvent GetValueEvent
        {
            get
            {
                return new StiGetGroupConditionEvent(this);
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

                if (Report != null && Report.CalculationMode == StiCalculationMode.Interpretation)
                {
                    if (this.Collapsed.Value.Length > 0)
                        e.Value = StiParser.ParseTextValue(this.Collapsed.Value, this);
                }

                var handler = Events[EventGetCollapsed] as StiValueEventHandler;
                if (handler != null)
                    handler(this, e);

                StiBlocklyHelper.InvokeBlockly(this.Report, this, GetCollapsedEvent, e);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), "DoGetCollapsed...ERROR");
                StiLogService.Write(this.GetType(), ex.Message);

                if (Report != null)
                    Report.WriteToReportRenderingMessages($"{Name} {ex.Message}");
            }
        }

        /// <summary>
        /// Occurs when the Collapsed value is calculated.
        /// </summary>
        [StiSerializable]
        [StiCategory("ValueEvents")]
        [Browsable(false)]
        [Description("Occurs when the Collapsed value is calculated.")]
        public StiGetCollapsedEvent GetCollapsedEvent
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

        #region OnBeginRender
        private static readonly object EventBeginRender = new object();

        /// <summary>
        /// Occurs when band is begin render.
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
            if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1 && ResetPageNumber)
                Report.EngineV1.ProcessResetPageNumber(Report);
        }

        /// <summary>
        /// Raises the BeginRender event for this component.
        /// </summary>
        [StiEngine(StiEngineVersion.EngineV2)]
        public void InvokeBeginRender()
        {
            OnBeginRender(EventArgs.Empty);

            var handler = Events[EventBeginRender] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the BeginRender event for this component.
        /// </summary>
        [StiEngine(StiEngineVersion.EngineV1)]
        public void InvokeBeginRender(StiDataBand dataBand, int position)
        {
            OnBeginRender(EventArgs.Empty);

            if (GroupHeaderBandInfoV1.LastPositionLineRendering != position)
            {
                GroupHeaderBandInfoV1.LastPositionLineRendering = position;
                Line++;

                var groups = dataBand.DataBandInfoV1.GroupHeaderComponents;

                var clear = false;
                foreach (StiGroupHeaderBand group in groups)
                {
                    if (clear)
                        group.Line = 0;
                    else
                        if (group == this) clear = true;
                }
            }

            var handler = Events[EventBeginRender] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when band is begin render.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when band is begin render.")]
        public StiBeginRenderEvent BeginRenderEvent
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
        /// Occurs when occurs rendering of one line data.
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
        [StiEngine(StiEngineVersion.EngineV2)]
        public void InvokeRendering()
        {
            try
            {
                OnRendering(EventArgs.Empty);

                var handler = Events[EventRendering] as EventHandler;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StiLogService.Write(this.GetType(), Name + " InvokeRendering...ERROR");
                StiLogService.Write(this.GetType(), Name + " " + ex.Message);

                if (Report != null)
                    Report.WriteToReportRenderingMessages($"Error in the 'Rendering' event of '{Name}' component: {ex.Message}");
            }
        }

        /// <summary>
        /// Raises the Rendering event for this component.
        /// </summary>
        [StiEngine(StiEngineVersion.EngineV1)]
        public void InvokeRendering(int position)
        {
            if (GroupHeaderBandInfoV1.LastPositionRendering == position) return;

            GroupHeaderBandInfoV1.LastPositionRendering = position;

            OnRendering(EventArgs.Empty);


            var handler = Events[EventRendering] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(this.Report, this, RenderingEvent);
        }

        /// <summary>
        /// Occurs when occurs rendering of one line data.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when occurs rendering of one line data.")]
        public StiRenderingEvent RenderingEvent
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
        /// Occurs when ends rendering band.
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
            OnEndRender(EventArgs.Empty);

            var handler = Events[EventEndRender] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(this.Report, this, EndRenderEvent);
        }

        /// <summary>
        /// Occurs when ends rendering band.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when ends rendering band.")]
        public StiEndRenderEvent EndRenderEvent
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
        #endregion

        #region StiBand override
        /// <summary>
        /// Returns the band header text.
        /// </summary>
        /// <returns>Band header text.</returns>
        public override string GetHeaderText()
        {
            string str;

            if (string.IsNullOrEmpty(Alias))
            {
                str = Name;
            }
            else
            {
                var useAliases = Report?.Designer?.UseAliases;

                if (StiOptions.Dictionary.ShowOnlyAliasForComponents && useAliases.GetValueOrDefault(false))
                    str = Alias;
                else
                    str = $"{Name} [{Alias}]";
            }

            var condition = string.IsNullOrWhiteSpace(Condition.Value) 
                ? Loc.Get("FormStyleDesigner", "NotSpecified") 
                : Condition.Value;

            return $"{str}; {Loc.Get("PropertyMain", "Condition")}: {condition}";
        }

        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(239, 155, 52);

        /// <summary>
        /// Gets header end color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(239, 155, 52);
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiGroupHeaderBand();
        }
        #endregion

        #region Methods
        public object GetCurrentConditionValue()
        {
            return StiGroupHeaderBandV2Builder.GetCurrentConditionValue(this);
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiComponent.
        /// </summary>
        public StiGroupHeaderBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiComponent with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiGroupHeaderBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;
        }
    }
}