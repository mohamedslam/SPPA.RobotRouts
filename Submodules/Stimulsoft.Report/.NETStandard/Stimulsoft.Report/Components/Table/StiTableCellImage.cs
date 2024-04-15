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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Linq;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Table
{
    [StiToolbox(false)]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiGdiPainter(typeof(StiTableCellImageGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiTableCellImageWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiTableCellImageQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfTableCellImageQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiTableCellImage : 
        StiImage, 
        IStiTableCell, 
        IStiTableComponent
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty(nameof(Locked));
            jObject.RemoveProperty(nameof(Linked));

            // StiTableCellImage
            jObject.AddPropertyEnum(nameof(CellDockStyle), CellDockStyle, StiDockStyle.None);
            jObject.AddPropertyIntArray(nameof(JoinCells), JoinCells);
            jObject.AddPropertyInt(nameof(ParentJoin), ParentJoin, -1);
            jObject.AddPropertyBool(nameof(Join), Join);
            jObject.AddPropertyInt(nameof(ID), ID, -1);
            jObject.AddPropertyInt(nameof(JoinWidth), JoinWidth);
            jObject.AddPropertyInt(nameof(JoinHeight), JoinHeight);
            jObject.AddPropertyEnum(nameof(CellType), CellType, StiTablceCellType.Image);
            jObject.AddPropertyBool(nameof(FixedWidth), FixedWidth);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(CellDockStyle):
                        this.CellDockStyle = property.DeserializeEnum<StiDockStyle>();
                        break;

                    case nameof(JoinCells):
                        this.JoinCells = property.DeserializeIntArray();
                        break;

                    case nameof(ParentJoin):
                        this.ParentJoin = property.DeserializeInt();
                        break;

                    case nameof(Join):
                        this.join = property.DeserializeBool();
                        break;

                    case nameof(ID):
                        this.ID = property.DeserializeInt();
                        break;

                    case nameof(JoinWidth):
                        this.joinWidth = property.DeserializeInt();
                        break;

                    case nameof(JoinHeight):
                        this.joinHeight = property.DeserializeInt();
                        break;

                    case nameof(CellType):
                        this.cellType = property.DeserializeEnum<StiTablceCellType>();
                        break;

                    case nameof(FixedWidth):
                        this.FixedWidth = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiTableCellImage;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.ImageEditor()
            });

            collection.Add(StiPropertyCategories.Cell, new[]
            {
                propHelper.CellType(),
                propHelper.CellDockStyle(),
                propHelper.FixedWidth()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.ImageAdditional, new[]
                {
                    propHelper.AspectRatio(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                    propHelper.Stretch()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.ImageAdditional, new[]
                {
                    propHelper.AspectRatio(),
                    propHelper.MultipleFactor(),
                    propHelper.HorAlignment(),
                    propHelper.VertAlignment(),
                    propHelper.ImageRotation(),
                    propHelper.ProcessingDuplicates(),
                    propHelper.Stretch()
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
                    propHelper.GrowToHeight(),
                    propHelper.CanBreak(),
                    propHelper.Enabled()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.CanBreak(),
                    propHelper.Enabled(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
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
                    propHelper.GlobalizedName()
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
                        StiPropertyEventId.GetExcelValueEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                        StiPropertyEventId.GetValueEvent,
                    }
                }
            };
        }
        #endregion

        #region ICloneable override
        public override object Clone(bool cloneProperties)
        {
            var cloneCell = (StiTableCellImage)base.Clone(cloneProperties);

            cloneCell.join = this.join;
            cloneCell.MinSize = new SizeD(0, 0);
            cloneCell.MaxSize = new SizeD(0, 0);
            cloneCell.CellDockStyle = this.CellDockStyle;
            cloneCell.joinWidth = this.joinWidth;
            cloneCell.joinHeight = this.joinHeight;
            cloneCell.cellType = this.cellType;
            cloneCell.Column = this.Column;
            cloneCell.FixedWidth = this.FixedWidth;
            cloneCell.ID = this.ID;
            cloneCell.JoinCells = (int[])this.JoinCells.Clone();
            cloneCell.ParentJoin = this.ParentJoin != -1 ? this.ParentJoin : -1;

            return cloneCell;
        }
        #endregion

        #region Properties Browsable(false)
        [Browsable(false)]
        public override StiDockStyle DockStyle
        {
            get
            {
                return base.DockStyle;
            }
            set
            {
                base.DockStyle = value;
            }
        }

        [Browsable(false)]
        public override StiRestrictions Restrictions
        {
            get
            {
                return base.Restrictions;
            }
            set
            {
                base.Restrictions = value;
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

        [Browsable(false)]
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

        [StiNonSerialized]
        [Browsable(false)]
        public override bool Locked
        {
            get
            {
                return IsDesigning && (!Report.IsPageDesigner);
            }
            set
            {
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public override bool Linked
        {
            get
            {
                return IsDesigning && (!Report.IsPageDesigner);
            }
            set
            {
            }
        }

        [Browsable(true)]
        public override bool CanShrink
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

        #region Properties override
        [Browsable(true)]
        [StiShowInContextMenu]
        public override bool CanGrow
        {
            get
            {
                return base.CanGrow;
            }
            set
            {
                if (base.CanGrow != value)
                {
                    base.CanGrow = value;
                    if (Parent is StiTable)
                    {
                        ((StiTable)Parent).ChangeGrowToHeightAtCell(this);
                    }
                }
            }
        }

        [Browsable(true)]
        public override bool GrowToHeight
        {
            get
            {
                return base.GrowToHeight;
            }
            set
            {
                base.GrowToHeight = value;
            }
        }
        #endregion

        #region IStiTableCell.Properties
        [StiSerializable(StiSerializationVisibility.List)]
        [Browsable(false)]
        public int[] JoinCells { get; set; } = new int[0];

        [StiSerializable]
        [Browsable(false)]
        [DefaultValue(-1)]
        public int ParentJoin { get; set; } = -1;

        private bool join;
        [StiSerializable, Browsable(false), DefaultValue(false), StiCategory("Table")]
        public bool Join
        {
            get 
            { 
                return join; 
            }
            set
            {
                if (join == value) return;

                if (value)
                    CreateJoin();

                else
                    DeleteJoin();

                join = value;
            }
        }

        [StiSerializable, Browsable(false), DefaultValue(-1)]
        public int ID { get; set; } = -1;

        private int joinWidth;
        [StiSerializable, Browsable(false), DefaultValue(0)]
        public int JoinWidth
        {
            get 
            { 
                return joinWidth; 
            }
            set 
            { 
                joinWidth = value; 
            }
        }

        private int joinHeight;
        [StiSerializable, Browsable(false), DefaultValue(0)]
        public int JoinHeight
        {
            get 
            { 
                return joinHeight; 
            }
            set 
            { 
                joinHeight = value; 
            }
        }

        [Browsable(false)]
        public bool Merged => ParentJoin != -1;

        [Browsable(false)]
        public bool ChangeTopPosition
        {
            get
            {
                if (!(Parent is StiTable)) return false;

                var index = Parent.Components.IndexOf(this);
                return index >= ((StiTable)Parent).ColumnCount;
            }
        }

        [Browsable(false)]
        public bool ChangeLeftPosition
        {
            get
            {
                if (!(Parent is StiTable)) return false;

                var index = Parent.Components.IndexOf(this);
                var findIndex = 0;
                while (findIndex < Parent.Components.Count)
                {
                    if (findIndex == index) return false;
                    findIndex += ((StiTable)Parent).ColumnCount;
                }
                return true;
            }
        }

        [Browsable(false)]
        public bool ChangeRightPosition
        {
            get
            {
                if (!(Parent is StiTable)) return true;

                var index = Parent.Components.IndexOf(this);
                var findIndex = ((StiTable)Parent).ColumnCount - 1;
                while (findIndex < Parent.Components.Count)
                {
                    if (findIndex == index) return false;
                    findIndex += ((StiTable)Parent).ColumnCount;
                }

                return true;
            }
        }

        private StiTablceCellType cellType = StiTablceCellType.Image;
        /// <summary>
        /// Get or sets a type of the cell content.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiTablceCellType.Image)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Cell")]
        [Description("Get or sets a type of the cell content.")]
        [StiOrder(100)]
        public StiTablceCellType CellType
        {
            get
            {
                return cellType;
            }
            set
            {
                cellType = value;
                var table = Parent as StiTable;
                if (table == null) return;

                switch (value)
                {
                    case StiTablceCellType.Text:
                        table.ChangeTableCellContentInText(this);
                        break;
                    case StiTablceCellType.CheckBox:
                        table.ChangeTableCellContentInCheckBox(this);
                        break;
                    case StiTablceCellType.RichText:
                        table.ChangeTableCellContentInRichText(this);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Get or sets a type of the cell docking.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiDockStyle.None)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Cell")]
        [Description("Get or sets a type of the cell docking.")]
        [StiOrder(120)]
        public StiDockStyle CellDockStyle { get; set; } = StiDockStyle.None;

        [StiNonSerialized]
        [Browsable(false)]
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates that the cell have fixed width.
        /// </summary>
        [StiSerializable]
        [Browsable(true)]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("Cell")]
        [Description("Gets or sets a value which indicates that the cell have fixed width.")]
        [StiOrder(150)]
        public bool FixedWidth { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public StiComponent ParentJoinCell { get; set; }

        [StiNonSerialized]
        [Browsable(false)]
        public object TableTag { get; set; }
        #endregion

        #region IStiTableCell.Methods
        public StiComponent GetJoinComponentByGuid(int id)
        {
            if (this.Parent == null && JoinCells.Length == 0)
                return null;

            foreach (StiComponent cell in Parent.Components)
            {
                if (((IStiTableCell)cell).ID == id)
                    return cell;
            }

            return null;
        }

        public StiComponent GetJoinComponentByIndex(int index)
        {
            if (index < 0 && Parent == null && JoinCells.Length == 0 && index >= JoinCells.Length)
                return null;

            foreach (StiComponent cell in Parent.Components)
            {
                if (((IStiTableCell)cell).ID == JoinCells[index])
                    return cell;
            }

            return null;
        }

        public bool ContainsGuid(int id)
        {
            return JoinCells.Length != 0 && JoinCells.Any(t => t == id);
        }

        private void CreateJoin()
        {
            var table = this.Parent as StiTable;
            if (table == null && Page == null)
                return;

            double sumWidth = 0;
            double sumHeight = 0;
            JoinCells = table.CreateJoin(ref sumWidth, ref sumHeight, ref joinWidth, ref joinHeight);
            if (JoinCells.Length == 0)
                return;

            double valueLeft = 0;
            double valueTop = 0;
            for (int index = 0; index < JoinCells.Length - 1; index++)
            {
                StiComponent cell = GetJoinComponentByIndex(index);
                if (index == 0)
                {
                    valueLeft = cell.Left;
                    valueTop = cell.Top;
                }
                cell.Enabled = false;
                ((IStiTableCell)cell).ParentJoin = this.ID;
            }

            this.ParentJoin = this.ID;
            this.ClientRectangle = new RectangleD(valueLeft, valueTop, sumWidth, sumHeight);
        }

        private void DeleteJoin()
        {
            if (Page == null && JoinCells.Length == 0)
                return;

            for (int index = 0; index < JoinCells.Length - 1; index++)
            {
                var cell = GetJoinComponentByIndex(index);
                cell.Enabled = true;
                ((IStiTableCell)cell).ParentJoin = -1;
            }

            this.ClientRectangle = GetNewClientRectangle();
            JoinCells = new int[0];
            ParentJoin = -1;
            joinWidth = 0;
            joinHeight = 0;
        }

        private RectangleD GetNewClientRectangle()
        {
            var cell = GetJoinComponentByIndex(JoinCells.Length - 2);
            int index = Parent.Components.IndexOf(cell);
            int thisIndex = Parent.Components.IndexOf(this);

            return ((thisIndex - index) == 1)
                ? new RectangleD(cell.Right, cell.Top, this.Right - cell.Right, cell.Height)
                : new RectangleD(cell.Left, cell.Bottom, cell.Width, this.Bottom - cell.Bottom);
        }

        public void SetJoinSize()
        {
            if (!join) return;

            var cell = GetJoinComponentByIndex(0);
            var left = cell.Left;
            var top = cell.Top;

            this.ClientRectangle = new RectangleD(left, top, this.Right - left, this.Bottom - top);
        }

        public double GetRealHeightAfterInsertRows()
        {
            if (!join)
                return base.Height;

            if (joinHeight == 1)
                return base.Height;

            var sumHeight = 0d;
            var firstNumberRow = Parent.Components.IndexOf(GetJoinComponentByIndex(0)) / ((StiTable) Parent).ColumnCount;
            var lastNumberRow = Parent.Components.IndexOf(this) / ((StiTable) Parent).ColumnCount;
            for (var indexRow = firstNumberRow; indexRow < lastNumberRow; indexRow++)
            {
                var indexCell = indexRow * ((StiTable) Parent).ColumnCount;
                sumHeight += Parent.Components[indexCell].Height;
            }

            return base.Height - sumHeight;
        }

        public double GetRealHeight()
        {
            if (!join)
                return base.Height;

            var cell = GetJoinComponentByIndex(JoinCells.Length - 2);
            var thisIndex = Parent.Components.IndexOf(this);
            var index = Parent.Components.IndexOf(cell);
            if (((StiTable) Parent).ColumnCount <= 1)
                return Bottom - cell.Bottom;

            return thisIndex - index == 1 ? cell.Height : Bottom - cell.Bottom;
        }

        public double GetRealTop()
        {
            if (!join)
                return base.Top;

            var cell = GetJoinComponentByIndex(JoinCells.Length - 2);
            var thisIndex = Parent.Components.IndexOf(this);
            var index = Parent.Components.IndexOf(cell);
            if (((StiTable) Parent).ColumnCount <= 1)
                return cell.Bottom;

            return thisIndex - index == 1 ? cell.Top : cell.Bottom;
        }

        public double GetRealWidth()
        {
            if (!join)
                return base.Width;

            var cell = GetJoinComponentByIndex(JoinCells.Length - 2);
            var thisIndex = Parent.Components.IndexOf(this);
            var index = Parent.Components.IndexOf(cell);

            if (((StiTable) Parent).RowCount <= 1)
                return this.Right - cell.Right;

            return thisIndex - index == 1 ? Right - cell.Right : cell.Width;
        }

        public double GetRealLeft()
        {
            if (!join)
                return this.Left;

            var cell = GetJoinComponentByIndex(JoinCells.Length - 2);
            var thisIndex = Parent.Components.IndexOf(this);
            var index = Parent.Components.IndexOf(cell);

            if (((StiTable) Parent).RowCount <= 1)
                return cell.Right;

            return thisIndex - index == 1 ? cell.Right : cell.Left;
        }
        #endregion

        #region Methods
        internal void SetJoin(bool isJoin)
        {
            join = isJoin;
        }
        #endregion

        #region Methods override
        public override StiComponent CreateNew()
        {
            return new StiTableCellImage();
        }
        #endregion
    }
}