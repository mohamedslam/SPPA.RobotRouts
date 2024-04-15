#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

namespace Stimulsoft.Base.Localization
{
    /// <summary>
    /// Provides a type converter to convert Enum objects to and from various other representations.
    /// </summary>
    public class StiExpressionEnumConverter : StiEnumConverter
    {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var expr = StiAppExpressionHelper.GetExpressionFromInstance(context);
				if (!string.IsNullOrWhiteSpace(expr))
					return expr;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				if (StiAppExpressionHelper.IsExpressionSpecified(context.Instance, context.PropertyDescriptor.Name))
				{
					StiAppExpressionHelper.SetExpression(context.Instance, context.PropertyDescriptor.Name, value as string);
					return context.PropertyDescriptor.GetValue(context.Instance);
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return !StiAppExpressionHelper.IsExpressionSpecified(context.Instance, context.PropertyDescriptor.Name);
		}

		public StiExpressionEnumConverter(Type type) : base(type)
		{
		}

		public StiExpressionEnumConverter()
		{
		}
	}
}