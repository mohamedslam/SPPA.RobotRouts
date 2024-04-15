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
using System.Drawing;
using System.Drawing.Drawing2D;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Localization;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
#endif

namespace Stimulsoft.Base.Drawing
{
    [JsonObject]
    public class StiBorderSide : ICloneable
    {
        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var border = base.MemberwiseClone() as StiBorderSide;
            border.Style = Style;
            return border;
        }
        #endregion

        #region IEquatable
        protected bool Equals(StiBorderSide other)
        {
            return Size.Equals(other.Size) && Color.Equals(other.Color) && side == other.side && Style == other.Style;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as StiBorderSide;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Size.GetHashCode();
                hashCode = (hashCode*397) ^ Color.GetHashCode();
                hashCode = (hashCode*397) ^ (int) side;
                hashCode = (hashCode*397) ^ (int) Style;
                return hashCode;
            }
        }

        public static bool operator ==(StiBorderSide left, StiBorderSide right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StiBorderSide left, StiBorderSide right)
        {
            return !Equals(left, right);
        }
        #endregion

        #region Methods
        public float GetSizeOffset()
        {
            if (Style == StiPenStyle.None) return 0f;
            if (Style == StiPenStyle.Double) return 1f;

            return (float)Size / 2;
        }

        internal float GetSize()
        {
            if (Style == StiPenStyle.None) return 0f;
            if (Style == StiPenStyle.Double) return 3f;

            return (float)Size;
        }

        internal void Draw(Graphics g, RectangleF rect, float zoom, Color emptyColor, StiAdvancedBorder border)
        {
            if (this.Style == StiPenStyle.None) return;
            else if (this.Style == StiPenStyle.Double) DrawDoubleInternal(g, rect, zoom, emptyColor, border);
            else if (this.Style == StiPenStyle.Solid && Size >= 1) DrawSolidInternal(g, rect, zoom, border);
            else DrawInternal(g, rect, zoom, border);
        }

        internal void DrawInternal(Graphics g, RectangleF rect, float zoom, StiAdvancedBorder border)
        {
            using (var pen = new Pen(Color))
            {
                pen.DashStyle = StiPenUtils.GetPenStyle(Style);
                pen.Width = (int)(Size * zoom);
                pen.StartCap = LineCap.NoAnchor;
                pen.EndCap = LineCap.NoAnchor;

                switch (side)
                {
                    case StiBorderSides.Top:
                        if (border.IsTopBorderSidePresent)
                            g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                        break;

                    case StiBorderSides.Bottom:
                        if (border.IsBottomBorderSidePresent)
                            g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                        break;

                    case StiBorderSides.Left:
                        if (border.IsLeftBorderSidePresent)
                            g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                        break;

                    case StiBorderSides.Right:
                        if (border.IsRightBorderSidePresent)
                            g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                        break;
                }
            }
        }

        internal void DrawSolidInternal(Graphics g, RectangleF rect, float zoom, StiAdvancedBorder border)
        {
            using (var brush = new SolidBrush(Color))
            {
                var size = (float)(Size * zoom);

                rect = GetAdvancedBorderRectangle(g, rect, zoom, border);

                switch (side)
                {
                    case StiBorderSides.Top:
                        if (border.IsTopBorderSidePresent)
                            g.FillRectangle(brush, rect.Left, rect.Top, rect.Width, size);
                        break;

                    case StiBorderSides.Bottom:
                        if (border.IsBottomBorderSidePresent)
                            g.FillRectangle(brush, rect.Left, rect.Bottom - size, rect.Width, size);
                        break;

                    case StiBorderSides.Left:
                        if (border.IsLeftBorderSidePresent)
                            g.FillRectangle(brush, rect.Left, rect.Top, size, rect.Height);
                        break;

                    case StiBorderSides.Right:
                        if (border.IsRightBorderSidePresent)
                            g.FillRectangle(brush, rect.Right - size, rect.Top, size, rect.Height);
                        break;
                }
            }
        }

        private void DrawDoubleInternal(Graphics g, RectangleF rect, float zoom, Color emptyColor, StiAdvancedBorder border)
        {
            using (var emptyPen = new Pen(emptyColor))
            using (var pen = new Pen(Color, 1))
            {
                pen.DashStyle = StiPenUtils.GetPenStyle(Style);
                pen.StartCap = LineCap.Square;
                pen.EndCap = LineCap.Square;

                var rectIn = rect;
                var rectOut = rect;

                rectIn.Inflate(-1, -1);
                rectOut.Inflate(1, 1);

                var left = 0f;
                var right = 0f;
                var top = 0f;
                var bottom = 0f;

                switch (side)
                {
                    #region Top
                    case StiBorderSides.Top:
                        if (border.IsTopBorderSidePresent)
                        {
                            left = rectIn.Left;
                            right = rectIn.Right;

                            if (!border.IsLeftBorderSidePresent) left = rectOut.Left;
                            if (!border.IsRightBorderSidePresent) right = rectOut.Right;

                            g.DrawLine(emptyPen, rect.Left, rect.Top, rect.Right, rect.Top);
                            g.DrawLine(pen, left, rectIn.Top, right, rectIn.Top);
                            g.DrawLine(pen, rectOut.Left, rectOut.Top, rectOut.Right, rectOut.Top);
                        }
                        break;
                    #endregion

                    #region Bottom
                    case StiBorderSides.Bottom:
                        if (border.IsBottomBorderSidePresent)
                        {
                            left = rectIn.Left;
                            right = rectIn.Right;

                            if (!border.IsLeftBorderSidePresent) left = rectOut.Left;
                            if (!border.IsRightBorderSidePresent) right = rectOut.Right;

                            g.DrawLine(emptyPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                            g.DrawLine(pen, left, rectIn.Bottom, right, rectIn.Bottom);
                            g.DrawLine(pen, rectOut.Left, rectOut.Bottom, rectOut.Right, rectOut.Bottom);
                        }
                        break;
                    #endregion

                    #region Left
                    case StiBorderSides.Left:
                        if (border.IsLeftBorderSidePresent)
                        {
                            top = rectIn.Top;
                            bottom = rectIn.Bottom;

                            if (!border.IsTopBorderSidePresent) top = rectOut.Top;
                            if (!border.IsBottomBorderSidePresent) bottom = rectOut.Bottom;

                            g.DrawLine(emptyPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                            g.DrawLine(pen, rectIn.Left, top, rectIn.Left, bottom);
                            g.DrawLine(pen, rectOut.Left, rectOut.Top, rectOut.Left, rectOut.Bottom);
                        }
                        break;
                    #endregion

                    #region Right
                    case StiBorderSides.Right:
                        if (border.IsRightBorderSidePresent)
                        {
                            top = rectIn.Top;
                            bottom = rectIn.Bottom;

                            if (!border.IsTopBorderSidePresent) top = rectOut.Top;
                            if (!border.IsBottomBorderSidePresent) bottom = rectOut.Bottom;

                            g.DrawLine(emptyPen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                            g.DrawLine(pen, rectIn.Right, top, rectIn.Right, bottom);
                            g.DrawLine(pen, rectOut.Right, rectOut.Top, rectOut.Right, rectOut.Bottom);
                        }
                        break;
                    #endregion
                }
            }
        }

        private RectangleF GetAdvancedBorderRectangle(Graphics g, RectangleF rect, float zoom, StiAdvancedBorder border)
        {
            if (border.IsTopBorderSidePresent)
            {
                var size = border.TopSide.GetSizeOffset() * zoom;
                rect.Y -= size;
                rect.Height += size;
            }

            if (border.IsBottomBorderSidePresent)
            {
                var size = border.BottomSide.GetSizeOffset() * zoom;
                rect.Height = rect.Height + size;
            }

            if (border.IsLeftBorderSidePresent)
            {
                var size = border.LeftSide.GetSizeOffset() * zoom;
                rect.X -= size;
                rect.Width += size;
            }

            if (border.IsRightBorderSidePresent)
            {
                var size = border.RightSide.GetSizeOffset() * zoom;
                rect.Width = rect.Width + size;
            }

            return rect;
        }
        #endregion

        #region Fields
        internal StiBorderSides side = StiBorderSides.None;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a border color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets a border color.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual Color Color { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets a border size.
        /// </summary>
        [StiSerializable]
        [DefaultValue(1d)]
        [Description("Gets or sets a border size.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual double Size { get; set; } = 1d;

        /// <summary>
        /// Gets or sets a border style.
        /// </summary>
        [Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
        [StiSerializable]
        [DefaultValue(StiPenStyle.None)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Description("Gets or sets a border style.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual StiPenStyle Style { get; set; } = StiPenStyle.None;

        /// <summary>
        /// Gets value indicates, that this object-frame is by default.
        /// </summary>
        [Browsable(false)]
        public bool IsDefault => Color == Color.Black &&
                                 Size == 1d &&
                                 Style == StiPenStyle.None;
        #endregion

        /// <summary>
        /// Creates a new instance of the StiBorderSide class.
		/// </summary>
        public StiBorderSide() :
			this(Color.Black, 1d, StiPenStyle.None)
		{
		}

        /// <summary>
        /// Creates a new instance of the StiBorderSide class.
        /// </summary>
        /// <param name="color">Border color.</param>
        /// <param name="size">Border size.</param>
        /// <param name="style">Border style.</param>
        public StiBorderSide(Color color, double size, StiPenStyle style)
        {
            this.Color = color;
            this.Size = size;
            this.Style = style;
        }
    }
}
