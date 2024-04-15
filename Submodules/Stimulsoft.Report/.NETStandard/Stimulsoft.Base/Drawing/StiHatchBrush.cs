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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class describes the HatchBrush.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
	[JsonObject]
	public class StiHatchBrush : StiBrush
	{
		#region Properties
	    /// <summary>
		/// Gets the color of spaces between the hatch lines drawn by this StiHatchBrush object.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets the color of spaces between the hatch lines drawn by this StiHatchBrush object.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(200)]
		public Color BackColor { get; set; }

	    private bool ShouldSerializeBackColor()
	    {
	        return BackColor != Color.White;
	    }

        /// <summary>
        /// Gets the color of hatch lines drawn by this StiHatchBrush object.
        /// </summary>
        [StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets the color of hatch lines drawn by this StiHatchBrush object.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(100)]
		public Color ForeColor { get; set; }

	    private bool ShouldSerializeForeColor()
	    {
	        return ForeColor != Color.Black;
	    }

        /// <summary>
        /// Gets the hatch style of this StiHatchBrush object.
        /// </summary>
        [Editor("Stimulsoft.Base.Drawing.Design.StiHatchStyleEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiHatchStyleConverter))]
		[Description("Gets the hatch style of this StiHatchBrush object.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(300)]
		[DefaultValue(HatchStyle.BackwardDiagonal)]
		public HatchStyle Style { get; set; }
        #endregion

        #region StiBrush.override
        public override StiBrushIdent Ident => StiBrushIdent.Hatch;
	    #endregion

        #region IEquatable
        protected bool Equals(StiHatchBrush other)
	    {
	        return BackColor.Equals(other.BackColor) && ForeColor.Equals(other.ForeColor) && Style == other.Style;
	    }

	    public override bool Equals(object obj)
	    {
	        return obj != null && (this == obj || obj.GetType() == GetType() && Equals((StiHatchBrush) obj));
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
                var hashCode = "StiHatchBrush".GetHashCode();
	            hashCode = (hashCode*397) ^ BackColor.GetHashCode();
	            hashCode = (hashCode*397) ^ ForeColor.GetHashCode();
	            hashCode = (hashCode*397) ^ (int) Style;
	            return hashCode;
	        }
	    }
	    #endregion

		#region Methods
        public void LoadValuesFromJson(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "BackColor":
                        this.BackColor = property.DeserializeColor();
                        break;

                    case "ForeColor":
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case "Style":
                        this.Style = property.DeserializeEnum<HatchStyle>();
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the StiHatchBrush class.
        /// </summary>
        public StiHatchBrush()
		{
			Style = HatchStyle.BackwardDiagonal;			
			ForeColor = Color.Black;
			BackColor = Color.White;
		}

		/// <summary>
		/// Creates a new instance of the StiHatchBrush class.
		/// </summary>
		/// <param name="style">Hatch style of this StiHatchBrush object.</param>
		/// <param name="foreColor">The color of hatch lines drawn by this StiHatchBrush object.</param>
		/// <param name="backColor">The color of spaces between the hatch lines drawn by this StiHatchBrush object.</param>
		public StiHatchBrush(HatchStyle style, Color foreColor, Color backColor)
		{
			this.Style = style;
			this.ForeColor = foreColor;
			this.BackColor = backColor;			
		}
	}
}
