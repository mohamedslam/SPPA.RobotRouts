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
using System.Drawing;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Json.Linq;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the class that realizes the band - Footer band.
    /// </summary>
    [StiServiceBitmap(typeof(StiFooterBand), "Stimulsoft.Report.Images.Components.StiFooterBand.png")]
    [StiToolbox(true)]
    [StiV1Builder(typeof(StiFooterBandV1Builder))]
    [StiV2Builder(typeof(StiFooterBandV2Builder))]
    [StiContextTool(typeof(IStiPrintOnAllPages))]
    [StiContextTool(typeof(IStiPrintIfEmpty))]
    [StiContextTool(typeof(IStiStartNewPage))]
    [StiContextTool(typeof(IStiBreakable))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiKeepFooterTogether))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiFooterBand :
        StiDynamicBand,
        IStiPrintOnAllPages,
        IStiPrintIfEmpty,
        IStiStartNewPage,
        IStiKeepFooterTogether,
        IStiPrintOnEvenOddPages
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyBool("CanBreak", CanBreak);

            // StiFooterBand
            jObject.AddPropertyBool("KeepFooterTogether", KeepFooterTogether, true);
            jObject.AddPropertyBool("StartNewPage", StartNewPage);
            jObject.AddPropertyFloat("StartNewPageIfLessThan", StartNewPageIfLessThan, 100f);
            jObject.AddPropertyBool("PrintIfEmpty", PrintIfEmpty);
            jObject.AddPropertyEnum("PrintOnEvenOddPages", PrintOnEvenOddPages, StiPrintOnEvenOddPagesType.Ignore);
            jObject.AddPropertyBool("PrintOnAllPages", PrintOnAllPages);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "KeepFooterTogether":
                        this.keepFooterTogether = property.DeserializeBool();
                        break;

                    case "StartNewPage":
                        this.StartNewPage = property.DeserializeBool();
                        break;

                    case "StartNewPageIfLessThan":
                        this.StartNewPageIfLessThan = property.DeserializeFloat();
                        break;

                    case "PrintIfEmpty":
                        this.printIfEmpty = property.DeserializeBool();
                        break;

                    case "PrintOnEvenOddPages":
                        this.PrintOnEvenOddPages = property.DeserializeEnum<StiPrintOnEvenOddPagesType>();
                        break;

                    case "PrintOnAllPages":
                        this.PrintOnAllPages = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiFooterBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

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
            

            // AppearanceCategory
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
                    propHelper.KeepFooterTogether(),
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
        public override string HelpUrl => "user-manual/report_internals_creating_lists_list_with_footer.htm";
        #endregion

        #region IStiKeepFooterTogether
        private bool keepFooterTogether = true;
        /// <summary>
        /// Gets or sets value indicates that the footer is printed with data.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the footer is printed with data.")]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepFooterTogether
        {
            get
            {
                return keepFooterTogether;
            }
            set
            {
                if (keepFooterTogether != value)
                {
                    CheckBlockedException("KeepFooterTogether");
                    keepFooterTogether = value;
                }
            }
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
                this.FooterBandInfoV1.ForceCanBreak = value;
            }
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties, bool cloneComponents)
        {
            var band = base.Clone(cloneProperties, cloneComponents) as StiFooterBand;

            band.footerBandInfoV1 = this.FooterBandInfoV1.Clone() as StiFooterBandInfoV1;
            band.footerBandInfoV2 = this.FooterBandInfoV2.Clone() as StiFooterBandInfoV2;

            return band;
        }
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

        #region IStiPrintIfEmpty
        private bool printIfEmpty;
        /// <summary>
        /// Gets or sets value indicates that the footer is printed if data not present.
        /// </summary>
        [DefaultValue(false)]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintIfEmpty)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the footer if data not present.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintIfEmpty
        {
            get
            {
                return printIfEmpty;
            }
            set
            {
                if (printIfEmpty != value)
                {
                    CheckBlockedException("PrintIfEmpty");
                    printIfEmpty = value;
                }
            }
        }
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

        #region Render override
        private StiFooterBandInfoV1 footerBandInfoV1;
        [Browsable(false)]
        public StiFooterBandInfoV1 FooterBandInfoV1
        {
            get
            {
                return footerBandInfoV1 ?? (footerBandInfoV1 = new StiFooterBandInfoV1());
            }
        }

        private StiFooterBandInfoV2 footerBandInfoV2;
        [Browsable(false)]
        public StiFooterBandInfoV2 FooterBandInfoV2
        {
            get
            {
                return footerBandInfoV2 ?? (footerBandInfoV2 = new StiFooterBandInfoV2());
            }
        }
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

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.FooterBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.FooterBand;

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType => StiComponentType.Detail;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiFooterBand");
        #endregion

        #region Events
        private static readonly object EventMoveFooterToBottom = new object();

        /// <summary>
        /// Internal use only!
        /// </summary>
        public event EventHandler MoveFooterToBottom
        {
            add
            {
                Events.AddHandler(EventMoveFooterToBottom, value);
            }
            remove
            {
                Events.RemoveHandler(EventMoveFooterToBottom, value);
            }
        }

        /// <summary>
        /// Raises the MoveFooterToBottom event for this component.
        /// </summary>
        protected virtual void OnMoveFooterToBottom(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the MoveFooterToBottom event for this component.
        /// </summary>
        public void InvokeMoveFooterToBottom()
        {
            OnMoveFooterToBottom(EventArgs.Empty);

            var handler = Events[EventMoveFooterToBottom] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiFooterBand();
        }
        #endregion

        /// <summary>
		/// Creates a new component of the type StiFooterBand.
		/// </summary>
		public StiFooterBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiFooterBand with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiFooterBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;
        }
    }
}