#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Units;
using System;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Pens = Stimulsoft.Drawing.Pens;
using Brushes = Stimulsoft.Drawing.Brushes;
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
    public class StiDimensionLinesHelper
    {
        public static bool IsAllowLocationDimensionLine(StiAction action)
        {
            switch (action)
            {
                case StiAction.SizeLeft:
                case StiAction.SizeLeftBottom:
                case StiAction.SizeLeftTop:
                case StiAction.SizeRightTop:
                case StiAction.SizeTop:
                case StiAction.Move:
                    return true;

                default:
                    return false;
            }
        }

        private static string GetUnitValueString(double value, StiPage page)
        {
            if ((page.Unit is StiCentimetersUnit || page.Unit is StiInchesUnit) &&
                value >= 0.2 && !(page is IStiForm))
                return ((decimal) value).ToString("F");

            return ((int)Math.Round(value, 0)).ToString();
        }

        private static void DrawLeftDimensionLine(Graphics g, StiPage page, Font font, RectangleF rect, double value)
        {
            if (double.IsNaN(rect.X) || double.IsNaN(rect.Y) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height)) return;
            if (rect.Height < StiScale.I10) return;

            string strValue = GetUnitValueString(value, page);

            float centerX = rect.X + rect.Width / 2;
            float centerY = rect.Y + rect.Height / 2;

            var size = g.MeasureString(strValue, font);
            size.Width += StiScale.I2;
            size = new SizeF(size.Height, size.Width);

            g.DrawLine(Pens.Blue, rect.X + StiScale.I5, rect.Y, rect.Right - StiScale.I3, rect.Y);
            g.DrawLine(Pens.Blue, rect.X + StiScale.I5, rect.Bottom, rect.Right - StiScale.I3, rect.Bottom);

            var strRect = new RectangleF(
                centerX - size.Width / 2,
                centerY - size.Height / 2,
                size.Width,
                size.Height);

            if (rect.Height > size.Height * 1.1)
            {
                g.DrawLine(Pens.Blue, centerX, rect.Y, centerX, centerY - size.Height / 2);
                g.DrawLine(Pens.Blue, centerX, centerY + size.Height / 2, centerX, rect.Bottom);
            }
            else
            {
                g.DrawLine(Pens.Blue, centerX, rect.Y, centerX, rect.Bottom);
                strRect.X -= rect.Width / 2;
            }

            using (Brush brush = new SolidBrush(Color.FromArgb(160, Color.White)))
            {
                g.FillRectangle(brush, strRect);
            }

            using (StringFormat sfHeight = new StringFormat())
            {
                sfHeight.Alignment = StringAlignment.Center;
                sfHeight.LineAlignment = StringAlignment.Center;

                size.Height *= 2;

                StiTextDrawing.DrawString(g, strValue, font, Brushes.Blue,
                    RectangleD.CreateFromRectangle(strRect), sfHeight, 270f);
            }
        }

        private static void DrawRightDimensionLine(Graphics g, StiPage page, Font font, RectangleF rect, double value)
        {
            if (double.IsNaN(rect.X) || double.IsNaN(rect.Y) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height)) return;
            if (rect.Height < StiScale.I10) return;

            var strValue = GetUnitValueString(value, page);

            float centerX = rect.X + rect.Width / 2;
            float centerY = rect.Y + rect.Height / 2;

            var size = g.MeasureString(strValue, font);
            size.Width += StiScale.I2;
            size = new SizeF(size.Height, size.Width);

            g.DrawLine(Pens.Blue, rect.X + StiScale.I3, rect.Y, rect.Right - StiScale.I5, rect.Y);
            g.DrawLine(Pens.Blue, rect.X + StiScale.I3, rect.Bottom, rect.Right - StiScale.I5, rect.Bottom);

            var strRect = new RectangleF(
                centerX - size.Width / 2,
                centerY - size.Height / 2,
                size.Width,
                size.Height);

            if (rect.Height > size.Height * 1.1)
            {
                g.DrawLine(Pens.Blue, centerX, rect.Y, centerX, centerY - size.Height / 2);
                g.DrawLine(Pens.Blue, centerX, centerY + size.Height / 2, centerX, rect.Bottom);
            }
            else
            {
                g.DrawLine(Pens.Blue, centerX, rect.Y, centerX, rect.Bottom);
                strRect.X += rect.Width / 2;
            }

            using (var brush = new SolidBrush(Color.FromArgb(160, Color.White)))
            {
                g.FillRectangle(brush, strRect);
            }

            using (var sfHeight = new StringFormat())
            {
                sfHeight.Alignment = StringAlignment.Center;
                sfHeight.LineAlignment = StringAlignment.Center;

                size.Height *= 2;

                StiTextDrawing.DrawString(g, strValue, font, Brushes.Blue,
                    RectangleD.CreateFromRectangle(strRect), sfHeight, 90f);
            }
        }

        private static void DrawTopDimensionLine(Graphics g, StiPage page, Font font, RectangleF rect, double value)
        {
            if (double.IsNaN(rect.X) || double.IsNaN(rect.Y) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height)) return;

            if (rect.Width < StiScale.I10) return;

            string strValue = GetUnitValueString(value, page);

            float centerX = rect.X + rect.Width / 2;
            float centerY = rect.Y + rect.Height / 2;

            var size = g.MeasureString(strValue, font);
            size.Width += StiScale.I2;

            g.DrawLine(Pens.Blue, rect.X, rect.Y + StiScale.I5, rect.X, rect.Bottom - StiScale.I3);
            g.DrawLine(Pens.Blue, rect.Right, rect.Y + StiScale.I5, rect.Right, rect.Bottom - StiScale.I3);

            var strRect = new RectangleF(
                centerX - size.Width / 2,
                centerY - size.Height / 2,
                size.Width,
                size.Height);

            if (rect.Width > size.Width * 1.1)
            {
                g.DrawLine(Pens.Blue, rect.X, centerY, centerX - size.Width / 2, centerY);
                g.DrawLine(Pens.Blue, centerX + size.Width / 2, centerY, rect.Right, centerY);
            }
            else
            {
                g.DrawLine(Pens.Blue, rect.X, centerY, rect.Right, centerY);
                strRect.Y -= rect.Height / 2;
            }

            using (var brush = new SolidBrush(Color.FromArgb(160, Color.White)))
            {
                g.FillRectangle(brush, strRect);
            }

            using (var sfWidth = new StringFormat())
            {
                sfWidth.Alignment = StringAlignment.Center;
                sfWidth.LineAlignment = StringAlignment.Center;
                size.Width *= 2;
                g.DrawString(strValue, font, Brushes.Blue, strRect, sfWidth);
            }
        }

        private static void DrawBottomDimensionLine(Graphics g, StiPage page, Font font, RectangleF rect, double value)
        {
            if (double.IsNaN(rect.X) || double.IsNaN(rect.Y) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height)) return;
            if (rect.Width < StiScale.I10) return;

            string strValue = GetUnitValueString(value, page);

            float centerX = rect.X + rect.Width / 2;
            float centerY = rect.Y + rect.Height / 2;

            var size = g.MeasureString(strValue, font);
            size.Width += StiScale.I2;

            g.DrawLine(Pens.Blue, rect.X, rect.Y + StiScale.I3, rect.X, rect.Bottom - StiScale.I5);
            g.DrawLine(Pens.Blue, rect.Right, rect.Y + StiScale.I3, rect.Right, rect.Bottom - StiScale.I5);

            var strRect = new RectangleF(
                centerX - size.Width / 2,
                centerY - size.Height / 2,
                size.Width,
                size.Height);

            if (rect.Width > size.Width * 1.1)
            {
                g.DrawLine(Pens.Blue, rect.X, centerY, centerX - size.Width / 2, centerY);
                g.DrawLine(Pens.Blue, centerX + size.Width / 2, centerY, rect.Right, centerY);
            }
            else
            {
                g.DrawLine(Pens.Blue, rect.X, centerY, rect.Right, centerY);
                strRect.Y += rect.Height / 2;
            }

            using (var brush = new SolidBrush(Color.FromArgb(160, Color.White)))
            {
                g.FillRectangle(brush, strRect);
            }

            using (var sfWidth = new StringFormat())
            {
                sfWidth.Alignment = StringAlignment.Center;
                sfWidth.LineAlignment = StringAlignment.Center;
                size.Width *= 2;
                g.DrawString(strValue, font, Brushes.Blue, strRect, sfWidth);
            }
        }

        private static void DrawLocationDimensionLine(Graphics g, StiPage page, Font font, RectangleF rect, double valueX, double valueY)
        {
            if (double.IsNaN(rect.X) || double.IsNaN(rect.Y) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height)) return;

            string strValueX = GetUnitValueString(valueX, page);
            string strValueY = GetUnitValueString(valueY, page);
            string strValue = $"{strValueX};{strValueY}";

            var size = g.MeasureString(strValue, font);
            size.Width += StiScale.I2;

            var linesRect = new RectangleF(
                rect.Right - (int)Math.Ceiling(50 * StiScale.Factor),
                rect.Bottom - (int)Math.Ceiling(16 * StiScale.Factor),
                (int)Math.Ceiling(50 * StiScale.Factor),
                (int)Math.Ceiling(16 * StiScale.Factor));

            using (var brush = new SolidBrush(Color.FromArgb(160, Color.White)))
            {
                g.FillRectangle(brush, linesRect);
            }

            var strRect = new RectangleF(
                rect.Right - size.Width,
                rect.Bottom - size.Height,
                size.Width,
                size.Height);

            using (var sfWidth = new StringFormat())
            {
                sfWidth.Alignment = StringAlignment.Far;
                sfWidth.LineAlignment = StringAlignment.Far;

                var clipBounds = g.ClipBounds;
                g.SetClip(strRect);
                g.DrawString(strValue, font, Brushes.Blue, strRect, sfWidth);
                g.SetClip(clipBounds);
            }

            g.DrawLine(Pens.Blue, rect.Right - StiScale.XXI(50), rect.Bottom, rect.Right - StiScale.I2, rect.Bottom);
            g.DrawLine(Pens.Blue, rect.Right, rect.Bottom - StiScale.I16, rect.Right, rect.Bottom - StiScale.I2);
        }

        public static void DrawDimensionLines(Graphics g, StiPage page)
        {
            bool isBandExist = false;
            bool isCrossBandExist = false;

            #region Get resize rect
            var rect = RectangleD.Empty;

            var comps = page.GetSelectedComponents();
            var clientRect = RectangleD.Empty;

            foreach (StiComponent comp in comps)
            {
                if (comp is StiBand)
                    isBandExist = true;

                if (comp.IsCross)
                    isCrossBandExist = true;

                var compRect = comp.GetPaintRectangle(false, false);
                var compClientRect = comp.ClientRectangle;

                compClientRect = StiComponent.DoOffsetRect(comp, compClientRect, comp.Page.OffsetRectangle);

                if (rect.IsEmpty)
                    rect = compRect;
                else
                    rect = rect.FitToRectangle(compRect);

                if (clientRect.IsEmpty)
                    clientRect = compClientRect;
                else
                    clientRect = clientRect.FitToRectangle(compClientRect);
            }

            if (rect.IsEmpty) return;
            #endregion

            if (rect.Width == 0 && rect.Height == 0) return;

            double gridSize = page.GridSize;
            gridSize = Math.Min(gridSize, page.Unit.ConvertFromHInches(20d));
            gridSize = Math.Max(gridSize, page.Unit.ConvertFromHInches(6d));

            var rectHor = rect;
            var rectVer = rect;
            rectHor.Inflate(gridSize * 2, 0);
            rectVer.Inflate(0, gridSize * 2);

            double stepHor = (rectHor.Width - rect.Width) / 2;
            double stepVer = (rectVer.Height - rect.Height) / 2;

            var rectLeft = new RectangleD(rectHor.X, rectHor.Y, stepHor, rectHor.Height);
            var rectRight = new RectangleD(rectHor.Right - stepHor, rectHor.Y, stepHor, rectHor.Height);
            var rectTop = new RectangleD(rectVer.X, rectVer.Y, rectVer.Width, stepVer);
            var rectBottom = new RectangleD(rectVer.X, rectVer.Bottom - stepVer, rectVer.Width, stepVer);
            var rectLocation = new RectangleD(
                rect.X - gridSize * StiScale.XX(6.5), 
                rect.Y - gridSize * StiScale.YY(1.7), 
                StiScale.XX(gridSize * 6.5), 
                StiScale.YY(gridSize * 1.7));

            double zoom = page.Zoom * StiScale.Factor;

            rectLeft = rectLeft.Multiply(zoom);
            rectLeft = page.Unit.ConvertToHInches(rectLeft);

            rectRight = rectRight.Multiply(zoom);
            rectRight = page.Unit.ConvertToHInches(rectRight);

            rectTop = rectTop.Multiply(zoom);
            rectTop = page.Unit.ConvertToHInches(rectTop);

            rectBottom = rectBottom.Multiply(zoom);
            rectBottom = page.Unit.ConvertToHInches(rectBottom);

            rectLocation = rectLocation.Multiply(zoom);
            rectLocation = page.Unit.ConvertToHInches(rectLocation);

            #region Process designer action
            bool left = false;
            bool right = false;
            bool top = false;
            bool bottom = false;

            switch (page.Report.Info.CurrentAction)
            {
                case StiAction.SizeBottom:
                    right = true;
                    break;

                case StiAction.SizeLeft:
                    bottom = true;
                    break;

                case StiAction.SizeLeftBottom:
                    left = true;
                    bottom = true;
                    break;

                case StiAction.SizeLeftTop:
                    left = true;
                    top = true;
                    break;

                case StiAction.SizeRight:
                    bottom = true;
                    break;

                case StiAction.SizeRightBottom:
                    right = true;
                    bottom = true;
                    break;

                case StiAction.SizeRightTop:
                    top = true;
                    right = true;
                    break;

                case StiAction.SizeTop:
                    right = true;
                    break;
            }
            #endregion

            if (isCrossBandExist)
            {
                if (bottom)
                {
                    top = true;
                    bottom = false;
                }
            }

            using (var font = new Font("Arial", 7, FontStyle.Bold))
            {
                if (left)
                    DrawLeftDimensionLine(g, page, font, rectLeft.ToRectangleF(), rect.Height);

                if (right)
                    DrawRightDimensionLine(g, page, font, rectRight.ToRectangleF(), rect.Height);

                if (top)
                    DrawTopDimensionLine(g, page, font, rectTop.ToRectangleF(), rect.Width);

                if (bottom)
                    DrawBottomDimensionLine(g, page, font, rectBottom.ToRectangleF(), rect.Width);

                if (!isBandExist && IsAllowLocationDimensionLine(page.Report.Info.CurrentAction))
                    DrawLocationDimensionLine(g, page, font, rectLocation.ToRectangleF(), clientRect.Left, clientRect.Top);
            }
        }
    }
}
