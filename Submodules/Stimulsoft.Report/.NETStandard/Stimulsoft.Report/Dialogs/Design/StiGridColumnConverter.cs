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

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dialogs.Design
{
	/// <summary>
	/// Converts GridColumn from one data type to another. 
	/// </summary>
	public class StiGridColumnConverter : TypeConverter
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
				var column = (StiGridColumn)value;

				var types = new[]
				{
				    typeof(string),
				    typeof(bool),
				    typeof(int),
				    typeof(HorizontalAlignment),
				    typeof(string),
				    typeof(string)
				};

				var info = typeof(StiGridColumn).GetConstructor(types);
				if (info != null)
				{
					var objs = new object[]	
					{	
						column.DataTextField, 
						column.Visible,
						column.Width,
						column.Alignment,
						column.HeaderText,
						column.NullText
					};
					
					return CreateNewInstanceDescriptor(info, objs);
				}
			}
			else if (destinationType == typeof(string))
			{
				var column = value as StiGridColumn;
				if (column != null)
				{
					return $"{column.DataTextField},{column.Visible},{column.Width},{column.Alignment},{column.HeaderText},{column.NullText}";
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}


		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))return true; 
			return base.CanConvertFrom(context, sourceType); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) return true;
			if (destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType); 
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
		    var text = value as string;
            if (text != null)
			{
				var splits = new[]{ ',' };
				var words = text.Split(splits);
				
				return new StiGridColumn(words[0], words[1] == "True", int.Parse(words[2]), 
					(HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), words[3]), words[4], words[4]);

			}
			return base.ConvertFrom(context, culture, value); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
