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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes class that realizes component - StiHorizontalLinePrimitive.
    /// </summary>
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiHorizontalLinePrimitive), "Stimulsoft.Report.Images.Components.StiHorizontalLinePrimitive.png")]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiHorizontalLinePrimitiveDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfHorizontalLinePrimitiveDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiHorizontalLinePrimitiveGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiHorizontalLinePrimitiveWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    public class StiHorizontalLinePrimitive :
        StiLinePrimitive,
        IStiHideBorderFromDesigner,
        IStiBorder
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiHorizontalLinePrimitive
            jObject.AddPropertyCap("StartCap", StartCap);
            jObject.AddPropertyCap("EndCap", EndCap);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "StartCap":
                        this.StartCap = property.DeserializeCap();
                        break;

                    case "EndCap":
                        this.EndCap = property.DeserializeCap();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiHorizontalLinePrimitive;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            // PrimitiveCategory
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
                    propHelper.StartCap(),
                    propHelper.EndCap()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
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
        public override string HelpUrl => "user-manual/report_internals_primitives.htm";
        #endregion

        #region ICloneable override
        public override object Clone(bool cloneProperties)
        {
            var cloneLine = (StiHorizontalLinePrimitive)base.Clone(cloneProperties);

            if (StartCap != null)
                cloneLine.StartCap = (StiCap)this.StartCap.Clone();
            else
                cloneLine.StartCap = null;

            if (EndCap != null)
                cloneLine.EndCap = (StiCap)this.EndCap.Clone();
            else
                cloneLine.EndCap = null;

            return cloneLine;
        }
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
                        StiBorderSides.Top, this.Color, this.Size, this.Style, false, 0, null);
                }
                return border;
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
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.HorizontalLinePrimitive;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Shapes;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiHorizontalLinePrimitive");
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the start cap settings.
        /// </summary>
        [Browsable(true)]
        [StiCategory("Primitive")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiOrder(StiPropertyOrder.PrimitiveStartCap)]
        [Description("Gets or sets the start cap settings.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiCap StartCap { get; set; } = new StiCap();

        /// <summary>
        /// Gets or sets the end cap settings.
        /// </summary>
        [Browsable(true)]
        [StiCategory("Primitive")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiOrder(StiPropertyOrder.PrimitiveEndCap)]
        [Description("Gets or sets the end cap settings.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiPropertyLevel(StiLevel.Standard)]
        public StiCap EndCap { get; set; } = new StiCap();
        #endregion

        #region Position
        [Browsable(false)]
        public override double Height
        {
            get
            {
                if (Page != null && Page.Unit != null)
                    return Page.Unit.ConvertFromHInches(1d);

                return 1;
            }
            set
            {
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiHorizontalLinePrimitive();
        }
        #endregion

        /// <summary>
        /// Creates a new StiHorizontalLinePrimitive.
        /// </summary>
        public StiHorizontalLinePrimitive() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new StiHorizontalLinePrimitive.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiHorizontalLinePrimitive(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}