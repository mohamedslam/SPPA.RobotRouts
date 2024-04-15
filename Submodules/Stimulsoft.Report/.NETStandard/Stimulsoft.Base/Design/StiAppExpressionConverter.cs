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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace Stimulsoft.Base.Design
{
	/// <summary>
	/// Converts StiPropertyExpression from one data type to another. 
	/// </summary>
	public class StiAppExpressionConverter : TypeConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, 
			object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(value, attributes); 
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true; 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
                var prop = (StiAppExpression)value;

				var types = new[]
				{
					typeof(string),
					typeof(string)
				};

                var info = typeof(StiAppExpression).GetConstructor(types);
				if (info != null)
				{
					var objs = new object[]	
					{
					    prop.Name,
					    prop.Expression
					};
					
					return CreateNewInstanceDescriptor(info, objs);
				}
			}

			else if (destinationType == typeof(string))
			{
                var prop = value as StiAppExpression;
				if (prop != null)
				{
					var propName = XmlConvert.EncodeName(prop.Name);
					var propExpression = XmlConvert.EncodeName(prop.Expression);

					return $"{propName},{propExpression}";	
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

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
                return true;

			if (destinationType == typeof(string))
                return true;

			return base.CanConvertTo(context, destinationType); 
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				var text = value as string;
				
				var splits = new[]{ ',' };
				var words = text.Split(splits);
				
				return new StiAppExpression(XmlConvert.DecodeName(words[0]), XmlConvert.DecodeName(words[1]));
			}

			return base.ConvertFrom(context, culture, value); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
