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
using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Design;
using Stimulsoft.Base.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Describes the base class for all report controls.
	/// </summary>
	[StiToolbox(false)]
	[StiServiceBitmap(typeof(StiReportControl), "Stimulsoft.Report.Dialogs.Bmp.StiReportControl.gif")]
	[StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Dialogs.Bmp.StiReportControl.gif")]
	[StiEngine(StiEngineVersion.All)]
	[StiDesigner("Stimulsoft.Report.Dialogs.Design.StiReportControlDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfReportControlDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
	public class StiReportControl : 
		StiContainer,
		IStiReportControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("UseParentStyles");
            jObject.RemoveProperty("MinSize");
            jObject.RemoveProperty("MaxSize");
            jObject.RemoveProperty("Interaction");
            jObject.RemoveProperty("PrintOn");
            jObject.RemoveProperty("Border");
            jObject.RemoveProperty("Brush");
            jObject.RemoveProperty("GrowToHeight");
            jObject.RemoveProperty("ShiftMode");
            jObject.RemoveProperty("Left");
            jObject.RemoveProperty("Top");
            jObject.RemoveProperty("Width");
            jObject.RemoveProperty("Height");
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");
            jObject.RemoveProperty("Printable");
            jObject.RemoveProperty("Pointer");
            jObject.RemoveProperty("Bookmark");
            jObject.RemoveProperty("Hyperlink");
            jObject.RemoveProperty("Conditions");
			jObject.RemoveProperty("GetPointerEvent");
			jObject.RemoveProperty("GetBookmarkEvent");
            jObject.RemoveProperty("GetHyperlinkEvent");
            jObject.RemoveProperty("BeforePrintEvent");
            jObject.RemoveProperty("AfterPrintEvent");
            jObject.RemoveProperty("GetDrillDownReportEvent");

            // StiReportControl
            jObject.AddPropertyJObject("EnterEvent", EnterEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("LeaveEvent", LeaveEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("MouseDownEvent", MouseDownEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("MouseUpEvent", MouseUpEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("MouseMoveEvent", MouseMoveEvent.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("TagValueBinding", TagValueBinding);
            jObject.AddPropertySize("Size", Size);
            jObject.AddPropertyPoint("Location", Location);
            jObject.AddPropertyFontMicrosoftSansSerif8("Font", Font);
            jObject.AddPropertyColor("BackColor", BackColor, SystemColors.Control);
            jObject.AddPropertyBool("Visible", Visible, true);
            jObject.AddPropertyEnum("RightToLeft", RightToLeft, RightToLeft.No);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "EnterEvent":
                        {
                            var _event = new StiEnterEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.EnterEvent = _event;
                        }
                        break;

                    case "LeaveEvent":
                        {
                            var _event = new StiLeaveEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.LeaveEvent = _event;
                        }
                        break;

                    case "MouseDownEvent":
                        {
                            var _event = new StiMouseDownEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.MouseDownEvent = _event;
                        }
                        break;

                    case "MouseUpEvent":
                        {
                            var _event = new StiMouseUpEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.MouseUpEvent = _event;
                        }
                        break;

                    case "MouseMoveEvent":
                        {
                            var _event = new StiMouseMoveEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.MouseMoveEvent = _event;
                        }
                        break;

                    case "TagValueBinding":
                        this.tagValueBinding = property.DeserializeString();
                        break;

                    case "Size":
                        this.Size = property.DeserializeSize();
                        break;

                    case "Location":
                        this.Location = property.DeserializePoint();
                        break;

                    case "Font":
                        this.font = property.DeserializeFont(this.font);
                        break;

                    case "BackColor":
                        this.backColor = property.DeserializeColor();
                        break;

                    case "Visible":
                        this.visible = property.DeserializeBool();
                        break;

                    case "RightToLeft":
                        this.rightToLeft = property.DeserializeEnum<RightToLeft>();
                        break;
                }
            }
        }
		#endregion

		#region IStiGetFonts
		public override List<StiFont> GetFonts()
		{
			var result = base.GetFonts();
			result.Add(new StiFont(Font));
			return result.Distinct().ToList();
		}
		#endregion

		#region ICloneable override
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return base.Clone();
		}
		#endregion

		#region IStiUnitConvert
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
		{
		}
		#endregion

		#region StiService override
		public override void PackService()
		{
			base.PackService();
			dataBindings = null;
		}

		/// <summary>
		/// Gets a service category.
		/// </summary>
		[Browsable(false)]
		public override string ServiceCategory
		{
			get
			{
				return StiLocalization.Get("Report", "Dialogs");
			}
		}	
		#endregion

		#region Off
		[StiNonSerialized]
		[Browsable(false)]
		public override bool UseParentStyles
		{
			get
			{
				return base.UseParentStyles;
			}
			set
			{
				if (base.UseParentStyles != value)
				{
					base.UseParentStyles = value;
				}
			}
		}

        [StiNonSerialized]
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

        [StiNonSerialized]
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
		public override StiInteraction Interaction
		{
			get
			{
				return base.Interaction;
			}
			set
			{
				base.Interaction = value;
			}
		}

        #region DockStyle Off (Wpf)
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogDockStyle)]
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
        #endregion

		#region IStiPrintOn Off
		[Browsable(false)]
		[StiNonSerialized]
		public override StiPrintOnType PrintOn
		{
			get 
			{
				return base.PrintOn;
			}
			set 
			{
				base.PrintOn = value;
			}
		}
		#endregion
		
		#region IStiBorder Off
		[Browsable(false)]
		[StiNonSerialized]
		public override StiBorder Border
		{
			get 
			{
				return base.Border;
			}
			set 
			{
				base.Border = value;
			}
		}
		#endregion

		#region IStiBrush Off
		[Browsable(false)]
		[StiNonSerialized]
		public override StiBrush Brush
		{
			get 
			{
				return base.Brush;
			}
			set 
			{
				base.Brush = value;
			}
		}
		#endregion

		#region Component off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool GrowToHeight
		{
			get 
			{
				return base.GrowToHeight;
			}
			set 
			{
			}
		}

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

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiShiftMode ShiftMode
		{
			get 
			{
				return base.ShiftMode;
			}
			set 
			{
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override double Left
		{
			get 
			{
				return base.Left;
			}
			set 
			{
				base.Left = value;
				UpdateReportControl("Location");
			}
		}

		
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override double Top
		{
			get 
			{
				return base.Top;
			}
			set 
			{
				base.Top = value;
				UpdateReportControl("Location");
			}
		}

		
		[Browsable(false)]
		[StiNonSerialized]
		public override double Width
		{
			get 
			{
				return base.Width;
			}
			set 
			{
				base.Width = value;
				UpdateReportControl("Size");
			}
		}

		
		[Browsable(false)]
		[StiNonSerialized]
		public override double Height
		{
			get 
			{
				return base.Height;
			}
			set 
			{
				base.Height = value;
				UpdateReportControl("Size");
			}
		}
		#endregion

		#region IStiCanShrink Off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool CanShrink
		{
			get
			{
				return base.CanShrink;
			}
			set
			{
				base.CanShrink = value;
			}
		}
		#endregion

		#region IStiCanGrow Off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool CanGrow
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

		#region StiComponent Off
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override bool Printable
		{
			get 
			{
				return base.Printable;
			}
			set 
			{
				base.Printable = value;
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiPointerExpression Pointer
		{
			get
			{
				return base.Pointer;
			}
			set
			{
				base.Pointer = value;
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiBookmarkExpression Bookmark
		{
			get
			{
				return base.Bookmark;
			}
			set
			{
				base.Bookmark = value;
			}
		}

	
		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiHyperlinkExpression Hyperlink
		{
			get
			{
				return base.Hyperlink;
			}
			set
			{
				base.Hyperlink = value;
			}
		}

		[Browsable(false)]
		[StiNonSerialized]
		public sealed override StiConditionsCollection Conditions
		{
			get
			{
				return base.Conditions;
			}
			set
			{
				base.Conditions = value;
			}
		}
		#endregion
		#endregion

		#region Events Off
		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetPointerEvent GetPointerEvent
		{
			get
			{
				return base.GetPointerEvent;
			}
			set
			{
				base.GetPointerEvent = value;
			}
		}

		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetBookmarkEvent GetBookmarkEvent
		{
			get
			{
				return base.GetBookmarkEvent;
			}
			set
			{
				base.GetBookmarkEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetHyperlinkEvent GetHyperlinkEvent
		{
			get
			{
				return base.GetHyperlinkEvent;
			}
			set
			{
				base.GetHyperlinkEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiBeforePrintEvent BeforePrintEvent
		{
			get
			{
				return base.BeforePrintEvent;
			}
			set
			{
				base.BeforePrintEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiAfterPrintEvent AfterPrintEvent
		{
			get
			{
				return base.AfterPrintEvent;
			}
			set
			{
				base.AfterPrintEvent = value;
			}
		}


		[StiEventHide]
		[StiNonSerialized]
		public sealed override StiGetDrillDownReportEvent GetDrillDownReportEvent
		{
			get
			{
				return base.GetDrillDownReportEvent;
			}
			set
			{
				base.GetDrillDownReportEvent = value;
			}
		}
		#endregion

		#region Events
		#region MouseLeaveEvent
		/// <summary>
		/// Gets or sets a script of the event MouseLeave.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event MouseLeave.")]
		public override StiMouseLeaveEvent MouseLeaveEvent
		{
			get
			{
				return base.MouseLeaveEvent;
			}
			set
			{
				base.MouseLeaveEvent = value;
			}
		}
		#endregion

		#region MouseEnterEvent
		/// <summary>
		/// Gets or sets a script of the event MouseEnter.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event MouseEnter.")]
		public override StiMouseEnterEvent MouseEnterEvent
		{
			get
			{
				return base.MouseEnterEvent;
			}
			set
			{
				base.MouseEnterEvent = value;
			}
		}
		#endregion

		#region GetTag
		/// <summary>
		/// Gets or sets a script of the event GetTag.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event GetTag.")]
		public override StiGetTagEvent GetTagEvent
		{
			get
			{
				return base.GetTagEvent;
			}
			set
			{
				base.GetTagEvent = value;
			}
		}
		#endregion

		#region GetToolTip
		/// <summary>
		/// Gets or sets a script of the event GetToolTip.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event GetToolTip.")]
		public override StiGetToolTipEvent GetToolTipEvent
		{
			get
			{
				return base.GetToolTipEvent;
			}
			set
			{
				base.GetToolTipEvent = value;
			}
		}
		#endregion

		#region Click
		/// <summary>
		/// Gets or sets a script of the event Click.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event Click.")]
		public override StiClickEvent ClickEvent
		{
			get
			{
				return base.ClickEvent;
			}
			set
			{
				base.ClickEvent = value;
			}
		}
		#endregion

		#region DoubleClick
		/// <summary>
		/// Gets or sets a script of the event DoubleClick.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event DoubleClick.")]
		public override StiDoubleClickEvent DoubleClickEvent
		{
			get
			{
				return base.DoubleClickEvent;
			}
			set
			{
				base.DoubleClickEvent = value;
			}
		}
		#endregion

		#region OnEnter
		private static readonly object EventEnter = new object();

		/// <summary>
		/// Occurs when the control is entered.
		/// </summary>
		public event EventHandler Enter
		{
			add
			{
				base.Events.AddHandler(EventEnter, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventEnter, value);
			}
		}

		public void InvokeEnter(EventArgs e)
		{
			EventHandler handler = base.Events[EventEnter] as EventHandler;
			if (handler != null)handler(this, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, EnterEvent);
		}


		/// <summary>
		/// Gets or sets a script of the event Enter.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event Enter.")]
		public virtual StiEnterEvent EnterEvent
		{
			get
			{				
				return new StiEnterEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region OnLeave
		private static readonly object EventLeave = new object();

		/// <summary>
		/// Occurs when the input focus leaves the control.
		/// </summary>
		public event EventHandler Leave
		{
			add
			{
				base.Events.AddHandler(EventLeave, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventLeave, value);
			}
		}

		public void InvokeLeave(EventArgs e)
		{
			EventHandler handler = base.Events[EventLeave] as EventHandler;
			if (handler != null)handler(this, e);
		}

		/// <summary>
		/// Gets or sets a script of the event Leave.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event Leave.")]
		public virtual StiLeaveEvent LeaveEvent
		{
			get
			{				
				return new StiLeaveEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region OnMouseDown
		private static readonly object EventMouseDown = new object();

		/// <summary>
		/// Occurs when the mouse pointer is over the control and a mouse button is pressed.
		/// </summary>
		public event MouseEventHandler MouseDown
		{
			add
			{
				base.Events.AddHandler(EventMouseDown, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventMouseDown, value);
			}
		}

		public void InvokeMouseDown(MouseEventArgs e)
		{
			var handler = base.Events[EventMouseDown] as MouseEventHandler;
			if (handler != null)handler(this, e);


			StiBlocklyHelper.InvokeBlockly(this.Report, this, MouseDownEvent, e);
		}

		/// <summary>
		/// Gets or sets a script of the event MouseDown.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event MouseDown.")]
		public virtual StiMouseDownEvent MouseDownEvent
		{
			get
			{				
				return new StiMouseDownEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region OnMouseUp
		private static readonly object EventMouseUp = new object();

		/// <summary>
		/// Occurs when the mouse pointer is over the control and a mouse button is released.
		/// </summary>
		public event MouseEventHandler MouseUp
		{
			add
			{
				base.Events.AddHandler(EventMouseUp, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventMouseUp, value);
			}
		}

		public void InvokeMouseUp(MouseEventArgs e)
		{
			var handler = base.Events[EventMouseUp] as MouseEventHandler;
			if (handler != null)handler(this, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, MouseUpEvent);
		}


		/// <summary>
		/// Gets or sets a script of the event MouseUp.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event MouseUp.")]
		public virtual StiMouseUpEvent MouseUpEvent
		{
			get
			{				
				return new StiMouseUpEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion

		#region OnMouseMove
		private static readonly object EventMouseMove = new object();

		/// <summary>
		/// Occurs when the mouse pointer is moved over the control.
		/// </summary>
		public event MouseEventHandler MouseMove
		{
			add
			{
				base.Events.AddHandler(EventMouseMove, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventMouseMove, value);
			}
		}

		public void InvokeMouseMove(MouseEventArgs e)
		{
			var handler = base.Events[EventMouseMove] as MouseEventHandler;
			if (handler != null)handler(this, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, MouseMoveEvent);
		}


		/// <summary>
		/// Gets or sets a script of the event MouseMove.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event MouseMove.")]
		public virtual StiMouseMoveEvent MouseMoveEvent
		{
			get
			{				
				return new StiMouseMoveEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion
		#endregion

		#region StiComponent override

        public override string HelpUrl
        {
            get
            {
                return null;
            }
        }

        [StiSerializable]
        [DefaultValue("")]
        [StiCategory("Appearance")]
        [Description("Gets or sets a style of a component.")]
        [Editor("Stimulsoft.Report.Design.StiDialogStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder(StiPropertyOrder.DialogComponentStyle)]
        public override string ComponentStyle
        {
            get
            {
                return base.ComponentStyle;
            }
            set
            {
                base.ComponentStyle = value;
            }
        }

		/// <summary>
		/// Return events collection of this component;
		/// </summary>
		public override StiEventsCollection GetEvents()
		{
			StiEventsCollection events = new StiEventsCollection();
			if (ClickEvent != null)events.Add(ClickEvent);
			if (GetTagEvent != null)events.Add(GetTagEvent);
			if (EnterEvent != null)events.Add(EnterEvent);
			if (LeaveEvent != null)events.Add(LeaveEvent);
			if (MouseLeaveEvent != null)events.Add(MouseLeaveEvent);
			if (MouseEnterEvent != null)events.Add(MouseEnterEvent);
			if (MouseDownEvent != null)events.Add(MouseDownEvent);
			if (MouseUpEvent != null)events.Add(MouseUpEvent);
			if (MouseMoveEvent != null)events.Add(MouseMoveEvent);
			return events;
		}


		/// <summary>
		/// May this container be located in the specified component.
		/// </summary>
		/// <param name="component">Component for checking.</param>
		/// <returns>true, if this container may is located in the specified component.</returns>
		public override bool CanContainIn(StiComponent component)
		{
			if (component is StiReportControl && ((StiReportControl)component).IsReportContainer)return true;
			if (component is StiForm)return true;

			return false;
		}
		

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.ReportControl;
			}
		}

        public override StiToolboxCategory ToolboxCategory
        {
            get
            {
                return StiToolboxCategory.Controls;
            }
        }


		/// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory
		{
			get 
			{
				return ServiceCategory;
			}
		}


		/// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName
		{
			get 
			{
				return StiLocalization.Get("Dialogs", "StiReportControl");
			}
		}
		#endregion

		#region Controls Property
        private object controlWpf = null;
        /// <summary>
        /// Gets ot sets WPF control for this Report Control.
        /// </summary>
        [Browsable(false)]
        public object ControlWpf
        {
            get
            {
                return controlWpf;
            }
            set
            {
                controlWpf = value;
            }
        }

		[StiCategory("Design")]
        [StiOrder(StiPropertyOrder.DialogToolTip)]
        [Browsable(true)]
        [StiPropertyLevel(StiLevel.Standard)]
		public override StiToolTipExpression ToolTip
		{
			get 
			{
				return base.ToolTip;
			}
			set 
			{
				base.ToolTip = value;
			}
		}


		[StiCategory("Design")]
        [StiOrder(StiPropertyOrder.DialogTag)]
        [Browsable(true)]
        [StiPropertyLevel(StiLevel.Standard)]
		public override StiTagExpression Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}


		private StiDataBindingsCollection dataBindings;
		/// <summary>
		/// Gets the data bindings for the control.
		/// </summary>
		[StiCategory("Data")]
		[DefaultValue("")]
		[Description("Gets the data bindings for the control.")]
        [StiOrder(StiPropertyOrder.DialogDataBindings)]
        [StiPropertyLevel(StiLevel.Standard)]
		public StiDataBindingsCollection DataBindings
		{
			get
			{
				return dataBindings;
			}
			set
			{
				dataBindings = value;
			}
		}


		private string tagValueBinding = "";
		/// <summary>
		/// Gets the data bindings for the tag value.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the tag value.")]
		public string TagValueBinding
		{
			get
			{
				return tagValueBinding;
			}
			set
			{
				tagValueBinding = value;
			}
		}


		private Color foreColor = Color.Black;
		/// <summary>
		/// Gets or sets the foreground color of the control.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("Appearance")]
		[Description("Gets or sets the foreground color of the control.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogForeColor)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual Color ForeColor
		{
			get 
			{
				return foreColor;
			}
			set 
			{
				foreColor = value;
				UpdateReportControl("ForeColor");
			}
		}
		

		/// <summary>
		/// Gets or sets the height and width of the control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Browsable(true)]
		[Description("Gets or sets the height and width of the control.")]
        [StiOrder(StiPropertyOrder.DialogSize)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Size Size
		{
			get
			{
				return new Size((int)base.Width, (int)base.Height);
			}
			set
			{
				base.Width = value.Width;
				base.Height = value.Height;
				UpdateReportControl("Size");
			}
		}


		private Point location = new Point(0, 0);
		/// <summary>
		/// Gets or sets the coordinates of the upper-left corner of the control relative to the upper-left corner of its container.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Browsable(true)]
		[Description("Gets or sets the coordinates of the upper-left corner of the control relative to the upper-left corner of its container.")]
        [StiOrder(StiPropertyOrder.DialogLocation)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Point Location
		{
			get
			{
				return new Point((int)base.Left, (int)base.Top);
			}
			set
			{
				base.Left = value.X;
				base.Top = value.Y;
				UpdateReportControl("Location");
			}
		}
		

		private Font font = new Font("Microsoft Sans Serif", 8);
		/// <summary>
		/// The font used to display text in the component.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Description("The font used to display text in the component.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogFont)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				UpdateReportControl("Font");
			}
		}


		private Color backColor = SystemColors.Control;
		/// <summary>
		/// Gets or sets the background color for the control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the background color for the control.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogBackColor)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				UpdateReportControl("BackColor");
			}
		}


		private bool ShouldSerializeBackColor()
		{
			return backColor != SystemColors.Control;
		}


		private bool ShouldSerializeForeColor()
		{
			return foreColor != SystemColors.ControlText;
		}


		/// <summary>
		/// Gets or sets a value indicating whether the control can respond to user interaction.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether the control can respond to user interaction.")]
        [StiOrder(StiPropertyOrder.DialogEnabled)]
        [StiPropertyLevel(StiLevel.Basic)]
		public override bool Enabled
		{
			get 
			{
				return base.Enabled;
			}
			set 
			{
				base.Enabled = value;
				UpdateReportControl("Enabled");
			}
		}


		private bool visible = true;
		/// <summary>
		/// Gets or sets a value indicating whether the control is displayed.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether the control is displayed.")]
        [StiOrder(StiPropertyOrder.DialogVisible)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				UpdateReportControl("Visible");
			}
		}



		private RightToLeft rightToLeft = RightToLeft.No;
		/// <summary>
		/// Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[DefaultValue(RightToLeft.No)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogRightToLeft)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual RightToLeft RightToLeft
		{
			get
			{
				return rightToLeft;
			}
			set
			{
				rightToLeft = value;
				UpdateReportControl("RightToLeft");
			}
		}
		#endregion
			
		#region this
		/// <summary>
		/// Use for update report control properties.
		/// </summary>
		/// <param name="propertyName">A name of the property which will be updated.</param>
		protected void UpdateReportControl(string propertyName)
		{
			if (Page != null)((StiForm)Page).InvokeReportControlUpdate(this, propertyName);
		}
        

		/// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public override RectangleD DefaultClientRectangle
		{
			get
			{
				return new RectangleD(0, 0, 96, 24);
			}
		}


		/// <summary>
		/// Gets default event for this report control.
		/// </summary>
		/// <returns>Default event.</returns>
		public virtual StiEvent GetDefaultEvent()
		{
			return this.ClickEvent;
		}

		
		[Browsable(false)]
		public virtual bool IsReportContainer
		{
			get
			{
				return false;
			}
		}
		

		/// <summary>
		/// Creates a new StiReportControl.
		/// </summary>
		public StiReportControl() : this(RectangleD.Empty)
		{
			
		}


		/// <summary>
		/// Creates a new StiReportControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiReportControl(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;
			dataBindings = new StiDataBindingsCollection(this);
		}
		#endregion
	}
}