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
    #region StiDatabaseType
    /// <summary>
    /// Enum contain types of the data connections.
    /// </summary>
    public enum StiConnectionType
    {
        /// <summary>
        /// SQL based connections.
        /// </summary>
        Sql,
        /// <summary>
        /// NoSQL base connections.
        /// </summary>
        NoSql,
        /// <summary>
        /// Azure base connections.
        /// </summary>
        Azure,
        /// <summary>
        /// Google base connections.
        /// </summary>
        Google,
        /// <summary>
        /// Other data connections.
        /// </summary>
        OnlineServices,
        /// <summary>
        /// Other data connections.
        /// </summary>
        Other,
        /// <summary>
        /// REST based connections.
        /// </summary>
        Rest
    }
    #endregion
}
