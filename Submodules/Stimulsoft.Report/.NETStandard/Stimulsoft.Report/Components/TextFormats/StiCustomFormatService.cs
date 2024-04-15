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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.TextFormats.Design;
using System.ComponentModel;
using System.Text;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Describes a service for the text formatting done as customized.
    /// </summary>
    [TypeConverter(typeof(StiCustomFormatConverter))]
    [StiFormatEditor("Stimulsoft.Report.Components.TextFormats.Design.StiCustomEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfFormatEditor("Stimulsoft.Report.WpfDesign.StiCustomEditor, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiServiceBitmap(typeof(StiCurrencyFormatService), "Stimulsoft.Report.Images.Formats.Format.png")]
    public class StiCustomFormatService : StiFormatService
    {
        #region StiFormatService override
        /// <summary>
		/// Gets a service name.
		/// </summary>
        [JsonIgnore]
        public override string ServiceName => Loc.Get("FormFormatEditor", "Custom");

        [JsonIgnore]
        public override int Position => 100;

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public override object Sample
        {
            get
            {
                if (StringFormat == null)
                    return string.Empty;

                var str = StringFormat.ToLowerInvariant().Trim();

                if (str == "d" || str == "f" || str == "g" || str.StartsWith("y"))
                    return DateTime.Now;

                else if (str.StartsWith("c") || str.StartsWith("n") || str.StartsWith("#") || str.StartsWith("$"))
                    return 123.45;

                else if (str.StartsWith("(#"))
                    return 1234567890;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets string of formatting.
        /// </summary>
        [DefaultValue("")]
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

        #region Methods.override
        public override bool Equals(object obj)
        {
            var format = obj as StiCustomFormatService;

            return format != null && StringFormat == format.StringFormat;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Methods
        public override StiFormatService CreateNew()
        {
            return new StiCustomFormatService();
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
                return string.Empty;

            if (string.IsNullOrEmpty(StringFormat))
                return arg.ToString();

            if (arg is string)
            {
                double val;
                if (Double.TryParse((string)arg, out val)) 
                    return base.Format(stringFormat, val);
            }

            return base.Format(stringFormat, arg);
        }
        #endregion

        /// <summary>
		/// Creates a new format of the type StiCustomFormatService.
		/// </summary>
		public StiCustomFormatService() : this(string.Empty)
        {
        }

        /// <summary>
        /// Creates a new format of the type StiCustomFormatService.
        /// </summary>
        /// <param name="stringFormat">String of formatting</param>
        public StiCustomFormatService(string stringFormat)
        {
            this.StringFormat = stringFormat;
        }
    }
}