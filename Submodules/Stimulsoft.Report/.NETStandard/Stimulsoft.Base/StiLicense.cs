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
using System.IO;
using System.Linq;
using System.Reflection;
using Stimulsoft.Base.Licenses;

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

namespace Stimulsoft.Base
{
    /// <summary>
    /// This class is used for setup licensing of the reporting tool.
    /// </summary>
    public static class StiLicense
    {
        #region Events
        internal static event EventHandler LicenseChanged;
        #endregion

        #region Properties
        private static StiLicenseKey licenseKey;
        internal static StiLicenseKey LicenseKey
        {
            get
            {
                return licenseKey;
            }
            set
            {
                if (value != licenseKey)
                {
                    licenseKey = value;

                    LicenseChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        private static string key;
        /// <summary>
        /// A string representation of the license key.
        /// </summary>
        public static string Key
        {
            get
            {
                return key;
            }
            set
            {
                SetNewLicenseKey(value);
            }
        }
        #endregion

        #region Methods
        internal static void SetNewLicenseKey(string value, bool throwException = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                key = null;
                LicenseKey = null;
            }
            else
            {
                StiLicenseKey newLicenseKey = null;
                try
                {
                    newLicenseKey = StiLicenseKey.Get(value);
                }
                catch
                {
                }

                if (IsValidLicenseKey(newLicenseKey))
                {
                    LicenseKey = newLicenseKey;
                    key = value;
                }
                else
                {
                    if (throwException)
                        throw new Exception("The license key is not valid!");

                    key = null;
                    LicenseKey = null;
                }
            }
        }

        internal static bool IsValidLicenseKey(StiLicenseKey licenseKey)
        {
            try
            {
                if (licenseKey == null || licenseKey.Signature == null || !typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key))
                    return false;

                using (var rsa = new RSACryptoServiceProvider(512))
                using (var sha = new SHA1CryptoServiceProvider())
                {
                    rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

                    return rsa.VerifyData(licenseKey.GetCheckBytes(), sha, licenseKey.GetSignatureBytes());
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Loads a license key from the specified file.
        /// </summary>
        public static void LoadFromFile(string file)
        {
            LoadFromString(File.ReadAllText(file));
        }

        /// <summary>
        /// Loads a license key from the specified stream.
        /// </summary>
        public static void LoadFromStream(Stream stream)
        {
            using (var br = new StreamReader(stream))
            {
                LoadFromString(br.ReadToEnd());
            }
        }

        /// <summary>
        /// Loads a license key from the specified string.
        /// </summary>
        public static void LoadFromString(string licenseKey)
        {
            Key = licenseKey;
        }

        public static void LoadFromEntryAssembly(string resourceName = "license.key")
        {
            LoadFromAssembly(Assembly.GetEntryAssembly(), resourceName);
        }

        public static void LoadFromAssembly(Assembly a, string resourceName = "license.key")
        {
            var stream = a.GetManifestResourceStream(null, resourceName);
            if (stream != null)
            {
                LoadFromStream(stream);
                stream.Dispose();
                return;
            }

            resourceName = a.GetManifestResourceNames()
                .FirstOrDefault(n => n.ToLowerInvariant()
                .EndsWithInvariantIgnoreCase(resourceName));
            if (string.IsNullOrWhiteSpace(resourceName)) return;

            stream = a.GetManifestResourceStream(null, resourceName);
            if (stream != null)
            {
                LoadFromStream(stream);
                stream.Dispose();
            }
        }
        #endregion
    }
}