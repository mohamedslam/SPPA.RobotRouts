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
#else

#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class describes a border.
    /// </summary>
    [Editor("Stimulsoft.Base.Drawing.Design.StiBorderEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
    [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiBorderConverter))]
    [RefreshProperties(RefreshProperties.All)]
    [JsonConverter(typeof(StiBorderJsonConverter))]
    [StiReferenceIgnore]
    [JsonObject]
    public class StiBorder :
        ICloneable
    {
        #region bitsBorder
        protected class BitsBorder : ICloneable
        {
            #region ICloneable
            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>A new object that is a copy of this instance.</returns>
            public virtual object Clone()
            {
                return MemberwiseClone();
            }
            #endregion

            #region IEquatable
            protected bool Equals(BitsBorder other)
            {
                return Side == other.Side &&
                    Color.Equals(other.Color) &&
                    Size.Equals(other.Size) &&
                    Style == other.Style &&
                    ShadowSize.Equals(other.ShadowSize) &&
                    Topmost == other.Topmost &&
                    DropShadow == other.DropShadow;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) 
                    return false;

                if (ReferenceEquals(this, obj)) 
                    return true;

                var other = obj as BitsBorder;
                return other != null && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (int)Side;

                    hashCode = (hashCode * 397) ^ Color.GetHashCode();
                    hashCode = (hashCode * 397) ^ Size.GetHashCode();
                    hashCode = (hashCode * 397) ^ (int)Style;
                    hashCode = (hashCode * 397) ^ ShadowSize.GetHashCode();
                    hashCode = (hashCode * 397) ^ Topmost.GetHashCode();
                    hashCode = (hashCode * 397) ^ DropShadow.GetHashCode();

                    return hashCode;
                }
            }
            #endregion

            #region Methods.Static
            public static bool IsDefault(BitsBorder bits)
            {
                return IsDefault(bits.Side, bits.Color, bits.Size, bits.Style, bits.DropShadow, bits.ShadowSize, bits.Topmost);
            }

            public static bool IsDefault(StiBorderSides side, Color color, double size, StiPenStyle style, bool dropShadow, double shadowSize, bool topmost)
            {
                return color == Color.Black &&
                       !dropShadow &&
                       !topmost &&
                       shadowSize == 4d &&
                       side == StiBorderSides.None &&
                       size == 1d &&
                       style == StiPenStyle.Solid;
            }
            #endregion

            #region Properties
            public StiBorderSides Side { get; set; }

            public Color Color { get; set; }

            public double Size { get; set; }

            public StiPenStyle Style { get; set; }

            public double ShadowSize { get; set; }

            public bool DropShadow { get; set; }

            public bool Topmost { get; set; }
            #endregion

            public BitsBorder(StiBorderSides side, Color color, double size, StiPenStyle style, double shadowSize, bool dropShadow, bool topmost)
            {
                Side = side;
                Color = color;
                Size = size;
                Style = style;
                ShadowSize = shadowSize;
                DropShadow = dropShadow;
                Topmost = topmost;
            }
        }
        #endregion

        #region Fields
        protected BitsBorder bits;
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            var border = (StiBorder)MemberwiseClone();

            if (border.ShadowBrush != null)
                border.ShadowBrush = (StiBrush)ShadowBrush.Clone();
            else
                border.ShadowBrush = null;

            if (bits != null)
            {
                if (!BitsBorder.IsDefault(bits))
                    border.bits = bits.Clone() as BitsBorder;
                else
                    border.bits = null;
            }

            return border;
        }
        #endregion

        #region IEquatable
        protected bool Equals(StiBorder other)
        {
            return Equals(bits, other.bits) && Equals(ShadowBrush, other.ShadowBrush);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;

            if (ReferenceEquals(this, obj)) 
                return true;

            var other = obj as StiBorder;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (((defaultHashCode * 397) ^ (bits != null ? bits.GetHashCode() : 0)) * 397) ^ (ShadowBrush != null ? ShadowBrush.GetHashCode() : 0);
            }
        }
        private static int defaultHashCode = "StiBorder".GetHashCode();
        #endregion

        #region Methods
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
            if (bits == null) return;

            if (IsDefault) return;

            if (drawBorderFormatting)
                DrawBorderShadow(g, rect, zoom);

            if (drawBorderSides)
            {
                Pen emptyPen = null;
                using (var pen = new Pen(bits.Color))
                {
                    if (bits.Style == StiPenStyle.Double)
                        emptyPen = new Pen(emptyColor);

                    pen.DashStyle = StiPenUtils.GetPenStyle(bits.Style);

                    #region Painting
                    if (bits.Style != StiPenStyle.None)
                    {
                        pen.Width = (int)(bits.Size * zoom);
                        pen.StartCap = LineCap.Square;
                        pen.EndCap = LineCap.Square;

                        var rectIn = rect;
                        var rectOut = rect;

                        if (bits.Style == StiPenStyle.Double)
                        {
                            rectIn.Inflate(-1, -1);
                            rectOut.Inflate(1, 1);
                            pen.Width = 1;
                        }

                        #region All border sides
                        if (IsAllBorderSidesPresent)
                        {
                            if (bits.Style == StiPenStyle.Double)
                            {
                                g.DrawRectangle(emptyPen, rect.X, rect.Y, rect.Width, rect.Height);
                                g.DrawRectangle(pen, rectIn.X, rectIn.Y, rectIn.Width, rectIn.Height);
                                g.DrawRectangle(pen, rectOut.X, rectOut.Y, rectOut.Width, rectOut.Height);
                            }
                            else
                            {
                                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                            }
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

                                if (!IsLeftBorderSidePresent) 
                                    left = rectOut.Left;

                                if (!IsRightBorderSidePresent) 
                                    right = rectOut.Right;

                                if (bits.Style == StiPenStyle.Double)
                                {
                                    g.DrawLine(emptyPen, rect.Left, rect.Top, rect.Right, rect.Top);
                                    g.DrawLine(pen, left, rectIn.Top, right, rectIn.Top);
                                    g.DrawLine(pen, rectOut.Left, rectOut.Top, rectOut.Right, rectOut.Top);
                                }
                                else
                                {
                                    g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                                }
                            }
                            #endregion

                            #region Left border side
                            var top = 0f;
                            var bottom = 0f;
                            if (IsLeftBorderSidePresent)
                            {
                                top = rectIn.Top;
                                bottom = rectIn.Bottom;

                                if (!IsTopBorderSidePresent) 
                                    top = rectOut.Top;

                                if (!IsBottomBorderSidePresent) 
                                    bottom = rectOut.Bottom;

                                if (Style == StiPenStyle.Double)
                                {
                                    g.DrawLine(emptyPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                                    g.DrawLine(pen, rectIn.Left, top, rectIn.Left, bottom);
                                    g.DrawLine(pen, rectOut.Left, rectOut.Top, rectOut.Left, rectOut.Bottom);
                                }
                                else
                                {
                                    g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                                }
                            }
                            #endregion

                            #region Bottom border side
                            if (IsBottomBorderSidePresent)
                            {
                                left = rectIn.Left;
                                right = rectIn.Right;

                                if (!IsLeftBorderSidePresent) 
                                    left = rectOut.Left;

                                if (!IsRightBorderSidePresent) 
                                    right = rectOut.Right;

                                if (bits.Style == StiPenStyle.Double)
                                {
                                    g.DrawLine(emptyPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                                    g.DrawLine(pen, left, rectIn.Bottom, right, rectIn.Bottom);
                                    g.DrawLine(pen, rectOut.Left, rectOut.Bottom, rectOut.Right, rectOut.Bottom);
                                }
                                else
                                {
                                    g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                                }
                            }
                            #endregion

                            #region Right border side
                            if (IsRightBorderSidePresent)
                            {
                                top = rectIn.Top;
                                bottom = rectIn.Bottom;

                                if (!IsTopBorderSidePresent) 
                                    top = rectOut.Top;

                                if (!IsBottomBorderSidePresent) 
                                    bottom = rectOut.Bottom;

                                if (bits.Style == StiPenStyle.Double)
                                {
                                    g.DrawLine(emptyPen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                                    g.DrawLine(pen, rectIn.Right, top, rectIn.Right, bottom);
                                    g.DrawLine(pen, rectOut.Right, rectOut.Top, rectOut.Right, rectOut.Bottom);
                                }
                                else
                                {
                                    g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                                }
                            }
                            #endregion
                        }

                        emptyPen?.Dispose();
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Draws shadow on the indicated Graphics.
        /// </summary>
        /// <param name="g">Graphics on which a border can be drawn.</param>
        /// <param name="rect">The rectangle that indicates an area of the border drawing.</param>
        /// <param name="zoom">The scale of a border to draw.</param>
        public virtual void DrawBorderShadow(Graphics g, RectangleF rect, float zoom)
        {
            if (IsDefault) return;
            if (bits == null) return;
            if (!bits.DropShadow) return;

            var dist = (float)(bits.ShadowSize * zoom);
            var rect2 = new RectangleF(rect.Left + dist,
                rect.Top + dist, rect.Width, rect.Height);

            using (var brush = StiBrush.GetBrush(ShadowBrush, rect2))
            {
                g.FillRectangle(brush, rect.Right, rect.Top + dist, dist, rect.Height - dist);
                g.FillRectangle(brush, rect.Left + dist, rect.Bottom, rect.Width, dist);
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public virtual StiBorderIdent Ident => StiBorderIdent.Border;

        /// <summary>
        /// Gets value which indicates that top side of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsTopBorderSidePresent
        {
            get
            {
                if (bits == null) 
                    return false;

                return (bits.Side & StiBorderSides.Top) != 0;
            }
        }

        /// <summary>
        /// Gets value which indicates that bottom side of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsBottomBorderSidePresent
        {
            get
            {
                if (bits == null) 
                    return false;

                return (bits.Side & StiBorderSides.Bottom) != 0;
            }
        }

        /// <summary>
        /// Gets value which indicates that left side of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsLeftBorderSidePresent
        {
            get
            {
                if (bits == null) 
                    return false;

                return (bits.Side & StiBorderSides.Left) != 0;
            }
        }

        /// <summary>
        /// Gets value which indicates that right side of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsRightBorderSidePresent
        {
            get
            {
                if (bits == null) 
                    return false;

                return (bits.Side & StiBorderSides.Right) != 0;
            }
        }

        /// <summary>
        /// Gets value which indicates that all sides of border is present.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsAllBorderSidesPresent
        {
            get
            {
                if (bits == null) 
                    return false;

                return bits.Side == StiBorderSides.All;
            }
        }

        [JsonIgnore]
        private bool IsDefaultShadowBrush => ShadowBrush is StiSolidBrush && ((StiSolidBrush)ShadowBrush).Color == Color.Black;

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
        public virtual StiBorderSides Side
        {
            get
            {
                return bits == null ? StiBorderSides.None : bits.Side;
            }
            set
            {
                if (value == StiBorderSides.None && bits == null)
                    return;

                if (bits != null)
                    bits.Side = value;
                else
                    bits = new BitsBorder(value, Color, Size, Style, ShadowSize, DropShadow, Topmost);
            }
        }

        /// <summary>
        /// Gets or sets a border color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets a border color.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(100)]
        public virtual Color Color
        {
            get
            {
                return bits == null ? Color.Black : bits.Color;
            }
            set
            {
                if (value == Color.Black && bits == null)return;

                if (bits != null)
                    bits.Color = value;
                else
                    bits = new BitsBorder(Side, value, Size, Style, ShadowSize, DropShadow, Topmost);
            }
        }

        /// <summary>
        /// Gets or sets a border size.
        /// </summary>
        [StiSerializable]
        [DefaultValue(1d)]
        [Description("Gets or sets a border size.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(400)]
        public virtual double Size
        {
            get
            {
                return bits == null ? 1d : bits.Size;
            }
            set
            {
                if (value == 1d && bits == null)return;

                if (bits != null)
                {
                    if (value >= 0)
                        bits.Size = value;
                }
                else
                {
                    bits = new BitsBorder(Side, Color, value, Style, ShadowSize, DropShadow, Topmost);
                }
            }
        }

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
        public virtual StiPenStyle Style
        {
            get
            {
                return bits == null ? StiPenStyle.Solid : bits.Style;
            }
            set
            {
                if (value == StiPenStyle.Solid && bits == null)return;

                if (bits != null)
                    bits.Style = value;
                else
                    bits = new BitsBorder(Side, Color, Size, value, ShadowSize, DropShadow, Topmost);
            }
        }

        /// <summary>
        /// Gets or sets the border shadow brush.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets the border shadow brush.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(200)]
        public virtual StiBrush ShadowBrush { get; set; }

        /// <summary>
        /// Gets or sets Shadow Size.
        /// </summary>
        [StiSerializable]
        [DefaultValue(4d)]
        [Description("Gets or sets Shadow Size.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(250)]
        public virtual double ShadowSize
        {
            get
            {
                return bits == null ? 4d : bits.ShadowSize;
            }
            set
            {
                if (value == 4d && bits == null)return;

                if (bits != null)
                {
                    if (value >= 0)
                        bits.ShadowSize = value;
                }
                else
                {
                    bits = new BitsBorder(Side, Color, Size, Style, value, DropShadow, Topmost);
                }
            }
        }

        /// <summary>
        /// Gets or sets value which indicates drop shadow or not.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates drop shadow or not.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(190)]
        public virtual bool DropShadow
        {
            get
            {
                return bits != null && bits.DropShadow;
            }
            set
            {
                if (value == false && bits == null)return;

                if (bits != null)
                    bits.DropShadow = value;
                else
                    bits = new BitsBorder(Side, Color, Size, Style, ShadowSize, value, Topmost);
            }
        }

        /// <summary>
        /// Gets or sets value which indicates that border sides will be drawn on top of all components.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value which indicates that border sides will be drawn on top of all components.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiOrder(600)]
        public virtual bool Topmost
        {
            get
            {
                return bits != null && bits.Topmost;
            }
            set
            {
                if (value == false && bits == null)return;

                if (bits != null)
                    bits.Topmost = value;
                else
                    bits = new BitsBorder(Side, Color, Size, Style, ShadowSize, DropShadow, value);
            }
        }

        /// <summary>
        /// Gets value indicates, that this object-frame is by default.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public virtual bool IsDefault => IsDefaultShadowBrush && bits == null;
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

                    case "ShadowBrush":
                        ShadowBrush = property.DeserializeBrush();
                        break;

                    case "ShadowSize":
                        ShadowSize = property.DeserializeDouble();
                        break;

                    case "DropShadow":
                        DropShadow = property.DeserializeBool();
                        break;

                    case "Topmost":
                        Topmost = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the StiBorder class.
        /// </summary>
        public StiBorder()
            : this(StiBorderSides.None, Color.Black, 1d, StiPenStyle.Solid, false, 4d, new StiSolidBrush(Color.Black))
        {
        }

        /// <summary>
        /// Creates a new instance of the StiBorder class.
        /// </summary>
        /// <param name="side">Border sides.</param>
        /// <param name="color">Border color.</param>
        /// <param name="size">Border size.</param>
        /// <param name="style">Border style.</param>
        public StiBorder(StiBorderSides side, Color color, double size, StiPenStyle style)
            : this(side, color, size, style, false, 4d, new StiSolidBrush(Color.Black))
        {
        }

        /// <summary>
        /// Creates a new instance of the StiBorder class.
        /// </summary>
        /// <param name="side">Border sides.</param>
        /// <param name="color">Border color.</param>
        /// <param name="size">Border size.</param>
        /// <param name="style">Border style.</param>
        /// <param name="dropShadow">Drop shadow or not.</param>
        /// <param name="shadowSize">Shadow size.</param>
        /// <param name="shadowBrush">Brush for drawing shadow of border.</param>
        public StiBorder(StiBorderSides side, Color color, double size, StiPenStyle style,
            bool dropShadow, double shadowSize, StiBrush shadowBrush) 
            : this(side, color, size, style, dropShadow, shadowSize, shadowBrush, false)
        {
        }

        /// <summary>
        /// Creates a new instance of the StiBorder class.
        /// </summary>
        /// <param name="side">Border sides.</param>
        /// <param name="color">Border color.</param>
        /// <param name="size">Border size.</param>
        /// <param name="style">Border style.</param>
        /// <param name="dropShadow">Drop shadow or not.</param>
        /// <param name="shadowSize">Shadow size.</param>
        /// <param name="shadowBrush">Brush for drawing shadow of border.</param>
        /// <param name="topmost">Value which indicates that border sides will be drawn on top of all components.</param>
        public StiBorder(StiBorderSides side, Color color, double size, StiPenStyle style,
            bool dropShadow, double shadowSize, StiBrush shadowBrush, bool topmost)
        {
            bits = BitsBorder.IsDefault(side, color, size, style, dropShadow, shadowSize, topmost)
                ? null
                : new BitsBorder(side, color, size, style, shadowSize, dropShadow, topmost);

            this.ShadowBrush = shadowBrush;
        }
    }
}
