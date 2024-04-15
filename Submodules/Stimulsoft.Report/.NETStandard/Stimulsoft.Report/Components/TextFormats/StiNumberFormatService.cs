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

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.TextFormats.Design;
using Stimulsoft.Report.Helpers;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Describes a service for the text formatting as number values.
    /// </summary>
    [TypeConverter(typeof(StiNumberFormatConverter))]
    [StiServiceBitmap(typeof(StiNumberFormatService), "Stimulsoft.Report.Images.Formats.Number.png")]
    [StiFormatEditor("Stimulsoft.Report.Components.TextFormats.Design.StiNumberEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfFormatEditor("Stimulsoft.Report.WpfDesign.StiNumberEditor, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiNumberFormatService : StiFormatService
    {
        #region bits
        private class bitsNumberFormatService : ICloneable
        {
            #region ICloneable
            public object Clone()
            {
                return this.MemberwiseClone();
            }
            #endregion

            #region Fields
            public int negativePattern;
            public string decimalSeparator;
            public int decimalDigits;
            public string groupSeparator;
            public int groupSize;
            public bool useGroupSeparator;
            public bool useLocalSetting;
            public string nullDisplay;
            public StiTextFormatState states;
            #endregion

            public bitsNumberFormatService(int negativePattern, string decimalSeparator, int decimalDigits, string groupSeparator,
                int groupSize, bool useGroupSeparator, bool useLocalSetting, string nullDisplay, StiTextFormatState states)
            {
                this.negativePattern = negativePattern;
                this.decimalSeparator = decimalSeparator;
                this.decimalDigits = decimalDigits;
                this.groupSeparator = groupSeparator;
                this.groupSize = groupSize;
                this.useGroupSeparator = useGroupSeparator;
                this.useLocalSetting = useLocalSetting;
                this.nullDisplay = nullDisplay;
                this.states = states;
            }
        }

        private bitsNumberFormatService bits;
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("StringFormat");

            // StiNumberFormatService
            jObject.AddPropertyIntNoDefaultValue("NegativePattern", NegativePattern);
            jObject.AddPropertyString("DecimalSeparator", DecimalSeparator, ".");
            jObject.AddPropertyInt("DecimalDigits", DecimalDigits, 2);
            jObject.AddPropertyString("GroupSeparator", GroupSeparator);
            jObject.AddPropertyInt("GroupSize", GroupSize, 3);
            jObject.AddPropertyBool("UseGroupSeparator", UseGroupSeparator, true);
            jObject.AddPropertyBool("UseLocalSetting", UseLocalSetting, true);
            jObject.AddPropertyString("NullDisplay", NullDisplay, " ");
            jObject.AddPropertyEnum("State", State, StiTextFormatState.None);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "NegativePattern":
                        this.NegativePattern = property.DeserializeInt();
                        break;

                    case "DecimalSeparator":
                        this.DecimalSeparator = property.DeserializeString();
                        break;

                    case "DecimalDigits":
                        this.DecimalDigits = property.DeserializeInt();
                        break;

                    case "GroupSeparator":
                        this.GroupSeparator = property.DeserializeString();
                        break;

                    case "GroupSize":
                        this.GroupSize = property.DeserializeInt();
                        break;

                    case "UseGroupSeparator":
                        this.UseGroupSeparator = property.DeserializeBool();
                        break;

                    case "UseLocalSetting":
                        this.UseLocalSetting = property.DeserializeBool();
                        break;

                    case "NullDisplay":
                        this.NullDisplay = property.DeserializeString();
                        break;

                    case "State":
                        this.State = property.DeserializeEnum<StiTextFormatState>();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var service = base.Clone() as StiNumberFormatService;

            if (this.bits != null)
                service.bits = this.bits.Clone() as bitsNumberFormatService;

            return service;
        }
        #endregion

        #region StiFormatService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        [JsonIgnore]
        public override string ServiceName => Loc.Get("FormFormatEditor", "Number");

        [JsonIgnore]
        public override int Position => 2;

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public override object Sample => -1234.123456789;

        /// <summary>
        /// Returns native format string.
        /// </summary>
        [JsonIgnore]
        public override string NativeFormatString => "{0:N2}";

        /// <summary>
        /// Gets or sets string of formatting.
        /// </summary>
        [StiNonSerialized]
        [JsonIgnore]
        public override string StringFormat
        {
            get
            {
                return base.StringFormat;
            }
            set
            {
                base.StringFormat = value;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets string value to show the value null.
        /// </summary>
        [DefaultValue(" ")]
        [StiSerializable]
        public string NullDisplay
        {
            get
            {
                return bits == null ? " " : bits.nullDisplay;
            }
            set
            {
                if (value == " " && bits == null)
                    return;

                if (bits != null)
                    bits.nullDisplay = value;
                else
                    bits = new bitsNumberFormatService(
                        this.NegativePattern, this.DecimalSeparator, this.DecimalDigits, this.GroupSeparator,
                        this.GroupSize, this.UseGroupSeparator, this.UseLocalSetting, value, this.State);
            }
        }

        /// <summary>
        /// Gets or sets the format pattern for negative numeric values.
        /// </summary>
        [StiSerializable]
        public int NegativePattern
        {
            get
            {
                return bits == null ? 1 : bits.negativePattern;
            }
            set
            {
                if (value == 1 && bits == null)
                    return;

                if (bits != null)
                    bits.negativePattern = value;
                else
                    bits = new bitsNumberFormatService(
                        value, this.DecimalSeparator, this.DecimalDigits, this.GroupSeparator,
                        this.GroupSize, this.UseGroupSeparator, this.UseLocalSetting, this.NullDisplay, this.State);
            }
        }

        /// <summary>
        /// Gets or sets the string to use as the decimal separator in currency values.
        /// </summary>
        [StiSerializable]
        [DefaultValue(".")]
        public string DecimalSeparator
        {
            get
            {
                return bits == null ? "." : bits.decimalSeparator;
            }
            set
            {
                if (value == "." && bits == null)
                    return;

                if (bits != null)
                    bits.decimalSeparator = value;
                else
                    bits = new bitsNumberFormatService(
                        this.NegativePattern, value, this.DecimalDigits, this.GroupSeparator,
                        this.GroupSize, this.UseGroupSeparator, this.UseLocalSetting, this.NullDisplay, this.State);
            }
        }

        /// <summary>
        /// Gets or sets indicates the number of decimal places to use in currency values.
        /// </summary>
        [StiSerializable]
        public int DecimalDigits
        {
            get
            {
                return bits == null ? 2 : bits.decimalDigits;
            }
            set
            {
                if (value == 2 && bits == null)
                    return;

                if (bits != null)
                    bits.decimalDigits = value;
                else
                    bits = new bitsNumberFormatService(
                        this.NegativePattern, this.DecimalSeparator, value, this.GroupSeparator,
                        this.GroupSize, this.UseGroupSeparator, this.UseLocalSetting, this.NullDisplay, this.State);
            }
        }

        /// <summary>
        /// DBS use only!
        /// </summary>
        [Browsable(false)]
        [StiNonSerialized]
        internal int? TotalNumberCapacity { get; set; }

        /// <summary>
        /// Gets or sets the string that separates groups of digits to the left 
        /// of the decimal in currency values.
        /// </summary>
        [StiSerializable]
        public string GroupSeparator
        {
            get
            {
                return bits == null ? " " : bits.groupSeparator;
            }
            set
            {
                if (value == " " && bits == null)
                    return;

                if (bits != null)
                    bits.groupSeparator = value;
                else
                    bits = new bitsNumberFormatService(
                        this.NegativePattern, this.DecimalSeparator, this.DecimalDigits, value,
                        this.GroupSize, this.UseGroupSeparator, this.UseLocalSetting, this.NullDisplay, this.State);
            }
        }

        /// <summary>
        /// Gets or sets the number of digits in group to the left of the decimal in currency values.
        /// </summary>
        [StiSerializable]
        [DefaultValue(3)]
        public int GroupSize
        {
            get
            {
                return bits == null ? 3 : bits.groupSize;
            }
            set
            {
                if (value == 3 && bits == null)
                    return;

                if (value > 9) value = 9;

                if (bits != null)
                    bits.groupSize = value;
                else
                    bits = new bitsNumberFormatService(
                        this.NegativePattern, this.DecimalSeparator, this.DecimalDigits, this.GroupSeparator,
                        value, this.UseGroupSeparator, this.UseLocalSetting, this.NullDisplay, this.State);
            }
        }

        /// <summary>
        /// Gets or sets value indicates it is necessary to use a group separator.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool UseGroupSeparator
        {
            get
            {
                return bits == null || bits.useGroupSeparator;
            }
            set
            {
                if (value && bits == null)
                    return;

                if (bits != null)
                    bits.useGroupSeparator = value;
                else
                    bits = new bitsNumberFormatService(
                        this.NegativePattern, this.DecimalSeparator, this.DecimalDigits, this.GroupSeparator,
                        this.GroupSize, value, this.UseLocalSetting, this.NullDisplay, this.State);
            }
        }

        /// <summary>
        /// Gets or sets value indicates it is necessary to use local settings.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        public bool UseLocalSetting
        {
            get
            {
                return bits == null || bits.useLocalSetting;
            }
            set
            {
                if (value && bits == null)
                    return;

                if (bits != null)
                    bits.useLocalSetting = value;
                else
                    bits = new bitsNumberFormatService(
                        this.NegativePattern, this.DecimalSeparator, this.DecimalDigits, this.GroupSeparator,
                        this.GroupSize, this.UseGroupSeparator, value, this.NullDisplay, this.State);
            }
        }

        [StiSerializable]
        [DefaultValue(StiTextFormatState.None)]
        public StiTextFormatState State
        {
            get
            {
                return bits == null ? StiTextFormatState.None : bits.states;
            }
            set
            {
                if (value == StiTextFormatState.None && bits == null)
                    return;

                if (bits != null)
                    bits.states = value;
                else
                    bits = new bitsNumberFormatService(
                        this.NegativePattern, this.DecimalSeparator, this.DecimalDigits, this.GroupSeparator,
                        this.GroupSize, this.UseGroupSeparator, this.UseLocalSetting, this.NullDisplay, value);
            }
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            var format = obj as StiNumberFormatService;

            if (format == null)
                return false;

            if (NullDisplay != format.NullDisplay)
                return false;

            if (NegativePattern != format.NegativePattern)
                return false;

            if (DecimalDigits != format.DecimalDigits)
                return false;

            if (DecimalSeparator != format.DecimalSeparator)
                return false;

            if (GroupSeparator != format.GroupSeparator)
                return false;

            if (GroupSize != format.GroupSize)
                return false;

            if (UseGroupSeparator != format.UseGroupSeparator)
                return false;

            if (UseLocalSetting != format.UseLocalSetting)
                return false;

            if (StringFormat != format.StringFormat)
                return false;

            if (State != format.State)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal static void FillLocalSetting(NumberFormatInfo format)
        {
            format.NumberDecimalDigits = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
            format.NumberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            format.NumberGroupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            format.NumberGroupSizes = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes;
            format.NumberNegativePattern = CultureInfo.CurrentCulture.NumberFormat.NumberNegativePattern;

            format.CurrencyDecimalDigits = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits;
            format.CurrencyDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            format.CurrencyGroupSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator;
            format.CurrencyGroupSizes = CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSizes;
            format.CurrencyNegativePattern = CultureInfo.CurrentCulture.NumberFormat.CurrencyNegativePattern;
            format.CurrencyPositivePattern = CultureInfo.CurrentCulture.NumberFormat.CurrencyPositivePattern;
            format.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

            format.PercentDecimalDigits = CultureInfo.CurrentCulture.NumberFormat.PercentDecimalDigits;
            format.PercentDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator;
            format.PercentGroupSeparator = CultureInfo.CurrentCulture.NumberFormat.PercentGroupSeparator;
            format.PercentGroupSizes = CultureInfo.CurrentCulture.NumberFormat.PercentGroupSizes;
            format.PercentNegativePattern = CultureInfo.CurrentCulture.NumberFormat.PercentNegativePattern;
            format.PercentPositivePattern = CultureInfo.CurrentCulture.NumberFormat.PercentPositivePattern;
            format.PercentSymbol = CultureInfo.CurrentCulture.NumberFormat.PercentSymbol;

            format.PerMilleSymbol = CultureInfo.CurrentCulture.NumberFormat.PerMilleSymbol;
            format.PositiveInfinitySymbol = CultureInfo.CurrentCulture.NumberFormat.PositiveInfinitySymbol;

            format.PositiveSign = CultureInfo.CurrentCulture.NumberFormat.PositiveSign;
            format.NaNSymbol = CultureInfo.CurrentCulture.NumberFormat.NaNSymbol;
            format.NegativeInfinitySymbol = CultureInfo.CurrentCulture.NumberFormat.NegativeInfinitySymbol;
            format.NegativeSign = CultureInfo.CurrentCulture.NumberFormat.NegativeSign;
        }

        /// <summary>
        /// Formats the specified object in order of regulations and returns a string.
        /// </summary>
        /// <param name="stringFormat">String of formatting.</param>
        /// <param name="arg">Object for formatting.</param>
        /// <returns>Formatted string.</returns>
        public override string Format(string stringFormat, object arg)
        {
            if (arg == null || arg is DBNull)
                return NullDisplay;

            var format = new NumberFormatInfo();

            if (UseLocalSetting)
            {
                FillLocalSetting(format);

                if ((State & StiTextFormatState.DecimalDigits) > 0)
                    format.NumberDecimalDigits = DecimalDigits;

                if ((State & StiTextFormatState.DecimalSeparator) > 0 && !string.IsNullOrEmpty(DecimalSeparator))
                    format.NumberDecimalSeparator = DecimalSeparator;

                if ((State & StiTextFormatState.GroupSeparator) > 0 && !string.IsNullOrEmpty(GroupSeparator))
                    format.NumberGroupSeparator = GroupSeparator;

                if ((State & StiTextFormatState.GroupSize) > 0)
                    format.NumberGroupSizes = new[] { GroupSize };

                if ((State & StiTextFormatState.NegativePattern) > 0)
                    format.NumberNegativePattern = NegativePattern;
            }
            else
            {
                if (string.IsNullOrEmpty(DecimalSeparator))
                    format.NumberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                else
                    format.NumberDecimalSeparator = DecimalSeparator;

                format.NumberDecimalDigits = DecimalDigits;
                if (GroupSeparator != null)
                    format.NumberGroupSeparator = GroupSeparator;

                format.NumberGroupSizes = new[] { GroupSize };
                format.NumberNegativePattern = NegativePattern;
            }

            if (!UseGroupSeparator)
                format.NumberGroupSeparator = string.Empty;

            if (StiOptions.Engine.AllowConvertingInFormatting && arg?.GetType() != null && !arg.GetType().IsNumericType())
            {
                var convertedArg = StiValueHelper.TryToNullableDecimal(arg);
                if (convertedArg != null)
                    arg = convertedArg;
            }

            if ((State & StiTextFormatState.Abbreviation) > 0)
            {
                var value = StiValueHelper.TryToNullableDecimal(arg);
                if (value != null)
                {
                    int decimalDigitsValue = ((State & StiTextFormatState.DecimalDigits) > 0) ? DecimalDigits : 0;

                    string postfix;
                    value = StiAbbreviationNumberFormatHelper.Format(value.Value, out postfix, decimalDigitsValue, TotalNumberCapacity);

                    return $"{value}{postfix}";
                }
            }

            return string.Format(format, "{0:N}", arg);
        }
        #endregion

        #region Methods.abstract
        public override StiFormatService CreateNew()
        {
            return new StiNumberFormatService();
        }
        #endregion

        /// <summary>
		/// Creates a new format of the type StiNumberFormatService.
		/// </summary>
		public StiNumberFormatService() : this(
            CultureInfo.CurrentCulture.NumberFormat.NumberNegativePattern,
            CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
            CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits,
            CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator,
            CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes[0],
            true,
            true,
            " ")
        {
        }

        /// <summary>
        /// Creates a new format of the type StiNumberFormatService with specified arguments.
        /// </summary>
        /// <param name="negativePattern">The format pattern for negative numeric values.</param>
        /// <param name="decimalSeparator">The string to use as the decimal separator in currency values.</param>
        /// <param name="decimalDigits">The number of decimal places to use in currency values.</param>
        /// <param name="groupSeparator">The string that separates groups of digits to the left of the decimal in currency values.</param>
        /// <param name="groupSize">The number of digits in group to the left of the decimal in currency values.</param>
        /// <param name="useGroupSeparator">Value indicates that it is necessary to use a group separator.</param>
        /// <param name="useLocalSetting">Value indicates that it is necessary to use local settings.</param>
        /// <param name="nullDisplay">String value to show the value null.</param>
        public StiNumberFormatService(
            int negativePattern,
            string decimalSeparator,
            int decimalDigits,
            string groupSeparator,
            int groupSize,
            bool useGroupSeparator,
            bool useLocalSetting,
            string nullDisplay) :

            this(
            negativePattern,
            0,
            decimalSeparator,
            decimalDigits,
            groupSeparator,
            groupSize,
            useGroupSeparator,
            useLocalSetting,
            nullDisplay)
        {
        }

        public StiNumberFormatService(
            int negativePattern,
            int decimalPlaces,
            string decimalSeparator,
            int decimalDigits,
            string groupSeparator,
            int groupSize,
            bool useGroupSeparator,
            bool useLocalSetting,
            string nullDisplay) :

            this(
            negativePattern,
            decimalPlaces,
            decimalSeparator,
            decimalDigits,
            groupSeparator,
            groupSize,
            useGroupSeparator,
            useLocalSetting,
            nullDisplay,
            StiTextFormatState.None)
        {
        }

        public StiNumberFormatService(
            int negativePattern,
            int decimalPlaces,
            string decimalSeparator,
            int decimalDigits,
            string groupSeparator,
            int groupSize,
            bool useGroupSeparator,
            bool useLocalSetting,
            string nullDisplay,
            StiTextFormatState state)
        {
            this.DecimalDigits = decimalDigits;
            this.DecimalSeparator = decimalSeparator;
            this.GroupSeparator = groupSeparator;
            this.GroupSize = groupSize;
            this.NegativePattern = negativePattern;
            this.UseGroupSeparator = useGroupSeparator;
            this.UseLocalSetting = useLocalSetting;
            this.NullDisplay = nullDisplay;
            this.State = state;
        }
    }
}