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
using System.Collections.Generic;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the class that realizes relations between Data Sources.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiDataRelationConverter))]
	public class StiDataRelation : 
		ICloneable,
		IStiName,
        IStiAlias,
        IStiInherited,
        IStiPropertyGridObject, 
        IStiJsonReportObject,
	    IStiAppDataRelation
    {
        #region enum Order
        public enum Order
        {
            NameInSource = 100,
            Name = 200,
            Alias = 300,
            Active = 350,
            ParentSource = 400,
            ChildSource = 500,
            ParentColumns = 600,
            ChildColumns = 700
        }
        #endregion

        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            // StiDataRelation
            jObject.AddPropertyBool("Inherited", Inherited);
            jObject.AddPropertyStringNullOrEmpty("Name", Name);
            jObject.AddPropertyStringArray("ChildColumns", ChildColumns);
            jObject.AddPropertyStringArray("ParentColumns", ParentColumns);
            jObject.AddPropertyStringNullOrEmpty("NameInSource", NameInSource);
            jObject.AddPropertyStringNullOrEmpty("Alias", Alias);
            jObject.AddPropertyBool("IsCloud", IsCloud);
            jObject.AddPropertyBool("Active", Active);
            jObject.AddPropertyStringNullOrEmpty("Key", Key);

            if (ParentSource != null)
                jObject.AddPropertyStringNullOrEmpty("ParentSource", ParentSource.Name);

            if (ChildSource != null)
                jObject.AddPropertyStringNullOrEmpty("ChildSource", ChildSource.Name);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Inherited":
                        this.Inherited = property.DeserializeBool();
                        break;

                    case "Active":
                        this.Active = property.DeserializeBool();
                        break;

                    case "Name":
                        this.name = property.DeserializeString();
                        break;

                    case "ChildColumns":
                        this.ChildColumns = property.DeserializeStringArray();
                        break;

                    case "ParentColumns":
                        this.ParentColumns = property.DeserializeStringArray();
                        break;

                    case "NameInSource":
                        this.NameInSource = property.DeserializeString();
                        break;

                    case "Alias":
                        this.Alias = property.DeserializeString();
                        break;

                    case "IsCloud":
                        {
                            var obj = property.Value.ToObject<bool?>();
                            this.IsCloud = obj != null && (bool)obj;
                        }
                        break;

                    case "Key":
                        this.Key = property.DeserializeString();
                        break;

                    case "ParentSource":
                        this.ParentSource = this.Dictionary.DataSources[property.DeserializeString()];
                        break;

                    case "ChildSource":
                        this.ChildSource = this.Dictionary.DataSources[property.DeserializeString()];
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
		[Browsable(false)]
        public StiComponentId ComponentId => StiComponentId.StiDataRelation;

        [Browsable(false)]
        public string PropName => this.name;

        public StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.NameInSource(),
                propHelper.Alias(),
                propHelper.Name()
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region IStiInherited
        [Browsable(false)]
		[DefaultValue(false)]
		[StiSerializable]
		public bool Inherited { get; set; }
        #endregion

		#region IStiName
		private string name;
		/// <summary>
		/// Gets or sets relation name.
		/// </summary>
		[StiSerializable]
		[StiCategory("Data")]
		[Description("Gets or sets relation name.")]
		[ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Name)]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (Dictionary != null &&
					Dictionary.Report != null &&
					Dictionary.Report.IsDesigning)
				{
					if (StiOptions.Designer.AutoCorrectDataRelationName)
					{
						value = StiNameValidator.CorrectName(value, Dictionary.Report);
					}
				}

				name = value;
			}
		}
        #endregion

        #region IStiAlias
        /// <summary>
		/// Gets or sets alias of relation.
		/// </summary>
		[StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets alias of relation.")]
        [ParenthesizePropertyName(true)]
        [StiOrder((int)Order.Alias)]
        public string Alias { get; set; }
        #endregion

        #region IStiAppDataRelation
        string IStiAppDataRelation.GetName()
        {
            return Name;
        }

        /// <summary>
        /// Returns reference to the dictionary which contains this datasource.
        /// </summary>
        /// <returns>Reference to the app.</returns>
        IStiAppDictionary IStiAppDataRelation.GetDictionary()
        {
            return Dictionary;
        }

        /// <summary>
        /// Returns parent data source of this relation.
        /// </summary>
        /// <returns>Reference to the data source.</returns>
        IStiAppDataSource IStiAppDataRelation.GetParentDataSource()
        {
            return ParentSource;
        }

        /// <summary>
        /// Returns child data source of this relation.
        /// </summary>
        /// <returns>Reference to the data source.</returns>
        IStiAppDataSource IStiAppDataRelation.GetChildDataSource()
        {
            return ChildSource;
        }

        /// <summary>
        /// Returns an enumeration of the parent column keys of the data relation.
        /// </summary>
        /// <returns>An reference to the enumeration.</returns>
        IEnumerable<string> IStiAppDataRelation.FetchParentColumns()
        {
            return ParentColumns;
        }

        /// <summary>
        /// Returns an enumeration of the child column keys of the data relation.
        /// </summary>
        /// <returns>An reference to the enumeration.</returns>
        IEnumerable<string> IStiAppDataRelation.FetchChildColumns()
        {
            return ChildColumns;
        }

        /// <summary>
        /// Returns the status of the relation.
        /// </summary>
        /// <returns>The status of the relation.</returns>
        bool IStiAppDataRelation.GetActiveState()
        {
            return Active;
        }
        #endregion

        #region IStiAppCell
        string IStiAppCell.GetKey()
        {
            Key = StiKeyHelper.GetOrGeneratedKey(Key);

            return Key;
        }

        void IStiAppCell.SetKey(string key)
        {
            Key = key;
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
        /// <summary>
		/// Gets or sets the dictionary of data in which the relation is kept.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Reference)]
		[Browsable(false)]
		public StiDictionary Dictionary { get; set; }

        /// <summary>
		/// Gets or sets Parent data source.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Reference)]
		[StiCategory("Data")]
		[Description("Gets or sets Parent data source.")]
        [StiOrder((int)Order.ParentSource)]
		public StiDataSource ParentSource { get; set; }

        /// <summary>
		/// Gets or sets Child data source.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.Reference)]
		[StiCategory("Data")]
		[Description("Gets or sets Child data source.")]
        [StiOrder((int)Order.ChildSource)]
		public StiDataSource ChildSource { get; set; }

        /// <summary>
		/// Gets or sets collection of child column names.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
		[StiCategory("Data")]
        [Description("Gets or sets collection of child column names.")]
        [TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiChildColumnsConverter))]
        [StiOrder((int)Order.ChildColumns)]
		public string[] ChildColumns { get; set; }

        /// <summary>
		/// Gets or sets collection of parent column names.
		/// </summary>
		[StiSerializable(StiSerializationVisibility.List)]
		[StiCategory("Data")]
        [Description("Gets or sets collection of parent column names.")]
        [TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiParentColumnsConverter))]
        [StiOrder((int)Order.ParentColumns)]
		public string[] ParentColumns { get; set; }

        /// <summary>
		/// Please instead property RelationName use property NameInSource.
		/// </summary>
		[StiNonSerialized]
		[Browsable(false)]
		[Obsolete("Please instead property RelationName use property NameInSource.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RelationName
		{
			get
			{
				return this.NameInSource;
			}
			set
			{
				this.NameInSource = value;
			}
		}

        /// <summary>
		/// Gets or sets name of relation in source of data.
		/// </summary>
		[StiSerializable]
		[StiCategory("Data")]
		[Description("Gets or sets name of relation in source of data.")]
        [StiOrder((int)Order.NameInSource)]
        [ParenthesizePropertyName(true)]
		public string NameInSource { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that this data relation is created dynamically by the Stimulsoft Server.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        [StiSerializable]
        public bool IsCloud { get; set; }

        /// <summary>
        /// Gets or sets value which indicates that this data relation is active.
        /// </summary>
        [StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets value which indicates that this data relation is active.")]
        [StiOrder((int)Order.Active)]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the key of the dictionary object.
        /// </summary>
        [DefaultValue(null)]
        [StiSerializable]
        [Browsable(false)]
        public string Key { get; set; }
        #endregion
		
		#region Methods
		public override string ToString()
		{
			return ToString(false);
		}

        public string ToString(bool onlyAlias)
        {
            if (onlyAlias && !string.IsNullOrWhiteSpace(Alias)) return Alias;
            if (Name == Alias || string.IsNullOrWhiteSpace(Alias)) return Name;

            return $"{Name} [{Alias}]";
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiRelation.
        /// </summary>
        public StiDataRelation() : this("", null, null, new string[0], new string[0])
		{
		}
		
		/// <summary>
		/// Creates a new object of the type StiRelation.
		/// </summary>
		/// <param name="nameInSource">Name of relation in source.</param>
		/// <param name="parentSource">Parent data source.</param>
		/// <param name="childSource">Child data source.</param>
		/// <param name="parentColumns">Columns parent.</param>
		/// <param name="childColumns">Columns child.</param>
		public StiDataRelation(
			string nameInSource, 
			StiDataSource parentSource, StiDataSource childSource, 
			string[] parentColumns, string[] childColumns) : 
			this(nameInSource, nameInSource, nameInSource, parentSource, childSource, parentColumns, childColumns)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiRelation.
		/// </summary>
        /// <param name="nameInSource">Name of relation in source.</param>
		/// <param name="name">Name of relation.</param>
		/// <param name="alias">Alias of relation.</param>
		/// <param name="parentSource">Parent data source.</param>
		/// <param name="childSource">Child data source.</param>
		/// <param name="parentColumns">Columns parent.</param>
		/// <param name="childColumns">Columns child.</param>
		public StiDataRelation(
			string nameInSource, 
			string name,
			string alias,
			StiDataSource parentSource, StiDataSource childSource, 
			string[] parentColumns, string[] childColumns)
		{
			this.NameInSource = nameInSource;
			this.name = name;
			this.Alias = alias;
			this.ParentSource = parentSource;
			this.ChildSource = childSource;
			this.ParentColumns = parentColumns;
			this.ChildColumns = childColumns;
		}

        /// <summary>
        /// Creates a new object of the type StiRelation.
        /// </summary>
        /// <param name="nameInSource">Name of relation in source.</param>
        /// <param name="name">Name of relation.</param>
        /// <param name="alias">Alias of relation.</param>
        /// <param name="parentSource">Parent data source.</param>
        /// <param name="childSource">Child data source.</param>
        /// <param name="parentColumns">Columns parent.</param>
        /// <param name="childColumns">Columns child.</param>
        /// <param name="key">Key string.</param>
        public StiDataRelation(
            string nameInSource,
            string name,
            string alias,
            StiDataSource parentSource, StiDataSource childSource,
            string[] parentColumns, string[] childColumns, string key)
        {
            this.NameInSource = nameInSource;
            this.name = name;
            this.Alias = alias;
            this.ParentSource = parentSource;
            this.ChildSource = childSource;
            this.ParentColumns = parentColumns;
            this.ChildColumns = childColumns;
            this.Key = key;
        }

        /// <summary>
        /// Creates a new object of the type StiRelation.
        /// </summary>
        /// <param name="nameInSource">Name of relation in source.</param>
        /// <param name="name">Name of relation.</param>
        /// <param name="alias">Alias of relation.</param>
        /// <param name="parentSource">Parent data source.</param>
        /// <param name="childSource">Child data source.</param>
        /// <param name="parentColumns">Columns parent.</param>
        /// <param name="childColumns">Columns child.</param>
        /// <param name="active">Is active relation.</param>
        public StiDataRelation(
            string nameInSource,
            string name,
            string alias,
            StiDataSource parentSource, StiDataSource childSource,
            string[] parentColumns, string[] childColumns, bool active)
        {
            this.NameInSource = nameInSource;
            this.name = name;
            this.Alias = alias;
            this.ParentSource = parentSource;
            this.ChildSource = childSource;
            this.ParentColumns = parentColumns;
            this.ChildColumns = childColumns;
            this.Active = active;
        }

        /// <summary>
        /// Creates a new object of the type StiRelation.
        /// </summary>
        /// <param name="nameInSource">Name of relation in source.</param>
        /// <param name="name">Name of relation.</param>
        /// <param name="alias">Alias of relation.</param>
        /// <param name="parentSource">Parent data source.</param>
        /// <param name="childSource">Child data source.</param>
        /// <param name="parentColumns">Columns parent.</param>
        /// <param name="childColumns">Columns child.</param>
        /// <param name="key">Key string.</param>
        /// <param name="active">Is active relation.</param>
        public StiDataRelation(
            string nameInSource,
            string name,
            string alias,
            StiDataSource parentSource, StiDataSource childSource,
            string[] parentColumns, string[] childColumns, string key, bool active)
        {
            this.NameInSource = nameInSource;
            this.name = name;
            this.Alias = alias;
            this.ParentSource = parentSource;
            this.ChildSource = childSource;
            this.ParentColumns = parentColumns;
            this.ChildColumns = childColumns;
            this.Key = key;
            this.Active = active;
        }
    }
}
