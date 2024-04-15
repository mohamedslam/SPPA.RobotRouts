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

using Stimulsoft.Base.Localization;
using Stimulsoft.Data.Engine;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Stimulsoft.Data.Design
{
    public class StiDataTopNConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context,
            Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            if (destinationType == typeof(InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var component = context.Instance as IStiDataTopN;

                if (component?.TopN == null || component.TopN.IsDefault)
                    return $"({Loc.Get("FormStyleDesigner", "NotSpecified")})";
                else
                    return component.TopN.ToString();
            }

            if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                var topN = value as StiDataTopN;

                var types = new[]
                {
                    typeof(StiDataTopNMode),
                    typeof(int),
                    typeof(bool),
                    typeof(string),
                    typeof(string)
                };

                var args = new object[]
                {
                    topN.Mode,
                    topN.Count,
                    topN.ShowOthers,
                    topN.OthersText,
                    topN.MeasureField
                };

                var info = typeof(StiDataTopN).GetConstructor(types);
                if (info != null)
                    return CreateNewInstanceDescriptor(info, args);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public object CreateNewInstanceDescriptor(ConstructorInfo info, object[] objs)
        {
            return new InstanceDescriptor(info, objs);
        }
    }
}