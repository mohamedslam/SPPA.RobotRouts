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
using System.Reflection;
using System.Xml;

namespace Stimulsoft.Report.Dictionary.Design
{
    /// <summary>
    /// Converts StiDataColumn from one data type to another. 
    /// </summary>
    public class StiDataColumnConverter : TypeConverter
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
				if (value is StiCalcDataColumn)
				{
					var column = (StiCalcDataColumn)value;

					var types = new[]
                    {
						typeof(string),
						typeof(string),
						typeof(Type),
                        typeof(string),
                        typeof(string)
					};

					var info = typeof(StiCalcDataColumn).GetConstructor(types);
					if (info != null)
					{
						var objs = new object[]	
                        {	
							column.Name,
							column.Alias, 
							column.Type,
                            column.Value,
                            column.Key
						};

                        return CreateNewInstanceDescriptor(info, objs);
					}
				}
                else if (value is StiDataTransformationColumn)
                {
                    var column = (StiDataTransformationColumn)value;

                    var types = new[]
                    {
                        typeof(string),
                        typeof(string),
                        typeof(Type),
                        typeof(string),
                        typeof(string),
                        typeof(StiDataTransformationMode)
                    };

                    var info = typeof(StiDataTransformationColumn).GetConstructor(types);
                    if (info != null)
                    {
                        var objs = new object[]
                        {
                            column.Name,
                            column.Alias,
                            column.Type,
                            column.Expression,
                            column.Key,
                            column.Mode
                        };

                        return CreateNewInstanceDescriptor(info, objs);
                    }
                }
                else if (value is StiDataColumn)
				{
					var column = (StiDataColumn)value;

					var types = new[]
                    {
						typeof(string),
						typeof(string),
						typeof(string),
						typeof(Type),
                        typeof(string)
					};

					var info = typeof(StiDataColumn).GetConstructor(types);
					if (info != null)
					{
						var objs = new object[]	
                        {	
							column.NameInSource,
							column.Name,
							column.Alias, 
							column.Type,
                            column.Key
						};
					
						return CreateNewInstanceDescriptor(info, objs);
					}
				}
			}
			else if (destinationType == typeof(string))
			{
				var column = value as StiDataColumn;
				if (column != null)
				{
                    if (column is StiCalcDataColumn)
                    {
                        if (!string.IsNullOrEmpty(column.Key))
                        {
                            var calcColumn = column as StiCalcDataColumn;
                            return string.Format(
                                "{0},{1},{2},{3},{4},{5}",
                                "CALC",
                                XmlConvert.EncodeName(column.Name),
                                XmlConvert.EncodeName(column.Alias),
                                ConvertTypeToString(column.Type),
                                XmlConvert.EncodeName(calcColumn.Value),
                                XmlConvert.EncodeName(column.Key)
                            );
                        }
                        else
                        {
                            var calcColumn = column as StiCalcDataColumn;
                            return string.Format(
                                "{0},{1},{2},{3}",
                                XmlConvert.EncodeName(column.Name),
                                XmlConvert.EncodeName(column.Alias),
                                ConvertTypeToString(column.Type),
                                XmlConvert.EncodeName(calcColumn.Value));
                        }
                    }
                    else if (column is StiDataTransformationColumn)
                    {
                        var transformColumn = column as StiDataTransformationColumn;
                        return string.Format(
                            "{0},{1},{2},{3},{4},{5},{6}",
                            "TRANSFORM",
                            XmlConvert.EncodeName(column.Name),
                            XmlConvert.EncodeName(column.Alias),
                            ConvertTypeToString(column.Type),
                            XmlConvert.EncodeName(transformColumn.Expression),
                            XmlConvert.EncodeName(column.Key),
                            transformColumn.Mode);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(column.Key))
                        {
                            return string.Format(
                                "{0},{1},{2},{3},{4},{5}",
                                "ORIGINAL",
                                XmlConvert.EncodeName(column.NameInSource),
                                XmlConvert.EncodeName(column.Name),
                                XmlConvert.EncodeName(column.Alias),
                                ConvertTypeToString(column.Type),
                                XmlConvert.EncodeName(column.Key));
                        }

                        if (column.Name == column.Alias && column.Name == column.NameInSource)
                        {
                            return string.Format(
                                "{0},{1}",
                                XmlConvert.EncodeName(column.Name),
                                ConvertTypeToString(column.Type));
                        }
                        else if (column.Name == column.NameInSource)
                        {
                            return string.Format(
                                "{0},{1},{2}",
                                XmlConvert.EncodeName(column.Name),
                                XmlConvert.EncodeName(column.Alias),
                                ConvertTypeToString(column.Type));
                        }
                        else
                        {
                            return string.Format(
                                "{0},{1},{2},{3},{4}",
                                "ORIGINAL",
                                XmlConvert.EncodeName(column.NameInSource),
                                XmlConvert.EncodeName(column.Name),
                                XmlConvert.EncodeName(column.Alias),
                                ConvertTypeToString(column.Type));
                        }
                    }
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
				var splits = new[]{','};
				var words = text.Split(splits);

                if (words.Length == 7 || words.Length == 6)
                {
                    if (words[0] == "CALC")
                    {
					    var type = StiTypeFinder.GetType(DecodeTypeName(words[3]));
                        type = CheckType(DecodeTypeName(words[2]), type);
					    return 
						    new StiCalcDataColumn(
						    XmlConvert.DecodeName(words[1]), 
						    XmlConvert.DecodeName(words[2]), 
						    type, 
						    XmlConvert.DecodeName(words[4]),
                            XmlConvert.DecodeName(words[5]));
                    }
                    else if (words[0] == "TRANSFORM")
                    {
                        var type = StiTypeFinder.GetType(DecodeTypeName(words[3]));
                        type = CheckType(DecodeTypeName(words[2]), type);
                        var mode = words.Length == 6
                            ? StiDataTransformationMode.Dimension 
                            : (StiDataTransformationMode)Enum.Parse(typeof(StiDataTransformationMode), XmlConvert.DecodeName(words[6]));

                        return
                            new StiDataTransformationColumn(
                            XmlConvert.DecodeName(words[1]),
                            XmlConvert.DecodeName(words[2]),
                            type,
                            XmlConvert.DecodeName(words[4]),
                            XmlConvert.DecodeName(words[5]),
                            mode);
                    }
                    else
                    {
                        var type = StiTypeFinder.GetType(DecodeTypeName(words[4]));
                        type = CheckType(DecodeTypeName(words[4]), type);

                        return
                            new StiDataColumn(
                            XmlConvert.DecodeName(words[1]),
                            XmlConvert.DecodeName(words[2]),
                            XmlConvert.DecodeName(words[3]),
                            type,
                            XmlConvert.DecodeName(words[5]));
                    }
                }
				else if (words.Length == 5)//Back compatibility
				{
					if (words[0] == "ORIGINAL")
					{
						var type = StiTypeFinder.GetType(DecodeTypeName(words[4]));
                        type = CheckType(DecodeTypeName(words[4]), type);

						return 
							new StiDataColumn(
							XmlConvert.DecodeName(words[1]),
							XmlConvert.DecodeName(words[2]),
							XmlConvert.DecodeName(words[3]),
							type);
					}
					else
					{
						var type = StiTypeFinder.GetType(DecodeTypeName(words[2]));
                        type = CheckType(DecodeTypeName(words[2]), type);

						return 
							new StiCalcDataColumn(
							XmlConvert.DecodeName(words[0]), 
							XmlConvert.DecodeName(words[1]), 
							type, 
							XmlConvert.DecodeName(words[4]));
					}
				}
				else if (words.Length == 4)
				{
					var type = StiTypeFinder.GetType(DecodeTypeName(words[2]));
                    type = CheckType(DecodeTypeName(words[2]), type);
					return 
						new StiCalcDataColumn(
						XmlConvert.DecodeName(words[0]), 
						XmlConvert.DecodeName(words[1]), 
						type, 
						XmlConvert.DecodeName(words[3]));
				}
				else if (words.Length == 3)
				{
					var type = StiTypeFinder.GetType(DecodeTypeName(words[2]));

                    type = CheckType(DecodeTypeName(words[2]), type);

					return 
						new StiDataColumn(
						XmlConvert.DecodeName(words[0]), 
						XmlConvert.DecodeName(words[1]), 
						type);
				}
				else
				{
                    Type type = null;
                    try
                    {
                        type = StiTypeFinder.GetType(DecodeTypeName(words[1]));
                    }
                    catch
                    {
                    }
                    type = CheckType(DecodeTypeName(words[1]), type);

					return new StiDataColumn(XmlConvert.DecodeName(words[0]), XmlConvert.DecodeName(words[0]), type);
				}
			}
			return base.ConvertFrom(context, culture, value); 
		}

        internal static Type CheckType(string typeName, Type type)
        {
            if (StiOptions.Engine.FullTrust && type == null && !string.IsNullOrEmpty(typeName))            
                type = CreateUndefinedType(typeName);            

            if (type == null)
                type = typeof(object);

            return type;
        }

        private static string EncodeTypeName(string typeName)
        {
            return typeName.Replace(",", "_x002c_");
        }

        private static string DecodeTypeName(string typeName)
        {
            return typeName.Replace("_x002c_", ",");
        }

        private static Type CreateUndefinedType(string typeName)
        {
            return new StiUndefinedType(typeName);
        }

        public static string ConvertTypeToString(Type type)
        {
            if (StiOptions.Engine.FullTrust)
                return EncodeTypeName(ConvertTypeToStringFullTrust(type));

            return EncodeTypeName(type.ToString());
        }

        private static string ConvertTypeToStringFullTrust(Type type)
        {
            if (type == typeof(StiUndefinedType))
                return ((StiUndefinedType)type).Type;

            return type.ToString();
        }

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
