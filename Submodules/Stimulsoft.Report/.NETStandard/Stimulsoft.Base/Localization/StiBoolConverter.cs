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
using System.Globalization;
using System.ComponentModel;
using Stimulsoft.Base.Design;

namespace Stimulsoft.Base.Localization
{
	/// <summary>
	/// Provides a type converter to convert Boolean objects to and from various other representations.
	/// </summary>
	public class StiBoolConverter : TypeConverter
	{
        #region Fields
        private static StandardValuesCollection values;
        #endregion

        #region Methods
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true; 

			return base.CanConvertFrom(context, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true; 

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var boolValue = value is bool ? (bool)value : false;
				if (boolValue)
				{
					string strTrue = null;
					
					if (StiPropertyGridOptions.Localizable)
						strTrue = StiLocalization.Get("PropertyEnum", "boolTrue");

				    return strTrue ?? "True";
				}
				else
				{
					string strFalse = null;

				    if (StiPropertyGridOptions.Localizable)
						strFalse = StiLocalization.Get("PropertyEnum", "boolFalse");

				    return strFalse ?? "False";
				}
			}
			return base.ConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				var strValue = ((string)value).Trim();

				try
				{
					string strTrue = null;

					if (StiPropertyGridOptions.Localizable)
						strTrue = StiLocalization.Get("PropertyEnum", "boolTrue");

					if (strTrue == null)
						strTrue = "True";

					return strValue == strTrue;
 				}
				catch
				{
					throw new Exception("ConvertInvalidPrimitive"); 
				}
 
			}

			return base.ConvertFrom(context, culture, value); 
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
		    return values ?? (values = new StandardValuesCollection(new object[] { true, false }));
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
            return true;
		}
		
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		#endregion
	} 

}
