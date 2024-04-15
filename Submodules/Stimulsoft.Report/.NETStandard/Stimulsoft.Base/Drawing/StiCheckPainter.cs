#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
{	                         										}
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
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiCheckPainter
    {
        public static void PaintCheckBox(Graphics g, Rectangle rect, StiCheckState checkState, Color color)
        {
            PaintCheckBox(g, RectangleD.CreateFromRectangle(rect), checkState, color, (float)StiScale.Factor);
        }

        public static void PaintCheckBox(Graphics g, Rectangle rect, bool checkState, Color color)
        {
            PaintCheckBox(g, RectangleD.CreateFromRectangle(rect), checkState ? StiCheckState.Checked : StiCheckState.Unchecked, color, (float)StiScale.Factor);
        }

        public static void PaintCheckBox(Graphics g, RectangleD rectD, StiCheckState checkState, Color color, float scale)
        {
            var size = GetCheckSize(scale);
            var rect = new RectangleD(rectD.X + (rectD.Width - size) / 2, rectD.Y + (rectD.Height - size) / 2, size, size).ToRectangle();

            using (var pen = new Pen(color))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }

            if (checkState == StiCheckState.Indeterminate)
            {
                using (var brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, rect.X + StiScale.I3, rect.Y + StiScale.I3, rect.Width - StiScale.I6 + 1, rect.Height - StiScale.I6 + 1);
                }
            }
            else if (checkState == StiCheckState.Checked)
            {
                rect.Inflate(-(int)(4 * scale), -(int)(4 * scale));

                var p1 = new PointF(rect.X, rect.Y + rect.Height * 0.7f);
                var p2 = new PointF(rect.X + rect.Width * .5f, rect.Bottom);
                var p3 = new PointF(rect.Right, rect.Y);

                var old = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(color))
                {
                    pen.Width = 2 * scale;
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, p1, p2);
                    g.DrawLine(pen, p2, p3);
                }
                g.SmoothingMode = old;
            }
        }

        public static int GetCheckSize(float scale)
        {
            var size = (int)(16 * scale);
            return (int)Math.Min(26 * scale, size);
        }

        public static int GetCheckSize()
        {
            return GetCheckSize((float)StiScale.Factor);
        }
    }
}
