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
using System;
using System.ComponentModel;
using System.Globalization;

using Stimulsoft.Report.Helpers;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Describes a service for the text formatting as currency values.
    /// </summary>
    [TypeConverter(typeof(StiCurrencyFormatConverter))]
    [StiServiceBitmap(typeof(StiCurrencyFormatService), "Stimulsoft.Report.Images.Formats.Currency.png")]
    [StiFormatEditor("Stimulsoft.Report.Components.TextFormats.Design.StiCurrencyEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfFormatEditor("Stimulsoft.Report.WpfDesign.StiCurrencyEditor, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiCurrencyFormatService : StiNumberFormatService
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("StringFormat");

            // StiCurrencyFormatService
            jObject.AddPropertyIntNoDefaultValue("PositivePattern", PositivePattern);
            jObject.AddPropertyString("Symbol", Symbol, "$");

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "PositivePattern":
                        this.PositivePattern = property.DeserializeInt();
                        break;

                    case "Symbol":
                        this.Symbol = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region StiFormatService override
        /// <summary>
		/// Gets a service name.
		/// </summary>
        [JsonIgnore]
        public override string ServiceName => Loc.Get("FormFormatEditor", "Currency");

        [JsonIgnore]
        public override int Position => 3;

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public override object Sample => -1234.12345679;

        /// <summary>
        /// Returns native format string.
        /// </summary>
        [JsonIgnore]
        public override string NativeFormatString => "{0:C2}";

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
		/// Gets or sets the format pattern for positive currency values.
		/// </summary>
		[StiSerializable]
        public int PositivePattern { get; set; }

        /// <summary>
		/// Gets or sets a currency symbol.
		/// </summary>
		[StiSerializable]
        [DefaultValue("$")]
        public string Symbol { get; set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            var format = obj as StiCurrencyFormatService;

            if (format == null)
                return false;

            if (!base.Equals(format))
                return false;

            if (PositivePattern != format.PositivePattern)
                return false;

            if (Symbol != format.Symbol)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
                    format.CurrencyDecimalDigits = DecimalDigits;

                if ((State & StiTextFormatState.DecimalSeparator) > 0 && !string.IsNullOrEmpty(DecimalSeparator))
                    format.CurrencyDecimalSeparator = DecimalSeparator;

                if ((State & StiTextFormatState.GroupSeparator) > 0 && !string.IsNullOrEmpty(GroupSeparator))
                    format.CurrencyGroupSeparator = GroupSeparator;

                if ((State & StiTextFormatState.GroupSize) > 0)
                    format.CurrencyGroupSizes = new[] { GroupSize };

                if ((State & StiTextFormatState.NegativePattern) > 0)
                    format.CurrencyNegativePattern = NegativePattern;

                if ((State & StiTextFormatState.PositivePattern) > 0)
                    format.CurrencyPositivePattern = PositivePattern;

                if ((State & StiTextFormatState.CurrencySymbol) > 0)
                    format.CurrencySymbol = Symbol;
            }
            else
            {

                format.CurrencyDecimalSeparator = string.IsNullOrEmpty(DecimalSeparator)
                    ? CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator
                    : DecimalSeparator;

                format.CurrencyDecimalDigits = DecimalDigits;

                if (GroupSeparator != null)
                    format.CurrencyGroupSeparator = GroupSeparator;

                format.CurrencyGroupSizes = new[] { GroupSize };
                format.CurrencyPositivePattern = PositivePattern;
                format.CurrencyNegativePattern = NegativePattern;

                if (Symbol != null)
                    format.CurrencySymbol = Symbol;
            }

            if (!UseGroupSeparator)
                format.CurrencyGroupSeparator = string.Empty;

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
                    if (UseLocalSetting)
                    {
                        if ((State & StiTextFormatState.DecimalDigits) > 0)
                            format.NumberDecimalDigits = DecimalDigits;

                        if ((State & StiTextFormatState.DecimalSeparator) > 0 && !string.IsNullOrEmpty(DecimalSeparator))
                            format.NumberDecimalSeparator = DecimalSeparator;

                        if ((State & StiTextFormatState.GroupSeparator) > 0 && !string.IsNullOrEmpty(GroupSeparator))
                            format.NumberGroupSeparator = GroupSeparator;

                        if ((State & StiTextFormatState.GroupSize) > 0)
                            format.NumberGroupSizes = new[] { GroupSize };
                    }

                    string postfix;
                    value = StiAbbreviationNumberFormatHelper.Format(value.Value, out postfix, format.NumberDecimalDigits, TotalNumberCapacity);

                    var str = TotalNumberCapacity == null
                        ? $"{string.Format(format, "{0:N}", value)}{postfix}"
                        : $"{value}{postfix}";
                    return FormatAsCurrency(value.Value, str);
                }
            }

            return string.Format(format, "{0:C}", arg);
        }

        private string FormatAsCurrency(decimal value, string str)
        {
            var symbol = GetCurrencySymbol();
            if (value >= 0)
            {
                var pattern = GetPositivePattern();
                return pattern.Replace("n", str).Replace("$", symbol);
            }
            else
            {
                str = str.Replace("-", "");
                var pattern = GetNegativePattern();
                return pattern.Replace("n", str).Replace("$", symbol);
            }
        }

        private string GetCurrencySymbol()
        {
            return UseLocalSetting && (State & StiTextFormatState.CurrencySymbol) == 0
                ? CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol
                : Symbol;
        }

        private string GetPositivePattern()
        {
            var index = UseLocalSetting && (State & StiTextFormatState.PositivePattern) == 0
                ? CultureInfo.CurrentCulture.NumberFormat.CurrencyPositivePattern
                : PositivePattern;

            var patterns = new[]
            {
                "$n",
                "n$",
                "$ n",
                "n $"
            };

            if (index >= 0 && index < patterns.Length)
                return patterns[index];
            else
                return "$n";
        }

        private string GetNegativePattern()
        {
            var index = UseLocalSetting && (State & StiTextFormatState.NegativePattern) == 0
                ? CultureInfo.CurrentCulture.NumberFormat.CurrencyNegativePattern
                : NegativePattern;

            var patterns = new[] 
            {
                "($n)",
                "-$n",
                "$-n",
                "$n-",
                "(n$)",
                "-n$",
                "n-$",
                "n$-",
                "-n $",
                "-$ n",
                "n $-",
                "$ n-",
                "$ -n",
                "n- $",
                "($ n)",
                "(n $)"
            };

            if (index >= 0 && index < patterns.Length)
                return patterns[index];
            else
                return "($n)";
        }
        #endregion

        #region Methods.abstract
        public override StiFormatService CreateNew()
        {
            return new StiCurrencyFormatService();
        }
        #endregion

        /// <summary>
        /// Creates a new format of the type StiCurrencyFormatService.
        /// </summary>
        public StiCurrencyFormatService() : this(
            CultureInfo.CurrentCulture.NumberFormat.CurrencyPositivePattern,
            CultureInfo.CurrentCulture.NumberFormat.CurrencyNegativePattern,
            CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator,
            CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits,
            CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator,
            CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSizes[0],
            CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol,
            true,
            true,
            " ")
        {
        }

        /// <summary>
        /// Creates a new format of the type StiCurrencyFormatService with specified arguments.
        /// </summary>
        /// <param name="positivePattern">The format pattern for positive currency values.</param>
        /// <param name="negativePattern">The format pattern for negative numeric values.</param>
        /// <param name="decimalSeparator">The string to use as the decimal separator in currency values.</param>
        /// <param name="decimalDigits">The number of decimal places to use in currency values.</param>
        /// <param name="groupSeparator">The string that separates groups of digits to the left of the decimal in currency values.</param>
        /// <param name="groupSize">The number of digits in group to the left of the decimal in currency values.</param>
        /// <param name="symbol">Currency symbol.</param>
        /// <param name="useGroupSeparator">Value indicates that it is necessary to use a group separator.</param>
        /// <param name="useLocalSetting">Value indicates that it is necessary to use local settings.</param>
        /// <param name="nullDisplay">String value to show the value null.</param>
        public StiCurrencyFormatService(
            int positivePattern,
            int negativePattern,
            string decimalSeparator,
            int decimalDigits,
            string groupSeparator,
            int groupSize,
            string symbol,
            bool useGroupSeparator,
            bool useLocalSetting,
            string nullDisplay) :

            this(
            positivePattern,
            negativePattern,
            0,
            decimalSeparator,
            decimalDigits,
            groupSeparator,
            groupSize,
            symbol,
            useGroupSeparator,
            useLocalSetting,
            nullDisplay)
        {
        }

        /// <summary>
        /// Creates a new format of the type StiCurrencyFormatService with specified arguments.
        /// </summary>
        /// <param name="positivePattern">The format pattern for positive currency values.</param>
        /// <param name="negativePattern">The format pattern for negative numeric values.</param>
        /// <param name="decimalPlaces">Do not use argument.</param>
        /// <param name="decimalSeparator">The string to use as the decimal separator in currency values.</param>
        /// <param name="decimalDigits">The number of decimal places to use in currency values.</param>
        /// <param name="groupSeparator">The string that separates groups of digits to the left of the decimal in currency values.</param>
        /// <param name="groupSize">The number of digits in group to the left of the decimal in currency values.</param>
        /// <param name="symbol">Currency symbol.</param>
        /// <param name="useGroupSeparator">Value indicates that it is necessary to use a group separator.</param>
        /// <param name="useLocalSetting">Value indicates that it is necessary to use local settings.</param>
        /// <param name="nullDisplay">String value to show the value null.</param>
        public StiCurrencyFormatService(
            int positivePattern,
            int negativePattern,
            int decimalPlaces,
            string decimalSeparator,
            int decimalDigits,
            string groupSeparator,
            int groupSize,
            string symbol,
            bool useGroupSeparator,
            bool useLocalSetting,
            string nullDisplay) :

            this(
            positivePattern,
            negativePattern,
            decimalPlaces,
            decimalSeparator,
            decimalDigits,
            groupSeparator,
            groupSize,
            symbol,
            useGroupSeparator,
            useLocalSetting,
            nullDisplay,
            StiTextFormatState.None)
        {
        }

        public StiCurrencyFormatService(
            int positivePattern,
            int negativePattern,
            int decimalPlaces,
            string decimalSeparator,
            int decimalDigits,
            string groupSeparator,
            int groupSize,
            string symbol,
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
            this.PositivePattern = positivePattern;
            this.Symbol = symbol;
            this.UseGroupSeparator = useGroupSeparator;
            this.UseLocalSetting = useLocalSetting;
            this.NullDisplay = nullDisplay;
            this.State = state;
        }
    }
}
