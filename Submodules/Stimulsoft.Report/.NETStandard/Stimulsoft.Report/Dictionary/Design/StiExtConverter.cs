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

using Stimulsoft.Base.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Stimulsoft.Report.Dictionary.Design
{
    /// <summary>
    /// Converts StiFile from one data type to another. 
    /// </summary>
    public class StiExtConverter : TypeConverter
    {
        #region Methods
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;

            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        protected object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
        #endregion

        #region Methods.Helpers
        protected string Encode(params string[] args)
        {
            var sb = new StringBuilder();

            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg)) continue;

                sb = sb.Length == 0 ? sb.Append(arg) : sb.AppendFormat(", {0}", arg);
            }

            return sb.ToString();
        }

        protected string Encode(string key, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : $"{key}={XmlConvert.EncodeName(value)}";
        }

        protected string EncodeBool(string key, bool value, bool defaultValue)
        {
            return value == defaultValue ? null : $"{key}={(value ? "True" : "False")}";
        }

        protected string EncodeEnum(string key, Enum value, Enum defaultValue)
        {
            return Equals(value, defaultValue) ? null : $"{key}={value}";
        }

        protected string EncodeBytes(string key, byte[] value)
        {
            var str = StiPacker.PackAndEncryptToString(value);
            return string.IsNullOrWhiteSpace(str) ? null : $"{key}={XmlConvert.EncodeName(str)}";
        }

        protected List<dynamic> DecodeElements(string value)
        {
            if (value == null)
                return null;

            var words = value.Split(',');
            var list = new List<dynamic>();

            foreach (var word in words)
            {
                var strs = word.Split('=');
                if (strs.Length != 2) continue;

                list.Add(new
                {
                    Key = strs[0].Trim().ToLowerInvariant(),
                    //Value = XmlConvert.DecodeName(strs[1])
                    Value = Report.Helpers.StiXmlDecodeFastHelper.XmlDecodeFast(strs[1])
                });
            }
            return list;
        }

        protected byte[] DecodeBytes(string value)
        {
            if (value == null)
                return null;

            //value = XmlConvert.DecodeName(value);
            value = Report.Helpers.StiXmlDecodeFastHelper.XmlDecodeFast(value);
            return StiPacker.UnpackAndDecrypt(value);
        }

        protected TEnum DecodeEnum<TEnum>(string value, TEnum defaultValue) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            TEnum result;
            return Enum.TryParse(value, true, out result) ? result : defaultValue;
        }
        #endregion
    }
}
