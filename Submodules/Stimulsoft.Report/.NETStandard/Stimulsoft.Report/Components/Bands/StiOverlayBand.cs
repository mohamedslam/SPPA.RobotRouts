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
    /// Desribes the class that realizes the band - Overlay Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiOverlayBand), "Stimulsoft.Report.Images.Components.StiOverlayBand.png")]
    [StiToolbox(true)]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiOverlayBand :
        StiStaticBand,
        IStiVertAlignment
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanBreak");
            jObject.RemoveProperty("Bookmark");
            jObject.RemoveProperty("Pointer");
            jObject.RemoveProperty("Hyperlink");

            // StiOverlayBand
            jObject.AddPropertyEnum("VertAlignment", VertAlignment, StiVertAlignment.Center);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "VertAlignment":
                        this.VertAlignment = property.DeserializeEnum<StiVertAlignment>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiOverlayBand;

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
                    propHelper.PrintOn(),
                    propHelper.ResetPageNumber(),
                    propHelper.VertAlignment()
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
        public override string HelpUrl => "user-manual/report_internals_watermarks_overlay_band.htm";
        #endregion

        #region IStiVertAlignment
        /// <summary>
        /// Gets or sets the vertical alignment of an object.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiVertAlignment.Center)]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorVertAlignment)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets the vertical alignment of an object.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiVertAlignment VertAlignment { get; set; } = StiVertAlignment.Center;
        #endregion

        #region StiBand override
        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(131, 124, 174);

        /// <summary>
        /// Gets header end color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(131, 124, 174);
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                if (Report != null && Report.EngineVersion != Engine.StiEngineVersion.EngineV1)
                    return StiComponentType.Static;

                else
                    return StiComponentType.Detail;
            }
        }

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.OverlayBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.OverlayBand;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiOverlayBand");
        #endregion

        #region IStiBreakable Browsable(false)
        /// <summary>
        /// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
        /// </summary>		
        [StiNonSerialized]
        [Browsable(false)]
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

        #region Pointer Browsable(false)
        [DefaultValue(false)]
        [StiNonSerialized]
        [Browsable(false)]
        public override StiPointerExpression Pointer
        {
            get
            {
                return base.Pointer;
            }
            set
            {
            }
        }
        #endregion

        #region Bookmark Browsable(false)
        [DefaultValue(false)]
        [StiNonSerialized]
        [Browsable(false)]
        public override StiBookmarkExpression Bookmark
        {
            get
            {
                return base.Bookmark;
            }
            set
            {
            }
        }
        #endregion

        #region Hyperlink Browsable(false)
        [DefaultValue(false)]
        [StiNonSerialized]
        [Browsable(false)]
        public override StiHyperlinkExpression Hyperlink
        {
            get
            {
                return base.Hyperlink;
            }
            set
            {
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiOverlayBand();
        }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiOverlayBand.
        /// </summary>
        public StiOverlayBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiOverlayBand with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes sizes and position of the component.</param>
        public StiOverlayBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}