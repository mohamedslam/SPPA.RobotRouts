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
using System.Linq;
using System.Reflection;
using Stimulsoft.Base.Localization;
using System.Threading;

namespace Stimulsoft.Base
{
	public class StiUniversalConverter : StiOrderConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, 
			object value, Attribute[] attributes)
		{
			var props = TypeDescriptor.GetProperties(value, attributes);
		    var names = props.Cast<PropertyDescriptor>().Select(p => p.Name).OrderBy(c => c).ToArray();
            return props.Sort(names);
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
			#region Find constructor
			ConstructorInfo currentInfo = null;
			if (value != null)
			{
				var cnInfos = value.GetType().GetConstructors();
								
				foreach (var cnInfo in cnInfos)
				{
					if (cnInfo.GetCustomAttributes(typeof(StiUniversalConstructorAttribute), false).Length > 0)
					{
						currentInfo = cnInfo;
						break;
					}
				}

			    if (currentInfo == null)
				{
					if (destinationType == typeof(string))
					    return string.Empty;

					return base.ConvertTo(context, culture, value, destinationType);
				}
			}
			#endregion

            if (destinationType == typeof(string) && currentInfo != null)
			{
				var attrs = currentInfo.GetCustomAttributes(typeof(StiUniversalConstructorAttribute), false);
				var name = ((StiUniversalConstructorAttribute)attrs[0]).Name;
				var displayName = StiLocalization.Get("PropertyMain", name, false);

				return string.IsNullOrEmpty(displayName) 
				    ? $"({name})" 
				    : $"({displayName})";
			}

            if (destinationType == typeof(InstanceDescriptor) && value != null && currentInfo != null)
			{
				var currentCulture = Thread.CurrentThread.CurrentCulture;

				try
				{
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

					#region Create parameters list
					var pars = currentInfo.GetParameters();
					var types = new Type[pars.Length];
					var objs = new object[pars.Length];

					for (var index = 0; index < pars.Length; index++)
					{
						types[index] = pars[index].ParameterType;

					    var propertyName = pars[index].Name.Substring(0, 1).ToUpperInvariant() + pars[index].Name.Substring(1);

						var prop = value.GetType().GetProperty(propertyName);
						if (prop == null)
						    throw new ArgumentException($"Property '{propertyName}' not present in '{value.GetType()}'");

						objs[index] = prop.GetValue(value, null);
					}
					#endregion
		
					return new InstanceDescriptor(currentInfo, objs);
				}
				finally
				{
                    Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}	
	}
}
