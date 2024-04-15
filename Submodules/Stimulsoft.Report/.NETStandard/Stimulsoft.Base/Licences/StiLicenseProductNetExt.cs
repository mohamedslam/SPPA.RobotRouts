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
    internal static class StiLicenseProductNetExt
    {
        internal static bool IsNetFrameworkAvailable(this StiLicenseProduct product)
        {
            return
                product.Ident == StiProductIdent.BIDesigner ||
                product.Ident == StiProductIdent.BIDesktop ||
                product.Ident == StiProductIdent.BIServer ||
                product.Ident == StiProductIdent.BICloud ||
                product.Ident == StiProductIdent.Ultimate ||
                product.Ident == StiProductIdent.Net ||
                product.Ident == StiProductIdent.Silverlight ||
                product.Ident == StiProductIdent.Web ||
                product.Ident == StiProductIdent.Wpf ||
                product.Ident == StiProductIdent.DbsWin ||
                product.Ident == StiProductIdent.DbsWeb ||
                product.Ident == StiProductIdent.CloudDashboards ||
                product.Ident == StiProductIdent.CloudReports ||
                product.Ident == StiProductIdent.Angular ||
                product.Ident == StiProductIdent.DbsAngular;
        }
    }
}