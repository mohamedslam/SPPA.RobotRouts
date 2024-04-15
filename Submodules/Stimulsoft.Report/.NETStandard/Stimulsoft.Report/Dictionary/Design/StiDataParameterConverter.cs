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

namespace Stimulsoft.Report.Dictionary.Design
{
	/// <summary>
	/// Converts StiDataParameter from one data type to another. 
	/// </summary>
	public class StiDataParameterConverter : TypeConverter
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
				if (value is StiDataParameter)
				{
					var parameter = (StiDataParameter)value;

                    if (StiOptions.Engine.ReconnectDataSourcesIfRequestFromUserVariableChanged)
                    {
                        var types = new Type[]
                        {
						    typeof(string),
						    typeof(string),
						    typeof(int),
						    typeof(int),
                            typeof(string),
					    };

                        var info = typeof(StiDataParameter).GetConstructor(types);
                        if (info != null)
                        {
                            var objs = new object[]	
                            {	
						        parameter.Name,
						        parameter.Value,
						        parameter.Type,
						        parameter.Size,
                                parameter.Key
					        };

                            return CreateNewInstanceDescriptor(info, objs);
                        }
                    }
                    else
                    {
                        var types = new Type[]
                        {
						    typeof(string),
						    typeof(int),
						    typeof(int),
                            typeof(string),
					    };

                        var info = typeof(StiDataParameter).GetConstructor(types);
                        if (info != null)
                        {
                            var objs = new object[]	
                            {	
							    parameter.Name,
							    parameter.Type,
							    parameter.Size,
                                parameter.Key
						    };

                            return CreateNewInstanceDescriptor(info, objs);
                        }
                    }
				}
			}
			else if (destinationType == typeof(string))
			{
				var parameter = value as StiDataParameter;
				if (parameter != null)
				{
                    if (!string.IsNullOrEmpty(parameter.Key))
                    {
                        return string.Format(
                            "{0},{1},{2},{3},{4}",
                            XmlConvert.EncodeName(parameter.Name),
                            XmlConvert.EncodeName(parameter.Value),
                            parameter.Type.ToString(),
                            parameter.Size.ToString(),
                            parameter.Key);
                    }
                    else
                    {
                        return string.Format(
                            "{0},{1},{2},{3}",
                            XmlConvert.EncodeName(parameter.Name),
                            XmlConvert.EncodeName(parameter.Value),
                            parameter.Type.ToString(),
                            parameter.Size.ToString());
                    }
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
			if (value is string)
			{
				string text = value as string;
				char[] splits = new char[1]{','};
				string[] words = text.Split(splits);

                if (words.Length == 4)
                {
                    return new StiDataParameter(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        int.Parse(words[2]),
                        int.Parse(words[3]));
                }
                else
                {
                    return new StiDataParameter(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        int.Parse(words[2]),
                        int.Parse(words[3]),
                        words[4]);
                }
            }
			return base.ConvertFrom(context, culture, value); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
