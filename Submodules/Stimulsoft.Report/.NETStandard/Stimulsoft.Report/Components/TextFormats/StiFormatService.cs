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
using System.Linq;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Describes a base service for formatting of the text.
    /// </summary>
    [JsonObject]
    [JsonConverter(typeof(StiFormatServiceJsonConverter))]
    [StiServiceBitmap(typeof(StiFormatService), "Stimulsoft.Report.Images.Formats.Format.png")]
    [StiServiceCategoryBitmap(typeof(StiFormatService), "Stimulsoft.Report.Images.Formats.Format.png")]
    public abstract class StiFormatService :
        StiService,
        IStiDefault,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            // StiFormatService
            jObject.AddPropertyStringNullOrEmpty("StringFormat", StringFormat);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "StringFormat":
                        this.stringFormat = property.DeserializeString();
                        break;
                }
            }
        }

        internal static StiFormatService CreateFromJsonObject(JObject jObject)
        {
            var formats = StiOptions.Services.Formats;
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            var format = formats.FirstOrDefault(x => x.GetType().Name == ident).CreateNew();
            format.LoadFromJsonObject(jObject);

            return format;
        }
        #endregion

        #region IStiDefault
        [JsonIgnore]
        public bool IsDefault => this is StiGeneralFormatService;
        #endregion

        #region StiService override
        /// <summary>
        /// Gets a service category.
        /// </summary>
        [JsonIgnore]
        public sealed override string ServiceCategory => Loc.Get("Services", "categoryTextFormat");

        /// <summary>
		/// Gets a service type.
		/// </summary>
        [JsonIgnore]
        public sealed override Type ServiceType => typeof(StiFormatService);
        #endregion

        #region Properties
        public string Ident => GetType().Name;

        [JsonIgnore]
        public abstract int Position { get; }

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public abstract object Sample { get; }

        /// <summary>
        /// Returns native format string.
        /// </summary>
        [JsonIgnore]
        public virtual string NativeFormatString => "{0}";

        /// <summary>
		/// Internal use only.
		/// </summary>
        [JsonIgnore]
        public bool IsFormatStringFromVariable
        {
            get
            {
                if (this is StiCustomFormatService && StringFormat != null)
                {
                    var str = StringFormat.Trim();
                    return str.Length > 2 && str.StartsWithInvariant("{") && str.EndsWithInvariant("}");
                }

                return false;
            }
        }

        private string stringFormat = string.Empty;
        /// <summary>
        /// Gets or sets string of formatting.
        /// </summary>
        [StiSerializable]
        public virtual string StringFormat
        {
            get
            {
                return stringFormat;
            }
            set
            {
                stringFormat = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Formats the specified object in order of format and returns a string.
        /// </summary>
        /// <param name="arg">Object for formatting.</param>
        /// <returns>Formatted string.</returns>
        public virtual string Format(object arg)
        {
            return Format(StringFormat, arg);
        }

        /// <summary>
        /// Formats the specified object in order of format and returns a string.
        /// </summary>
        /// <param name="format">String of formatting.</param>
        /// <param name="arg">Object for formatting.</param>
        /// <returns>Formatted string.</returns>
        public virtual string Format(string format, object arg)
        {
            try
            {
                if (arg == null || arg is DBNull)
                    return string.Empty;

                if (string.IsNullOrEmpty(StringFormat))
                    return arg.ToString();

                var isNegative = false;
                if (arg is TimeSpan && StiOptions.Engine.AllowOldTimeSpanFormatting)
                {
                    var time = (TimeSpan)arg;
                    var date = DateTime.Today;
                    if (time.Ticks < 0)
                    {
                        time = time.Negate();
                        isNegative = true;
                    }
                    date = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
                    arg = date;
                }
                return string.Format("{0}{1:" + format + "}", isNegative ? "-" : "", arg);
            }
            catch
            {
                return arg == null ? string.Empty : arg.ToString();
            }
        }

        public override string ToString()
        {
            return ServiceName;
        }
        #endregion

        #region Methods.abstract
        public abstract StiFormatService CreateNew();
        #endregion
    }
}