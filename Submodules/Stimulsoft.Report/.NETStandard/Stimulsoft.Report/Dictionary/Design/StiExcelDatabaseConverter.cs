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
using System.Drawing.Text;
using System.Drawing;
using System.Reflection;

namespace Stimulsoft.Report.Dictionary.Design
{
	/// <summary>
    /// Converts ExcelDatabase from one data type to another. 
	/// </summary>
    public class StiExcelDatabaseConverter : TypeConverter
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
			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
                var data = (StiExcelDatabase)value;

                var types = new[]
                {	 
                    typeof(string),
                    typeof(string),
                    typeof(string),
                    typeof(bool)
                };

                var info = typeof(StiExcelDatabase).GetConstructor(types);
				if (info != null)
				{
					var objs = new object[]	
                    {	
                        data.Name,
                        data.PathData,
                        data.Key,
                        data.FirstRowIsHeader
                    };
					
					return CreateNewInstanceDescriptor(info, objs);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}


		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
