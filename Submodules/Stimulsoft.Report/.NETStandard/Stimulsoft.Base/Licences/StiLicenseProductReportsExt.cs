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
    internal static class StiLicenseProductReportsExt
    {
        internal static bool IsReportsAvailable(this StiLicenseProduct product)
        {
            return 
                product.Ident == StiProductIdent.BIDesigner ||
                product.Ident == StiProductIdent.BIDesktop ||
                product.Ident == StiProductIdent.BIServer ||
                product.Ident == StiProductIdent.BICloud ||
                product.Ident == StiProductIdent.Ultimate ||
                product.Ident == StiProductIdent.Flex ||
                product.Ident == StiProductIdent.Java ||
                product.Ident == StiProductIdent.Js ||
                product.Ident == StiProductIdent.Net ||
                product.Ident == StiProductIdent.NetCore ||
                product.Ident == StiProductIdent.Php ||
                product.Ident == StiProductIdent.Silverlight ||
                product.Ident == StiProductIdent.Uwp ||
                product.Ident == StiProductIdent.Web ||
                product.Ident == StiProductIdent.Wpf ||
                product.Ident == StiProductIdent.Angular ||
                product.Ident == StiProductIdent.CloudReports;
        }
    }
}