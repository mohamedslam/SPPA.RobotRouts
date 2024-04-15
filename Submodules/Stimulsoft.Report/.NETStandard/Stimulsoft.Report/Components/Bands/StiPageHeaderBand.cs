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
    /// Describes the class that realizes the band - Page Header Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiPageHeaderBand), "Stimulsoft.Report.Images.Components.StiPageHeaderBand.png")]
    [StiToolbox(true)]
    [StiV1Builder(typeof(StiPageHeaderBandV1Builder))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiPageHeaderBand :
        StiStaticBand,
        IStiPrintOnEvenOddPages
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanBreak");

            // StiPageHeaderBand
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
                    case "PrintOnEvenOddPages":
                        this.PrintOnEvenOddPages = property.DeserializeEnum<StiPrintOnEvenOddPagesType>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiPageHeaderBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            // PositionCategory
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
                    propHelper.Enabled()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.Enabled(),
                    propHelper.PrintOn(),
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
        public override string HelpUrl => "user-manual/report_internals_page_bands_pageheader_band.htm";
        #endregion

        #region IStiBreakable override
        [StiNonSerialized]
        [Browsable(false)]
        public override bool CanBreak
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        /// <summary>
        /// Divides content of components in two parts. Returns result of dividing. If true, then component is successful divided.
        /// </summary>
        /// <param name="dividedComponent">Component for store part of content.</param>
        /// <returns>If true, then component is successful divided.</returns>
        public override bool Break(StiComponent dividedComponent, double devideFactor, ref double divLine)
        {
            divLine = 0;

            var newCont = StiComponentDivider.BreakContainer(this.Height, this);
            ((StiContainer)dividedComponent).Components.Clear();
            ((StiContainer)dividedComponent).Components.AddRange(newCont.Components);

            return true;
        }
        #endregion

        #region StiBand override
        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(206, 207, 206);

        /// <summary>
		/// Gets header end color.
		/// </summary>
		[Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(206, 207, 206);
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.PageHeaderBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
		/// Gets a component priority.
		/// </summary>
		public override int Priority
        {
            get
            {
                if (Page.TitleBeforeHeader)
                    return (int)StiComponentPriority.PageHeaderBandBefore;

                else
                    return (int)StiComponentPriority.PageHeaderBandAfter;
            }
        }

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
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV2)
                    return base.ComponentType;
                return StiComponentType.Simple;
            }
        }

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        [Browsable(false)]
        public override string LocalizedName => StiLocalization.Get("Components", "StiPageHeaderBand");
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

        #region IStiPrintOnFirstPage Obsolete
        [DefaultValue(true)]
        [StiNonSerialized]
        [Browsable(false)]
        public bool PrintOnFirstPage
        {
            get
            {
                return (PrintOn & StiPrintOnType.ExceptFirstPage) == 0;
            }
            set
            {
                if (value)
                {
                    if ((PrintOn & StiPrintOnType.ExceptFirstPage) > 0)
                        PrintOn -= StiPrintOnType.ExceptFirstPage;
                }
                else
                    PrintOn |= StiPrintOnType.ExceptFirstPage;
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiPageHeaderBand();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns Master component of this component.
        /// </summary>
        public StiComponent GetMaster()
        {
            foreach (StiComponent component in Parent.Components)
            {
                if (component is StiDataBand && (!(component is StiEmptyBand)))
                    return component;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiPageHeaderBand.
        /// </summary>
        public StiPageHeaderBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiPageHeaderBand with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiPageHeaderBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = true;
        }
    }
}