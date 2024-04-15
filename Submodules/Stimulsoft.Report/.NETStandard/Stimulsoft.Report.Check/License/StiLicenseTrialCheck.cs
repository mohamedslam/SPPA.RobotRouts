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

using Stimulsoft.Base;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Plans;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiLicenseTrialCheck : StiCheck
    {
        #region Properties
        public override StiCheckObjectType ObjectType => StiCheckObjectType.Report;

        public override string ElementName => null;

        public override string ShortMessage => StiLocalizationExt.Get("Publish", "TrialVersion");

        public override string LongMessage
        {
            get
            {
                if (StiLicenseKeyValidator.IsTrial(StiLicense.LicenseKey))
                    return StiLocalizationExt.Get("CheckLicense", "StiLicenseTrialCheckLong");

                else if (!StiLicenseKeyValidator.IsValidOnAnyPlatform(StiLicense.LicenseKey))
                    return StiLocalizationExt.Get("CheckLicense", "StiValidSubscriptioRequiredCheckLong");

                else
                    return "";
                
            }
        }

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
#if CLOUD
            var isTrial = StiCloudPlan.IsTrial(report != null ? report.ReportGuid : null);
#elif SERVER
            var isTrial = StiVersionX.IsSvr;
#else
            var isTrial = StiLicenseKeyValidator.IsTrial(StiLicense.LicenseKey) || !StiLicenseKeyValidator.IsValidOnAnyPlatform(StiLicense.LicenseKey);
#endif
            if (isTrial)
            {
                var check = new StiLicenseTrialCheck();

                check.Actions.Add(new StiBuyProductAction());

                return check;
            }
            else
                return null;            
        }
        #endregion
    }
}
