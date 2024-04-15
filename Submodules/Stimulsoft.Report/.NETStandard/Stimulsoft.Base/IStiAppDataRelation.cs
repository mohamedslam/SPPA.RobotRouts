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
    /// Provides access to the datarelation.
    /// </summary>
    public interface IStiAppDataRelation : IStiAppCell
    {
        /// <summary>
        /// Returns a name of the component.
        /// </summary>
        /// <returns>The name of component.</returns>
        string GetName();

        /// <summary>
        /// Returns a reference to the dictionary which contains this datasource.
        /// </summary>
        /// <returns>A reference to the app.</returns>
        IStiAppDictionary GetDictionary();

        /// <summary>
        /// Returns parent data source of this relation.
        /// </summary>
        /// <returns>A reference to the data source.</returns>
        IStiAppDataSource GetParentDataSource();

        /// <summary>
        /// Returns child data source of this relation.
        /// </summary>
        /// <returns>A reference to the data source.</returns>
        IStiAppDataSource GetChildDataSource();

        /// <summary>
        /// Returns an enumeration of the parent column keys of the data relation.
        /// </summary>
        /// <returns>An reference to the enumeration.</returns>
        IEnumerable<string> FetchParentColumns();

        /// <summary>
        /// Returns an enumeration of the child column keys of the data relation.
        /// </summary>
        /// <returns>An reference to the enumeration.</returns>
        IEnumerable<string> FetchChildColumns();

        /// <summary>
        /// Returns the status of the relation.
        /// </summary>
        /// <returns>The status of the relation.</returns>
        bool GetActiveState();
    }
}