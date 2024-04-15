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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes class that realizes component - StiRectanglePrimitive.
    /// </summary>
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiRectanglePrimitive), "Stimulsoft.Report.Images.Components.StiRectanglePrimitive.png")]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiRectanglePrimitiveDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfRectanglePrimitiveDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiRectanglePrimitiveGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiRectanglePrimitiveWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    public class StiRectanglePrimitive :
        StiCrossLinePrimitive,
        IStiHideBorderFromDesigner,
        IStiBorder
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiRectanglePrimitive
            jObject.AddPropertyBool("TopSide", TopSide, true);
            jObject.AddPropertyBool("LeftSide", LeftSide, true);
            jObject.AddPropertyBool("BottomSide", BottomSide, true);
            jObject.AddPropertyBool("RightSide", RightSide, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "TopSide":
                        this.TopSide = property.DeserializeBool();
                        break;

                    case "LeftSide":
                        this.LeftSide = property.DeserializeBool();
                        break;

                    case "BottomSide":
                        this.BottomSide = property.DeserializeBool();
                        break;

                    case "RightSide":
                        this.RightSide = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiRectanglePrimitive;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Primitive, new[]
                {
                    propHelper.Color(),
                    propHelper.SizeFloat(),
                    propHelper.Style()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Primitive, new[]
                {
                    propHelper.Color(),
                    propHelper.SizeFloat(),
                    propHelper.Style(),
                    propHelper.TopSide(),
                    propHelper.LeftSide(),
                    propHelper.BottomSide(),
                    propHelper.RightSide()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize()
                });
            }
            
            collection.Add(StiPropertyCategories.Appearance, new[]
            {
                propHelper.ComponentStyle()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.Enabled()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.InteractionEditor(),
                    propHelper.AnchorMode(),
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
        public override string HelpUrl => "user-manual/getting_started_report_with_cross_primitives.htm?zoom_highlightsub=Rectangle%2BPrimitive";
        #endregion

        #region IStiBorder
        private StiBorder border;
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [Browsable(false)]
        [StiNonSerialized]
        public StiBorder Border
        {
            get
            {
                if (border == null)
                {
                    border = new StiBorder(
                        StiBorderSides.All, this.Color, this.Size, this.Style, false, 0, null);
                }
                return border;
            }
            set
            {
            }
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
            if (GetStartPoint() == null && GetEndPoint() == null)
                base.Convert(oldUnit, newUnit, isReportSnapshot);
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.RectanglePrimitive;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Shapes;
        
        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiRectanglePrimitive");
        #endregion

        #region Properties.Position
        public override double Width
        {
            get
            {
                StiPointPrimitive startPoint = GetStartPoint();
                StiPointPrimitive endPoint = GetEndPoint();

                if (startPoint == null || endPoint == null)
                    return base.Width;

                else
                {
                    var startPos = new PointD(startPoint.Left, startPoint.Top);
                    var endPos = new PointD(endPoint.Left, endPoint.Top);

                    startPos = startPoint.ComponentToPage(startPos);
                    endPos = endPoint.ComponentToPage(endPos);

                    return Math.Round(endPos.X - startPos.X, 2);
                }
            }
            set
            {
                base.Width = value;

                var startPoint = GetStartPoint();
                var endPoint = GetEndPoint();

                if (startPoint != null && endPoint != null)
                {
                    var startPos = new PointD(startPoint.Left, startPoint.Top);

                    startPos = startPoint.ComponentToPage(startPos);
                    var endPos = new PointD(startPos.X + value, startPos.Y);
                    endPos = endPoint.PageToComponent(endPos);
                    endPoint.Left = endPos.X;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets os sets property which indicates to draw top side of the rectangle or no.
        /// </summary>
        [StiCategory("Primitive")]
        [StiOrder(StiPropertyOrder.PrimitiveTopSide)]
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets os sets property which indicates to draw top side of the rectangle or no.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool TopSide { get; set; } = true;

        /// <summary>
        /// Gets os sets property which indicates to draw left side of the rectangle or no.
        /// </summary>
        [StiCategory("Primitive")]
        [StiOrder(StiPropertyOrder.PrimitiveLeftSide)]
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets os sets property which indicates to draw left side of the rectangle or no.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool LeftSide { get; set; } = true;

        /// <summary>
        /// Gets os sets property which indicates to draw bottom side of the rectangle or no.
        /// </summary>
        [StiCategory("Primitive")]
        [StiOrder(StiPropertyOrder.PrimitiveBottomSide)]
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets os sets property which indicates to draw bottom side of the rectangle or no.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool BottomSide { get; set; } = true;

        /// <summary>
        /// Gets os sets property which indicates to draw right side of the rectangle or no.
        /// </summary>
        [StiCategory("Primitive")]
        [StiOrder(StiPropertyOrder.PrimitiveRightSide)]
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets os sets property which indicates to draw right side of the rectangle or no.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual bool RightSide { get; set; } = true;
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiRectanglePrimitive();
        }
        #endregion

        /// <summary>
		/// Creates a new StiRectanglePrimitive.
		/// </summary>
		public StiRectanglePrimitive() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new StiRectanglePrimitive.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiRectanglePrimitive(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}