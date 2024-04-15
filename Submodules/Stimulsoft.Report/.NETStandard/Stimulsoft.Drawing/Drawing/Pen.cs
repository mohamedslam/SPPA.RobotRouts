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
using System.Drawing;
using Stimulsoft.Drawing.Drawing2D;

namespace Stimulsoft.Drawing
{
    public sealed class Pen : ICloneable, IDisposable
    {
        private SixLabors.ImageSharp.Drawing.Processing.Pen sixPen;
        internal SixLabors.ImageSharp.Drawing.Processing.Pen SixPen
        {
            get
            {
                if (sixPen == null)
                {
                    float sixWidth = Math.Max(width, 0.5f);
                    if (brush != null)
                    {
                        sixPen = new SixLabors.ImageSharp.Drawing.Processing.Pen(brush.SixBrush, sixWidth, dashPattern);
                    }
                    else
                    {
                        var sixColor = SixLabors.ImageSharp.Color.Black;
                        if (!color.IsEmpty)
                            sixColor = SixLabors.ImageSharp.Color.FromRgba(color.R, color.G, color.B, color.A);

                        sixPen = new SixLabors.ImageSharp.Drawing.Processing.Pen(sixColor, sixWidth, dashPattern);
                    }

                    if (lineJoin == System.Drawing.Drawing2D.LineJoin.Miter)
                        sixPen.JointStyle = SixLabors.ImageSharp.Drawing.JointStyle.Miter;
                    else if (lineJoin == System.Drawing.Drawing2D.LineJoin.Round)
                        sixPen.JointStyle = SixLabors.ImageSharp.Drawing.JointStyle.Round;
                    else if (lineJoin == System.Drawing.Drawing2D.LineJoin.Bevel)
                        sixPen.JointStyle = SixLabors.ImageSharp.Drawing.JointStyle.Square;

                    if (endCap == System.Drawing.Drawing2D.LineCap.Flat)
                        sixPen.EndCapStyle = SixLabors.ImageSharp.Drawing.EndCapStyle.Butt;
                    if (endCap == System.Drawing.Drawing2D.LineCap.Round)
                        sixPen.EndCapStyle = SixLabors.ImageSharp.Drawing.EndCapStyle.Round;
                    else if (endCap == System.Drawing.Drawing2D.LineCap.Square)
                        sixPen.EndCapStyle = SixLabors.ImageSharp.Drawing.EndCapStyle.Square;
                }

                return sixPen;
            }
        }

        internal System.Drawing.Pen netPen;

        private Brush brush;

        private System.Drawing.Drawing2D.PenAlignment alignment = System.Drawing.Drawing2D.PenAlignment.Center;
        public System.Drawing.Drawing2D.PenAlignment Alignment
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPen.Alignment;
                else
                    return alignment;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netPen.Alignment = (System.Drawing.Drawing2D.PenAlignment)value;
                else
                {
                    alignment = value;
                    Reset();
                }
            }
        }

        private Color color = Color.Empty;
        public Color Color
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPen.Color;
                else
                    return color;
            }
        }

        private float[] dashPattern = new float[] { };
        public float[] DashPattern
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPen.DashPattern;
                else
                    return dashPattern;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netPen.DashPattern = value;
                else
                {
                    dashPattern = value;
                    dashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                    Reset();
                }
            }
        }

        private System.Drawing.Drawing2D.DashStyle dashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        public System.Drawing.Drawing2D.DashStyle DashStyle
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPen.DashStyle;
                else
                    return dashStyle;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netPen.DashStyle = (System.Drawing.Drawing2D.DashStyle)value;
                else
                {
                    dashStyle = value;
                    if (dashStyle == System.Drawing.Drawing2D.DashStyle.DashDotDot)
                        dashPattern = new float[] { 3, 1, 1, 1, 1, 1 };
                    else if (dashStyle == System.Drawing.Drawing2D.DashStyle.Dash)
                        dashPattern = new float[] { 3, 1 };
                    else if (dashStyle == System.Drawing.Drawing2D.DashStyle.DashDot)
                        dashPattern = new float[] { 3, 1, 1, 1 };
                    else if (dashStyle == System.Drawing.Drawing2D.DashStyle.Dot)
                        dashPattern = new float[] { 1, 1 };
                    else if (dashStyle == System.Drawing.Drawing2D.DashStyle.Solid)
                        dashPattern = new float[] { };

                    Reset();
                }
            }
        }

        private System.Drawing.Drawing2D.LineCap startCap = System.Drawing.Drawing2D.LineCap.Square;
        public System.Drawing.Drawing2D.LineCap StartCap
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPen.StartCap;
                else
                    return startCap;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netPen.StartCap = (System.Drawing.Drawing2D.LineCap)value;
                else
                {
                    startCap = value;
                    Reset();
                }
            }
        }

        private System.Drawing.Drawing2D.LineCap endCap = System.Drawing.Drawing2D.LineCap.Flat;
        public System.Drawing.Drawing2D.LineCap EndCap
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPen.EndCap;
                else
                    return endCap;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netPen.EndCap = (System.Drawing.Drawing2D.LineCap)value;
                else
                {
                    endCap = value;
                    Reset();
                }
            }
        }

        private System.Drawing.Drawing2D.LineJoin lineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
        public System.Drawing.Drawing2D.LineJoin LineJoin
        {

            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPen.LineJoin;
                else
                    return lineJoin;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)value;
                else
                {
                    lineJoin = value;
                    Reset();
                }
            }
        }

        private float width = 1;
        public float Width
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netPen.Width;
                else
                    return width;
            }
            set
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    netPen.Width = value;
                else
                {
                    width = value;
                    if (width < 1) width = 1;
                    Reset();
                }
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
        }

        private void Reset()
        {
            sixPen = null;
        }

        public Pen(Brush brush)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPen = new System.Drawing.Pen(brush.NetBrush);
            else
                this.brush = brush;
        }

        public Pen(Color color)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPen = new System.Drawing.Pen(color);
            else
                this.color = color;
        }

        public Pen(Brush brush, float width)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPen = new System.Drawing.Pen(brush.NetBrush, width);
            else
            {
                this.brush = brush;
                this.width = width;
            }
        }

        public Pen(Color color, float width)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netPen = new System.Drawing.Pen(color, width);
            else
            {
                this.color = color;
                this.width = width;
            }
        }
    }
}
