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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using System.Threading;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
    {
        #region Methods
        public static void Load(string fileName)
        {
            if (!File.Exists(fileName)) return;

            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                Load(stream);
            }
        }

        public static void Load(Stream stream)
        {
            if (stream == null || stream.Length == 0) return;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                var tr = new XmlTextReader(stream);
                tr.DtdProcessing = DtdProcessing.Ignore;

                tr.Read();
                tr.Read();
                tr.Read();

                if (tr.Name == "StiOptionsHelper" || tr.Name == "StiOptions")
                {
                    string propertyStr = null;
                    string classTypeStr = null;
                    var isFirst = false;
                    while (tr.Read())
                    {
                        if (tr.IsStartElement())
                        {
                            if (tr.Depth == 1)
                            {
                                if (isFirst)
                                {
                                    ApplyNewValue(propertyStr, classTypeStr, null);
                                }

                                propertyStr = tr.Name;
                                classTypeStr = tr.GetAttribute("type");
                                isFirst = true;
                            }
                            else
                            {
                                string value = tr.ReadString();
                                ApplyNewValue(propertyStr, classTypeStr, value);
                                isFirst = false;
                            }
                        }

                        tr.Read();
                    }
                }

                tr.Close();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
                stream.Close();
            }
        }

        private static void ApplyNewValue(string propertyStr, string classTypeStr, string valueStr)
        {
            var baseType = StiTypeFinder.GetType($"{classTypeStr}, Stimulsoft.Report, {StiVersion.VersionInfo}");
            if (baseType == null) return;

            var propInfo = baseType.GetProperty(propertyStr);
            var fieldInfo = propInfo == null ? baseType.GetField(propertyStr) : null;

            if (propInfo == null && fieldInfo == null) return;

            object objValue = null;

            if (valueStr != null)
            {
                var propType = propInfo != null ? propInfo.PropertyType : fieldInfo.FieldType;
                if (propType.IsEnum)
                {
                    objValue = Enum.Parse(propType, valueStr);
                }
                else if (propType == typeof(string))
                {
                    objValue = valueStr;
                }
                else if (propType == typeof(double))
                {
                    objValue = double.Parse(valueStr);
                }
                else if (propType == typeof(bool))
                {
                    objValue = bool.Parse(valueStr);
                }
                else if (propType == typeof(int))
                {
                    objValue = int.Parse(valueStr);
                }
                else if (propType == typeof(float))
                {
                    objValue = float.Parse(valueStr);
                }
                else if (propType == typeof(char))
                {
                    objValue = char.Parse(valueStr);
                }
                else if (propType == typeof(Font))
                {
                    var strs = valueStr.Split(',');
                    if (strs.Length == 6)
                    {
                        var size = float.Parse(strs[1]);
                        var style = (FontStyle)Enum.Parse(typeof(FontStyle), strs[2]);
                        var unit = (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), strs[3]);
                        var gdiCharSet = byte.Parse(strs[4]);
                        var gdiVerticalFont = bool.Parse(strs[5]);

                        objValue = new Font(strs[0], size, style, unit, gdiCharSet, gdiVerticalFont);
                    }
                }
                else if (propType == typeof(StiLevel?))
                {
                    objValue = Enum.Parse(typeof(StiLevel), valueStr);
                }
                else if (propType == typeof(Size))
                {
                    var strs = valueStr.Split(',');
                    objValue = new Size(int.Parse(strs[0]), int.Parse(strs[1]));

                }
                else if (propType == typeof(object))
                {
                    var attrs = propInfo.GetCustomAttributes(typeof(StiOptionsUniversalTypeObjectAttribute), true);
                    if (attrs.Length > 0)
                    {
                        var universalAttribute = attrs[0] as StiOptionsUniversalTypeObjectAttribute;

                        if (universalAttribute.UniversalType == StiOptionsUniversalType.Enum)
                            objValue = Enum.Parse(StiTypeFinder.GetType($"{universalAttribute.Type}, {StiVersion.VersionInfo}"), valueStr);
                    }
                }
            }

            if (propInfo != null)
                propInfo.SetValue(null, objValue, null);

            if (fieldInfo != null)
                fieldInfo.SetValue(null, objValue);
        }

        public static void Save(Stream stream)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                using (var tw = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    tw.Formatting = Formatting.Indented;

                    tw.WriteStartDocument(true);
                    tw.WriteStartElement("StiOptions");

                    #region Save
                    var rootType = typeof(StiOptions);
                    var members = rootType.GetMembers(BindingFlags.Public);

                    foreach (var info in members)
                    {
                        if (info.DeclaringType.IsClass)
                        {
                            SaveClass(tw, "Stimulsoft.Report.StiOptions", info.Name);
                        }
                    }
                    #endregion

                    tw.WriteEndElement();
                    tw.Flush();
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private static void SaveClass(XmlTextWriter tw, string parentType, string typeStr)
        {
            var fullTypeStr = $"{parentType}+{typeStr}";
            var str = $"{fullTypeStr}, Stimulsoft.Report, {StiVersion.VersionInfo}";
            var type = StiTypeFinder.GetType(str);
            if (type == null) return;

            foreach (var info in type.GetMembers(BindingFlags.Public).OrderBy(m => m.Name).Where(info => info.DeclaringType.IsClass))
            {
                SaveClass(tw, fullTypeStr, info.Name);
            }

            var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static).Where(m => m is PropertyInfo || m is FieldInfo);
            foreach (var member in members)
            {
                object actualValue = null;
                if (!CheckAttributes(member, ref actualValue)) continue;

                var memberType = member is PropertyInfo
                    ? ((PropertyInfo) member).PropertyType
                    : ((FieldInfo) member).FieldType;

                tw.WriteStartElement(member.Name);
                tw.WriteAttributeString("type", fullTypeStr);
                SaveMember(member, memberType, actualValue, tw);
                tw.WriteEndElement();
            }
        }

        private static void SaveMember(MemberInfo member, Type propertyType, object value, XmlTextWriter tw)
        {
            if (value == null) return;

            if (propertyType.IsEnum)
            {
                tw.WriteElementString("value", ((int)value).ToString());
            }
            else if (propertyType == typeof(string) ||
                propertyType == typeof(double) ||
                propertyType == typeof(bool) ||
                propertyType == typeof(int) ||
                propertyType == typeof(float) ||
                propertyType == typeof(char))
            {
                tw.WriteElementString("value", value.ToString());
            }
            else if (propertyType == typeof(Font))
            {
                var font = (Font)value;
                tw.WriteElementString("value", $"{font.Name},{font.Size},{(int) font.Style},{(int) font.Unit},{font.GdiCharSet},{font.GdiVerticalFont}");
            }
            else if (propertyType == typeof(StiLevel?))
            {
                tw.WriteElementString("value", ((int)((StiLevel)value)).ToString());
            }
            else if (propertyType == typeof(Size))
            {
                var size = (Size)value;
                tw.WriteElementString("value", $"{size.Width},{size.Height}");
            }
            else if (propertyType == typeof(object))
            {
                var attrs = member.GetCustomAttributes(typeof(StiOptionsUniversalTypeObjectAttribute), true);
                if (attrs.Length == 0) return;

                var universalTypeAttr = attrs[0] as StiOptionsUniversalTypeObjectAttribute;
                if (universalTypeAttr.UniversalType == StiOptionsUniversalType.Enum)
                    tw.WriteElementString("value", ((int) value).ToString());
            }
        }

        private static bool CheckAttributes(MemberInfo member, ref object actualValue)
        {
            var index = 0;
            var isBreak = true;
            DefaultValueAttribute defaultValueAttribute = null;
            StiOptionsFontHelperAttribute fontAttr = null;
            var attrs = member.GetCustomAttributes(false);
            while (index < attrs.Length)
            {
                if (attrs[index] is StiSerializableAttribute)
                    isBreak = false;

                else if (attrs[index] is DefaultValueAttribute)
                    defaultValueAttribute = attrs[index] as DefaultValueAttribute;

                else if (attrs[index] is StiOptionsFontHelperAttribute)
                    fontAttr = attrs[index] as StiOptionsFontHelperAttribute;

                index++;
            }
            if (isBreak) return false;

            actualValue = member is PropertyInfo ? ((PropertyInfo)member).GetValue(null, null) : ((FieldInfo)member).GetValue(null);
            if (defaultValueAttribute != null)
            {
                var defaultValue = defaultValueAttribute.Value;
                if (Equals(defaultValue, actualValue)) return false;
            }
            if (fontAttr != null && CheckFont(fontAttr.Index, actualValue as Font)) return false;
            if (actualValue is Size && ((Size)actualValue).IsEmpty) return false;

            return true;
        }

        private static bool CheckFont(int index, Font font)
        {
            if (font == null) return false;

            var actualFont = GetFont(index);
            return actualFont.Name == font.Name &&  actualFont.Size == font.Size && actualFont.Style == font.Style;
        }

        private static Font GetFont(int index)
        {
            switch (index)
            {
                case 0:
                    return new Font("Arial", 7);

                case 1:
                    return new Font("Arial", 10);

                case 2:
                    return new Font("Arial", 8);
            }

            return null;
        }
        #endregion
    }
}