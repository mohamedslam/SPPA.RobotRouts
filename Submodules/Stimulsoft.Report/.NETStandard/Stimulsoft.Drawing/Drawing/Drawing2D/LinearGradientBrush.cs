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
using System.Drawing;
using System.Numerics;

namespace Stimulsoft.Drawing.Drawing2D
{
    public sealed class LinearGradientBrush : Brush
    {
        private SixLabors.ImageSharp.Drawing.Processing.LinearGradientBrush sixBrush;
        internal override SixLabors.ImageSharp.Drawing.Processing.IBrush SixBrush => sixBrush;

        private System.Drawing.Drawing2D.LinearGradientBrush netBrush;
        internal override System.Drawing.Brush NetBrush => netBrush;

        public System.Drawing.Drawing2D.Blend Blend { get; set; }

        public void SetSigmaBellShape(float focus, float scale)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netBrush.SetSigmaBellShape(focus, scale);
        }

        public LinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netBrush = new System.Drawing.Drawing2D.LinearGradientBrush(point1, point2, color1, color2);
            else
            {
                var sixColor1 = ColorExt.ToSixColor(color1);
                var sixColor2 = ColorExt.ToSixColor(color2);
                var sixPoint1 = PointExt.ToSixPoint(point1);
                var sixPoint2 = PointExt.ToSixPoint(point2);
                var colorStop1 = new SixLabors.ImageSharp.Drawing.Processing.ColorStop(0, sixColor1);
                var colorStop2 = new SixLabors.ImageSharp.Drawing.Processing.ColorStop(1, sixColor2);

                sixBrush = new SixLabors.ImageSharp.Drawing.Processing.LinearGradientBrush(sixPoint1, sixPoint2, SixLabors.ImageSharp.Drawing.Processing.GradientRepetitionMode.DontFill, new SixLabors.ImageSharp.Drawing.Processing.ColorStop[] { colorStop1, colorStop2 });
            }
        }

        public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle): this(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), color1, color2, angle)
        {
        }

        public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netBrush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, color1, color2, angle);
            else
            {
                var topLeft = new PointF(rect.Left, rect.Top);
                var topRight = Vector2.Transform(new Vector2(rect.Right, rect.Top), Matrix3x2.CreateRotation(angle * (float)Math.PI / 180));

                var sixColor1 = ColorExt.ToSixColor(color1);
                var sixColor2 = ColorExt.ToSixColor(color2);
                var sixPoint1 = PointExt.ToSixPoint(topLeft);
                var sixPoint2 = new SixLabors.ImageSharp.PointF(topRight.X, topRight.Y);
                var colorStop1 = new SixLabors.ImageSharp.Drawing.Processing.ColorStop(0, sixColor1);
                var colorStop2 = new SixLabors.ImageSharp.Drawing.Processing.ColorStop(1, sixColor2);

                sixBrush = new SixLabors.ImageSharp.Drawing.Processing.LinearGradientBrush(sixPoint1, sixPoint2, SixLabors.ImageSharp.Drawing.Processing.GradientRepetitionMode.DontFill, new SixLabors.ImageSharp.Drawing.Processing.ColorStop[] { colorStop1, colorStop2 });
            }
        }
    }
}
