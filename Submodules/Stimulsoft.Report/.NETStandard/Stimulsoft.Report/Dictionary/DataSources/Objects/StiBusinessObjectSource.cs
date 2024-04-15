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
using System.Data;
using System.ComponentModel;
using System.Reflection;
using Stimulsoft.Base;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Design;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the Data Source realizing access to Business Objects.
	/// </summary>
	public class StiBusinessObjectSource : StiDataTableSource
    {
        #region Methods
        /// <summary>
		/// Returns the name to categories of the Data Source.
		/// </summary>
		public override string GetCategoryName()
		{
			StiDataAdapterService dataAdapter = StiDataAdapterService.GetDataAdapter(this);
			StiData data = Dictionary.DataStore[this.NameInSource] as StiData;
			if (data != null)return dataAdapter.GetDataCategoryName(data);
			return base.GetCategoryName();
        }

        protected override Type GetDataAdapterType()
        {
            return typeof(StiBusinessObjectAdapterService);
        }
        #endregion

        #region Methods.override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiBusinessObjectSource;
            }
        }

        public override StiDataSource CreateNew()
        {
            return new StiBusinessObjectSource();
        }
        #endregion

        #region this
        /// <summary>
		/// Creates a new object of the type StiBusinessObjectSource.
		/// </summary>
		public StiBusinessObjectSource() : this("", "", "")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiBusinessObjectSource.
		/// </summary>
		/// <param name="nameInSource">Name of business object.</param>
		/// <param name="name">Data Source name.</param>
		public StiBusinessObjectSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiBusinessObjectSource.
		/// </summary>
		/// <param name="nameInSource">Name of business object.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiBusinessObjectSource(string nameInSource, string name, string alias) : base(nameInSource, name, alias)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiBusinessObjectSource.
        /// </summary>
        /// <param name="nameInSource">Name of business object.</param>
        /// <param name="name">Data Source name.</param>
        /// <param name="alias">Data Source alias.</param>
        /// <param name="key">Key string.</param>
        public StiBusinessObjectSource(string nameInSource, string name, string alias, string key)
            : base(nameInSource, name, alias, key)
        {
        }
		#endregion
	}
}