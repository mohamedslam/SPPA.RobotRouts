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
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base.Drawing.Design;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class describes a border.
    /// </summary>
    [Editor("Stimulsoft.Base.Drawing.Design.StiSimpleBorderEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
    [TypeConverter(typeof(StiSimpleBorderConverter))]
    [RefreshProperties(RefreshProperties.All)]
    [JsonConverter(typeof(StiSimpleBorderJsonConverter))]
    [StiReferenceIgnore]
    [JsonObject]
    public class StiSimpleBorder : ICloneable
    {
        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            return (StiSimpleBorder)MemberwiseClone();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Tests to see whether the specified object is a StiSimpleBorder with the same dimensions as this StiSimpleBorder.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is a StiSimpleBorder and has the same width and height as this StiSimpleBorder; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var border = (StiSimpleBorder)obj;
            return border != null
                   && border.Color == Color
                   && border.Side == Side
                   && border.Size == Size
                   && border.Style == Style;
        }

        /// <summary>
		/// Returns a hash code for this StiPaddings structure.
		/// </summary>
		/// <returns>An integer value that specifies a hash value for this StiPaddings structure.</returns>
		public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public StiBorder GetBorder()
        {
            return new StiBorder(Side, Color, Size, Style);
        }

        public float GetSizeOffset()
        {
            if (Style == StiPenStyle.None)
                return 0f;

            if (Style == StiPenStyle.Double)
                return 1f;

            return (float)Size / 2;
        }

        internal float GetSize()
        {
            if (Style == StiPenStyle.None)
                return 0f;

            if (Style == StiPenStyle.Double)
                return 3f;

            return (float)Size;
        }

        internal float GetSizeIncludingSide()
        {
            if (Style == StiPenStyle.None)
                return 0f;

            if (Style == StiPenStyle.Double)
                return 3f;

            if (Side == StiBorderSides.None)
                return 0f;

            return (float)Size;
        }

        /// <summary>
        /// Draws this border on the indicated Graphics.
        /// </summary>
        /// <param name="g">Graphics on which a border can be drawn.</param>
        /// <param name="rect">The rectangle that indicates an area of the border drawing.</param>
        /// <param name="zoom">The scale of a border to draw.</param>
        public void Draw(Graphics g, RectangleD rect, double zoom)
        {
            Draw(g, rect, zoom, Color.White);
        }

        /// <summary>
        /// Draws this border on the indicated Graphics.
        /// </summary>
        /// <param name="g">Graphics on which a border can be drawn.</param>
        /// <param name="rect">The rectangle that indicates an area of the border drawing.</param>
        /// <param name="zoom">The scale of a border to draw.</param>
        /// <param name="emptyColor">The color of space between double lines (used only when border style equal Double).</param>
        public void Draw(Graphics g, RectangleD rect, double zoom, Color emptyColor)
        {
            Draw(g, rect.ToRectangleF(), (float)zoom, emptyColor);
        }

        /// <summary>
        /// Draws this border on the indicated Graphics.
        /// </summary>
        /// <param name="g">Graphics on which a border can be drawn.</param>
        /// <param name="rect">The rectangle that indicates an area of the border drawing.</param>
        /// <param name="zoom">The scale of a border to draw.</param>
        public void Draw(Graphics g, RectangleF rect, float zoom)
        {
            Draw(g, rect, zoom, Color.White);
        }

        /// <summary>
        /// Draws this border on the indicated Graphics.
        /// </summary>
        /// <param name="g">Graphics on which a border can be drawn.</param>
        /// <param name="rect">The rectangle that indicates an area of the border drawing.</param>
        /// <param name="zoom">The scale of a border to draw.</param>
        /// <param name="emptyColor">The color of space between double lines (used only when border style equal Double).</param>
        public virtual void Draw(Graphics g, RectangleF rect, float zoom, Color emptyColor)
        {
            Draw(g, rect, zoom, emptyColor, true, true);
        }

        /// <summary>
        /// Draws this border on the indicated Graphics.
        /// </summary>
        /// <param name="g">Graphics on which a border can be drawn.</param>
        /// <param name="rect">The rectangle that indicates an area of the border drawing.</param>
        /// <param name="zoom">The scale of a border to draw.</param>
        /// <param name="emptyColor">The color of space between double lines (used only when border style equal Double).</param>
        public virtual void Draw(Graphics g, RectangleF rect, float zoom, Color emptyColor, bool drawBorderFormatting, bool drawBorderSides)
        {
            if (IsDefault) return;

            if (!drawBorderSides) return;

            Pen emptyPen = null;

            using (var pen = new Pen(Color))
            {
                if (Style == StiPenStyle.Double)
                    emptyPen = new Pen(emptyColor);

                pen.DashStyle = StiPenUtils.GetPenStyle(Style);

                #region Painting
                if (Style != StiPenStyle.None)
                {
                    pen.Width = (int)(Size * zoom);
                    pen.StartCap = LineCap.Square;
                    pen.EndCap = LineCap.Square;

                    var rectIn = rect;
                    var rectOut = rect;

                    if (Style == StiPenStyle.Double)
                    {
                        rectIn.Inflate(-1, -1);
                        rectOut.Inflate(1, 1);
                        pen.Width = 1;
                    }

                    #region All border sides
                    if (IsAllBorderSidesPresent)
                    {
                        if (Style == StiPenStyle.Double)
                        {
                            g.DrawRectangle(emptyPen, rect.X, rect.Y, rect.Width, rect.Height);
                            g.DrawRectangle(pen, rectIn.X, rectIn.Y, rectIn.Width, rectIn.Height);
                            g.DrawRectangle(pen, rectOut.X, rectOut.Y, rectOut.Width, rectOut.Height);
                        }
                        else
                            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                    #endregion

                    else
                    {
                        #region Top border side
                        var left = 0f;
                        var right = 0f;
                        if (IsTopBorderSidePresent)
                        {
                            left = rectIn.Left;
                            right = rectIn.Right;

                            if (!IsLeftBorderSidePresent) left = rectOut.Left;
                            if (!IsRightBorderSidePresent) right = rectOut.Right;

                            if (Style == StiPenStyle.Double)
                            {
                                g.DrawLine(emptyPen, rect.Left, rect.Top, rect.Right, rect.Top);
                                g.DrawLine(pen, left, rectIn.Top, right, rectIn.Top);
                                g.DrawLine(pen, rectOut.Left, rectOut.Top, rectOut.Right, rectOut.Top);
                            }
                            else
                                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                        }
                        #endregion

                        #region Left border side
                        var top = 0f;
                        var bottom = 0f;
                        if (IsLeftBorderSidePresent)
                        {
                            top = rectIn.Top;
                            bottom = rectIn.Bottom;

                            if (!IsTopBorderSidePresent) top = rectOut.Top;
                            if (!IsBottomBorderSidePresent) bottom = rectOut.Bottom;

                            if (Style == StiPenStyle.Double)
                            {
                                g.DrawLine(emptyPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                                g.DrawLine(pen, rectIn.Left, top, rectIn.Left, bottom);
                                g.DrawLine(pen, rectOut.Left, rectOut.Top, rectOut.Left, rectOut.Bottom);
                            }
                            else
                                g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                        }
                        #endregion

                        #region Bottom border side
                        if (IsBottomBorderSidePresent)
                        {
                            left = rectIn.Left;
                            right = rectIn.Right;

                            if (!IsLeftBorderSidePresent) left = rectOut.Left;
                            if (!IsRightBorderSidePresent) right = rectOut.Right;

                            if (Style == StiPenStyle.Double)
                            {
                                g.DrawLine(emptyPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                                g.DrawLine(pen, left, rectIn.Bottom, right, rectIn.Bottom);
                                g.DrawLine(pen, rectOut.Left, rectOut.Bottom, rectOut.Right, rectOut.Bottom);
                            }
                            else
                                g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                        }
                        #endregion

                        #region Right border side
                        if (IsRightBorderSidePresent)
                        {
                            top = rectIn.Top;
                            bottom = rectIn.Bottom;

                            if (!IsTopBorderSidePresent) top = rectOut.Top;
                            if (!IsBottomBorderSidePresent) bottom = rectOut.Bottom;

                            if (Style == StiPenStyle.Double)
                            {
                                g.DrawLine(emptyPen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                                g.DrawLine(pen, rectIn.Right, top, rectIn.Right, bottom);
                                g.DrawLine(pen, rectOut.Right, rectOut.Top, rectOut.Right, rectOut.Bottom);
                            }
                            else
                                g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                        }
                        #endregion
                    }

                    if (emptyPen != null)
                        emptyPen.Dispose();
                }
                #endregion
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets value which indicates that top side of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsTopBorderSidePresent => (Side & StiBorderSides.Top) != 0;

        /// <summary>
        /// Gets value which indicates that bottom side of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsBottomBorderSidePresent => (Side & StiBorderSides.Bottom) != 0;

        /// <summary>
        /// Gets value which indicates that left side of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsLeftBorderSidePresent => (Side & StiBorderSides.Left) != 0;

        /// <summary>
        /// Gets value which indicates that right side of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsRightBorderSidePresent => (Side & StiBorderSides.Right) != 0;

        /// <summary>
        /// Gets value which indicates that all sides of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsAllBorderSidesPresent => Side == StiBorderSides.All;

        /// <summary>
        /// Gets value which indicates that none of sides of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsNoneBorderSidesPresent => Side == StiBorderSides.None;

        /// <summary>
        /// Gets or sets frame borders.
        /// </summary>
        [Editor("Stimulsoft.Base.Drawing.Design.StiBorderSidesEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiSerializable]
        [DefaultValue(StiBorderSides.None)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Description("Gets or sets frame borders.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(300)]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual StiBorderSides Side { get; set; } = StiBorderSides.None;

        /// <summary>
        /// Gets or sets a border color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets a border color.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(100)]
        public virtual Color Color { get; set; } = Color.Gray;

        private bool ShouldSerializeColor()
        {
            return Color != Color.Gray;
        }

        /// <summary>
        /// Gets or sets a border size.
        /// </summary>
        [StiSerializable]
        [DefaultValue(1d)]
        [Description("Gets or sets a border size.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(400)]
        public virtual double Size { get; set; } = 1d;

        /// <summary>
        /// Gets or sets a border style.
        /// </summary>
        [Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
        [StiSerializable]
        [DefaultValue(StiPenStyle.Solid)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Description("Gets or sets a border style.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(500)]
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual StiPenStyle Style { get; set; }

        /// <summary>
        /// Gets value indicates, that this object-frame is by default.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsDefault => 
            Side == StiBorderSides.None && 
            Color == Color.Gray && 
            Size == 1d && 
            Style == StiPenStyle.Solid;
        #endregion

        #region Methods.Json
        public void LoadFromJson(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Side":
                        Side = property.DeserializeEnum<StiBorderSides>();
                        break;

                    case "Color":
                        Color = property.DeserializeColor();
                        break;

                    case "Size":
                        Size = property.DeserializeDouble();
                        break;

                    case "Style":
                        Style = property.DeserializeEnum<StiPenStyle>(); 
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the StiSimpleBorder class.
        /// </summary>
        public StiSimpleBorder() 
            : this(StiBorderSides.None, Color.Gray, 1d, StiPenStyle.Solid)
        {
        }

        /// <summary>
        /// Creates a new instance of the StiSimpleBorder class.
        /// </summary>
        /// <param name="side">Border sides.</param>
        /// <param name="color">Border color.</param>
        /// <param name="size">Border size.</param>
        /// <param name="style">Border style.</param>
        public StiSimpleBorder(StiBorderSides side, Color color, double size, StiPenStyle style)
        {
            this.Side = side;
            this.Color = color;
            this.Size = size;
            this.Style = style;
        }
    }
}
