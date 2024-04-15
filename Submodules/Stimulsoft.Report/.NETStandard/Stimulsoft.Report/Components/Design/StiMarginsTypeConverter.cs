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
using System.Reflection;
using System.ComponentModel.Design.Serialization;

namespace Stimulsoft.Report.Components.Design
{
	/// <summary>
	/// Provides a type converter to convert Margins to and from various other representations.
	/// </summary>
	public class StiMarginsTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))return true; 
			return base.CanConvertFrom(context, sourceType); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))return true; 
			return base.CanConvertTo(context, destinationType); 
		} 

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				var text = ((string) value).Trim();

			    var ch = culture == null 
			        ? CultureInfo.CurrentCulture.TextInfo.ListSeparator[0] 
			        : culture.TextInfo.ListSeparator[0];

                if (text.IndexOf(ch) != -1)
				{
					var strs = text.Split(ch);

					return new StiMargins(
						double.Parse(strs[0]),
						double.Parse(strs[1]),
						double.Parse(strs[2]),
						double.Parse(strs[3]));
				}
				else
				{
					var strs = text.Split(',');

                    if (strs.Length == 1)
                        return new StiMargins(double.Parse(strs[0]));
                    else
                        return new StiMargins(
						    double.Parse(strs[0]),
						    double.Parse(strs[1]),
						    double.Parse(strs[2]),
						    double.Parse(strs[3]));
				}
            }
			return base.ConvertFrom(context, culture, value); 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{			
			if (destinationType == null)
			    throw new ArgumentNullException(nameof(destinationType));

			var margins = (StiMargins)value;
			
			if (destinationType == typeof(string))
			{
			    var ch = culture == null 
			        ? CultureInfo.CurrentCulture.TextInfo.ListSeparator[0] 
			        : culture.TextInfo.ListSeparator[0];

			    return $"{margins.Left}{ch}{margins.Right}{ch}{margins.Top}{ch}{margins.Bottom}";
			}

			if (destinationType == typeof(InstanceDescriptor))
			{
				var types = new[]
                {
					typeof(double),
					typeof(double),
					typeof(double),
					typeof(double)
                };
				
				var info = typeof(StiMargins).GetConstructor(types);
				if (info != null)
				{
					object[] objs = 
                    {	
						margins.Left,
						margins.Right, 
						margins.Top,
						margins.Bottom
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

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
	}
}
