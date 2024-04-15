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
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Stimulsoft.Drawing.Text;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Stimulsoft.Drawing
{
    public class FontConverter : TypeConverter
    {
        public FontConverter()
        {
        }

        ~FontConverter()
        {
            // required to match API definition
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
                return true;

            if (destinationType == typeof(InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            if ((destinationType == typeof(string)) && (value is Font))
            {
                Font font = (Font)value;
                StringBuilder sb = new StringBuilder();
                sb.Append(font.Name).Append(culture.TextInfo.ListSeparator[0] + " ");
                sb.Append(font.Size);

                switch (font.Unit)
                {
                    // MS throws ArgumentException, if unit is set 
                    // to GraphicsUnit.Display
                    // Don't know what to append for GraphicsUnit.Display
                    case System.Drawing.GraphicsUnit.Display:
                        sb.Append("display"); break;

                    case System.Drawing.GraphicsUnit.Document:
                        sb.Append("doc"); break;

                    case System.Drawing.GraphicsUnit.Point:
                        sb.Append("pt"); break;

                    case System.Drawing.GraphicsUnit.Inch:
                        sb.Append("in"); break;

                    case System.Drawing.GraphicsUnit.Millimeter:
                        sb.Append("mm"); break;

                    case System.Drawing.GraphicsUnit.Pixel:
                        sb.Append("px"); break;

                    case System.Drawing.GraphicsUnit.World:
                        sb.Append("world"); break;
                }

                if (font.Style != System.Drawing.FontStyle.Regular)
                    sb.Append(culture.TextInfo.ListSeparator[0] + " style=").Append(font.Style);

                return sb.ToString();
            }

            if ((destinationType == typeof(InstanceDescriptor)) && (value is Font))
            {
                Font font = (Font)value;
                ConstructorInfo met = typeof(Font).GetConstructor(new Type[] { typeof(string), typeof(float), typeof(System.Drawing.FontStyle), typeof(System.Drawing.GraphicsUnit) });
                object[] args = new object[4];
                args[0] = font.Name;
                args[1] = font.Size;
                args[2] = font.Style;
                args[3] = font.Unit;
                return new InstanceDescriptor(met, args);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            System.Drawing.FontStyle f_style;
            float f_size;
            System.Drawing.GraphicsUnit f_unit;
            string font;
            string units;
            string[] fields;

            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            font = (string)value;
            font = font.Trim();

            if (font.Length == 0)
            {
                return null;
            }

            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }

            // Format is FontFamily, size[<units>[, style=1,2,3]]
            // This is a bit tricky since the comma can be used for styles and fields
            fields = font.Split(new char[] { culture.TextInfo.ListSeparator[0] });
            if (fields.Length < 1)
            {
                throw new ArgumentException("Failed to parse font format");
            }

            font = fields[0];
            f_size = 8f;
            units = "px";
            f_unit = System.Drawing.GraphicsUnit.Pixel;
            if (fields.Length > 1)
            {   // We have a size
                for (int i = 0; i < fields[1].Length; i++)
                {
                    if (Char.IsLetter(fields[1][i]))
                    {
                        f_size = (float)TypeDescriptor.GetConverter(typeof(float)).ConvertFromString(context, culture, fields[1].Substring(0, i));
                        units = fields[1].Substring(i);
                        break;
                    }
                }
                if (units == "display")
                {
                    f_unit = System.Drawing.GraphicsUnit.Display;
                }
                else if (units == "doc")
                {
                    f_unit = System.Drawing.GraphicsUnit.Document;
                }
                else if (units == "pt")
                {
                    f_unit = System.Drawing.GraphicsUnit.Point;
                }
                else if (units == "in")
                {
                    f_unit = System.Drawing.GraphicsUnit.Inch;
                }
                else if (units == "mm")
                {
                    f_unit = System.Drawing.GraphicsUnit.Millimeter;
                }
                else if (units == "px")
                {
                    f_unit = System.Drawing.GraphicsUnit.Pixel;
                }
                else if (units == "world")
                {
                    f_unit = System.Drawing.GraphicsUnit.World;
                }
            }

            f_style = System.Drawing.FontStyle.Regular;
            if (fields.Length > 2)
            {   // We have style
                string compare;

                for (int i = 2; i < fields.Length; i++)
                {
                    compare = fields[i];

                    if (compare.IndexOf("Regular") != -1)
                    {
                        f_style |= System.Drawing.FontStyle.Regular;
                    }
                    if (compare.IndexOf("Bold") != -1)
                    {
                        f_style |= System.Drawing.FontStyle.Bold;
                    }
                    if (compare.IndexOf("Italic") != -1)
                    {
                        f_style |= System.Drawing.FontStyle.Italic;
                    }
                    if (compare.IndexOf("Strikeout") != -1)
                    {
                        f_style |= System.Drawing.FontStyle.Strikeout;
                    }
                    if (compare.IndexOf("Underline") != -1)
                    {
                        f_style |= System.Drawing.FontStyle.Underline;
                    }
                }
            }

            return new Font(font, f_size, f_style, f_unit);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            Object value;
            byte charSet = 1;
            float size = 8;
            String name = null;
            bool vertical = false;
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
            FontFamily fontFamily = null;
            System.Drawing.GraphicsUnit unit = System.Drawing.GraphicsUnit.Point;

            if ((value = propertyValues["GdiCharSet"]) != null)
                charSet = (byte)value;

            if ((value = propertyValues["Size"]) != null)
                size = (float)value;

            if ((value = propertyValues["Unit"]) != null)
                unit = (System.Drawing.GraphicsUnit)value;

            if ((value = propertyValues["Name"]) != null)
                name = (String)value;

            if ((value = propertyValues["GdiVerticalFont"]) != null)
                vertical = (bool)value;

            if ((value = propertyValues["Bold"]) != null)
            {
                bool bold = (bool)value;
                if (bold == true)
                    style |= System.Drawing.FontStyle.Bold;
            }

            if ((value = propertyValues["Italic"]) != null)
            {
                bool italic = (bool)value;
                if (italic == true)
                    style |= System.Drawing.FontStyle.Italic;
            }

            if ((value = propertyValues["Strikeout"]) != null)
            {
                bool strike = (bool)value;
                if (strike == true)
                    style |= System.Drawing.FontStyle.Strikeout;
            }

            if ((value = propertyValues["Underline"]) != null)
            {
                bool underline = (bool)value;
                if (underline == true)
                    style |= System.Drawing.FontStyle.Underline;
            }

            /* ?? Should default font be culture dependent ?? */
            if (name == null)
                fontFamily = new FontFamily("Tahoma");
            else
            {
                name = name.ToLower();
                //FontCollection collection = new InstalledFontCollection();
                //FontFamily[] installedFontList = collection.Families;
                //foreach (FontFamily font in installedFontList)
                //{
                //    if (name == font.Name.ToLower())
                //    {
                //        fontFamily = font;
                //        break;
                //    }
                //}

                //// font family not found in installed fonts
                //if (fontFamily == null)
                //{
                //    collection = new PrivateFontCollection();
                //    FontFamily[] privateFontList = collection.Families;
                //    foreach (FontFamily font in privateFontList)
                //    {
                //        if (name == font.Name.ToLower())
                //        {
                //            fontFamily = font;
                //            break;
                //        }
                //    }
                //}

                //// font family not found in private fonts also
                //if (fontFamily == null)
                //    fontFamily = FontFamily.GenericSansSerif;
            }

            return new Font(fontFamily, size, style, unit, charSet, vertical);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties
            (ITypeDescriptorContext context,
            object value, Attribute[] attributes)
        {
            if (value is Font)
                return TypeDescriptor.GetProperties(value, attributes);

            return base.GetProperties(context, value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public sealed class FontNameConverter : TypeConverter
        , IDisposable
        {
            FontFamily[] fonts;

            public FontNameConverter()
            {
                fonts = FontFamily.Families;
            }
            void IDisposable.Dispose()
            {
            }
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                    return value;
                return base.ConvertFrom(context, culture, value);
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                string[] values = new string[fonts.Length];
                for (int i = fonts.Length; i > 0;)
                {
                    i--;
                    values[i] = fonts[i].Name;
                }

                return new TypeConverter.StandardValuesCollection(values);
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                // We allow other values other than those in the font list.
                return false;
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                // Yes, we support picking an element from the list. 
                return true;
            }
        }

        public class FontUnitConverter : EnumConverter
        {
            public FontUnitConverter() : base(typeof(System.Drawing.GraphicsUnit)) { }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return base.GetStandardValues(context);
            }

        }
    }
}
