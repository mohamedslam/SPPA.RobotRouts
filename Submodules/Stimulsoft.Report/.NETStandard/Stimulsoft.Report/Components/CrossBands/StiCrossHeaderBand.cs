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
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// The class describes the Cross Header Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiCrossHeaderBand), "Stimulsoft.Report.Images.Components.StiCrossHeaderBand.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.catCrossBands.png")]
    [StiGdiPainter(typeof(StiCrossBandGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiCrossBandWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiToolbox(true)]
	[StiContextTool(typeof(IStiPrintIfEmpty))]
	[StiContextTool(typeof(IStiPrintOnAllPages))]
	[StiContextTool(typeof(IStiCanGrow))]
	[StiContextTool(typeof(IStiCanShrink))]
	[StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
	public class StiCrossHeaderBand : StiHeaderBand
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("NewPageBefore");
            jObject.RemoveProperty("NewPageAfter");
            jObject.RemoveProperty("NewColumnBefore");
            jObject.RemoveProperty("NewColumnAfter");
            jObject.RemoveProperty("SkipFirst");
            jObject.RemoveProperty("BreakIfLessThan");
            jObject.RemoveProperty("GrowToHeight");
            jObject.RemoveProperty("ResetPageNumber");
            jObject.RemoveProperty("StartNewPage");
            jObject.RemoveProperty("StartNewPageIfLessThan");
            jObject.RemoveProperty("PrintAtBottom");
            jObject.RemoveProperty("PrintOnAllPages");
            jObject.RemoveProperty("PrintIfEmpty");
            jObject.RemoveProperty("PrintOnEvenOddPages");
            jObject.RemoveProperty("PrintOn");
            jObject.RemoveProperty("DockStyle");
            jObject.RemoveProperty("MinSize");
            jObject.RemoveProperty("MaxSize");
            jObject.RemoveProperty("MaxHeight");
            jObject.RemoveProperty("MinHeight");

            // StiCrossHeaderBand
            jObject.AddPropertyDouble("MinWidth", MinWidth, 0d);
            jObject.AddPropertyDouble("MaxWidth", MaxWidth, 0d);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "MinWidth":
                        this.MinWidth = property.DeserializeDouble();
                        break;

                    case "MaxWidth":
                        this.MaxWidth = property.DeserializeDouble();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCrossHeaderBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Width()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Width(),
                    propHelper.MinWidth(),
                    propHelper.MaxWidth()
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
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.KeepHeaderTogether(),
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
                    propHelper.KeepHeaderTogether(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled()
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
        public override string HelpUrl => "user-manual/report_internals_bands_band_types_cross_bands.htm";
        #endregion

        #region IStiPageBreak Browsable(false)
        [Browsable(false)]
        [StiNonSerialized]
        public override bool NewPageBefore
        {
            get
            {
                return base.NewPageBefore;
            }
            set
            {
                base.NewPageBefore = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override bool NewPageAfter
        {
            get
            {
                return base.NewPageAfter;
            }
            set
            {
                base.NewPageAfter = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override bool NewColumnBefore
        {
            get
            {
                return base.NewColumnBefore;
            }
            set
            {
                base.NewColumnBefore = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override bool NewColumnAfter
        {
            get
            {
                return base.NewColumnAfter;
            }
            set
            {
                base.NewColumnAfter = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override bool SkipFirst
        {
            get
            {
                return base.SkipFirst;
            }
            set
            {
                base.SkipFirst = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override float BreakIfLessThan
        {
            get
            {
                return base.BreakIfLessThan;
            }
            set
            {
                base.BreakIfLessThan = value;
            }
        }
        #endregion

        #region GrowToHeight Browsable(false)
        [Browsable(false)]
        [StiNonSerialized]
        public override bool GrowToHeight
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        #endregion

		#region IStiResetPageNumber Browsable(false)
		[Browsable(false)]
		[StiNonSerialized]
		public override bool ResetPageNumber
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		#endregion

		#region IStiStartNewPage Browsable(false)
		[DefaultValue(false)]
		[StiNonSerialized]
		[Browsable(false)]
		public override bool StartNewPage
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[DefaultValue(false)]
		[StiNonSerialized]
		[Browsable(false)]
		public override float StartNewPageIfLessThan
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		#endregion

		#region IStiPrintAtBottom Browsable(false)
		[DefaultValue(false)]
		[StiNonSerialized]
		[Browsable(false)]
		public override bool PrintAtBottom
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		#endregion

		#region IStiBreakable Browsable
        /// <summary>
        /// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorCanBreak)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates whether the component can or cannot break its contents on several pages.")]
        [StiPropertyLevel(StiLevel.Professional)]
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

		#region IStiPrintOnAllPages Browsable(false)
		[DefaultValue(false)]
		[StiNonSerialized]
		[Browsable(false)]
		public override bool PrintOnAllPages
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
		#endregion	

		#region IStiPrintIfEmpty Browsable(false)
		[DefaultValue(false)]
		[StiNonSerialized]
		[Browsable(false)]
		public override bool PrintIfEmpty
		{
			get
			{
				return base.PrintIfEmpty;
			}
			set
			{
			}
		}
		#endregion

		#region IStiPrintOnEvenOddPages Browsable(false)
		[DefaultValue(false)]
		[StiNonSerialized]
		[Browsable(false)]
		public override StiPrintOnEvenOddPagesType PrintOnEvenOddPages
		{
			get
			{
				return base.PrintOnEvenOddPages;
			}
			set
			{
			}
		}
		#endregion

		#region IStiPrintOn Browsable(false)
		[DefaultValue(false)]
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
			}
		}
		#endregion

		#region StiComponent override
        /// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.CrossHeaderBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Cross;

        /// <summary>
		/// Gets a component priority.
		/// </summary>
		public override int Priority => (int)StiComponentPriority.CrossHeaderBand;

        /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "CrossBands");

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiCrossHeaderBand");

        /// <summary>
		/// Gets value, indicates that this cross is the component.
		/// </summary>
		public override bool IsCross => true;
        #endregion

		#region Dock override
		/// <summary>
		/// Gets or sets a type of the component docking.
		/// </summary>
		[StiNonSerialized]
		[Browsable(false)]
		public override StiDockStyle DockStyle
		{
			get 
			{
				return StiDockStyle.Left;
			}
			set 
			{
			}
		}
		#endregion

		#region Position override
        [Browsable(false)]
        [StiNonSerialized]
        public override SizeD MinSize
        {
            get
            {
                return base.MinSize;
            }
            set
            {
                base.MinSize = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override SizeD MaxSize
        {
            get
            {
                return base.MaxSize;
            }
            set
            {
                base.MaxSize = value;
            }
        }

        /// <summary>
        /// Gets or sets minimal width of band.
        /// </summary>
        [StiCategory("Position")]
        [StiOrder(500)]
        [Description("Gets or sets minimal width of band.")]
        [DefaultValue(0d)]
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual double MinWidth
        {
            get
            {
                return base.MinSize.Width;
            }
            set
            {
                if (base.MinSize.Width != value)
                    base.MinSize = new SizeD(value, 0);
            }
        }

        /// <summary>
        /// Gets or sets maximal width of band.
        /// </summary>
        [StiCategory("Position")]
        [StiOrder(510)]
        [Description("Gets or sets maximal width of band.")]
        [DefaultValue(0d)]
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual double MaxWidth
        {
            get
            {
                return base.MaxSize.Width;
            }
            set
            {
                if (base.MaxSize.Width != value)
                    base.MaxSize = new SizeD(value, 0);
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override double MaxHeight
        {
            get
            {
                return base.MaxHeight;
            }
            set
            {
                base.MaxHeight = value;
            }
        }

        [Browsable(false)]
        [StiNonSerialized]
        public override double MinHeight
        {
            get
            {
                return base.MinHeight;
            }
            set
            {
                base.MinHeight = value;
            }
        }

		[Browsable(true)]
        [StiPropertyLevel(StiLevel.Basic)]
		public override double Width
		{
			get 
			{
				return base.Width;
			}
			set 
			{
				base.Width = value;
			}
		}

		[Browsable(false)]
		public override double Height
		{
			get 
			{
				return base.Height;
			}
			set 
			{
				base.Height = value;
			}
		}

		/// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 30, 50);

        /// <summary>
		/// Gets or sets a rectangle of the component selection.
		/// </summary>
		public override RectangleD SelectRectangle
		{
			get
			{
				var headerSize = Page.Unit.ConvertFromHInches(this.HeaderSize);
				return new RectangleD(Left, Top, Width, Height + headerSize);
			}
			set
			{
				var headerSize = Page.Unit.ConvertFromHInches(this.HeaderSize);
				Left = value.Left;
				Top = value.Top;
				Width = value.Width;
				Height = value.Height - headerSize;
			}
		}
		
		/// <summary>
		/// Gets or sets a rectangle of the component which it fills. Docking occurs in accordance to the area
		/// (Cross - components are docked by ClientRectangle).
		/// </summary>
		[Browsable(false)]
		public override RectangleD DisplayRectangle
		{
			get
			{
				return SelectRectangle;
			}
			set
			{
				SelectRectangle = value;
			}
		}
		#endregion

		#region StiBand override
		/// <summary>
		/// Gets the header height.
		/// </summary>
		public override double HeaderSize
		{
			get
			{
				if (Report.Info.ShowHeaders)
				{
					return StiAlignValue.AlignToMaxGrid(15, 
						Page.Unit.ConvertToHInches(Page.GridSize), true) - 4;
				}
				else return 0;
			}
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossHeaderBand();
        }
        #endregion

		/// <summary>
		/// Creates a new component of the type StiCrossHeaderBand.
		/// </summary>
		public StiCrossHeaderBand() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a component of the type StiCrossHeaderBand with specified location.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiCrossHeaderBand(RectangleD rect): base(rect)
		{
			PlaceOnToolbox = false;
		}
	}
}