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

using Stimulsoft.Data.Functions;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Describes a service for the text formatting as a date value.
    /// </summary>
    [TypeConverter(typeof(StiDateFormatConverter))]
    [StiServiceBitmap(typeof(StiDateFormatService), "Stimulsoft.Report.Images.Formats.Date.png")]
    [StiFormatEditor("Stimulsoft.Report.Components.TextFormats.Design.StiDateEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfFormatEditor("Stimulsoft.Report.WpfDesign.StiDateEditor, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiDateFormatService : StiFormatService
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyString("StringFormat", StringFormat, "d");

            // StiDateFormatService
            jObject.AddPropertyString("NullDisplay", NullDisplay, " ");

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "NullDisplay":
                        this.NullDisplay = property.DeserializeString();
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
        public override string ServiceName => Loc.Get("FormFormatEditor", "Date");

        [JsonIgnore]
        public override int Position => 4;

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public override object Sample => DateTime.Now;

        /// <summary>
        /// Returns native format string.
        /// </summary>
        [JsonIgnore]
        public override string NativeFormatString => "{0:" + Loc.Get("Formats", "date11") + "}";

        /// <summary>
        /// Gets or sets string of formatting.
        /// </summary>
        [DefaultValue("d")]
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
		/// Gets or sets string value to show null date.
		/// </summary>
		[DefaultValue(" ")]
        [StiSerializable]
        public string NullDisplay { get; set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            var format = obj as StiDateFormatService;

            if (format == null)
                return false;

            if (NullDisplay != format.NullDisplay)
                return false;

            if (StringFormat != format.StringFormat)
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
        /// <param name="stringFormat">String for formatting.</param>
		/// <param name="arg">Object for formatting.</param>
		/// <returns>Formatted string.</returns>
		public override string Format(string stringFormat, object arg)
        {
            if (arg == null || arg is DBNull)
                return NullDisplay;

            if (string.IsNullOrEmpty(StringFormat))
                return arg.ToString();



            if (StiOptions.Engine.AllowConvertingInFormatting && !(arg is DateTime || arg is TimeSpan || arg is DateTimeOffset))
            {
                var convertedArg = StiValueHelper.TryToNullableDateTime(arg);
                if (convertedArg != null)
                    arg = convertedArg;
            }

            if (arg is DateTime || arg is TimeSpan || arg is DateTimeOffset)
            {
                var sf = stringFormat.ToUpperInvariant();

                if (sf == "Q" || sf == "QI" || sf == "YQ" || sf == "YQI")
                    return FormatQuarter(stringFormat, arg);
                else
                    return string.Format("{0:" + stringFormat + "}", arg);
            }

            return arg.ToString();
        }

        private static string FormatQuarter(string stringFormat, object arg)
        {
            if (arg is DateTimeOffset)
                arg = ((DateTimeOffset) arg).DateTime;

            if (arg is DateTime)
            {
                var date = (DateTime) arg;
                if (stringFormat == "Q")
                    return Funcs.QuarterName(date);

                if (stringFormat == "QI")
                    return Funcs.QuarterIndex(date).ToString();

                if (stringFormat == "YQ")
                    return $"{date.Year}-{Funcs.QuarterName(date)}";

                else if (stringFormat == "YQI")
                    return $"{date.Year}-{Funcs.QuarterIndex(date)}";
            }

            return arg.ToString();
        }
        #endregion

        #region Methods.abstract
        public override StiFormatService CreateNew()
        {
            return new StiDateFormatService();
        }
        #endregion

        /// <summary>
		/// Creates a new format of the type StiDateFormatService.
		/// </summary>
		public StiDateFormatService() : this("d", " ")
        {
        }

        /// <summary>
        /// Creates a new format of the type StiDateFormatService.
        /// </summary>
        /// <param name="stringFormat">String of formatting.</param>
        /// <param name="nullDisplay">String value to show null date.</param>
        public StiDateFormatService(
            string stringFormat,
            string nullDisplay)
        {
            this.StringFormat = stringFormat;
            this.NullDisplay = nullDisplay;
        }
    }
}