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
using Stimulsoft.Report.Controls;
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
	/// Represents a Windows combo box control.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiComboBoxControl), "Stimulsoft.Report.Dialogs.Bmp.StiComboBoxControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiComboBoxControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiComboBoxControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiComboBoxControl : StiReportControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiComboBoxControl
            jObject.AddPropertyJObject("SelectedIndexChangedEvent", SelectedIndexChangedEvent.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("TextBinding", TextBinding);
            jObject.AddPropertyStringNullOrEmpty("SelectedItemBinding", SelectedItemBinding);
            jObject.AddPropertyStringNullOrEmpty("SelectedValueBinding", SelectedValueBinding);
            jObject.AddPropertyStringNullOrEmpty("ItemsBinding", ItemsBinding);
            jObject.AddPropertyString("Text", Text);
            jObject.AddPropertyEnum("DropDownStyle", DropDownStyle, ComboBoxStyle.DropDown);
            jObject.AddPropertyInt("ItemHeight", ItemHeight, 14);
            jObject.AddPropertyInt("MaxLength", MaxLength);
            jObject.AddPropertyInt("MaxDropDownItems", MaxDropDownItems, 8);
            jObject.AddPropertyInt("DropDownWidth", DropDownWidth, 121);
            jObject.AddPropertyBool("Sorted", Sorted);
            //jObject.Add("Items", Items);             //Anton

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

                    case "TextBinding":
                        this.textBinding = property.DeserializeString();
                        break;

                    case "SelectedItemBinding":
                        this.selectedItemBinding = property.DeserializeString();
                        break;

                    case "SelectedValueBinding":
                        this.selectedValueBinding = property.DeserializeString();
                        break;

                    case "ItemsBinding":
                        this.itemsBinding = property.DeserializeString();
                        break;

                    case "Text":
                        this.text = property.DeserializeString();
                        break;

                    case "DropDownStyle":
                        this.dropDownStyle = property.DeserializeEnum<ComboBoxStyle>();
                        break;

                    case "ItemHeight":
                        this.itemHeight = property.DeserializeInt();
                        break;

                    case "MaxLength":
                        this.maxLength = property.DeserializeInt();
                        break;

                    case "MaxDropDownItems":
                        this.maxDropDownItems = property.DeserializeInt();
                        break;

                    case "DropDownWidth":
                        this.dropDownWidth = property.DeserializeInt();
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
                return StiComponentId.StiComboBoxControl;
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
				return (int)StiReportControlToolboxPosition.ComboBoxControl;
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
				return StiLocalization.Get("Dialogs", "StiComboBoxControl");
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
		private ComboBox control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public ComboBox Control
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


		private string text = "ComboBox";
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


		private void OnCollectionChanged(object sender, EventArgs e)
		{
			UpdateReportControl("Items");
		}


		private StiArrayList items;
		/// <summary>
		/// Gets an object representing the collection of the items.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
		[StiCategory("Data")]
        [Editor(StiEditors.StringCollection, typeof(UITypeEditor))]
        [Description("Gets an object representing the collection of the items.")]
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


		private ComboBoxStyle dropDownStyle = ComboBoxStyle.DropDown;
		/// <summary>
		/// Gets or sets a value specifying the style.
		/// </summary>
		[StiSerializable]
		[DefaultValue(ComboBoxStyle.DropDown)]
		[StiCategory("Appearance")]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets a value specifying the style.")]
        [StiOrder(StiPropertyOrder.DialogDropDownStyle)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual ComboBoxStyle DropDownStyle
		{
			get
			{
				return dropDownStyle;
			}
			set
			{
				dropDownStyle = value;
				UpdateReportControl("DropDownStyle");
			}
		}


		private int itemHeight = 14;
		/// <summary>
		/// Gets or sets the height of an item.
		/// </summary>
		[StiSerializable]
		[DefaultValue(14)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the height of an item.")]
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
		/// Gets or sets the index specifying the currently selected item.
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
		/// Gets or sets currently selected item.
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


		private int maxLength = 0;
		/// <summary>
		/// Gets or sets the maximum number of characters allowed.
		/// </summary>
		[StiSerializable]
		[DefaultValue(0)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the maximum number of characters allowed.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogMaxLength)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual int MaxLength
		{
			get
			{
				return maxLength;
			}
			set
			{
				maxLength = value;
				UpdateReportControl("MaxLength");
			}
		}


		private int maxDropDownItems = 8;
		/// <summary>
		/// Gets or sets the maximum number.
		/// </summary>
		[StiSerializable]
		[DefaultValue(8)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the maximum number.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogMaxDropDownItems)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual int MaxDropDownItems
		{
			get
			{
				return maxDropDownItems;
			}
			set
			{
				maxDropDownItems = value;
				UpdateReportControl("MaxDropDownItems");
			}
		}


		private int dropDownWidth = 121;
		/// <summary>
		/// Gets or sets the width of the of the drop-down portion.
		/// </summary>
		[StiSerializable]
		[DefaultValue(121)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the width of the of the drop-down portion.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogDropDownWidth)]
        [StiPropertyLevel(StiLevel.Professional)]
		public virtual int DropDownWidth
		{
			get
			{
				return dropDownWidth;
			}
			set
			{
				dropDownWidth = value;
				UpdateReportControl("DropDownWidth");
			}
		}


		private bool sorted = false;
		/// <summary>
		/// Gets or sets a value indicating whether the items are sorted.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether the items are sorted.")]
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
            return new StiComboBoxControl();
        }
        #endregion

		/// <summary>
		/// Creates a new StiComboBoxControl.
		/// </summary>
		public StiComboBoxControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiComboBoxControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiComboBoxControl(RectangleD rect) : base(rect)
		{
			items = new StiArrayList();
			items.CollectionChanged += new EventHandler(OnCollectionChanged);

			base.BackColor = SystemColors.Window;
			PlaceOnToolbox = true;

			Text = this.LocalizedName;
		}
	}
}