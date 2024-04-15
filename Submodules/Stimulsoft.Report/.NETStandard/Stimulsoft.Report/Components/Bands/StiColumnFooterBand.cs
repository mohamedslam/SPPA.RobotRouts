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

using System.Drawing;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Json.Linq;
using System;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Desribes the class that realizes the band - Column Footer Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiColumnFooterBand), "Stimulsoft.Report.Images.Components.StiColumnFooterBand.png")]
    [StiV2Builder(typeof(StiColumnFooterBandV2Builder))]
    [StiToolbox(true)]
    [StiContextTool(typeof(IStiPrintIfEmpty))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiPrintOnAllPages))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiColumnFooterBand : StiFooterBand
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiFooterBand
            jObject.AddPropertyBool("KeepFooterTogether", KeepFooterTogether, true);
            jObject.AddPropertyBool("StartNewPage", StartNewPage);
            jObject.AddPropertyFloat("StartNewPageIfLessThan", StartNewPageIfLessThan, 100f);
            jObject.AddPropertyBool("PrintIfEmpty", PrintIfEmpty);
            jObject.AddPropertyEnum("PrintOnEvenOddPages", PrintOnEvenOddPages, StiPrintOnEvenOddPagesType.Ignore);
            jObject.AddPropertyBool("PrintOnAllPages", PrintOnAllPages);

            return jObject;
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiColumnFooterBand;

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

        #region IStiPrintAtBottom EngineV2 only
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public override bool PrintAtBottom
        {
            get
            {
                return base.PrintAtBottom;
            }
            set
            {
                base.PrintAtBottom = value;
            }
        }
        #endregion

        #region StiBand override
        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(239, 109, 73);

        /// <summary>
        /// Gets header end color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(239, 109, 73);
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/report_internals_columns_columns_on_data_band_columnfooter_band.htm";
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.ColumnFooterBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.ColumnFooterBand;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiColumnFooterBand");
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiColumnFooterBand();
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiColumnFooterBand.
        /// </summary>
        public StiColumnFooterBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiColumnFooterBand with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes sizes and position of the component.</param>
        public StiColumnFooterBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}