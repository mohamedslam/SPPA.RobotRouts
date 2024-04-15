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
    /// Converts StiIconSetItem from one data type to another. 
	/// </summary>
    public class StiIconSetItemConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false; 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			#region Convert to InstanceDescriptor
			if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                #region StiIconSetItem
                if (value is StiIconSetItem)
                {
                    var item = (StiIconSetItem)value;

                    var types = new[]
                    {
					    typeof(StiIcon),
					    typeof(StiIconSetOperation),
                        typeof(StiIconSetValueType),
                        typeof(float)
					};

                    var info = typeof(StiIconSetItem).GetConstructor(types);
                    if (info != null)
                    {
                        var objs = new object[]	
						{	
							item.Icon,
							item.Operation,
                            item.ValueType,
                            item.Value,
						};

                        return CreateNewInstanceDescriptor(info, objs);
                    }
                }
                #endregion
            }
			#endregion
			
			#region Convert to string
			else if (destinationType == typeof(string))
            {
                #region StiIconSetItem
                var item = value as StiIconSetItem;
                if (item != null)
				{
                    return string.Format(
                        "{0},{1},{2},{3}",
                        XmlConvert.EncodeName(item.Icon.ToString()),
                        XmlConvert.EncodeName(item.Operation.ToString()),
                        XmlConvert.EncodeName(item.ValueType.ToString()),
                        XmlConvert.EncodeName(item.Value.ToString()));
     
                }
                #endregion
            }
			#endregion

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

                #region StiIconSetItem                
                return
                    new StiIconSetItem(
                        (StiIcon)Enum.Parse(typeof(StiIcon), XmlConvert.DecodeName(words[0])),
                        (StiIconSetOperation)Enum.Parse(typeof(StiIconSetOperation), XmlConvert.DecodeName(words[1])),
                        (StiIconSetValueType)Enum.Parse(typeof(StiIconSetValueType), XmlConvert.DecodeName(words[2])),
                        float.Parse(XmlConvert.DecodeName(words[3])));
                #endregion
  
			}
			return base.ConvertFrom(context, culture, value); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
