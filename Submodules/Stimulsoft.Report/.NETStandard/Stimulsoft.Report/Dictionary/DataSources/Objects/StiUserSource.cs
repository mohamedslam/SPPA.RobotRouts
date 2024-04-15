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
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Events;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the Data Source realizing access to UserSource.
	/// </summary>
	public class StiUserSource : StiDataStoreSource
	{
		#region DataAdapter
        protected override Type GetDataAdapterType()
        {
            return typeof(StiUserAdapterService);
        }
		#endregion

        #region Methods.override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiUserSource;
            }
        }

        public override StiDataSource CreateNew()
        {
            return new StiUserSource();
        }
        #endregion

		#region this
		/// <summary>
		/// Creates a new object of the type StiUserSource.
		/// </summary>
		public StiUserSource() : this("", "", "")
		{
		}


		/// <summary>
		/// Creates a new object of the type StiUserSource.
		/// </summary>
		/// <param name="nameInSource">Name of UserSource in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		public StiUserSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}


		/// <summary>
		/// Creates a new object of the type StiUserSource.
		/// </summary>
		/// <param name="nameInSource">Name of UserSource in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiUserSource(string nameInSource, string name, string alias) : base(nameInSource, name, alias)
		{
		}


        /// <summary>
        /// Creates a new object of the type StiUserSource.
        /// </summary>
        /// <param name="nameInSource">Name of UserSource in the DataStore.</param>
        /// <param name="name">Data Source name.</param>
        /// <param name="alias">Data Source alias.</param>
        public StiUserSource(string nameInSource, string name, string alias, string key) : base(nameInSource, name, alias, key)
        {
        }
		#endregion
	}
}