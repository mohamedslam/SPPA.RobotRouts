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
    public class StiPointFConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType));
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
            var numArray = new float[strArray.Length];
            var converter = TypeDescriptor.GetConverter(typeof(float));

            for (var i = 0; i < numArray.Length; i++)
            {
                numArray[i] = (float)converter.ConvertFromString(context, culture, strArray[i]);
            }

            if (numArray.Length != 2)
                throw new Exception("ConvertInvalidPrimitive");

            return new PointF(numArray[0], numArray[1]);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            if (value is PointF)
            {
                if (destinationType == typeof(string))
                {
                    var point = (PointF)value;
                    if (culture == null)
                        culture = CultureInfo.CurrentCulture;
                    
                    var separator = culture.TextInfo.ListSeparator + " ";
                    var converter = TypeDescriptor.GetConverter(typeof(float));
                    var strArray = new[]
                    {
                        converter.ConvertToString(context, culture, point.X),
                        converter.ConvertToString(context, culture, point.Y)
                    };

                    return string.Join(separator, strArray);
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    var point2 = (PointF)value;

                    var constructor = typeof(PointF).GetConstructor(new[] { typeof(float), typeof(float) });
                    if (constructor != null)
                        return new InstanceDescriptor(constructor, new object[] { point2.X, point2.Y });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
                throw new ArgumentNullException("propertyValues");

            var valueX = propertyValues["X"];
            var valueY = propertyValues["Y"];

            if (valueX == null || valueY == null || !(valueX is float) || !(valueY is float))
                throw new Exception("ConvertInvalidPrimitive");

            return new PointF((float)valueX, (float)valueY);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(PointF), attributes).Sort(new[] { "X", "Y" });
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

    }
}