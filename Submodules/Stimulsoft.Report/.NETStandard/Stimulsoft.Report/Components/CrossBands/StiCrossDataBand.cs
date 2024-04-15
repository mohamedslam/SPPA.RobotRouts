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
using Stimulsoft.Report.Engine;
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
    /// Class describes Cross Data Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiCrossDataBand), "Stimulsoft.Report.Images.Components.StiCrossDataBand.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.catCrossBands.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiDataBandDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfDataBandDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiCrossBandGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiCrossBandWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiCrossDataBandV1Builder))]
    [StiContextTool(typeof(IStiDataSource))]
    [StiContextTool(typeof(IStiMasterComponent))]
    [StiContextTool(typeof(IStiDataRelation))]
    [StiContextTool(typeof(IStiSort))]
    [StiContextTool(typeof(IStiPrintIfDetailEmpty))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiCrossDataBand : StiDataBand
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
            jObject.RemoveProperty("KeepHeaderTogether");
            jObject.RemoveProperty("KeepFooterTogether");
            jObject.RemoveProperty("KeepChildTogether");
            jObject.RemoveProperty("KeepGroupTogether");
            jObject.RemoveProperty("PrintAtBottom");
            jObject.AddPropertyBool("CanBreak", CanBreak);
            jObject.RemoveProperty("PrintOnAllPages");
            jObject.RemoveProperty("PrintOn");
            jObject.RemoveProperty("RightToLeft");
            jObject.RemoveProperty("ColumnGaps");
            jObject.RemoveProperty("ColumnWidth");
            jObject.RemoveProperty("Columns");
            jObject.RemoveProperty("MinRowsInColumn");
            jObject.RemoveProperty("ColumnDirection");
            jObject.AddPropertyBool("ResetDataSource", ResetDataSource, true);
            jObject.AddPropertyBool("ResetDataSource", ResetDataSource, true);
            jObject.RemoveProperty("DockStyle");
            jObject.RemoveProperty("MinSize");
            jObject.RemoveProperty("MaxSize");
            jObject.RemoveProperty("MaxHeight");
            jObject.RemoveProperty("MinHeight");

            // StiCrossDataBand
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
        public override StiComponentId ComponentId => StiComponentId.StiCrossDataBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.DataBandEditor()
            });

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
                    propHelper.ComponentStyle(),
                    propHelper.OddStyle(),
                    propHelper.EvenStyle()
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
                    propHelper.OddStyle(),
                    propHelper.EvenStyle(),
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
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.Enabled(),
                    propHelper.KeepDetails(),
                    propHelper.PrintIfDetailEmpty()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.CalcInvisible(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.KeepDetails(),
                    propHelper.PrintIfDetailEmpty()
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
                        StiPropertyEventId.GetPointerEvent
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
                    StiPropertyCategories.RenderEvents,
                    new[]
                    {
                        StiPropertyEventId.BeginRenderEvent,
                        StiPropertyEventId.EndRenderEvent,
                        StiPropertyEventId.RenderingEvent,
                    }
                },
                {
                    StiPropertyCategories.ValueEvents,
                    new[]
                    {
                        StiPropertyEventId.GetCollapsedEvent,
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

        #region IStiStateSaveRestore
        /// <summary>
        /// Restores the earlier saved object state.
        /// </summary>
        /// <param name="stateName">A name of the state being restored.</param>
        public override void RestoreState(string stateName)
        {
            base.RestoreState(stateName);
            SetColumnModeToParent();
        }
        #endregion

        #region IStiKeepHeaderTogether Browsable(false)
        [DefaultValue(false)]
        [StiNonSerialized]
        [Browsable(false)]
        public override bool KeepHeaderTogether
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

        #region IStiKeepFooterTogether Browsable(false)
        [DefaultValue(false)]
        [StiNonSerialized]
        [Browsable(false)]
        public override bool KeepFooterTogether
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

        #region IStiKeepChildTogether Browsable(false)
        [DefaultValue(false)]
        [StiNonSerialized]
        [Browsable(false)]
        public override bool KeepChildTogether
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

        #region IStiKeepGroupTogether Browsable(false)
        [DefaultValue(false)]
        [StiNonSerialized]
        [Browsable(false)]
        public override bool KeepGroupTogether
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

        #region IStiEnumerator
        internal void SetColumnModeToParent()
        {
            if (ColumnMode && Parent is StiDataBand && DataSource == null)
            {
                ((StiDataBand)Parent).isEofValue = IsEof;
                ((StiDataBand)Parent).isBofValue = IsBof;
                ((StiDataBand)Parent).positionValue = Position;
            }
        }

        internal void GetColumnModeFromParent()
        {
            if (ColumnMode && Parent is StiDataBand && DataSource == null)
            {
                Enabled = ((StiDataBand)Parent).Enabled;
                isEofValue = ((StiDataBand)Parent).isEofValue;
                isBofValue = ((StiDataBand)Parent).isBofValue;
                positionValue = ((StiDataBand)Parent).positionValue;
            }
        }

        /// <summary>
        /// Sets the position at the beginning.
        /// </summary>
        public override void First()
        {
            base.First();
            SetColumnModeToParent();
        }

        /// <summary>
        /// Move on the previous row.
        /// </summary>
        public override void Prior()
        {
            base.Prior();
            SetColumnModeToParent();
        }

        /// <summary>
        /// Move on the next row.
        /// </summary>
        public override void Next()
        {
            base.Next();
            SetColumnModeToParent();
        }

        /// <summary>
        /// Move on the last row.
        /// </summary>
        public override void Last()
        {
            base.Last();
            SetColumnModeToParent();
        }
        #endregion

        #region Columns Browsable(false)
        [StiNonSerialized]
        [Browsable(false)]
        public override bool RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override double ColumnGaps
        {
            get
            {
                return base.ColumnGaps;
            }
            set
            {
                base.ColumnGaps = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override double ColumnWidth
        {
            get
            {
                return base.ColumnWidth;
            }
            set
            {
                base.ColumnWidth = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override int Columns
        {
            get
            {
                return base.Columns;
            }
            set
            {
                base.Columns = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override int MinRowsInColumn
        {
            get
            {
                return base.MinRowsInColumn;
            }
            set
            {
                base.MinRowsInColumn = value;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override StiColumnDirection ColumnDirection
        {
            get
            {
                return base.ColumnDirection;
            }
            set
            {
                base.ColumnDirection = value;
            }
        }
        #endregion

        #region StiDataBand override
        /// <summary>
        /// Gets or sets value, indicates to reset Data Source postion to begin when preparation for rendering.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Data")]
        public override bool ResetDataSource
        {
            get
            {
                return base.ResetDataSource;
            }
            set
            {
                base.ResetDataSource = value;
            }
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.CrossDataBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Cross;

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.CrossDataBand;

        /// <summary>
        /// Gets a localized name of the component category.
        /// </summary>
        public override string LocalizedCategory => StiLocalization.Get("Report", "CrossBands");

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiCrossDataBand");

        /// <summary>
        /// Gets value, indicates that this cross is the component.
        /// </summary>
        public override bool IsCross => true;

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType
        {
            get
            {
                #region EngineV2
                if (this.Report != null && this.Report.EngineVersion == StiEngineVersion.EngineV2)
                    return base.ComponentType;
                
                #endregion

                #region EngineV1
                else
                {
                    if (MasterComponent != null)
                    {
                        if (MasterComponent is StiCrossDataBand &&
                            MasterComponent.Parent == this.Parent) return StiComponentType.Detail;
                    }
                    return StiComponentType.Simple;
                }
                #endregion
            }
        }
        #endregion

        #region Dock override
        [Browsable(false)]
        internal bool IsRightToLeft { get; set; }

        /// <summary>
        /// Gets or sets a type of the component docking.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        public override StiDockStyle DockStyle
        {
            get
            {
                return IsRightToLeft ? StiDockStyle.Right : StiDockStyle.Left;
            }
            set
            {
                base.DockStyle = value;
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

        #region Render override
        internal int ColumnCurrent { get; set; }
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
            return new StiCrossDataBand();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        internal bool ColumnMode { get; set; }
        #endregion

        /// <summary>
        /// Creates a new component of the type StiCrossDataBand.
        /// </summary>
        internal StiCrossDataBand(bool columnMode) : this()
        {
            ColumnMode = columnMode;
            ResetDataSource = false;
        }

        /// <summary>
        /// Creates a new component of the type StiCrossDataBand.
        /// </summary>
        public StiCrossDataBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiCrossDataBand with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiCrossDataBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
            ResetDataSource = true;
            DockStyle = StiDockStyle.Left;
        }
    }
}