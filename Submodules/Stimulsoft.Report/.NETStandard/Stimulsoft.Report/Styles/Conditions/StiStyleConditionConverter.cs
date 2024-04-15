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

namespace Stimulsoft.Report.Design
{
    /// <summary>
    /// Converts StiStyleCondition from one data type to another. 
    /// </summary>
    public class StiStyleConditionConverter : TypeConverter
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
                #region StiStyleCondition
                if (value is StiStyleCondition)
                {
                    var styleCondition = value as StiStyleCondition;
                    var types = new[]
                    {
                        typeof(StiStyleConditionType),
                        typeof(StiStyleConditionOperation),
                        typeof(StiStyleConditionOperation),
                        typeof(StiStyleConditionOperation),
                        typeof(StiStyleConditionOperation),
                        typeof(StiStyleConditionOperation),
                        typeof(StiStyleComponentPlacement),
                        typeof(int),
                        typeof(StiStyleComponentType),
                        typeof(StiStyleLocation),
                        typeof(string)
                    };

                    var info = typeof(StiStyleCondition).GetConstructor(types);
                    if (info != null)
                    {
                        var objs = new object[]
                        {
                            styleCondition.Type,
                            styleCondition.OperationPlacement,
                            styleCondition.OperationPlacementNestedLevel,
                            styleCondition.OperationComponentType,
                            styleCondition.OperationLocation,
                            styleCondition.OperationComponentName,
                            styleCondition.Placement,
                            styleCondition.PlacementNestedLevel,
                            styleCondition.ComponentType,
                            styleCondition.Location,
                            styleCondition.ComponentName
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
                //this branch of code is never called
                //because the StiStyleCondition class does not have the attribute [StiSerializable],
                //and accordingly this converter is not called when the report is saved.
                //but for reasons of backward compatibility, we do not change anything.

                #region StiStyleCondition
                var condition = value as StiStyleCondition;
                if (condition != null)
                {
                    return string.Format(
                        "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        XmlConvert.EncodeName(condition.Type.ToString()),
                        XmlConvert.EncodeName(condition.OperationPlacement.ToString()),
                        XmlConvert.EncodeName(condition.OperationPlacementNestedLevel.ToString()),
                        XmlConvert.EncodeName(condition.OperationComponentType.ToString()),
                        XmlConvert.EncodeName(condition.OperationLocation.ToString()),
                        XmlConvert.EncodeName(condition.OperationComponentName.ToString()),
                        XmlConvert.EncodeName(condition.Placement.ToString()),
                        condition.PlacementNestedLevel,
                        XmlConvert.EncodeName(condition.ComponentType.ToString()),
                        XmlConvert.EncodeName(condition.Location.ToString()),
                        XmlConvert.EncodeName(condition.ComponentName));
                }
                #endregion
            }
            #endregion

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
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
                var text = value as string;
                var splits = new char[] { ',' };
                var words = text.Split(splits);

                var condition = new StiStyleCondition
                {
                    Type = (StiStyleConditionType) Enum.Parse(typeof(StiStyleConditionType), XmlConvert.DecodeName(words[0])),
                    OperationPlacement = (StiStyleConditionOperation) Enum.Parse(typeof(StiStyleConditionOperation), XmlConvert.DecodeName(words[1])),
                    OperationPlacementNestedLevel = (StiStyleConditionOperation) Enum.Parse(typeof(StiStyleConditionOperation), XmlConvert.DecodeName(words[2])),
                    OperationComponentType = (StiStyleConditionOperation) Enum.Parse(typeof(StiStyleConditionOperation), XmlConvert.DecodeName(words[3])),
                    OperationLocation = (StiStyleConditionOperation) Enum.Parse(typeof(StiStyleConditionOperation), XmlConvert.DecodeName(words[4])),
                    OperationComponentName = (StiStyleConditionOperation) Enum.Parse(typeof(StiStyleConditionOperation), XmlConvert.DecodeName(words[5])),
                    Placement = (StiStyleComponentPlacement) Enum.Parse(typeof(StiStyleComponentPlacement), XmlConvert.DecodeName(words[6])),
                    PlacementNestedLevel = int.Parse(words[7]),
                    ComponentType = (StiStyleComponentType) Enum.Parse(typeof(StiStyleComponentType), XmlConvert.DecodeName(words[8])),
                    Location = (StiStyleLocation) Enum.Parse(typeof(StiStyleLocation), XmlConvert.DecodeName(words[9])),
                    ComponentName = XmlConvert.DecodeName(words[10])
                };

                return condition;
            }
            return base.ConvertFrom(context, culture, value);
        }

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
    }
}
