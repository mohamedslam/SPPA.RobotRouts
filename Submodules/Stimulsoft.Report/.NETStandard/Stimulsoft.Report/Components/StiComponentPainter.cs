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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using FontFamily = Stimulsoft.Drawing.FontFamily;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Summary description for StiComponentPainter.
    /// </summary>
    public class StiComponentPainter
	{
		public static void DrawCheckStyle(Graphics g, RectangleD rect, StiCheckStyle checkStyle, 
			StiCheckBox checkBox, float zoom, bool isUIDrawing = false)
		{
			if (checkStyle == StiCheckStyle.None) return;

			var oldSmoothingMode = g.SmoothingMode;
			var state = g.Save();

			try
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.SetClip(rect.ToRectangleF(), CombineMode.Intersect);

				using (var sf = new StringFormat())
				{
					char value = ' ';
					string selectedFontFamily = null;
					int styleIndex = 0;

					#region Set style
					switch (checkStyle)
					{
						case StiCheckStyle.Cross:
							value = (char)251;
							selectedFontFamily = "Wingdings";
							break;

						case StiCheckStyle.Check:
							value = (char)252;
							selectedFontFamily = "Wingdings";
							styleIndex = 1;
							break;

						case StiCheckStyle.CrossRectangle:
							value = (char)253;
							selectedFontFamily = "Wingdings";
							styleIndex = 2;
							break;

						case StiCheckStyle.CheckRectangle:
							value = (char)254;
							selectedFontFamily = "Wingdings";
							styleIndex = 3;
							break;

						case StiCheckStyle.CrossCircle:
							value = (char)86;
							selectedFontFamily = "Wingdings 2";
							styleIndex = 4;
							break;

						case StiCheckStyle.DotCircle:
							value = (char)164;
							selectedFontFamily = "Wingdings";
							styleIndex = 5;
							break;

						case StiCheckStyle.DotRectangle:
							value = (char)169;
							selectedFontFamily = "Wingdings 2";
							styleIndex = 6;
							break;

						case StiCheckStyle.NoneCircle:
							value = (char)161;
							selectedFontFamily = "Wingdings";
							styleIndex = 7;
							break;

						case StiCheckStyle.NoneRectangle:
							value = (char)168;
							selectedFontFamily = "Wingdings";
							styleIndex = 8;
							break;
					}
					#endregion

					bool needAsBitmap = false;
					try
					{
						var textBrush = isUIDrawing 
							? new StiSolidBrush(StiUX.InputGlyph) 
							: checkBox.TextBrush;

						using (var fontFamily = new FontFamily(selectedFontFamily))
						{
							if (fontFamily.GetName(0) != selectedFontFamily)
							{
								needAsBitmap = true;
							}
							else
							{
								using (var gp = new GraphicsPath())
								{
									gp.AddString(value.ToString(), fontFamily, 0,
										Math.Min((float)rect.Height, (float)rect.Width),
										new RectangleF(0, 0, (float)rect.Width, (float)rect.Height), sf);

									var bounds = gp.GetBounds();
									float xx = ((float)rect.Width - bounds.Width) / 2 - bounds.Left;
									float yy = ((float)rect.Height - bounds.Height) / 2 - bounds.Top;

									g.TranslateTransform((float)rect.X + xx, (float)rect.Y + yy);

									using (var brush = StiBrush.GetBrush(textBrush, bounds))
									{
										g.FillPath(brush, gp);
									}

									if (!isUIDrawing)
									{
										using (var pen = new Pen(checkBox.ContourColor))
										{
											pen.Width = (float)(checkBox.Size * zoom);
											g.DrawPath(pen, gp);
										}
									}
								}
							}
						}
					}
					catch
					{
						needAsBitmap = true;
					}

					if (needAsBitmap)
					{
						using (var img = StiImageUtils.GetImage(typeof(StiReport), "Stimulsoft.Report.Images.CheckStyles.png", false))
						{
							var srcRect = new RectangleF(styleIndex * 200, 0, 200, 200);

							var min = Math.Min(rect.Width, rect.Height);
							var destRect = new RectangleF(
								(float)(rect.X + (rect.Width - min) / 2),
								(float)(rect.Y + (rect.Height - min) / 2),
								(float)min,
								(float)min);

							g.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);
						}
					}
				}
			}
			finally
			{
				g.Restore(state);
				g.SmoothingMode = oldSmoothingMode;
			}
		}
	}
}