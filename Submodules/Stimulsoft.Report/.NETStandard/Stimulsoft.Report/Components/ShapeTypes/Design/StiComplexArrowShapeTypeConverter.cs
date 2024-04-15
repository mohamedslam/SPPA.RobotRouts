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

namespace Stimulsoft.Report.Components.ShapeTypes.Design
{
    /// <summary>
    /// Provides a type converter to convert ArrowShapeType objects to and from various other representations.
    /// </summary>
    public class StiArrowShapeTypeConverter : StiShapeTypeServiceConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, 
			Type destinationType)
		{
		    return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
				var shape = (StiArrowShapeType)value;
				var types = new[]
				{	
					typeof(StiShapeDirection),
					typeof(float),
					typeof(float)
				};

				var info = typeof(StiArrowShapeType).GetConstructor(types);
				if (info != null)
				{
					var objs = new object[]	
					{	
						shape.Direction,
						shape.ArrowWidth,
						shape.ArrowHeight
					};
					
					return CreateNewInstanceDescriptor(info, objs);
				}								
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}