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

namespace Stimulsoft.Base
{
    /// <summary>
    /// Describes the interface to access base data dictionary functionality.
    /// </summary>
	public interface IStiAppDictionary
    {
        /// <summary>
        /// Returns an enumeration of the data source from this dictionary.
        /// </summary>
        /// <returns>The enumeration of the data source.</returns>
        IEnumerable<IStiAppDataSource> FetchDataSources();

        /// <summary>
        /// Returns an enumeration of the data relations from this dictionary.
        /// </summary>
        /// <returns>The enumeration of the data relations.</returns>
        IEnumerable<IStiAppDataRelation> FetchDataRelations();

        /// <summary>
        /// Returns an enumeration of the variables from this dictionary.
        /// </summary>
        /// <returns>The enumeration of the variables.</returns>
        IEnumerable<IStiAppVariable> FetchVariables();

        /// <summary>
        /// Returns datasource from the data dictionary by its name.
        /// </summary>
        /// <param name="name">A name of the datasource.</param>
        /// <returns>The datasource from the data dictionary. Returns null, if datasource with specified name is not exists.</returns>
        IStiAppDataSource GetDataSourceByName(string name);

        /// <summary>
        /// Returns data column from the data dictionary by its name.
        /// </summary>
        /// <param name="name">A name of the data column.</param>
        /// <returns>The data column from the data dictionary. Returns null, if data column with specified name is not exists.</returns>
        IStiAppDataColumn GetColumnByName(string name);

        /// <summary>
        /// Returns variable from the data dictionary by its name.
        /// </summary>
        /// <param name="name">A name of the variable.</param>
        /// <returns>The variable from the data dictionary. Returns null, if variable with specified name is not exists.</returns>
        IStiAppVariable GetVariableByName(string name);

        /// <summary>
        /// Returns a value from the variable by its name.
        /// </summary>
        /// <param name="name">A name of the variable.</param>
        /// <returns>A value which contains in the variable.</returns>
        object GetVariableValueByName(string name);

        /// <summary>
        /// Returns true if a specified name is a name of a system variable.
        /// </summary>
        /// <param name="name">The name of the system variable.</param>
        /// <returns>True, if the specified name is the name of system variable.</returns>
        bool IsSystemVariable(string name);

        /// <summary>
        /// Returns true if a specified variable is a read-only variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>True, if the specified variable is a read-only variable.</returns>
        bool IsReadOnlyVariable(string name);

        /// <summary>
        /// Returns value of a specified system variable.
        /// </summary>
        /// <param name="name">A name of the system variable.</param>
        /// <returns>The value of the specified system variable.</returns>
        object GetSystemVariableValue(string name);

        /// <summary>
        /// Returns reference to the app which contains this dictionary.
        /// </summary>
        /// <returns>A reference to the app.</returns>
        IStiApp GetApp();

        /// <summary>
        /// Opens specified connections to the data. Opens all connections if none of them is specified.
        /// </summary>
        IEnumerable<IStiAppConnection> OpenConnections(IEnumerable<IStiAppConnection> connections);

        /// <summary>
        /// Closes all specified connections. Closes all connections if none of them is specified.
        /// </summary>
        void CloseConnections(IEnumerable<IStiAppConnection> connections);
    }
}
