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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Represents a Windows List View control for displaying an ListView.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiListViewControl), "Stimulsoft.Report.Dialogs.Bmp.StiListViewControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiListViewControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiListViewControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiGuiMode(StiGuiMode.Gdi)]
	public class StiListViewControl : StiReportControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiListViewControl
            jObject.AddPropertyJObject("SelectedIndexChangedEvent", SelectedIndexChangedEvent.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "SelectedIndexChangedEvent":
                        {
                            var _event = new StiSelectedIndexChangedEvent();
                            _event.LoadFromJsonObject((JObject)property.Value);
                            this.SelectedIndexChangedEvent = _event;
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
                return StiComponentId.StiListViewControl;
            }
        }

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.ListViewControl;
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
				return StiLocalization.Get("Dialogs", "StiListViewControl");
			}
		}
		#endregion
        
		#region Controls Property
		private ListView control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public ListView Control
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
		#endregion

		#region Report Control Override
		/// <summary>
		/// Gets default event for this report control.
		/// </summary>
		/// <returns>Default event.</returns>
		public override StiEvent GetDefaultEvent()
		{
			return this.ClickEvent;
		}
		#endregion

		#region Events
		#region OnSelectedIndexChanged
		private static readonly object EventSelectedIndexChanged = new object();

		public event EventHandler SelectedIndexChanged
		{
			add
			{
				base.Events.AddHandler(EventSelectedIndexChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(EventSelectedIndexChanged, value);
			}
		}

		public void InvokeSelectedIndexChanged(EventArgs e)
		{
			var handler = base.Events[EventSelectedIndexChanged] as EventHandler;
			if (handler != null)handler(this, e);

			StiBlocklyHelper.InvokeBlockly(this.Report, this, SelectedIndexChangedEvent);
		}


		/// <summary>
		/// Occurs when the SelectedIndex property has changed.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Occurs when the SelectedIndex property has changed.")]
		public virtual StiSelectedIndexChangedEvent SelectedIndexChangedEvent
		{
			get
			{				
				return new StiSelectedIndexChangedEvent(this);
			}
			set
			{
				if (value != null)value.Set(this, value.Script);
			}
		}
		#endregion
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiListViewControl();
        }
        #endregion

		#region this
		/// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public override RectangleD DefaultClientRectangle
		{
			get
			{
				return new RectangleD(0, 0, 96, 96);
			}
		}

		/// <summary>
		/// Creates a new StiListViewControl.
		/// </summary>
		public StiListViewControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiListViewControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiListViewControl(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;	
			base.BackColor = SystemColors.Window;
		}
		#endregion
	}
}