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

using Stimulsoft.Base.Design;
using System.Linq;

namespace Stimulsoft.Base.Licenses
{
    /// <summary>
    /// This class is used for the license checking.
    /// </summary>
    public static class StiLicenseKeyValidator
    {
        #region Methods
		internal static bool IsValidOnWebFramework(StiLicenseKey key)
        {
            return IsValidOnAnyPlatform(key) && key.Products.Any(p => p.IsWebFrameworkAvailable());
        }

        internal static bool IsValidOnNetFramework(StiLicenseKey key)
        {
            return IsValidOnAnyPlatform(key) && key.Products.Any(p => p.IsNetFrameworkAvailable());
        }

        internal static bool IsValidOnAnyPlatform(StiLicenseKey key)
        {
            return 
                key != null &&
                key.Signature != null &&
                key.Products != null &&
                key.Products.Any(p => p.ExpirationDate > StiVersion.Created);
        }

        internal static bool IsTrial(StiLicenseKey key)
        {
            return
                key == null ||
                key.Signature == null ||
                key.Products == null ||
                !key.Products.Any();
        }

        internal static bool IsValid(StiProductIdent ident, StiLicenseKey key)
        {
            return IsValidOnAnyPlatform(key) && key.Products.Any(p => p.Ident == ident || p.Ident == StiProductIdent.Ultimate);
        }

        internal static bool IsValidOnForms(StiLicenseKey key)
        {
            return
                key != null &&
                key.Signature != null &&
                key.Products != null &&
                key.Products.Any(p => p.ExpirationDate > StiVersion.Created && (p.Ident == StiProductIdent.Ultimate || p.Ident == StiProductIdent.FormsWeb));
        }

        internal static bool IsValidInReportsDesignerOrOnPlatform(StiProductIdent ident, StiLicenseKey key)
        {
            return StiDesignerAppStatus.IsRunning
                ? IsValidOnAnyPlatform(key) && key.Products.Any(p => p.IsReportsAvailable())
                : IsValid(ident, key);
        }

        internal static bool IsValidInDashboardsDesignerOrOnPlatform(StiProductIdent ident, StiLicenseKey key)
        {
            return StiDesignerAppStatus.IsRunning
                ? IsValidOnAnyPlatform(key) && key.Products.Any(p => p.IsDashboardsAvailable())
                : IsValid(ident, key);
        }

        internal static StiLicenseKey GetLicenseKey()
        {
            if (string.IsNullOrWhiteSpace(StiLicense.Key))
                return null;

            if (StiLicense.LicenseKey == null)
                return null;

            if (StiLicense.LicenseKey.Signature == null)
                return null;

            return StiLicense.LicenseKey;
        }
        #endregion
    }
}