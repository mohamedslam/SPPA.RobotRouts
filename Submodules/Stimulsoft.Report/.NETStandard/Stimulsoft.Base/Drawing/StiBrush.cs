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

using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pens = Stimulsoft.Drawing.Pens;
using Brush = Stimulsoft.Drawing.Brush;
using LinearGradientBrush = Stimulsoft.Drawing.Drawing2D.LinearGradientBrush;
using HatchBrush = Stimulsoft.Drawing.Drawing2D.HatchBrush;
using TextureBrush = Stimulsoft.Drawing.TextureBrush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class describes a brush.
    /// </summary>
    [TypeConverter(typeof(StiBrushConverter))]
    [RefreshProperties(RefreshProperties.All)]
    [Editor("Stimulsoft.Base.Drawing.Design.StiBrushEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
    [JsonConverter(typeof(StiBrushJsonConverter))]
    [StiReferenceIgnore]
    [JsonObject]
    public abstract class StiBrush : ICloneable
    {
        #region ICloneable
        /// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IEquatable
        public override bool Equals(object obj)
        {
            return obj != null && this == obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public abstract StiBrushIdent Ident { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns true if this brush is empty - brush is null or StiEmptyBrush.
        /// </summary>
        public static bool IsEmpty(StiBrush brush)
        {
            return brush == null || brush is StiEmptyBrush;
        }

        /// <summary>
        /// Returns true if this brush is transparent - brush is null, empty or solid and color is transparent.
        /// </summary>
        public static bool IsTransparent(StiBrush brush)
        {
            return IsEmpty(brush) || (
                brush is StiSolidBrush 
				&& (((StiSolidBrush)brush).Color == Color.Transparent || ((StiSolidBrush)brush).Color.A == 0)
            );
        }

        public Image GetImage(int width, int height)
        {
            var image = new Bitmap(width, height);
            using (var g = Graphics.FromImage(image))
            {
                if (this is StiEmptyBrush)
                {
                    g.Clear(Color.White);
                    g.DrawRectangle(Pens.DarkGray, 0, 0, width - 1, height - 1);
                }
                else
                {
                    var rect = new Rectangle(0, 0, width, height);

                    using (var brush = GetBrush(this, rect))
                        g.FillRectangle(brush, rect);
                }
            }
            return image;
        }

        public static StiBrush Light(StiBrush baseBrush, byte value)
		{
		    if (baseBrush is StiSolidBrush)
		        return new StiSolidBrush(StiColorUtils.Light(((StiSolidBrush) baseBrush).Color, value));

		    if (baseBrush is StiGradientBrush)
		    {
		        var gradientBrush = baseBrush as StiGradientBrush;
		        return new StiGradientBrush(
		            StiColorUtils.Light(gradientBrush.StartColor, value),
		            StiColorUtils.Light(gradientBrush.EndColor, value),
		            gradientBrush.Angle);
		    }

		    if (baseBrush is StiHatchBrush)
		    {
		        var hatchBrush = baseBrush as StiHatchBrush;
		        return new StiHatchBrush(
		            hatchBrush.Style,
		            StiColorUtils.Light(hatchBrush.ForeColor, value),
		            StiColorUtils.Light(hatchBrush.BackColor, value));
		    }

		    if (baseBrush is StiGlareBrush)
		    {
		        var glareBrush = baseBrush as StiGlareBrush;
		        return new StiGlareBrush(
		            StiColorUtils.Light(glareBrush.StartColor, value),
		            StiColorUtils.Light(glareBrush.EndColor, value),
		            glareBrush.Angle);
		    }

		    if (baseBrush is StiGlassBrush)
		    {
		        var glassBrush = baseBrush as StiGlassBrush;
		        return new StiGlassBrush(
		            StiColorUtils.Light(glassBrush.Color, value),
		            glassBrush.DrawHatch,
		            glassBrush.Blend);
		    }

		    return baseBrush;
		}


		public static StiBrush Dark(StiBrush baseBrush, byte value)
		{
		    if (baseBrush is StiSolidBrush)
		        return new StiSolidBrush(StiColorUtils.Dark(((StiSolidBrush) baseBrush).Color, value));

		    if (baseBrush is StiGradientBrush)
		    {
		        var gradientBrush = baseBrush as StiGradientBrush;
		        return new StiGradientBrush(
		            StiColorUtils.Dark(gradientBrush.StartColor, value),
		            StiColorUtils.Dark(gradientBrush.EndColor, value),
		            gradientBrush.Angle);
		    }

		    if (baseBrush is StiHatchBrush)
		    {
		        var hatchBrush = baseBrush as StiHatchBrush;
		        return new StiHatchBrush(
		            hatchBrush.Style,
		            StiColorUtils.Dark(hatchBrush.ForeColor, value),
		            StiColorUtils.Dark(hatchBrush.BackColor, value));
		    }

		    if (baseBrush is StiGlareBrush)
		    {
		        var glareBrush = baseBrush as StiGlareBrush;
		        return new StiGlareBrush(
		            StiColorUtils.Dark(glareBrush.StartColor, value),
		            StiColorUtils.Dark(glareBrush.EndColor, value),
		            glareBrush.Angle);
		    }

		    if (baseBrush is StiGlassBrush)
		    {
		        var glassBrush = baseBrush as StiGlassBrush;
		        return new StiGlassBrush(
		            StiColorUtils.Dark(glassBrush.Color, value),
		            glassBrush.DrawHatch,
		            glassBrush.Blend);
		    }

		    return baseBrush;
		}

        /// <summary>
        /// Returns the gdi brush from the report brush.
        /// </summary>
        /// <param name="brush">Report brush.</param>
        /// <param name="rect">Rectangle for gradient.</param>
        /// <returns>Gdi brush.</returns>
        public static Brush GetBrush(StiBrush brush, Rectangle rect)
        {
            return GetBrush(brush, RectangleD.CreateFromRectangle(rect));
        }

		/// <summary>
		/// Returns the gdi brush from the report brush.
		/// </summary>
		/// <param name="brush">Report brush.</param>
		/// <param name="rect">Rectangle for gradient.</param>
		/// <returns>Gdi brush.</returns>
		public static Brush GetBrush(StiBrush brush, RectangleF rect)
		{
			return GetBrush(brush, RectangleD.CreateFromRectangle(rect));
		}

		/// <summary>
		/// Returns the standard brush from the report brush.
		/// </summary>
		/// <param name="brush">Report brush.</param>
		/// <param name="rect">Rectangle for gradient.</param>
		/// <returns>Gdi brush.</returns>
		public static Brush GetBrush(StiBrush brush, RectangleD rect)
		{
		    if (brush is StiEmptyBrush || brush is StiStyleBrush || brush is StiDefaultBrush)
		        return new SolidBrush(Color.Transparent);

		    if (brush is StiSolidBrush)
		        return new SolidBrush(((StiSolidBrush) brush).Color);

		    if (brush is StiGradientBrush)
		    {
		        var rectF = rect.ToRectangleF();
		        if (rectF.Width < 1) rectF.Width = 1;
		        if (rectF.Height < 1) rectF.Height = 1;

		        var gradientBrush = brush as StiGradientBrush;

                if (gradientBrush.Angle == 0)
                    return new LinearGradientBrush(
                        new PointF(rectF.Left - 1, rectF.Top - 1),
                        new PointF(rectF.Right + 1, rectF.Bottom + 1),
                        gradientBrush.StartColor,
                        gradientBrush.EndColor);
                if (gradientBrush.Angle == 180)
                    return new LinearGradientBrush(
                        new PointF(rectF.Left - 1, rectF.Top - 1),
                        new PointF(rectF.Right + 1, rectF.Bottom + 1),
                        gradientBrush.EndColor,
                        gradientBrush.StartColor);
                return new LinearGradientBrush(
                    rectF,
                    gradientBrush.StartColor,
                    gradientBrush.EndColor,
                    (float)gradientBrush.Angle);
            }

		    if (brush is StiHatchBrush)
		    {
		        var hatchBrush = brush as StiHatchBrush;
		        return new HatchBrush(hatchBrush.Style,
		            hatchBrush.ForeColor, hatchBrush.BackColor);
		    }

		    if (brush is StiGlareBrush)
		    {
		        var rectF = rect.ToRectangleF();
		        if (rectF.Width < 1) rectF.Width = 1;
		        if (rectF.Height < 1) rectF.Height = 1;

		        var glareBrush = brush as StiGlareBrush;
		        var br = new LinearGradientBrush(rectF,
		            glareBrush.StartColor, glareBrush.EndColor, (float) glareBrush.Angle);

		        br.SetSigmaBellShape(glareBrush.Focus, glareBrush.Scale);
		        return br;
		    }

		    if (brush is StiGlassBrush)
		    {
		        var bmp = new Bitmap((int) rect.Width + 1, (int) rect.Height + 1);
		        using (var gg = Graphics.FromImage(bmp))
		        {
		            ((StiGlassBrush) brush).Draw(gg, new RectangleF(0, 0, (float) rect.Width + 1, (float) rect.Height + 1));
		        }

		        var textureBrush = new TextureBrush(bmp);
		        textureBrush.TranslateTransform((float) rect.X, (float) rect.Y);
		        return textureBrush;
		    }

		    return null;
		}

        public static StiBrush LoadFromJson(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident");

            switch (ident.Value.ToObject<string>())
            {
				case "StiStyleBrush":
					return new StiStyleBrush();

				case "StiDefaultBrush":
					return new StiDefaultBrush();

				case "StiEmptyBrush":
                    return new StiEmptyBrush();

                case "StiSolidBrush":
                    var solid = new StiSolidBrush();
                    solid.LoadValuesFromJson(jObject);
                    return solid;

                case "StiGradientBrush":
                    var gradient = new StiGradientBrush();
                    gradient.LoadValuesFromJson(jObject);
                    return gradient;

                case "StiGlareBrush":
                    var glare = new StiGlareBrush();
                    glare.LoadValuesFromJson(jObject);
                    return glare;

                case "StiGlassBrush":
                    var glass = new StiGlassBrush();
                    glass.LoadValuesFromJson(jObject);
                    return glass;

                case "StiHatchBrush":
                    var hatch = new StiHatchBrush();
                    hatch.LoadValuesFromJson(jObject);
                    return hatch;
            }

            throw new Exception("Type is not supported!");
        }
		
		/// <summary>
		/// Transform a brush into a color.
		/// </summary>
		/// <param name="brush">Brush for converting.</param>
		/// <returns>Converted color.</returns>
		public static Color ToColor(StiBrush brush)
		{
			if (brush is StiStyleBrush)
				return Color.White;
			
			if (brush is StiDefaultBrush)
				return Color.White;

			if (brush is StiEmptyBrush)
			    return Color.Transparent;

			if (brush is StiSolidBrush)
			    return ((StiSolidBrush)brush).Color;

			if (brush is StiGradientBrush)
			    return ((StiGradientBrush)brush).StartColor;

			if (brush is StiGlareBrush)
			    return ((StiGlareBrush)brush).StartColor;

            if (brush is StiGlassBrush)
                return ((StiGlassBrush)brush).Color;

			if (brush is StiHatchBrush)
			    return ((StiHatchBrush)brush).ForeColor;

			return Color.Empty;
		}
		#endregion
    }
}