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

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Represents a Windows radio button.
	/// </summary>	
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiRadioButtonControl), "Stimulsoft.Report.Dialogs.Bmp.StiRadioButtonControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiRadioButtonControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiRadioButtonControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiRadioButtonControl : StiReportControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiRadioButtonControl
            jObject.AddPropertyJObject("CheckedChangedEvent", CheckedChangedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("TextBinding", TextBinding);
            jObject.AddPropertyStringNullOrEmpty("CheckedBinding", CheckedBinding);
            jObject.AddPropertyBool("Checked", Checked);
            jObject.AddPropertyString("Text", Text);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "CheckedChangedEvent":
                        {
                            var _event = new StiCheckedChangedEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);

                            this.CheckedChangedEvent = _event;
                        }
                        break;

                    case "TextBinding":
                        this.textBinding = property.DeserializeString();
                        break;

                    case "Checked":
                        this.checkedValue = property.DeserializeBool();
                        break;

                    case "Text":
                        this.text = property.DeserializeString();
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
                return StiComponentId.StiRadioButtonControl;
            }
        }

		/// <summary>
		/// Return events collection of this component;
		/// </summary>
		public override StiEventsCollection GetEvents()
		{
			StiEventsCollection events = base.GetEvents();
			if (CheckedChangedEvent != null)events.Add(CheckedChangedEvent);
			return events;
		}


		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.RadioButtonControl;
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
				return StiLocalization.Get("Dialogs", "StiRadioButtonControl");
			}
		}
		#endregion

		#region Events
		#region OnCheckedChanged
		private static readonly object EventCheckedChanged = new object();

		/// <summary>
		/// Occurs when the value of the Checked property changes.
		/// </summary>
		public event EventHandler CheckedChanged
		{
			add
			{
				base.Events.AddHandler(EventCheckedChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventCheckedChanged, value);
			}
		}

		public void InvokeCheckedChanged(EventArgs e)
		{
			EventHandler handler = base.Events[EventCheckedChanged] as EventHandler;
			if (handler != null)handler(this, e);


			StiBlocklyHelper.InvokeBlockly(this.Report, this, CheckedChangedEvent);
		}

		/// <summary>
		/// Gets or sets a script of the event CheckedChanged.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event CheckedChanged.")]
		public virtual StiCheckedChangedEvent CheckedChangedEvent
		{
			get
			{				
				return new StiCheckedChangedEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion
		#endregion

		#region Controls Property
		private RadioButton control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public RadioButton Control
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

		private string textBinding = "";
		/// <summary>
		/// Gets the data bindings for the text.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the text.")]
        [StiOrder(StiPropertyOrder.DialogTextBinding)]
		public string TextBinding
		{
			get
			{
				return textBinding;
			}
			set
			{
				textBinding = value;
			}
		}


		private string checkedBinding = "";
		/// <summary>
		/// Gets the data bindings for the checked.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the checked.")]
        [StiOrder(StiPropertyOrder.DialogCheckedBinding)]
		public string CheckedBinding
		{
			get
			{
				return checkedBinding;
			}
			set
			{
				checkedBinding = value;
			}
		}


		private bool checkedValue = false;
		/// <summary>
		/// Gets or sets a value indicating whether the control is checked.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[DefaultValue(false)]
		[StiCategory("Appearance")]
		[Description("Gets or sets a value indicating whether the control is checked.")]
        [StiOrder(StiPropertyOrder.DialogChecked)]
        [StiPropertyLevel(StiLevel.Basic)]
		public bool Checked
		{
			get 
			{
				return checkedValue;
			}
			set 
			{
				checkedValue = value;
				UpdateReportControl("Checked");
			}
		}


		private string text = "RadioButton";
		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Browsable(true)]
		[Description("Gets or sets the text associated with this control.")]
        [StiOrder(StiPropertyOrder.DialogText)]
        [StiPropertyLevel(StiLevel.Basic)]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{				
				text = value;
				UpdateReportControl("Text");
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
			return this.CheckedChangedEvent;
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiRadioButtonControl();
        }
        #endregion

		#region this
		/// <summary>
		/// Creates a new StiRadioButtonControl.
		/// </summary>
		public StiRadioButtonControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiRadioButtonControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiRadioButtonControl(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;	
			Text = this.LocalizedName;
		}
		#endregion
	}
}