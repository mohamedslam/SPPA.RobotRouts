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
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the base adapter of data for access to data in the store.
    /// </summary>
    public abstract class StiDataStoreAdapterService : StiDataAdapterService
	{
		#region StiDataAdapterService override
		/// <summary>
		/// Fills a name and alias of the Data Source relying on data.
		/// </summary>
		/// <param name="data">Data relying on which names will be filled.</param>
		/// <param name="dataSource">Data Source in which names will be filled.</param>
		public override void SetDataSourceNames(StiData data, StiDataSource dataSource)
		{
			((StiDataStoreSource)dataSource).NameInSource = data.Name;
		}

		/// <summary>
		/// Creates a new Data Source and adds it to the dictionary.
		/// </summary>
		/// <param name="dictionary">Dictionary to add Data Source.</param>
		/// <returns>Created Data Source.</returns>
		public override StiDataSource Create(StiDictionary dictionary, bool addToDictionary)
		{
			var dataSource = StiActivator.CreateObject(GetDataSourceType()) as StiDataSource;
			if (dataSource != null && addToDictionary)
			{
				dataSource.Name = dataSource.Alias = StiNameCreation.CreateName(dictionary.Report,
					StiLocalization.Get("PropertyMain", "DataSource"));
				dictionary.DataSources.Add(dataSource);
			}
			return dataSource;
		}

		/// <summary>
		/// Calls the form for Data Source edition.
		/// </summary>
		/// <param name="dictionary">Dictionary in which Data Source is located.</param>
		/// <param name="dataSource">Data Source.</param>
		/// <returns>Result of gialog form.</returns>
		public override bool Edit(StiDictionary dictionary, StiDataSource dataSource)
		{
            return StiDataEditorsHelper.Get().DataStoreAdapterEdit(this, dictionary, dataSource);
		}

		/// <summary>
		/// Calls the form for a new Data Source edition.
		/// </summary>
		/// <param name="dictionary">Dictionary in which Data Source is located.</param>
		/// <param name="dataSource">Data Source.</param>
		/// <returns>Result of gialog form.</returns>
		public override bool New(StiDictionary dictionary, StiDataSource dataSource)
		{
            return StiDataEditorsHelper.Get().DataStoreAdapterNew(this, dictionary, dataSource);
		}
		#endregion
	}
}
