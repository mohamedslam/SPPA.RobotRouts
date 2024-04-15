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
using Stimulsoft.Base;

namespace Stimulsoft.Report.Dictionary.Design
{
    /// <summary>
    /// Converts StiVariable from one data type to another. 
    /// </summary>
    public class StiVariableConverter : TypeConverter
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
            #region InstanceDescriptor
            if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                var variable = (StiVariable)value;

                if (variable.IsCategory && !variable.RequestFromUser)
                {
                    var types = new[]
                    {
                        typeof(string)
                    };

                    var info = typeof(StiVariable).GetConstructor(types);
                    if (info == null)
                        return base.ConvertTo(context, culture, value, destinationType);

                    var objs = new object[]
                    {
                        variable.Category
                    };

                    return CreateNewInstanceDescriptor(info, objs);
                }
                else
                {
                    var types = new[]
                    {
                        typeof(string),
                        typeof(string),
                        typeof(string),
                        typeof(string),
                        typeof(Type),
                        typeof(string),
                        typeof(bool),
                        typeof(StiVariableInitBy),
                        typeof(bool),
                        typeof(StiDialogInfo),
                        typeof(string),
                        typeof(bool),
                        typeof(StiSelectionMode)
                    };

                    var info = typeof(StiVariable).GetConstructor(types);
                    if (info == null)
                        return base.ConvertTo(context, culture, value, destinationType);

                    var objs = new object[]
                    {
                        variable.Category,
                        variable.Name,
                        variable.Alias,
                        variable.Description,
                        variable.Type,
                        variable.Value,
                        variable.ReadOnly,
                        variable.InitBy,
                        variable.RequestFromUser,
                        (variable.DialogInfo == null || variable.DialogInfo.IsDefault) ? new StiDialogInfo() : variable.DialogInfo,
                        variable.Key,
                        variable.AllowUseAsSqlParameter,
                        variable.Selection
                    };

                    return CreateNewInstanceDescriptor(info, objs);
                }
            }
            #endregion

            #region String
            if (destinationType == typeof(string))
            {
                var variable = value as StiVariable;
                if (variable != null)
                {
                    var variableValue = XmlConvert.EncodeName(variable.Value);
                    var variableCategory = XmlConvert.EncodeName(variable.Category);
                    var variableName = XmlConvert.EncodeName(variable.Name);
                    var variableAlias = XmlConvert.EncodeName(variable.Alias);
                    var variableDescription = XmlConvert.EncodeName(variable.Description);
                    var variableType = variable.Type != null ? variable.Type.ToString() : string.Empty;

#if STIDRAWING
                    variableType = variableType.Replace("Stimulsoft.Drawing", "System.Drawing");
#endif

                    #region Category
                    if (((variable.Name == null && variable.Alias == null) || 
                        (variable.Name == "" && variable.Alias == "")) &&
                        !variable.RequestFromUser)
                    {
                        return variable.Inherited ?
                            string.Format("{0},{1}", variable.Category, variable.Inherited) :
                            variable.Category;
                    }
                    #endregion

                    #region Arguments == 10, [Category, Name, Alias, Description, Type, Value, ReadOnly, InitBy (As Bool), RequestFromUser, Inherited]
                    if ((variable.DialogInfo == null || variable.DialogInfo.IsDefault) && string.IsNullOrWhiteSpace(variable.Key) && !variable.AllowUseAsSqlParameter
                        && variable.Selection == StiSelectionMode.FromVariable)
                    {
                        return string.Format(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                            variableCategory,
                            variableName,
                            variableAlias,
                            variableDescription,
                            variableType,
                            variableValue,
                            variable.ReadOnly,
                            variable.InitBy == StiVariableInitBy.Expression,
                            variable.RequestFromUser,
                            variable.Inherited);
                    }
                    #endregion

                    #region Arguments == 11, [Category, Name, Alias, Description, Type, Value, ReadOnly, InitBy (As Bool), RequestFromUser, Inherited, DialogInfo]
                    if (string.IsNullOrWhiteSpace(variable.Key) && !variable.AllowUseAsSqlParameter && variable.Selection == StiSelectionMode.FromVariable)
                    {
                        return string.Format(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                            variableCategory,
                            variableName,
                            variableAlias,
                            variableDescription,
                            variableType,
                            variableValue,
                            variable.ReadOnly,
                            variable.InitBy == StiVariableInitBy.Expression,
                            variable.RequestFromUser,
                            variable.Inherited,
                            (variable.DialogInfo != null && !variable.DialogInfo.IsDefault)
                                ? StiDialogInfoConverter.ConvertDialogInfoToString(variable.DialogInfo)
                                : string.Empty);
                    }
                    #endregion

                    #region Arguments == 12, [Category, Name, Alias, Description, Type, Value, ReadOnly, InitBy (As Bool), RequestFromUser, Inherited, DialogInfo, Key]
                    if (!variable.AllowUseAsSqlParameter && variable.Selection == StiSelectionMode.FromVariable)
                    {
                        return string.Format(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                            variableCategory,
                            variableName,
                            variableAlias,
                            variableDescription,
                            variableType,
                            variableValue,
                            variable.ReadOnly,
                            variable.InitBy == StiVariableInitBy.Expression,
                            variable.RequestFromUser,
                            variable.Inherited,
                            (variable.DialogInfo != null && !variable.DialogInfo.IsDefault)
                                ? StiDialogInfoConverter.ConvertDialogInfoToString(variable.DialogInfo)
                                : string.Empty,
                            !string.IsNullOrEmpty(variable.Key) ? XmlConvert.EncodeName(variable.Key) : string.Empty);
                    }
                    #endregion

                    if (variable.Selection == StiSelectionMode.FromVariable)
                    {
                        #region Arguments == 13, [Category, Name, Alias, Description, Type, Value, ReadOnly, InitBy (As Bool), RequestFromUser, Inherited, DialogInfo, Key, AllowUseAsSqlParameter]
                        return string.Format(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                            variableCategory,
                            variableName,
                            variableAlias,
                            variableDescription,
                            variableType,
                            variableValue,
                            variable.ReadOnly,
                            variable.InitBy == StiVariableInitBy.Expression,
                            variable.RequestFromUser,
                            variable.Inherited,
                            (variable.DialogInfo != null && !variable.DialogInfo.IsDefault) ? StiDialogInfoConverter.ConvertDialogInfoToString(variable.DialogInfo) : string.Empty,
                            !string.IsNullOrEmpty(variable.Key) ? XmlConvert.EncodeName(variable.Key) : string.Empty,
                            variable.AllowUseAsSqlParameter);
                        #endregion
                    }
                    else
                    {
                        #region Arguments == 14, [Category, Name, Alias, Description, Type, Value, ReadOnly, InitBy (As Bool), RequestFromUser, Inherited, DialogInfo, Key, AllowUseAsSqlParameter, SelectionMode]
                        return string.Format(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
                            variableCategory,
                            variableName,
                            variableAlias,
                            variableDescription,
                            variableType,
                            variableValue,
                            variable.ReadOnly,
                            variable.InitBy == StiVariableInitBy.Expression,
                            variable.RequestFromUser,
                            variable.Inherited,
                            (variable.DialogInfo != null && !variable.DialogInfo.IsDefault) ? StiDialogInfoConverter.ConvertDialogInfoToString(variable.DialogInfo) : string.Empty,
                            !string.IsNullOrEmpty(variable.Key) ? XmlConvert.EncodeName(variable.Key) : string.Empty,
                            variable.AllowUseAsSqlParameter,
                            variable.Selection);
                        #endregion
                    }
                }
            }
            #endregion

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

                var words = text.Split(',');

                #region Arguments == 1, Category
                if (words.Length == 1)
                    return new StiVariable(words[0]);
                #endregion

                #region Arguments == 2, Inherited category
                if (words.Length == 2)
                    return new StiVariable(words[0]) { Inherited = true };
                #endregion

                #region Arguments == 5, Alias equal name, [Category, Name, Type, Value, ReadOnly]
                if (words.Length == 5)
                {
                    StiVariableInitBy initBy;
                    var val = XmlConvert.DecodeName(words[3]);
                    var type = ParseType(words[2]);

                    try
                    {
                        StiVariable.GetValue(val, type);
                        initBy = StiVariableInitBy.Value;
                    }
                    catch
                    {
                        initBy = StiVariableInitBy.Expression;
                    }

                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[1]),
                        type,
                        val,
                        words[4] == "True",
                        initBy);
                }
                #endregion

                #region Arguments == 6, Alias not equal name, [Category, Name, Alias, Type, Value, ReadOnly]
                if (words.Length == 6)
                {
                    StiVariableInitBy initBy;
                    var val = XmlConvert.DecodeName(words[4]);
                    var type = ParseType(words[3]);

                    try
                    {
                        StiVariable.GetValue(val, type);
                        initBy = StiVariableInitBy.Value;
                    }
                    catch
                    {
                        initBy = StiVariableInitBy.Expression;
                    }

                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        type,
                        val,
                        words[5] == "True",
                        initBy);
                }
                #endregion

                #region Arguments == 7, [Category, Name, Alias, Type, Value, ReadOnly, Function]
                if (words.Length == 7)
                {
                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        ParseType(words[3]),
                        XmlConvert.DecodeName(words[4]),
                        words[5] == "True",
                        words[6] == "True" ? StiVariableInitBy.Expression : StiVariableInitBy.Value);
                }
                #endregion

                #region Arguments == 8, [Category, Name, Alias, Type, Value, ReadOnly, Function, Inherited]
                if (words.Length == 8)
                {
                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        ParseType(words[3]),
                        XmlConvert.DecodeName(words[4]),
                        words[5] == "True",
                        words[6] == "True" ? StiVariableInitBy.Expression : StiVariableInitBy.Value)
                    {
                        Inherited = words[7] == "True"
                    };
                }
                #endregion

                #region Arguments == 9, [Category, Name, Alias, Description, Type, Value, ReadOnly, Function, RequestFromUser]
                if (words.Length == 9)
                {
                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        XmlConvert.DecodeName(words[3]),
                        ParseType(words[4]),
                        XmlConvert.DecodeName(words[5]),
                        words[6] == "True",
                        words[7] == "True" ? StiVariableInitBy.Expression : StiVariableInitBy.Value,
                        words[8] == "True");
                }
                #endregion

                #region Arguments == 10, [Category, Name, Alias, Description, Type, Value, ReadOnly, Function, RequestFromUser, Inherited]
                if (words.Length == 10)
                {
                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        XmlConvert.DecodeName(words[3]),
                        ParseType(words[4]),
                        XmlConvert.DecodeName(words[5]),
                        words[6] == "True",
                        words[7] == "True" ? StiVariableInitBy.Expression : StiVariableInitBy.Value,
                        words[8] == "True")
                    {
                        Inherited = words[9] == "True"
                    };
                }
                #endregion

                #region Arguments == 11, [Category, Name, Alias, Description, Type, Value, ReadOnly, Function, RequestFromUser, Inherited, DialogInfo]
                if (words.Length == 11)
                {
                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        XmlConvert.DecodeName(words[3]),
                        ParseType(words[4]),
                        XmlConvert.DecodeName(words[5]),
                        words[6] == "True",
                        words[7] == "True" ? StiVariableInitBy.Expression : StiVariableInitBy.Value,
                        words[8] == "True",
                        StiDialogInfoConverter.ConvertFromStringToDialogInfo(words[10]))
                    {
                        Inherited = words[9] == "True"
                    };
                }
                #endregion

                #region Arguments == 12, [Category, Name, Alias, Description, Type, Value, ReadOnly, Function, RequestFromUser, Inherited, DialogInfo, Key]
                if (words.Length == 12)
                {
                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        XmlConvert.DecodeName(words[3]),
                        ParseType(words[4]),
                        XmlConvert.DecodeName(words[5]),
                        words[6] == "True",
                        words[7] == "True" ? StiVariableInitBy.Expression : StiVariableInitBy.Value,
                        words[8] == "True",
                        StiDialogInfoConverter.ConvertFromStringToDialogInfo(words[10]),
                        string.IsNullOrWhiteSpace(words[11]) ? null : XmlConvert.DecodeName(words[11]))
                    {
                        Inherited = words[9] == "True"
                    };
                }
                #endregion

                #region Arguments == 13, [Category, Name, Alias, Description, Type, Value, ReadOnly, Function, RequestFromUser, Inherited, DialogInfo, Key, AllowUseAsSqlParameter]
                if (words.Length == 13)
                {
                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        XmlConvert.DecodeName(words[3]),
                        ParseType(words[4]),
                        XmlConvert.DecodeName(words[5]),
                        words[6] == "True",
                        words[7] == "True" ? StiVariableInitBy.Expression : StiVariableInitBy.Value,
                        words[8] == "True",
                        StiDialogInfoConverter.ConvertFromStringToDialogInfo(words[10]),
                        string.IsNullOrWhiteSpace(words[11]) ? null : XmlConvert.DecodeName(words[11]),
                        words[12] == "True")
                    {
                        Inherited = words[9] == "True"
                    };
                }
                #endregion

                #region Arguments == 14, [Category, Name, Alias, Description, Type, Value, ReadOnly, Function, RequestFromUser, Inherited, DialogInfo, Key, AllowUseAsSqlParameter, SelectionMode]
                if (words.Length == 14)
                {
                    return new StiVariable(
                        XmlConvert.DecodeName(words[0]),
                        XmlConvert.DecodeName(words[1]),
                        XmlConvert.DecodeName(words[2]),
                        XmlConvert.DecodeName(words[3]),
                        ParseType(words[4]),
                        XmlConvert.DecodeName(words[5]),
                        words[6] == "True",
                        words[7] == "True" ? StiVariableInitBy.Expression : StiVariableInitBy.Value,
                        words[8] == "True",
                        StiDialogInfoConverter.ConvertFromStringToDialogInfo(words[10]),
                        string.IsNullOrWhiteSpace(words[11]) ? null : XmlConvert.DecodeName(words[11]),
                        words[12] == "True",
                        (StiSelectionMode)Enum.Parse(typeof(StiSelectionMode), words[13]))
                    {
                        Inherited = words[9] == "True"
                    };
                }
                #endregion
            }
            return base.ConvertFrom(context, culture, value);
        }

        private Type ParseType(string str)
        {
            return (string.IsNullOrWhiteSpace(str) || str == "null") ? null : StiTypeFinder.GetType(str);
        }

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
    }
}
