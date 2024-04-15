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
using Stimulsoft.Report.Controls;
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
	/// Represents a Windows list box control.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiListBoxControl), "Stimulsoft.Report.Dialogs.Bmp.StiListBoxControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiListBoxControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiListBoxControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiListBoxControl : StiReportControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("ForeColor");

            // StiListBoxControl
            jObject.AddPropertyJObject("SelectedIndexChangedEvent", SelectedIndexChangedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("ItemsBinding", ItemsBinding);
            jObject.AddPropertyStringNullOrEmpty("SelectedItemBinding", SelectedItemBinding);
            jObject.AddPropertyStringNullOrEmpty("SelectedValueBinding", SelectedValueBinding);
            jObject.AddPropertyStringNullOrEmpty("SelectedIndexBinding", SelectedIndexBinding);
            jObject.AddPropertyInt("ItemHeight", ItemHeight, 13);
            jObject.AddPropertyEnum("SelectionMode", SelectionMode, SelectionMode.One);
            jObject.AddPropertyBool("Sorted", Sorted);
            //jObject.Add("Items", Items);              // Anton

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

                    case "ItemsBinding":
                        this.itemsBinding = property.DeserializeString();
                        break;

                    case "SelectedItemBinding":
                        this.selectedItemBinding = property.DeserializeString();
                        break;

                    case "SelectedValueBinding":
                        this.selectedValueBinding = property.DeserializeString();
                        break;

                    case "SelectedIndexBinding":
                        this.selectedIndexBinding = property.DeserializeString();
                        break;

                    case "ItemHeight":
                        this.itemHeight = property.DeserializeInt();
                        break;

                    case "SelectionMode":
                        this.selectionMode = property.DeserializeEnum<SelectionMode>();
                        break;

                    case "Sorted":
                        this.sorted = property.DeserializeBool();
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
                return StiComponentId.StiListBoxControl;
            }
        }

		/// <summary>
		/// Return events collection of this component;
		/// </summary>
		public override StiEventsCollection GetEvents()
		{
			StiEventsCollection events = base.GetEvents();
			if (SelectedIndexChangedEvent != null)events.Add(SelectedIndexChangedEvent);
			return events;
		}

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.ListBoxControl;
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
				return StiLocalization.Get("Dialogs", "StiListBoxControl");
			}
		}
		#endregion

		#region Events
		#region OnSelectedIndexChanged
		private static readonly object EventSelectedIndexChanged = new object();

		/// <summary>
		/// Occurs when the SelectedIndex property has changed.
		/// </summary>
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
		/// Gets or sets a script of the event SelectedIndexChanged.
		/// </summary>
		[StiSerializable]
		[StiCategory("ControlsEvents")]
		[Browsable(false)]
		[Description("Gets or sets a script of the event SelectedIndexChanged.")]
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

		#region Controls Property
		private ListBox control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public ListBox Control
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


		private string itemsBinding = "";
		/// <summary>
		/// Gets the data bindings for the items.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the items.")]
        [StiOrder(StiPropertyOrder.DialogItemsBinding)]
		public string ItemsBinding
		{
			get
			{
				return itemsBinding;
			}
			set
			{
				itemsBinding = value;
			}
		}


		private string selectedItemBinding = "";
		/// <summary>
		/// Gets the data bindings for the selected item.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the selected item.")]
        [StiOrder(StiPropertyOrder.DialogSelectedItemBinding)]
		public string SelectedItemBinding
		{
			get
			{
				return selectedItemBinding;
			}
			set
			{
				selectedItemBinding = value;
			}
		}


		private string selectedValueBinding = "";
		/// <summary>
		/// Gets the data bindings for the selected value.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the selected value.")]
        [StiOrder(StiPropertyOrder.DialogSelectedValueBinding)]
		public string SelectedValueBinding
		{
			get
			{
				return selectedValueBinding;
			}
			set
			{
				selectedValueBinding = value;
			}
		}


		private string selectedIndexBinding = "";
		/// <summary>
		/// Gets the data bindings for the selected index.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the selected index.")]
        [StiOrder(StiPropertyOrder.DialogSelectedIndexBinding)]
		public string SelectedIndexBinding
		{
			get
			{
				return selectedIndexBinding;
			}
			set
			{
				selectedIndexBinding = value;
			}
		}


		private void OnCollectionChanged(object sender, EventArgs e)
		{
			UpdateReportControl("Items");
		}


		[StiNonSerialized]
		[Browsable(false)]
		public override Color ForeColor
		{
			get 
			{
				return base.ForeColor;
			}
			set 
			{
				base.ForeColor = value;
			}
		}


		private StiArrayList items;
		/// <summary>
		/// Gets the items of the ListBox.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
		[StiCategory("Data")]
        [Editor(StiEditors.StringCollection, typeof(UITypeEditor))]
        [Description("Gets the items of the ListBox.")]
        [StiOrder(StiPropertyOrder.DialogItems)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual StiArrayList Items
		{
			get
			{
				return items;
			}
			set
			{
				items = value;
				items.CollectionChanged += new EventHandler(OnCollectionChanged);
				UpdateReportControl("Items");
			}
		}


		private int itemHeight = 13;
		/// <summary>
		/// Gets or sets the height of an item in the ListBox.
		/// </summary>
		[StiSerializable]
		[DefaultValue(13)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the height of an item in the ListBox.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogItemHeight)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual int ItemHeight
		{
			get
			{
				return itemHeight;
			}
			set
			{
				itemHeight = value;
				UpdateReportControl("ItemHeight");
			}
		}



		private int selectedIndex = -1;
		/// <summary>
		/// Gets or sets the zero-based index of the currently selected item in a ListBox.
		/// </summary>
		[Browsable(false)]
		public virtual int SelectedIndex
		{
			get
			{
				return selectedIndex;
			}
			set
			{
				selectedIndex = value;
				UpdateReportControl("SelectedIndex");
			}
		}


		private object selectedItem = null;
		/// <summary>
		/// Gets or sets the currently selected item in the ListBox.
		/// </summary>
		[Browsable(false)]
		public virtual object SelectedItem
		{
			get
			{
				return selectedItem;
			}
			set
			{
				selectedItem = value;
				UpdateReportControl("SelectedItem");
			}
		}


		private object selectedValue = null;
		/// <summary>
		/// Gets or sets the value of the member property specified by the ValueMember property.
		/// </summary>
		[Browsable(false)]
		public virtual object SelectedValue
		{
			get
			{
				return selectedValue;
			}
			set
			{
				selectedValue = value;
				UpdateReportControl("SelectedValue");
			}
		}


		private SelectionMode selectionMode = SelectionMode.One;
		/// <summary>
		/// Gets or sets the method in which items are selected in the ListBox.
		/// </summary>
		[StiSerializable]
		[DefaultValue(SelectionMode.One)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets the method in which items are selected in the ListBox.")]
        [StiOrder(StiPropertyOrder.DialogSelectionMode)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual SelectionMode SelectionMode
		{
			get
			{
				return selectionMode;
			}
			set
			{
				selectionMode = value;
				UpdateReportControl("SelectionMode");
			}
		}


		private bool sorted = false;
		/// <summary>
		/// Gets or sets a value indicating whether the items in the ListBox are sorted alphabetically.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether the items in the ListBox are sorted alphabetically.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogSorted)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool Sorted
		{
			get
			{
				return sorted;
			}
			set
			{
				sorted = value;
				UpdateReportControl("Sorted");
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
			return this.SelectedIndexChangedEvent;
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiListBoxControl();
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
		/// Creates a new StiListBoxControl.
		/// </summary>
		public StiListBoxControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiListBoxControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiListBoxControl(RectangleD rect) : base(rect)
		{
			items = new StiArrayList();
			items.CollectionChanged += new EventHandler(OnCollectionChanged);

			base.BackColor = SystemColors.Window;
			PlaceOnToolbox = true;
		}
		#endregion
	}
}
