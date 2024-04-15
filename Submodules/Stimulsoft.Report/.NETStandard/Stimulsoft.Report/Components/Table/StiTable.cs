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
using Stimulsoft.Report.Events;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Report.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components.Table
{
    [StiServiceBitmap(typeof(StiTable), "Stimulsoft.Report.Images.Components.StiTable.png")]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiDataBandDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfDataBandDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
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
    [StiEngine(StiEngineVersion.EngineV2)]
    public class StiTable : 
        StiDataBand,
        IStiTableComponent
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty(nameof(Columns));
            jObject.RemoveProperty(nameof(ColumnWidth));
            jObject.RemoveProperty(nameof(ColumnGaps));
            jObject.RemoveProperty(nameof(MinRowsInColumn));

            // StiTable
            jObject.AddPropertyBool(nameof(DockableTable), DockableTable, true);
            jObject.AddPropertyEnum(nameof(AutoWidth), AutoWidth, StiTableAutoWidth.None);
            jObject.AddPropertyEnum(nameof(AutoWidthType), AutoWidthType, StiTableAutoWidthType.None);
            jObject.AddPropertyInt(nameof(RowCount), RowCount, 5);
            jObject.AddPropertyInt(nameof(ColumnCount), ColumnCount, 5);
            jObject.AddPropertyInt(nameof(FooterRowsCount), FooterRowsCount);
            jObject.AddPropertyInt(nameof(HeaderRowsCount), HeaderRowsCount);
            jObject.AddPropertyEnum(nameof(HeaderPrintOn), HeaderPrintOn, StiPrintOnType.AllPages);
            jObject.AddPropertyBool(nameof(HeaderCanGrow), HeaderCanGrow, true);
            jObject.AddPropertyBool(nameof(HeaderCanShrink), HeaderCanShrink);
            jObject.AddPropertyBool(nameof(HeaderCanBreak), HeaderCanBreak);
            jObject.AddPropertyBool(nameof(HeaderPrintAtBottom), HeaderPrintAtBottom);
            jObject.AddPropertyBool(nameof(HeaderPrintIfEmpty), HeaderPrintIfEmpty, true);
            jObject.AddPropertyBool(nameof(HeaderPrintOnAllPages), HeaderPrintOnAllPages, true);
            jObject.AddPropertyEnum(nameof(HeaderPrintOnEvenOddPages), HeaderPrintOnEvenOddPages, StiPrintOnEvenOddPagesType.Ignore);
            jObject.AddPropertyEnum(nameof(FooterPrintOn), FooterPrintOn, StiPrintOnType.AllPages);
            jObject.AddPropertyBool(nameof(FooterCanGrow), FooterCanGrow, true);
            jObject.AddPropertyBool(nameof(FooterCanShrink), FooterCanShrink);
            jObject.AddPropertyBool(nameof(FooterCanBreak), FooterCanBreak);
            jObject.AddPropertyBool(nameof(FooterPrintAtBottom), FooterPrintAtBottom);
            jObject.AddPropertyBool(nameof(FooterPrintIfEmpty), FooterPrintIfEmpty, true);
            jObject.AddPropertyBool(nameof(FooterPrintOnAllPages), FooterPrintOnAllPages);
            jObject.AddPropertyEnum(nameof(FooterPrintOnEvenOddPages), FooterPrintOnEvenOddPages, StiPrintOnEvenOddPagesType.Ignore);
            jObject.AddPropertyInt(nameof(NumberID), NumberID);
            jObject.AddPropertyStringNullOrEmpty("TableStyleFX", TableStyleFX?.StyleId.ToString());
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(DockableTable):
                        this.DockableTable = property.DeserializeBool();
                        break;

                    case nameof(AutoWidth):
                        this.AutoWidth = property.DeserializeEnum<StiTableAutoWidth>();
                        break;

                    case nameof(AutoWidthType):
                        this.AutoWidthType = property.DeserializeEnum<StiTableAutoWidthType>();
                        break;

                    case nameof(RowCount):
                        this.rowCount = property.DeserializeInt();
                        break;

                    case nameof(ColumnCount):
                        this.columnCount = property.DeserializeInt();
                        break;

                    case nameof(FooterRowsCount):
                        this.footerRowsCount = property.DeserializeInt();
                        break;

                    case nameof(HeaderRowsCount):
                        this.headerRowsCount = property.DeserializeInt();
                        break;

                    case nameof(HeaderPrintOn):
                        this.HeaderPrintOn = property.DeserializeEnum<StiPrintOnType>();
                        break;

                    case nameof(HeaderCanGrow):
                        this.HeaderCanGrow = property.DeserializeBool();
                        break;

                    case nameof(HeaderCanShrink):
                        this.HeaderCanShrink = property.DeserializeBool();
                        break;

                    case nameof(HeaderCanBreak):
                        this.HeaderCanBreak = property.DeserializeBool();
                        break;

                    case nameof(HeaderPrintAtBottom):
                        this.HeaderPrintAtBottom = property.DeserializeBool();
                        break;

                    case nameof(HeaderPrintIfEmpty):
                        this.HeaderPrintIfEmpty = property.DeserializeBool();
                        break;

                    case nameof(HeaderPrintOnAllPages):
                        this.HeaderPrintOnAllPages = property.DeserializeBool();
                        break;

                    case nameof(HeaderPrintOnEvenOddPages):
                        this.HeaderPrintOnEvenOddPages = property.DeserializeEnum<StiPrintOnEvenOddPagesType>();
                        break;

                    case nameof(FooterPrintOn):
                        this.FooterPrintOn = property.DeserializeEnum<StiPrintOnType>();
                        break;

                    case nameof(FooterCanGrow):
                        this.FooterCanGrow = property.DeserializeBool();
                        break;

                    case nameof(FooterCanShrink):
                        this.FooterCanShrink = property.DeserializeBool();
                        break;

                    case nameof(FooterCanBreak):
                        this.FooterCanBreak = property.DeserializeBool();
                        break;

                    case nameof(FooterPrintAtBottom):
                        this.FooterPrintAtBottom = property.DeserializeBool();
                        break;

                    case nameof(FooterPrintIfEmpty):
                        this.FooterPrintIfEmpty = property.DeserializeBool();
                        break;

                    case nameof(FooterPrintOnAllPages):
                        this.FooterPrintOnAllPages = property.DeserializeBool();
                        break;

                    case nameof(FooterPrintOnEvenOddPages):
                        this.FooterPrintOnEvenOddPages = property.DeserializeEnum<StiPrintOnEvenOddPagesType>();
                        break;

                    case nameof(NumberID):
                        this.NumberID = property.DeserializeInt();
                        break;

                    case "TableStyleFX":
                        this.TableStyleFX = StiTableStyleFX.CreateFromJson(property.DeserializeString());
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiTable;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Data,
                    new[]
                    {
                        propHelper.DataBandEditor(),
                    }
                },
                {
                    StiPropertyCategories.Table,
                    new[]
                    {
                        propHelper.TableAutoWidth(),
                        propHelper.AutoWidthType(),
                        propHelper.ColumnCount(),
                        propHelper.RowCount(),
                        propHelper.HeaderRowsCount(),
                        propHelper.FooterRowsCount(),
                        propHelper.RightToLeft(),
                        propHelper.DockableTable()
                    }
                },
                {
                    StiPropertyCategories.HeaderTable,
                    new[]
                    {
                        propHelper.HeaderPrintOn(),
                        propHelper.HeaderCanGrow(),
                        propHelper.HeaderCanShrink(),
                        propHelper.HeaderCanBreak(),
                        propHelper.HeaderPrintAtBottom(),
                        propHelper.HeaderPrintIfEmpty(),
                        propHelper.HeaderPrintOnAllPages(),
                        propHelper.HeaderPrintOnEvenOddPages()
                    }
                },
                {
                    StiPropertyCategories.FooterTable,
                    new[]
                    {
                        propHelper.FooterPrintOn(),
                        propHelper.FooterCanGrow(),
                        propHelper.FooterCanShrink(),
                        propHelper.FooterCanBreak(),
                        propHelper.FooterPrintAtBottom(),
                        propHelper.FooterPrintIfEmpty(),
                        propHelper.FooterPrintOnAllPages(),
                        propHelper.FooterPrintOnEvenOddPages()
                    }
                }
            };

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
                    propHelper.CanShrink(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.KeepDetailsTogether(),
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
        public override string HelpUrl => "user-manual/report_internals_table.htm";
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties, bool cloneComponents)
        {
            var cloneTable = (StiTable)base.Clone(cloneProperties, false);

            cloneTable.NumberID = this.NumberID;
            cloneTable.rowCount = this.rowCount;
            cloneTable.columnCount = this.columnCount;
            cloneTable.footerRowsCount = this.footerRowsCount;
            cloneTable.headerRowsCount = this.headerRowsCount;
            cloneTable.DockableTable = this.DockableTable;

            cloneTable.HeaderPrintOn = this.HeaderPrintOn;
            cloneTable.HeaderCanGrow = this.HeaderCanGrow;
            cloneTable.HeaderCanShrink = this.HeaderCanShrink;
            cloneTable.HeaderCanBreak = this.HeaderCanBreak;
            cloneTable.HeaderPrintAtBottom = this.HeaderPrintAtBottom;
            cloneTable.HeaderPrintIfEmpty = this.HeaderPrintIfEmpty;
            cloneTable.HeaderPrintOnAllPages = this.HeaderPrintOnAllPages;
            cloneTable.HeaderPrintOnEvenOddPages = this.HeaderPrintOnEvenOddPages;
            cloneTable.FooterPrintOn = this.FooterPrintOn;
            cloneTable.FooterCanGrow = this.FooterCanGrow;
            cloneTable.FooterCanShrink = this.FooterCanShrink;
            cloneTable.FooterCanBreak = this.FooterCanBreak;
            cloneTable.FooterPrintAtBottom = this.FooterPrintAtBottom;
            cloneTable.FooterPrintIfEmpty = this.FooterPrintIfEmpty;
            cloneTable.FooterPrintOnAllPages = this.FooterPrintOnAllPages;
            cloneTable.FooterPrintOnEvenOddPages = this.FooterPrintOnEvenOddPages;
            cloneTable.AutoWidth = this.AutoWidth;
            cloneTable.AutoWidthType = this.AutoWidthType;
            cloneTable.TableStyleFX = this.TableStyleFX?.Clone() as StiTableStyleFX;

            cloneTable.Components = new StiComponentsCollection(cloneTable);
            if (!cloneComponents) return cloneTable;

            foreach (StiComponent comp in this.Components)
            {
                switch (((IStiTableCell)comp).CellType)
                {
                    case StiTablceCellType.Text:
                        var cloneTableCellText = (StiTableCell)comp.Clone(true);
                        cloneTableCellText.Parent = cloneTable;
                        cloneTable.Components.Add(cloneTableCellText);
                        break;

                    case StiTablceCellType.Image:
                        var cloneTableCellImage = (StiTableCellImage)comp.Clone(true);
                        cloneTableCellImage.Parent = cloneTable;
                        cloneTable.Components.Add(cloneTableCellImage);
                        break;

                    case StiTablceCellType.CheckBox:
                        var cloneTableCellCheckBox = (StiTableCellCheckBox)comp.Clone(true);
                        cloneTableCellCheckBox.Parent = cloneTable;
                        cloneTable.Components.Add(cloneTableCellCheckBox);
                        break;

                    case StiTablceCellType.RichText:
                        var cloneTableCellRichText = (StiTableCellRichText)comp.Clone(true);
                        cloneTableCellRichText.Parent = cloneTable;
                        cloneTable.Components.Add(cloneTableCellRichText);
                        break;
                }
            }
            return cloneTable;
        }
        #endregion

        #region IStiUnitConvert
        /// <summary>
        /// Converts a component out of one unit into another.
        /// </summary>
        /// <param name="oldUnit">Old units.</param>
        /// <param name="newUnit">New units.</param>
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
        {
            base.Convert(oldUnit, newUnit, isReportSnapshot);
            DistributeRows();
            DistributeColumns();
        }
        #endregion

        #region Properties Browsable(false)
        [Browsable(false)]
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
        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiTable");

        public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Table;
        #endregion

        #region Properties
        [Browsable(false)]
        internal bool IsConverted { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates that the table will be adjusted to the top of the parent component area.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(true)]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Table")]
        [StiOrder(StiPropertyOrder.TableDockableTable)]
        [Description("Gets or sets a value which indicates that the table will be adjusted to the top of the parent component area.")]
        public bool DockableTable { get; set; } = true;

        /// <summary>
        /// Gets or sets which range use table component for adjusting columns width.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(typeof(StiTableAutoWidth), "None")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Table")]
        [StiOrder(StiPropertyOrder.TableAutoWidth)]
        [Description("Gets or sets which range use table component for adjusting columns width.")]
        public StiTableAutoWidth AutoWidth { get; set; } = StiTableAutoWidth.None;

        /// <summary>
        /// Gets or sets how to table component adjust columns width.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(typeof(StiTableAutoWidthType), "None")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Table")]
        [StiOrder(StiPropertyOrder.TableAutoWidthType)]
        [Description("Gets or sets how to table component adjust columns width.")]
        public StiTableAutoWidthType AutoWidthType { get; set; } = StiTableAutoWidthType.None;

        private int rowCount = 5;
        /// <summary>
        /// Get or sets a number of rows in the table.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(5)]
        [StiCategory("Table")]
        [StiOrder(StiPropertyOrder.TableRowCount)]
        [Description("Get or sets a number of rows in the table. ")]
        public int RowCount
        {
            get 
            { 
                return rowCount; 
            }
            set
            {
                if (value <= 0) return;

                var oldValue = rowCount;
                rowCount = value;

                ChangeRowCount(oldValue, value);
            }
        }

        private int columnCount = 5;
        /// <summary>
        /// Get or sets a number of columns in the table.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(5)]
        [StiCategory("Table")]
        [StiOrder(StiPropertyOrder.TableColumnCount)]
        [Description("Get or sets a number of columns in the table.")]
        public int ColumnCount
        {
            get 
            { 
                return columnCount; 
            }
            set
            {
                if (value <= 0) return;

                int oldValue = columnCount;
                columnCount = value;

                ChangeColumnCount(oldValue, value);
            }
        }

        private int footerRowsCount = 0;
        /// <summary>
        /// Get or sets a number of footer rows in the table.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(0)]
        [StiCategory("Table")]
        [StiOrder(StiPropertyOrder.TableFooterRowsCount)]
        [Description("Get or sets a number of footer rows in the table.")]
        public int FooterRowsCount
        {
            get 
            { 
                return footerRowsCount; 
            }
            set 
            {
                if (!this.IsDesigning || value <= rowCount - headerRowsCount)
                {
                    footerRowsCount = value;
                    RefreshTableStyle();
                }
            }
        }

        private int headerRowsCount;
        /// <summary>
        /// Get or sets a number of header rows in the table.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(0)]
        [StiCategory("Table")]
        [StiOrder(StiPropertyOrder.TableHeaderRowsCount)]
        [Description("Get or sets a number of header rows in the table.")]
        public int HeaderRowsCount
        {
            get 
            { 
                return headerRowsCount; 
            }
            set
            {
                if (!this.IsDesigning || value <= rowCount - footerRowsCount)
                {
                    headerRowsCount = value;
                    RefreshTableStyle();
                }
            }
        }

        /// <summary>
        /// Default cell height
        /// </summary>
        [Browsable(false)]
        public double DefaultHeightCell
        {
            get
            {
                if (this.Page == null || Page.Report == null) return 1.0;

                switch (Page.Report.ReportUnit)
                {
                    case StiReportUnitType.Centimeters:
                        return 0.8;

                    case StiReportUnitType.HundredthsOfInch:
                        return 30;

                    case StiReportUnitType.Inches:
                        return 0.3;

                    case StiReportUnitType.Millimeters:
                        return 8;

                    default:
                        return 1.0;
                }
            }
        }
        #endregion

        #region Properties.Header
        /// <summary>
        /// Gets or sets a value which indicates how header of table will be print on pages.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(StiPrintOnType.AllPages)]
        [StiCategory("HeaderTable")]
        [StiOrder(StiPropertyOrder.TableBandHeaderPrintOn)]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates how header of table will be print on pages.")]
        public StiPrintOnType HeaderPrintOn { get; set; } = StiPrintOnType.AllPages;

        /// <summary>
        /// Gets or sets value indicates that header of table can grow its height.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(true)]
        [StiCategory("HeaderTable")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.TableBandHeaderCanGrow)]
        [Description("Gets or sets value indicates that header of table can grow its height.")]
        public bool HeaderCanGrow { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that this header can shrink its height.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(false)]
        [StiCategory("HeaderTable")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.TableBandHeaderCanShrink)]
        [Description("Gets or sets value which indicates that this header can shrink its height.")]
        public bool HeaderCanShrink { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that this header can break its content.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(false)]
        [StiCategory("HeaderTable")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.TableBandHeaderCanBreak)]
        [Description("Gets or sets value which indicates that this header can break its content.")]
        public bool HeaderCanBreak { get; set; }

        /// <summary>
        /// Gets or sets value indicates that the header of table will be print at bottom of page.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(false)]
        [StiCategory("HeaderTable")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.TableBandHeaderPrintAtBottom)]
        [Description("Gets or sets value indicates that the header of table will be print at bottom of page.")]
        public bool HeaderPrintAtBottom { get; set; }

        /// <summary>
        /// Gets or sets value indicates that the header will be print if data not present in table.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(true)]
        [StiCategory("HeaderTable")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.TableBandHeaderPrintIfEmpty)]
        [Description("Gets or sets value indicates that the header will be print if data not present in table.")]
        public bool HeaderPrintIfEmpty { get; set; } = true;

        /// <summary>
        /// Gets or sets value indicates that the header of table will be printed on all pages.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(true)]
        [StiCategory("HeaderTable")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.TableBandHeaderPrintOnAllPages)]
        [Description("Gets or sets value indicates that the header of table will be printed on all pages.")]
        public virtual bool HeaderPrintOnAllPages { get; set; } = true;

        /// <summary>
        /// Gets or sets value indicates how the header of table will be printed on even-odd pages.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(StiPrintOnEvenOddPagesType.Ignore)]
        [StiCategory("HeaderTable")]
        [StiOrder(StiPropertyOrder.TableBandHeaderPrintOnEvenOddPages)]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates how the header of table will be printed on even-odd pages.")]
        public virtual StiPrintOnEvenOddPagesType HeaderPrintOnEvenOddPages { get; set; } = StiPrintOnEvenOddPagesType.Ignore;
        #endregion

        #region Properties.Footer
        /// <summary>
        /// Gets or sets a value which indicates how footer of table will be print on pages.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(StiPrintOnType.AllPages)]
        [StiCategory("FooterTable")]
        [StiOrder(StiPropertyOrder.TableBandFooterPrintOn)]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates how footer of table will be print on pages.")]
        public StiPrintOnType FooterPrintOn { get; set; } = StiPrintOnType.AllPages;

        /// <summary>
        /// Gets or sets value indicates that footer of table can grow its height.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(true)]
        [StiCategory("FooterTable")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.TableBandFooterCanGrow)]
        [Description("Gets or sets value indicates that footer of table can grow its height.")]
        public bool FooterCanGrow { get; set; } = true;

        /// <summary>
        /// Gets or sets value which indicates that this footer can shrink its height.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(false)]
        [StiCategory("FooterTable")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.TableBandFooterCanShrink)]
        [Description("Gets or sets value which indicates that this footer can shrink its height.")]
        public bool FooterCanShrink { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that this footer can break its content.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(false)]
        [StiCategory("FooterTable")]
        [StiOrder(StiPropertyOrder.TableBandFooterCanBreak)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that this footer can break its content.")]
        public bool FooterCanBreak { get; set; }

        /// <summary>
        /// Gets or sets value indicates that the footer of table will be print at bottom of page.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(false)]
        [StiCategory("FooterTable")]
        [StiOrder(StiPropertyOrder.TableBandFooterPrintAtBottom)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the footer of table will be print at bottom of page.")]
        public bool FooterPrintAtBottom { get; set; }

        /// <summary>
        /// Gets or sets value indicates that the footer will be print if data not present in table.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(true)]
        [StiCategory("FooterTable")]
        [StiOrder(StiPropertyOrder.TableBandFooterPrintIfEmpty)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the footer will be print if data not present in table.")]
        public bool FooterPrintIfEmpty { get; set; } = true;

        /// <summary>
        /// Gets or sets value indicates that the footer of table will be printed on all pages.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(false)]
        [StiCategory("FooterTable")]
        [StiOrder(StiPropertyOrder.TableBandFooterPrintOnAllPages)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the footer of table will be printed on all pages.")]
        public virtual bool FooterPrintOnAllPages { get; set; } = false;

        /// <summary>
        /// Gets or sets value indicates how the footer of table will be printed on even-odd pages.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(StiPrintOnEvenOddPagesType.Ignore)]
        [StiCategory("FooterTable")]
        [StiOrder(StiPropertyOrder.TableBandFooterPrintOnEvenOddPages)]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates how the footer of table will be printed on even-odd pages.")]
        public virtual StiPrintOnEvenOddPagesType FooterPrintOnEvenOddPages { get; set; } = StiPrintOnEvenOddPagesType.Ignore;
        #endregion

        #region Properties override
        [Editor("Stimulsoft.Report.Design.StiTableStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public override string ComponentStyle
        {
            get
            {
                return base.ComponentStyle;
            }
            set
            {
                base.ComponentStyle = value;

                RefreshTableStyle();
            }
        }

        [Browsable(true)]
        [StiCategory("Table")]
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

        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 300, 120);

        public override Color HeaderStartColor => Color.FromArgb(144, 192, 241);

        public override Color HeaderEndColor => Color.FromArgb(144, 192, 241);
        #endregion

        #region Properties Browsable(false)
        [StiSerializable]
        [DefaultValue(0)]
        [Browsable(false)]
        public int NumberID { get; set; }

        [Browsable(false)]
        [StiNonSerialized]
        public override int Columns => 0;

        [Browsable(false)]
        [StiNonSerialized]
        public override double ColumnWidth => 0;

        [Browsable(false)]
        [StiNonSerialized]
        public override double ColumnGaps => 0;

        [Browsable(false)]
        public override StiColumnDirection ColumnDirection => base.ColumnDirection;

        [Browsable(false)]
        [StiNonSerialized]
        public override int MinRowsInColumn => 0;

        [Browsable(false)]
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

        [Browsable(false)]
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
        public override SizeD MinSize
        {
            get
            {
                return new SizeD(0, 0);
            }
            set
            {
                base.MinSize = value;
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
                double oldWidth = base.Width;
                if (value != base.Width)
                    base.Width = value;
                else
                    return;

                if (value > 0 && value != oldWidth)
                {
                    ResizeWidthCell(oldWidth);
                }
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

                if (this.Components.Count == 0 && Page != null)
                {
                    SelectAll();
                    CreateCell();
                }
            }
        }
        
        [Browsable(false)]
        public override bool Dockable
        {
            get
            {
                return DockableTable;
            }
            set
            {
            }
        }

        private StiTableStyleFX tableStyleFX;
        [StiSerializable(StiSerializationVisibility.Class)]
        [TypeConverter(typeof(StiTableStyleConverter))]
        [Browsable(false)]
        public StiTableStyleFX TableStyleFX
        {
            get
            {
                return tableStyleFX;
            }
            set
            {
                if (tableStyleFX == value) return;

                tableStyleFX = value;
                RefreshTableStyle();
            }
        }

        [Obsolete("TableStyle property is obsolete.")]
        [Browsable(false)]
        public StiTableStyle TableStyle
        {
            get
            {
                return StiTableStyle.StyleNone;
            }
            set
            {
            }
        }
        #endregion

        #region Methods
        internal void ChangeGrowToHeightAtCell(StiComponent cell)
        {
            var index = Components.IndexOf(cell);
            if (index == -1) return;

            var numberRow = index / columnCount;
            var startIndex = numberRow * columnCount;
            var canGrow = false;
            for (var indexCell = startIndex; indexCell < startIndex + columnCount; indexCell++)
            {
                var comp = Components[indexCell];
                if (!comp.CanGrow) continue;

                canGrow = true;
                break;
            }

            for (var indexCell = startIndex; indexCell < startIndex + columnCount; indexCell++)
            {
                var comp = Components[indexCell];
                comp.GrowToHeight = canGrow;
            }
        }
        #endregion

        #region Methods.Style
        public void RefreshTableStyle()
        {
            ApplyCustomStyle();
        }

        private void ApplyCustomStyle()
        {
            if (this.Components.Count == 0) return;

            Stimulsoft.Report.StiTableStyle style = null;
            if (!string.IsNullOrEmpty(base.ComponentStyle))
            {
                if (this.Report != null)
                    style = this.Report.Styles[base.ComponentStyle] as Stimulsoft.Report.StiTableStyle;
            }
            else
            {
                style = tableStyleFX;
            }

            if (style == null) return;

            //Header and Footer
            var headerBrush = new StiSolidBrush(style.HeaderColor);
            var headerTextBrush = new StiSolidBrush(style.HeaderForeground);

            var footerBrush = new StiSolidBrush(style.FooterColor);
            var footerTextBrush = new StiSolidBrush(style.FooterForeground);

            //Data
            var dataBorder = new StiBorder(StiBorderSides.All, style.GridColor, 1, StiPenStyle.Solid, false, 4, new StiSolidBrush(style.GridColor));
            var dataLeftBorder = new StiBorder(StiBorderSides.All, style.GridColor, 1, StiPenStyle.Solid,
                            false, 4, new StiSolidBrush(style.GridColor));
            var dataRightBorder = new StiBorder(StiBorderSides.All, style.GridColor, 1, StiPenStyle.Solid,
                false, 4, new StiSolidBrush(style.GridColor));

            var dataBrush = new StiSolidBrush(style.DataColor);
            var dataTextBrush = new StiSolidBrush(style.DataForeground);

            #region Fill Header
            int numberRow = 0;
            if (this.headerRowsCount > 0)
            {
                int indexCell = 0;
                for (int indexRow = 0; indexRow < headerRowsCount; indexRow++)
                {
                    for (int indexCol = 0; indexCol < columnCount; indexCol++)
                    {
                        #region SetStyle
                        var headerCell = this.Components[indexCell];
                        if (headerCell != null)
                        {
                            switch (((IStiTableCell)headerCell).CellType)
                            {
                                case StiTablceCellType.Text:
                                    ((StiTableCell)headerCell).Border = new StiBorder();
                                    ((StiTableCell)headerCell).Brush = (StiBrush)headerBrush.Clone();
                                    ((StiTableCell)headerCell).TextBrush = (StiSolidBrush)headerTextBrush.Clone();
                                    break;

                                case StiTablceCellType.Image:
                                    ((StiTableCellImage)headerCell).Border = new StiBorder();
                                    ((StiTableCellImage)headerCell).Brush = (StiBrush)headerBrush.Clone();
                                    break;

                                case StiTablceCellType.CheckBox:
                                    ((StiTableCellCheckBox)headerCell).Border = new StiBorder();
                                    ((StiTableCellCheckBox)headerCell).Brush = (StiBrush)headerBrush.Clone();
                                    break;

                                case StiTablceCellType.RichText:
                                    ((StiTableCellRichText)headerCell).Border = new StiBorder();
                                    ((StiTableCellRichText)headerCell).BackColor = StiBrush.ToColor((StiBrush)headerBrush.Clone());
                                    break;
                            }
                        }
                        indexCell++;
                        #endregion
                    }
                }
                numberRow = headerRowsCount;
            }
            #endregion

            #region Fill Footer
            if (this.footerRowsCount > 0)
            {
                int nRow = this.rowCount - this.footerRowsCount;
                int indexCell = nRow * this.columnCount;
                for (int indexRow = nRow; indexRow < this.rowCount; indexRow++)
                {
                    for (int indexCol = 0; indexCol < columnCount; indexCol++)
                    {
                        #region SetStyle
                        var footerCell = this.Components[indexCell];
                        if (footerCell != null)
                        {
                            switch (((IStiTableCell)footerCell).CellType)
                            {
                                case StiTablceCellType.Text:
                                    ((StiTableCell)footerCell).Border = new StiBorder();
                                    ((StiTableCell)footerCell).Brush = (StiBrush)footerBrush.Clone();
                                    ((StiTableCell)footerCell).TextBrush = (StiSolidBrush)footerTextBrush.Clone();
                                    break;

                                case StiTablceCellType.Image:
                                    ((StiTableCellImage)footerCell).Border = new StiBorder();
                                    ((StiTableCellImage)footerCell).Brush = (StiBrush)footerBrush.Clone();
                                    break;

                                case StiTablceCellType.CheckBox:
                                    ((StiTableCellCheckBox)footerCell).Border = new StiBorder();
                                    ((StiTableCellCheckBox)footerCell).Brush = (StiBrush)footerBrush.Clone();
                                    break;

                                case StiTablceCellType.RichText:
                                    ((StiTableCellRichText)footerCell).Border = new StiBorder();
                                    ((StiTableCellRichText)footerCell).BackColor = StiBrush.ToColor((StiBrush)footerBrush.Clone());
                                    break;
                            }
                        }
                        indexCell++;
                        #endregion
                    }
                }
            }
            #endregion

            #region Fill Data
            int indexDataCell = numberRow * this.columnCount;
            for (int indexRow = numberRow; indexRow < rowCount - this.footerRowsCount; indexRow++)
            {
                for (int indexCol = 0; indexCol < columnCount; indexCol++)
                {
                    #region SetStyle
                    var dataCell = this.Components[indexDataCell];
                    if (dataCell != null)
                    {
                        switch (((IStiTableCell)dataCell).CellType)
                        {
                            case StiTablceCellType.Text:
                                if (indexCol == 0)
                                    ((StiTableCell)dataCell).Border = (StiBorder)dataLeftBorder.Clone();

                                else if (indexCol == columnCount - 1)
                                    ((StiTableCell)dataCell).Border = (StiBorder)dataRightBorder.Clone();

                                else
                                    ((StiTableCell)dataCell).Border = (StiBorder)dataBorder.Clone();

                                ((StiTableCell)dataCell).Brush = (StiBrush)dataBrush.Clone();
                                ((StiTableCell)dataCell).TextBrush = (StiSolidBrush)dataTextBrush.Clone();
                                break;

                            case StiTablceCellType.Image:
                                if (indexCol == 0)
                                    ((StiTableCellImage)dataCell).Border = (StiBorder)dataLeftBorder.Clone();

                                else if (indexCol == columnCount - 1)
                                    ((StiTableCellImage)dataCell).Border = (StiBorder)dataRightBorder.Clone();

                                else
                                    ((StiTableCellImage)dataCell).Border = (StiBorder)dataBorder.Clone();

                                ((StiTableCellImage)dataCell).Brush = (StiBrush)dataBrush.Clone();
                                break;

                            case StiTablceCellType.CheckBox:
                                if (indexCol == 0)
                                    ((StiTableCellCheckBox)dataCell).Border = (StiBorder)dataLeftBorder.Clone();

                                else if (indexCol == columnCount - 1)
                                    ((StiTableCellCheckBox)dataCell).Border = (StiBorder)dataRightBorder.Clone();

                                else
                                    ((StiTableCellCheckBox)dataCell).Border = (StiBorder)dataBorder.Clone();

                                ((StiTableCellCheckBox)dataCell).Brush = (StiBrush)dataBrush.Clone();
                                break;

                            case StiTablceCellType.RichText:
                                if (indexCol == 0)
                                    ((StiTableCellRichText)dataCell).Border = (StiBorder)dataLeftBorder.Clone();

                                else if (indexCol == columnCount - 1)
                                    ((StiTableCellRichText)dataCell).Border = (StiBorder)dataRightBorder.Clone();

                                else
                                    ((StiTableCellRichText)dataCell).Border = (StiBorder)dataBorder.Clone();

                                ((StiTableCellRichText)dataCell).BackColor = StiBrush.ToColor((StiBrush)dataBrush.Clone());
                                break;
                        }
                    }
                    indexDataCell++;
                    #endregion
                }
            }
            #endregion
        }
        #endregion

        #region Methods.Change Row&Column Count
        private void ChangeRowCount(int oldValue, int value)
        {
            if (Page == null || Report == null) return;
            if (value <= 0 || value == oldValue) return;

            if (value < headerRowsCount + footerRowsCount)
            {
                headerRowsCount = 0;
                footerRowsCount = 0;
            }
            base.MinHeight = Page.GridSize * value;

            var difference = value - oldValue;

            if (difference > 0)
                AddNewRows(difference);

            else
                DeleteLastRows(Math.Abs(difference), oldValue);
            
        }

        private void ChangeColumnCount(int oldValue, int value)
        {
            if (Page == null || Report == null) return;
            if (value <= 0 || value == oldValue) return;

            var difference = value - oldValue;
            
            if (difference > 0)
                AddTableNewColumns(difference, oldValue);

            else
                DeleteTableColumns(Math.Abs(difference), oldValue);
        }
        #endregion

        #region Methods.JoinCells
        /// <summary>
        /// Join of the allocated cells
        /// </summary>
        public int[] CreateJoin(ref double sumWidth, ref double sumHeight, ref int joinWidth, ref int joinHeight)
        {
            int count = GetCountSelectedCells();
            if (count <= 1 || Page == null) return new int[0];

            var allX = new int[count];
            var allY = new int[count];

            int row = 0;
            int col = 0;
            int selectedCount = 0;
            foreach (StiComponent c in this.Components)
            {
                if (c.IsSelected)
                {
                    allX[selectedCount] = col;
                    allY[selectedCount] = row;
                    selectedCount++;
                    c.IsSelected = false;
                }

                col++;
                if (col == columnCount)
                {
                    col = 0;
                    row++;
                }
            }

            int leftX, leftY, rightX, rightY;
            FindLeftSelectedElement(allX, out leftX, out rightX);
            FindRightSelectedElement(allY, out leftY, out rightY);
            joinWidth = rightX - leftX + 1;
            joinHeight = rightY - leftY + 1;

            var joinCells = new int[(rightY - leftY + 1) * (rightX - leftX + 1)];
            int indexNewCell = 0;
            for (int index1 = leftY; index1 <= rightY; index1++)
            {
                StiComponent selectCell = null;
                for (int index2 = leftX; index2 <= rightX; index2++)
                {
                    int nomer = index1 * columnCount + index2;
                    selectCell = this.Components[nomer];
                    if (((IStiTableCell)selectCell).Merged)
                    {
                        var parentJoinCell = ((IStiTableCell)selectCell).GetJoinComponentByGuid(((IStiTableCell)selectCell).ParentJoin);
                        ((IStiTableCell)parentJoinCell).Join = false;
                    }

                    joinCells[indexNewCell] = ((IStiTableCell)selectCell).ID;
                    if (index1 == leftY) sumWidth += selectCell.Width;
                    indexNewCell++;
                }
                sumHeight += selectCell.Height;
            }

            return joinCells;
        }

        private int GetCountSelectedCells()
        {
            int count = 0;
            foreach (StiComponent cell in this.Components)
            {
                if (cell.IsSelected)
                    count++;
            }

            return count;
        }

        private void FindLeftSelectedElement(int[] allX, out int leftX, out int rightX)
        {
            leftX = allX[0];
            rightX = allX[0];

            for (int index = 1; index < allX.Length; index++)
            {
                if (leftX > allX[index])
                    leftX = allX[index];
                if (rightX < allX[index])
                    rightX = allX[index];
            }
        }

        private void FindRightSelectedElement(int[] allY, out int leftY, out int rightY)
        {
            leftY = allY[0];
            rightY = allY[0];

            for (int index = 1; index < allY.Length; index++)
            {
                if (leftY > allY[index])
                    leftY = allY[index];
                if (rightY < allY[index])
                    rightY = allY[index];
            }
        }
        #endregion

        #region Methods.TableCell.Content
        internal void ChangeTableCellContentInImage(StiTableCell cellText)
        {
            int indexCell = Components.IndexOf(cellText);
            if (indexCell == -1) return;

            var cellImage = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellImage") as StiTableCellImage;

            #region Copy Properties
            cellImage.ClientRectangle = cellText.ClientRectangle;
            cellImage.Name = cellText.Name;
            cellImage.Border = cellText.Border;
            cellImage.Brush = cellText.Brush;
            cellImage.ID = cellText.ID;

            cellImage.Restrictions = cellText.Restrictions;
            cellImage.Page = cellText.Page;
            cellImage.Parent = cellText.Parent;

            cellImage.CanBreak = cellText.CanBreak;
            cellImage.CanGrow = cellText.CanGrow;
            cellImage.CanShrink = cellText.CanShrink;
            cellImage.Enabled = cellText.Enabled;
            cellImage.GrowToHeight = cellText.GrowToHeight;
            cellImage.Printable = cellText.Printable;

            if (cellText.Text.Value.Length != 0)
                cellImage.DataColumn = cellText.Text.Value.Substring(1, cellText.Text.Value.Length - 2);

            if (cellText.Join)
            {
                cellImage.SetJoin(cellText.Join);
                cellImage.ParentJoin = cellText.ParentJoin;
                cellImage.JoinCells = cellText.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellImage);
            cellImage.Select();
        }

        internal void ChangeTableCellContentInImage(StiTableCellCheckBox cellCheckBox)
        {
            int indexCell = Components.IndexOf(cellCheckBox);
            if (indexCell == -1) return;

            var cellImage = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellImage") as StiTableCellImage;

            #region Copy Properties
            cellImage.ClientRectangle = cellCheckBox.ClientRectangle;
            cellImage.Name = cellCheckBox.Name;
            cellImage.Border = cellCheckBox.Border;
            cellImage.Brush = cellCheckBox.Brush;
            cellImage.ID = cellCheckBox.ID;

            cellImage.Restrictions = cellCheckBox.Restrictions;
            cellImage.Page = cellCheckBox.Page;
            cellImage.Parent = cellCheckBox.Parent;

            cellImage.CanGrow = cellCheckBox.CanGrow;
            cellImage.CanShrink = cellCheckBox.CanShrink;
            cellImage.Enabled = cellCheckBox.Enabled;
            cellImage.GrowToHeight = cellCheckBox.GrowToHeight;
            cellImage.Printable = cellCheckBox.Printable;

            if (cellCheckBox.Checked.Value.Length != 0)
                cellImage.DataColumn = cellCheckBox.Checked.Value.Substring(1, cellCheckBox.Checked.Value.Length - 2);

            if (cellCheckBox.join)
            {
                cellImage.SetJoin(cellCheckBox.Join);
                cellImage.ParentJoin = cellCheckBox.ParentJoin;
                cellImage.JoinCells = cellCheckBox.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellImage);
            cellImage.Select();
        }

        internal void ChangeTableCellContentInImage(StiTableCellRichText cellRichText)
        {
            int indexCell = Components.IndexOf(cellRichText);
            if (indexCell == -1) return;

            var cellImage = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellImage") as StiTableCellImage;

            #region Copy Properties
            cellImage.ClientRectangle = cellRichText.ClientRectangle;
            cellImage.Name = cellRichText.Name;
            cellImage.Border = cellRichText.Border;
            cellImage.Brush = new StiSolidBrush(cellRichText.BackColor);
            cellImage.ID = cellRichText.ID;

            cellImage.Restrictions = cellRichText.Restrictions;
            cellImage.Page = cellRichText.Page;
            cellImage.Parent = cellRichText.Parent;

            cellImage.CanGrow = cellRichText.CanGrow;
            cellImage.CanShrink = cellRichText.CanShrink;
            cellImage.Enabled = cellRichText.Enabled;
            cellImage.GrowToHeight = cellRichText.GrowToHeight;
            cellImage.Printable = cellRichText.Printable;

            if (cellRichText.DataColumn.Length != 0)
                cellImage.DataColumn = cellRichText.DataColumn;

            if (cellRichText.Join)
            {
                cellImage.SetJoin(cellRichText.Join);
                cellImage.ParentJoin = cellRichText.ParentJoin;
                cellImage.JoinCells = cellRichText.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellImage);
            cellImage.Select();
        }

        internal void ChangeTableCellContentInText(StiTableCellImage cellImage)
        {
            int indexCell = Components.IndexOf(cellImage);
            if (indexCell == -1) return;

            var cellText = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;

            #region Copy Properties
            cellText.ClientRectangle = cellImage.ClientRectangle;
            cellText.Name = cellImage.Name;
            cellText.Border = cellImage.Border;
            cellText.Brush = cellImage.Brush;
            cellText.ID = cellImage.ID;

            cellText.Restrictions = cellImage.Restrictions;
            cellText.Page = cellImage.Page;
            cellText.Parent = cellImage.Parent;

            cellText.CanBreak = cellImage.CanBreak;
            cellText.CanGrow = cellImage.CanGrow;
            cellText.CanShrink = cellImage.CanShrink;
            cellText.Enabled = cellImage.Enabled;
            cellText.GrowToHeight = cellImage.GrowToHeight;
            cellText.Printable = cellImage.Printable;

            if (cellImage.DataColumn.Length != 0)
                cellText.Text.Value = "{" + cellImage.DataColumn + "}";

            if (cellImage.Join)
            {
                cellText.SetJoin(cellImage.Join);
                cellText.ParentJoin = cellImage.ParentJoin;
                cellText.JoinCells = cellImage.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellText);
            cellText.Select();
        }

        internal void ChangeTableCellContentInText(StiTableCellCheckBox cellCheckBox)
        {
            int indexCell = Components.IndexOf(cellCheckBox);
            if (indexCell == -1) return;

            var cellText = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;

            #region Copy Properties
            cellText.ClientRectangle = cellCheckBox.ClientRectangle;
            cellText.Name = cellCheckBox.Name;
            cellText.Border = cellCheckBox.Border;
            cellText.Brush = cellCheckBox.Brush;
            cellText.ID = cellCheckBox.ID;

            cellText.Restrictions = cellCheckBox.Restrictions;
            cellText.Page = cellCheckBox.Page;
            cellText.Parent = cellCheckBox.Parent;

            cellText.CanGrow = cellCheckBox.CanGrow;
            cellText.CanShrink = cellCheckBox.CanShrink;
            cellText.Enabled = cellCheckBox.Enabled;
            cellText.GrowToHeight = cellCheckBox.GrowToHeight;
            cellText.Printable = cellCheckBox.Printable;

            if (cellCheckBox.Checked.Value.Length != 0)
                cellText.Text.Value = cellCheckBox.Checked.Value;

            if (cellCheckBox.Join)
            {
                cellText.SetJoin(cellCheckBox.Join);
                cellText.ParentJoin = cellCheckBox.ParentJoin;
                cellText.JoinCells = cellCheckBox.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellText);
            cellText.Select();
        }

        internal void ChangeTableCellContentInText(StiTableCellRichText cellRichText)
        {
            int indexCell = Components.IndexOf(cellRichText);
            if (indexCell == -1) return;

            var cellText = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;

            #region Copy Properties
            cellText.ClientRectangle = cellRichText.ClientRectangle;
            cellText.Name = cellRichText.Name;
            cellText.Border = cellRichText.Border;
            cellText.Brush = new StiSolidBrush(cellRichText.BackColor);
            cellText.ID = cellRichText.ID;

            cellText.Restrictions = cellRichText.Restrictions;
            cellText.Page = cellRichText.Page;
            cellText.Parent = cellRichText.Parent;

            cellText.CanGrow = cellRichText.CanGrow;
            cellText.CanShrink = cellRichText.CanShrink;
            cellText.Enabled = cellRichText.Enabled;
            cellText.GrowToHeight = cellRichText.GrowToHeight;
            cellText.Printable = cellRichText.Printable;

            if (cellRichText.DataColumn.Length != 0)
                cellText.Text.Value = $"{{{cellRichText.DataColumn}}}";

            if (cellRichText.Join)
            {
                cellText.SetJoin(cellRichText.Join);
                cellText.ParentJoin = cellRichText.ParentJoin;
                cellText.JoinCells = cellRichText.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellText);
            cellText.Select();
        }

        internal void ChangeTableCellContentInCheckBox(StiTableCellImage cellImage)
        {
            int indexCell = Components.IndexOf(cellImage);
            if (indexCell == -1) return;

            var cellCheckBox = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellCheckBox") as StiTableCellCheckBox;

            #region Copy Properties
            cellCheckBox.ClientRectangle = cellImage.ClientRectangle;
            cellCheckBox.Name = cellImage.Name;
            cellCheckBox.Border = cellImage.Border;
            cellCheckBox.Brush = cellImage.Brush;
            cellCheckBox.ID = cellImage.ID;

            cellCheckBox.Restrictions = cellImage.Restrictions;
            cellCheckBox.Page = cellImage.Page;
            cellCheckBox.Parent = cellImage.Parent;

            cellCheckBox.CanGrow = cellImage.CanGrow;
            cellCheckBox.CanShrink = cellImage.CanShrink;
            cellCheckBox.Enabled = cellImage.Enabled;
            cellCheckBox.GrowToHeight = cellImage.GrowToHeight;
            cellCheckBox.Printable = cellImage.Printable;

            if (cellImage.DataColumn.Length != 0)
                cellCheckBox.Checked.Value = "{" + cellImage.DataColumn + "}";

            if (cellImage.Join)
            {
                cellCheckBox.SetJoin(cellImage.Join);
                cellCheckBox.ParentJoin = cellImage.ParentJoin;
                cellCheckBox.JoinCells = cellImage.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellCheckBox);
            cellCheckBox.Select();
        }

        internal void ChangeTableCellContentInCheckBox(StiTableCell cellText)
        {
            int indexCell = Components.IndexOf(cellText);
            if (indexCell == -1) return;

            var cellCheckBox = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellCheckBox") as StiTableCellCheckBox;

            #region Copy Properties
            cellCheckBox.ClientRectangle = cellText.ClientRectangle;
            cellCheckBox.Name = cellText.Name;
            cellCheckBox.Border = cellText.Border;
            cellCheckBox.Brush = cellText.Brush;
            cellCheckBox.ID = cellText.ID;

            cellCheckBox.Restrictions = cellText.Restrictions;
            cellCheckBox.Page = cellText.Page;
            cellCheckBox.Parent = cellText.Parent;

            cellCheckBox.CanGrow = cellText.CanGrow;
            cellCheckBox.CanShrink = cellText.CanShrink;
            cellCheckBox.Enabled = cellText.Enabled;
            cellCheckBox.GrowToHeight = cellText.GrowToHeight;
            cellCheckBox.Printable = cellText.Printable;

            if (cellText.Text.Value.Length != 0)
                cellCheckBox.Checked.Value = cellText.Text.Value;

            if (cellText.Join)
            {
                cellCheckBox.SetJoin(cellText.Join);
                cellCheckBox.ParentJoin = cellText.ParentJoin;
                cellCheckBox.JoinCells = cellText.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellCheckBox);
            cellCheckBox.Select();
        }

        internal void ChangeTableCellContentInCheckBox(StiTableCellRichText cellRichText)
        {
            int indexCell = Components.IndexOf(cellRichText);
            if (indexCell == -1) return;

            var cellCheckBox = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellCheckBox") as StiTableCellCheckBox;

            #region Copy Properties
            cellCheckBox.ClientRectangle = cellRichText.ClientRectangle;
            cellCheckBox.Name = cellRichText.Name;
            cellCheckBox.Border = cellRichText.Border;
            cellCheckBox.Brush = new StiSolidBrush(cellRichText.BackColor);
            cellCheckBox.ID = cellRichText.ID;

            cellCheckBox.Restrictions = cellRichText.Restrictions;
            cellCheckBox.Page = cellRichText.Page;
            cellCheckBox.Parent = cellRichText.Parent;

            cellCheckBox.CanGrow = cellRichText.CanGrow;
            cellCheckBox.CanShrink = cellRichText.CanShrink;
            cellCheckBox.Enabled = cellRichText.Enabled;
            cellCheckBox.GrowToHeight = cellRichText.GrowToHeight;
            cellCheckBox.Printable = cellRichText.Printable;

            if (cellRichText.DataColumn.Length != 0)
                cellCheckBox.Checked.Value = "{" + cellRichText.DataColumn + "}";

            if (cellRichText.Join)
            {
                cellCheckBox.join = cellRichText.Join;
                cellCheckBox.ParentJoin = cellRichText.ParentJoin;
                cellCheckBox.JoinCells = cellRichText.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellCheckBox);
            cellCheckBox.Select();
        }

        internal void ChangeTableCellContentInRichText(StiTableCell cellText)
        {
            int indexCell = Components.IndexOf(cellText);
            if (indexCell == -1) return;

            var cellRichText = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellRichText") as StiTableCellRichText;

            #region Copy Properties
            cellRichText.ClientRectangle = cellText.ClientRectangle;
            cellRichText.Name = cellText.Name;
            cellRichText.Border = cellText.Border;
            cellRichText.BackColor = StiBrush.ToColor(cellText.Brush);
            cellRichText.ID = cellText.ID;

            cellRichText.Restrictions = cellText.Restrictions;
            cellRichText.Page = cellText.Page;
            cellRichText.Parent = cellText.Parent;

            cellRichText.CanBreak = cellText.CanBreak;
            cellRichText.CanGrow = cellText.CanGrow;
            cellRichText.CanShrink = cellText.CanShrink;
            cellRichText.Enabled = cellText.Enabled;
            cellRichText.GrowToHeight = cellText.GrowToHeight;
            cellRichText.Printable = cellText.Printable;

            if (cellText.Text.Value.Length != 0)
                cellRichText.DataColumn = cellText.Text.Value.Substring(1, cellText.Text.Value.Length - 2);

            if (cellText.Join)
            {
                cellRichText.SetJoin(cellText.Join);
                cellRichText.ParentJoin = cellText.ParentJoin;
                cellRichText.JoinCells = cellText.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellRichText);
            cellRichText.Select();
        }

        internal void ChangeTableCellContentInRichText(StiTableCellImage cellImage)
        {
            int indexCell = Components.IndexOf(cellImage);
            if (indexCell == -1) return;

            var cellRichText = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellRichText") as StiTableCellRichText;

            #region Copy Properties
            cellRichText.ClientRectangle = cellImage.ClientRectangle;
            cellRichText.Name = cellImage.Name;
            cellRichText.Border = cellImage.Border;
            cellRichText.BackColor = StiBrush.ToColor(cellImage.Brush);
            cellRichText.ID = cellImage.ID;

            cellRichText.Restrictions = cellImage.Restrictions;
            cellRichText.Page = cellImage.Page;
            cellRichText.Parent = cellImage.Parent;

            cellRichText.CanBreak = cellImage.CanBreak;
            cellRichText.CanGrow = cellImage.CanGrow;
            cellRichText.CanShrink = cellImage.CanShrink;
            cellRichText.Enabled = cellImage.Enabled;
            cellRichText.GrowToHeight = cellImage.GrowToHeight;
            cellRichText.Printable = cellImage.Printable;

            if (cellImage.DataColumn.Length != 0)
                cellRichText.DataColumn = cellImage.DataColumn;

            if (cellImage.Join)
            {
                cellRichText.SetJoin(cellImage.Join);
                cellRichText.ParentJoin = cellImage.ParentJoin;
                cellRichText.JoinCells = cellImage.JoinCells;
            }
            #endregion

            this.Components.RemoveAt(indexCell);
            this.Components.Insert(indexCell, cellRichText);
            cellRichText.Select();
        }

        internal void ChangeTableCellContentInRichText(StiTableCellCheckBox cellCheckBox)
        {
            int indexCell = Components.IndexOf(cellCheckBox);
            if (indexCell == -1) return;

            var cellRichText = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCellRichText") as StiTableCellRichText;

            #region Copy Properties
            cellRichText.ClientRectangle = cellCheckBox.ClientRectangle;
            cellRichText.Name = cellCheckBox.Name;
            cellRichText.Border = cellCheckBox.Border;
            cellRichText.BackColor = StiBrush.ToColor(cellCheckBox.Brush);
            cellRichText.ID = cellCheckBox.ID;

            cellRichText.Restrictions = cellCheckBox.Restrictions;
            cellRichText.Page = cellCheckBox.Page;
            cellRichText.Parent = cellCheckBox.Parent;

            cellRichText.CanGrow = cellCheckBox.CanGrow;
            cellRichText.CanShrink = cellCheckBox.CanShrink;
            cellRichText.Enabled = cellCheckBox.Enabled;
            cellRichText.GrowToHeight = cellCheckBox.GrowToHeight;
            cellRichText.Printable = cellCheckBox.Printable;

            if (cellCheckBox.Checked.Value.Length != 0)
                cellRichText.DataColumn = cellCheckBox.Checked.Value.Substring(1, cellCheckBox.Checked.Value.Length - 2);

            if (cellCheckBox.join)
            {
                cellRichText.SetJoin(cellCheckBox.Join);
                cellRichText.ParentJoin = cellCheckBox.ParentJoin;
                cellRichText.JoinCells = cellCheckBox.JoinCells;
            }
            #endregion

            Components.RemoveAt(indexCell);
            Components.Insert(indexCell, cellRichText);
            cellRichText.Select();
        }
        #endregion

        #region Methods.CreateCells
        private void SetCellID(IStiTableCell cell)
        {
            cell.ID = NumberID;
            NumberID++;
        }

        public void CreateCell()
        {
            if (Page == null || Parent == null || Page.Report == null)
                return;

            var rect = this.ClientRectangle;
            if (rect.Width < 0) return;

            #region Set Height
            base.Height = rowCount * DefaultHeightCell;
            rect = this.ClientRectangle;
            #endregion

            if (this.Components.Count != 0) this.Components.Clear();

            double w = rect.Width / columnCount;
            int indexW = (int)(w / Page.GridSize);
            double h = rect.Height / rowCount;
            int indexH = (int)(h / Page.GridSize);

            double allWidth = indexW * Page.GridSize;
            double lastWidth = rect.Width - allWidth * (columnCount - 1);
            double allHeight = indexH * Page.GridSize;
            double lastHeight = rect.Height - allHeight * (rowCount - 1);

            double posX = 0;
            int index = 1;
            for (int index1 = 0; index1 < rowCount; index1++)
            {
                for (int index2 = 0; index2 < columnCount; index2++)
                {
                    var cell = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;
                    SetCellID(cell);
                    cell.Name = $"{this.Name}_Cell{index}";

                    double cellWidth = index2 == columnCount - 1 ? lastWidth : allWidth;
                    double cellHeight = index1 == rowCount - 1 ? lastHeight : allHeight;
                    cell.ClientRectangle = new RectangleD(posX, allHeight * index1, cellWidth, cellHeight);
                    SetStyleForCell(cell, false);

                    posX = cell.Right;
                    this.Components.Add(cell);
                    index++;
                }
                posX = 0;
            }

            ResizeHeightCell();
            RefreshTableStyle();
        }

        private void SetStyleForCell(StiTableCell cell, bool select)
        {
            cell.Restrictions = StiRestrictions.None 
                                | StiRestrictions.AllowMove
                                | StiRestrictions.AllowSelect
                                | StiRestrictions.AllowChange;
            cell.Brush = new StiSolidBrush(Color.White);
            cell.Font = new Font("Arial", 9f);
            cell.IsSelected = select;
            cell.CanGrow = false;
            cell.GrowToHeight = false;
            cell.Page = this.Page;
            cell.Parent = this;
        }

        private void AddNewRows(int count)
        {
            if (Components.Count == 0) return;

            int numberCell = this.Components.Count + 1;
            var rect = this.ClientRectangle;
            rect.Height += DefaultHeightCell * count;
            base.ClientRectangle = rect;

            for (int index1 = 0; index1 < count; index1++)
            {
                for (int index2 = 0; index2 < columnCount; index2++)
                {
                    var newCell = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;
                    var topCell = Components[Components.Count - columnCount];
                    newCell.Name = $"{this.Name}_Cell{numberCell}";
                    SetCellID(newCell);
                    double newLeft = ((IStiTableCell)topCell).GetRealLeft();
                    double newWidth = ((IStiTableCell)topCell).GetRealWidth();
                    newCell.ClientRectangle = new RectangleD(newLeft, topCell.Bottom, newWidth, this.DefaultHeightCell);
                    SetStyleForCell(newCell, true);
                    this.Components.Add(newCell);
                    numberCell++;
                }
            }
            RefreshTableStyle();
        }

        private void DeleteLastRows(int count, int oldValue)
        {
            if (Components.Count == 0) return;

            double sumHeight = 0;
            for (int index1 = oldValue - count; index1 < oldValue; index1++)
            {
                if (index1 * columnCount >= this.Components.Count) break; //fix
                for (int index2 = 0; index2 < columnCount; index2++)
                {
                    var cell = this.Components[index1 * columnCount + index2];
                    if (((IStiTableCell)cell).Merged)
                    {
                        var parentJoinCell = ((IStiTableCell)cell).GetJoinComponentByGuid(((IStiTableCell)cell).ParentJoin);
                        ((IStiTableCell)parentJoinCell).Join = false;
                    }
                }
                sumHeight += this.Components[index1 * columnCount].Height;
            }

            int newCount = (oldValue - count) * columnCount;
            while (this.Components.Count > newCount)
            {
                this.Components.RemoveAt(this.Components.Count - 1);
            }

            base.Height -= sumHeight;
            RefreshTableStyle();
        }

        private void AddTableNewColumns(int count, int oldValue)
        {
            if (Components.Count == 0) return;

            var rect = this.ClientRectangle;
            int countGrid = (int)(rect.Width / Page.GridSize);
            int countColumns = columnCount;
            int colGridForCell = countGrid / countColumns;
            int rest = countGrid - colGridForCell * countColumns;

            double[] widths = new double[countColumns];
            for (int index1 = 0; index1 < countColumns; index1++)
            {
                widths[index1] = colGridForCell * Page.GridSize;
                if (index1 < rest)
                    widths[index1] += Page.GridSize;
            }
            widths[countColumns - 1] += rect.Width - countGrid * Page.GridSize;

            int index = 0;
            int indexColumn = 0;
            double posX = 0;
            for (int index1 = 0; index1 < rowCount; index1++)
            {
                for (int index2 = 0; index2 < oldValue; index2++)
                {
                    var cell = this.Components[index];
                    cell.Name = $"{this.Name}_Cell{index + 1}";
                    cell.ClientRectangle = new RectangleD(posX, cell.Top, widths[indexColumn], cell.Height);
                    posX += widths[indexColumn];
                    index++;
                    indexColumn++;

                    if (index2 == oldValue - 1)
                    {
                        double newHeight = ((IStiTableCell)cell).GetRealHeight();
                        double newTop = ((IStiTableCell)cell).GetRealTop();

                        #region Create new Cell
                        for (int index3 = 0; index3 < count; index3++)
                        {
                            var newCell = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;
                            newCell.Name = $"{this.Name}_Cell{index + 1}";
                            SetCellID(newCell);
                            newCell.ClientRectangle = new RectangleD(posX, newTop, widths[indexColumn], newHeight);
                            SetStyleForCell(newCell, true);
                            this.Components.Insert(index, newCell);
                            posX += widths[indexColumn];
                            index++;
                            indexColumn++;
                        }
                        #endregion
                    }
                }
                posX = 0;
                indexColumn = 0;
            }

            for (int index1 = 0; index1 < this.Components.Count; index1++)
            {
                var cells = this.Components[index1];
                if (cells != null && ((IStiTableCell)cells).Join)
                {
                    ((IStiTableCell)cells).SetJoinSize();
                }
            }

            RefreshTableStyle();
        }

        private void DeleteTableColumns(int count, int oldValue)
        {
            if (Components.Count == 0) return;

            int countColumn = oldValue - count;
            int index = 0;
            for (int index1 = 0; index1 < rowCount; index1++)
            {
                index += countColumn;
                for (int index2 = 0; index2 < count; index2++)
                {
                    this.Components.RemoveAt(index);
                }
            }

            var rect = this.ClientRectangle;
            int countGrid = (int)(rect.Width / Page.GridSize);
            int countGridOnCell = countGrid / countColumn;
            int rest = countGrid - countGridOnCell * countColumn;
            double[] widths = new double[countColumn];
            for (int index1 = 0; index1 < countColumn; index1++)
            {
                widths[index1] = countGridOnCell * Page.GridSize;
                if (index1 < rest)
                    widths[index1] += Page.GridSize;
            }

            double posX = 0;
            index = 0;
            for (int index1 = 0; index1 < rowCount; index1++)
            {
                for (int index2 = 0; index2 < countColumn; index2++)
                {
                    var cell = this.Components[index];
                    cell.Name = $"{this.Name}_Cell{index + 1}";
                    cell.Left = posX;
                    cell.Width = widths[index2];

                    posX += widths[index2];
                    index++;
                }
                posX = 0;
            }

            for (int index1 = 0; index1 < this.Components.Count; index1++)
            {
                var cells = this.Components[index1];
                if (cells != null && ((IStiTableCell)cells).Join)
                {
                    ((IStiTableCell)cells).SetJoinSize();
                }
            }

            RefreshTableStyle();
        }

        public void InsertColumnToLeft(int numberColumn)
        {
            if (numberColumn < 0)
                return;
            if (numberColumn != 0)
            {
                int indexCell = numberColumn;
                for (int indexRow = 0; indexRow < this.rowCount; indexRow++)
                {
                    var cell = this.Components[indexCell];
                    var leftCell = this.Components[indexCell - 1];
                    if (((IStiTableCell)cell).Merged && ((IStiTableCell)leftCell).Merged)
                    {
                        var parentJoinCell = ((IStiTableCell)cell).GetJoinComponentByGuid(((IStiTableCell)cell).ParentJoin);
                        if (((IStiTableCell) parentJoinCell).ContainsGuid(((IStiTableCell) leftCell).ID))
                            ((IStiTableCell) parentJoinCell).Join = false;
                    }

                    indexCell += this.columnCount;
                }
            }

            int indexNewCell = numberColumn;
            for (int index = 0; index < this.rowCount; index++)
            {
                var cell = Components[indexNewCell];
                var newCell = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;
                SetCellID(newCell);
                newCell.Top = ((IStiTableCell)cell).GetRealTop();
                newCell.Height = ((IStiTableCell)cell).GetRealHeight();
                SetStyleForCell(newCell, false);
                this.Components.Insert(indexNewCell, newCell);
                indexNewCell++;
                indexNewCell += this.columnCount;
            }
            this.columnCount++;

            ResizeWidthCellsAfterChanges();
            RefreshTableStyle();
        }

        public void InsertColumnToRight(int numberColumn)
        {
            if (numberColumn < 0)
                return;
            if (numberColumn != columnCount - 1)
            {
                int indexCell = numberColumn;
                for (int indexRow = 0; indexRow < this.rowCount; indexRow++)
                {
                    var cell = this.Components[indexCell];
                    var rightCell = this.Components[indexCell + 1];
                    if (((IStiTableCell)cell).Merged && ((IStiTableCell)rightCell).Merged)
                    {
                        var parentJoinCell = ((IStiTableCell)cell).GetJoinComponentByGuid(((IStiTableCell)cell).ParentJoin);
                        if (((IStiTableCell)parentJoinCell).ContainsGuid(((IStiTableCell)rightCell).ID))
                            ((IStiTableCell)parentJoinCell).Join = false;
                    }
                    indexCell += this.columnCount;
                }
            }

            int indexNewCell = numberColumn;
            for (int index = 0; index < this.rowCount; index++)
            {
                var cell = Components[indexNewCell];
                var newCell = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;
                SetCellID(newCell);
                newCell.Top = ((IStiTableCell)cell).GetRealTop();
                newCell.Height = ((IStiTableCell)cell).GetRealHeight();
                SetStyleForCell(newCell, false);
                this.Components.Insert(indexNewCell + 1, newCell);
                indexNewCell++;
                indexNewCell += this.columnCount;
            }
            this.columnCount++;

            ResizeWidthCellsAfterChanges();
            RefreshTableStyle();
        }

        public void InsertRowAbove(int numberRow)
        {
            if (numberRow < 0)
                return;
            if (numberRow != 0)
            {
                int indexCell = numberRow * columnCount;
                for (int indexRow = 0; indexRow < this.columnCount; indexRow++)
                {
                    var cell = this.Components[indexCell];
                    var leftCell = this.Components[indexCell - columnCount];

                    if (((IStiTableCell)cell).Merged && ((IStiTableCell)leftCell).Merged)
                    {
                        var parentJoinCell = ((IStiTableCell)cell).GetJoinComponentByGuid(((IStiTableCell)cell).ParentJoin);
                        if (((IStiTableCell)parentJoinCell).ContainsGuid(((IStiTableCell)leftCell).ID))
                            ((IStiTableCell)parentJoinCell).Join = false;
                    }
                    indexCell++;
                }
            }

            int indexNewCell = numberRow * columnCount;
            for (int index = 0; index < this.columnCount; index++)
            {
                var cell = Components[indexNewCell + index];
                var newCell = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;
                SetCellID(newCell);
                newCell.Left = ((IStiTableCell)cell).GetRealLeft();
                newCell.Width = ((IStiTableCell)cell).GetRealWidth();
                newCell.Height = this.DefaultHeightCell;
                SetStyleForCell(newCell, false);
                this.Components.Insert(indexNewCell, newCell);
                indexNewCell++;
            }
            this.rowCount++;

            ResizeHeightCellsAfterChanges(this.DefaultHeightCell);
            RefreshTableStyle();
        }

        public void InsertRowBelow(int numberRow)
        {
            if (numberRow < 0) return;

            if (numberRow != rowCount)
            {
                int indexCell = numberRow * columnCount;
                for (int indexRow = 0; indexRow < this.columnCount; indexRow++)
                {
                    var cell = this.Components[indexCell];
                    var leftCell = this.Components[indexCell - columnCount];
                    if (((IStiTableCell)cell).Merged && ((IStiTableCell)leftCell).Merged)
                    {
                        var parentJoinCell = ((IStiTableCell)cell).GetJoinComponentByGuid(((IStiTableCell)cell).ParentJoin);
                        if (((IStiTableCell)parentJoinCell).ContainsGuid(((IStiTableCell)leftCell).ID))
                            ((IStiTableCell)parentJoinCell).Join = false;
                    }
                    indexCell++;
                }
            }

            int indexNewCell = numberRow * columnCount;
            for (int index = 0; index < this.columnCount; index++)
            {
                var cell = (numberRow == rowCount)
                    ? Components[indexNewCell - columnCount]
                    : Components[indexNewCell + index];

                var newCell = StiActivator.CreateObject("Stimulsoft.Report.Components.Table.StiTableCell") as StiTableCell;
                SetCellID(newCell);
                newCell.Left = ((IStiTableCell)cell).GetRealLeft();
                newCell.Width = ((IStiTableCell)cell).GetRealWidth();
                newCell.Height = this.DefaultHeightCell;
                SetStyleForCell(newCell, false);
                this.Components.Insert(indexNewCell, newCell);
                indexNewCell++;
            }
            this.rowCount++;

            ResizeHeightCellsAfterChanges(this.DefaultHeightCell);
            RefreshTableStyle();
        }

        public List<StiComponent> DeleteRows(int firstRow, int lastRow)
        {
            //remove all rows
            int countDeleteRows = lastRow - firstRow + 1;
            if (countDeleteRows == rowCount)
                return null;

            var removeComps = new List<StiComponent>();

            //delete rows
            int numberCell = firstRow * columnCount;
            double sumHeight = 0;
            for (int indexRow = firstRow; indexRow <= lastRow; indexRow++)
            {
                for (int indexCol = 0; indexCol < columnCount; indexCol++)
                {
                    var cell = this.Components[numberCell];
                    if (cell != null)
                    {
                        if (((IStiTableCell)cell).Merged)
                            ((IStiTableCell)((IStiTableCell)cell).GetJoinComponentByGuid(((IStiTableCell)cell).ParentJoin)).Join = false;

                        if (indexCol == 0) sumHeight += ((IStiTableCell)cell).GetRealHeight();
                        this.Components.RemoveAt(numberCell);

                        removeComps.Add(cell);
                    }
                }
            }
            this.rowCount -= countDeleteRows;

            ResizeHeightCellsAfterChanges(sumHeight * -1);
            RefreshTableStyle();

            return removeComps;
        }

        public List<StiComponent> DeleteColumns(int firstColumn, int lastColumn)
        {
            //remove all columns
            int countDeleteColumns = lastColumn - firstColumn + 1;
            if (countDeleteColumns == columnCount) return null;

            var removeComps = new List<StiComponent>();

            //delete columns
            int numberCell = firstColumn;
            for (int indexRow = 0; indexRow < rowCount; indexRow++)
            {
                for (int indexCol = firstColumn; indexCol <= lastColumn; indexCol++)
                {
                    var cell = this.Components[numberCell];
                    if (cell != null)
                    {
                        if (((IStiTableCell)cell).Merged)
                            ((IStiTableCell)((IStiTableCell)cell).GetJoinComponentByGuid(((IStiTableCell)cell).ParentJoin)).Join = false;
                        this.Components.RemoveAt(numberCell);
                        removeComps.Add(cell);
                    }
                }
                numberCell += columnCount - countDeleteColumns;
            }
            this.columnCount -= countDeleteColumns;

            ResizeWidthCellsAfterChanges();
            RefreshTableStyle();

            return removeComps;
        }
        #endregion

        #region Methods.ResizeCells
        public void DistributeRows()
        {
            if (IsConverted || this.Parent == null && this.Page == null && this.Components.Count == 0) return;

            var rect = this.ClientRectangle;
            double gridCountHeight = rect.Height / Page.GridSize;
            double cellGridCount = gridCountHeight / rowCount;
            int restHeight = (int)(gridCountHeight - cellGridCount * rowCount);

            var heights = new double[rowCount];
            for (int indexRow = 0; indexRow < rowCount; indexRow++)
            {
                heights[indexRow] = cellGridCount * Page.GridSize;
                if (indexRow < restHeight)
                    heights[indexRow] += Page.GridSize;
            }
            heights[heights.Length - 1] += rect.Height - gridCountHeight * Page.GridSize;

            double posY = 0;
            int indexCell = 0;
            for (int indexRow = 0; indexRow < rowCount; indexRow++)
            {
                for (int indexCol = 0; indexCol < columnCount; indexCol++)
                {
                    var cell = this.Components[indexCell];
                    if (cell != null)
                    {
                        cell.ClientRectangle = new RectangleD(((IStiTableCell)cell).GetRealLeft(), posY, ((IStiTableCell)cell).GetRealWidth(), heights[indexRow]);
                        if (((IStiTableCell)cell).Join) ((IStiTableCell)cell).SetJoinSize();
                        indexCell++;
                    }
                }
                posY += heights[indexRow];
            }
        }

        public void DistributeColumns()
        {
            if (IsConverted || this.Parent == null && this.Page == null && this.Components.Count == 0) return;

            var rect = this.ClientRectangle;
            double gridCountWidth = rect.Width / Page.GridSize;
            double cellGridCount = gridCountWidth / columnCount;
            int restWidth = (int)(gridCountWidth - cellGridCount * columnCount);
            var widths = new double[columnCount];
            for (int indexCol = 0; indexCol < columnCount; indexCol++)
            {
                widths[indexCol] = cellGridCount * Page.GridSize;
                if (indexCol < restWidth)
                    widths[indexCol] += Page.GridSize;
            }
            widths[widths.Length - 1] += rect.Width - gridCountWidth * Page.GridSize;

            double posX = 0;
            int indexCell = 0;
            for (int indexRow = 0; indexRow < rowCount; indexRow++)
            {
                for (int indexCol = 0; indexCol < columnCount; indexCol++)
                {
                    var cell = this.Components[indexCell];
                    if (cell != null)
                    {
                        cell.ClientRectangle = new RectangleD(posX, ((IStiTableCell)cell).GetRealTop(), widths[indexCol], ((IStiTableCell)cell).GetRealHeight());
                        posX += widths[indexCol];
                        if (((IStiTableCell)cell).Join) ((IStiTableCell)cell).SetJoinSize();
                        indexCell++;
                    }
                }
                posX = 0;
            }
        }

        public void AutoSizeCells()
        {
            if (IsConverted || Components.Count == 0 || Page == null) return;

            var rect = this.ClientRectangle;
            int gridCountWidth = (int)(rect.Width / Page.GridSize);
            int cellGridCount = gridCountWidth / columnCount;
            int restWidth = gridCountWidth - cellGridCount * columnCount;
            var widths = new double[columnCount];
            for (int indexCol = 0; indexCol < columnCount; indexCol++)
            {
                widths[indexCol] = cellGridCount * Page.GridSize;
                if (indexCol < restWidth)
                    widths[indexCol] += Page.GridSize;
            }
            widths[widths.Length - 1] += rect.Width - gridCountWidth * Page.GridSize;

            int gridCountHeight = (int)(rect.Height/ Page.GridSize);
            cellGridCount = gridCountHeight / rowCount;
            int restHeight = gridCountHeight - cellGridCount * rowCount;
            var heights = new double[rowCount];
            for (int indexRow = 0; indexRow < rowCount; indexRow++)
            {
                heights[indexRow] = cellGridCount * Page.GridSize;
                if (indexRow < restHeight)
                    heights[indexRow] += Page.GridSize;
            }
            heights[heights.Length - 1] += rect.Height - gridCountHeight * Page.GridSize;

            double posX = 0;
            double posY = 0;
            int indexCell = 0;
            for (int indexRow = 0; indexRow < rowCount; indexRow++)
            {
                for (int indexCol = 0; indexCol < columnCount; indexCol++)
                {
                    var cell = this.Components[indexCell];
                    cell.ClientRectangle = new RectangleD(posX, posY, widths[indexCol], heights[indexRow]);
                    posX += widths[indexCol];
                    indexCell++;
                }
                posX = 0;
                posY += heights[indexRow];
            }
        }

        private void ResizeWidthCellsAfterChanges()
        {
            if (IsConverted) return;

            var rect = this.ClientRectangle;
            int countGrid = (int)(rect.Width / Page.GridSize);
            int colGridForCell = countGrid / this.columnCount;
            int rest = countGrid - colGridForCell * this.columnCount;

            var widths = new double[this.columnCount];
            for (int index1 = 0; index1 < this.columnCount; index1++)
            {
                widths[index1] = colGridForCell * Page.GridSize;
                if (index1 < rest)
                    widths[index1] += Page.GridSize;
            }
            widths[this.columnCount - 1] += rect.Width - countGrid * Page.GridSize;
            
            int index = 0;
            int indexColumn = 0;
            double posX = 0;
            for (int index1 = 0; index1 < this.rowCount; index1++)
            {
                for (int index2 = 0; index2 < this.columnCount; index2++)
                {
                    var cell = this.Components[index];
                    cell.Name = $"{this.Name}_Cell{index + 1}";
                    if (((IStiTableCell)cell).Join)
                    {
                        cell.Left = posX;
                        cell.Width = widths[indexColumn];
                        ((IStiTableCell)cell).SetJoinSize();
                    }
                    else
                    {
                        cell.ClientRectangle = new RectangleD(posX, cell.Top, widths[indexColumn], cell.Height);
                    }

                    posX += widths[indexColumn];
                    index++;
                    indexColumn++;
                }
                indexColumn = 0;
                posX = 0;
            }
        }

        private void ResizeHeightCellsAfterChanges(double changeHeight)
        {
            if (IsConverted) return;

            var rect = this.ClientRectangle;
            rect.Height += changeHeight;

            double posY = 0;
            int indexCell = 0;
            StiComponent cell = null;
            for (int indexRow = 0; indexRow < this.rowCount; indexRow++)
            {
                for (int indexCol = 0; indexCol < this.columnCount; indexCol++)
                {
                    cell = this.Components[indexCell];
                    cell.Name = $"{this.Name}_Cell{indexCell + 1}";
                    if (((IStiTableCell)cell).Join)
                    {
                        cell.Height = ((IStiTableCell)cell).GetRealHeightAfterInsertRows();
                        cell.Top = posY;
                        ((IStiTableCell)cell).SetJoinSize();
                    }
                    else
                    {
                        cell.Top = posY;
                    }
                    
                    indexCell++;
                }
                if (cell != null)
                    posY += ((IStiTableCell)cell).GetRealHeight();
            }

            base.ClientRectangle = rect;
        }

        private void ResizeWidthCell(double oldWidth)
        {
            if (IsConverted || this.Components.Count == 0 || Page == null)
                return;

            double difference = base.Width - oldWidth;

            double w = difference / columnCount;
            int index = (int)(w / Page.GridSize);
            double allWidth = Page.GridSize * index;
            double lastWidth = difference - allWidth * (columnCount - 1);

            for (int index1 = 0; index1 < columnCount; index1++)
            {
                for (int index2 = 0; index2 < rowCount; index2++)
                {
                    var cell = this.Components[index2 * columnCount + index1];
                    if (index1 == columnCount - 1)
                    {
                        cell.Left += index1 * allWidth;
                        cell.Width += lastWidth;
                    }
                    else
                    {
                        cell.Left += index1 * allWidth;
                        cell.Width += allWidth;
                    }
                }
            }
        }

        private void ResizeHeightCell()
        {
            if (IsConverted || Page == null && this.Components.Count == 0)
                return;

            var rect = this.ClientRectangle;
            if (rowCount == 1)
            {
                for (int index1 = 0; index1 < columnCount; index1++)
                {
                    var cell = this.Components[index1];
                    cell.Top = 0;
                    cell.Height = rect.Height;
                }
            }
            else
            {
                var heights = new double[rowCount];
                int countGrid = (int)(rect.Height / Page.GridSize);
                int indexH = countGrid / rowCount;
                int rest = countGrid - indexH * rowCount;
                double allHeight = indexH * Page.GridSize;

                for (int index1 = 0; index1 < rowCount; index1++)
                {
                    heights[index1] = allHeight;
                    if (index1 < rest)
                        heights[index1] += Page.GridSize;
                }
                heights[rowCount - 1] += rect.Height - this.Page.GridSize * countGrid;

                double posY = 0;
                for (int index1 = 0; index1 < rowCount; index1++)
                {
                    for (int index2 = 0; index2 < columnCount; index2++)
                    {
                        var cell = this.Components[index1 * columnCount + index2];
                        if (((IStiTableCell)cell).Join)
                        {
                            var firstCell = ((IStiTableCell)cell).GetJoinComponentByIndex(0);
                            cell.Left = firstCell.Left;
                            cell.Top = firstCell.Top;
                            cell.Height = posY + heights[index1] - firstCell.Top;
                        }
                        else
                        {
                            cell.Top = posY;
                            cell.Height = heights[index1];
                        }
                    }
                    posY += heights[index1];
                }
            }
        }
        #endregion

        #region Methods.Rendering
        internal StiDataBand StartRenderTableBand(ref Hashtable newTableComponents)
        {
            if (!Report.IsInteractionRendering)
            {
                if (!this.Enabled) return null;
            }

            var page = this.Parent as StiPage;
            var dataBand = this.Parent as StiDataBand;
            var panel = this.Parent as StiPanel;

            if (this.DockableTable)
            {
                if (dataBand != null)
                {
                    var panelInDataBand = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPanel") as StiPanel;
                    panelInDataBand.Name = this.Name + "_Panel";
                    panelInDataBand.CanBreak = true;
                    panelInDataBand.ClientRectangle = dataBand.ClientRectangle;
                    panelInDataBand.DockStyle = StiDockStyle.Fill;
                    int indexTable = dataBand.Components.IndexOf(this);
                    dataBand.Components.Insert(indexTable, panelInDataBand);
                    return StartRenderTable(panelInDataBand, 0, ref newTableComponents);
                }
                else if (page != null)
                {
                    int index = this.Page.Components.IndexOf(this);
                    return StartRenderTable(page, index, ref newTableComponents);
                }
                else if (panel != null)
                {
                    int index = panel.Components.IndexOf(this);
                    return StartRenderTable(panel, index, ref newTableComponents);
                }
            }
            else
            {
                if (dataBand != null)
                {
                    var panelInDataBand = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPanel") as StiPanel;
                    panelInDataBand.Name = this.Name + "_Panel";
                    panelInDataBand.CanBreak = true;
                    panelInDataBand.CanGrow = true;
                    panelInDataBand.ClientRectangle = this.ClientRectangle;
                    int indexTable = dataBand.Components.IndexOf(this);
                    dataBand.Components.Insert(indexTable, panelInDataBand);
                    return StartRenderTable(panelInDataBand, 0, ref newTableComponents);
                }
                else if (page != null)
                {
                    int index = this.Page.Components.IndexOf(this);
                    var newBand = StiActivator.CreateObject("Stimulsoft.Report.Components.StiDataBand") as StiDataBand;
                    newBand.Name = this.Name + "_Band";
                    newBand.Left = this.Left;
                    newBand.Top = this.Top;
                    this.Page.Components.Insert(index, newBand);

                    var panelInDataBand = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPanel") as StiPanel;
                    panelInDataBand.Name = this.Name + "_Panel";
                    panelInDataBand.CanBreak = true;
                    panelInDataBand.CanGrow = true;
                    panelInDataBand.ClientRectangle = this.ClientRectangle;
                    panelInDataBand.Top = 0;

                    newBand.Components.Add(panelInDataBand);
                    return StartRenderTable(panelInDataBand, 0, ref newTableComponents);
                }
                else if (panel != null)
                {
                    int index = panel.Parent.Components.IndexOf(panel);
                    return StartRenderTable(panel, index, ref newTableComponents);
                }
            }
            return null;
        }

        private StiDataBand StartRenderTable(StiContainer parentContainer, int index, ref Hashtable newTableComponents)
        {
            ApplyCustomStyle();

            var page = this.Page;
            ArrayList list;
            if (newTableComponents.ContainsKey(page))
            {
                list = newTableComponents[page] as ArrayList;
            }
            else
            {
                list = new ArrayList();
                newTableComponents.Add(page, list);
            }

            base.Enabled = false;
            int lastElementHeader = 0;

            StiDataBand tableData = null;
            var rect = this.ClientRectangle;
            double posY = this.DockableTable ? rect.Y : 0;
            double headerHeight = 0;
            double footerHeight = 0;
            double dataBandHeight = 0;
            int rCount = this.Components.Count / this.columnCount;

            ReverseCells(this.RightToLeft);

            #region SetColumns
            for (int indexRow = 0; indexRow < rowCount; indexRow++)
            {
                for (int indexCol = 0; indexCol < columnCount; indexCol++)
                {
                    (Components[columnCount * indexRow + indexCol] as IStiTableCell).Column = indexCol;
                    ((IStiTableCell)Components[columnCount * indexRow + indexCol]).TableTag = this;
                }
            }
            #endregion

            #region Check for Grouping
            bool isGroupingPresent = false;
            int indexOfTable = this.Parent.Components.IndexOf(this) - 1;
            while (indexOfTable >= 0)
            {
                var comp = this.Parent.Components[indexOfTable];
                if (!(comp is StiHeaderBand))
                {
                    if (comp is StiChildBand) { }
                    else if (comp is StiEmptyBand) { }
                    else if (comp is StiGroupHeaderBand)
                    {
                        isGroupingPresent = true;
                        break;
                    }
                    else break;
                }
                indexOfTable--;
            }
            #endregion

            #region Create HeaderBand
            if (headerRowsCount > 0)
            {
                for (int index1 = 0; index1 < headerRowsCount; index1++)
                {
                    headerHeight += ((IStiTableCell)Components[index1 * columnCount]).GetRealHeight();
                }

                var listCellHeader = new Hashtable();
                var joinCells = new Hashtable();

                #region Create HeaderBand
                StiDynamicBand tableHeader;
                if (isGroupingPresent)
                {
                    tableHeader = StiActivator.CreateObject("Stimulsoft.Report.Components.StiGroupHeaderBand") as StiGroupHeaderBand;
                    ((StiGroupHeaderBand)tableHeader).GroupHeaderBandInfoV2.IsTableGroupHeader = true;
                    tableHeader.Name = this.Name + "_GrHd";
                    ((StiGroupHeaderBand)tableHeader).PrintOnAllPages = this.HeaderPrintOnAllPages;
                }
                else
                {
                    tableHeader = StiActivator.CreateObject("Stimulsoft.Report.Components.StiHeaderBand") as StiHeaderBand;
                    tableHeader.Name = this.Name + "_Hd";
                    ((StiHeaderBand)tableHeader).HeaderBandInfoV2.IsTableHeader = true;
                    ((StiHeaderBand)tableHeader).PrintOnAllPages = this.HeaderPrintOnAllPages;
                    ((StiHeaderBand)tableHeader).PrintIfEmpty = this.HeaderPrintIfEmpty;
                    ((StiHeaderBand)tableHeader).PrintOnEvenOddPages = this.HeaderPrintOnEvenOddPages;
                }

                tableHeader.Height = headerHeight;
                tableHeader.ClientRectangle = new Stimulsoft.Base.Drawing.RectangleD(rect.X, posY, rect.Width, headerHeight);
                tableHeader.Border = this.Border;
                tableHeader.Brush = this.Brush;
                tableHeader.Page = this.Page;
                tableHeader.Parent = this.Page;
                tableHeader.CanGrow = this.HeaderCanGrow;
                tableHeader.CanShrink = this.HeaderCanShrink;
                tableHeader.CanBreak = this.HeaderCanBreak;
                tableHeader.PrintAtBottom = this.HeaderPrintAtBottom;
                tableHeader.PrintOn = this.HeaderPrintOn;

                //copy event with Conditions
                tableHeader.Conditions = this.Conditions.Clone() as StiConditionsCollection;
                CopyEventHandlersToComponent(tableHeader, true);
                #endregion

                double yPosHeaderPanel = 0;
                for (int indexHeader = 0; indexHeader < headerRowsCount; indexHeader++)
                {
                    #region Create panel
                    double __height = ((IStiTableCell)Components[lastElementHeader]).GetRealHeight(); // the header height of this line

                    var panel = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPanel") as StiPanel;
                    panel.ClientRectangle = new RectangleD(rect.X, yPosHeaderPanel, rect.Width, __height);
                    panel.Name = tableHeader.Name + "_HPnl" + indexHeader.ToString();
                    panel.CanGrow = true;
                    panel.CanShrink = true;
                    panel.DockStyle = StiDockStyle.Top;
                    tableHeader.Components.Add(panel);
                    yPosHeaderPanel += __height;
                    #endregion

                    lastElementHeader += columnCount;
                    for (int indexCell = columnCount * indexHeader; indexCell < lastElementHeader; indexCell++)
                    {
                        var tableCell = Components[indexCell];
                        bool join = false;

                        #region Check joined cells
                        StiComponent parentJoin = null;
                        if (((IStiTableCell)tableCell).ParentJoin != -1)
                        {
                            int row = 0;
                            parentJoin = GetParentJoin(((IStiTableCell)tableCell).ParentJoin, ref row);
                            if (listCellHeader.ContainsKey(parentJoin))
                            {
                                if (indexHeader == (int)listCellHeader[parentJoin])
                                {
                                    continue;
                                }
                                else
                                {
                                    var emptyText = StiActivator.CreateObject("Stimulsoft.Report.Components.StiText") as StiText;
                                    emptyText.Page = tableCell.Page;
                                    emptyText.Conditions = parentJoin.Conditions;
                                    CopyEventHandlersToComponent(emptyText, true, parentJoin);
                                    emptyText.SetPaintRectangle(tableCell.GetPaintRectangle());
                                    emptyText.ClientRectangle = new RectangleD(((IStiTableCell)tableCell).GetRealLeft(), 0,
                                        ((IStiTableCell)tableCell).GetRealWidth(), ((IStiTableCell)tableCell).GetRealHeight());
                                    emptyText.Name = tableCell.Name + "_Emp";
                                    emptyText.GrowToHeight = true;
                                    emptyText.Brush = new StiEmptyBrush();
                                    emptyText.DockStyle = ((IStiTableCell)tableCell).CellDockStyle;
                                    panel.Components.Add(emptyText);
                                    list.Add(emptyText);
                                    if (tableCell == parentJoin)
                                    {
                                        ((IStiTableCell)joinCells[parentJoin]).ParentJoinCell = emptyText;
                                    }
                                    continue;
                                }
                            }
                            else
                            {
                                listCellHeader.Add(parentJoin, indexHeader);
                                tableCell = parentJoin;
                                join = true;
                            }
                        }
                        #endregion

                        #region Clone TableCell
                        tableCell.Top = 0;
                        switch (((IStiTableCell)tableCell).CellType)
                        {
                            case StiTablceCellType.Image:
                                var newCellImage = (StiImage)((StiTableCellImage)tableCell).Clone(true);
                                newCellImage.Restrictions = newCellImage.Restrictions ^ StiRestrictions.AllowDelete;
                                newCellImage.DockStyle = ((StiTableCellImage)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newCellImage);
                                list.Add(newCellImage);
                                if (join)
                                {
                                    newCellImage.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newCellImage.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newCellImage);
                                }
                                break;
                            case StiTablceCellType.Text:
                                var newCellText = (StiText)((StiTableCell)tableCell).Clone(true);
                                newCellText.Restrictions = newCellText.Restrictions ^ StiRestrictions.AllowDelete;
                                newCellText.DockStyle = ((StiTableCell)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newCellText);
                                list.Add(newCellText);
                                if (join)
                                {
                                    newCellText.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newCellText.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newCellText);
                                }
                                break;
                            case StiTablceCellType.CheckBox:
                                var newCheckBox = (StiCheckBox)((StiTableCellCheckBox)tableCell).Clone(true);
                                newCheckBox.Restrictions = newCheckBox.Restrictions ^ StiRestrictions.AllowDelete;
                                newCheckBox.DockStyle = ((StiTableCellCheckBox)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newCheckBox);
                                list.Add(newCheckBox);
                                if (join)
                                {
                                    newCheckBox.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newCheckBox.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newCheckBox);
                                }
                                break;
                            case StiTablceCellType.RichText:
                                var newRichText = (StiRichText)((StiTableCellRichText)tableCell).Clone(true);
                                newRichText.Restrictions = newRichText.Restrictions ^ StiRestrictions.AllowDelete;
                                newRichText.DockStyle = ((StiTableCellRichText)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newRichText);
                                list.Add(newRichText);
                                if (join)
                                {
                                    newRichText.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newRichText.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newRichText);
                                }
                                break;
                        }
                        #endregion
                    }
                }
                parentContainer.Components.Insert(index, tableHeader);
                list.Add(tableHeader);
                index++;
                posY += headerHeight + 1;
            }
            else if (isGroupingPresent && footerRowsCount > 0)
            {
                var tableHeaderEmpty = StiActivator.CreateObject("Stimulsoft.Report.Components.StiGroupHeaderBand") as StiGroupHeaderBand;

                tableHeaderEmpty.Height = 0;
                tableHeaderEmpty.ClientRectangle = new Stimulsoft.Base.Drawing.RectangleD(rect.X, posY, rect.Width, 0);
                tableHeaderEmpty.Name = this.Name + "_GrHdEmp";
                tableHeaderEmpty.Page = this.Page;
                tableHeaderEmpty.Parent = this.Page;
                tableHeaderEmpty.PrintAtBottom = this.HeaderPrintAtBottom;

                parentContainer.Components.Insert(index, tableHeaderEmpty);
                list.Add(tableHeaderEmpty);
                index++;
                posY += 1;
            }
            #endregion

            #region Create DataBand
            if (rowCount - headerRowsCount - footerRowsCount > 0)
            {
                this.IsConverted = true;
                tableData = (StiDataBand)this.Clone();

                #region Clone
                tableData.Expressions = this.Expressions?.Clone() as StiAppExpressionCollection;
                tableData.Conditions = this.Conditions?.Clone() as StiConditionsCollection;
                tableData.Interaction = this.Interaction?.Clone() as StiInteraction;

                if (tableData.Interaction != null)
                    tableData.Interaction.ParentComponent = tableData;

                tableData.Properties = this.Properties.Clone() as StiRepositoryItems;

                tableData.Border = this.Border?.Clone() as StiBorder;
                tableData.Brush = this.Brush?.Clone() as StiBrush;

                tableData.Sort = this.Sort?.Clone() as string[];
                tableData.Filters = this.Filters?.Clone() as StiFiltersCollection;

                tableData.OddStyle = this.OddStyle;
                tableData.EvenStyle = this.EvenStyle;
                tableData.DataSourceName = this.DataSourceName;
                tableData.BusinessObjectGuid = this.BusinessObjectGuid;
                tableData.SelectedLine = this.SelectedLine;
                tableData.DataRelationName = this.DataRelationName;

                tableData.FilterMode = this.FilterMode;
                tableData.FilterEngine = this.FilterEngine;
                tableData.FilterOn = this.FilterOn;
                #endregion

                this.IsConverted = false;
                tableData.Components.Clear();
                SetFilter(tableData);

                for (int index1 = headerRowsCount; index1 < rCount - footerRowsCount; index1++)
                {
                    dataBandHeight += ((IStiTableCell)this.Components[index1 * columnCount]).GetRealHeight();
                }

                tableData.Parent = this.Parent;
                tableData.Page = this.Page;
                tableData.Enabled = true;
                tableData.ClientRectangle = new RectangleD(rect.X, posY, rect.Width, dataBandHeight);
                tableData.Name = this.Name + "_DB";
                tableData.MasterComponent = this.MasterComponent;

                if (!string.IsNullOrEmpty(this.ComponentStyle))
                {
                    var tableStyle = this.Report.Styles[this.ComponentStyle] as Stimulsoft.Report.StiTableStyle;
                    if (tableStyle != null)
                    {
                        if (string.IsNullOrEmpty(tableData.OddStyle))
                        {
                            var oddStyle = new StiStyle
                            {
                                Name = tableData.Name + "OddStyle",
                                Brush = new StiSolidBrush(tableStyle.DataColor),
                                TextBrush = new StiSolidBrush(tableStyle.DataForeground)
                            };
                            this.Report.Styles.Add(oddStyle);

                            tableData.OddStyle = oddStyle.Name;
                        }

                        if (string.IsNullOrEmpty(tableData.EvenStyle))
                        {
                            var evenStyle = new StiStyle
                            {
                                Name = tableData.Name + "EvenStyle",
                                Brush = new StiSolidBrush(tableStyle.AlternatingDataColor),
                                TextBrush = new StiSolidBrush(tableStyle.AlternatingDataForeground)
                            };
                            this.Report.Styles.Add(evenStyle);

                            tableData.EvenStyle = evenStyle.Name;
                        }
                    }
                }
                else
                {
                    if (this.tableStyleFX != null)
                    {
                        if (string.IsNullOrEmpty(tableData.OddStyle))
                        {
                            var oddStyle = new StiStyle
                            {
                                Name = tableData.Name + "OddStyle",
                                Brush = new StiSolidBrush(tableStyleFX.DataColor),
                                TextBrush = new StiSolidBrush(tableStyleFX.DataForeground)
                            };
                            this.Report.Styles.Add(oddStyle);

                            tableData.OddStyle = oddStyle.Name;
                        }

                        if (string.IsNullOrEmpty(tableData.EvenStyle))
                        {
                            var evenStyle = new StiStyle
                            {
                                Name = tableData.Name + "EvenStyle",
                                Brush = new StiSolidBrush(tableStyleFX.AlternatingDataColor),
                                TextBrush = new StiSolidBrush(tableStyleFX.AlternatingDataForeground)
                            };
                            this.Report.Styles.Add(evenStyle);

                            tableData.EvenStyle = evenStyle.Name;
                        }
                    }
                }

                var listCellHeader = new Hashtable();
                var joinCells = new Hashtable();
                int elementDataBand = headerRowsCount * this.columnCount;
                double yPosDataPanel = 0;
                for (int indexDataBand = 0; indexDataBand < rowCount - headerRowsCount - footerRowsCount; indexDataBand++)
                {
                    #region Create panel
                    var __height = ((IStiTableCell)Components[elementDataBand]).GetRealHeight();
                    var panel = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPanel") as StiPanel;
                    panel.ClientRectangle = new RectangleD(rect.X, yPosDataPanel, rect.Width, __height);
                    panel.Name = tableData.Name + "_DPnl" + indexDataBand.ToString();
                    panel.CanGrow = true;
                    panel.CanBreak = true;
                    panel.CanShrink = true;
                    panel.DockStyle = StiDockStyle.Top;
                    tableData.Components.Add(panel);
                    yPosDataPanel += __height;
                    #endregion

                    for (int indexCell = elementDataBand; indexCell < elementDataBand + columnCount; indexCell++)
                    {
                        var tableCell = Components[indexCell];
                        bool join = false;

                        #region Check joined cells
                        StiComponent parentJoin = null;
                        if (((IStiTableCell)tableCell).ParentJoin != -1)
                        {
                            int row = 0;
                            parentJoin = GetParentJoin(((IStiTableCell)tableCell).ParentJoin, ref row);
                            if (listCellHeader.ContainsKey(parentJoin))
                            {
                                if (indexDataBand == (int)listCellHeader[parentJoin])
                                {
                                    continue;
                                }
                                else
                                {
                                    var emptyText = StiActivator.CreateObject("Stimulsoft.Report.Components.StiText") as StiText;
                                    emptyText.Page = tableCell.Page;
                                    emptyText.Conditions = parentJoin.Conditions;
                                    CopyEventHandlersToComponent(emptyText, true, parentJoin);
                                    emptyText.SetPaintRectangle(tableCell.GetPaintRectangle());
                                    emptyText.ClientRectangle = new RectangleD(((IStiTableCell)tableCell).GetRealLeft(), 0,
                                        ((IStiTableCell)tableCell).GetRealWidth(), ((IStiTableCell)tableCell).GetRealHeight());
                                    emptyText.Name = tableCell.Name + "_Emp";
                                    emptyText.GrowToHeight = true;
                                    emptyText.Brush = new StiEmptyBrush();
                                    emptyText.DockStyle = ((IStiTableCell)tableCell).CellDockStyle;
                                    panel.Components.Add(emptyText);
                                    list.Add(emptyText);
                                    if (tableCell == parentJoin)
                                    {
                                        ((IStiTableCell)joinCells[parentJoin]).ParentJoinCell = emptyText;
                                    }
                                    continue;
                                }
                            }
                            else
                            {
                                listCellHeader.Add(parentJoin, indexDataBand);
                                tableCell = parentJoin;
                                join = true;
                            }
                        }
                        #endregion

                        #region Clone TableCell
                        tableCell.Top = 0;
                        switch (((IStiTableCell)tableCell).CellType)
                        {
                            case StiTablceCellType.Image:
                                var newCellImage = (StiImage)((StiTableCellImage)tableCell).Clone(true);
                                newCellImage.Restrictions = newCellImage.Restrictions ^ StiRestrictions.AllowDelete;
                                newCellImage.DockStyle = ((StiTableCellImage)tableCell).CellDockStyle;
                                //newCellImage.Brush = new StiEmptyBrush();
                                SetInteraction(tableCell);
                                panel.Components.Add(newCellImage);
                                list.Add(newCellImage);
                                if (join)
                                {
                                    newCellImage.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newCellImage.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newCellImage);
                                }
                                break;

                            case StiTablceCellType.Text:
                                var newCellText = (StiText)((StiTableCell)tableCell).Clone(true);
                                newCellText.Restrictions = newCellText.Restrictions ^ StiRestrictions.AllowDelete;
                                newCellText.DockStyle = ((StiTableCell)tableCell).CellDockStyle;
                                //newCellText.Brush = new StiEmptyBrush();
                                SetInteraction(tableCell);
                                panel.Components.Add(newCellText);
                                list.Add(newCellText);
                                if (join)
                                {
                                    newCellText.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        panel.CanGrow = false;
                                    }
                                    joinCells.Add(parentJoin, newCellText);
                                }
                                break;

                            case StiTablceCellType.CheckBox:
                                var newCheckBox = (StiCheckBox)((StiTableCellCheckBox)tableCell).Clone(true);
                                newCheckBox.Restrictions = newCheckBox.Restrictions ^ StiRestrictions.AllowDelete;
                                newCheckBox.DockStyle = ((StiTableCellCheckBox)tableCell).CellDockStyle;
                                //newCheckBox.Brush = new StiEmptyBrush();
                                SetInteraction(tableCell);
                                panel.Components.Add(newCheckBox);
                                list.Add(newCheckBox);
                                if (join)
                                {
                                    newCheckBox.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newCheckBox.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newCheckBox);
                                }
                                break;
                            case StiTablceCellType.RichText:
                                var newRichText = (StiRichText)((StiTableCellRichText)tableCell).Clone(true);
                                newRichText.Restrictions = newRichText.Restrictions ^ StiRestrictions.AllowDelete;
                                newRichText.DockStyle = ((StiTableCellRichText)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newRichText);
                                list.Add(newRichText);
                                if (join)
                                {
                                    newRichText.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newRichText.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newRichText);
                                }
                                break;
                        }
                        #endregion
                    }
                    elementDataBand += columnCount;
                }

                parentContainer.Components.Insert(index, tableData);
                list.Add(tableData);
                posY += dataBandHeight + 0.8;
                index++;
            }
            #endregion

            #region Create FooterBand
            if (footerRowsCount > 0)
            {
                var listCellFooter = new Hashtable();
                var joinCells = new Hashtable();
                int numberElementFooter = this.Components.Count - footerRowsCount * columnCount;
                for (int index1 = rCount - footerRowsCount; index1 < rCount; index1++)
                {
                    footerHeight += ((IStiTableCell)Components[index1 * columnCount]).GetRealHeight();
                }

                #region Create Footer
                StiDynamicBand tableFooter;
                if (isGroupingPresent)
                {
                    tableFooter = StiActivator.CreateObject("Stimulsoft.Report.Components.StiGroupFooterBand") as StiGroupFooterBand;
                    ((StiGroupFooterBand)tableFooter).GroupFooterBandInfoV2.IsTableGroupFooter = true;
                    tableFooter.Name = this.Name + "_GrFt";
                    //((StiGroupFooterBand)tableFooter).PrintOnAllPages = this.footerPrintOnAllPages;
                }
                else
                {
                    tableFooter = StiActivator.CreateObject("Stimulsoft.Report.Components.StiFooterBand") as StiFooterBand;
                    tableFooter.Name = this.Name + "_Ft";
                    ((StiFooterBand)tableFooter).FooterBandInfoV2.IsTableFooter = true;
                    ((StiFooterBand)tableFooter).PrintOnAllPages = this.FooterPrintOnAllPages;
                    ((StiFooterBand)tableFooter).PrintIfEmpty = this.FooterPrintIfEmpty;
                    ((StiFooterBand)tableFooter).PrintOnEvenOddPages = this.FooterPrintOnEvenOddPages;
                }

                tableFooter.ClientRectangle = new RectangleD(rect.X, posY, rect.Width, footerHeight);
                tableFooter.Border = this.Border;
                tableFooter.Brush = this.Brush;
                tableFooter.Interaction = null;
                tableFooter.Page = this.Page;
                tableFooter.Parent = this.Page;
                tableFooter.CanGrow = this.FooterCanGrow;
                tableFooter.CanShrink = this.FooterCanShrink;
                tableFooter.CanBreak = this.FooterCanBreak;
                tableFooter.PrintAtBottom = this.FooterPrintAtBottom;
                tableFooter.PrintOn = this.FooterPrintOn;

                //copy event with Conditions
                tableFooter.Conditions = this.Conditions.Clone() as StiConditionsCollection;
                CopyEventHandlersToComponent(tableFooter, true);
                #endregion

                double yPosFooterPanel = 0;
                for (int indexFooter = 0; indexFooter < footerRowsCount; indexFooter++)
                {
                    double __height = ((IStiTableCell)Components[numberElementFooter]).GetRealHeight(); // the footer height of this line
                    #region Create panel
                    var panel = StiActivator.CreateObject("Stimulsoft.Report.Components.StiPanel") as StiPanel;
                    panel.ClientRectangle = new RectangleD(rect.X, yPosFooterPanel, rect.Width, __height);
                    panel.Name = tableFooter.Name + "_FPnl" + indexFooter.ToString();
                    panel.CanGrow = true;
                    panel.CanShrink = true;
                    panel.DockStyle = StiDockStyle.Top;
                    tableFooter.Components.Add(panel);
                    yPosFooterPanel += __height;
                    #endregion

                    for (int indexCell = numberElementFooter; indexCell < numberElementFooter + columnCount; indexCell++)
                    {
                        var tableCell = Components[indexCell];
                        bool join = false;

                        #region Check joined cells
                        StiComponent parentJoin = null;
                        if (((IStiTableCell)tableCell).ParentJoin != -1)
                        {
                            int row = 0;
                            parentJoin = GetParentJoin(((IStiTableCell)tableCell).ParentJoin, ref row);
                            if (listCellFooter.ContainsKey(parentJoin))
                            {
                                if (indexFooter == (int)listCellFooter[parentJoin])
                                {
                                    continue;
                                }
                                else
                                {
                                    var emptyText = StiActivator.CreateObject("Stimulsoft.Report.Components.StiText") as StiText;
                                    emptyText.Page = tableCell.Page;
                                    emptyText.Conditions = parentJoin.Conditions;
                                    CopyEventHandlersToComponent(emptyText, true, parentJoin);
                                    emptyText.SetPaintRectangle(tableCell.GetPaintRectangle());
                                    emptyText.ClientRectangle = new RectangleD(((IStiTableCell)tableCell).GetRealLeft(), 0,
                                        ((IStiTableCell)tableCell).GetRealWidth(), ((IStiTableCell)tableCell).GetRealHeight());
                                    emptyText.Name = tableCell.Name + "_Emp";
                                    emptyText.GrowToHeight = true;
                                    emptyText.Brush = new StiEmptyBrush();
                                    emptyText.DockStyle = ((IStiTableCell)tableCell).CellDockStyle;
                                    panel.Components.Add(emptyText);
                                    list.Add(emptyText);
                                    if (tableCell == parentJoin)
                                    {
                                        ((IStiTableCell)joinCells[parentJoin]).ParentJoinCell = emptyText;
                                    }
                                    continue;
                                }
                            }
                            else
                            {
                                listCellFooter.Add(parentJoin, indexFooter);
                                tableCell = parentJoin;
                                join = true;
                            }
                        }
                        #endregion

                        #region Clone TableCell
                        tableCell.Top = 0;
                        switch (((IStiTableCell)tableCell).CellType)
                        {
                            case StiTablceCellType.Image:
                                var newCellImage = (StiImage)((StiTableCellImage)tableCell).Clone(true);
                                newCellImage.Restrictions = newCellImage.Restrictions ^ StiRestrictions.AllowDelete;
                                newCellImage.DockStyle = ((StiTableCellImage)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newCellImage);
                                list.Add(newCellImage);
                                if (join)
                                {
                                    newCellImage.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newCellImage.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newCellImage);
                                }
                                break;
                            case StiTablceCellType.Text:
                                var newCellText = (StiText)((StiTableCell)tableCell).Clone(true);
                                newCellText.Restrictions = newCellText.Restrictions ^ StiRestrictions.AllowDelete;
                                newCellText.DockStyle = ((StiTableCell)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newCellText);
                                list.Add(newCellText);
                                if (join)
                                {
                                    newCellText.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newCellText.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newCellText);
                                }
                                break;
                            case StiTablceCellType.CheckBox:
                                var newCheckBox = (StiCheckBox)((StiTableCellCheckBox)tableCell).Clone(true);
                                newCheckBox.Restrictions = newCheckBox.Restrictions ^ StiRestrictions.AllowDelete;
                                newCheckBox.DockStyle = ((StiTableCellCheckBox)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newCheckBox);
                                list.Add(newCheckBox);
                                if (join)
                                {
                                    newCheckBox.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newCheckBox.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newCheckBox);
                                }
                                break;
                            case StiTablceCellType.RichText:
                                var newRichText = (StiRichText)((StiTableCellRichText)tableCell).Clone(true);
                                newRichText.Restrictions = newRichText.Restrictions ^ StiRestrictions.AllowDelete;
                                newRichText.DockStyle = ((StiTableCellRichText)tableCell).CellDockStyle;
                                SetInteraction(tableCell);
                                panel.Components.Add(newRichText);
                                list.Add(newRichText);
                                if (join)
                                {
                                    newRichText.CanGrow = true;
                                    if (!IsEqualRows(tableCell, Components[indexCell]))
                                    {
                                        newRichText.Height = ((IStiTableCell)tableCell).GetRealHeight();
                                    }
                                    joinCells.Add(parentJoin, newRichText);
                                }
                                break;
                        }
                        #endregion
                    }

                    numberElementFooter += columnCount;
                }
                parentContainer.Components.Insert(index, tableFooter);
                index++;
                list.Add(tableFooter);
            }
            else if (isGroupingPresent && headerRowsCount > 0)
            {
                var tableFooterEmpty = StiActivator.CreateObject("Stimulsoft.Report.Components.StiGroupFooterBand") as StiGroupFooterBand;

                tableFooterEmpty.Height = 0;
                tableFooterEmpty.ClientRectangle = new Stimulsoft.Base.Drawing.RectangleD(rect.X, posY, rect.Width, 0);
                tableFooterEmpty.Name = this.Name + "_GrFtEmp";
                tableFooterEmpty.Page = this.Page;
                tableFooterEmpty.Parent = this.Page;
                tableFooterEmpty.PrintAtBottom = this.FooterPrintAtBottom;

                parentContainer.Components.Insert(index, tableFooterEmpty);
                list.Add(tableFooterEmpty);
                index++;
            }
            #endregion

            ReverseCells(this.RightToLeft);

            return tableData;
        }

        private void ReverseCells(bool rightToLeft)
        {
            if (!rightToLeft) return;

            for (int indexRow = 0; indexRow < rowCount; indexRow++)
            {
                int rowOffset = indexRow * columnCount;
                int tempCount = columnCount / 2;
                for (int indexCol = 0; indexCol < tempCount; indexCol++)
                {
                    var tempComp = Components[rowOffset + indexCol];
                    Components[rowOffset + indexCol] = Components[rowOffset + columnCount - 1 - indexCol];
                    Components[rowOffset + columnCount - 1 - indexCol] = tempComp;
                }
            }
            for (int indexComp = 0; indexComp < Components.Count; indexComp++)
            {
                var comp = Components[indexComp];
                comp.Left = this.Width - comp.Right;
            }
        }

        private void SetFilter(StiDataBand comp)
        {
            IStiFilter filter = comp;
            IStiDataSource dataSource = comp;

            if (!dataSource.IsDataSourceEmpty)
            {
                if (filter.FilterMethodHandler == null && filter.FilterOn)
                {
                    string correctedDataName = StiNameValidator.CorrectName(this.Name, comp.Report);

                    var type = comp.Report.GetType();
                    var method = type.GetMethod(correctedDataName + "__GetFilter", new Type[] { typeof(object), typeof(StiFilterEventArgs) });

                    if (method != null)
                    {
                        try
                        {
                            filter.FilterMethodHandler = Delegate.CreateDelegate(
                                typeof(StiFilterEventHandler), comp.Report, correctedDataName + "__GetFilter")
                                as StiFilterEventHandler;

                        }
                        catch (Exception e)
                        {
                            StiLogService.Write(comp.GetType(), "StiFilterEventHandler...ERROR");
                            StiLogService.Write(comp.GetType(), e);
                        }
                    }
                }
            }
        }

        private void SetInteraction(StiComponent cell)
        {
            if (cell.Interaction == null) return;

            //StiInteraction interaction = cell.Interaction;
            //if (interaction.SortingEnabled && !string.IsNullOrEmpty(interaction.SortingColumn))
            //{
            //    int indexOfDot = interaction.SortingColumn.IndexOf(".", StringComparison.InvariantCulture);
            //    if (indexOfDot != -1)
            //    {
            //        string st = interaction.SortingColumn.Substring(0, indexOfDot);
            //        if (!st.EndsWith("_DataBand"))
            //        {
            //            interaction.SortingColumn = interaction.SortingColumn.Insert(indexOfDot, "_DataBand");
            //        }
            //    }
            //}
        }

        private StiComponent GetParentJoin(int id, ref int rowNumber)
        {
            int index = 0;
            foreach (IStiTableCell cell in Components)
            {
                if (cell.ID == id)
                {
                    rowNumber = index / columnCount;
                    return cell as StiComponent;
                }
                index++;
            }

            return null;
        }

        private bool IsEqualRows(StiComponent comp1, StiComponent comp2)
        {
            int index1 = Components.IndexOf(comp1) / columnCount;
            int index2 = Components.IndexOf(comp2) / columnCount;
            return index1 == index2;
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiTable();
        }
        #endregion

        #region this
        /// <summary>
        /// Creates an object of the type StiTable.
        /// </summary>
        public StiTable()
            : this(RectangleD.Empty)
        {
            
        }

        /// <summary>
        ///  Creates an object of the type StiTable.
        /// </summary>
        /// <param name="rect">The rectangle decribes size and position of the component.</param>
        public StiTable(RectangleD rect)
            : base(rect)
        {
            this.rowCount = this.columnCount = 5;
            PlaceOnToolbox = false;
        }
        #endregion
    }
}