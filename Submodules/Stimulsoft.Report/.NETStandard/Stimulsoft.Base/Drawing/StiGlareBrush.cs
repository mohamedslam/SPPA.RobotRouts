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

using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class describes a glare gradient brush.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
	[JsonObject]
	public class StiGlareBrush : StiBrush
	{
		#region Properties
	    /// <summary>
		/// Gets or sets the starting color for the gradient.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the starting color for the gradient.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(200)]
		public Color StartColor { get; set; }

	    private bool ShouldSerializeStartColor()
	    {
	        return StartColor != Color.Black;
	    }

        /// <summary>
        /// Gets or sets the ending color for the gradient.
        /// </summary>
        [StiSerializable]
		[TypeConverter(typeof(StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets the ending color for the gradient.")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(300)]
		public Color EndColor { get; set; }

	    private bool ShouldSerializeEndColor()
	    {
	        return EndColor != Color.White;
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

        private float focus;
		/// <summary>
		/// Gets or sets value from 0 through 1 that specifies the center of the gradient (the point where the gradient is composed of only the ending color).
		/// </summary>
		[StiSerializable]
		[DefaultValue(0.5f)]
		[Description("Gets or sets value from 0 through 1 that specifies the center of the gradient (the point where the gradient is composed of only the ending color).")]
		[RefreshProperties(RefreshProperties.All)]
		[StiOrder(400)]
		public float Focus
		{
			get
			{
				return focus;
			}
			set
			{
			    if (focus == value) return;

			    if (value > 1f || value < 0)
			        throw new ArgumentException("Focus must be in range between 0 and 1!");
					
			    focus = value;
			}
		}


		private float scale;
		/// <summary>
		/// Gets or sets value from 0 through 1 that specifies how fast the colors falloff from the focus. 
		/// </summary>
		[StiSerializable]
		[Description("Gets or sets value from 0 through 1 that specifies how fast the colors falloff from the focus.")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(1f)]
		[StiOrder(500)]
        [StiGuiMode(StiGuiMode.Gdi)]
		public float Scale
		{
			get
			{
				return scale;
			}
			set
			{
			    if (scale == value) return;

			    if (value > 1f || value < 0)
			        throw new ArgumentException("Scale must be in range between 0 and 1!");

			    scale = value;
			}
		}
        #endregion

        #region StiBrush.Override
        public override StiBrushIdent Ident => StiBrushIdent.Glare;
	    #endregion

        #region IEquatable
        protected bool Equals(StiGlareBrush other)
	    {
	        return StartColor.Equals(other.StartColor) && EndColor.Equals(other.EndColor) && Angle.Equals(other.Angle) && focus.Equals(other.focus) && scale.Equals(other.scale);
	    }

	    public override bool Equals(object obj)
	    {
	        return obj != null && (this == obj || obj.GetType() == GetType() && Equals((StiGlareBrush) obj));
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
                var hashCode = "StiGlareBrush".GetHashCode();
	            hashCode = (hashCode*397) ^ StartColor.GetHashCode();
	            hashCode = (hashCode*397) ^ EndColor.GetHashCode();
	            hashCode = (hashCode*397) ^ Angle.GetHashCode();
	            hashCode = (hashCode*397) ^ focus.GetHashCode();
	            hashCode = (hashCode*397) ^ scale.GetHashCode();
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

                    case "Focus":
                        this.Focus = property.DeserializeFloat();
                        break;

                    case "Scale":
                        this.Scale = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the StiGlareBrush class.
        /// </summary>
        public StiGlareBrush() : this(Color.Black, Color.White, 0)
		{
		}

		/// <summary>
		/// Creates a new instance of the StiGlareBrush class.
		/// </summary>
		/// <param name="startColor">the starting color for the gradient.</param>
		/// <param name="endColor">The ending color for the gradient.</param>
		/// <param name="angle">The angle, measured in degrees clockwise from the x-axis, of the gradient's orientation line.</param>
		public StiGlareBrush(Color startColor, Color endColor, double angle) : this(startColor, endColor, angle, 0.5f, 1f)
		{
		}

		/// <summary>
		/// Creates a new instance of the StiGlareBrush class.
		/// </summary>
		/// <param name="startColor">the starting color for the gradient.</param>
		/// <param name="endColor">The ending color for the gradient.</param>
		/// <param name="angle">The angle, measured in degrees clockwise from the x-axis, of the gradient's orientation line.</param>
		/// <param name="focus">The value from 0 through 1 that specifies the center of the gradient (the point where the gradient is composed of only the ending color).</param>
		/// <param name="scale">The value from 0 through 1 that specifies how fast the colors falloff from the focus.</param>
		public StiGlareBrush(Color startColor, Color endColor, double angle, float focus, float scale)
		{
			this.StartColor = startColor;
			this.EndColor = endColor;
			this.Angle = angle;

			this.focus = focus;
			this.scale = scale;
		}
	}
}
