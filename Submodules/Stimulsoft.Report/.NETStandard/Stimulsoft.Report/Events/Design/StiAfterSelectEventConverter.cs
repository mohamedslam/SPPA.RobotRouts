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

namespace Stimulsoft.Report.Events.Design
{
	/// <summary>
	/// Provides a type converter to convert StiAfterSelectEvent objects to and from various other representations.
	/// </summary>
	public class StiAfterSelectEventConverter : StiEventConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
			object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is StiEvent)
			{
				return ((StiEvent)value).Script;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string valueStr = value as string;
			if (valueStr != null)return new StiAfterSelectEvent(valueStr);

			return base.ConvertFrom(context, culture, value); 
		}
	}
}
