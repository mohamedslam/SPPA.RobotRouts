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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
using Stimulsoft.Base.Json;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    /// Describes a service for the text formatting by default.
    /// </summary>
    [StiServiceBitmap(typeof(StiGeneralFormatService), "Stimulsoft.Report.Images.Formats.General.png")]
    public class StiGeneralFormatService : StiFormatService
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
        public override string ServiceName => Loc.Get("FormFormatEditor", "General");

        [JsonIgnore]
        public override int Position => 1;

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public override object Sample => Loc.Get("FormFormatEditor", "SampleText");

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

        #region Methods
        public override bool Equals(object obj)
        {
            var format = obj as StiGeneralFormatService;

            return format != null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Fields.Static
        public static StiGeneralFormatService Default = new StiGeneralFormatService();
        #endregion

        #region Methods.abstract
        public override StiFormatService CreateNew()
        {
            return new StiGeneralFormatService();
        }
        #endregion

        /// <summary>
		/// Creates format by default.
		/// </summary>
		public StiGeneralFormatService()
        {
            this.StringFormat = string.Empty;
        }
    }
}