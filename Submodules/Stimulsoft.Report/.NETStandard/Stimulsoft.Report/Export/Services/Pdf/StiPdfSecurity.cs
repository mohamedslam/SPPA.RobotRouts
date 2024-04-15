#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using System.Security;
using System.Collections.Generic;

namespace Stimulsoft.Report.Export.Tools
{
    [SuppressUnmanagedCodeSecurity]
    public class StiPdfSecurity
    {
        #region Security procedures

        private static byte[] paddingString = {0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75, 0x8A, 0x41, 0x64, 0x00, 0x4E, 0x56, 0xFF, 0xFA, 0x01, 0x08,
											   0x2E, 0x2E, 0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80, 0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53, 0x69, 0x7A};

        private byte[] ownerValue = new byte[48];
        private byte[] userValue = new byte[48];
        private byte[] ownerExtendedValue = new byte[32];
        private byte[] userExtendedValue = new byte[32];
        private byte[] permsValue = new byte[16];
        private byte[] encryptionKey = null;
        private byte[] IDValue = null;

        private string passwordOwner = string.Empty;
        private string passwordUser = string.Empty;
        private uint securityFlags = 0;
        private StiPdfEncryptionKeyLength keyLength = StiPdfEncryptionKeyLength.Bit40;

        StiPdfExportService pdfService = null;


        #region Pad Password
        private byte[] PadPassword(string InputPassword)
        {
            StringBuilder password = new StringBuilder(InputPassword);
            for (int index = 0; index < paddingString.Length; index++)
            {
                password.Append((char)paddingString[index]);
            }
            if (password.Length > 32) password.Remove(32, password.Length - 32);
            byte[] result = new byte[32];
            for (int index = 0; index < 32; index++)
            {
                result[index] = (byte)password[index];
            }
            return result;
        }
        #endregion

        #region ComputingCryptoValues
        public bool ComputingCryptoValues(StiUserAccessPrivileges userAccessPrivileges, string passwordInputOwner, string passwordInputUser, StiPdfEncryptionKeyLength keyLength, byte[] IDValue)
        {
            securityFlags = 0xFFFFFFC0;	//none for revision 2
            uint tempFlags = 0x00000000;
            if ((userAccessPrivileges & StiUserAccessPrivileges.PrintDocument) != 0)
                tempFlags |= 1 << 2;
            if ((userAccessPrivileges & StiUserAccessPrivileges.ModifyContents) != 0)
                tempFlags |= 1 << 3;
            if ((userAccessPrivileges & StiUserAccessPrivileges.CopyTextAndGraphics) != 0)
                tempFlags |= 1 << 4;
            if ((userAccessPrivileges & StiUserAccessPrivileges.AddOrModifyTextAnnotations) != 0)
                tempFlags |= 1 << 5;
            securityFlags |= tempFlags;

            if (passwordInputOwner == null)
                passwordInputOwner = string.Empty;

            if (passwordInputUser == null)
                passwordInputUser = string.Empty;

            passwordOwner = passwordInputOwner;
            passwordUser = passwordInputUser;

            this.keyLength = keyLength;
            this.IDValue = IDValue;

            bool encrypted = false;
            if ((passwordOwner.Length > 0) || (passwordUser.Length > 0) || (userAccessPrivileges != StiUserAccessPrivileges.All))
            {
                encrypted = true;
                bool error = ComputingCryptoValues2();
                if (!error)
                {
                    encrypted = false;	//encrypt not work
                    throw new Exception("Encryption not work!");
                }
            }
            return encrypted;
        }

        private bool ComputingCryptoValues2()
        {
            if (keyLength == StiPdfEncryptionKeyLength.Bit128_r4) return ComputingCryptoValuesV4();
            if (keyLength == StiPdfEncryptionKeyLength.Bit256_r5 || keyLength == StiPdfEncryptionKeyLength.Bit256_r6) return ComputingCryptoValuesV5();

            string password = passwordOwner;
            if (passwordOwner == string.Empty) password = passwordUser;
            byte[] pass1 = PadPassword(password);

            #region compute owner value
            //add data to hash
            byte[] hash = StiMD5Helper.ComputeHash(pass1);

            if (keyLength == StiPdfEncryptionKeyLength.Bit128)
            {
                //for release 3
                for (int index1 = 0; index1 < 50; index1++)
                {
                    hash = StiMD5Helper.ComputeHash(hash);
                }
            }

            if (keyLength == StiPdfEncryptionKeyLength.Bit40)
            {
                byte[] buf = new byte[5];
                Array.Copy(hash, buf, 5);
                hash = buf;
            }

            byte[] result1 = PadPassword(passwordUser);

            //encrypt the user password
            Tools.StiEncryption.RC4(ref result1, hash);

            if (keyLength == StiPdfEncryptionKeyLength.Bit128)
            {
                //for release 3
                for (int index2 = 1; index2 <= 19; index2++)
                {
                    for (int tempIndex = 0; tempIndex < 16; tempIndex++)
                    {
                        hash[tempIndex] ^= (byte)(index2 - 1);
                        hash[tempIndex] ^= (byte)index2;
                    }
                    StiEncryption.RC4(ref result1, hash);
                }
            }

            result1.CopyTo(ownerValue, 0);
            #endregion

            #region compute encryption key and user value
            string password2 = passwordUser;
            byte[] pass2 = PadPassword(password2);

            byte[] pp = BitConverter.GetBytes(securityFlags);

            MemoryStream ms = new MemoryStream();
            ms.Write(pass2, 0, pass2.Length);
            ms.Write(result1, 0, result1.Length);
            ms.Write(pp, 0, pp.Length);
            ms.Write(IDValue, 0, IDValue.Length);
            ms.Seek(0, SeekOrigin.Begin);

            hash = StiMD5Helper.ComputeHash(ms);

            ms.Close();

            if (keyLength == StiPdfEncryptionKeyLength.Bit128)
            {
                //for release 3
                for (int index1 = 0; index1 < 50; index1++)
                {
                    hash = StiMD5Helper.ComputeHash(hash);
                }
            }

            //create encryption key
            encryptionKey = (keyLength == StiPdfEncryptionKeyLength.Bit128) ? new byte[16] : new byte[5];
            for (int index = 0; index < encryptionKey.Length; index++)
            {
                encryptionKey[index] = hash[index];
            }

            byte[] result2 = new byte[32];

            if (keyLength == StiPdfEncryptionKeyLength.Bit128)
            {
                #region for release 3
                ms = new MemoryStream();
                ms.Write(paddingString, 0, paddingString.Length);
                ms.Write(IDValue, 0, IDValue.Length);
                ms.Seek(0, SeekOrigin.Begin);

                hash = StiMD5Helper.ComputeHash(ms);

                ms.Close();

                StiEncryption.RC4(ref hash, encryptionKey);

                byte[] newKey = new byte[16];
                for (int index2 = 1; index2 <= 19; index2++)
                {
                    for (int tempIndex = 0; tempIndex < 16; tempIndex++)
                    {
                        newKey[tempIndex] = (byte)(encryptionKey[tempIndex] ^ (byte)index2);
                    }
                    StiEncryption.RC4(ref hash, newKey);
                }

                paddingString.CopyTo(result2, 0);
                hash.CopyTo(result2, 0);
                #endregion
            }
            else
            {
                #region for release 2
                paddingString.CopyTo(result2, 0);

                //encrypt the user password
                StiEncryption.RC4(ref result2, encryptionKey);
                #endregion
            }

            result2.CopyTo(userValue, 0);
            #endregion

            return true;
        }

        private bool ComputingCryptoValuesV4()
        {
            string password = passwordOwner;
            if (passwordOwner == string.Empty) password = passwordUser;
            byte[] pass1 = PadPassword(password);

            #region compute owner value
            //add data to hash
            byte[] hash = StiMD5Helper.ComputeHash(pass1);

            for (int index1 = 0; index1 < 50; index1++)
            {
                hash = StiMD5Helper.ComputeHash(hash);
            }

            byte[] result1 = PadPassword(passwordUser);

            //encrypt the user password
            Tools.StiEncryption.RC4(ref result1, hash);

            for (int index2 = 1; index2 <= 19; index2++)
            {
                for (int tempIndex = 0; tempIndex < 16; tempIndex++)
                {
                    hash[tempIndex] ^= (byte)(index2 - 1);
                    hash[tempIndex] ^= (byte)index2;
                }
                StiEncryption.RC4(ref result1, hash);
            }

            result1.CopyTo(ownerValue, 0);
            #endregion

            #region compute encryption key and user value
            string password2 = passwordUser;
            byte[] pass2 = PadPassword(password2);

            byte[] pp = BitConverter.GetBytes(securityFlags);

            MemoryStream ms = new MemoryStream();
            ms.Write(pass2, 0, pass2.Length);
            ms.Write(result1, 0, result1.Length);
            ms.Write(pp, 0, pp.Length);
            ms.Write(IDValue, 0, IDValue.Length);
            ms.Seek(0, SeekOrigin.Begin);

            hash = StiMD5Helper.ComputeHash(ms);

            ms.Close();

            for (int index1 = 0; index1 < 50; index1++)
            {
                hash = StiMD5Helper.ComputeHash(hash);
            }

            //create encryption key
            encryptionKey = new byte[16];
            for (int index = 0; index < encryptionKey.Length; index++)
            {
                encryptionKey[index] = hash[index];
            }

            byte[] result2 = new byte[32];

            ms = new MemoryStream();
            ms.Write(paddingString, 0, paddingString.Length);
            ms.Write(IDValue, 0, IDValue.Length);
            ms.Seek(0, SeekOrigin.Begin);

            hash = StiMD5Helper.ComputeHash(ms);

            ms.Close();

            StiEncryption.RC4(ref hash, encryptionKey);

            byte[] newKey = new byte[16];
            for (int index2 = 1; index2 <= 19; index2++)
            {
                for (int tempIndex = 0; tempIndex < 16; tempIndex++)
                {
                    newKey[tempIndex] = (byte)(encryptionKey[tempIndex] ^ (byte)index2);
                }
                StiEncryption.RC4(ref hash, newKey);
            }

            paddingString.CopyTo(result2, 0);
            hash.CopyTo(result2, 0);

            result2.CopyTo(userValue, 0);
            #endregion

            return true;
        }

        private bool ComputingCryptoValuesV5()
        {
            Random rnd = new Random();
            byte[] buf = null;
            byte[] hash = null;

            byte[] baseEncryptionKey = new byte[32];
            rnd.NextBytes(baseEncryptionKey);

            byte[] userValidationSalt = new byte[8];
            byte[] userKeySalt = new byte[8];
            byte[] ownerValidationSalt = new byte[8];
            byte[] ownerKeySalt = new byte[8];
            rnd.NextBytes(userValidationSalt);
            rnd.NextBytes(userKeySalt);
            rnd.NextBytes(ownerValidationSalt);
            rnd.NextBytes(ownerKeySalt);

            #region Computing the encryption dictionary’s U (user password) and UE (user encryption key) values
            byte[] passUser = Encoding.UTF8.GetBytes(passwordUser);
            if (passUser.Length > 127)
            {
                byte[] tempBuf = new byte[127];
                Array.Copy(passUser, 0, tempBuf, 0, 127);
                passUser = tempBuf;
            }

            hash = GetHashV5(passUser, userValidationSalt, null);

            //make U value
            hash.CopyTo(userValue, 0);
            userValidationSalt.CopyTo(userValue, 32);
            userKeySalt.CopyTo(userValue, 40);

            hash = GetHashV5(passUser, userKeySalt, null);

            //make UE value
            EncodeKeyDataV5(hash, baseEncryptionKey, true).CopyTo(userExtendedValue, 0);
            #endregion

            #region Computing the encryption dictionary’s O (owner password) and OE (owner encryption key) values
            string password = passwordOwner;
            if (passwordOwner == string.Empty) password = passwordUser;

            byte[] passOwner = Encoding.UTF8.GetBytes(password);
            if (passOwner.Length > 127)
            {
                byte[] tempBuf = new byte[127];
                Array.Copy(passOwner, 0, tempBuf, 0, 127);
                passOwner = tempBuf;
            }

            hash = GetHashV5(passOwner, ownerValidationSalt, userValue);

            //make O value
            hash.CopyTo(ownerValue, 0);
            ownerValidationSalt.CopyTo(ownerValue, 32);
            ownerKeySalt.CopyTo(ownerValue, 40);

            hash = GetHashV5(passOwner, ownerKeySalt, userValue);

            //make OE value
            EncodeKeyDataV5(hash, baseEncryptionKey, true).CopyTo(ownerExtendedValue, 0);
            #endregion

            #region Computing the encryption dictionary’s Perms (permissions) value
            buf = new byte[16];

            rnd.NextBytes(buf); //Set bytes 12-15 to 4 bytes of random data

            UInt64 pExt = securityFlags | 0xFFFFFFFF00000000;
            BitConverter.GetBytes(pExt).CopyTo(buf, 0);

            buf[8] = (byte)'T'; //ASCII value 'T' or 'F' according to the EncryptMetadata Boolean.
            buf[9] = (byte)'a';
            buf[10] = (byte)'d';
            buf[11] = (byte)'b';

            permsValue = EncodeKeyDataV5(baseEncryptionKey, buf, false);
            #endregion

            #region Computing an encryption key
            hash = GetHashV5(passUser, userKeySalt, null);
            encryptionKey = DecodeKeyDataV5(hash, userExtendedValue, true);
            #endregion

            #region Verifying key calculation
            buf = DecodeKeyDataV5(encryptionKey, permsValue, false);
            if ((buf[9] != 'a') || (buf[10] != 'd') || (buf[11] != 'b'))
            {
                ThrowEncryptionError(102);
            }
            #endregion

            return true;
        }

        #region Encode/Decode V5 methods
        private byte[] EncodeKeyDataV5(byte[] key, byte[] data, bool cbc)
        {
            byte[] result = null;
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Mode = cbc ? CipherMode.CBC : CipherMode.ECB;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.None;
                aes.Key = key;
                aes.IV = new byte[16];

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                        result = msEncrypt.ToArray();
                    }
                }
            }
            return result;
        }

        private byte[] DecodeKeyDataV5(byte[] key, byte[] data, bool cbc)
        {
            byte[] result = new byte[32];
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Mode = cbc ? CipherMode.CBC : CipherMode.ECB;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.None;
                aes.Key = key;
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        csDecrypt.Read(result, 0, result.Length);
                    }
                }
            }
            return result;
        }
        #endregion

        #region Hash calculations
        private byte[] GetHashV5(byte[] password, byte[] salt, byte[] ownerkey)
        {
            if (keyLength == StiPdfEncryptionKeyLength.Bit256_r5)
                return Calculate_hash_r5(password, salt, ownerkey);
            else
                return Calculate_hash_r6(password, salt, ownerkey);
        }

        private static byte[] Calculate_hash_r5(byte[] password, byte[] salt, byte[] ownerkey)
        {
            byte[] buf = new byte[password.Length + 8 + (ownerkey != null ? 48 : 0)];
            password.CopyTo(buf, 0);
            salt.CopyTo(buf, password.Length);
            if (ownerkey != null)
                ownerkey.CopyTo(buf, password.Length + 8);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(buf);
        }

        private static byte[] Calculate_hash_r6(byte[] password, byte[] salt, byte[] ownerkey)
        {
            byte[] data = new byte[(128 + 64 + 48) * 64];
            byte[] block = new byte[64];
            int block_size = 32;
            int data_len = 0;

            byte[] buf = null;
            byte[] key = new byte[16];
            byte[] iv = new byte[16];

            SHA256Managed sha256 = new SHA256Managed();
            SHA384Managed sha384 = new SHA384Managed();
            SHA512Managed sha512 = new SHA512Managed();

            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.CBC;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.None;

            /* Step 1: calculate initial data block */
            buf = new byte[password.Length + 8 + (ownerkey != null ? 48 : 0)];
            password.CopyTo(buf, 0);
            salt.CopyTo(buf, password.Length);
            if (ownerkey != null)
                ownerkey.CopyTo(buf, password.Length + 8);
            sha256.ComputeHash(buf).CopyTo(block, 0);

            for (int i = 0; i < 64 || i < data[data_len * 64 - 1] + 32; i++)
            {
                /* Step 2: repeat password and data block 64 times */
                password.CopyTo(data, 0);
                Array.Copy(block, 0, data, password.Length, block_size);
                data_len = password.Length + block_size;
                if (ownerkey != null)
                {
                    ownerkey.CopyTo(data, password.Length + block_size);
                    data_len += 48;
                }
                for (int j = 1; j < 64; j++)
                    Array.Copy(data, 0, data, j * data_len, data_len);

                /* Step 3: encrypt data using data block as key and iv */
                byte[] result = null;
                Array.Copy(block, 0, key, 0, 16);
                Array.Copy(block, 16, iv, 0, 16);
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data_len * 64);
                        csEncrypt.FlushFinalBlock();
                        result = msEncrypt.ToArray();
                    }
                }
                result.CopyTo(data, 0);

                /* Step 4: determine SHA-2 hash size for this round */
                int sum = 0;
                for (int j = 0; j < 16; j++)
                    sum += data[j];

                /* Step 5: calculate data block for next round */
                block_size = 32 + (sum % 3) * 16;
                switch (block_size)
                {
                    case 32:
                        sha256.ComputeHash(data, 0, data_len * 64).CopyTo(block, 0);
                        break;
                    case 48:
                        sha384.ComputeHash(data, 0, data_len * 64).CopyTo(block, 0);
                        break;
                    case 64:
                        sha512.ComputeHash(data, 0, data_len * 64).CopyTo(block, 0);
                        break;
                }
            }

            byte[] hash = new byte[32];
            Array.Copy(block, 0, hash, 0, 32);
            return hash;
        }
        #endregion

        #endregion

        #region EncryptData
        public byte[] EncryptData(byte[] data, int currentObjectNumber, int currentGenerationNumber)
        {
            if (keyLength == StiPdfEncryptionKeyLength.Bit40 || keyLength == StiPdfEncryptionKeyLength.Bit128)
            {
                #region V2 40bit and V3 128bit
                int objectNumber = currentObjectNumber;
                int generationNumber = currentGenerationNumber;

                byte[] hashData = new byte[encryptionKey.Length + 5];
                encryptionKey.CopyTo(hashData, 0);
                BitConverter.GetBytes(objectNumber).CopyTo(hashData, hashData.Length - 5);
                BitConverter.GetBytes((ushort)generationNumber).CopyTo(hashData, hashData.Length - 2);

                byte[] hash = StiMD5Helper.ComputeHash(hashData);

                if (keyLength == StiPdfEncryptionKeyLength.Bit40)
                {
                    byte[] buf = new byte[10];
                    Array.Copy(hash, buf, 10);
                    hash = buf;
                }
                StiEncryption.RC4(ref data, hash);
                #endregion
            }
            else if (keyLength == StiPdfEncryptionKeyLength.Bit128_r4)
            {
                #region V4 128bit
                int objectNumber = currentObjectNumber;
                int generationNumber = currentGenerationNumber;

                byte[] hashData = new byte[16 + 5 + 4];
                encryptionKey.CopyTo(hashData, 0);
                BitConverter.GetBytes(objectNumber).CopyTo(hashData, 16);
                BitConverter.GetBytes((ushort)generationNumber).CopyTo(hashData, 19);
                hashData[21] = 0x73;
                hashData[22] = 0x41;
                hashData[23] = 0x6C;
                hashData[24] = 0x54;

                byte[] hash = StiMD5Helper.ComputeHash(hashData);

                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.BlockSize = 128;
                    aes.Key = hash;
                    aes.GenerateIV();

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(aes.IV, 0, aes.IV.Length);
                            csEncrypt.Write(data, 0, data.Length);
                            csEncrypt.FlushFinalBlock();
                            data = msEncrypt.ToArray();
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region V5 256bit
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.BlockSize = 128;
                    aes.Key = encryptionKey;
                    aes.GenerateIV();

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(aes.IV, 0, aes.IV.Length);
                            csEncrypt.Write(data, 0, data.Length);
                            csEncrypt.FlushFinalBlock();
                            data = msEncrypt.ToArray();
                        }
                    }
                }
                #endregion
            }
            return data;
        }
        #endregion

        private void ThrowEncryptionError(int step)
        {
            Win32Exception myEx = new Win32Exception(Marshal.GetLastWin32Error());
            throw new Exception(string.Format("{0} {1}, code #{2:X8}: {3}", StiLocalization.Get("Export", "EncryptionError"), step, myEx.ErrorCode, myEx.Message));
        }
        #endregion

        #region RenderEncodeRecord
        internal void RenderEncodeRecord(StreamWriter sw)
        {
            StringBuilder sbOwner = new StringBuilder();
            StringBuilder sbUser = new StringBuilder();
            int sbLength = 32;
            if (keyLength == StiPdfEncryptionKeyLength.Bit256_r5 || keyLength == StiPdfEncryptionKeyLength.Bit256_r6) sbLength = 48;
            for (int index = 0; index < sbLength; index++)
            {
                sbOwner.Append((char)ownerValue[index]);
                sbUser.Append((char)userValue[index]);
            }

            sw.WriteLine("/Filter /Standard");
            if (keyLength == StiPdfEncryptionKeyLength.Bit128)
            {
                sw.WriteLine("/CF<</StdCF<</AuthEvent/DocOpen/CFM/V2/Length 16>>>>");
                sw.WriteLine("/StmF /StdCF");
                sw.WriteLine("/StrF /StdCF");
                sw.WriteLine("/R 3");
                sw.WriteLine("/V 2");
                sw.WriteLine("/Length 128");
            }
            else if (keyLength == StiPdfEncryptionKeyLength.Bit128_r4)
            {
                sw.WriteLine("/CF<</StdCF<</AuthEvent/DocOpen/CFM/AESV2/Length 16>>>>");
                sw.WriteLine("/StmF /StdCF");
                sw.WriteLine("/StrF /StdCF");
                sw.WriteLine("/R 4");
                sw.WriteLine("/V 4");
                sw.WriteLine("/Length 128");
            }
            else if (keyLength == StiPdfEncryptionKeyLength.Bit256_r5 || keyLength == StiPdfEncryptionKeyLength.Bit256_r6)
            {
                sw.WriteLine("/CF<</StdCF<</AuthEvent/DocOpen/CFM/AESV3/Length 32>>>>");
                sw.WriteLine("/StmF /StdCF");
                sw.WriteLine("/StrF /StdCF");
                sw.WriteLine("/R {0}", keyLength == StiPdfEncryptionKeyLength.Bit256_r5 ? 5 : 6);
                sw.WriteLine("/V 5");
                sw.WriteLine("/Length 256");

                StringBuilder sbOwnerE = new StringBuilder();
                StringBuilder sbUserE = new StringBuilder();
                for (int index = 0; index < 32; index++)
                {
                    sbOwnerE.Append((char)ownerExtendedValue[index]);
                    sbUserE.Append((char)userExtendedValue[index]);
                }
                StringBuilder sbPerms = new StringBuilder();
                for (int index = 0; index < 16; index++)
                {
                    sbPerms.Append((char)permsValue[index]);
                }

                pdfService.StoreString(string.Format("/OE ({0})", StiPdfExportService.ConvertToEscapeSequencePlusTabs(sbOwnerE.ToString()))); sw.WriteLine("");
                pdfService.StoreString(string.Format("/UE ({0})", StiPdfExportService.ConvertToEscapeSequencePlusTabs(sbUserE.ToString()))); sw.WriteLine("");
                pdfService.StoreString(string.Format("/Perms ({0})", StiPdfExportService.ConvertToEscapeSequencePlusTabs(sbPerms.ToString()))); sw.WriteLine("");
            }
            else
            {
                sw.WriteLine("/R 2");
                sw.WriteLine("/V 1");
                sw.WriteLine("/Length 40");
            }
            pdfService.StoreString(string.Format("/O ({0})", StiPdfExportService.ConvertToEscapeSequencePlusTabs(sbOwner.ToString()))); sw.WriteLine("");
            pdfService.StoreString(string.Format("/U ({0})", StiPdfExportService.ConvertToEscapeSequencePlusTabs(sbUser.ToString()))); sw.WriteLine("");
            sw.WriteLine("/P {0}", (int)securityFlags);
        }
        #endregion

        #region Digital signature

        #region Win32
        private class Win32
        {
            #region Structs

            //typedef struct _CERT_CONTEXT {
            //  DWORD      dwCertEncodingType;
            //  BYTE *     pbCertEncoded;
            //  DWORD      cbCertEncoded;
            //  PCERT_INFO pCertInfo;
            //  HCERTSTORE hCertStore;
            //}CERT_CONTEXT, *PCERT_CONTEXT;

            [StructLayout(LayoutKind.Sequential)]
            public struct CERT_CONTEXT
            {
                public uint dwCertEncodingType;
                public IntPtr pbCertEncoded;
                public uint cbCertEncoded;
                public IntPtr pCertInfo;
                public IntPtr hCertStore;
            }


            //typedef struct _CRYPTOAPI_BLOB {
            //  DWORD cbData;
            //  BYTE *pbData;
            //}CRYPT_INTEGER_BLOB, *PCRYPT_INTEGER_BLOB, CRYPT_UINT_BLOB, *PCRYPT_UINT_BLOB, CRYPT_OBJID_BLOB, *PCRYPT_OBJID_BLOB,

            [StructLayout(LayoutKind.Sequential)]
            public struct CRYPT_OBJID_BLOB
            {
                public uint cbData;
                public IntPtr pbData;
            }


            //typedef struct _CRYPT_ALGORITHM_IDENTIFIER {
            //  LPSTR            pszObjId;
            //  CRYPT_OBJID_BLOB Parameters;
            //}CRYPT_ALGORITHM_IDENTIFIER, *PCRYPT_ALGORITHM_IDENTIFIER;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct CRYPT_ALGORITHM_IDENTIFIER
            {
                [MarshalAs(UnmanagedType.LPStr)]
                public string pszObjId;
                public CRYPT_OBJID_BLOB Parameters;
            }


            //typedef struct _CRYPT_SIGN_MESSAGE_PARA {
            //  DWORD                      cbSize;
            //  DWORD                      dwMsgEncodingType;
            //  PCCERT_CONTEXT             pSigningCert;
            //  CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
            //  void *                     pvHashAuxInfo;
            //  DWORD                      cMsgCert;
            //  PCCERT_CONTEXT *           rgpMsgCert;
            //  DWORD                      cMsgCrl;
            //  PCCRL_CONTEXT *            rgpMsgCrl;
            //  DWORD                      cAuthAttr;
            //  PCRYPT_ATTRIBUTE           rgAuthAttr;
            //  DWORD                      cUnauthAttr;
            //  PCRYPT_ATTRIBUTE           rgUnauthAttr;
            //  DWORD                      dwFlags;
            //  DWORD                      dwInnerContentType;
            //  CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
            //  void                       pvHashEncryptionAuxInfo;
            //}CRYPT_SIGN_MESSAGE_PARA, *PCRYPT_SIGN_MESSAGE_PARA;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct CRYPT_SIGN_MESSAGE_PARA
            {
                public UInt32 cbSize;
                public UInt32 dwMsgEncodingType;
                public IntPtr pSigningCert;
                public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
                public IntPtr pvHashAuxInfo;
                public UInt32 cMsgCert;
                public IntPtr rgpMsgCert;
                public UInt32 cMsgCrl;
                public IntPtr rgpMsgCrl;
                public UInt32 cAuthAttr;
                public IntPtr rgAuthAttr;
                public UInt32 cUnauthAttr;
                public IntPtr rgUnauthAttr;
                public UInt32 dwFlags;
                public UInt32 dwInnerContentType;
                public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
                public IntPtr pvHashEncryptionAuxInfo;
            }

            #endregion

            #region Constants
            public const uint PKCS_7_ASN_ENCODING = 0x00010000;
            public const uint X509_ASN_ENCODING = 0x00000001;
            public const uint CERT_SYSTEM_STORE_CURRENT_USER = 0x00010000;
            public const uint CERT_SYSTEM_STORE_LOCAL_MACHINE = 0x00020000;
            public const uint CERT_FIND_SUBJECT_NAME = 0x00020007;
            public const uint CERT_FIND_SUBJECT_STR = 0x00080007;
            public const int CERT_STORE_PROV_SYSTEM = 10;
            public const uint CERT_STORE_READONLY_FLAG = 0x00008000;
            public const uint CERT_CLOSE_STORE_CHECK_FLAG = 0x00000002;
            //			public const string szOID_RSA_MD5 = "1.2.840.113549.1.1.4"; 
            public const string szOID_RSA_SHA1RSA = "1.2.840.113549.1.1.5";
            public const string szOID_sha256NoSign = "2.16.840.1.101.3.4.2.1";
            public const uint CRYPTUI_SELECT_ISSUEDTO_COLUMN = 0x000000001;
            public const uint CRYPTUI_SELECT_ISSUEDBY_COLUMN = 0x000000002;
            public const uint CRYPTUI_SELECT_INTENDEDUSE_COLUMN = 0x000000004;
            public const uint CRYPTUI_SELECT_FRIENDLYNAME_COLUMN = 0x000000008;
            public const uint CRYPTUI_SELECT_LOCATION_COLUMN = 0x000000010;
            public const uint CRYPTUI_SELECT_EXPIRATION_COLUMN = 0x000000020;

            public const string szOID_CP_GOST_R3411 = "1.2.643.2.2.9";
            public const string szOID_CP_GOST_R3411_R3410 = "1.2.643.2.2.4";
            public const string szOID_CP_GOST_R3411_R3410EL = "1.2.643.2.2.3";
            #endregion

            #region DllImport
            [DllImport(@"crypt32.dll", EntryPoint = "CryptSignMessage", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern Boolean CryptSignMessage(
                ref CRYPT_SIGN_MESSAGE_PARA pSignPara,
                Boolean fDetachedSignature,
                Int32 cToBeSigned,
                IntPtr[] rgpbToBeSigned,
                Int32[] rgcbToBeSigned,
                Byte[] pbSignedBlob,
                ref Int32 pcbSignedBlob);


            [DllImport("Crypt32.dll", EntryPoint = "CertOpenStore", SetLastError = true)]
            public static extern IntPtr CertOpenStore
                (
                IntPtr lpszStoreProvider,
                UInt32 dwEncodingType,
                IntPtr hCryptProv,
                UInt32 dwFlags,
                byte[] pvPara
                );

            //[DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            //public static extern IntPtr CertOpenSystemStore(
            //    IntPtr hprov,
            //    String szSubsystemProtocol);

            [DllImport("Crypt32.dll", EntryPoint = "CertFindCertificateInStore", SetLastError = true)]
            public static extern IntPtr CertFindCertificateInStore
                (
                IntPtr hCertStore,
                UInt32 dwCertEncodingType,
                UInt32 dwFindFlags,
                UInt32 dwFindType,
                IntPtr pvFindPara,
                IntPtr pPrevCertContext
                );

            [DllImport("crypt32.dll", SetLastError = true)]
            public static extern int CertCloseStore(
                IntPtr hCertStore,
                uint dwFlags);

            [DllImport("crypt32.dll", SetLastError = true)]
            public static extern int CertFreeCertificateContext(
                IntPtr pCertContext);

            [DllImport("cryptUI.dll", SetLastError = true)]
            public static extern IntPtr CryptUIDlgSelectCertificateFromStore(
                IntPtr hCertStore,
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.LPWStr)] string pwszTitle,
                [MarshalAs(UnmanagedType.LPWStr)] string pwszDisplayString,
                uint dwDontUseColumn,
                uint dwFlags,
                IntPtr pvReserved);

            [DllImport(@"crypt32.dll")]
            public static extern int CertVerifyTimeValidity(
                IntPtr pTimeToVerify,
                IntPtr pCertInfo);

            #endregion
        }
        #endregion

        private static void ThrowDigitalSignError(int step, string message = null, bool showCode = false)
        {
            string nameError = StiLocalization.Get("Export", "DigitalSignatureError");
            Win32Exception myEx = new Win32Exception(Marshal.GetLastWin32Error());
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception(string.Format(nameError + " {0}, code #{1:X8}: {2}", step, myEx.ErrorCode, myEx.Message));
            }
            if (showCode)
            {
                throw new Exception(string.Format(nameError + " {0}: {1} \r\n(code #{2:X8}: {3})", step, message, myEx.ErrorCode, myEx.Message));
            }
            throw new Exception(string.Format(nameError + " {0}: {1}", step, message));
        }

        public byte[] CreateSignature(byte[] buf, bool getCertificateFromCryptoUI, string certificateID, bool useLocalMachineCertificates,
            byte[] certificateData, string certificatePassword, string certificateThumbprint, out bool isGost, ref string signedBy, int offsetFilter, int offsetSignedBy)
        {
            int outBufferSize = 0;
            byte[] outputBuffer = null;

            IntPtr pCertContext = IntPtr.Zero;
            IntPtr hStoreHandle = IntPtr.Zero;

            X509Store store = null;
            X509Certificate2Collection oCerts = null;
            X509Certificate2 cert1 = null;

            uint ENCODING_TYPE = Win32.PKCS_7_ASN_ENCODING | Win32.X509_ASN_ENCODING;

            string CERT_STORE_NAME = "MY";
            uint CERT_SYSTEM_STORE = Win32.CERT_SYSTEM_STORE_CURRENT_USER;
            if (useLocalMachineCertificates)
            {
                //CERT_STORE_NAME = "Root";
                CERT_SYSTEM_STORE = Win32.CERT_SYSTEM_STORE_LOCAL_MACHINE | Win32.CERT_STORE_READONLY_FLAG;
            }

            if (getCertificateFromCryptoUI)
            {
                #region Open store
                hStoreHandle = Win32.CertOpenStore(
                    (IntPtr)Win32.CERT_STORE_PROV_SYSTEM,
                    0,
                    IntPtr.Zero,
                    CERT_SYSTEM_STORE,
                    Encoding.Unicode.GetBytes(CERT_STORE_NAME));
                if (hStoreHandle == IntPtr.Zero) ThrowDigitalSignError(1);
                #endregion

                #region CryptUI SelectCertificateFromStore
                pCertContext = Win32.CryptUIDlgSelectCertificateFromStore(
                    hStoreHandle,
                    IntPtr.Zero,
                    null,
                    null,
                    Win32.CRYPTUI_SELECT_LOCATION_COLUMN | Win32.CRYPTUI_SELECT_ISSUEDBY_COLUMN | Win32.CRYPTUI_SELECT_FRIENDLYNAME_COLUMN,
                    0,
                    IntPtr.Zero);
                if (pCertContext == IntPtr.Zero) ThrowDigitalSignError(2, StiLocalization.Get("Export", "DigitalSignatureCertificateNotSelected"));
                #endregion
            }
            else
            {
                #region Process given certificate
                if (certificateData != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(certificatePassword))
                        {
                            if (useLocalMachineCertificates)
                            {
                                cert1 = new X509Certificate2(certificateData, certificatePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                            }
                            else
                            {
                                cert1 = new X509Certificate2(certificateData, certificatePassword);
                            }
                        }
                        else
                        {
                            cert1 = new X509Certificate2(certificateData);
                        }
                        if (cert1 == null) ThrowDigitalSignError(2, "Certificate data or password is incorrect");
                    }
                    catch (Exception ex)
                    {
                        ThrowDigitalSignError(21, ex.Message);
                    }
                }
                #endregion

                if (cert1 == null)
                {
                    if (string.IsNullOrWhiteSpace(certificateThumbprint) && string.IsNullOrEmpty(certificateID)) ThrowDigitalSignError(2, "Subject name string is empty");

                    #region Get certificate from X509Store
                    store = new X509Store(CERT_STORE_NAME, useLocalMachineCertificates ? StoreLocation.LocalMachine : StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadOnly);

                    if (!string.IsNullOrWhiteSpace(certificateThumbprint))
                    {
                        oCerts = (X509Certificate2Collection)store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
                    }
                    else
                    {
                        oCerts = (X509Certificate2Collection)store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificateID, false);
                        if (oCerts.Count == 0)
                        {
                            oCerts = (X509Certificate2Collection)store.Certificates.Find(X509FindType.FindBySubjectName, certificateID, false);
                        }
                    }
                    oCerts = oCerts.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                    if (oCerts.Count > 0)
                    {
                        cert1 = oCerts[0];
                    }
                    #endregion
                }

                if (cert1 != null)
                {
                    pCertContext = cert1.Handle;
                }
                else
                //if (pCertContext == null || pCertContext == IntPtr.Zero)
                {
                    oCerts.Clear();
                    oCerts = null;
                    store.Close();
                    store = null;

                    #region Open store
                    hStoreHandle = Win32.CertOpenStore(
                        (IntPtr)Win32.CERT_STORE_PROV_SYSTEM,
                        0,
                        IntPtr.Zero,
                        CERT_SYSTEM_STORE,
                        Encoding.Unicode.GetBytes(CERT_STORE_NAME));
                    if (hStoreHandle == IntPtr.Zero) ThrowDigitalSignError(1);
                    #endregion

                    #region Get certificate from Win32 store
                    int certificateIsValid = 0;
                    IntPtr SIGNER_NAME = Marshal.StringToBSTR(certificateID);
                    do
                    {
                        pCertContext = Win32.CertFindCertificateInStore(
                            hStoreHandle,
                            ENCODING_TYPE,
                            0,
                            Win32.CERT_FIND_SUBJECT_STR,
                            SIGNER_NAME,
                            pCertContext);
                        if (pCertContext == IntPtr.Zero) ThrowDigitalSignError(2, "Certificate is not found", true);

                        Win32.CERT_CONTEXT SignerCert = (Win32.CERT_CONTEXT)Marshal.PtrToStructure(pCertContext, typeof(Win32.CERT_CONTEXT));
                        certificateIsValid = Win32.CertVerifyTimeValidity(IntPtr.Zero, SignerCert.pCertInfo);
                    }
                    while (certificateIsValid != 0);
                    #endregion
                }
            }

            if (cert1 == null)
            {
                #region Try to create X509 certificate for get SignatureAlgorithm value
                try
                {
                    cert1 = new X509Certificate2(pCertContext);
                }
                catch
                {
                }
                #endregion
            }

            isGost = (cert1 != null) && (cert1.SignatureAlgorithm.Value == Win32.szOID_CP_GOST_R3411_R3410 || cert1.SignatureAlgorithm.Value == Win32.szOID_CP_GOST_R3411_R3410EL);

            if (isGost)
            {
                Encoding.ASCII.GetBytes("CryptoPro#20PDF").CopyTo(buf, offsetFilter);
            }
            if (string.IsNullOrEmpty(signedBy) && (cert1 != null))
            {
                #region Get SignedBy from certificate
                try
                {
                    string tempSt = cert1.Subject;
                    int pos = tempSt.IndexOf("CN=", StringComparison.InvariantCulture);
                    if (pos != -1)
                    {
                        pos += 3;
                        if (tempSt[pos] == '"')
                        {
                            int pos2 = tempSt.IndexOf('"', pos + 1);
                            if (pos2 != -1)
                            {
                                signedBy = tempSt.Substring(pos + 1, pos2 - pos - 1);
                            }
                        }
                        else
                        {
                            int pos2 = tempSt.IndexOf(',', pos);
                            if (pos2 == -1) pos2 = tempSt.Length;
                            signedBy = tempSt.Substring(pos, pos2 - pos);
                        }
                    }
                }
                catch
                {
                }
                #endregion

                if (!string.IsNullOrEmpty(signedBy))
                {
                    signedBy = MakeSignedByString(signedBy);
                    if (!string.IsNullOrWhiteSpace(signedBy))
                    {
                        byte[] data = new byte[signedBy.Length];
                        for (int index = 0; index < signedBy.Length; index++)
                        {
                            data[index] = (byte)signedBy[index];
                        }
                        data.CopyTo(buf, offsetSignedBy);
                    }
                }
            }

            #region Prepare structures
            Win32.CRYPT_SIGN_MESSAGE_PARA SignPara = new Win32.CRYPT_SIGN_MESSAGE_PARA();
            SignPara.cbSize = (uint)Marshal.SizeOf(typeof(Win32.CRYPT_SIGN_MESSAGE_PARA));
            SignPara.dwMsgEncodingType = ENCODING_TYPE;
            SignPara.pSigningCert = pCertContext;
            if (isGost)
            {
                SignPara.HashAlgorithm.pszObjId = Win32.szOID_CP_GOST_R3411;
            }
            else
            {
                if (StiOptions.Export.Pdf.DigitalSignatureDigestSHA256)
                    SignPara.HashAlgorithm.pszObjId = Win32.szOID_sha256NoSign;
                else
                    SignPara.HashAlgorithm.pszObjId = Win32.szOID_RSA_SHA1RSA;
            }
            SignPara.HashAlgorithm.Parameters.cbData = 0;
            SignPara.HashAlgorithm.Parameters.pbData = IntPtr.Zero;
            SignPara.pvHashAuxInfo = IntPtr.Zero;
            SignPara.cMsgCrl = 0;
            SignPara.rgpMsgCrl = IntPtr.Zero;
            SignPara.cAuthAttr = 0;
            SignPara.rgAuthAttr = IntPtr.Zero;
            SignPara.cUnauthAttr = 0;
            SignPara.rgUnauthAttr = IntPtr.Zero;
            SignPara.dwFlags = 0;
            SignPara.dwInnerContentType = 0;
            SignPara.pvHashEncryptionAuxInfo = IntPtr.Zero;
            SignPara.cMsgCert = 1;

            IntPtr[] MessageArray = new IntPtr[1];
            MessageArray[0] = Marshal.AllocHGlobal(buf.Length);
            Marshal.Copy(buf, 0, MessageArray[0], buf.Length);
            Int32[] MessageSizeArray = new Int32[1];
            MessageSizeArray[0] = buf.Length;

            GCHandle GC = GCHandle.Alloc(pCertContext, GCHandleType.Pinned);
            SignPara.rgpMsgCert = GC.AddrOfPinnedObject();
            #endregion

            #region Sign message
            bool res = Win32.CryptSignMessage(
                ref SignPara,
                true,
                1,
                MessageArray,
                MessageSizeArray,
                null,
                ref outBufferSize);

            if (res == false) ThrowDigitalSignError(3);

            outputBuffer = new byte[outBufferSize];

            res = Win32.CryptSignMessage(
                ref SignPara,
                true,
                1,
                MessageArray,
                MessageSizeArray,
                outputBuffer,
                ref outBufferSize);

            if (res == false) ThrowDigitalSignError(31);
            #endregion

            #region Free resources
            GC.Free();

            if (MessageArray[0] != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(MessageArray[0]);
            }

            if (cert1 != null)
            {
                cert1.Reset();
                cert1 = null;
            }

            if (store != null)
            {
                oCerts.Clear();
                oCerts = null;
                store.Close();
                store = null;
            }

            if (hStoreHandle != IntPtr.Zero)
            {
                int errorInt = 0;
                if (pCertContext != IntPtr.Zero)
                {
                    errorInt = Win32.CertFreeCertificateContext(pCertContext);
                    if (errorInt == 0) ThrowDigitalSignError(4);
                }

                errorInt = Win32.CertCloseStore(hStoreHandle, Win32.CERT_CLOSE_STORE_CHECK_FLAG);
                if (errorInt == 0) ThrowDigitalSignError(5);
            }
            #endregion

            return outputBuffer;
        }

        public string MakeSignedByString(string input, bool padding = true)
        {
            string signedBy = pdfService.ConvertToHexString(input, true);
            if (padding)
            {
                if (signedBy.Length < 256)
                {
                    signedBy += new string(' ', 256 - signedBy.Length);
                }
                else if (signedBy.Length > 256)
                {
                    signedBy = null;
                }
            }
            return signedBy;
        }
        #endregion

        public static string GetCertificateThumbprintFromCryptoUI(bool useLocalMachineCertificates, out string certificateName, out string errorMessage)
        {
            string thumbprint = null;
            errorMessage = null;

            try
            {
                IntPtr pCertContext = IntPtr.Zero;
                IntPtr hStoreHandle = IntPtr.Zero;

                string CERT_STORE_NAME = "MY";
                uint CERT_SYSTEM_STORE = Win32.CERT_SYSTEM_STORE_CURRENT_USER;
                if (useLocalMachineCertificates)
                {
                    //CERT_STORE_NAME = "Root";
                    CERT_SYSTEM_STORE = Win32.CERT_SYSTEM_STORE_LOCAL_MACHINE | Win32.CERT_STORE_READONLY_FLAG;
                }

                #region Open store
                hStoreHandle = Win32.CertOpenStore(
                    (IntPtr)Win32.CERT_STORE_PROV_SYSTEM,
                    0,
                    IntPtr.Zero,
                    CERT_SYSTEM_STORE,
                    Encoding.Unicode.GetBytes(CERT_STORE_NAME));
                if (hStoreHandle == IntPtr.Zero) ThrowDigitalSignError(1);
                #endregion

                #region CryptUI SelectCertificateFromStore
                pCertContext = Win32.CryptUIDlgSelectCertificateFromStore(
                    hStoreHandle,
                    IntPtr.Zero,
                    null,
                    null,
                    Win32.CRYPTUI_SELECT_LOCATION_COLUMN | Win32.CRYPTUI_SELECT_ISSUEDBY_COLUMN | Win32.CRYPTUI_SELECT_FRIENDLYNAME_COLUMN,
                    0,
                    IntPtr.Zero);
                if (pCertContext == IntPtr.Zero) ThrowDigitalSignError(2, StiLocalization.Get("Export", "DigitalSignatureCertificateNotSelected"));
                #endregion

                #region Try to create X509 certificate for get SignatureAlgorithm value
                try
                {
                    var cert1 = new X509Certificate2(pCertContext);

                    if (!cert1.HasPrivateKey)
                    {
                        cert1.Reset();
                        errorMessage = "The certificate contains no private key!";
                        throw new CryptographicException(errorMessage);
                    }

                    thumbprint = cert1.Thumbprint;

                    certificateName = GetFieldValue(cert1.Subject, "CN");

                    cert1.Reset();
                }
                catch
                {
                    certificateName = "";
                }
                #endregion

                if (hStoreHandle != IntPtr.Zero)
                {
                    int errorInt = 0;
                    if (pCertContext != IntPtr.Zero)
                    {
                        errorInt = Win32.CertFreeCertificateContext(pCertContext);
                        if (errorInt == 0) ThrowDigitalSignError(44);
                    }

                    errorInt = Win32.CertCloseStore(hStoreHandle, Win32.CERT_CLOSE_STORE_CHECK_FLAG);
                    if (errorInt == 0) ThrowDigitalSignError(45);
                }
            }
            catch
            {
                certificateName = "";
            }
            
            return thumbprint;
        }

        public class StiCertificateInfo
        {
            public string Name;
            public string Issuer;
            public DateTime From;
            public DateTime To;
            public string Thumbprint;
        }

        public static List<StiCertificateInfo> GetCertificatesList(bool useLocalMachineCertificates)
        {
            List<StiCertificateInfo> list = new List<StiCertificateInfo>();

            try
            {
                string CERT_STORE_NAME = "MY";
                X509Store store = new X509Store(CERT_STORE_NAME, useLocalMachineCertificates ? StoreLocation.LocalMachine : StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection oCerts = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                if (oCerts.Count > 0)
                {
                    foreach (var cert in oCerts)
                    {
                        if (cert.HasPrivateKey)
                        {
                            var certInfo = new StiCertificateInfo();
                            certInfo.Name = !string.IsNullOrWhiteSpace(cert.FriendlyName) ? cert.FriendlyName : GetFieldValue(cert.Subject, "CN");
                            certInfo.Issuer = GetFieldValue(cert.Issuer, "CN");
                            certInfo.From = cert.NotBefore;
                            certInfo.To = cert.NotAfter;
                            certInfo.Thumbprint = cert.Thumbprint;

                            list.Add(certInfo);
                        }
                    }
                }
                oCerts.Clear();
                store.Close();
            }
            catch
            {
            }

            return list;
        }

        private static string GetFieldValue(string certName, string fieldName)
        {
            try
            {
                var index = certName.IndexOf(fieldName + "=");
                if (index < 0)
                    return certName;

                var name = certName.Substring(index + fieldName.Length + 1);
                var end = name.IndexOf(",");
                if (end < 0)
                    return name;
                return name.Substring(0, end);
            }
            catch
            {
                return certName;
            }
        }

        public StiPdfSecurity(StiPdfExportService service)
        {
            this.pdfService = service;
        }
    }
}
