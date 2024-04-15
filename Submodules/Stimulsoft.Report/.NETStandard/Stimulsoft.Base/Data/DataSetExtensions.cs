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
using System.Data;
using System.IO;
using Stimulsoft.Base.Helpers;

namespace Stimulsoft.Base
{
    public static class DataSetExtensions
    {
        #region Fields.Static
        private static string EncryptedId = "8pTP&%#5uKAvvS10";
        #endregion

        #region Methods
        public static byte[] WriteToArray(this DataSet dataSet)
        {
            if (dataSet == null) 
                return null;

            using (var stream = new MemoryStream())
            {
                dataSet.WriteXml(stream, XmlWriteMode.WriteSchema);
                return stream.ToArray();
            }
        }

        public static string WriteToString(this DataSet dataSet)
        {
            if (dataSet == null) 
                return null;

            using (var writer = new StringWriter())
            {
                dataSet.WriteXml(writer, XmlWriteMode.WriteSchema);
                return writer.ToString();
            }
        }

        public static string WriteToEncryptedString(this DataSet dataSet)
        {
            var array = dataSet.WriteToArray();

            array = StiPacker.Pack(array);
            array = StiEncryption.Encrypt(array, EncryptedId);
            
            return array != null ? Convert.ToBase64String(array) : null;
        }

        public static void Read(this DataSet dataSet, byte[] schema, byte[] data = null)
        {
            if (schema != null)
            {
                using (var stream = new MemoryStream(schema))
                {
                    dataSet.ReadXmlSchema(stream);
                }
            }

            if (data != null)
            {
                using (var stream = new MemoryStream(data))
                {
                    dataSet.ReadXml(stream, XmlReadMode.Auto);
                }
            }
        }

        public static void Read(this DataSet dataSet, string schema, string data = null)
        {
            if (schema != null)
            {
                using (var reader = new StringReader(schema))
                {
                    dataSet.ReadXmlSchema(reader);
                }
            }

            if (data != null)
            {
                using (var reader = new StringReader(data))
                {
                    dataSet.ReadXml(reader, XmlReadMode.Auto);
                }
            }
        }

        public static void ReadFromEncryptedString(this DataSet dataSet, string str)
        {
            if (str == null) return;

            var array = Convert.FromBase64String(str);
            array = StiEncryption.Decrypt(array, EncryptedId);
            array = StiPacker.Unpack(array);
            dataSet.Read(null, array);
        }
        #endregion
    }
}
