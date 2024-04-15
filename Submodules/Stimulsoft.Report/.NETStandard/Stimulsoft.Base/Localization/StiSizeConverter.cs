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
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections;
using System.Reflection;

namespace Stimulsoft.Base.Localization
{
    public class StiSizeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var str = value as string;
            if (str == null)
                return base.ConvertFrom(context, culture, value);

            var str2 = str.Trim();
            if (str2.Length == 0)
                return null;

            if (culture == null)
                culture = CultureInfo.CurrentCulture;
            
            var ch = culture.TextInfo.ListSeparator[0];
            var strArray = str2.Split(ch);
            var numArray = new int[strArray.Length];
            var converter = TypeDescriptor.GetConverter(typeof(int));
            for (var i = 0; i < numArray.Length; i++)
            {
                numArray[i] = (int)converter.ConvertFromString(context, culture, strArray[i]);
            }

            if (numArray.Length != 2)
                throw new Exception("ConvertInvalidPrimitive");

            return new Size(numArray[0], numArray[1]);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            if (value is Size)
            {
                if (destinationType == typeof(string))
                {
                    var size = (Size)value;
                    if (culture == null)
                        culture = CultureInfo.CurrentCulture;

                    var separator = culture.TextInfo.ListSeparator + " ";
                    var converter = TypeDescriptor.GetConverter(typeof(int));
                    var strArray = new[]
                    {
                        converter.ConvertToString(context, culture, size.Width),
                        converter.ConvertToString(context, culture, size.Height)
                    };

                    return string.Join(separator, strArray);
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    var size2 = (Size)value;

                    var constructor = typeof(Size).GetConstructor(new[] { typeof(int), typeof(int) });
                    if (constructor != null)
                        return new InstanceDescriptor(constructor, new object[] { size2.Width, size2.Height });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
                throw new ArgumentNullException("propertyValues");

            var valueWidth = propertyValues["Width"];
            var valueHeight = propertyValues["Height"];

            if (valueWidth == null || valueHeight == null || !(valueWidth is int) || !(valueHeight is int))
                throw new Exception("ConvertInvalidPrimitive");

            return new Size((int)valueWidth, (int)valueHeight);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}