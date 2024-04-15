#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;

namespace Stimulsoft.Base
{
    public class StiMD5Helper
    {
        public static byte[] ComputeHash(Stream stream)
        {
            var md5 = new Crypto.MD5();
            var buf = new byte[4096];
            var len = buf.Length;

            while (len == buf.Length)
            {
                len = stream.Read(buf, 0, buf.Length);
                if (len > 0)
                    md5.BlockUpdate(buf, 0, len);
            }

            return md5.GetHash();
        }

        public static byte[] ComputeHash(byte[] buf)
        {
            return ComputeHash(buf, 0, buf.Length);
        }

        public static byte[] ComputeHash(byte[] buf, int offset, int count)
        {
            byte[] hash;

            if (StiBaseOptions.FIPSCompliance)
            {
                var md5 = new Crypto.MD5();
                md5.BlockUpdate(buf, offset, count);
                hash = md5.GetHash();
            }
            else
            {
                using (var hashMD5 = new MD5CryptoServiceProvider())
                {
                    hash = hashMD5.ComputeHash(buf, offset, count);
                }
            }
            return hash;
        }

        public static string ComputeHash(string input)
        {
            var buffer = ComputeHash(Encoding.UTF8.GetBytes(input));
            var result = string.Empty;

            foreach (var value in buffer)
            {
                result += value.ToString("x2");
            }

            return result;
        }

        static StiMD5Helper()
        {
            try
            {
                StiBaseOptions.FIPSCompliance = true;
#if !BLAZOR
#if NETSTANDARD
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return;
#endif
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Lsa\\FIPSAlgorithmPolicy");
                if (key == null) return;

                var value = key.GetValue("Enabled");
                if (value != null && value is int && (int) value == 1)
                    StiBaseOptions.FIPSCompliance = true;
#endif

            }
            catch
            {
            }

        }
    }
}
