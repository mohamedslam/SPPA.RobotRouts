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

using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the base class for access to data in DataStore.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.Dictionary.Design.StiDataStoreSourceConverter))]
	public abstract class StiDataStoreSource : StiDataSource
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDataStoreSource
            jObject.AddPropertyStringNullOrEmpty("NameInSource", NameInSource);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "NameInSource":
                        this.NameInSource = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiDataStoreSource;

	    public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            // DataCategory
            var list = new[]
            {
                propHelper.Name(),
                propHelper.Alias(),
                propHelper.NameInSource(),
                propHelper.ConnectOnStart(),
            };
            objHelper.Add(StiPropertyCategories.Data, list);

            return objHelper;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the name to categories of the Data Source.
        /// </summary>
        public override string GetCategoryName()
		{			
			var dataAdapter = StiDataAdapterService.GetDataAdapter(this);

            if (Dictionary != null)
		    {
                var data = Dictionary.DataStore[this.NameInSource];
                if (data != null && dataAdapter != null)
                    return dataAdapter.GetDataCategoryName(data);
            }

		    if (NameInSource.EndsWithInvariant($".{Name}") && (NameInSource.Length > Name.Length + 1))
		        return NameInSource.Substring(0, NameInSource.Length - (Name.Length + 1));

		    var index = NameInSource.LastIndexOf(".", StringComparison.InvariantCulture);
		    if (index != -1)
		        return NameInSource.Substring(0, index);

		    if (!string.IsNullOrEmpty(NameInSource))
		        return NameInSource;

			return base.GetCategoryName();
		}
        #endregion

        #region Properties
        /// <summary>
        /// Please instead property DataName use property NameInSource.
        /// </summary>
        [StiNonSerialized]
		[Browsable(false)]
		[Obsolete("Please instead property DataName use property NameInSource.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DataName
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
		/// Gets or sets the name of Data Source in source of data.
		/// </summary>
		[StiSerializable]
		[StiCategory("Data")]
		[Description("Gets or sets the name of Data Source in source of data.")]
        [StiOrder((int)Order.NameInSource)]
		public virtual string NameInSource { get; set; }
	    #endregion

        /// <summary>
        /// Creates a new object of the type StiDataStoreSource.
        /// </summary>
        public StiDataStoreSource() : this("", "", "")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiDataTableSource.
		/// </summary>
        /// <param name="nameInSource">Name of data in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		public StiDataStoreSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiDataStoreSource.
		/// </summary>
        /// <param name="nameInSource">Name of data in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiDataStoreSource(string nameInSource, string name, string alias) : base(name, alias)
		{
			this.NameInSource = nameInSource;
		}

        /// <summary>
        /// Creates a new object of the type StiDataStoreSource.
        /// </summary>
        /// <param name="nameInSource">Name of data in the DataStore.</param>
        /// <param name="name">Data Source name.</param>
        /// <param name="alias">Data Source alias.</param>
        public StiDataStoreSource(string nameInSource, string name, string alias, string key) : base(name, alias, key)
        {
            this.NameInSource = nameInSource;
        }
	}
}