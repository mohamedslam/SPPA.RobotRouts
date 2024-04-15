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
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;

namespace Stimulsoft.Report.Components.TextFormats.Design
{
	/// <summary>
	/// Provides a type converter to convert StiPercentageFormatService objects to and from various other representations.
	/// </summary>
	public class StiPercentageFormatConverter : TypeConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, 
			object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(value, attributes); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, 
			Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))return true;
			return base.CanConvertTo(context, destinationType);
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
				var service = (StiPercentageFormatService)value;

                var needState = service.UseLocalSetting && service.State != StiTextFormatState.None;

				var types = !needState 
				    ? new[]
				    {	 
						typeof(int),
						typeof(int),
						typeof(string),
						typeof(int),
						typeof(string),
						typeof(int),
						typeof(string),
						typeof(bool),
						typeof(bool),
						typeof(string)
				    }
					: new[]
				    {	 
						typeof(int),
						typeof(int),
						typeof(int),
						typeof(string),
						typeof(int),
						typeof(string),
						typeof(int),
						typeof(string),
						typeof(bool),
						typeof(bool),
						typeof(string),
						typeof(StiTextFormatState)
				    };

				var info = typeof(StiPercentageFormatService).GetConstructor(types);
				if (info != null)
				{
					var objs = !needState 
					    ? new object[]	
					    {	
							service.PositivePattern,
							service.NegativePattern, 
							service.DecimalSeparator,
							service.DecimalDigits,
							service.GroupSeparator,
							service.GroupSize,
							service.Symbol,
							service.UseGroupSeparator,
							service.UseLocalSetting,
							service.NullDisplay
					    }
					    : new object[]	
					    {
							service.PositivePattern,
							service.NegativePattern,
                            0,
							service.DecimalSeparator,
							service.DecimalDigits,
							service.GroupSeparator,
							service.GroupSize,
							service.Symbol,
							service.UseGroupSeparator,
							service.UseLocalSetting,
							service.NullDisplay,
                            service.State
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
