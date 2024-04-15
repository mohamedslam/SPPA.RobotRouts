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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Brushes = Stimulsoft.Drawing.Brushes;
#endif

namespace Stimulsoft.Report.Painters
{
    public abstract class StiLinePrimitiveGdiPainter : StiComponentGdiPainter
    {
        #region Methods
        public virtual void DrawDot(Graphics g, Pen pen, double x, double y, float size)
        {
            var oldSmoothingMode = g.SmoothingMode;
            if (!StiOptions.Engine.DisableAntialiasingInPainters) g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF((float)x - size, (float)y - size, size * 2, size * 2);

            g.FillEllipse(Brushes.White, rect);
            g.DrawEllipse(pen, rect);

            g.SmoothingMode = oldSmoothingMode;
        }

        public void AddDotToPath(GraphicsPath path, double x, double y, float size)
        {
            var rect = new RectangleF((float)x - size, (float)y - size, size * 2, size * 2);

            path.AddEllipse(rect);
        }       
        #endregion

        #region Methods.Painter
        public override void PaintHighlight(StiComponent component, StiPaintEventArgs e)
        {
        }

        protected void CheckRectForOverflow(ref RectangleD rect)
        {
            int maxValue = int.MaxValue / 4;
            if (rect.X > maxValue) rect.X = maxValue;
            if (rect.X < -maxValue) rect.X = -maxValue;
            if (rect.Width > maxValue) rect.Width = maxValue;
            if (rect.Width < -maxValue) rect.Width = -maxValue;
            if (rect.Y > maxValue) rect.Y = maxValue;
            if (rect.Y < -maxValue) rect.Y = -maxValue;
            if (rect.Height > maxValue) rect.Height = maxValue;
            if (rect.Height < -maxValue) rect.Height = -maxValue;
        }
        #endregion
    }
}
