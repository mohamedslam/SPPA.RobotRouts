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
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Base.Drawing.Design
{
	/// <summary>
	/// Converts a StiBorder object from one data type to another.
	/// </summary>
	public class StiBorderConverter : TypeConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, 
			object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(value, attributes); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, 
			Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;

			if (destinationType == typeof(string))
				return true;

			return base.CanConvertTo(context, destinationType);
		}
	
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(string) && value is StiBorder)
			{
			    var enc = new StiEnumConverter(typeof(StiBorderSides));					

			    return enc.ConvertTo(context, culture, ((StiBorder)value).Side, typeof(string)) as string;
			}

			if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                #region StiAdvancedBorder
                var advancedBorder = value as StiAdvancedBorder;
                if (advancedBorder != null)
                {
                    var types = new[]
                    {	
						typeof(Color),
						typeof(double),
						typeof(StiPenStyle),

                        typeof(Color),
						typeof(double),
						typeof(StiPenStyle),

                        typeof(Color),
						typeof(double),
						typeof(StiPenStyle),

                        typeof(Color),
						typeof(double),
						typeof(StiPenStyle),

						typeof(bool),
						typeof(double),
						typeof(StiBrush),
                        typeof(bool)
					};

                    var info = typeof(StiAdvancedBorder).GetConstructor(types);
                    if (info != null)
                    {
                        var objs = new object[]	
                        {
                            advancedBorder.TopSide.Color,
                            advancedBorder.TopSide.Size,
                            advancedBorder.TopSide.Style,

                            advancedBorder.BottomSide.Color,
                            advancedBorder.BottomSide.Size,
                            advancedBorder.BottomSide.Style,

                            advancedBorder.LeftSide.Color,
                            advancedBorder.LeftSide.Size,
                            advancedBorder.LeftSide.Style,

                            advancedBorder.RightSide.Color,
                            advancedBorder.RightSide.Size,
                            advancedBorder.RightSide.Style,

                            advancedBorder.DropShadow,
                            advancedBorder.ShadowSize,
                            advancedBorder.ShadowBrush,
                            advancedBorder.Topmost
						};

                        return CreateNewInstanceDescriptor(info, objs);
                    }
                }
                #endregion

                #region StiBorder
                else if (value is StiBorder)
                {
                    var border = (StiBorder)value;                    
                    var types = new[]
                    {	
						typeof(StiBorderSides),
						typeof(Color),
						typeof(double),
						typeof(StiPenStyle),
						typeof(bool),
						typeof(double),
						typeof(StiBrush),
                        typeof(bool)
					};

                    var info = typeof(StiBorder).GetConstructor(types);
                    if (info != null)
                    {
                        var objs = new object[]	
                        {	
							border.Side,
							border.Color, 
							border.Size,
							border.Style,
							border.DropShadow,
							border.ShadowSize,
							border.ShadowBrush,
                            border.Topmost
						};

                        return CreateNewInstanceDescriptor(info, objs);
                    }
                }
                #endregion
            }

			return base.ConvertTo(context, culture, value, destinationType);
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
