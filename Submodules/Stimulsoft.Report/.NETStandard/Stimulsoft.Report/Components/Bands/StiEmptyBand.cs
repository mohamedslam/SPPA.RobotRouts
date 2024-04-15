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
using System.Drawing;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.Design;
using System.Drawing.Design;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Desribes the class that realizes the band - Empty Band.
    /// </summary>
    [StiServiceBitmap(typeof(StiEmptyBand), "Stimulsoft.Report.Images.Components.StiEmptyBand.png")]
    [StiToolbox(true)]
    [StiDesigner(typeof(StiComponentDesigner))]
    [StiContextTool(typeof(IStiCanGrow))]
    [StiContextTool(typeof(IStiCanShrink))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiSelectAllQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfSelectAllQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiEmptyBand :
        StiBand,
        IStiOddEvenStyles
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiEmptyBand
            jObject.AddPropertyStringNullOrEmpty("EvenStyle", EvenStyle);
            jObject.AddPropertyStringNullOrEmpty("OddStyle", OddStyle);
            jObject.AddPropertyJObject("BeginRenderEvent", BeginRenderEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("RenderingEvent", RenderingEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("EndRenderEvent", EndRenderEvent.SaveToJsonObject(mode));
            jObject.AddPropertyEnum("SizeMode", SizeMode, StiEmptySizeMode.AlignFooterToTop);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "EvenStyle":
                        this.EvenStyle = property.DeserializeString();
                        break;

                    case "OddStyle":
                        this.OddStyle = property.DeserializeString();
                        break;

                    case "BeginRenderEvent":
                        {
                            var _event = new StiBeginRenderEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.BeginRenderEvent = _event;
                        }
                        break;

                    case "RenderingEvent":
                        {
                            var _event = new StiRenderingEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.RenderingEvent = _event;
                        }
                        break;

                    case "EndRenderEvent":
                        {
                            var _event = new StiEndRenderEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.EndRenderEvent = _event;
                        }
                        break;

                    case "SizeMode":
                        this.SizeMode = property.DeserializeEnum<StiEmptySizeMode>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiEmptyBand;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

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
                    propHelper.MinHeight(),
                    propHelper.MaxHeight()
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
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.Enabled(),
                    propHelper.PrintOn(),
                    propHelper.ResetPageNumber(),
                    propHelper.SizeMode()
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
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/report_internals_empty_band.htm";
        #endregion

        #region StiBand override
        /// <summary>
        /// Returns the band header text.
        /// </summary>
        /// <returns>Band header text.</returns>
        public override string GetHeaderText()
        {
            return ToString();
        }

        /// <summary>
        /// Gets header start color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderStartColor => Color.FromArgb(186, 235, 137);

        /// <summary>
        /// Gets header end color.
        /// </summary>
        [Browsable(false)]
        public override Color HeaderEndColor => Color.FromArgb(186, 235, 137);
        #endregion

        #region StiComponent override
        /// <summary>
        /// Return events collection of this component.
        /// </summary>
        public override StiEventsCollection GetEvents()
        {
            var events = base.GetEvents();

            if (BeginRenderEvent != null)
                events.Add(BeginRenderEvent);

            if (RenderingEvent != null)
                events.Add(RenderingEvent);

            if (EndRenderEvent != null)
                events.Add(EndRenderEvent);

            return events;
        }
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.EmptyBand;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Bands;

        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.EmptyBand;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiEmptyBand");
        #endregion

        #region IStiOddEvenStyles
        /// <summary>
        /// Gets or sets value, indicates style of even lines.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceEvenStyle)]
        [Description("Gets or sets value, indicates style of even lines.")]
        [Editor("Stimulsoft.Report.Design.StiStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual string EvenStyle
        {
            get
            {
                return Properties.Get(StiDataBand.PropertyEvenStyle, string.Empty) as string;
            }
            set
            {
                Properties.Set(StiDataBand.PropertyEvenStyle, value, string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets value, indicates style of odd lines.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceOddStyle)]
        [Description("Gets or sets value, indicates style of odd lines.")]
        [Editor("Stimulsoft.Report.Design.StiStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public virtual string OddStyle
        {
            get
            {
                return Properties.Get(StiDataBand.PropertyOddStyle, string.Empty) as string;
            }
            set
            {
                Properties.Set(StiDataBand.PropertyOddStyle, value, string.Empty);
            }
        }
        #endregion

        #region Events
        #region OnBeginRender
        private static readonly object EventBeginRender = new object();

        /// <summary>
        /// Occurs when band begin render.
        /// </summary>
        public event EventHandler BeginRender
        {
            add
            {
                Events.AddHandler(EventBeginRender, value);
            }
            remove
            {
                Events.RemoveHandler(EventBeginRender, value);
            }
        }

        /// <summary>
        /// Raises the BeginRender event for this component.
        /// </summary>
        protected virtual void OnBeginRender(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the BeginRender event for this component.
        /// </summary>
        public void InvokeBeginRender()
        {
            OnBeginRender(EventArgs.Empty);

            var handler = Events[EventBeginRender] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when band begin render.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when band begin render.")]
        public StiBeginRenderEvent BeginRenderEvent
        {
            get
            {
                return new StiBeginRenderEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion

        #region OnRendering
        private static readonly object EventRendering = new object();

        /// <summary>
        /// Occurs when a data row rendering.
        /// </summary>
        public event EventHandler Rendering
        {
            add
            {
                Events.AddHandler(EventRendering, value);
            }
            remove
            {
                Events.RemoveHandler(EventRendering, value);
            }
        }

        /// <summary>
        /// Raises the Rendering event for this component.
        /// </summary>
        protected virtual void OnRendering(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the Rendering event for this component.
        /// </summary>
        public void InvokeRendering()
        {
            OnRendering(EventArgs.Empty);

            var handler = Events[EventRendering] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(this.Report, this, RenderingEvent);
        }

        /// <summary>
        /// Occurs when a data row rendering.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when a data row rendering.")]
        public StiRenderingEvent RenderingEvent
        {
            get
            {
                return new StiRenderingEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion

        #region OnEndRender
        private static readonly object EventEndRender = new object();

        /// <summary>
        /// Occurs when band rendering is finished.
        /// </summary>
        public event EventHandler EndRender
        {
            add
            {
                Events.AddHandler(EventEndRender, value);
            }
            remove
            {
                Events.RemoveHandler(EventEndRender, value);
            }
        }

        /// <summary>
        /// Raises the EndRender event for this component.
        /// </summary>
        protected virtual void OnEndRender(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the EndRender event for this component.
        /// </summary>
        public void InvokeEndRender()
        {
            OnEndRender(EventArgs.Empty);

            var handler = Events[EventEndRender] as EventHandler;
            if (handler != null)
                handler(this, EventArgs.Empty);

            StiBlocklyHelper.InvokeBlockly(this.Report, this, EndRenderEvent);
        }

        /// <summary>
        /// Occurs when band rendering is finished.
        /// </summary>
        [StiSerializable]
        [StiCategory("RenderEvents")]
        [Browsable(false)]
        [Description("Occurs when band rendering is finished.")]
        public StiEndRenderEvent EndRenderEvent
        {
            get
            {
                return new StiEndRenderEvent(this);
            }
            set
            {
                if (value != null)
                    value.Set(this, value.Script);
            }
        }
        #endregion
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiEmptyBand();
        }
        #endregion

        #region Properties
        /// <summary>
        /// This property allows to indicate how to change the size of the last row on a page.
        /// </summary>
        [DefaultValue(StiEmptySizeMode.AlignFooterToTop)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorSizeMode)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("This property allows to indicate how to change the size of the last row on a page.")]
        [StiPropertyLevel(StiLevel.Standard)]
        public virtual StiEmptySizeMode SizeMode { get; set; } = StiEmptySizeMode.AlignFooterToTop;
        #endregion

        /// <summary>
        /// Creates a new component of the type StiHeaderBand.
        /// </summary>
        public StiEmptyBand() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiHeaderBand with specified location.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiEmptyBand(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}