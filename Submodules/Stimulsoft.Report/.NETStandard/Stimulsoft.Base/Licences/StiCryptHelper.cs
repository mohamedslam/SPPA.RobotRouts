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

using Stimulsoft.Base.Helpers;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Stimulsoft.Base.Licenses
{
    public static class StiCryptHelper
    {
        #region Fields
        private const string encryptionKey = "fheur89fhsodifjs04ejfdszifhv8urg";
        #endregion

        #region Methods
        public static byte[] Encrypt(byte[] bytes, string password = null)
        {
            if (string.IsNullOrWhiteSpace(password))
                password = encryptionKey;

            using (var algorithm = CreateAlgorithm())
            using (var encryptor = algorithm.CreateEncryptor(new PasswordDeriveBytes(password, null).GetBytes(16), new byte[16]))
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                return memoryStream.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] bytes, string password = null)
        {
            if (string.IsNullOrWhiteSpace(password))
                password = encryptionKey;

#if BLAZOR
            return StiBytesToStringConverter.ConvertStringToBytes(Stimulsoft.System.Crypt.AES.Decrypt(Convert.ToBase64String(bytes), password));
#else
            using (var algorithm = CreateAlgorithm())
            using (var decryptor = algorithm.CreateDecryptor(new PasswordDeriveBytes(password, null).GetBytes(16), new byte[16]))
            using (var memoryStream = new MemoryStream(bytes))
            using (var decrypt = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var binaryReader = new BinaryReader(decrypt))
            {
                return binaryReader.ReadAllBytes();
            }
#endif
        }

        public static byte[] Recrypt(byte[] bytes, string oldPassword, string newPassword)
        {
            bytes = Decrypt(bytes, oldPassword);
            return Encrypt(bytes, newPassword);
        }

        public static string Encrypt(string str, string password = null)
        {
            var bytes = StiBytesToStringConverter.ConvertStringToBytes(str);
            var encryptedBytes = Encrypt(bytes, password);
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string str, string password = null)
        {
            if (string.IsNullOrWhiteSpace(password))
                password = encryptionKey;

#if BLAZOR
            return Stimulsoft.System.Crypt.AES.Decrypt(str, password);
#else

            var bytes = Convert.FromBase64String(str);

            using (var algorithm = CreateAlgorithm())
            using (var decryptor = algorithm.CreateDecryptor(new PasswordDeriveBytes(password, null).GetBytes(16), new byte[16]))
            using (var memoryStream = new MemoryStream(bytes))
            using (var decrypt = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var streamReader = new StreamReader(decrypt))
            {
                return streamReader.ReadToEnd();
            }
#endif
        }

        public static string Recrypt(string str, string oldPassword, string newPassword)
        {
            str = Decrypt(str, oldPassword);
            return Encrypt(str, newPassword);
        }

        internal static byte[] ReadAllBytes(this BinaryReader reader)
        {
            var bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[bufferSize];
                int count;

                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);

                return ms.ToArray();
            }

        }

        internal static string MD5(string value)
        {
            return StiMD5Helper.ComputeHash(value);
        }

        private static SymmetricAlgorithm CreateAlgorithm()
        {
            try
            {
                return Rijndael.Create();
            }
            catch (TargetInvocationException)
            {
                return new AesCryptoServiceProvider();
            }
        }
        #endregion
    }
}
