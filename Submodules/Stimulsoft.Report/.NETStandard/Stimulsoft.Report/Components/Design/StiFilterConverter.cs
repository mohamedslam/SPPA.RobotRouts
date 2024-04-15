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
using System.Xml;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Stimulsoft.Report.Components.Design
{
	/// <summary>
	/// Converts StiFilter from one data type to another. 
	/// </summary>
	public class StiFilterConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false; 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
				if (value is StiFilter)
				{
					var filter = (StiFilter)value;
					if (filter.Item == StiFilterItem.Expression)
					{
						var types = new[]
						{
							typeof(string)
						};

						var info = typeof(StiFilter).GetConstructor(types);
						if (info != null)
						{
							var objs = new object[]	
							{	
								filter.Expression
							};
					
							return CreateNewInstanceDescriptor(info, objs);
						}
					}
					else
					{
						var types = new[]
						{
							typeof(string),
							typeof(StiFilterCondition),
							typeof(string),
							typeof(string),
							typeof(StiFilterDataType)
						};

						var info = typeof(StiFilter).GetConstructor(types);
						if (info != null)
						{
							var objs = new object[]	
							{	
								filter.Column,
								filter.Condition,
								filter.Value1,
								filter.Value2,
								filter.DataType
							};
					
							return CreateNewInstanceDescriptor(info, objs);
						}
					}
				}
			}
			else if (destinationType == typeof(string))
			{
				var filter = value as StiFilter;
				if (filter != null)
				{
				    if (filter.Item == StiFilterItem.Expression)
				        return XmlConvert.EncodeName(filter.Expression);

				    return
				        string.Format(
				            "{0},{1},{2},{3},{4}",
				            XmlConvert.EncodeName(filter.Column),
				            filter.Condition.ToString(),
				            XmlConvert.EncodeName(filter.Value1),
				            XmlConvert.EncodeName(filter.Value2),
				            filter.DataType.ToString());
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
				var splits = new []{ ',' };
				var words = text.Split(splits);

			    if (words.Length == 1)
			        return new StiFilter(XmlConvert.DecodeName(words[0]));

			    var filterCondition = words[1];

			    #region Correct Filter condition spelling
			    if (filterCondition == "GreaterThen") filterCondition = "GreaterThan";
			    if (filterCondition == "GreaterThenOrEqualTo") filterCondition = "GreaterThanOrEqualTo";
			    if (filterCondition == "LessThen") filterCondition = "LessThan";
			    if (filterCondition == "LessThenOrEqualTo") filterCondition = "LessThanOrEqualTo";
			    #endregion

			    return new StiFilter(
			        XmlConvert.DecodeName(words[0]),
			        (StiFilterCondition) Enum.Parse(typeof(StiFilterCondition), filterCondition),
			        XmlConvert.DecodeName(words[2]),
			        XmlConvert.DecodeName(words[3]),
			        (StiFilterDataType) Enum.Parse(typeof(StiFilterDataType), words[4]));
			}
			return base.ConvertFrom(context, culture, value); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
