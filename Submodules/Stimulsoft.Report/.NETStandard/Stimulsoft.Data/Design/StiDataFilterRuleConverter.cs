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
using System.Reflection;
using Stimulsoft.Data.Engine;

namespace Stimulsoft.Data.Design
{
	public class StiDataFilterRuleConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false; 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
                var rule = value as StiDataFilterRule;
                if (rule != null)
			    {
                    var types = new[]
			        {
			            typeof(string),
                        typeof(string),
                        typeof(string),
                        typeof(StiDataFilterCondition),
						typeof(StiDataFilterOperation),
						typeof(string),
                        typeof(string),
                        typeof(bool),
                        typeof(bool)
                    };

			        var info = typeof(StiDataFilterRule).GetConstructor(types);
			        if (info != null)
			        {
			            var objs = new object[]
			            {
			                rule.Key,
                            rule.Path,
                            rule.Path2,
                            rule.Condition,
							rule.Operation,
							rule.Value,
                            rule.Value2,
                            rule.IsEnabled,
                            rule.IsExpression
                        };

			            return CreateNewInstanceDescriptor(info, objs);
			        }
			    }
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) 
				return true;

			return base.CanConvertTo(context, destinationType); 
		}

        private object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
	}
}
