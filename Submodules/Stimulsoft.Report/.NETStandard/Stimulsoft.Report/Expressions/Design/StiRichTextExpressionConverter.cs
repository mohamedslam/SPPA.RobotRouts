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
using System.Globalization;

namespace Stimulsoft.Report.Components.Design
{
	public class StiRichTextExpressionConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))return false; 
			return base.CanConvertFrom(context, sourceType); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))return true; 
			return base.CanConvertTo(context, destinationType); 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");
			
			if (destinationType == typeof(string))
			{
				try
				{
					string str = null;
					if (value is StiExpression)str = ((StiExpression)value).Value;
					else str = value as string;

					StiRichText richText = context.Instance as StiRichText;

					using (Controls.StiRichTextBox rich = new Controls.StiRichTextBox(false))
					{
                        string str2 = StiRichText.UnpackRtf(XmlConvert.DecodeName(str));
                        if (str2.TrimStart().ToLowerInvariant().IndexOf("rtf") == -1) return str2;
						rich.Rtf = str2;
						return rich.Text;
					}
				}
				catch
				{
				}
				return string.Empty;
			}

			return base.ConvertTo(context, culture, value, destinationType); 
		}
	}
}
