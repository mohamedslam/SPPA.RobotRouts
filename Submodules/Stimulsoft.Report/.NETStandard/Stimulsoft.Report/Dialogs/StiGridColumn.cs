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
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Base.Json.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Describes a grid column.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Dialogs.Design.StiGridColumnConverter))]
	[StiSerializable]
	public class StiGridColumn : 
        ICloneable,
        IStiJsonReportObject
	{
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            // StiGridColumn
            jObject.AddPropertyStringNullOrEmpty("DataTextField", DataTextField);
            jObject.AddPropertyBool("Visible", Visible, true);
            jObject.AddPropertyInt("Width", Width);
            jObject.AddPropertyEnum("Alignment", Alignment, HorizontalAlignment.Left);
            jObject.AddPropertyStringNullOrEmpty("HeaderText", HeaderText);
            jObject.AddPropertyStringNullOrEmpty("NullText", NullText);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "DataTextField":
                        this.dataTextField = property.DeserializeString();
                        break;

                    case "Visible":
                        this.visible = property.DeserializeBool();
                        break;

                    case "Width":
                        this.width = property.DeserializeInt();
                        break;

                    case "Alignment":
                        this.alignment = property.DeserializeEnum<HorizontalAlignment>();
                        break;

                    case "HeaderText":
                        this.headerText = property.DeserializeString();
                        break;

                    case "NullText":
                        this.nullText = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion

		#region Properties
		private string dataTextField;
        /// <summary>
        /// Gets or sets name of data column to be shown.
        /// </summary>
		[Editor("Stimulsoft.Report.Dialogs.Design.StiGridDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("Data")]
        [Description("Gets or sets name of data column to be shown.")]
        [StiOrder(StiPropertyOrder.DialogDataTextField)]
		public string DataTextField
		{
			get
			{
				return dataTextField;
			}
			set
			{
				dataTextField = value;
			}
		}


		private bool visible = true;
        /// <summary>
        /// Gets or sets visiblity of column.
        /// </summary>
		[DefaultValue(true)]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("Data")]
        [Description("Gets or sets visiblity of column.")]
        [StiOrder(StiPropertyOrder.DialogVisible)]
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}


		private int width = 0;
        /// <summary>
        /// Gets or sets width of column.
        /// </summary>
		[DefaultValue(0)]
		[StiCategory("Data")]
        [Description("Gets or sets width of column.")]
        [StiOrder(StiPropertyOrder.DialogWidth)]
		public int Width
		{
			get
			{
				return width;
			}
			set
			{				
				width = value;
			}
		}


		private HorizontalAlignment alignment = HorizontalAlignment.Left;
        /// <summary>
        /// Gets or sets horizontal alignment of column content.
        /// </summary>
		[DefaultValue(HorizontalAlignment.Left)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiCategory("Data")]
        [Description("Gets or sets horizontal alignment of column content.")]
        [StiOrder(StiPropertyOrder.DialogAlignment)]
		public HorizontalAlignment Alignment
		{
			get
			{
				return alignment;
			}
			set
			{
				alignment = value;
			}
		}


		private string headerText = "";
        /// <summary>
        /// Gets or sets header text of column.
        /// </summary>
		[DefaultValue("")]
		[StiCategory("Data")]
        [Description("Gets or sets header text of column.")]
        [StiOrder(StiPropertyOrder.DialogHeaderText)]
		public string HeaderText
		{
			get
			{
				return headerText;
			}
			set
			{
				headerText = value;
			}
		}


		private string nullText = "";
        /// <summary>
        /// Gets or sets text which will be shown instead null values.
        /// </summary>
		[DefaultValue("")]
		[StiCategory("Data")]
        [Description("Gets or sets text which will be shown instead null values.")]
        [StiOrder(StiPropertyOrder.DialogNullText)]
		public string NullText
		{
			get
			{
				return nullText;
			}
			set
			{
				nullText = value;
			}
		}

		#endregion

		private StiGridColumnsCollection gridColumnCollection = null;
		[Browsable(false)]
		public StiGridColumnsCollection GridColumnCollection
		{
			get
			{
				return gridColumnCollection;
			}
			set
			{
				gridColumnCollection = value;
			}
		}

		public override string ToString()
		{
			if (gridColumnCollection == null)return "Column";
			return "Column" + (gridColumnCollection.IndexOf(this) + 1).ToString();
		}


		/// <summary>
		/// Creates a new object of the type StiGridColumn.
		/// </summary>
		public StiGridColumn() : this(string.Empty, true, 0, HorizontalAlignment.Left,
			"", "")
		{
		}


		/// <summary>
		/// Creates a new object of the type StiGridColumn.
		/// </summary>
		/// <param name="dataTextField"></param>
		/// <param name="visible">Visibility of column in grid.</param>
		public StiGridColumn(string dataTextField, bool visible, int width, HorizontalAlignment alignment, 
			string headerText, string nullText)
		{
			this.dataTextField = dataTextField;
			this.visible = visible;
			this.width = width;
			this.alignment = alignment;
			this.headerText = headerText;
			this.nullText = nullText;
		}
	}
}
