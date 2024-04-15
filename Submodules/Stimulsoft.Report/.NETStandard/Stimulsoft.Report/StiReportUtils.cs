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
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
#endif

namespace Stimulsoft.Report
{
    public class StiReportUtils
	{
		public static void MakeBlackAndWhite(StiReport report)
		{
			foreach (StiPage page in report.RenderedPages)
			{
				var comps = page.GetComponents();
				foreach (StiComponent comp in comps)
				{
					var brush = comp as IStiBrush;
				    if (brush != null)
				        brush.Brush = new StiSolidBrush(ConvertColor(StiBrush.ToColor(brush.Brush), 200));

				    var textBrush = comp as IStiTextBrush;
				    if (textBrush != null)
				        textBrush.TextBrush = new StiSolidBrush(ConvertColor(StiBrush.ToColor(textBrush.TextBrush), 100));

				    var border = comp as IStiBorder;
					if (border != null)
					{
						border.Border.Color = ConvertColor(border.Border.Color, 100);
						border.Border.ShadowBrush = new StiSolidBrush(ConvertColor(StiBrush.ToColor(border.Border.ShadowBrush), 200));
					}

					var backColor = comp as IStiBackColor;
				    if (backColor != null)
				        backColor.BackColor = ConvertColor(backColor.BackColor, 200);

				    var foreColor = comp as IStiForeColor;
				    if (foreColor != null)
				        foreColor.ForeColor = ConvertColor(foreColor.ForeColor, 100);

				    var borderColor = comp as IStiBorderColor;
				    if (borderColor != null)
				        borderColor.BorderColor = ConvertColor(borderColor.BorderColor, 100);

				    var primitive = comp as StiLinePrimitive;
				    if (primitive != null)
				        primitive.Color = ConvertColor(primitive.Color, 100);

				    var image = comp as StiImage;
					if (image != null)
					{
					    using (var gdiImage = image.TakeGdiImageToDraw())
					    {
					        image.PutImageToDraw(ConvertImage(gdiImage));
					    }
					}
				}
			}
		}

		private static Image ConvertImage(Image image)
		{
			if (image is Metafile) return image;
		    if (image == null) return null;

		    var bmp = image as Bitmap;
		    var newBmp = new Bitmap(bmp.Width, bmp.Height);
		    var g = Graphics.FromImage(newBmp);

		    var matrix = new ColorMatrix(new float[][]
		    {
		        new float[]{0.3f, 0.3f, 0.3f, 0, 0},
		        new float[]{0.55f, 0.55f, 0.55f, 0, 0},
		        new float[]{0.15f, 0.15f, 0.15f, 0, 0},
		        new float[]{0, 0, 0, 1, 0, 0},
		        new float[]{0, 0, 0, 0, 1, 0},
		        new float[]{0, 0, 0, 0, 0, 1}
		    });

		    var attributes = new ImageAttributes();
		    attributes.SetColorMatrix(matrix);
		    g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
		        0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
		    g.Dispose();

		    bmp.Dispose();
		    return newBmp;
		}

		private static Color ConvertColor(Color color, int range)
		{
			var value = (color.R + color.G + color.B) / 3;
		    return Color.FromArgb(color.A, value >= range ? Color.White : Color.Black);
		}
	}
}
