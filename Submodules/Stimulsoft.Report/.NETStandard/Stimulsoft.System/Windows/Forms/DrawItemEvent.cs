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

using System;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class DrawItemEventArgs : EventArgs
    {

        private global::System.Drawing.Color backColor;

        private global::System.Drawing.Color foreColor;

        private Font font;

        private readonly Graphics graphics;

        private readonly int index;

        private readonly global::System.Drawing.Rectangle rect;

        private readonly DrawItemState state;

        public DrawItemEventArgs(Graphics graphics, Font font, global::System.Drawing.Rectangle rect,
                             int index, DrawItemState state)
        {
            this.graphics = graphics;
            this.font = font;
            this.rect = rect;
            this.index = index;
            this.state = state;
            this.foreColor = global::System.Drawing.SystemColors.WindowText;
            this.backColor = global::System.Drawing.SystemColors.Window;
        }

        public DrawItemEventArgs(Graphics graphics, Font font, global::System.Drawing.Rectangle rect,
                             int index, DrawItemState state, global::System.Drawing.Color foreColor, global::System.Drawing.Color backColor)
        {
            this.graphics = graphics;
            this.font = font;
            this.rect = rect;
            this.index = index;
            this.state = state;
            this.foreColor = foreColor;
            this.backColor = backColor;
        }

        public global::System.Drawing.Color BackColor
        {
            get
            {
                if ((state & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    return global::System.Drawing.SystemColors.Highlight;
                }
                return backColor;
            }
        }

        public global::System.Drawing.Rectangle Bounds
        {
            get
            {
                return rect;
            }
        }

        public Font Font
        {
            get
            {
                return font;
            }
        }

        public global::System.Drawing.Color ForeColor
        {
            get
            {
                if ((state & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    return global::System.Drawing.SystemColors.HighlightText;
                }
                return foreColor;
            }
        }

        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public DrawItemState State
        {
            get
            {
                return state;
            }
        }

        public virtual void DrawBackground()
        {
            Brush backBrush = new SolidBrush(BackColor);
            Graphics.FillRectangle(backBrush, rect);
            backBrush.Dispose();
        }

        public virtual void DrawFocusRectangle()
        {
            /*if ((state & DrawItemState.Focus) == DrawItemState.Focus
                && (state & DrawItemState.NoFocusRect) != DrawItemState.NoFocusRect)
                ControlPaint.DrawFocusRectangle(Graphics, rect, ForeColor, BackColor);*/
        }
    }
}
