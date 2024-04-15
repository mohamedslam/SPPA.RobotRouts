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

using Stimulsoft.Base.Localization;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing;
#endif

namespace Stimulsoft.Base.Drawing.Design
{
    /// <summary>
    /// Converts a Font object from one data type to another.
    /// </summary>
    public class StiExpressionFontConverter : FontConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
           object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null && StiVisualStatePermissionAttribute.IsFromDefaultStateAllowed(context))
                    return Loc.GetReport("FromDefaultState");

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
    }
}
