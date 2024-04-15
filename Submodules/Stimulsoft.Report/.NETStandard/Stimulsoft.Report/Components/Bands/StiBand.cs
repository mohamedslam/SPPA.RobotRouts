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
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Painters;
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
    /// Describes the base class for bands.
    /// </summary>
    [StiToolbox(false)]
	[StiDesigner("Stimulsoft.Report.Components.Design.StiBandDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfBandDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiBandGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiBandWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiV1Builder(typeof(StiBandV1Builder))]
    [StiV2Builder(typeof(StiBandV2Builder))]
	[StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.catBands.png")]
	[StiEngine(StiEngineVersion.All)]
	public abstract class StiBand : 
		StiContainer,
		IStiResetPageNumber
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyBool("CanGrow", CanGrow, true);
            jObject.RemoveProperty("GrowToHeight");
            jObject.RemoveProperty("ShiftMode");
            jObject.RemoveProperty("Printable");
            jObject.RemoveProperty("DockStyle");
            jObject.RemoveProperty("MinSize");
            jObject.RemoveProperty("MaxSize");

            // StiBand
            jObject.AddPropertyBool("ResetPageNumber", ResetPageNumber);
            jObject.AddPropertyDouble("MinHeight", MinHeight, 0.0);
            jObject.AddPropertyDouble("MaxHeight", MaxHeight, 0.0);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ResetPageNumber":
                        this.ResetPageNumber = property.DeserializeBool();
                        break;

                    case "MinHeight":
                        this.MinHeight = property.DeserializeDouble();
                        break;

                    case "MaxHeight":
                        this.MaxHeight = property.DeserializeDouble();
                        break;
                }
            }
        }
        #endregion

        #region IStiResetPageNumber
	    /// <summary>
		/// Allows to reset page number on this band.
		/// </summary>
		[DefaultValue(false)]
		[StiSerializable]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorResetPageNumber)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Allows to reset page number on this band.")]
		[StiEngine(StiEngineVersion.EngineV2)]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool ResetPageNumber { get; set; }
	    #endregion

		#region IStiCanGrow override
		/// <summary>
		/// Gets or sets value indicates that this object can grow.
		/// </summary>
		[DefaultValue(true)]
		[StiCategory("Behavior")]
		[StiOrder(StiPropertyOrder.BehaviorCanGrow)]
		[Description("Gets or sets value indicates that this object can grow.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
		public override bool CanGrow
		{
			get
			{
				return base.CanGrow;
			}
			set
			{
                base.CanGrow = value; 
			}
		}
		#endregion		

		#region StiComponent override
        [Browsable(false)]
		[StiNonSerialized]
        public override StiAnchorMode Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }
        
        public override string GetQuickInfo()
		{
			switch (Report.Info.QuickInfoType)
			{
				case StiQuickInfoType.ShowComponentsNames:
					return string.Empty;

				default:
					return base.GetQuickInfo();
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public override bool GrowToHeight
		{
			get 
			{
				return base.GrowToHeight;
			}
			set 
			{
			}
		}
        
		[StiNonSerialized]
		[Browsable(false)]
		public override StiShiftMode ShiftMode
		{
			get
			{
			    return StiOptions.Engine.UseCheckSizeForContinuedContainers 
			        ? base.ShiftMode 
			        : StiShiftMode.None;
			}
			set 
			{
			}
		}
	
		/// <summary>
		/// May this container be located in the specified component.
		/// </summary>
		/// <param name="component">Component for checking.</param>
		/// <returns>true, if this container may is located in the specified component.</returns>
		public override bool CanContainIn(StiComponent component)
		{
			if (component is IStiReportControl) 
                return false;

            if (this.IsCross && component.IsCross) 
                return false;

            if (this.IsCross && component is StiBand) 
                return true;

            if (this is Table.StiTable && component is StiBand) 
                return true;

            if (component is StiBand) 
                return false;

			return base.CanContainIn(component);
		}

		/// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Bands");

	    /// <summary>
		/// Gets the type of processing when printing.
		/// </summary>
		public override StiComponentType ComponentType => StiComponentType.Master;
	    #endregion		

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone(bool cloneProperties, bool cloneComponents)
		{
			var band = (StiBand)base.Clone(cloneProperties, cloneComponents);

			band.bandInfoV2 = this.BandInfoV2.Clone() as StiBandInfoV2;

			return band;
		}
		#endregion

		#region Render override
		private StiBandInfoV2 bandInfoV2;
		[Browsable(false)]
		public StiBandInfoV2 BandInfoV2
		{
			get
			{
			    return bandInfoV2 ?? (bandInfoV2 = new StiBandInfoV2());
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public override bool Printable
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

		#region Dock override
		/// <summary>
		/// Gets or sets a type of the component docking.
		/// </summary>
		[Browsable(false)]
		[StiNonSerialized]
		public override StiDockStyle DockStyle
		{
			get
			{
			    return StiOptions.Engine.DockPageFooterToBottom && this is StiPageFooterBand
			        ? StiDockStyle.Bottom
			        : StiDockStyle.Top;
			}
			set
			{
			}
		}
		
		/// <summary>
		/// Gets value indicates that this is an automatic docking.
		/// </summary>
		public override bool IsAutomaticDock => true;
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
        /// Gets or sets minimal height of band.
        /// </summary>
        [StiCategory("Position")]
        [StiOrder(500)]
        [Description("Gets or sets minimal height of band.")]
        [DefaultValue(0d)]
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual double MinHeight
        {
            get
            {
                return base.MinSize.Height;
            }
            set
            {
                if (base.MinSize.Height != value)
                    base.MinSize = new SizeD(0, value);
            }
        }

        /// <summary>
        /// Gets or sets maximal height of band.
        /// </summary>
        [StiCategory("Position")]
        [StiOrder(500)]
        [Description("Gets or sets maximal height of band.")]
        [DefaultValue(0d)]
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual double MaxHeight
        {
            get
            {
                return base.MaxSize.Height;
            }
            set
            {
                if (base.MaxSize.Height != value)
                    base.MaxSize = new SizeD(0, value);
            }
        }

		[Browsable(false)]
		public override double Left
		{
			get 
			{
				return base.Left;
			}
			set 
			{
				base.Left = value;
			}
		}

		[Browsable(false)]
		public override double Top
		{
			get 
			{
				return base.Top;
			}
			set 
			{
				base.Top = value;
			}
		}

		[Browsable(false)]
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

		/// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 300, 30);

	    /// <summary>
		/// Gets or sets a rectangle of the component which it fills. Docking occurs in accordance to the area
		/// (Cross - components are docked by ClientRectangle).
		/// </summary>
		public override RectangleD DisplayRectangle
		{
			get
			{
			    if (Report != null && Report.IsRendering)
			        return new RectangleD(Left, Top, Width, Height);

			    var headerSize = 0d;
                var footerSize = 0d;

				if (Page != null && (Report == null || Report.Info.ShowHeaders))
				{
                    headerSize = Page.Unit.ConvertFromHInches(this.HeaderSize);
                    footerSize = Page.Unit.ConvertFromHInches(this.FooterSize);
				}

				return new RectangleD(Left, Top - headerSize, Width, Height + headerSize + footerSize);
			}
			set
			{
                if (Report != null && Report.IsRendering)
                {
                    Left = value.Left;
                    Top = value.Top;
                    Width = value.Width;
                    Height = value.Height;
                }
                else
                {
                    var headerSize = Page.Unit.ConvertFromHInches(this.HeaderSize);
                    var footerSize = Page.Unit.ConvertFromHInches(this.FooterSize);

                    if (Report != null && !Report.Info.ShowHeaders)
                    {
                        headerSize = 0;
                        footerSize = 0;
                    }

                    Left = value.Left;
                    Top = value.Top + headerSize;
                    Width = value.Width;
                    Height = value.Height - headerSize - footerSize;
                }
			}
		}

        protected internal override void SetDirectDisplayRectangle(RectangleD rect)
        {
            this.DisplayRectangle = rect;
        }

		/// <summary>
		/// Gets or sets a rectangle of the component selection.
		/// </summary>
		public override RectangleD SelectRectangle
		{
			get
			{
				var headerSize = Page.Unit.ConvertFromHInches(this.HeaderSize);

				if (DockStyle == StiDockStyle.Left || DockStyle == StiDockStyle.Right)
					return new RectangleD(Left - headerSize, Top, Width + headerSize, Height);

				else
				    return new RectangleD(Left, Top - headerSize, Width, Height + headerSize);
			}
			set
			{
				var headerSize = Page.Unit.ConvertFromHInches(this.HeaderSize);

				if (DockStyle == StiDockStyle.Left || DockStyle == StiDockStyle.Right)
				{
					Left = value.Left + headerSize;
					Top = value.Top;
					Width = value.Width - headerSize;
					Height = value.Height;
				}
				else
				{
					Left = value.Left;
					Top = value.Top + headerSize;
					Width = value.Width;
					Height = value.Height - headerSize;
				}
			}
		}
		#endregion

        #region Properties
        /// <summary>
        /// Gets nested level of this band.
        /// </summary>
        internal int NestedLevel
        {
            get
            {
                #region StiPageHeaderBand
                var pageHeaderBand = this as StiPageHeaderBand;
                if (pageHeaderBand != null)
                {
                    var level = 1;
                    foreach (StiComponent component in this.Page.Components)
                    {
                        if (component == pageHeaderBand)
                            return level;

                        if (component is StiPageHeaderBand)
                            level++;
                    }
                    return level;
                }
                #endregion

                #region StiPageFooterBand
                var pageFooterBand = this as StiPageFooterBand;
                if (pageFooterBand != null)
                {
                    var level = 1;
                    foreach (StiComponent component in this.Page.Components)
                    {
                        if (component == pageFooterBand)
                            return level;

                        if (component is StiPageFooterBand)
                            level++;
                    }
                    return level;
                }
                #endregion

                #region StiReportTitleBand
                var reportTitleBand = this as StiReportTitleBand;
                if (reportTitleBand != null)
                {
                    var level = 1;
                    foreach (StiComponent component in this.Page.Components)
                    {
                        if (component == reportTitleBand)
                            return level;

                        if (component is StiReportTitleBand)
                            level++;
                    }
                    return level;
                }
                #endregion

                #region StiReportSummaryBand
                var reportSummaryBand = this as StiReportSummaryBand;
                if (reportSummaryBand != null)
                {
                    var level = 1;
                    for (var index = this.Page.Components.Count - 1; index >= 0; index-- )
                    {
                        var component = this.Page.Components[index];
                        if (component == reportSummaryBand)
                            return level;

                        if (component is StiReportSummaryBand) level++;
                    }
                    return level;
                }
                #endregion

                #region StiEmptyBand
                var emptyBand = this as StiEmptyBand;
                if (emptyBand != null)
                {
                    var level = 1;
                    foreach (StiComponent component in this.Page.Components)
                    {
                        if (component == emptyBand)
                            return level;

                        if (component is StiEmptyBand)
                            level++;
                    }
                    return level;
                }
                #endregion

                #region StiOverlayBand
                var overlayBand = this as StiOverlayBand;
                if (overlayBand != null)
                {
                    var level = 1;
                    foreach (StiComponent component in this.Page.Components)
                    {
                        if (component == overlayBand)
                            return level;

                        if (component is StiOverlayBand)
                            level++;
                    }
                    return level;
                }
                #endregion

                #region StiDataBand
                var dataBand = this as StiDataBand;
                if (dataBand != null)
                {
                    var master = dataBand.MasterComponent as StiDataBand;
                    var level = 1;
                    while (master != null)
                    {
                        master = master.MasterComponent as StiDataBand;
                        level++;
                    }
                    return level;
                }
                #endregion

                #region StiHeaderBand
                var headerBand = this as StiHeaderBand;
                if (headerBand != null)
                {
                    var master = StiHeaderBandV2Builder.GetMaster(headerBand);
                    return master == null ? 1 : master.NestedLevel;
                }
                #endregion

                #region StiFooterBand
                var footerBand = this as StiFooterBand;
                if (footerBand != null)
                {
                    var master = StiFooterBandV2Builder.GetMaster(footerBand);
                    return master == null ? 1 : master.NestedLevel;
                }
                #endregion

                #region StiGroupHeaderBand
                var groupHeaderBand = this as StiGroupHeaderBand;
                if (groupHeaderBand != null)
                {
                    var master = StiGroupHeaderBandV2Builder.GetMaster(groupHeaderBand);
                    return master == null ? 1 : master.NestedLevel;
                }
                #endregion

                #region StiGroupFooterBand
                var groupFooterBand = this as StiGroupFooterBand;
                if (groupFooterBand != null)
                {
                    var master = StiGroupFooterBandV2Builder.GetMaster(groupFooterBand);
                    return master == null ? 1 : master.NestedLevel;
                }
                #endregion

                #region StiChildBand
                var childBand = this as StiChildBand;
                if (childBand != null)
                {
                    var band = childBand.GetMaster();
                    return band == null ? 1 : band.NestedLevel;
                }
                #endregion

                return 0;
            }
        }

	    [Browsable(false)]
        [StiNonSerialized]
        public RectangleD? RectangleMoveComponent { get; set; }
	    #endregion

		#region this
		/// <summary>
		/// Returns the band header text.
		/// </summary>
		/// <returns>Band header text.</returns>
		public virtual string GetHeaderText()
		{
			return ToString();
		}

		/// <summary>
		/// Gets header start color.
		/// </summary>
		public abstract Color HeaderStartColor { get; }

		/// <summary>
		/// Gets header end color.
		/// </summary>
		public abstract Color HeaderEndColor { get; }

		/// <summary>
		/// Gets the header height.
		/// </summary>
		[Browsable(false)]
		public virtual double HeaderSize
		{
			get
			{
			    if (Report == null || !Report.Info.ShowHeaders)
			        return 0;

			    return StiAlignValue.AlignToMaxGrid(15,
			        Page.Unit.ConvertToHInches(Page.GridSize), true);
			}
		}

		/// <summary>
		/// Gets the footer height.
		/// </summary>
		[Browsable(false)]
		public virtual double FooterSize
		{
			get
			{
			    if (Report == null || !Report.Info.ShowHeaders)
			        return 0;

			    return StiAlignValue.AlignToMaxGrid(15,
			        Page.Unit.ConvertToHInches(Page.GridSize), true);
			}
		}
        #endregion

        /// <summary>
        /// Creates a new band.
        /// </summary>
        public StiBand() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new band.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiBand(RectangleD rect) : base(rect)
		{
			CanGrow = true;
			DockStyle = StiDockStyle.Top;
			PlaceOnToolbox = false;
		}
	}
}