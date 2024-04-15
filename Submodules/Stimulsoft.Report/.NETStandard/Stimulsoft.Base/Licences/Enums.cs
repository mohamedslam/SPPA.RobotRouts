#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;

namespace Stimulsoft.Base.Licenses
{
    #region StiProductIdent
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiProductIdent : long
    {
        Ultimate = 1,
        
        Net = 2,
        Wpf = 3,
		Web = 4,
        Silverlight = 5,
        
        Js = 6,
        Java = 7,
        Php = 8,
		NetCore = 9,
		Uwp = 10,
        Flex = 11,

        BIDesigner = 12,

        DbsJs = 13,
        DbsWin = 14,
        DbsWeb = 15,

        BIDesktop = 16,
        BIServer = 17,
        BICloud = 18,

        CloudReports = 20,
        CloudDashboards = 21,

        Angular = 22,
        DbsAngular = 23,

        DbsPhp = 24,

        FormsWin = 25,
        FormsWeb = 26,
        FormsJs = 27,
        FormsPhp = 28
    }
    #endregion

    #region StiActivationType
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiActivationType
    {
        Server = 1,
        Developer
    }
    #endregion

    #region StiLicenseResultFormat
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StiLicenseResultFormat
    {
        Object = 1,
        Base64
    }
    #endregion

    #region StiLicenseKeyPropertyOrder
    public enum StiLicenseKeyPropertyOrder
    {
        ActivationDate = 1,
        Signature = 2,
        Owner = 3,
        UserName = 4,

        Products = 5,

        StartDate = 6,
        EndDate = 7,
        DeviceId = 8,
        PlanId = 9,
                
        ProductName = 10,
        ProductLogo = 11,
        ProductFavIcon = 12,
        ProductDescription = 13,
        ProductUrl = 14
    }
    #endregion
}
