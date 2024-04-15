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

using System;
using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;
using System.Drawing.Text;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Class describes all text option.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiTextOptionsConverter))]
	[RefreshProperties(RefreshProperties.All)]
	public sealed class StiTextOptions : 
        ICloneable,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            // bits
            jObject.AddPropertyBool("RightToLeft", RightToLeft);
            jObject.AddPropertyBool("LineLimit", LineLimit);
            jObject.AddPropertyFloat("Angle", Angle, 0f);
            jObject.AddPropertyFloat("FirstTabOffset", FirstTabOffset, 40f);
            jObject.AddPropertyFloat("DistanceBetweenTabs", DistanceBetweenTabs, 20f);
            jObject.AddPropertyEnum("HotkeyPrefix", HotkeyPrefix, HotkeyPrefix.None);
            jObject.AddPropertyEnum("Trimming", Trimming, StringTrimming.None);
            jObject.AddPropertyBool("WordWrap", WordWrap);

            if (jObject.Count > 0)
                return jObject;

            return null;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "RightToLeft":
                        RightToLeft = property.DeserializeBool();
                        break;

                    case "LineLimit":
                        LineLimit = property.DeserializeBool();
                        break;

                    case "Angle":
                        Angle = property.DeserializeFloat();
                        break;

                    case "FirstTabOffset":
                        FirstTabOffset = property.DeserializeFloat();
                        break;

                    case "DistanceBetweenTabs":
                        DistanceBetweenTabs = property.DeserializeFloat();
                        break;

                    case "HotkeyPrefix":
                        HotkeyPrefix = property.DeserializeEnum<HotkeyPrefix>();
                        break;

                    case "Trimming":
                        Trimming = property.DeserializeEnum<StringTrimming>();
                        break;

                    case "WordWrap":
                        WordWrap = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region bitsTextOptions
        private sealed class BitsTextOptions : ICloneable
        {
            #region ICloneable
            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>A new object that is a copy of this instance.</returns>
            public object Clone()
            {
                return MemberwiseClone();
            }
            #endregion

            #region IEquatable
            private bool Equals(BitsTextOptions other)
            {
                return RightToLeft == other.RightToLeft && LineLimit == other.LineLimit && Angle.Equals(other.Angle) && FirstTabOffset.Equals(other.FirstTabOffset) &&
                    DistanceBetweenTabs.Equals(other.DistanceBetweenTabs) && HotkeyPrefix == other.HotkeyPrefix && Trimming == other.Trimming;
            }

            public override bool Equals(object obj)
            {
                return obj != null && (this == obj || obj.GetType() == GetType() && Equals((BitsTextOptions) obj));
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = RightToLeft ? 1231 : 1237;
                    hashCode = (hashCode * 397) ^ (LineLimit ? 1231 : 1237);
                    hashCode = (hashCode*397) ^ Angle.GetHashCode();
                    hashCode = (hashCode*397) ^ FirstTabOffset.GetHashCode();
                    hashCode = (hashCode*397) ^ DistanceBetweenTabs.GetHashCode();
                    hashCode = (hashCode*397) ^ (int) HotkeyPrefix;
                    hashCode = (hashCode*397) ^ (int) Trimming;
                    return hashCode;
                }
            }
            #endregion

            public bool RightToLeft { get; set; }

            public bool LineLimit { get; set; }

            public float Angle { get; set; }

            public float FirstTabOffset { get; set; }

            public float DistanceBetweenTabs { get; set; }

            public HotkeyPrefix HotkeyPrefix { get; set; }

            public StringTrimming Trimming { get; set; }

            public BitsTextOptions(bool rightToLeft, bool lineLimit, float angle, float firstTabOffset, float distanceBetweenTabs,
                HotkeyPrefix hotkeyPrefix, StringTrimming trimming)
            {
                RightToLeft = rightToLeft;
                LineLimit = lineLimit;
                Angle = angle;
                FirstTabOffset = firstTabOffset;
                DistanceBetweenTabs = distanceBetweenTabs;
                HotkeyPrefix = hotkeyPrefix;
                Trimming = trimming;
            }
        }
        #endregion

        #region Fields
        private BitsTextOptions bits;
        #endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
            var textOptions = new StiTextOptions();

            if (bits != null)
                textOptions.bits = bits.Clone() as BitsTextOptions;

            textOptions.WordWrap = WordWrap;

            return textOptions;
		}
		#endregion

        #region IEquatable
	    private bool Equals(StiTextOptions other)
	    {
	        return Equals(bits, other.bits) && WordWrap.Equals(other.WordWrap);
	    }

	    public override bool Equals(object obj)
	    {
	        return obj != null && (this == obj || obj is StiTextOptions && Equals((StiTextOptions) obj));
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            return ((("StiTextOptions".GetHashCode() * 397) ^ (bits != null ? bits.GetHashCode() : 0)) *397) ^ (WordWrap ? 1231 : 1237);
	        }
	    }
        #endregion

        #region Methods
        public StringFormat GetStringFormat()
		{
			return GetStringFormat(false, 1);
		}

		public StringFormat GetStringFormat(bool antialiasing, float zoom)
		{
		    var stringFormat = antialiasing 
		        ? new StringFormat(StringFormat.GenericTypographic) 
		        : new StringFormat();

			stringFormat.FormatFlags = 0;
			if (!WordWrap)
			    stringFormat.FormatFlags = StringFormatFlags.NoWrap;

			if (RightToLeft)
			    stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

			if (LineLimit)
			    stringFormat.FormatFlags |= StringFormatFlags.LineLimit;						
			
			stringFormat.Trimming = Trimming;
			stringFormat.HotkeyPrefix = HotkeyPrefix;
			stringFormat.SetTabStops(FirstTabOffset * zoom, new[]{ DistanceBetweenTabs * zoom });
			
			return stringFormat;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets horizontal output direction.
		/// </summary>
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets horizontal output direction.")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public bool RightToLeft
		{
            get
            {
                return bits != null && bits.RightToLeft;
            }
            set
            {
                if (value == false && bits == null)
                    return;

                if (bits != null)
                    bits.RightToLeft = value;
                else
                    bits = new BitsTextOptions(value, LineLimit, Angle,
                        FirstTabOffset, DistanceBetweenTabs, HotkeyPrefix, Trimming);
            }
		}

		/// <summary>
		/// Gets or sets value, that show compleleted lines only.
		/// </summary>
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value, that show compleleted lines only.")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public bool LineLimit
		{
            get
            {
                return bits != null && bits.LineLimit;
            }
            set
            {
                if (value == false && bits == null)
                    return;
                if (bits != null)
                    bits.LineLimit = value;
                else
                    bits = new BitsTextOptions(RightToLeft, value, Angle,
                        FirstTabOffset, DistanceBetweenTabs, HotkeyPrefix, Trimming);
            }
		}

        /// <summary>
		/// Gets or sets word wrap.
		/// </summary>
		[DefaultValue(false)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[Browsable(false)]
		public bool WordWrap { get; set; }

        /// <summary>
		/// Gets or sets angle of a text rotation.
		/// </summary>
		[DefaultValue(0f)]
		[Description("Gets or sets angle of a text rotation.")]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Angle
		{
            get
            {
                return bits == null ? 0f : bits.Angle;
            }
            set
            {
                if (value == 0f && bits == null)
                    return;

                if (bits != null)
                    bits.Angle = value;
                else
                    bits = new BitsTextOptions(RightToLeft, LineLimit, value,
                        FirstTabOffset, DistanceBetweenTabs, HotkeyPrefix, Trimming);
            }
		}
		
		/// <summary>
		/// Gets or sets first tab offset.
		/// </summary>
		[DefaultValue(40f)]
		[Description("Gets or sets first tab offset.")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float FirstTabOffset
		{
            get
            {
                return bits == null ? 40f : bits.FirstTabOffset;
            }
            set
            {
                if (value < 0) return;
                if (value == 40f && bits == null)
                    return;

                if (bits != null)
                    bits.FirstTabOffset = value;
                else
                    bits = new BitsTextOptions(RightToLeft, LineLimit, Angle,
                        value, DistanceBetweenTabs, HotkeyPrefix, Trimming);
            }
		}
		
		/// <summary>
		/// Gets or sets distance between tabs.
		/// </summary>
		[DefaultValue(20f)]
		[Description("Gets or sets distance between tabs.")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float DistanceBetweenTabs
		{
            get
            {
                return bits == null ? 20f : bits.DistanceBetweenTabs;
            }
            set
            {
                if (value < 0) return;
                if (value == 20f && bits == null)
                    return;
                if (bits != null)
                    bits.DistanceBetweenTabs = value;
                else
                    bits = new BitsTextOptions(RightToLeft, LineLimit, Angle,
                        FirstTabOffset, value, HotkeyPrefix, Trimming);
            }
		}
		
		/// <summary>
		/// Gets or sets type of drawing hot keys.
		/// </summary>
		[DefaultValue(HotkeyPrefix.None)]
		[TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets type of drawing hot keys.")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public HotkeyPrefix HotkeyPrefix
		{
            get
            {
                return bits == null ? HotkeyPrefix.None : bits.HotkeyPrefix;
            }
            set
            {
                if (value == HotkeyPrefix.None && bits == null)
                    return;
                if (bits != null)
                    bits.HotkeyPrefix = value;
                else
                    bits = new BitsTextOptions(RightToLeft, LineLimit, Angle,
                        FirstTabOffset, DistanceBetweenTabs, value, Trimming);
            }
		}
		
		/// <summary>
		/// Gets or sets type to trim the end of a line.
		/// </summary>
		[DefaultValue(StringTrimming.None)]
		[TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [Description("Gets or sets type to trim the end of a line.")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public StringTrimming Trimming
		{
            get
            {
                return bits == null ? StringTrimming.None : bits.Trimming;
            }
            set
            {
                if (value == StringTrimming.None && bits == null)
                    return;
                if (bits != null)
                    bits.Trimming = value;
                else
                    bits = new BitsTextOptions(RightToLeft, LineLimit, Angle,
                        FirstTabOffset, DistanceBetweenTabs, HotkeyPrefix, value);
            }
		}

		[Browsable(false)]
		public bool IsDefault => !WordWrap && bits == null;
        #endregion

		/// <summary>
		/// Creates a new object of the type StiTextOptions.
		/// </summary>
		public StiTextOptions() : 
			this(false, false, false, 0, HotkeyPrefix.None, StringTrimming.None)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiTextOptions.
		/// </summary>
		/// <param name="rightToLeft">Horizontal output direction.</param>
		/// <param name="lineLimit">Show completed lines only.</param>
		/// <param name="wordWrap">Word wrap.</param>
		/// <param name="angle">Angle of a text rotation.</param>
		/// <param name="hotkeyPrefix">Type to draw hot keys.</param>
		/// <param name="trimming">Type to trim the end of a line.</param>
		public StiTextOptions(bool rightToLeft, bool lineLimit, bool wordWrap,
			float angle, HotkeyPrefix hotkeyPrefix, StringTrimming trimming) : 
			this(rightToLeft, lineLimit, wordWrap, angle, hotkeyPrefix, trimming,
			40f, 20f)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiTextOptions.
		/// </summary>
		/// <param name="rightToLeft">Horizontal output direction.</param>
		/// <param name="lineLimit">Show completed lines only.</param>
		/// <param name="wordWrap">Word wrap.</param>
		/// <param name="angle">Angle of a text rotation.</param>
		/// <param name="hotkeyPrefix">Type to draw hot keys.</param>
		/// <param name="trimming">Type to trim the end of a line.</param>
		/// <param name="firstTabOffset">First tab offset.</param>
		/// <param name="distanceBetweenTabs">Distance between tabs.</param>
		public StiTextOptions(bool rightToLeft, bool lineLimit, bool wordWrap,
			float angle, HotkeyPrefix hotkeyPrefix, StringTrimming trimming,
			float firstTabOffset, float distanceBetweenTabs)
		{
            WordWrap = wordWrap;

            if (rightToLeft == false &&
                lineLimit == false &&
                angle == 0f &&
                hotkeyPrefix == HotkeyPrefix.None &&
                trimming == StringTrimming.None &&
                firstTabOffset == 40f &&
                distanceBetweenTabs == 20f)
            {
                bits = null;
            }
            else
            {
                bits = new BitsTextOptions(rightToLeft, lineLimit, angle, firstTabOffset, distanceBetweenTabs, hotkeyPrefix, trimming);
            }
		}
    }
}
