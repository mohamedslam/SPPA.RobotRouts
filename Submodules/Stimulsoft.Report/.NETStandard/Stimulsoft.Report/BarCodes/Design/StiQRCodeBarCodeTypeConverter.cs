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

using Stimulsoft.Base.Drawing;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.BarCodes.Design
{
    /// <summary>
    /// Provides a type converter to convert BarCodeType objects to and from various other representations.
    /// </summary>
    public class StiQRCodeBarCodeTypeConverter : StiBarCodeTypeServiceConverter
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
				var barCode = (StiQRCodeBarCodeType)value;				

				var types = new[]
				{	
					typeof(float),
					typeof(StiQRCodeErrorCorrectionLevel),
					typeof(StiQRCodeSize),
                    typeof(Image),
                    typeof(double),
					typeof(StiQRCodeBodyShapeType),
					typeof(StiQRCodeEyeFrameShapeType),
					typeof(StiQRCodeEyeBallShapeType),
					typeof(StiBrush),
					typeof(StiBrush),
					typeof(StiBrush),
					typeof(bool)
				};

                var info = typeof(StiQRCodeBarCodeType).GetConstructor(types);
				if (info != null)
				{
					var objs = new object[]	
					{
						barCode.Module,
						barCode.ErrorCorrectionLevel,
						barCode.MatrixSize,
                        barCode.Image,
                        barCode.ImageMultipleFactor,
						barCode.BodyShape,
						barCode.EyeFrameShape,
						barCode.EyeBallShape,
						barCode.BodyBrush,
						barCode.EyeFrameBrush,
						barCode.EyeBallBrush,
						barCode.ProcessTilde
					};

                    return CreateNewInstanceDescriptor(info, objs);
                }								
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

	}
}
