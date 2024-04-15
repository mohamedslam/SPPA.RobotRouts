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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.TextFormats.Design;
using System;
using System.ComponentModel;

namespace Stimulsoft.Report.Components.TextFormats
{
    /// <summary>
    ///  Describes a service for formatting of the text, like boolean values.
    /// </summary>
    [TypeConverter(typeof(StiBooleanFormatConverter))]
	[StiServiceBitmap(typeof(StiBooleanFormatService), "Stimulsoft.Report.Images.Formats.Boolean.png")]
	[StiFormatEditor("Stimulsoft.Report.Components.TextFormats.Design.StiBooleanEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfFormatEditor("Stimulsoft.Report.WpfDesign.StiBooleanEditor, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
	public class StiBooleanFormatService : StiFormatService
    {
        #region bits
        private class bitsBooleanFormatService : ICloneable
        {
            #region ICloneable
            public object Clone()
            {
                return this.MemberwiseClone();
            }
            #endregion

            #region Fields
            public string falseValue;
            public string trueValue;
            public string falseDisplay;
            public string trueDisplay;
            public string nullDisplay;
            #endregion

            public bitsBooleanFormatService(string falseValue, string trueValue, string falseDisplay, string trueDisplay,
                string nullDisplay)
            {
                this.falseValue = falseValue;
                this.trueValue = trueValue;
                this.falseDisplay = falseDisplay;
                this.trueDisplay = trueDisplay;
                this.nullDisplay = nullDisplay;
            }
        }

        private bitsBooleanFormatService bits;
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiBooleanFormatService
            if (bits != null)
            {
                jObject.AddPropertyStringNullOrEmpty("FalseValue", bits.falseValue);
                jObject.AddPropertyStringNullOrEmpty("TrueValue", bits.trueValue);
                jObject.AddPropertyStringNullOrEmpty("FalseDisplay", bits.falseDisplay);
                jObject.AddPropertyStringNullOrEmpty("TrueDisplay", bits.trueDisplay);
                jObject.AddPropertyStringNullOrEmpty("NullDisplay", bits.nullDisplay);
            }

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "FalseValue":
                        this.FalseValue = property.DeserializeString();
                        break;

                    case "TrueValue":
                        this.TrueValue = property.DeserializeString();
                        break;

                    case "FalseDisplay":
                        this.FalseDisplay = property.DeserializeString();
                        break;

                    case "TrueDisplay":
                        this.TrueDisplay = property.DeserializeString();
                        break;

                    case "NullDisplay":
                        this.NullDisplay = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var service = base.Clone() as StiBooleanFormatService;

            if (this.bits != null)
                service.bits = this.bits.Clone() as bitsBooleanFormatService;

            return service;
        }
        #endregion
        
        #region StiFormatService override
        /// <summary>
		/// Gets a service name.
		/// </summary>
        [JsonIgnore]
        public override string ServiceName => Loc.Get("FormFormatEditor", "Boolean");

        [JsonIgnore]
        public override int Position => 7;

        /// <summary>
        /// Gets value to show a sample of formatting.
        /// </summary>
        [JsonIgnore]
        public override object Sample => false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the string value for identification the value false.
        /// </summary>
        [StiSerializable]
        [DefaultValue("False")]
        public string FalseValue
        {
            get
            {
                return bits == null ? "False" : bits.falseValue;
            }
            set
            {
                if (value == "False" && bits == null)
                    return;

                if (bits != null)
                    bits.falseValue = value;
                else
                    bits = new bitsBooleanFormatService(
                        value, this.TrueValue, this.FalseDisplay, this.TrueDisplay,
                        this.NullDisplay);
            }
        }

        /// <summary>
        /// Gets or sets string value for identification the value true.
        /// </summary>
        [DefaultValue("True")]
        [StiSerializable]
        public string TrueValue
        {
            get
            {
                return bits == null ? "True" : bits.trueValue;
            }
            set
            {
                if (value == "True" && bits == null)
                    return;

                if (bits != null)
                    bits.trueValue = value;
                else
                    bits = new bitsBooleanFormatService(
                        this.FalseValue, value, this.FalseDisplay, this.TrueDisplay,
                        this.NullDisplay);
            }
        }

        /// <summary>
        /// Gets or sets the string value to show the value false.
        /// </summary>
        [StiSerializable]
        [DefaultValue("False")]
        public string FalseDisplay
        {
            get
            {
                return bits == null ? "False" : bits.falseDisplay;
            }
            set
            {
                if (value == "False" && bits == null)
                    return;

                if (bits != null)
                    bits.falseDisplay = value;
                else
                    bits = new bitsBooleanFormatService(
                        this.FalseValue, this.TrueValue, value, this.TrueDisplay,
                        this.NullDisplay);
            }
        }

        /// <summary>
        /// Gets or sets the string value to show the value true.
        /// </summary>
        [DefaultValue("True")]
        [StiSerializable]
        public string TrueDisplay
        {
            get
            {
                return bits == null ? "True" : bits.trueDisplay;
            }
            set
            {
                if (value == "True" && bits == null)
                    return;

                if (bits != null)
                    bits.trueDisplay = value;
                else
                    bits = new bitsBooleanFormatService(
                        this.FalseValue, this.TrueValue, this.FalseDisplay, value,
                        this.NullDisplay);
            }
        }

        /// <summary>
        /// Gets or sets the string value to show the value null.
        /// </summary>
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
                    bits = new bitsBooleanFormatService(
                        this.FalseValue, this.TrueValue, this.FalseDisplay, this.TrueDisplay,
                        value);
            }
        }
        #endregion        

        #region Methods
        public override bool Equals(object obj)
        {
            var format = obj as StiBooleanFormatService;
            if (format == null)
                return false;

            if (FalseValue != format.FalseValue)
                return false;

            if (TrueValue != format.TrueValue)
                return false;

            if (FalseDisplay != format.FalseDisplay)
                return false;

            if (TrueDisplay != format.TrueDisplay)
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
		/// Formats the specified object on specified rule and returns the line.
		/// </summary>
        /// <param name="stringFormat">String of the formatting.</param>
		/// <param name="arg">Object for formatting.</param>
		/// <returns>Formatted string.</returns>
		public override string Format(string stringFormat, object arg)
		{
			if (arg == null || arg is DBNull)
			    return NullDisplay;

		    var a = arg.ToString();

			if (a == TrueValue)
			    return TrueDisplay;

			if (a == FalseValue)
			    return FalseDisplay;

			return NullDisplay;
        }
        #endregion

        #region Methods.abstract
        public override StiFormatService CreateNew()
        {
            return new StiBooleanFormatService();
        }
        #endregion

		/// <summary>
		/// Creates a new format of the type StiBooleanFormatService.
		/// </summary>
		public StiBooleanFormatService() : this(
			"False", 
			"True", 
			StiLocalization.Get("FormFormatEditor", "nameFalse"), 
			StiLocalization.Get("FormFormatEditor", "nameTrue"),
			" ")
		{
		}

		/// <summary>
		/// Creates a new format of the type StiBooleanFormatService with specified parameters.
		/// </summary>
		/// <param name="falseValue">String value to define the value false.</param>
		/// <param name="trueValue">String value to define the value true.</param>
		/// <param name="falseDisplay">String value to show the value false.</param>
		/// <param name="trueDisplay">String value to show the value true.</param>
		/// <param name="nullDisplay">String value  to show the value null.</param>
		public StiBooleanFormatService(
			string falseValue,
			string trueValue, 
			string falseDisplay, 
			string trueDisplay,
			string nullDisplay)
		{
			this.FalseValue =	falseValue;
			this.TrueValue =	trueValue;
			this.FalseDisplay = falseDisplay;
			this.TrueDisplay =	trueDisplay;
			this.NullDisplay =	nullDisplay;
		}
	}
}
