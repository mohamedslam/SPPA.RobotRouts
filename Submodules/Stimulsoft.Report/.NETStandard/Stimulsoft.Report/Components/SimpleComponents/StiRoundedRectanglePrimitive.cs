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
using System.ComponentModel;

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Describes class that realizes component - StiRoundedRectanglePrimitive.
    /// </summary>
    [StiToolbox(true)]
	[StiServiceBitmap(typeof(StiRoundedRectanglePrimitive), "Stimulsoft.Report.Images.Components.StiRoundedRectanglePrimitive.png")]
    [StiGdiPainter(typeof(StiRoundedRectanglePrimitiveGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiRoundedRectanglePrimitiveWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiRoundedRectanglePrimitive : StiRectanglePrimitive
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiRoundedRectanglePrimitive
            jObject.AddPropertyFloat("Round", Round, 0.2f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Round":
                        this.round = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiRoundedRectanglePrimitive;

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
                    propHelper.Round(),
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
        public override string HelpUrl => "user-manual/report_internals_primitives_crossprimitives.htm?zoom_highlightsub=Rounded%2BRectangle";
        #endregion

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.RoundedRectanglePrimitive;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Shapes;
        
        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiRoundedRectanglePrimitive");
        #endregion

		#region Properties
        private float round = 0.2f;
        /// <summary>
        /// Gets or sets the factor of rounding.
        /// </summary>
        [StiCategory("Primitive")]
        [StiOrder(StiPropertyOrder.PrimitiveRound)]
        [StiSerializable]
        [Description("Gets or sets the factor of rounding.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual float Round
        {
            get
            {
                return round;
            }
            set
            {
                if (value > 0 && value <= 0.5)
                    round = value;
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiRoundedRectanglePrimitive();
        }
        #endregion

        /// <summary>
        /// Creates a new StiRoundedRectanglePrimitive.
		/// </summary>
		public StiRoundedRectanglePrimitive() : this(RectangleD.Empty)
		{
		}

		/// <summary>
        /// Creates a new StiRoundedRectanglePrimitive.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiRoundedRectanglePrimitive(RectangleD rect) : base(rect)
		{
            PlaceOnToolbox = false;
		}
	}
}