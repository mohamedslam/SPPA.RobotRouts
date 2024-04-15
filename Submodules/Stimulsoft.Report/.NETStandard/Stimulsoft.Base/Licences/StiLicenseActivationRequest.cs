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

using System.ComponentModel;

namespace Stimulsoft.Base.Licenses
{
    public class StiLicenseActivationRequest : StiLicenseObject
    {
        #region Properties
        protected override string EncryptKey => "aoc#wm5eoAtrr$a5@m9w";

        public string MachineName { get; set; }

        public string MachineUserName { get; set; }

        public string MachineUserDomainName { get; set; }

        public string MachineOSVersion { get; set; }

        public string MachineAddress { get; set; }
        
        public string MachineGuid { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        [DefaultValue(StiActivationType.Server)]
        public StiActivationType Type { get; set; }

        public string DeviceId { get; set; }

        public string SessionKey { get; set; }

        public string Version { get; set; }
        #endregion

        #region Methods.Static
        public static StiLicenseActivationRequest Get(byte[] bytes)
        {
            var request = new StiLicenseActivationRequest();
            request.DecryptFromBytes(bytes);
            return request;
        }

        public static StiLicenseActivationRequest Get(string str)
        {
            var request = new StiLicenseActivationRequest();
            request.DecryptFromString(str);
            return request;
        }
        #endregion

        public StiLicenseActivationRequest()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.DeviceId = string.Empty;
            this.Type = StiActivationType.Server;
        }
    }
}