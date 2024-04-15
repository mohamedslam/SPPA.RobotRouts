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
using System.Data;

using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the adapter of the data transformation.
    /// </summary>
    public class StiDataTransformationAdapterService : StiDataStoreAdapterService
	{
		#region StiService override
	    /// <summary>
	    /// Gets a service name.
	    /// </summary>
	    public override string ServiceName => string.Format(Loc.Get("Adapters", "AdapterConnection"), Loc.Get("PropertyMain", "DataTransformation"));
	    #endregion

        #region Properties
        public override bool IsObjectAdapter => true;
	    #endregion

		#region StiDataAdapterService override
		/// <summary>
		/// Calls the form for Data Source edition.
		/// </summary>
		/// <param name="dictionary">Dictionary in which Data Source is located.</param>
		/// <param name="dataSource">Data Source.</param>
		/// <returns>Result of gialog form.</returns>
		public override bool Edit(StiDictionary dictionary, StiDataSource dataSource)
		{
		    throw new NotImplementedException();
		}

		/// <summary>
		/// Calls the form for a new Data Source edition.
		/// </summary>
		/// <param name="dictionary">Dictionary in which Data Source is located.</param>
		/// <param name="dataSource">Data Source.</param>
		/// <returns>Result of gialog form.</returns>
		public override bool New(StiDictionary dictionary, StiDataSource dataSource)
		{
		    throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the array of data types to which the Data Source may refer.
		/// </summary>
		/// <returns>Array of data types.</returns>
		public override Type[] GetDataTypes()
		{
			return null;
		}

        /// <summary>
        /// Returns a collection of columns of data.
        /// </summary>
        /// <param name="data">Data to find column.</param>
        /// <returns>Collection of columns found.</returns>
        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource)
        {
            return GetColumnsFromData(data, dataSource, CommandBehavior.SchemaOnly);
        }

        /// <summary>
        /// Returns a collection of columns of data.
        /// </summary>
        /// <param name="data">Data to find column.</param>
        /// <returns>Collection of columns found.</returns>
        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource, CommandBehavior retrieveMode)
		{
			return new StiDataColumnsCollection();
		}

        /// <summary>
        /// Returns a collection of parameters of data.
        /// </summary>
        /// <param name="data">Data to find parameters.</param>
        /// <returns>Collection of parameters found.</returns>
        public override StiDataParametersCollection GetParametersFromData(StiData data, StiDataSource dataSource)
        {
            return new StiDataParametersCollection();
        }

		/// <summary>
		/// Returns name of category for data.
		/// </summary>
		public override string GetDataCategoryName(StiData data)
		{
			return ServiceName;
		}

		/// <summary>
		/// Returns the type of the Data Source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public override Type GetDataSourceType()
		{
			return typeof(StiDataTransformation);
		}

		public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
		{
            StiDataLeader.Disconnect(dataSource);
        }
		#endregion
	}
}
