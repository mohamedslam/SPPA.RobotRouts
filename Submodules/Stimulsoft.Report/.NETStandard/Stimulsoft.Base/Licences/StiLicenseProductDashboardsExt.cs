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

using System;

namespace Stimulsoft.Base.Licenses
{
    internal static class StiLicenseProductDashboardsExt
    {
        internal static bool IsDashboardsAvailable(this StiLicenseProduct product)
        {
            return 
                product.Ident == StiProductIdent.BIDesigner ||
                product.Ident == StiProductIdent.BIDesktop ||
                product.Ident == StiProductIdent.BIServer ||
                product.Ident == StiProductIdent.BICloud ||                
                product.Ident == StiProductIdent.Ultimate ||
                product.Ident == StiProductIdent.DbsJs ||
                product.Ident == StiProductIdent.DbsWeb ||
                product.Ident == StiProductIdent.DbsWin ||
                product.Ident == StiProductIdent.DbsPhp ||
                product.Ident == StiProductIdent.DbsAngular ||
                product.Ident == StiProductIdent.CloudDashboards;
        }
    }
}