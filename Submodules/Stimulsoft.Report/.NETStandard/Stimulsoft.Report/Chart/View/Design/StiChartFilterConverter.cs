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

using Stimulsoft.Report.Components;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace Stimulsoft.Report.Chart.Design
{
    /// <summary>
    /// Converts StiChartFilter from one data type to another. 
    /// </summary>
    public class StiChartFilterConverter : TypeConverter
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
                if (value is StiChartFilter)
                {
                    var filter = (StiChartFilter)value;

                    var types = new Type[]
                    {
                        typeof(StiFilterItem),
                        typeof(StiFilterDataType),
                        typeof(StiFilterCondition),
                        typeof(string)
                    };

                    var info = typeof(StiChartFilter).GetConstructor(types);
                    if (info != null)
                    {
                        var objs = new object[]    
                        {
                            filter.Item,
                            filter.DataType,
                            filter.Condition,
                            filter.Value
                        };

                        return CreateNewInstanceDescriptor(info, objs);
                    }
                }
            }
            else if (destinationType == typeof(string))
            {
                var filter = value as StiChartFilter;
                if (filter != null)
                {
                    return string.Format(
                        "{0},{1},{2},{3}",
                        filter.Item.ToString(),
                        filter.DataType.ToString(),
                        filter.Condition.ToString(),
                        XmlConvert.EncodeName(filter.Value));
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
                var splits = new char[1] { ',' };
                var words = text.Split(splits);
                var filterCondition = words[2];

                filterCondition = FixOldErrors(filterCondition);

                return new StiChartFilter(
                    (StiFilterItem)Enum.Parse(typeof(StiFilterItem), words[0]),
                    (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), words[1]),
                    (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), filterCondition),
                    XmlConvert.DecodeName(words[3]));

            }
            return base.ConvertFrom(context, culture, value);
        }

        private static string FixOldErrors(string filterCondition)
        {
            if (filterCondition == "GreaterThen") 
                filterCondition = "GreaterThan";

            if (filterCondition == "GreaterThenOrEqualTo") 
                filterCondition = "GreaterThanOrEqualTo";

            if (filterCondition == "LessThen") 
                filterCondition = "LessThan";

            if (filterCondition == "LessThenOrEqualTo") 
                filterCondition = "LessThanOrEqualTo";

            return filterCondition;
        }

        protected object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
    }
}