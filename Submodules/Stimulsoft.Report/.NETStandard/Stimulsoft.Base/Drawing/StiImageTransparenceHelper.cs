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
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiImageTransparenceHelper
    {
        public static Bitmap GetTransparentedImage(Image source, float transparency)
        {
            if (source == null) 
                return null;

            float[][] matrix = 
            {
                new []{1f, 0f, 0f, 0f, 0f},
                new []{0f, 1f, 0f, 0f, 0f},
                new []{0f, 0f, 1f, 0f, 0f},
                new []{0f, 0f, 0f, transparency, 0f},
                new []{0f, 0f, 0f, 0f, 1f}};


            var attrs = new ImageAttributes();
            attrs.SetColorMatrix(new ColorMatrix(matrix), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            var destination = new Bitmap(source.Width, source.Height);
            using (var g = Graphics.FromImage(destination))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(source,
                    new Rectangle(Point.Empty, source.Size),
                    0, 0, source.Width, source.Height,
                    GraphicsUnit.Pixel, attrs);
            }
            return destination;
        } 
    }
}
