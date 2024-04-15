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

namespace Stimulsoft.Base.Licenses
{
    public class StiLicenseActivationResponse : StiLicenseObject
    {
        #region Properties
        protected override string EncryptKey => "aoc#wm5eoAtrr$a5@m9w";

        public StiLicenseKey LicenseKey { get; set; }

        /// <summary>
        /// An information about exception which occurs during activation process.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// True or False value which depend on the status of the running command.
        /// </summary>
        public bool ResultSuccess { get; set; }

        /// <summary>
        /// A message about the running command. This Result can be skipped.
        /// </summary>
        public StiNotice ResultNotice { get; set; }
        #endregion

        #region Methods.Static
        public static StiLicenseActivationResponse MakeWrong(StiNotice notice)
        {
            return new StiLicenseActivationResponse
            {
                ResultSuccess = false,
                ResultNotice = notice 
            };
        }

        public static StiLicenseActivationResponse MakeFine(StiLicenseKey licenseKey)
        {
            return new StiLicenseActivationResponse
            {
                ResultSuccess = true,
                LicenseKey = licenseKey
            };
        }
        
        public static StiLicenseActivationResponse Get(byte[] bytes)
        {
            var response = new StiLicenseActivationResponse();
            response.DecryptFromBytes(bytes);
            return response;
        }

        public static StiLicenseActivationResponse Get(string str)
        {
            var response = new StiLicenseActivationResponse();
            response.DecryptFromString(str);
            return response;
        }
        #endregion
    }
}