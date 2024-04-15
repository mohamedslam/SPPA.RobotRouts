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

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dialogs
{
	
	/// <summary>
	/// Represents a Windows combo box control.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiComboBoxControl), "Stimulsoft.Report.Dialogs.Bmp.StiLookUpBoxControl.gif")]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiLookUpBoxControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiLookUpBoxControl : StiComboBoxControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiLookUpBoxControl
            jObject.AddPropertyStringNullOrEmpty("KeysBinding", KeysBinding);
            jObject.AddPropertyStringNullOrEmpty("SelectedKeyBinding", SelectedKeyBinding);
            //jObject.Add("Keys", Keys);            // Anton

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "KeysBinding":
                        this.keysBinding = property.DeserializeString();
                        break;

                    case "SelectedKeyBinding":
                        this.selectedKeyBinding = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

		#region LookUpContainer
		internal class LookUpContainer
		{
			public string Value;
			public object Key;

			public override string ToString()
			{
				return Value;
			}

			public LookUpContainer(string value, object key)
			{
				this.Value = value;
				this.Key = key;
			}
		}
		#endregion

		#region StiComponent override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiLookUpBoxControl;
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
				return (int)StiReportControlToolboxPosition.LookUpBoxControl;
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
				return StiLocalization.Get("Dialogs", "StiLookUpBoxControl");
			}
		}
		#endregion

		#region Controls Property
		private StiArrayList keys;
		/// <summary>
		/// Gets an object representing the collection of the keys.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
		[StiCategory("Data")]
        [Editor(StiEditors.StringCollection, typeof(UITypeEditor))]
        [Description("Gets an object representing the collection of the keys.")]
        [StiOrder(StiPropertyOrder.DialogKeys)]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual StiArrayList Keys
		{
			get
			{
				return keys;
			}
			set
			{
				keys = value;
				keys.CollectionChanged += new EventHandler(OnKeysCollectionChanged);
				UpdateReportControl("Keys");
			}
		}

		private void OnKeysCollectionChanged(object sender, EventArgs e)
		{
			UpdateReportControl("Keys");
		}


		private string keysBinding = "";
		/// <summary>
		/// Gets the data bindings for the keys.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the keys.")]
        [StiOrder(StiPropertyOrder.DialogKeysBinding)]
		public string KeysBinding
		{
			get
			{
				return keysBinding;
			}
			set
			{
				keysBinding = value;
			}
		}


		private object selectedKey = null;
		/// <summary>
		/// Gets or sets currently selected key in the LookUpBox.
		/// </summary>
		[Browsable(false)]
		public virtual object SelectedKey
		{
			get
			{
				return selectedKey;
			}
			set
			{
				selectedKey = value;
				UpdateReportControl("SelectedKey");
			}
		}


		private string selectedKeyBinding = "";
		/// <summary>
		/// Gets the data bindings for the selected key.
		/// </summary>
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
		[Description("Gets the data bindings for the selected key.")]
        [StiOrder(StiPropertyOrder.DialogSelectedKeyBinding)]
		public string SelectedKeyBinding
		{
			get
			{
				return selectedKeyBinding;
			}
			set
			{
				selectedKeyBinding = value;
			}
		}

		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiLookUpBoxControl();
        }
        #endregion

		#region this
		/// <summary>
		/// Creates a new StiComboBoxControl.
		/// </summary>
		public StiLookUpBoxControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiComboBoxControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiLookUpBoxControl(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;
			Text = "LookUpBox";

			keys = new StiArrayList();
			keys.CollectionChanged += new EventHandler(OnKeysCollectionChanged);
		}
		#endregion
	}
}