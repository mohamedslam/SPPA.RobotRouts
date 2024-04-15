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

namespace Stimulsoft.Base.Drawing.Design
{
	/// <summary>
	/// Converts a PointM object from one data type to another.
	/// </summary>
	public class PointMConverter : TypeConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(value, attributes); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
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

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) 
		{
			if (destinationType == typeof(string))
			{
				if (value == null)
				    return "null";

			    var ch = culture == null 
			        ? CultureInfo.CurrentCulture.TextInfo.ListSeparator[0] 
			        : culture.TextInfo.ListSeparator[0];

			    var point = (PointM)value;
			    return $"{point.X.ToString(CultureInfo.InvariantCulture)}{ch}{point.Y.ToString(CultureInfo.InvariantCulture)}";
			}

			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
                var point = (PointM)value;

				var types = new[]
                {	
					typeof(decimal),
					typeof(decimal)
				};

                var info = typeof(PointM).GetConstructor(types);
				if (info != null)
				{
					var objs = new object[]	
                    {	
						point.X,
						point.Y
					};
					
					return CreateNewInstanceDescriptor(info, objs);
				}
					
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true; 

			return base.CanConvertFrom(context, sourceType); 
		} 

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
		    var str = value as string;
            if (str != null)
			{
				str = str.Trim().ToLowerInvariant();

			    var ch = culture == null 
			        ? CultureInfo.CurrentCulture.TextInfo.ListSeparator[0] 
			        : culture.TextInfo.ListSeparator[0];

				var words = str.Split(ch);

                return new PointM(decimal.Parse(words[0]), decimal.Parse(words[1]));
            }

			return base.ConvertFrom(context, culture, value); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
