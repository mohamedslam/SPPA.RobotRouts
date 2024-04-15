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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using System;

namespace Stimulsoft.Base.Plans
{
    #region StiPlanIdent
    /// <summary>
    /// An enumeration contains the types of the user plans.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiPlanIdent
    {
        #region Cloud        
        CloudTrial = 100,
        CloudDeveloper = 101,
        CloudSingle = 102,
        CloudTeam = 103,
        CloudEnterprise = 104,
        #endregion

        #region Server
        ServerTrial = 200,
        ServerTeam5 = 201,
        ServerTeam10 = 202,
        ServerTeam25 = 203,
        ServerTeam50 = 204,
        ServerBusiness = 205,
        ServerEnterprise = 206,
        ServerWorldWide = 207,
        #endregion

        #region BIServer
        BIServerSingle = 300,
        BIServerTeam = 301,
        BIServerSite = 302,
        BIServerWorldWide = 303,
        #endregion        
    }
    #endregion

    #region StiCloudPlanIdent
    [Flags]
    public enum StiCloudPlanIdent
	{
	    Trial = 0,
        RBase = 1,
        RSingle = 2,
        RTeam = 4,
        REnterprise = 8,
        DBase = 32,
        DSingle = 64,
        DTeam = 128,
        DEnterprise = 256,

        /// <summary>
        /// Back compatibility
        /// </summary>
        Developer = 1,

        Expiried = 512
    }
    #endregion
}
