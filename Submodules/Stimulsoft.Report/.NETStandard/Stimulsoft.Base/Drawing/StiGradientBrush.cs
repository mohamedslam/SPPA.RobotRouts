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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Class describes a gradient brush.
	/// </summary>
	[RefreshProperties(RefreshProperties.All)]
	[JsonObject]
	public class StiGradientBrush : StiBrush
	{
		#region Properties
	    /// <summary>
		/// Gets or sets the starting color for the gradient.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the starting color for the gradient.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(200)]
		public Color StartColor { get; set; }

	    private bool ShouldSerializeStartColor()
	    {
	        return StartColor != Color.White;
	    }

        /// <summary>
        /// Gets or sets the ending color for the gradient.
        /// </summary>
        [StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the ending color for the gradient.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(300)]
		public Color EndColor { get; set; }

	    private bool ShouldSerializeEndColor()
	    {
	        return EndColor != Color.Black;
	    }

        /// <summary>
        /// Gets or sets the angle, measured in degrees clockwise from the x-axis, of the gradient's orientation line. 
        /// </summary>
        [StiSerializable]
		[Description("Gets or sets the angle, measured in degrees clockwise from the x-axis, of the gradient's orientation line.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(100)]
		[DefaultValue(0d)]
		public double Angle { get; set; }
        #endregion

        #region StiBrush.Override
        public override StiBrushIdent Ident => StiBrushIdent.Gradient;
	    #endregion

        #region IEquatable
        protected bool Equals(StiGradientBrush other)
	    {
	        return StartColor.Equals(other.StartColor) && EndColor.Equals(other.EndColor) && Angle.Equals(other.Angle);
	    }

	    public override bool Equals(object obj)
	    {
	        return obj != null && (this == obj || obj.GetType() == GetType() && Equals((StiGradientBrush) obj));
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
                var hashCode = "StiGradientBrush".GetHashCode();
	            hashCode = (hashCode*397) ^ StartColor.GetHashCode();
	            hashCode = (hashCode*397) ^ EndColor.GetHashCode();
	            hashCode = (hashCode*397) ^ Angle.GetHashCode();
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
                    case "StartColor":
                        this.StartColor = property.DeserializeColor();
                        break;

                    case "EndColor":
                        this.EndColor = property.DeserializeColor();
                        break;

                    case "Angle":
                        this.Angle = property.DeserializeDouble();
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the StiGradientBrush class.
        /// </summary>
        public StiGradientBrush()
		{
			this.StartColor = Color.Black;
			this.EndColor = Color.White;
			this.Angle = 0;
		}

		/// <summary>
		/// Creates a new instance of the StiGradientBrush class.
		/// </summary>
		/// <param name="startColor">the starting color for the gradient.</param>
		/// <param name="endColor">The ending color for the gradient.</param>
		/// <param name="angle">The angle, measured in degrees clockwise from the x-axis, of the gradient's orientation line.</param>
		public StiGradientBrush(Color startColor, Color endColor, double angle)
		{
			this.StartColor = startColor;
			this.EndColor = endColor;
			this.Angle = angle;
		}
	}
}
