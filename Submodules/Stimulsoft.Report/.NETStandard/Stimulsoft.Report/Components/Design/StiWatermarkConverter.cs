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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using System.Reflection;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components.Design
{
	/// <summary>
	/// Provides a type converter to convert Watermark to and from various other representations.
	/// </summary>
	public class StiWatermarkConverter : StiUniversalConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, 
			object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(value, attributes); 
		} 

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false; 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(string))
			{
				if (value == null || string.IsNullOrEmpty(((StiWatermark)value).Text)) 
					return $"({Loc.Get("PropertyMain", "Watermark")})";
                else
				    return ((StiWatermark)value).Text;
			}

		    if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
				var watermark = (StiWatermark)value;

			    var types = new[]
			    {
			        typeof (StiBrush),
			        typeof (string),
			        typeof (float),
			        typeof (Font),
			        typeof (bool),
			        typeof (bool),
			        typeof (bool),
			        typeof (bool)

			    };

				var info = typeof(StiWatermark).GetConstructor(types);
				if (info != null)
				{
				    var objs = new object[]
				    {
				        watermark.TextBrush,
				        watermark.Text,
				        watermark.Angle,
				        watermark.Font,
				        watermark.ShowBehind,
				        watermark.Enabled,
				        watermark.AspectRatio,
				        watermark.RightToLeft
				    };
					
					return CreateNewInstanceDescriptor(info, objs);
				}
					
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))return true;
			if (destinationType == typeof(string)) return true; 
			return base.CanConvertTo(context, destinationType); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
