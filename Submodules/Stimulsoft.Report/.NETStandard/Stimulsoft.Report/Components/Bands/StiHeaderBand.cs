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
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using System.ComponentModel;
using System.Drawing;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Desribes the class that realizes the band - Header Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiHeaderBand), "Stimulsoft.Report.Images.Components.StiHeaderBand.png")]
    [StiToolbox(true)]
    [StiV1Builder(typeof(StiHeaderBandV1Builder))]
    [StiV2Builder(typeof(StiHeaderBandV2Builder))]
    [StiContextTool(typeof(IStiPrintIfEmpty))]
    [StiContextTool(typeof(IStiPrintOnAllPages))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiStartNewPage))]
    [StiContextTool(typeof(IStiBreakable))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiHeaderBand :
        StiDynamicBand,
        IStiPrintIfEmpty,
        IStiPrintOnAllPages,
        IStiPrintOnEvenOddPages,
        IStiStartNewPage,
        IStiKeepHeaderTogether
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyBool("CanBreak", CanBreak);

            // StiHeaderBand
            jObject.AddPropertyBool("KeepHeaderTogether", KeepHeaderTogether, true);
            jObject.AddPropertyBool("StartNewPage", StartNewPage);
            jObject.AddPropertyFloat("StartNewPageIfLessThan", StartNewPageIfLessThan, 100f);
            jObject.AddPropertyBool("PrintIfEmpty", PrintIfEmpty);
            jObject.AddPropertyBool("PrintOnAllPages", PrintOnAllPages, true);
            jObject.AddPropertyEnum("PrintOnEvenOddPages", PrintOnEvenOddPages, StiPrintOnEvenOddPagesType.Ignore);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "KeepHeaderTogether":
                        this.keepHeaderTogether = property.DeserializeBool();
                        break;

                    case "StartNewPage":
                        this.StartNewPage = property.DeserializeBool();
                        break;

                    case "StartNewPageIfLessThan":
                        this.StartNewPageIfLessThan = property.DeserializeFloat();
                        break;

                    case "PrintIfEmpty":
                        this.PrintIfEmpty = property.DeserializeBool();
                        break;

                    case "PrintOnAllPages":
                        this.PrintOnAllPages = property.DeserializeBool();
                        break;

                    case "PrintOnEvenOddPages":
                        this.PrintOnEvenOddPages = property.DeserializeEnum<StiPrintOnEvenOddPagesType>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiHeaderBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

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
                    propHelper.KeepHeaderTogether(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.PrintAtBottom(),
                    propHelper.PrintIfEmpty(),
                    propHelper.PrintOn(),
                    propHelper.PrintOnAllPages(),
                    propHelper.PrintOnEvenOddPages(),
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
        public override string HelpUrl => "user-manual/report_internals_creating_lists_list_with_header.htm";
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties, bool cloneComponents)
        {
            var band = base.Clone(cloneProperties, cloneComponents) as StiHeaderBand;

            band.headerBandInfoV1 = this.HeaderBandInfoV1.Clone() as StiHeaderBandInfoV1;
            band.headerBandInfoV2 = this.HeaderBandInfoV2.Clone() as StiHeaderBandInfoV2;

            return band;
        }
        #endregion

        #region IStiKeepHeaderTogether
        private bool keepHeaderTogether = true;
        /// <summary>
        /// Gets or sets value indicates that header is printed with data together.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that header is printed with data together.")]
        [StiShowInContextMenu]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepHeaderTogether
        {
            get
            {
                return keepHeaderTogether;
            }
            set
            {
                if (keepHeaderTogether != value)
                {
                    CheckBlockedException("KeepHeaderTogether");
                    keepHeaderTogether = value;
                }
            }
        }
        #endregion

        #region IStiBreakable
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
                this.HeaderBandInfoV1.ForceCanBreak = value;
            }
        }
        #endregion

        #region IStiStartNewPage
        /// <summary>
        /// Gets or sets value indicates that it is necessary to print every new string on a new page.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.BehaviorStartNewPage)]
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

        #region IStiResetPageNumber
        /// <summary>
        /// Allows to reset page number on this band.
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

        #region IStiPrintIfEmpty
        /// <summary>
        /// Gets or sets value indicates that the header if data not present.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintIfEmpty)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the header if data not present.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintIfEmpty { get; set; }
        #endregion

        #region IStiPrintOnAllPages
        /// <summary>
        /// Gets or sets value indicates that the component is printed on all pages.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintOnAllPages)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the component is printed on all pages.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintOnAllPages { get; set; } = true;
        #endregion

        #region IStiPrintOnEvenOddPages
        /// <summary>
        /// Gets or sets value indicates that the component is printed on even-odd pages.
        /// </summary>
        [DefaultValue(StiPrintOnEvenOddPagesType.Ignore)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintOnEvenOddPages)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the component is printed on even-odd pages.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiPrintOnEvenOddPagesType PrintOnEvenOddPages { get; set; } = StiPrintOnEvenOddPagesType.Ignore;
        #endregion

        #region Render override
        private StiHeaderBandInfoV1 headerBandInfoV1;
        [Browsable(false)]
        public StiHeaderBandInfoV1 HeaderBandInfoV1
        {
            get
            {
                return headerBandInfoV1 ?? (headerBandInfoV1 = new StiHeaderBandInfoV1());
            }
        }

        private StiHeaderBandInfoV2 headerBandInfoV2;
        [Browsable(false)]
        public StiHeaderBandInfoV2 HeaderBandInfoV2
        {
            get
            {
                return headerBandInfoV2 ?? (headerBandInfoV2 = new StiHeaderBandInfoV2());
            }
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.HeaderBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.HeaderBand;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiHeaderBand");

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType => StiComponentType.Detail;
        #endregion

        #region StiBand override
        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(178, 197, 223);

        /// <summary>
        /// Gets header end color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(178, 197, 223);
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiHeaderBand();
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiHeaderBand.
        /// </summary>
        public StiHeaderBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiHeaderBand with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiHeaderBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;
        }
    }
}