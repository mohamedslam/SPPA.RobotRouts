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
using System.Reflection;
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
	/// Describes the Data Source realizing access to IEnumerable. This type of data source is obsolete.
	/// </summary>
	public class StiEnumerableSource : StiBusinessObjectSource
	{
        #region Methods.override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiEnumerableSource;
            }
        }

        public override StiDataSource CreateNew()
        {
            return new StiEnumerableSource();
        }
        #endregion

		#region this
		/// <summary>
		/// Creates a new object of the type StiEnumerableSource.
		/// </summary>
		public StiEnumerableSource() : base()
		{
		}

		/// <summary>
		/// Creates a new object of the type StiEnumerableSource.
		/// </summary>
		/// <param name="nameInSource">Name of IEnumerable in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		public StiEnumerableSource(string nameInSource, string name) : base(nameInSource, name)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiEnumerableSource.
		/// </summary>
		/// <param name="nameInSource">Name of IEnumerable in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiEnumerableSource(string nameInSource, string name, string alias) : base(nameInSource, name, alias)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiEnumerableSource.
        /// </summary>
        /// <param name="nameInSource">Name of IEnumerable in the DataStore.</param>
        /// <param name="name">Data Source name.</param>
        /// <param name="alias">Data Source alias.</param>
        /// <param name="key">Key string.</param>
        public StiEnumerableSource(string nameInSource, string name, string alias, string key)
            : base(nameInSource, name, alias, key)
        {
        }
		#endregion
	}
}