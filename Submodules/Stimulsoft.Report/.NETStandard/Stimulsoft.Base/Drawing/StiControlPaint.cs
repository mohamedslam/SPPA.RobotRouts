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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#endif

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using TextureBrush = Stimulsoft.Drawing.TextureBrush;
using LinearGradientBrush = Stimulsoft.Drawing.Drawing2D.LinearGradientBrush;
using Font = Stimulsoft.Drawing.Font;
using Pen = Stimulsoft.Drawing.Pen;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
using DrawItemState = Stimulsoft.System.Windows.Forms.DrawItemState;
using ButtonState = Stimulsoft.System.Windows.Forms.ButtonState;
using ImageList = Stimulsoft.System.Windows.Forms.ImageList;
using RightToLeft = Stimulsoft.System.Windows.Forms.RightToLeft;
using ControlPaint = Stimulsoft.System.Windows.Forms.ControlPaint;
using SystemInformation = Stimulsoft.System.Windows.Forms.SystemInformation;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Provides methods used to paint controls and their elements.
    /// </summary>
    public static class StiControlPaint
    {
        #region Methods
        public static void DrawImageBackground(Graphics g, Image image, Rectangle rect)
        {
            var brushRect = rect;
            g.TranslateTransform(rect.X, rect.Y);

            brushRect.X = 0;
            brushRect.Y = 0;

            using (var brush = new TextureBrush(image))
            {
                g.FillRectangle(brush, brushRect);
            }

            g.TranslateTransform(-rect.X, -rect.Y);
        }

        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="text">String to draw.</param>
        /// <param name="font">Font object that defines the text format of the string.</param>
        /// <param name="brush">Brush object that determines the color and texture of the drawn text.</param>
        /// <param name="layoutRectangle">The RectangleF structure that specifies the location of the drawn text.</param>
        /// <param name="format">The StringFormat object that specifies formatting attributes, such as line spacing and alignment, that are applied to the drawn text.</param>
        /// <param name="angle">An angle of the text rotation.</param>
        public static void DrawString(Graphics graphics, string text, Font font, Brush brush,
            Rectangle layoutRectangle, StringFormat format, float angle)
        {
            if (angle != 0)
            {
                var svClip = graphics.Clip;
                graphics.SetClip(layoutRectangle, CombineMode.Intersect);

                var state = graphics.Save();

                graphics.TranslateTransform(
                    layoutRectangle.Left + layoutRectangle.Width / 2,
                    layoutRectangle.Top + layoutRectangle.Height / 2);

                graphics.RotateTransform(angle);

                layoutRectangle.X = -layoutRectangle.Width / 2;
                layoutRectangle.Y = -layoutRectangle.Height / 2;

                var drawRect = new Rectangle(layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height);

                if (angle > 45 && angle < 135 || angle > 225 && angle < 315)
                    drawRect = new Rectangle(layoutRectangle.Y, layoutRectangle.X, layoutRectangle.Height, layoutRectangle.Width);

                graphics.DrawString(text, font, brush, drawRect, format);
                graphics.Restore(state);
                graphics.SetClip(svClip, CombineMode.Replace);
            }
            else
                graphics.DrawString(text, font, brush, layoutRectangle, format);
        }

        /// <summary>
        /// Draws the specified image in a disabled state.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="image">The Image to draw.</param>
        /// <param name="x">The X coordinate of the top left of the border image.</param>
        /// <param name="y">The Y coordinate of the top left of the border image.</param>
        public static void DrawImageDisabled(Graphics graphics, Image image, int x, int y)
        {
            if (image is Bitmap)
                DrawImageDisabled(graphics, image as Bitmap, x, y);
        }

        /// <summary>
        /// Draws the specified image in a disabled state.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="bmp">The Bitmap to draw.</param>
        /// <param name="x">The X coordinate of the top left of the border bitmap.</param>
        /// <param name="y">The Y coordinate of the top left of the border bitmap.</param>
        public static void DrawImageDisabled(Graphics graphics, Bitmap bmp, int x, int y)
        {
            var imageAttr = new ImageAttributes();

            var disableMatrix = new ColorMatrix(new[]
            {
                 new[]{0.3f,0.3f,0.3f,0,0},
                 new[]{0.59f,0.59f,0.59f,0,0},
                 new[]{0.11f,0.11f,0.11f,0,0},
                 new[]{0,0,0,0.4f,0,0},
                 new[]{0,0,0,0,0.4f,0},
                 new[]{0,0,0,0,0,0.4f}
             });

            imageAttr.SetColorMatrix(disableMatrix);

            graphics.DrawImage(bmp, new Rectangle(x, y, bmp.Width, bmp.Height),
                0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imageAttr);
        }

        /// <summary>
        /// Draws the specified image in a disabled state.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="bmp">The Bitmap to draw.</param>
        public static void DrawImageDisabled(Graphics graphics, Image bmp, Rectangle rect)
        {
            var imageAttr = new ImageAttributes();

            var disableMatrix = new ColorMatrix(new[]
            {
                 new[]{0.3f,0.3f,0.3f,0,0},
                 new[]{0.59f,0.59f,0.59f,0,0},
                 new[]{0.11f,0.11f,0.11f,0,0},
                 new[]{0,0,0,0.4f,0,0},
                 new[]{0,0,0,0,0.4f,0},
                 new[]{0,0,0,0,0,0.4f}
             });

            imageAttr.SetColorMatrix(disableMatrix);

            graphics.DrawImage(bmp, rect,
                0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imageAttr);
        }


        /// <summary>
        /// Draws a button control.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="bounds">The bounds that represents the dimensions of the button.</param>
        /// <param name="image">Image for draws on button.</param>
        /// <param name="isPressed">The Button is pressed.</param>
        /// <param name="isFocused">The Button is focused.</param>
        /// <param name="isMouseOverButton">Mouse pointer is over the button.</param>
        /// <param name="enabled">The Button is enabled.</param>
        public static void DrawButton(Graphics graphics, Rectangle bounds, Image image,
            bool isPressed, bool isFocused, bool isMouseOverButton, bool enabled, bool flat)
        {
            #region Flat
            if (flat)
            {
                bounds.Width++;
                bounds.Height++;

                var btColorStart = StiColors.ControlStart;
                var btColorEnd = StiColors.ControlEnd;

                #region isMouseOverButton
                if (isMouseOverButton)
                {
                    btColorStart = StiColors.ControlStartLight;
                    btColorEnd = StiColors.ControlEndLight;
                }
                #endregion

                #region isPressed
                if (isPressed)
                {
                    btColorStart = StiColors.ControlStartDark;
                    btColorEnd = StiColors.ControlEndDark;
                }
                #endregion

                if (!enabled) btColorStart = btColorEnd;

                using (Brush brush = new LinearGradientBrush(bounds, btColorStart, btColorEnd, 90))
                {
                    graphics.FillRectangle(brush, bounds);
                }

                var color = SystemColors.ControlDark;

                if (isFocused) color = StiColors.SelectedText;

                using (var pen = new Pen(color))
                {
                    bounds.X--;
                    bounds.Y--;
                    bounds.Width++;
                    bounds.Height++;
                    graphics.DrawRectangle(pen, bounds);
                }

            }
            #endregion

            #region 3D
            else
            {
                var state = ButtonState.Normal;
                if (isPressed) state = ButtonState.Pushed;
                if (!enabled) state = ButtonState.Inactive;

                bounds.Width++;
                bounds.Height++;

                if (bounds.Width > 0 && bounds.Height > 0)
                    ControlPaint.DrawButton(graphics, bounds, state);

            }
            #endregion

            #region PaintImage
            if (isPressed)
            {
                bounds.X++;
                bounds.Y++;
            }

            if (image != null)
            {
                if (flat)
                {
                    bounds.X++;
                    bounds.Y++;
                }

                if (enabled)
                {
                    graphics.DrawImage(image, new Rectangle(
                        bounds.X + (bounds.Width - image.Width) / 2,
                        bounds.Y + (bounds.Height - image.Height - 1) / 2,
                        image.Width, image.Height));
                }
                else
                {
                    StiControlPaint.DrawImageDisabled(graphics, image,
                        bounds.X + (bounds.Width - image.Width) / 2,
                        bounds.Y + (bounds.Height - image.Height - 1) / 2);
                }
            }
            #endregion
        }

        /// <summary>
        /// Draws a border.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="bounds">The bounds that represents the dimensions of the border rectangle.</param>
        /// <param name="isFocused">The Border is focused.</param>
        public static void DrawBorder(Graphics graphics, Rectangle bounds, bool isMouseOver, bool isFocused, bool isEnabled)
        {
            var color = StiUX.InputBorder;

            if (!isEnabled)
                color = StiUX.InputBorderDisabled;

            else if (isFocused)
                color = StiUX.InputBorderFocused;

            else if (isMouseOver)
                color = StiUX.InputBorderMouseOver;

            StiDrawing.DrawRectangle(graphics, color, bounds.X, bounds.Y,
                bounds.Width - 1, bounds.Height - 1);
        }

        /// <summary>
        /// Draws an item background.
        /// </summary>
        /// <param name="g">The Graphics object to draw on.</param>
        /// <param name="bounds">The bounds that represents the dimensions of the item rectangle.</param>
        /// <param name="state">Specifies the state of an item that is being drawn.</param>
        public static void DrawItemBackground(Graphics g, Rectangle bounds, DrawItemState state, Color? backColor = null)
        {
            bounds.Width++;
            var background = GetItemBackground(state, backColor.GetValueOrDefault(StiUX.InputBackground));
            StiDrawing.FillRectangle(g, background, bounds);
        }

        public static Color GetItemBackground(bool isItemSelected, bool isMouseOver, bool isBoxFocused)
        {
            return GetItemBackground(isItemSelected, isMouseOver, isBoxFocused, false, false, StiUX.ItemBackground);
        }

        public static Color GetItemForeground(DrawItemState state)
        {
            var isItemSelected = (state & DrawItemState.Selected) != 0;
            var isItemMouseOver = (state & DrawItemState.HotLight) != 0;
            var isBoxDisabled = (state & DrawItemState.Disabled) != 0;
            
            return GetItemForeground(isItemSelected, isItemMouseOver, false, isBoxDisabled);
        }

        public static Color GetItemForeground(bool isItemSelected, bool isItemMouseOver, bool isBoxFocused, Color? baseColor = null)
        {
            return GetItemForeground(isItemSelected, isItemMouseOver, isBoxFocused, false, baseColor);
        }

        public static Color GetItemForeground(bool isItemSelected, bool isItemMouseOver, bool isBoxFocused, bool isBoxDisabled, Color? baseColor = null)
        {
            if (isBoxDisabled)
                return StiUX.ItemForegroundDisabled;

            if (isItemMouseOver)
                return isBoxFocused ? StiUX.ItemForegroundActiveMouseOver : StiUX.ItemForegroundMouseOver;

            if (isItemSelected)
                return isBoxFocused ? StiUX.ItemForegroundActiveSelected : StiUX.ItemForegroundSelected;

            return baseColor.GetValueOrDefault(StiUX.ItemForeground);
        }

        public static Color GetItemBackground(DrawItemState state, Color backColor)
        {            
            var isItemSelected = (state & DrawItemState.Selected) != 0;
            var isItemMouseOver = (state & DrawItemState.HotLight) != 0;
            var isBoxDisabled = (state & DrawItemState.Disabled) != 0;
            var isBoxEdit = (state & DrawItemState.ComboBoxEdit) != 0;

            return GetItemBackground(isItemSelected, isItemMouseOver, false, isBoxDisabled, isBoxEdit, backColor);
        }

        public static Color GetItemBackground(bool isItemSelected, bool isItemMouseOver,
            bool isBoxFocused, bool isBoxDisabled, bool isBoxEdit, Color backColor)
        {
            if (isBoxDisabled)
                return StiUX.ItemBackgroundDisabled;

            if (!isBoxEdit)
            {
                if (isItemMouseOver)
                    return isBoxFocused ? StiUX.ItemBackgroundActiveMouseOver : StiUX.ItemBackgroundMouseOver;

                if (isItemSelected)
                    return isBoxFocused ? StiUX.ItemBackgroundActiveSelected : StiUX.ItemBackgroundSelected;
            }
            else
            {
                if (isItemMouseOver)
                    return StiUX.InputBackgroundMouseOver;
            }

            return backColor;
        }

        /// <summary>
		/// Draws a list item.
		/// </summary>
		/// <param name="graphics">The Graphics object to draw on.</param>
		/// <param name="bounds">The bounds that represents the dimensions of the item rectangle.</param>
		/// <param name="state">Specifies the state of an item that is being drawn.</param>
		/// <param name="text">The Text of the item.</param>
		/// <param name="imageList">Specifies the ImageList of an item.</param>
		/// <param name="imageIndex">Specifies the ImageIndex of an item.</param>
		/// <param name="font">The Font to draw the item. </param>
        /// <param name="foreColor">The Color to draw the string with.</param>
		/// <param name="textStartPos">The Position for draws of the text.</param>
		/// <param name="rightToLeft">Specifies that text is right to left.</param>
        public static void DrawItem(Graphics graphics, Rectangle bounds, DrawItemState state, string text,
            ImageList imageList, int imageIndex, Font font, Color backColor, Color foreColor, int textStartPos,
            RightToLeft rightToLeft)
        {
            DrawItem(graphics, bounds, state, text, imageList, imageIndex, font, backColor, foreColor,
                textStartPos, rightToLeft, StringAlignment.Near);
        }

        /// <summary>
        /// Draws a list item.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="bounds">The bounds that represents the dimensions of the item rectangle.</param>
        /// <param name="state">Specifies the state of an item that is being drawn.</param>
        /// <param name="text">The Text of the item.</param>
        /// <param name="imageList">Specifies the ImageList of an item.</param>
        /// <param name="imageIndex">Specifies the ImageIndex of an item.</param>
        /// <param name="font">The Font to draw the item. </param>
        /// <param name="foreColor">The Color to draw the string with.</param>
        /// <param name="textStartPos">The Position for draws of the text.</param>
        /// <param name="rightToLeft">Specifies that text is right to left.</param>
        public static void DrawItem(Graphics graphics, Rectangle bounds, DrawItemState state, string text,
            ImageList imageList, int imageIndex, Font font, Color backColor, Color foreColor, int textStartPos,
            RightToLeft rightToLeft, StringAlignment alignment)
        {
            DrawItemBackground(graphics, bounds, state, backColor);

            #region Paint image
            var imageWidth = 0;

            if (imageList != null && imageIndex >= 0 && imageIndex < imageList.Images.Count)
            {
                var imageRect = new Rectangle(
                    bounds.X + StiScaleUI.I1, bounds.Y + (bounds.Height - imageList.ImageSize.Height) / 2,
                    imageList.ImageSize.Width, imageList.ImageSize.Height);

                imageList.Draw(graphics, imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, imageIndex);

                imageWidth = imageList.ImageSize.Width + StiScaleUI.I2;
            }
            #endregion

            #region Paint text
            if (!string.IsNullOrWhiteSpace(text))
            {
                using (var sf = new StringFormat())
                {
                    sf.Alignment = alignment;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.FormatFlags = StringFormatFlags.NoWrap;
                    sf.Trimming = StringTrimming.EllipsisCharacter;

                    if (rightToLeft == RightToLeft.Yes)
                        sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                    bounds.X += imageWidth + textStartPos;
                    bounds.Width -= imageWidth + textStartPos;


                    var isComboBoxEdit = (state & DrawItemState.ComboBoxEdit) != 0;
                    if (((state & DrawItemState.Focus) != 0 || (state & DrawItemState.Selected) != 0) && !isComboBoxEdit)
                    {
                        using (var brush = new SolidBrush(StiUX.ItemForegroundSelected))
                        {
                            graphics.DrawString(text, font, brush, bounds, sf);
                        }
                    }
                    else
                    {
                        using (var brush = new SolidBrush(foreColor))
                        {
                            graphics.DrawString(text, font, brush, bounds, sf);
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Draws a list item.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="bounds">The bounds that represents the dimensions of the item rectangle.</param>
        /// <param name="state">Specifies the state of an item that is being drawn.</param>
        /// <param name="text">The Text of the item.</param>
        /// <param name="imageList">Specifies the ImageList of an item.</param>
        /// <param name="imageIndex">Specifies the ImageIndex of an item.</param>
        /// <param name="font">The Font to draw the item.</param>
        /// <param name="foreColor">The Color to draw the string with.</param>
        /// <param name="rightToLeft">Specifies that text is right to left.</param>
        public static void DrawItem(Graphics graphics, Rectangle bounds, DrawItemState state, string text,
            ImageList imageList, int imageIndex, Font font, Color backColor, Color foreColor, RightToLeft rightToLeft)
        {
            DrawItem(graphics, bounds, state, text, imageList, imageIndex, font, backColor, foreColor, 0, rightToLeft);
        }

        /// <summary>
        /// Draws a check.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="x">The X coordinate of the top left of the check.</param>
        /// <param name="y">The Y coordinate of the top left of the check.</param>
        public static void DrawCheck(Graphics graphics, int x, int y, bool enabled)
        {
            x -= StiScale.I3;
            y -= StiScale.I3;
            var p1 = new Point(x, y + StiScale.I2);
            var p2 = new Point(x + StiScale.I2, y + StiScale.I4);
            var p3 = new Point(x + StiScale.I6, y);

            var color = Color.Black;
            if (!enabled)
                color = SystemColors.ControlDark;

            using (var pen = new Pen(color))
            {
                graphics.DrawLine(pen, p1, p2);
                graphics.DrawLine(pen, p2, p3);

                p1.Y++;
                p2.Y++;
                p3.Y++;

                graphics.DrawLine(pen, p1, p2);
                graphics.DrawLine(pen, p2, p3);

                p1.Y++;
                p2.Y++;
                p3.Y++;

                graphics.DrawLine(pen, p1, p2);
                graphics.DrawLine(pen, p2, p3);
            }
        }

        /// <summary>
        /// Draws a focus rectangle with button.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="bounds">The bounds that represents the dimensions of the focus rectangle.</param>
        /// <param name="buttonBounds">The bounds that represents the dimensions of the button rectangle.</param>
        public static void DrawFocus(Graphics graphics, Rectangle bounds, Rectangle buttonBounds)
        {
            var focusedRect = new Rectangle(
                bounds.X + StiScale.I2, bounds.Y + StiScale.I2,
                bounds.Width - buttonBounds.Width - StiScale.I4, bounds.Height - StiScale.I3);

            ControlPaint.DrawFocusRectangle(graphics, focusedRect);
        }

        /// <summary>
        /// Draws a focus rectangle.
        /// </summary>
        /// <param name="graphics">The Graphics object to draw on.</param>
        /// <param name="bounds">The bounds that represents the dimensions of the focus rectangle.</param>
        public static void DrawFocus(Graphics graphics, Rectangle bounds)
        {
            var focusedRect = new Rectangle(
                bounds.X + StiScale.I2, bounds.Y + StiScale.I2,
                bounds.Width - StiScale.I3, bounds.Height - StiScale.I3);

            ControlPaint.DrawFocusRectangle(graphics, focusedRect);
        }

        /// <summary>
        /// Scrolls the contents of the specified window's client area.
        /// </summary>
        /// <param name="hWnd">Handle to the window where the client area is to be scrolled.</param>
        /// <param name="xAmount">Specifies the amount, in device units, of horizontal scrolling.</param>
        /// <param name="yAmount">Specifies the amount, in device units, of vertical scrolling.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        public static bool ScrollWindow(IntPtr hWnd, int xAmount, int yAmount)
        {
            return Win32.ScrollWindowEx(hWnd, xAmount, yAmount, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 10);
        }

        public static Rectangle GetButtonRect(Rectangle bounds, bool flat, RightToLeft rightToLeft)
        {
            return GetButtonRect(bounds, flat, SystemInformation.HorizontalScrollBarArrowWidth - StiScale.I1, rightToLeft);
        }

        public static Rectangle GetButtonRect(Rectangle bounds, bool flat, int buttonWidth, RightToLeft rightToLeft)
        {
            var borderWidth = SystemInformation.Border3DSize.Width;
            var borderHeight = SystemInformation.Border3DSize.Height;

            if (flat)
                borderWidth = borderHeight = StiScale.I1;

            if (rightToLeft == RightToLeft.Yes)
            {
                return new Rectangle(
                    bounds.Left + borderWidth,
                    bounds.Top + borderHeight,
                    buttonWidth + StiScale.I1,
                    bounds.Height - borderHeight * 2);
            }
            else
            {
                return new Rectangle(
                    bounds.Right - buttonWidth - borderWidth,
                    bounds.Top + borderHeight + StiScale.I1,
                    buttonWidth,
                    bounds.Height - borderHeight * 2 - StiScale.I1);
            }
        }

        public static Rectangle GetContentRect(Rectangle bounds, bool flat, RightToLeft rightToLeft)
        {
            var buttonRect = GetButtonRect(bounds, flat, rightToLeft);

            var borderWidth = SystemInformation.Border3DSize.Width;
            var borderHeight = SystemInformation.Border3DSize.Height;
            if (flat)
                borderWidth = borderHeight = StiScale.I1;

            if (rightToLeft == RightToLeft.Yes)
            {
                return new Rectangle(bounds.Left + borderWidth + buttonRect.Width, bounds.Top + borderHeight,
                    bounds.Width - borderWidth * 2, bounds.Height - borderHeight * 2);
            }
            else
            {
                return new Rectangle(bounds.Left + borderWidth, bounds.Top + borderHeight,
                    bounds.Width - borderWidth * 2 - buttonRect.Width - StiScale.I2, bounds.Height - borderHeight * 2);
            }
        }
        #endregion
    }
}
