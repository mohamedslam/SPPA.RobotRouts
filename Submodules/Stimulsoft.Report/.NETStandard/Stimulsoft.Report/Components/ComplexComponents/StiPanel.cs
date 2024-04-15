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

using System;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes the class of Container.
    /// </summary>
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiPanel), "Stimulsoft.Report.Images.Components.StiPanel.png")]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiContextTool(typeof(IStiBreakable))]
    [StiContextTool(typeof(IStiShift))]
    [StiContextTool(typeof(IStiGrowToHeight))]
    [StiEngine(StiEngineVersion.EngineV2)]
    public class StiPanel : StiContainer
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiPanel
            jObject.AddPropertyBool("RightToLeft", RightToLeft);
            jObject.AddPropertyDouble("ColumnGaps", ColumnGaps, 0d);
            jObject.AddPropertyDouble("ColumnWidth", ColumnWidth, 0d);
            jObject.AddPropertyInt("Columns", Columns);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "RightToLeft":
                        this.rightToLeft = property.DeserializeBool();
                        break;

                    case "ColumnGaps":
                        this.columnGaps = property.DeserializeDouble();
                        break;

                    case "ColumnWidth":
                        this.columnWidth = property.DeserializeDouble();
                        break;

                    case "Columns":
                        this.columns = property.DeserializeInt();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiPanel;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            objHelper.Add(StiPropertyCategories.Columns, new[]
            {
                propHelper.Columns(),
                propHelper.ColumnWidth(),
                propHelper.ColumnGaps(),
                propHelper.RightToLeft()
            });

            if (level == StiLevel.Basic)
            {
                objHelper.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height()
                });
            }
            else
            {
                objHelper.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                objHelper.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle()
                });
            }
            else
            {
                objHelper.Add(StiPropertyCategories.Appearance, new[]
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
                objHelper.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.Enabled()
                });
            }
            else if (level == StiLevel.Standard)
            {
                objHelper.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.CanBreak(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            else
            {
                objHelper.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.GrowToHeight(),
                    propHelper.CanBreak(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                objHelper.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name()
                });
            }
            else if (level == StiLevel.Standard)
            {
                objHelper.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                });
            }
            else
            {
                objHelper.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                });
            }

            return objHelper;
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
        public override string HelpUrl => "user-manual/report_internals_panels.htm";
        #endregion

        #region IStiBreakable override
        /// <summary>
        /// Gets or sets value which indicates whether the component can or cannot break its contents on several pages.
        /// </summary>        
        [Browsable(true)]
        [Description("Gets or sets value which indicates whether the component can or cannot break its contents on several pages.")]
        [StiShowInContextMenu]
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

        #region Columns.Properties
        private bool rightToLeft;
        /// <summary>
        /// Gets or sets horizontal column direction.
        /// </summary>
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets horizontal column direction.")]
        [StiOrder(StiPropertyOrder.ColumnsRightToLeft)]
        [StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
        [StiCategory("Columns")]
        public virtual bool RightToLeft
        {
            get
            {
                return rightToLeft;
            }
            set
            {
                if (rightToLeft != value)
                {
                    CheckBlockedException("RightToLeft");
                    rightToLeft = value;
                }
            }
        }

        private double columnGaps;
        /// <summary>
        ///Gets or sets distance between two columns.
        /// </summary>
        [StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
        [DefaultValue(0d)]
        [StiCategory("Columns")]
        [StiOrder(StiPropertyOrder.ColumnsColumnGaps)]
        [Description("Gets or sets distance between two columns.")]
        public virtual double ColumnGaps
        {
            get
            {
                return columnGaps;
            }
            set
            {
                if (columnGaps == value) return;

                CheckBlockedException("ColumnGaps");
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "ColumnGaps", 
                        $"Value of '{value}' is not valid for 'ColumnGaps'. " + "'ColumnGaps' must be greater than or equal to 0.");
                }
                columnGaps = Math.Round(value, 2);
            }
        }

        private double columnWidth;
        /// <summary>
        /// Gets or sets width of column.
        /// </summary>
        [StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
        [DefaultValue(0d)]
        [StiCategory("Columns")]
        [StiOrder(StiPropertyOrder.ColumnsColumnWidth)]
        [Description("Gets or sets width of column.")]
        public virtual double ColumnWidth
        {
            get
            {
                return columnWidth;
            }
            set
            {
                if (columnWidth == value) return;

                CheckBlockedException("ColumnWidth");
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "ColumnWidth",
                        $"Value of '{value}' is not valid for 'ColumnWidth'. " + "'ColumnWidth' must be greater than or equal to 0.");
                }
                columnWidth = Math.Round(value, 2);
            }
        }

        private int columns;
        /// <summary>
        /// Gets or sets columns count.
        /// </summary>
        [StiSerializable(StiSerializeTypes.SerializeToCode | StiSerializeTypes.SerializeToDesigner | StiSerializeTypes.SerializeToSaveLoad)]
        [DefaultValue(0)]
        [StiCategory("Columns")]
        [StiOrder(StiPropertyOrder.ColumnsColumns)]
        [Description("Gets or sets columns count.")]
        public virtual int Columns
        {
            get
            {
                return columns;
            }
            set
            {
                if (columns == value) return;

                CheckBlockedException("Columns");
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "Columns",
                        $"Value of '{value}' is not valid for 'Columns'. " + "'Columns' must be greater than or equal to 0.");
                }
                columns = value;
            }
        }

        public virtual double GetColumnWidth()
        {
            double panelColumnWidth = this.ColumnWidth;
            if (panelColumnWidth == 0)
            {
                if (Columns == 0) return Width;
                panelColumnWidth = Width / Columns - ColumnGaps;
            }
            return panelColumnWidth;
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.Panel;

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Panel;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiPanel");
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiPanel();
        }
        #endregion

        /// <summary>
        /// Creates a new panel.
        /// </summary>
        public StiPanel()
            : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new panel.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the container.</param>
        public StiPanel(RectangleD rect)
            : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}