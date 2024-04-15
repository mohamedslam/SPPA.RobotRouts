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

using Stimulsoft.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Stimulsoft.Report.Dictionary.Design
{
    /// <summary>
    /// Converts StiDialogInfo from one data type to another. 
    /// </summary>
    public class StiDialogInfoConverter : TypeConverter
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
                var dialogInfo = (StiDialogInfo)value;

                #region StiItemsInitializationType.Columns
                if (dialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns)
                {
                    if (dialogInfo.BindingValue)
                    {
                        var types = new Type[]
                        {
							typeof(StiDateTimeType),
							typeof(string),
                            typeof(bool),
                            typeof(string),
                            typeof(string),
                            typeof(string),
                            typeof(bool),
                            typeof(StiVariable),
                            typeof(string),                            
                            typeof(StiVariableSortDirection),
                            typeof(StiVariableSortField)
                        };

                        var info = typeof(StiDialogInfo).GetConstructor(types);
                        if (info != null)
                        {
                            var keysColumn = dialogInfo.KeysColumn ?? string.Empty;
                            var valuesColumn = dialogInfo.ValuesColumn ?? string.Empty;
                            var statesColumn = dialogInfo.CheckedColumn ?? string.Empty;
                            var bindingValuesColumn = dialogInfo.BindingValuesColumn ?? string.Empty;

                            var objs = new object[]	
                            {	
                                dialogInfo.DateTimeType,
								dialogInfo.Mask,
                                dialogInfo.AllowUserValues,
                                keysColumn,
                                valuesColumn,
                                statesColumn,
                                dialogInfo.BindingValue,
                                dialogInfo.BindingVariable,
                                bindingValuesColumn,                                
                                dialogInfo.SortDirection,
                                dialogInfo.SortField
                            };

                            return CreateNewInstanceDescriptor(info, objs);
                        }
                    }
                    else
                    {
                        var types = new Type[]
                        {
							typeof(StiDateTimeType),
							typeof(string),
                            typeof(bool),
                            typeof(string),
                            typeof(string),
                            typeof(string),
                            typeof(StiVariableSortDirection),
                            typeof(StiVariableSortField)
                        };

                        var info = typeof(StiDialogInfo).GetConstructor(types);
                        if (info != null)
                        {
                            var keysColumn = dialogInfo.KeysColumn ?? string.Empty;
                            var valuesColumn = dialogInfo.ValuesColumn ?? string.Empty;
                            var statesColumn = dialogInfo.CheckedColumn ?? string.Empty;

                            var objs = new object[]	
                            {	
                                dialogInfo.DateTimeType,
								dialogInfo.Mask,
                                dialogInfo.AllowUserValues,
                                keysColumn,
                                valuesColumn,
                                statesColumn,
                                dialogInfo.SortDirection,
                                dialogInfo.SortField
                            };

                            return CreateNewInstanceDescriptor(info, objs);
                        }
                    }                    
                }
                #endregion

                #region StiItemsInitializationType.Items
                else
                {
                    var types = new Type[]
                    {
						typeof(StiDateTimeType),
						typeof(string),
                        typeof(bool),
                        typeof(StiVariableSortDirection),
                        typeof(string[]),
                        typeof(string[]),
                        typeof(bool[])                        
                    };

                    var info = typeof(StiDialogInfo).GetConstructor(types);
                    if (info != null)
                    {
                        var keys = dialogInfo.Keys ?? (new string[0]);
                        var values = dialogInfo.Values ?? (new string[0]);
                        var states = dialogInfo.CheckedStates != null && dialogInfo.CheckedStates.Length > 0 ? dialogInfo.CheckedStates : null;

                        var objs = new object[]	
                        {	
                            dialogInfo.DateTimeType,
							dialogInfo.Mask,
                            dialogInfo.AllowUserValues,
                            dialogInfo.SortDirection,
                            keys,
                            values,
                            states
                        };

                        return CreateNewInstanceDescriptor(info, objs);
                    }
                }
                #endregion
            }
            #endregion

            #region String
            else if (destinationType == typeof(string))
			{
                var info = value as StiDialogInfo;
                if (info != null)
                    return StiObjectStateSaver.WriteObjectStateToString(info);
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
                var info = new StiDialogInfo();
                StiObjectStateSaver.ReadObjectStateFromString(info, value as string);

                return info;
            }

			return base.ConvertFrom(context, culture, value); 
		}

        public static string ConvertDialogInfoToString(StiDialogInfo info)
        {
            if (info == null || info.IsDefault)
            {
                return "null";
            }
            else
            {
                var converter = new StiDialogInfoConverter();
                return XmlConvert.EncodeName(converter.ConvertToInvariantString(info));
            }
        }

        public static StiDialogInfo ConvertFromStringToDialogInfo(string str)
        {
            if (str == "null" || string.IsNullOrWhiteSpace(str))
                return new StiDialogInfo();

            return new StiDialogInfoConverter().
                ConvertFromInvariantString(XmlConvert.DecodeName(str)) as StiDialogInfo;
        }

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
