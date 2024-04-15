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
    /// Desribes the class that realizes the band - Child Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiChildBand), "Stimulsoft.Report.Images.Components.StiChildBand.png")]
	[StiToolbox(true)]
	[StiContextTool(typeof(IStiCanGrow))]
	[StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiKeepChildTogether))]
	[StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiChildBand : 
        StiDynamicBand,
        IStiKeepChildTogether
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiChildBand
            jObject.AddPropertyBool("KeepChildTogether", KeepChildTogether, true);
            jObject.AddPropertyBool("PrintIfParentDisabled", PrintIfParentDisabled);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "KeepChildTogether":
                        this.keepChildTogether = property.DeserializeBool();
                        break;

                    case "PrintIfParentDisabled":
                        this.printIfParentDisabled = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiChildBand;

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
                    propHelper.KeepChildTogether(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.PrintAtBottom(),
                    propHelper.PrintIfParentDisabled(),
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
                        StiPropertyEventId.MouseLeaveEvent,
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

        #region IStiKeepChildTogether
        private bool keepChildTogether = true;
        /// <summary>
        /// Gets or sets value indicates that childs are to be kept together.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that childs are to be kept together.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool KeepChildTogether
        {
            get
            {
                return keepChildTogether;
            }
            set
            {
                if (keepChildTogether != value)
                {
                    CheckBlockedException("KeepChildTogether");
                    keepChildTogether = value;
                }
            }
        }
        #endregion

        #region Pointer EngineV2 Only
        [StiEngine(StiEngineVersion.EngineV2)]
        public override StiPointerExpression Pointer
        {
            get
            {
                return base.Pointer;
            }
            set
            {
                base.Pointer = value;
            }
        }
        #endregion

        #region Bookmark EngineV2 Only
        [StiEngine(StiEngineVersion.EngineV2)]
		public override StiBookmarkExpression Bookmark
		{
			get
			{
				return base.Bookmark;
			}
			set
			{
				base.Bookmark = value;
			}
		}
		#endregion

		#region Hyperlink EngineV2 Only
		[StiEngine(StiEngineVersion.EngineV2)]
		public override StiHyperlinkExpression Hyperlink
		{
			get
			{
				return base.Hyperlink;
			}
			set
			{
				base.Hyperlink = value;
			}
		}
		#endregion

        #region StiBand override
        /// <summary>
		/// Gets header start color.
		/// </summary>
		[Browsable(false)]
		public override Color HeaderStartColor
		{
			get
			{
                var band = GetMaster();
                return band != null 
                    ? StiColorUtils.Light(band.HeaderStartColor, 50) 
                    : Color.White;
			}
		}

		/// <summary>
		/// Gets header end color.
		/// </summary>
		[Browsable(false)]
		public override Color HeaderEndColor
		{
			get
			{
                var band = GetMaster();
                return band != null 
                    ? StiColorUtils.Light(band.HeaderEndColor, 50) 
                    : Color.White;
			}
		}
		#endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/report_internals_child_band.htm";
        #endregion

		#region StiComponent override
		/// <summary>
		/// Gets the type of processing when printing.
		/// </summary>
		public override StiComponentType ComponentType => StiComponentType.Detail;

        /// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.ChildBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
		/// Gets a component priority.
		/// </summary>
		public override int Priority => (int)StiComponentPriority.ChildBand;

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiChildBand");
        #endregion

        #region PrintIfParentDisabled
        private bool printIfParentDisabled;
        /// <summary>
        /// Gets or sets value which indicates that if the parent band is disabled then the child band will be printed anyway.
        /// </summary>
        [DefaultValue(false)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintIfParentDisabled)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that if the parent band is disabled then the child band will be printed anyway.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool PrintIfParentDisabled
        {
            get
            {
                return printIfParentDisabled;
            }
            set
            {
                if (printIfParentDisabled != value)
                {
                    CheckBlockedException("PrintIfParentDisabled");
                    printIfParentDisabled = value;
                }
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiChildBand();
        }
        #endregion

        #region Methods
        /// <summary>
		/// Returns the component Master of the object.
		/// </summary>
		/// <returns>Master component.</returns>
		public StiBand GetMaster()
		{
			var index = Parent.Components.IndexOf(this) - 1;
			
			while (index >= 0)
			{
				if (Parent.Components[index] is StiBand && !(Parent.Components[index] is StiChildBand))
					return Parent.Components[index] as StiBand;
				index--;
			}
			return null;
		}
        #endregion

        /// <summary>
        /// Creates a new component of the type StiChildBand.
        /// </summary>
        public StiChildBand() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new component of the type StiChildBand with specified location.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the component.</param>
		public StiChildBand(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = false;
		}
	}
}