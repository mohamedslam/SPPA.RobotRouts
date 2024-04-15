#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;

namespace Stimulsoft.Base.Helpers
{
    /// <summary>
    /// This class helps in working with packing/unpacking data.
    /// </summary>
    public static class StiPacker
    {
        #region Consts
        private static string EncryptedId = "8pTP&%#5uK889910";
        #endregion

        #region Properties
        [DefaultValue(true)]
        public static bool AllowPacking { get; set; }
        #endregion

        #region Methods.Bytes
        /// <summary>
        /// Packs the byte array.
        /// </summary>
        public static byte[] Pack(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0 || !AllowPacking) return bytes;

            return AddZipSignature(StiGZipHelper.Pack(bytes));
        }

        /// <summary>
        /// Unpacks byte array.
        /// </summary>
        public static byte[] Unpack(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0 || !IsPacked(bytes)) return bytes;

            try
            {
                Array.Resize(ref bytes, bytes.Length - 3);
                return StiGZipHelper.Unpack(bytes);
            }
            catch (InvalidDataException)//Bad unpacking
            {
                return bytes;
            }
        }

        /// <summary>
        /// Packs and encrypts the byte array.
        /// </summary>
        public static byte[] PackAndEncrypt(byte[] bytes, string encryptedId)
        {
            return StiEncryption.Encrypt(Pack(bytes), encryptedId);
        }

        /// <summary>
        /// Unpacks and uncrypts byte array.
        /// </summary>
        public static byte[] UnpackAndDecrypt(byte[] bytes, string encryptedId)
        {
            return Unpack(StiEncryption.Decrypt(bytes, encryptedId));
        }

        /// <summary>
        /// Packs and encrypts the byte array to the byte64 string.
        /// </summary>
        public static string PackAndEncryptToString(byte[] bytes)
        {
            bytes = Pack(bytes);
            bytes = StiEncryption.Encrypt(bytes, EncryptedId);
            return bytes != null ? Convert.ToBase64String(bytes) : null;
        }

        /// <summary>
        /// Unpacks and uncrypts byte array from base64 string.
        /// </summary>
        public static byte[] UnpackAndDecrypt(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;

            var array = Convert.FromBase64String(str);
            array = StiEncryption.Decrypt(array, EncryptedId);
            return Unpack(array);
        }
        #endregion

        #region Methods.Stream
        /// <summary>
        /// Packs the stream.
        /// </summary>
        public static Stream Pack(Stream stream)
        {
            if (stream == null || stream.Length == 0 || !AllowPacking) return stream;

            return StiGZipHelper.Pack(stream);
        }

        /// <summary>
        /// Unpacks stream.
        /// </summary>
        public static Stream Unpack(Stream stream)
        {
            if (stream == null || stream.Length == 0 || !IsPacked(stream)) return stream;

            try
            {
                return StiGZipHelper.Unpack(stream);
            }
            catch (InvalidDataException)//Bad unpacking
            {
                return stream;
            }
        }
        #endregion

        #region Methods.Bytes.String
        public static string PackToString(byte[] bytes)
        {
            bytes = Pack(bytes);
            return bytes != null ? Convert.ToBase64String(bytes) : null;
        }

        public static byte[] UnpackFromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;

            var array = Convert.FromBase64String(str);
            return Unpack(array);
        }

        /// <summary>
        /// Packs string and convert it to byte array.
        /// </summary>
        public static byte[] PackToBytes(string str, bool allowPacking = true)
        {
            if (string.IsNullOrEmpty(str))return null;

            var bytes = StiGZipHelper.ConvertStringToByteArray(str);

            if (bytes != null && bytes.Length != 0 && AllowPacking && allowPacking)
            {
                bytes = AddZipSignature(StiGZipHelper.Pack(bytes));
            }

            return bytes;
        }

        /// <summary>
        /// Unpacks byte array and convert it to string.
        /// </summary>
        public static string UnpackToString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;

            bytes = Unpack(bytes);

            return StiGZipHelper.ConvertByteArrayToString(bytes);
        }
        #endregion

        #region Methods.Bytes.XElement
        /// <summary>
        /// Packs XElement and convert it to byte array.
        /// </summary>
        public static byte[] PackToBytes(XElement element)
        {
            if (element == null) return null;

            return PackToBytes(element.ToString());
        }


        /// <summary>
        /// Unpacks byte array and convert it to XElement.
        /// </summary>
        public static XElement UnpackToXElement(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;

            var str = UnpackToString(bytes);
            if (str == null) return null;
            return XElement.Parse(str);
        }
        #endregion

        #region Methods.Helper
        private static byte[] AddZipSignature(byte[] bytes)
        {
            Array.Resize(ref bytes, bytes.Length + 3);
            bytes[bytes.Length - 3] = (int)'Z';
            bytes[bytes.Length - 2] = (int)'I';
            bytes[bytes.Length - 1] = (int)'P';
            return bytes;
        }

        /// <summary>
        /// Returns true if the specified stream is packed.
        /// </summary>
        public static bool IsPacked(Stream stream)
        {
            if (stream.Length < 4) return false;

            stream.Seek(stream.Length - 3, SeekOrigin.Current);

            var first = stream.ReadByte();
            var second = stream.ReadByte();
            var third = stream.ReadByte();
            stream.Seek(-stream.Length - 3, SeekOrigin.Current);

            return IsPacked(first, second, third);
        }

        /// <summary>
        /// Returns true if the specified byte array is packed.
        /// </summary>
        public static bool IsPacked(byte[] bytes)
        {
            return bytes.Length > 3 && IsPacked(bytes[bytes.Length - 3], bytes[bytes.Length - 2], bytes[bytes.Length - 1]);
        }

        /// <summary>
        /// Returns true if the specified bytes equal to first bytes in packed data.
        /// </summary>
        private static bool IsPacked(int first, int second, int third)
        {
            return first == 'Z' && second == 'I' && third == 'P';//ZIP
        }
        #endregion

        static StiPacker()
        {
            AllowPacking = true;
        }
    }
}
