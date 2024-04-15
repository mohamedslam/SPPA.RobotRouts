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
using System.Drawing;
using System.Globalization;
using System.Xml;

namespace Stimulsoft.Report.Chart.Design
{
    /// <summary>
    /// Converts StiChartCondition from one data type to another. 
    /// </summary>
    public class StiChartConditionConverter : StiChartFilterConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                if (value is StiChartCondition)
                {
                    var condition = (StiChartCondition)value;

                    var types = new Type[]
                    {
                        typeof(Color),
                        typeof(StiFilterItem),
                        typeof(StiFilterDataType),
                        typeof(StiFilterCondition),
                        typeof(string),
                        typeof(StiMarkerType),
                        typeof(float),
                    };

                    var info = typeof(StiChartCondition).GetConstructor(types);
                    if (info != null)
                    {
                        var objs = new object[]    
                        {
                            condition.Color,
                            condition.Item,
                            condition.DataType,
                            condition.Condition,
                            condition.Value,
                            condition.MarkerType,
                            condition.MarkerAngle
                        };

                        return CreateNewInstanceDescriptor(info, objs);
                    }
                }
            }
            else if (destinationType == typeof(string))
            {
                var condition = value as StiChartCondition;
                if (condition != null)
                {
                    return string.Format(
                        "{0},{1},{2},{3},{4},{5},{6}",
                        StiReportObjectStringConverter.ConvertColorToString(condition.Color),
                        condition.Item.ToString(),
                        condition.DataType.ToString(),
                        condition.Condition.ToString(),
                        XmlConvert.EncodeName(condition.Value),
                        condition.MarkerType.ToString(),
                        condition.MarkerAngle.ToString());
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var text = value as string;
                var splits = new char[1] { ',' };
                var words = text.Split(splits);
                var condition = words[3];

                condition = FixOldErrors(condition);

                #region Args(5)
                if (words.Length == 5)
                {
                    return new StiChartCondition(
                        StiReportObjectStringConverter.ConvertStringToColor(words[0]),
                        (StiFilterItem)Enum.Parse(typeof(StiFilterItem), words[1]),
                        (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), words[2]),
                        (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), condition),
                        XmlConvert.DecodeName(words[4]));
                }
                #endregion

                #region Args(7)
                else if (words.Length == 7)
                {
                    return new StiChartCondition(
                        StiReportObjectStringConverter.ConvertStringToColor(words[0]),
                        (StiFilterItem)Enum.Parse(typeof(StiFilterItem), words[1]),
                        (StiFilterDataType)Enum.Parse(typeof(StiFilterDataType), words[2]),
                        (StiFilterCondition)Enum.Parse(typeof(StiFilterCondition), condition),
                        XmlConvert.DecodeName(words[4]),
                        (StiMarkerType)Enum.Parse(typeof(StiMarkerType), words[5]),
                        float.Parse(words[6]));
                }
                #endregion
            }
            return base.ConvertFrom(context, culture, value);
        }

        private static string FixOldErrors(string condition)
        {
            if (condition == "GreaterThen")
                condition = "GreaterThan";

            else if (condition == "GreaterThenOrEqualTo")
                condition = "GreaterThanOrEqualTo";

            else if (condition == "LessThen")
                condition = "LessThan";

            else if (condition == "LessThenOrEqualTo")
                condition = "LessThanOrEqualTo";

            else if (condition == "GreaterThen")
                condition = "GreaterThan";

            return condition;
        }
    }
}