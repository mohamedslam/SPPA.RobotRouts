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

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Stimulsoft.Base
{
    public class StiEncryption
    {
        #region Consts
        private const int rand_m = 714025;
        private const int rand_a = 4096;
        private const int rand_c = 150889;
        #endregion

        #region Methods
        public static byte[] Encrypt(byte[] src, byte[] key)
        {
            return EncryptAdv(src, key);
        }

        public static byte[] Encrypt(byte[] src, string password)
        {
            var byteKey = GetKeyFromPassword(password);
            return EncryptAdv(src, byteKey);
        }

        public static string Encrypt(string src, string password)
        {
            if (src == null) return null;
            
            var byteSrc = Encoding.UTF8.GetBytes(src);
            var byteKey = GetKeyFromPassword(password);
            var dst = EncryptAdv(byteSrc, byteKey);

            return Convert.ToBase64String(dst);
        }

        public static byte[] Decrypt(byte[] src, byte[] key)
        {
            return DecryptAdv(src, key);
        }

        public static byte[] Decrypt(byte[] src, string password)
        {
            var byteKey = GetKeyFromPassword(password);
            return DecryptAdv(src, byteKey);
        }

        public static string Decrypt(string src, string password)
        {
            if (src == null) return null;

            var byteSrc = Convert.FromBase64String(src);
            var byteKey = GetKeyFromPassword(password);
            var dst = DecryptAdv(byteSrc, byteKey);

            return Encoding.UTF8.GetString(dst);
        }

        public static byte[] GenerateRandomKey()
        {
            var key = new byte[32];

            RandomNumberGenerator.Create();
            var rand = new Random();

            for (var index = 0; index < 32; index++)
            {
                key[index] = (byte)rand.Next(0, 255);
            }

            return key;
        }

        private static byte[] EncryptAdv(byte[] src, byte[] key)
        {
            if (src == null) return null;

            var dst = new byte[src.Length];

            dst = CryptRandom(src, key, true);
            dst = CryptXor(dst, key);
            dst = CryptShift(dst, key, true);

            return dst;
        }

        private static byte[] DecryptAdv(byte[] src, byte[] key)
        {
            if (src == null) return null;

            var dst = new byte[src.Length];

            dst = CryptShift(src, key, false);
            dst = CryptXor(dst, key);
            dst = CryptRandom(dst, key, false);

            return dst;
        }

        private static byte[] CryptXor(byte[] src, byte[] key)
        {
            var dst = new byte[src.Length];

            var pos = 0;
            var keyPos = 0;

            while (pos < src.Length)
            {
                if (keyPos >= key.Length) keyPos = 0;
                dst[pos] = (byte)(src[pos] ^ key[keyPos]);

                pos++;
                keyPos++;
            }

            return dst;
        }

        private static byte[] CryptShift(byte[] src, byte[] key, bool encrypt)
        {
            var dst = new byte[src.Length];

            var pos = 0;
            var keyPos = 0;

            while (pos < src.Length)
            {
                if (keyPos >= key.Length) keyPos = 0;

                if (encrypt) dst[pos] = ShiftLeft(src[pos], key[keyPos]);
                else dst[pos] = ShiftRight(src[pos], key[keyPos]);

                pos++;
                keyPos++;
            }

            return dst;
        }

        private static byte ShiftLeft(byte value, byte count)
        {
            var res = value << (count & 0x07);
            res = (res & 0x00FF) | ((res & 0xFF00) >> 8);
            return (byte)res;
        }

        private static byte ShiftRight(byte value, byte count)
        {
            var res = value << (8 - (count & 0x07));
            res = (res & 0x00FF) | ((res & 0xFF00) >> 8);
            return (byte)res;
        }

        private static byte[] CryptRandom(byte[] src, byte[] key, bool encrypt)
        {
            var dst = new byte[src.Length];
            var pos = 0;

            var randomSeed = SetRandomSeed(key);

            var randomMix = GetMixArray(src.Length, randomSeed);
            while (pos < src.Length)
            {
                if (encrypt) dst[pos] = src[randomMix[pos]];
                else dst[randomMix[pos]] = src[pos];

                pos++;
            }

            return dst;
        }

        private static int[] GetMixArray(int count, UInt32 randomSeed)
        {
            var check = new int[count];
            var mix = new int[count];
            for (var index = 0; index < count; index++)
            {
                check[index] = index;
            }
            for (var indexMix = 0; indexMix < count; indexMix++)
            {
                var rnd = GetRandom(0, count - indexMix - 1, ref randomSeed);
                mix[indexMix] = check[rnd];
                check[rnd] = check[count - indexMix - 1];
            }
            return mix;
        }

        private static UInt32 SetRandomSeed(byte[] key)
        {
            var randomSeed = (uint)(key[0] | (key[1] << 8) | (key[key.Length - 2] << 16) | (key[key.Length - 1] << 24));
            randomSeed = randomSeed % rand_m;
            return randomSeed;
        }

        private static int GetRandom(int min, int max, ref UInt32 randomSeed)
        {
            randomSeed = (randomSeed * rand_a + rand_c) % rand_m;
            var jran = (int)(min + (max - min + 1) * randomSeed / rand_m);
            return jran;
        }

        private static byte[] GetKeyFromPassword(string password)
        {
            var ms = new MemoryStream();

            var hash = StiMD5Helper.ComputeHash(Encoding.UTF8.GetBytes(password));

            ms.Write(hash, 0, hash.Length);
            var pos = hash.Length;
            while (pos < password.Length)
            {
                hash = StiMD5Helper.ComputeHash(Encoding.UTF8.GetBytes(password.Substring(0, pos)));

                ms.Write(hash, 0, (pos + hash.Length < password.Length ? hash.Length : password.Length - pos));
                pos += hash.Length;
            }

            var result = ms.ToArray();
            ms.Close();
            ms.Dispose();

            return result;
        }

        #endregion

    }
}
