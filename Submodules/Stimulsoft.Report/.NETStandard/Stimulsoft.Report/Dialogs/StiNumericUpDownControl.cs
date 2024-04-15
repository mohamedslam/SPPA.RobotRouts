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
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Represents a Windows up-down control that displays numeric values.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiNumericUpDownControl), "Stimulsoft.Report.Dialogs.Bmp.StiNumericUpDownControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiNumericUpDownControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiNumericUpDownControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiNumericUpDownControl : StiReportControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiNumericUpDownControl
            jObject.AddPropertyStringNullOrEmpty("MinimumBinding", MinimumBinding);
            jObject.AddPropertyStringNullOrEmpty("MaximumBinding", MaximumBinding);
            jObject.AddPropertyStringNullOrEmpty("ValueBinding", ValueBinding);
            jObject.AddPropertyInt("Minimum", Minimum);
            jObject.AddPropertyInt("Maximum", Maximum, 100);
            jObject.AddPropertyInt("Increment", Increment, 1);
            jObject.AddPropertyInt("Value", Value);
            jObject.AddPropertyJObject("ValueChangedEvent", ValueChangedEvent.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "MinimumBinding":
                        this.minimumBinding = property.DeserializeString();
                        break;

                    case "MaximumBinding":
                        this.maximumBinding = property.DeserializeString();
                        break;

                    case "ValueBinding":
                        this.valueBinding = property.DeserializeString();
                        break;

                    case "Minimum":
                        this.minimum = property.DeserializeInt();
                        break;

                    case "Maximum":
                        this.maximum = property.DeserializeInt();
                        break;

                    case "Increment":
                        this.increment = property.DeserializeInt();
                        break;

                    case "Value":
                        this.valueData = property.DeserializeInt();
                        break;

                    case "ValueChangedEvent":
                        {
                            var _event = new StiValueChangedEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.ValueChangedEvent = _event;
                        }
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
                return StiComponentId.StiNumericUpDownControl;
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
				return (int)StiReportControlToolboxPosition.NumericUpDownControl;
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
				return StiLocalization.Get("Dialogs", "StiNumericUpDownControl");
			}
		}
		#endregion

		#region Controls Property
		private NumericUpDown control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public NumericUpDown Control
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


		private string minimumBinding = "";
		/// <summary>
		/// Gets the data bindings for the minimum.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the minimum.")]
        [StiOrder(StiPropertyOrder.DialogMinimumBinding)]
		public string MinimumBinding
		{
			get
			{
				return minimumBinding;
			}
			set
			{
				minimumBinding = value;
			}
		}


		private string maximumBinding = "";
		/// <summary>
		/// Gets the data bindings for the maximum.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the maximum.")]
        [StiOrder(StiPropertyOrder.DialogMaximumBinding)]
		public string MaximumBinding
		{
			get
			{
				return maximumBinding;
			}
			set
			{
				maximumBinding = value;
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


		private int minimum = 0;
		/// <summary>
		/// Gets or sets the minimum allowed value for the up-down control.
		/// </summary>
		[StiSerializable]
		[DefaultValue(0)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the minimum allowed value for the up-down control.")]
        [StiOrder(StiPropertyOrder.DialogMinimum)]
        [StiPropertyLevel(StiLevel.Basic)]
		public int Minimum
		{
			get 
			{
				return minimum;
			}
			set 
			{
				minimum = value;
				UpdateReportControl("Minimum");
			}
		}


		private int maximum = 100;
		/// <summary>
		/// Gets or sets the maximum value for the up-down control.
		/// </summary>
		[StiSerializable]
		[DefaultValue(100)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the maximum value for the up-down control.")]
        [StiOrder(StiPropertyOrder.DialogMaximum)]
        [StiPropertyLevel(StiLevel.Basic)]
		public int Maximum
		{
			get 
			{
				return maximum;
			}
			set 
			{
				maximum = value;
				UpdateReportControl("Maximum");
			}
		}


		private int increment = 1;
		/// <summary>
		/// Gets or sets the value to increment or decrement the up-down control when the up or down buttons are clicked.
		/// </summary>
		[StiSerializable]
		[DefaultValue(1)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the value to increment or decrement the up-down control when the up or down buttons are clicked.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogIncrement)]
        [StiPropertyLevel(StiLevel.Basic)]
		public int Increment
		{
			get 
			{
				return increment;
			}
			set 
			{
				increment = value;
				UpdateReportControl("Increment");
			}
		}


		private int valueData = 0;
		/// <summary>
		/// Gets or sets the value assigned to the up-down control.
		/// </summary>
		[StiSerializable]
		[DefaultValue(0)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the value assigned to the up-down control.")]
        [StiOrder(StiPropertyOrder.DialogValue)]
        [StiPropertyLevel(StiLevel.Basic)]
		public int Value
		{
			get 
			{
				return valueData;
			}
			set 
			{
				valueData = value;
				UpdateReportControl("Value");
			}
		}

		#endregion

		#region Events
		#region OnValueChanged
		private static readonly object EventValueChanged = new object();

		/// <summary>
		/// Occurs when the Value property has been changed in some way.
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
            return new StiNumericUpDownControl();
        }
        #endregion

		/// <summary>
		/// Creates a new StiNumericUpDownControl.
		/// </summary>
		public StiNumericUpDownControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiNumericUpDownControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiNumericUpDownControl(RectangleD rect) : base(rect)
		{
			base.BackColor = SystemColors.Window;
			PlaceOnToolbox = true;
		}
	}
}