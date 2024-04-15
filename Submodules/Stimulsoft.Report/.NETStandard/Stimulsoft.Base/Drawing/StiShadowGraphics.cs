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

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Base.Drawing
{	
	public class StiShadowGraphics : IDisposable
	{
        #region IDisposable
		public void Dispose()
		{
			Graphics.Dispose();
			shadowBitmap.Dispose();
		}
        #endregion

        #region Fields
	    private Bitmap shadowBitmap;
		private float factor = 3f;
        #endregion

	    #region Properties
	    public Graphics Graphics { get; }
        #endregion

        #region Methods
        public void DrawShadow(Graphics g, Rectangle rect, int shadowSize)
		{
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.InterpolationMode = InterpolationMode.HighQualityBilinear;

			g.DrawImage(shadowBitmap, 
				rect.X - 5 * factor + shadowSize,
				rect.Y - 5 * factor + shadowSize,
				rect.Width + 10 * factor, 
				rect.Height + 10 * factor);
		}

		public void DrawShadow(Graphics g, RectangleF rect, float shadowSize)
		{
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.InterpolationMode = InterpolationMode.HighQualityBilinear;

			g.DrawImage(shadowBitmap, 
				rect.X - 5 * factor + shadowSize,
				rect.Y - 5 * factor + shadowSize,
				rect.Width + 10 * factor, 
				rect.Height + 10 * factor);
		}
        #endregion

		public StiShadowGraphics(RectangleF rect)
		{                
			shadowBitmap = new Bitmap(
				(int)Math.Round(rect.Width / factor) + 10, 
				(int)Math.Round(rect.Height / factor) + 10, 
				PixelFormat.Format32bppArgb);

			Graphics = Graphics.FromImage(shadowBitmap);
			Graphics.ScaleTransform(1 / factor, 1 / factor);
			Graphics.TranslateTransform(5 * factor, 5 * factor);
                             
			Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Graphics.CompositingQuality = CompositingQuality.HighQuality;
			Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
		}
	}
}
