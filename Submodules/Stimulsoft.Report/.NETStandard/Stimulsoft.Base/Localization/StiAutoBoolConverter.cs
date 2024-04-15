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

using Stimulsoft.Base.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Stimulsoft.Base.Localization
{
    /// <summary>
    /// Provides a type converter to convert StiAutoBool objects to and from various other representations.
    /// </summary>
    public class StiAutoBoolConverter : TypeConverter
    {
        #region Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var strValue = value as string;

                if (StiPropertyGridOptions.Localizable)
                {
                    if (Loc.GetEnum("boolFalse") == strValue)
                        return StiAutoBool.False;

                    if (Loc.GetEnum("boolTrue") == strValue)
                        return StiAutoBool.True;

                    if (Loc.GetMain("Auto") == strValue)
                        return StiAutoBool.Auto;
                }

                StiAutoBool result;
                if (Enum.TryParse(strValue, true, out result))
                    return result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {

            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            if ((destinationType == typeof(string)) && value != null)
            {
                var enumValue = (StiAutoBool)value;
                if (StiPropertyGridOptions.Localizable)
                {
                    if (enumValue == StiAutoBool.False)
                        return Loc.GetEnum("boolFalse");

                    if (enumValue == StiAutoBool.True)
                        return Loc.GetEnum("boolTrue");

                    if (enumValue == StiAutoBool.Auto)
                        return Loc.GetMain("Auto");
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.Values == null)
            {
                var values = Enum.GetValues(this.Type);
                var comparer = this.Comparer;
                if (comparer != null)
                    Array.Sort(values, 0, values.Length, comparer);

                this.Values = new StandardValuesCollection(values);
            }
            return this.Values;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return !this.Type.IsDefined(typeof(FlagsAttribute), false);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return Enum.IsDefined(this.Type, value);
        }
        #endregion

        #region Properties
        protected virtual IComparer Comparer => InvariantComparer.Default;

        protected Type EnumType => this.Type;

        protected StandardValuesCollection Values { get; set; }

        protected Type Type { get; set; }
        #endregion

        public StiAutoBoolConverter(Type type)
        {
            this.Type = type;
        }

        public StiAutoBoolConverter()
        {
        }
    }
}