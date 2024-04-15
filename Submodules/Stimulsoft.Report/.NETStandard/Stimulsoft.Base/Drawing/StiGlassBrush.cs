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

using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using HatchBrush = Stimulsoft.Drawing.Drawing2D.HatchBrush;
#endif

namespace Stimulsoft.Base.Drawing
{
	/// <summary>
	/// Class describes GlassBrush.
	/// </summary>
	[RefreshProperties(RefreshProperties.All)]
	[JsonObject]
	public class StiGlassBrush : StiBrush
	{
		#region Properties
	    /// <summary>
        /// Gets or sets the color of this StiGlassBrush object.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("Gets or sets a color of this StiGlassBrush object.")]
		[RefreshProperties(RefreshProperties.All)]
		public Color Color { get; set; }

        private bool ShouldSerializeColor()
        {
            return Color != Color.Silver;
        }

        /// <summary>
        /// Gets or sets value which indicates draw hatch at background or not.
        /// </summary>
        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets a value which indicates draw hatch at background or not.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(190)]
        public virtual bool DrawHatch { get; set; } = true;

	    private float blend;
        /// <summary>
        /// Gets or sets blend factor.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.2f)]
        [Description("Gets or sets a blend factor.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(190)]
        public virtual float Blend
        {
            get
            {
                return blend;
            }
            set
            {
                if (blend == value) return;

                if (value >= 0 && value <= 1)
                    blend = value;
                else
                    throw new ArgumentOutOfRangeException("Value must be in range between 0 and 1.");
            }
        }
        #endregion

	    #region StiBrush.Override
        public override StiBrushIdent Ident => StiBrushIdent.Glass;
	    #endregion

        #region IEquatable
        protected bool Equals(StiGlassBrush other)
	    {
	        return Color.Equals(other.Color) && DrawHatch.Equals(other.DrawHatch) && blend.Equals(other.blend);
	    }

	    public override bool Equals(object obj)
	    {
	        return obj != null && (this == obj || obj.GetType() == GetType() && Equals((StiGlassBrush) obj));
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
                var hashCode = "StiGlassBrush".GetHashCode();
	            hashCode = (hashCode*397) ^ Color.GetHashCode();
	            hashCode = (hashCode*397) ^ DrawHatch.GetHashCode();
	            hashCode = (hashCode*397) ^ blend.GetHashCode();
	            return hashCode;
	        }
	    }
	    #endregion

		#region Methods
        public Color GetTopColor()
        {
            return StiColorUtils.Light(Color, (byte)(64 * Blend));
        }

        public Color GetTopColorLight()
        {
            return StiColorUtils.Light(StiColorUtils.Light(Color, (byte)(64 * Blend)), 5);
        }

        public Color GetBottomColor()
        {
            return this.Color;
        }

        public Color GetBottomColorLight()
        {
            return StiColorUtils.Light(GetBottomColor(), 2);
        }

        public Brush GetTopBrush()
        {
            if (this.DrawHatch)
                return new HatchBrush(HatchStyle.DarkDownwardDiagonal, GetTopColor(), GetTopColorLight());
            else
                return new SolidBrush(GetTopColor());
        }

        public Brush GetBottomBrush()
        {
            if (this.DrawHatch)
                return new HatchBrush(HatchStyle.DarkDownwardDiagonal, GetBottomColor(), GetBottomColorLight());
            else
                return new SolidBrush(GetBottomColor());
        }

        public RectangleF GetTopRectangle(RectangleF rect)
        {
            var rect1 = rect;

            rect1.Height /= 2;

            if (rect1.Height * 2 < rect.Height) rect1.Height++;

            return rect1;
        }

        public RectangleF GetBottomRectangle(RectangleF rect)
        {
            var rect1 = GetTopRectangle(rect);
            var rect2 = rect;

            rect2.Height = rect.Height - rect1.Height;
            rect2.Y = rect1.Bottom;

            return rect2;
        }

        public void Draw(Graphics g, RectangleF rect)
        {
            var rect1 = GetTopRectangle(rect);
            var rect2 = GetBottomRectangle(rect);

            using (var brush1 = GetTopBrush())
            using (var brush2 = GetBottomBrush())
            {
                g.FillRectangle(brush1, rect1);
                g.FillRectangle(brush2, rect2);
            }
        }

        public void LoadValuesFromJson(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Color":
                        this.Color = property.DeserializeColor();
                        break;

                    case "DrawHatch":
                        this.DrawHatch = property.DeserializeBool();
                        break;

                    case "Blend":
                        this.blend = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the StiGlassBrush class.
        /// </summary>
        public StiGlassBrush()
		{
			this.Color = Color.Silver;
            this.DrawHatch = true;
            this.blend = 0.2f;
		}

		/// <summary>
        /// Creates a new instance of the StiGlassBrush class.
		/// </summary>
        /// <param name="color">A color of this StiGlassBrush object.</param>
        public StiGlassBrush(Color color, bool drawHatch, float blend)
		{
			this.Color = color;
            this.DrawHatch = drawHatch;
            this.blend = blend;
		}
	}
}
