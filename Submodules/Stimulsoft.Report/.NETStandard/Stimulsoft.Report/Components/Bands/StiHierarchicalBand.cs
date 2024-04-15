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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base.Json.Linq;
using System;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the class that realizes a Hierarchical Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiHierarchicalBand), "Stimulsoft.Report.Images.Components.StiHierarchicalBand.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiDataBandDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfDataBandDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiV1Builder(typeof(StiHierarchicalBandV1Builder))]
    [StiV2Builder(typeof(StiHierarchicalBandV2Builder))]
    [StiContextTool(typeof(IStiDataSource))]
    [StiContextTool(typeof(IStiMasterComponent))]
    [StiContextTool(typeof(IStiDataRelation))]
    [StiContextTool(typeof(IStiSort))]
    [StiContextTool(typeof(IStiKeepGroupTogether))]
    [StiContextTool(typeof(IStiKeepChildTogether))]
    [StiContextTool(typeof(IStiKeepHeaderTogether))]
    [StiContextTool(typeof(IStiKeepFooterTogether))]
    [StiContextTool(typeof(IStiPrintOnAllPages))]
    [StiContextTool(typeof(IStiPrintIfDetailEmpty))]
    [StiContextTool(typeof(IStiStartNewPage))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiBreakable))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiDataSourceQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 0)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiDataRelationQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 1)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiMasterComponentQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 2)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiBandQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 3)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, 4)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfDataSourceQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 0)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfDataRelationQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 1)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfMasterComponentQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 2)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfBandQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 3)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo, 4)]
    public class StiHierarchicalBand : StiDataBand
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiHierarchicalBand
            jObject.AddPropertyStringNullOrEmpty("KeyDataColumn", KeyDataColumn);
            jObject.AddPropertyStringNullOrEmpty("MasterKeyDataColumn", MasterKeyDataColumn);
            jObject.AddPropertyStringNullOrEmpty("ParentValue", ParentValue);
            jObject.AddPropertyDouble("Indent", Indent, 20d);
            jObject.AddPropertyStringNullOrEmpty("Headers", Headers);
            jObject.AddPropertyStringNullOrEmpty("Footers", Footers);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "KeyDataColumn":
                        this.KeyDataColumn = property.DeserializeString();
                        break;

                    case "MasterKeyDataColumn":
                        this.MasterKeyDataColumn = property.DeserializeString();
                        break;

                    case "ParentValue":
                        this.ParentValue = property.DeserializeString();
                        break;

                    case "Indent":
                        this.Indent = property.DeserializeDouble();
                        break;

                    case "Headers":
                        this.Headers = property.DeserializeString();
                        break;

                    case "Footers":
                        this.Footers = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiHierarchicalBand;

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
                collection.Add(StiPropertyCategories.Hierarchical, new[]
                {
                    propHelper.KeyDataColumn(),
                    propHelper.MasterKeyDataColumn(),
                    propHelper.ParentValue(),
                    propHelper.Indent()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Hierarchical, new[]
                {
                    propHelper.KeyDataColumn(),
                    propHelper.MasterKeyDataColumn(),
                    propHelper.ParentValue(),
                    propHelper.Indent(),
                    propHelper.Headers(),
                    propHelper.Footers()
                });
            }
            
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
                collection.Add(StiPropertyCategories.Columns, new[]
                {
                    propHelper.Columns(),
                    propHelper.ColumnWidth(),
                    propHelper.ColumnGaps(),
                    propHelper.ColumnDirection(),
                    propHelper.RightToLeft()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Columns, new[]
                {
                    propHelper.Columns(),
                    propHelper.ColumnWidth(),
                    propHelper.ColumnGaps(),
                    propHelper.ColumnDirection(),
                    propHelper.MinRowsInColumn(),
                    propHelper.RightToLeft()
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
                    propHelper.CanBreak(),
                    propHelper.Enabled()
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
                    propHelper.PrintAtBottom(),
                    propHelper.PrintIfDetailEmpty(),
                    propHelper.PrintOn(),
                    propHelper.PrintOnAllPages(),
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
        public override string HelpUrl => "user-manual/index.html?report_internals_hierarchical_band.htm";
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.HierarchicalBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiHierarchicalBand");
        #endregion

        #region StiBand override
        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(118, 167, 151);

        /// <summary>
		/// Gets header end color.
		/// </summary>
		[Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(118, 167, 151);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets column which contains data key. 
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiDataColumnConverter))]
        [StiCategory("Hierarchical")]
        [DefaultValue("")]
        [Description("Gets or sets column which contains data key.")]
        [StiOrder(StiPropertyOrder.HierarchicalKeyDataColumn)]
        [StiPropertyLevel(StiLevel.Basic)]
        public string KeyDataColumn { get; set; } = string.Empty;

        /// <summary>
		/// Gets or sets column which contains data master key. 
		/// </summary>
		[StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(StiDataColumnConverter))]
        [StiCategory("Hierarchical")]
        [DefaultValue("")]
        [Description("Gets or sets column which contains data master key.")]
        [StiOrder(StiPropertyOrder.HierarchicalMasterKeyDataColumn)]
        [StiPropertyLevel(StiLevel.Basic)]
        public string MasterKeyDataColumn { get; set; } = string.Empty;

        /// <summary>
		/// Gets or sets column which contains parent value which identifies parent rows. 
		/// </summary>
		[StiSerializable]
        [StiCategory("Hierarchical")]
        [DefaultValue("")]
        [Description("Gets or sets column which contains parent value which identifies parent rows.")]
        [StiOrder(StiPropertyOrder.HierarchicalParentValue)]
        [StiPropertyLevel(StiLevel.Basic)]
        public string ParentValue { get; set; } = string.Empty;

        /// <summary>
		/// Gets or sets indent from the left side of band for offset of data levels. 
		/// </summary>
		[StiSerializable]
        [DefaultValue(20d)]
        [StiCategory("Hierarchical")]
        [Description("Gets or sets indent from the left side of band for offset of data levels.")]
        [StiOrder(StiPropertyOrder.HierarchicalIndent)]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual double Indent { get; set; } = 20d;

        /// <summary>
        /// Gets or sets the list of headers for the hierarchical band.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiHeadersEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Hierarchical")]
        [DefaultValue("")]
        [Description("Gets or sets the list of headers for the hierarchical band.")]
        [StiOrder(StiPropertyOrder.HierarchicalHeaders)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Standard)]
        public string Headers { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of footers for the hierarchical band.
        /// </summary>
        [StiSerializable]
        [Editor("Stimulsoft.Report.Components.Design.StiFootersEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Hierarchical")]
        [DefaultValue("")]
        [Description("Gets or sets the list of footers for the hierarchical band.")]
        [StiOrder(StiPropertyOrder.HierarchicalFooters)]
        [StiEngine(StiEngineVersion.EngineV2)]
        [StiPropertyLevel(StiLevel.Standard)]
        public string Footers { get; set; } = string.Empty;
        #endregion

        #region Render override
        private StiHierarchicalBandInfoV2 hierarchicalBandInfoV2;
        [Browsable(false)]
        public StiHierarchicalBandInfoV2 HierarchicalBandInfoV2
        {
            get
            {
                return hierarchicalBandInfoV2 ?? (hierarchicalBandInfoV2 = new StiHierarchicalBandInfoV2());
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiHierarchicalBand();
        }
        #endregion

        /// <summary>
		/// Creates an object of the type StiHierarchicalBand.
		/// </summary>
		public StiHierarchicalBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        ///  Creates an object of the type StiHierarchicalBand.
        /// </summary>
        /// <param name="rect">The rectangle decribes size and position of the component.</param>
        public StiHierarchicalBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}