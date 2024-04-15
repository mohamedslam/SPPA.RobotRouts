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
using System.IO.Compression;

namespace Stimulsoft.Base
{
    public sealed class StiGZipHelper
    {
		#region Methods
        public static byte[] ConvertStringToByteArray(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            {
                writer.Write(str);
                writer.Flush();
                memoryStream.Flush();
                memoryStream.Close();
                return memoryStream.ToArray();
            }
        }


        public static string ConvertByteArrayToString(byte[] bytes)
        {
            if (bytes == null) return null;

            using (var memoryStream = new MemoryStream(bytes))
            using (var reader = new StreamReader(memoryStream))
            {
                string str = reader.ReadToEnd();
                reader.Close();
                memoryStream.Flush();
                memoryStream.Close();
                return str;
            }
        }


        public static string Pack(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;

            return Convert.ToBase64String(Pack(ConvertStringToByteArray(str)));
        }


        public static string Unpack(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;

            return ConvertByteArrayToString(Unpack(Convert.FromBase64String(str)));
        }


        public static byte[] Pack(byte[] bytes)
        {
            if (bytes == null) return null;

            using (var memoryStream = new MemoryStream())
            using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                zipStream.Write(bytes, 0, bytes.Length);
                zipStream.Close();

                return memoryStream.ToArray();
            }
        }

        public static Stream Pack(Stream stream)
        {
            var resultMemoryStream = new MemoryStream();
            using (var zipStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                var data = new byte[2048];
                while (true)
                {
                    var size = zipStream.Read(data, 0, data.Length);
                    if (size > 0)
                        resultMemoryStream.Write(data, 0, size);
                    else
                        break;
                }
                return resultMemoryStream;
            }
        }


        public static byte[] Unpack(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            using (var zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (var resultMemoryStream = new MemoryStream())
            {
                var data = new byte[2048];
                while (true)
                {
                    var size = zipStream.Read(data, 0, data.Length);
                    if (size > 0)
                        resultMemoryStream.Write(data, 0, size);
                    else
                        break;
                }
                return resultMemoryStream.ToArray();
            }
        }

        public static Stream Unpack(Stream stream)
        {
            var resultMemoryStream = new MemoryStream();
            using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))            
            {
                var data = new byte[2048];
                while (true)
                {
                    var size = zipStream.Read(data, 0, data.Length);
                    if (size > 0)
                        resultMemoryStream.Write(data, 0, size);
                    else
                        break;
                }                
            }
            return resultMemoryStream;
        }

        public static void Unpack(Stream stream, Stream resultStream)
        {
            using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                var data = new byte[2048];
                while (true)
                {
                    var size = zipStream.Read(data, 0, data.Length);
                    if (size > 0)
                        resultStream.Write(data, 0, size);
                    else
                        break;
                }
            }
            resultStream.Seek(0, SeekOrigin.Begin);
        }
		#endregion
    }
}
