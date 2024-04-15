#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.Collections.Generic;
using System.Data;

namespace Stimulsoft.Base
{
    /// <summary>
    /// Provides access to the datasource.
    /// </summary>
    public interface IStiAppDataSource : IStiAppCell
    {
        /// <summary>
        /// Returns a name of the component in the data source.
        /// </summary>
        /// <returns>The name of component.</returns>
        string GetNameInSource();

        /// <summary>
        /// Returns a name of the component.
        /// </summary>
        /// <returns>The name of component.</returns>
        string GetName();

        /// <summary>
        /// Returns a DataTable with data from this datasource.
        /// </summary>
        /// <param name="allowConnectToData">Allow to call Connect() method. By default is true.</param>
        /// <returns>The DataTable with data.</returns>
        DataTable GetDataTable(bool allowConnectToData);

        /// <summary>
        /// Returns a reference to the dictionary which contains this datasource.
        /// </summary>
        /// <returns>Reference to the app.</returns>
        IStiAppDictionary GetDictionary();

        /// <summary>
        /// Returns an enumeration of the data columns from this dictionary.
        /// </summary>
        /// <returns>The enumeration of the data columns.</returns>
        IEnumerable<IStiAppDataColumn> FetchColumns();

        /// <summary>
        /// Returns a connection to data for this data source.
        /// </summary>
        /// <returns>Reference to the connection.</returns>
        IStiAppConnection GetConnection();

        /// <summary>
        /// Returns an enumeration of the parent data relations for this data source.
        /// </summary>
        /// <returns>The enumeration of the data relations.</returns>
        IEnumerable<IStiAppDataRelation> FetchParentRelations(bool activePreferred);

        /// <summary>
        /// Returns an enumeration of the child data relations for this data source.
        /// </summary>
        /// <returns>The enumeration of the data relations.</returns>
        IEnumerable<IStiAppDataRelation> FetchChildRelations(bool activePreferred);

        /// <summary>
        /// Returns an array of values for the specified column in the specified position.
        /// </summary>
        /// <param name="names">An array of names of the data column.</param>
        /// <returns>The enumeration of the data column values.</returns>
        IEnumerable<object[]> FetchColumnValues(string[] names);
    }
}