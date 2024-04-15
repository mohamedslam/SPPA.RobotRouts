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
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Stimulsoft.Report.BarCodes.Design
{
    /// <summary>
    /// Provides a type converter to convert BarCodeType objects to and from various other representations.
    /// </summary>
    public class StiPdf417BarCodeTypeConverter : StiBarCodeTypeServiceConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, 
			object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(value, attributes); 
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

		public override bool CanConvertTo(ITypeDescriptorContext context, 
			Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
				var barCode = (StiPdf417BarCodeType)value;				

				var types = new[]
				{	
					typeof(float),
					typeof(StiPdf417EncodingMode),
					typeof(StiPdf417ErrorsCorrectionLevel),
					typeof(int),
					typeof(int),
					typeof(bool),
					typeof(bool),
					typeof(float),
					typeof(int),
					typeof(bool)
				};

				var info = typeof(StiPdf417BarCodeType).GetConstructor(types);
				if (info != null)
				{
					var objs = new object[]
					{
						barCode.Module,
						barCode.EncodingMode,
						barCode.ErrorsCorrectionLevel,
						barCode.DataColumns,
						barCode.DataRows,
						barCode.AutoDataColumns,
						barCode.AutoDataRows,
						barCode.AspectRatio,
						barCode.RatioY,
						barCode.ProcessTilde
					};

                    return CreateNewInstanceDescriptor(info, objs);
                }								
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

	}
}
