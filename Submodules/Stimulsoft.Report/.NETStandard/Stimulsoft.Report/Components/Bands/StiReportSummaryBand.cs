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
using Stimulsoft.Report.Dialogs;
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
    /// Describes the class that realizes the band - Report Summary.
    /// </summary>
    [StiServiceBitmap(typeof(StiReportSummaryBand), "Stimulsoft.Report.Images.Components.StiReportSummaryBand.png")]
    [StiToolbox(true)]
    [StiV1Builder(typeof(StiReportSummaryBandV1Builder))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiPrintIfEmpty))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiReportSummaryBand :
        StiDynamicBand,
        IStiPrintIfEmpty,
        IStiKeepReportSummaryTogether
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("PrintOn");

            // StiReportSummaryBand
            jObject.AddPropertyBool("KeepReportSummaryTogether", KeepReportSummaryTogether, true);
            jObject.AddPropertyBool("PrintIfEmpty", PrintIfEmpty, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "KeepReportSummaryTogether":
                        this.keepReportSummaryTogether = property.DeserializeBool();
                        break;

                    case "PrintIfEmpty":
                        this.PrintIfEmpty = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiReportSummaryBand;

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
                    propHelper.KeepReportSummaryTogether(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.PrintAtBottom(),
                    propHelper.PrintIfEmpty(),
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
        public override string HelpUrl => "user-manual/report_internals_report_bands_reportsummaryband.htm";
        #endregion

        #region IStiPrintOn override
        [StiNonSerialized]
        [Browsable(false)]
        public override StiPrintOnType PrintOn
        {
            get
            {
                return base.PrintOn;
            }
            set
            {
                base.PrintOn = value;
            }
        }
        #endregion

        #region IStiKeepReportSummaryTogether
        private bool keepReportSummaryTogether = true;
        /// <summary>
        /// Gets or sets value indicates that the report summary is printed with data.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the report summary is printed with data.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepReportSummaryTogether
        {
            get
            {
                return keepReportSummaryTogether;
            }
            set
            {
                if (keepReportSummaryTogether != value)
                {
                    CheckBlockedException("KeepReportSummaryTogether");
                    keepReportSummaryTogether = value;
                }
            }
        }
        #endregion

        #region IStiPrintIfEmpty
        /// <summary>
        /// Gets or sets value indicates that the footer is printed at bottom of page.
        /// </summary>
        [DefaultValue(true)]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintIfEmpty)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the footer is printed at bottom of page.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintIfEmpty { get; set; } = true;
        #endregion

        #region StiBand override
        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(159, 213, 183);

        /// <summary>
        /// Gets header end color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(159, 213, 183);
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.ReportSummaryBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
        /// May this container be located in the specified component.
        /// </summary>
        /// <param name="component">Component for checking.</param>
        /// <returns>true, if this container may is located in the specified component.</returns>
        public override bool CanContainIn(StiComponent component)
        {
            if (component is IStiReportControl) return false;
            if (component is StiPage) return true;
            return false;
        }

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiReportSummaryBand");

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.ReportSummaryBand;

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV2)
                    return base.ComponentType;

                return StiComponentType.Master;
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiReportSummaryBand();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the component Master of the object.
        /// </summary>
        /// <returns>Master component.</returns>
        public StiComponent GetMaster()
        {
            foreach (StiComponent component in Parent.Components)
            {
                if (component is StiDataBand) return component;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiComponent.
        /// </summary>
        public StiReportSummaryBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiComponent with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiReportSummaryBand(RectangleD rect) : base(rect)
        {
            DockStyle = StiDockStyle.Top;
            PlaceOnToolbox = false;
        }
    }
}