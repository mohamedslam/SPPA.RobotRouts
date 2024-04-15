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

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Describes a service for the text formatting as percentage values.
    /// </summary>
    [TypeConverter(typeof(StiPercentageFormatConverter))]
    [StiServiceBitmap(typeof(StiPercentageFormatService), "Stimulsoft.Report.Images.Formats.Percentage.png")]
    [StiFormatEditor("Stimulsoft.Report.Components.TextFormats.Design.StiPercentageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfFormatEditor("Stimulsoft.Report.WpfDesign.StiPercentageEditor, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiPercentageFormatService : StiCurrencyFormatService
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("StringFormat");

            return jObject;
        }
        #endregion

        #region StiFormatService override
        /// <summary>
		/// Gets a service name.
		/// </summary>
        [JsonIgnore]
        public override string ServiceName => Loc.Get("FormFormatEditor", "Percentage");

        [JsonIgnore]
        public override int Position => 6;

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

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public override object Sample => -1.2312;

        /// <summary>
        /// Returns native format string.
        /// </summary>
        [JsonIgnore]
        public override string NativeFormatString => "{0:P2}";
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            var format = obj as StiPercentageFormatService;

            if (format == null)
                return false;

            if (!base.Equals(format))
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
                    format.PercentDecimalDigits = DecimalDigits;

                if ((State & StiTextFormatState.DecimalSeparator) > 0 && !string.IsNullOrEmpty(DecimalSeparator))
                    format.PercentDecimalSeparator = DecimalSeparator;

                if ((State & StiTextFormatState.GroupSeparator) > 0 && !string.IsNullOrEmpty(GroupSeparator))
                    format.PercentGroupSeparator = GroupSeparator;

                if ((State & StiTextFormatState.GroupSize) > 0)
                    format.PercentGroupSizes = new[] { GroupSize };

                if ((State & StiTextFormatState.NegativePattern) > 0)
                    format.PercentNegativePattern = NegativePattern;

                if ((State & StiTextFormatState.PositivePattern) > 0)
                    format.PercentPositivePattern = PositivePattern;

                if ((State & StiTextFormatState.PercentageSymbol) > 0)
                    format.PercentSymbol = Symbol;
            }
            else
            {

                format.PercentDecimalSeparator = string.IsNullOrEmpty(DecimalSeparator)
                    ? CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator
                    : DecimalSeparator;

                format.PercentDecimalDigits = DecimalDigits;
                format.PercentGroupSeparator = GroupSeparator;
                format.PercentGroupSizes = new[] { GroupSize };
                format.PercentPositivePattern = PositivePattern;
                format.PercentNegativePattern = NegativePattern;
                format.PercentSymbol = Symbol;
            }

            if (!UseGroupSeparator)
                format.PercentGroupSeparator = string.Empty;

            if (StiOptions.Engine.AllowConvertingInFormatting && arg?.GetType() != null && !arg.GetType().IsNumericType())
            {
                var convertedArg = StiValueHelper.TryToNullableDecimal(arg);
                if (convertedArg != null)
                    arg = convertedArg;
            }

            return string.Format(format, "{0:P}", arg);
        }
        #endregion

        #region Methods.abstract
        public override StiFormatService CreateNew()
        {
            return new StiPercentageFormatService();
        }
        #endregion

        /// <summary>
		/// Creates a new format of the type StiPercentageFormatService.
		/// </summary>
		public StiPercentageFormatService() : this(
            CultureInfo.CurrentCulture.NumberFormat.PercentPositivePattern,
            CultureInfo.CurrentCulture.NumberFormat.PercentNegativePattern,
            CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator,
            CultureInfo.CurrentCulture.NumberFormat.PercentDecimalDigits,
            CultureInfo.CurrentCulture.NumberFormat.PercentGroupSeparator,
            CultureInfo.CurrentCulture.NumberFormat.PercentGroupSizes[0],
            CultureInfo.CurrentCulture.NumberFormat.PercentSymbol,
            true,
            true,
            " ")
        {
        }

        /// <summary>
        /// Creates a new format of the type StiPercentageFormatService with specified parameters.
        /// </summary>
        /// <param name="positivePattern">The format pattern for positive currency values.</param>
        /// <param name="negativePattern">The format pattern for negative numeric values.</param>
        /// <param name="decimalSeparator">The string to use as the decimal separator in currency values.</param>
        /// <param name="decimalDigits">The number of decimal places to use in currency values.</param>
        /// <param name="groupSeparator">The string that separates groups of digits to the left of the decimal in currency values.</param>
        /// <param name="groupSize">The number of digits in group to the left of the decimal in currency values.</param>
        /// <param name="symbol">Currency synbol.</param>
        /// <param name="useGroupSeparator">Value indicates that it is necessary to use a group separator.</param>
        /// <param name="useLocalSetting">Value indicates that it is necessary to use local settings.</param>
        /// <param name="nullDisplay">String value to show the value null.</param>
        public StiPercentageFormatService(
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

        public StiPercentageFormatService(
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

        public StiPercentageFormatService(
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