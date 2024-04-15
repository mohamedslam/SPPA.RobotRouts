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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Represents a Windows date-time picker control.
	/// </summary>	
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiDateTimePickerControl), "Stimulsoft.Report.Dialogs.Bmp.StiDateTimePickerControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiDateTimePickerControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiDateTimePickerControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiDateTimePickerControl : 
		StiReportControl,
		IStiDateTimePickerControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDateTimePickerControl
            jObject.AddPropertyJObject("ValueChangedEvent", ValueChangedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("ValueBinding", ValueBinding);
            jObject.AddPropertyStringNullOrEmpty("MaxDateBinding", MaxDateBinding);
            jObject.AddPropertyStringNullOrEmpty("MinDateBinding", MinDateBinding);
            jObject.AddPropertyEnum("Format", Format, DateTimePickerFormat.Long);
            jObject.AddPropertyEnum("DropDownAlign", DropDownAlign, LeftRightAlignment.Left);
            jObject.AddPropertyBool("ShowUpDown", ShowUpDown);
            jObject.AddPropertyBool("Today", Today, true);
            jObject.AddPropertyStringNullOrEmpty("CustomFormat", CustomFormat);
            jObject.AddPropertyDateTime("MaxDate", MaxDate);      // DateTime     //Anton
            jObject.AddPropertyDateTime("MinDate", MinDate);      // DateTime     //Anton
            jObject.AddPropertyDateTime("Value", Value);          // DateTime     //Anton

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ValueChangedEvent":
                        {
                            var _event = new StiValueChangedEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.ValueChangedEvent = _event;
                        }
                        break;

                    case "ValueBinding":
                        this.valueBinding = property.DeserializeString();
                        break;

                    case "MaxDateBinding":
                        this.maxDateBinding = property.DeserializeString();
                        break;

                    case "MinDateBinding":
                        this.minDateBinding = property.DeserializeString();
                        break;

                    case "Format":
                        this.format = property.DeserializeEnum<DateTimePickerFormat>();
                        break;

                    case "DropDownAlign":
                        this.dropDownAlign = property.DeserializeEnum<LeftRightAlignment>();
                        break;

                    case "ShowUpDown":
                        this.showUpDown = property.DeserializeBool();
                        break;

                    case "Today":
                        this.today = property.DeserializeBool();
                        break;

                    case "CustomFormat":
                        this.customFormat = property.DeserializeString();
                        break;

                    case "MaxDate":
						this.maxDate = property.DeserializeDateTime();
                        break;

                    case "MinDate":
                        this.minDate = property.DeserializeDateTime();
						break;

                    case "Value":
                        this.valueDateTime = property.DeserializeDateTime();
						break;
                }
            }
        }
        #endregion

		#region StiComponent override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiDateTimePickerControl;
            }
        }

		/// <summary>
		/// Return events collection of this component;
		/// </summary>
		public override StiEventsCollection GetEvents()
		{
			StiEventsCollection events = base.GetEvents();
			if (ValueChangedEvent != null)events.Add(ValueChangedEvent);
			return events;
		}

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.DateTimePickerControl;
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
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName
		{
			get 
			{
				return StiLocalization.Get("Dialogs", "StiDateTimePickerControl");
			}
		}
		#endregion

		#region Events
		#region OnValueChanged
		private static readonly object EventValueChanged = new object();

		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		public event EventHandler ValueChanged
		{
			add
			{
				base.Events.AddHandler(EventValueChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventValueChanged, value);
			}
		}

		public void InvokeValueChanged(EventArgs e)
		{
			var handler = base.Events[EventValueChanged] as EventHandler;
			if (handler != null)handler(this, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, ValueChangedEvent);
		}


		/// <summary>
		/// Gets or sets a script of the event ValueChanged.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event ValueChanged.")]
		public virtual StiValueChangedEvent ValueChangedEvent
		{
			get
			{				
				return new StiValueChangedEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion
		#endregion

		#region Controls Property
		private DateTimePicker control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public DateTimePicker Control
		{
			get
			{
				return control;
			}
			set
			{
				control = value;
			}
		}


		private string valueBinding = "";
		/// <summary>
		/// Gets the data bindings for the value.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the value.")]
        [StiOrder(StiPropertyOrder.DialogValueBinding)]
		public string ValueBinding
		{
			get
			{
				return valueBinding;
			}
			set
			{
				valueBinding = value;
			}
		}


		private string maxDateBinding = "";
		/// <summary>
		/// Gets the data bindings for the max date.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the max date.")]
        [StiOrder(StiPropertyOrder.DialogMaxDateBinding)]
		public string MaxDateBinding
		{
			get
			{
				return maxDateBinding;
			}
			set
			{
				maxDateBinding = value;
			}
		}


		private string minDateBinding = "";
		/// <summary>
		/// Gets the data bindings for the min date.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the min date.")]
        [StiOrder(StiPropertyOrder.DialogMinDateBinding)]
		public string MinDateBinding
		{
			get
			{
				return minDateBinding;
			}
			set
			{
				minDateBinding = value;
			}
		}


		/// <summary>
		/// Gets or sets the height of the control.
		/// </summary>
		[StiCategory("Position")]
		public override double Height
		{
			get
			{
				return 20;
			}
			set
			{
				base.Height = 20;
			}
		}
		

		private DateTimePickerFormat format = DateTimePickerFormat.Long;
		/// <summary>
		/// Gets or sets the format of the date and time displayed in the control.
		/// </summary>
		[StiSerializable]
		[DefaultValue(DateTimePickerFormat.Long)]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets the format of the date and time displayed in the control.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogFormat)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual DateTimePickerFormat Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
				UpdateReportControl("Format");
			}
		}



		private LeftRightAlignment dropDownAlign = LeftRightAlignment.Left;
		/// <summary>
		/// Gets or sets the alignment of the drop-down calendar on the date-time picker control.
		/// </summary>
		[StiSerializable]
		[DefaultValue(LeftRightAlignment.Left)]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets the alignment of the drop-down calendar on the date-time picker control.")]
        [StiOrder(StiPropertyOrder.DialogDropDownAlign)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual LeftRightAlignment DropDownAlign
		{
			get
			{
				return dropDownAlign;
			}
			set
			{
				dropDownAlign = value;
				UpdateReportControl("DropDownStyle");
			}
		}


		private bool showUpDown = false;
		/// <summary>
		/// Gets or sets a value indicating whether an up-down control is used to adjust the date-time value.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether an up-down control is used to adjust the date-time value.")]
        [StiOrder(StiPropertyOrder.DialogShowUpDown)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool ShowUpDown
		{
			get
			{
				return showUpDown;
			}
			set
			{
				showUpDown = value;
				UpdateReportControl("ShowUpDown");
			}
		}


		private bool today = true;
		/// <summary>
		/// Gets or sets value which indicates the date is set equal current date.
		/// </summary>
		[StiSerializable]
		[DefaultValue(true)]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets value which indicates the date is set equal current date.")]
        [StiOrder(StiPropertyOrder.DialogToday)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual bool Today
		{
			get
			{
				return today;
			}
			set
			{
				today = value;
				UpdateReportControl("Today");
			}
		}


		private string customFormat = string.Empty;
		/// <summary>
		/// Gets or sets the custom date-time format string.
		/// </summary>
		[StiSerializable]
		[DefaultValue("")]
		[StiCategory("Behavior")]
		[Description("Gets or sets the custom date-time format string.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogCustomFormat)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual string CustomFormat
		{
			get
			{
				return customFormat;
			}
			set
			{
				customFormat = value;
				UpdateReportControl("CustomFormat");
			}
		}



		private DateTime maxDate = new DateTime(9998, 12, 31);
		/// <summary>
		/// Gets or sets the maximum date and time that can be selected in the control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Description("Gets or sets the maximum date and time that can be selected in the control.")]
        [StiOrder(StiPropertyOrder.DialogMaxDate)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual DateTime MaxDate
		{
			get
			{
				return maxDate;
			}
			set
			{
				maxDate = value;
				UpdateReportControl("MaxDate");
			}
		}


		private DateTime minDate = new DateTime(1753, 01, 01);
		/// <summary>
		/// Gets or sets the minimum date and time that can be selected in the control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Description("Gets or sets the minimum date and time that can be selected in the control.")]
        [StiOrder(StiPropertyOrder.DialogMinDate)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual DateTime MinDate
		{
			get
			{
				return minDate;
			}
			set
			{
				minDate = value;
				UpdateReportControl("MinDate");
			}
		}

		private DateTime valueDateTime = DateTime.Now;
		/// <summary>
		/// Gets or sets the date-time value assigned to the control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Description("Gets or sets the date-time value assigned to the control.")]
        [StiOrder(StiPropertyOrder.DialogValue)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual DateTime Value
		{
			get
			{
				return valueDateTime;
			}
			set
			{
				valueDateTime = value;
				UpdateReportControl("Value");
			}
		}
		#endregion

		#region Report Control Override
		/// <summary>
		/// Gets default event for this report control.
		/// </summary>
		/// <returns>Default event.</returns>
		public override StiEvent GetDefaultEvent()
		{
			return this.ValueChangedEvent;
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiDateTimePickerControl();
        }
        #endregion

		/// <summary>
		/// Creates a new StiDateTimePickerControl.
		/// </summary>
		public StiDateTimePickerControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiDateTimePickerControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiDateTimePickerControl(RectangleD rect) : base(rect)
		{
			base.BackColor = SystemColors.Window;
			valueDateTime = DateTime.Now;
			PlaceOnToolbox = true;
		}
	}
}