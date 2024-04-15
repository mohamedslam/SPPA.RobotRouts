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
    /// Describes the class that realizes the band - Report Title Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiReportTitleBand), "Stimulsoft.Report.Images.Components.StiReportTitleBand.png")]
    [StiToolbox(true)]
    [StiV1Builder(typeof(StiReportTitleBandV1Builder))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiPrintIfEmpty))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiReportTitleBand :
        StiStaticBand,
        IStiPrintIfEmpty
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("PrintOn");
            jObject.RemoveProperty("CanBreak");

            // StiReportTitleBand
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
                    case "PrintIfEmpty":
                        this.PrintIfEmpty = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiReportTitleBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

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
        public override string HelpUrl => "user-manual/report_internals_report_bands_reporttitleband.htm";
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

        #region IStiPrintIfEmpty
        /// <summary>
        /// Gets or sets value indicates that the report title band is printed if data is not present.
        /// </summary>
        [DefaultValue(true)]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintIfEmpty)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the report title band is printed if data is not present.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintIfEmpty { get; set; } = true;
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
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV2)
                {
                    if (Page.TitleBeforeHeader)
                        return StiComponentType.Static;
                    else
                        return StiComponentType.Master;
                }
                return StiComponentType.Simple;
            }
        }

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.ReportTitleBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority
        {
            get
            {
                if (Page.TitleBeforeHeader)
                    return (int)StiComponentPriority.ReportTitleBandBefore;

                if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV1)
                    return (int)StiComponentPriority.ReportTitleBandAfterV1;

                else
                    return (int)StiComponentPriority.ReportTitleBandAfterV2;
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
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiReportTitleBand");
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiReportTitleBand();
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
        /// Creates a new component of the type StiReportTitleBand.
        /// </summary>
        public StiReportTitleBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type  StiReportTitleBand.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiReportTitleBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}