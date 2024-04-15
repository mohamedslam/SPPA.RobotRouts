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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.TextFormats.Design;
using System;
using System.ComponentModel;

using Stimulsoft.Base.Helpers;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Describes a service for the text formatting as a time value.
    /// </summary>
    [TypeConverter(typeof(StiTimeFormatConverter))]
    [StiServiceBitmap(typeof(StiTimeFormatService), "Stimulsoft.Report.Images.Formats.Time.png")]
    [StiFormatEditor("Stimulsoft.Report.Components.TextFormats.Design.StiTimeEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfFormatEditor("Stimulsoft.Report.WpfDesign.StiTimeEditor, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    public class StiTimeFormatService : StiFormatService
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.AddPropertyString("StringFormat", StringFormat, "t");

            return jObject;
        }
        #endregion

        #region StiFormatService override
        /// <summary>
		/// Gets a service name.
		/// </summary>
        [JsonIgnore]
        public override string ServiceName => Loc.Get("FormFormatEditor", "Time");

        [JsonIgnore]
        public override int Position => 5;

        /// <summary>
		/// Gets or sets string of formatting.
		/// </summary>
		[DefaultValue("t")]
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
        /// Returns native format string.
        /// </summary>
        [JsonIgnore]
        public override string NativeFormatString => "{0:" + Loc.Get("Formats", "time03") + "}";

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public override object Sample => DateTime.Now;
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            var format = obj as StiPercentageFormatService;

            return format != null && this.StringFormat == format.StringFormat;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Formats the specified object in order of regulations and returns a string.
        /// </summary>
        /// <param name="format">String for formatting.</param>
        /// <param name="arg">Object for formatting.</param>
        /// <returns>Formatted string.</returns>
        public override string Format(string format, object arg)
        {
            try
            {
                if (arg == null || arg is DBNull)
                    return string.Empty;

                if (string.IsNullOrEmpty(StringFormat))
                    return arg.ToString();

                if (StiOptions.Engine.AllowConvertingInFormatting && !(arg is DateTime || arg is TimeSpan || arg is DateTimeOffset))
                {
                    var convertedArg = StiValueHelper.TryToNullableDateTime(arg);
                    if (convertedArg != null)
                        arg = convertedArg;
                }

                if (arg is TimeSpan)
                {
                    if (format == "HH:mm") format = "hh':'mm";
                    if (format == "H:mm") format = "h':'mm";
                    if (format == "HH:mm:ss") format = "hh':'mm':'ss";
                }

                return base.Format(format, arg);
            }
            catch
            {
                return arg == null ? string.Empty : arg.ToString();
            }
        }
        #endregion

        #region Methods.abstract
        public override StiFormatService CreateNew()
        {
            return new StiTimeFormatService();
        }
        #endregion

        /// <summary>
		/// Creates a new format of the type StiTimeFormatService.
		/// </summary>
		public StiTimeFormatService() : this("t")
        {
        }

        /// <summary>
        /// Creates a new format of the type StiTimeFormatService.
        /// </summary>
        /// <param name="stringFormat">String of formatting.</param>
        public StiTimeFormatService(string stringFormat)
        {
            this.StringFormat = stringFormat;
        }
    }
}