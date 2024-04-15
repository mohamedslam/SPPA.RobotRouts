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
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using Stimulsoft.Base.Localization;
using System.Linq;

namespace Stimulsoft.Base.Drawing.Design
{
    /// <summary>
    /// Converts a StiSimpleBorder object from one data type to another.
    /// </summary>
    public class StiSimpleBorderConverter : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context,
            object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
            Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor)) 
                return true;

            if (destinationType == typeof(string)) 
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value == null && 
                StiVisualStatePermissionAttribute.IsFromDefaultStateAllowed(context))
                return Loc.GetReport("FromDefaultState");

            if (destinationType == typeof(string) && value is StiSimpleBorder)
            {
                var enc = new StiEnumConverter(typeof(StiBorderSides));

                return enc.ConvertTo(context, culture, ((StiSimpleBorder)value).Side, typeof(string)) as string;
            }

            if (destinationType == typeof(InstanceDescriptor) && value is StiSimpleBorder)
            {
                var border = (StiSimpleBorder)value;
                var types = new[]
                {
                    typeof(StiBorderSides),
                    typeof(Color),
                    typeof(double),
                    typeof(StiPenStyle)
                };

                var info = typeof(StiSimpleBorder).GetConstructor(types);
                if (info != null)
                {
                    var objs = new object[]
                    {
                        border.Side,
                        border.Color,
                        border.Size,
                        border.Style
                    };

                    return CreateNewInstanceDescriptor(info, objs);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
    }
}
