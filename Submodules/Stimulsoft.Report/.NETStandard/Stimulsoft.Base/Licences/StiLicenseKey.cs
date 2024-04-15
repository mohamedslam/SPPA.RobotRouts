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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Plans;

namespace Stimulsoft.Base.Licenses
{
    public class StiLicenseKey : StiLicenseObject, ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            var newKey = new StiLicenseKey();
            newKey.LoadFromString(this.SaveToString());
            return newKey;
        }

        public byte[] GetCheckBytes()
        {
            var key = this.Clone() as StiLicenseKey;
            key.Signature = null;
            return key.SaveToBytes();
        }

        public byte[] GetSignatureBytes()
        {
            return Convert.FromBase64String(this.Signature);
        }
        #endregion

        #region Properties
        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.ActivationDate)]
        public DateTime ActivationDate { get; set; }

        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.Signature)]
        public string Signature { get; set; }

        [DefaultValue("")]
        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.Owner)]
        public string Owner { get; set; }

        [DefaultValue("")]
        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.UserName)]
        public string UserName { get; set; }
        #endregion

        #region Properties.Server
        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.StartDate)]
        public DateTime? StartDate { get; set; }

        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.EndDate)]
        public DateTime? EndDate { get; set; }

        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.DeviceId)]
        public string DeviceId { get; set; }

        [DefaultValue(null)]
        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.PlanId)]
        public StiPlanIdent? PlanId { get; set; }
        #endregion

        #region Properties.Developer
        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.Products)]
        public List<StiLicenseProduct> Products { get; set; }
        #endregion

        #region Properties.WhiteLabel
        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.ProductName)]
        public string ProductName { get; set; }

        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.ProductLogo)]
        public byte[] ProductLogo { get; set; }

        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.ProductFavIcon)]
        public byte[] ProductFavIcon { get; set; }

        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.ProductDescription)]
        public string ProductDescription { get; set; }

        [JsonProperty(Order = (int)StiLicenseKeyPropertyOrder.ProductUrl)]
        public string ProductUrl { get; set; }
        #endregion

        #region Properties.Status
        [JsonIgnore]
        public bool IsServerLicense => PlanId != null;

        [JsonIgnore]
        public bool IsProductLicense => Products != null && Products.Count > 0;
        #endregion

        #region Methods.SaveLoad
        public static StiLicenseKey Get(byte[] bytes)
        {
            var key = new StiLicenseKey();
            key.DecryptFromBytes(bytes);
            return key;
        }

        public static StiLicenseKey Get(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            var key = new StiLicenseKey();
            key.DecryptFromString(str);
            return key;
        }

        public string GetCSharpCode(int maxSymbols = 100, bool onlyKey = false)
        {
            var sb = GetCode(maxSymbols, onlyKey);
            sb = sb.Append(";");
            return sb.ToString();

        }

        public string GetVbNetCode(int maxSymbols = 100, bool onlyKey = false)
        {
            return GetCode(maxSymbols, onlyKey).ToString();
        }

        private StringBuilder GetCode(int maxSymbols = 100, bool onlyKey = false)
        {
            var sb = new StringBuilder();

            if (!onlyKey)
                sb = sb.Append("StiLicense.Key = ");

            var initLength = sb.Length;
            var firstLine = !onlyKey;
            var str = EncryptToString();

            while (true)
            {
                var max = firstLine ? maxSymbols - initLength : maxSymbols;

                if (str.Length > max)
                {
                    sb = sb.AppendFormat("\"{0}\"+\r\n", str.Substring(0, max));
                    str = str.Substring(max);
                }
                else
                {
                    sb = sb.AppendFormat("\"{0}\"", str);
                    break;
                }
                firstLine = false;
            }

            return sb;
        }
        #endregion
        
        public StiLicenseKey()
        {
            this.ActivationDate = DateTime.Today;
            this.Owner = string.Empty;
            this.UserName = string.Empty;
        }
    }
}