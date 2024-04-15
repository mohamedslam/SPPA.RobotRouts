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
    /// Class describes Group Header Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiGroupFooterBand), "Stimulsoft.Report.Images.Components.StiGroupFooterBand.png")]
    [StiToolbox(true)]
    [StiV1Builder(typeof(StiGroupFooterBandV1Builder))]
    [StiV2Builder(typeof(StiGroupFooterBandV2Builder))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiGroupFooterBand :
        StiDynamicBand,
        IStiKeepGroupFooterTogether
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiGroupFooterBand
            jObject.AddPropertyBool("KeepGroupFooterTogether", KeepGroupFooterTogether, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "KeepGroupFooterTogether":
                        this.keepGroupFooterTogether = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiGroupFooterBand;

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
                    propHelper.KeepGroupFooterTogether(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.PrintAtBottom(),
                    propHelper.PrintOn(),
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
        public override string HelpUrl => "user-manual/report_internals_groups_groupfooterband.htm";
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties, bool cloneComponents)
        {
            var band = base.Clone(cloneProperties, cloneComponents) as StiGroupFooterBand;

            band.groupFooterBandInfoV1 = this.GroupFooterBandInfoV1.Clone() as StiGroupFooterBandInfoV1;
            band.groupFooterBandInfoV2 = this.GroupFooterBandInfoV2.Clone() as StiGroupFooterBandInfoV2;

            return band;
        }
        #endregion

        #region IStiBreakable override EngineV2 only
        /// <summary>
        /// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
        /// </summary>		
        [StiEngine(StiEngineVersion.EngineV2)]
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
            }
        }
        #endregion

        #region IStiKeepGroupFooterTogether
        private bool keepGroupFooterTogether = true;
        /// <summary>
        /// Gets or sets value indicates that the group footer is printed with data.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the group footer is printed with data.")]
        [StiShowInContextMenu]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepGroupFooterTogether
        {
            get
            {
                return keepGroupFooterTogether;
            }
            set
            {
                if (keepGroupFooterTogether != value)
                {
                    CheckBlockedException("KeepGroupFooterTogether");
                    keepGroupFooterTogether = value;
                }
            }
        }
        #endregion

        #region Render
        private StiGroupFooterBandInfoV1 groupFooterBandInfoV1;
        [Browsable(false)]
        public StiGroupFooterBandInfoV1 GroupFooterBandInfoV1
        {
            get
            {
                return groupFooterBandInfoV1 ?? (groupFooterBandInfoV1 = new StiGroupFooterBandInfoV1());
            }
        }

        private StiGroupFooterBandInfoV2 groupFooterBandInfoV2;
        [Browsable(false)]
        public StiGroupFooterBandInfoV2 GroupFooterBandInfoV2
        {
            get
            {
                return groupFooterBandInfoV2 ?? (groupFooterBandInfoV2 = new StiGroupFooterBandInfoV2());
            }
        }

        [Browsable(false)]
        [StiEngine(StiEngineVersion.EngineV2)]
        public int Line => this.GroupFooterBandInfoV2.GroupHeader != null
            ? this.GroupFooterBandInfoV2.GroupHeader.Line
            : 1;
        #endregion

        #region StiBand override
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

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.GroupFooterBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
		/// Gets the type of processing when printing.
		/// </summary>
		public override StiComponentType ComponentType => StiComponentType.Detail;

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiGroupFooterBand");

        /// <summary>
		/// Gets a component priority.
		/// </summary>
		public override int Priority => (int)StiComponentPriority.GroupFooterBand;
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiGroupFooterBand();
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiGroupFooterBand.
        /// </summary>
        public StiGroupFooterBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiGroupFooterBand with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiGroupFooterBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;
        }
    }
}