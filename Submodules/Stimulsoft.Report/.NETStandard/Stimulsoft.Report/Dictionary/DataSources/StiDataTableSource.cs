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
using System;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the Data Source realizing access to DataTable.
    /// </summary>
    public class StiDataTableSource : StiDataStoreSource
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDataStoreSource
            jObject.AddPropertyStringNullOrEmpty("NameInSource", NameInSource);

            return jObject;
        }
        #endregion

		#region Methods
		public override string GetCategoryName()
		{
			var nameInSourceStr = this.NameInSource;
			if (string.IsNullOrEmpty(nameInSourceStr))
			    return base.GetCategoryName();

			nameInSourceStr = nameInSourceStr.ToLowerInvariant();

            if (Dictionary != null)
            {
                var database = Dictionary.Databases.ToList().FirstOrDefault(d => d.Name.ToLowerInvariant() == nameInSourceStr);
                if (database != null)
                    return database.Name;
            }

			var dataAdapter = StiDataAdapterService.GetDataAdapter(this) as StiDataTableAdapterService;
			if (dataAdapter != null)
			{
				var data = dataAdapter.GetDataFromDataSource(Dictionary, this);
				if (data == null)
				    return base.GetCategoryName();

				return dataAdapter.GetDataCategoryName(data);
			}

			return base.GetCategoryName();
		}

        protected override Type GetDataAdapterType()
        {
            return typeof(StiDataTableAdapterService);
        }

	    public override StiDataSource CreateNew()
	    {
	        return new StiDataTableSource();
	    }
        #endregion

        #region Properties
        public override StiComponentId ComponentId => StiComponentId.StiDataTableSource;
	    #endregion

		/// <summary>
		/// Creates a new object of the type StiDataTableSource.
		/// </summary>
		public StiDataTableSource() : this("", "", "")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiDataTableSource.
		/// </summary>
		/// <param name="nameInSource">Name of DataTable in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		public StiDataTableSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiDataTableSource.
		/// </summary>
		/// <param name="nameInSource">Name of DataTable in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiDataTableSource(string nameInSource, string name, string alias) : base(nameInSource, name, alias)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiDataTableSource.
        /// </summary>
        /// <param name="nameInSource">Name of DataTable in the DataStore.</param>
        /// <param name="name">Data Source name.</param>
        /// <param name="alias">Data Source alias.</param>
        /// <param name="alias">Key string.</param>
        public StiDataTableSource(string nameInSource, string name, string alias, string key) : base(nameInSource, name, alias, key)
        {
        }
	}
}

