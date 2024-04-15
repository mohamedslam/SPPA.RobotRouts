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

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the Data Source realizing access to DataView.
    /// </summary>
    public class StiDataViewSource : StiDataStoreSource
	{
		#region Methods
        protected override Type GetDataAdapterType()
        {
            return typeof(StiDataViewAdapterService);
        }

	    public override StiDataSource CreateNew()
	    {
	        return new StiDataViewSource();
	    }
        #endregion

        #region Properties
        public override StiComponentId ComponentId => StiComponentId.StiDataViewSource;
	    #endregion

		/// <summary>
		/// Creates a new object of the type StiDataViewSource.
		/// </summary>
		public StiDataViewSource() : this("", "", "")
		{
		}

		/// <summary>
		/// Creates a new object of the type StiDataViewSource.
		/// </summary>
		/// <param name="nameInSource">Name of DataView in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		public StiDataViewSource(string nameInSource, string name) : this(nameInSource, name, name)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiDataViewSource.
		/// </summary>
		/// <param name="nameInSource">Name of DataView in the DataStore.</param>
		/// <param name="name">Data Source name.</param>
		/// <param name="alias">Data Source alias.</param>
		public StiDataViewSource(string nameInSource, string name, string alias) : base(nameInSource, name, alias)
		{
		}

        /// <summary>
        /// Creates a new object of the type StiDataViewSource.
        /// </summary>
        /// <param name="nameInSource">Name of DataView in the DataStore.</param>
        /// <param name="name">Data Source name.</param>
        /// <param name="alias">Data Source alias.</param>
        /// <param name="key">Key string.</param>
        public StiDataViewSource(string nameInSource, string name, string alias, string key)
            : base(nameInSource, name, alias, key)
        {
        }
	}
}