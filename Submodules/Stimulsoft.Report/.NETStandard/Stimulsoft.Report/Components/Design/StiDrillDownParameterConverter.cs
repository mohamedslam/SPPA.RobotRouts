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
using System.Globalization;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Components.Design
{
	/// <summary>
	/// Converts StiDrillDownParameter from one data type to another. 
	/// </summary>
    public class StiDrillDownParameterConverter : StiUniversalConverter
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
            if (destinationType == typeof(string))
            {
                var parameter = value as StiDrillDownParameter;
                if (parameter != null)
                {
                    if (!string.IsNullOrEmpty(parameter.Name) && (parameter.Expression == null || string.IsNullOrEmpty(parameter.Expression.Value)))
                        return parameter.Name;

                    if (string.IsNullOrEmpty(parameter.Name) && parameter.Expression != null && !string.IsNullOrEmpty(parameter.Expression.Value))
                        return parameter.Expression.Value;

                    if (string.IsNullOrEmpty(parameter.Name) && (parameter.Expression == null || string.IsNullOrEmpty(parameter.Expression.Value)))
                        return "{}";

                    return $"{parameter.Name},{parameter.Expression.Value}";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }
    }
}
