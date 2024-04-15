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

using System.Collections;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Brush = Stimulsoft.Drawing.Brush;
using Brushes = Stimulsoft.Drawing.Brushes;
using Image = Stimulsoft.Drawing.Image;
using Font = Stimulsoft.Drawing.Font;
using GraphicsState = Stimulsoft.Drawing.Drawing2D.GraphicsState;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiTextInCellsGdiPainter : StiTextGdiPainter
    {
        public override void PaintBorder(StiText textComp, Graphics g, RectangleD rect, bool drawBorderFormatting, bool drawTopmostBorderSides)
        {
            if ((!textComp.IsDesigning) || textComp.Report.IsPageDesigner)
            {
                base.PaintBorder(textComp, g, rect, drawBorderFormatting, drawTopmostBorderSides);
                return;
            }
        }

        public override void PaintBackground(StiText textComp, Graphics g, RectangleD rect)
        {
            if (textComp.IsDesigning)
            {
                Color color = Color.FromArgb(150, Color.White);
                StiDrawing.FillRectangle(g, color, rect.Left, rect.Top, rect.Width, rect.Height);
            }

            if ((!textComp.IsDesigning) || textComp.Report.IsPageDesigner)
            {
                base.PaintBackground(textComp, g, rect);
                return;
            }
        }

        public override void PaintText(StiText textComp, Graphics g, RectangleD rect)
        {
            var textInCells = textComp as StiTextInCells;

            if ((!textComp.IsDesigning) || textComp.Report.IsPageDesigner)
            {
                base.PaintText(textComp, g, rect);
                return;
            }            

			float borderSize = (float)(textInCells.Border.Size / 2 * textInCells.Page.Zoom);

			GraphicsState state = g.Save();
			g.SetClip(rect.ToRectangleF());

			string text = textInCells.GetTextForPaint();

			var unit = textInCells.Page.Unit;

			float horSpacing = (float)unit.ConvertToHInches(textInCells.HorSpacing * textInCells.Page.Zoom);
			float vertSpacing = (float)unit.ConvertToHInches(textInCells.VertSpacing * textInCells.Page.Zoom);
			float cellWidth = (float)unit.ConvertToHInches(textInCells.CellWidth * textInCells.Page.Zoom);
			float cellHeight = (float)unit.ConvertToHInches(textInCells.CellHeight * textInCells.Page.Zoom);

            if (textInCells.CellWidth == 0) cellWidth = (float)(textInCells.Font.GetHeight() * 1.5f * textInCells.Page.Zoom /* * StiDpiHelper.DeviceCapsScale */);
            if (textInCells.CellHeight == 0) cellHeight = (float)(textInCells.Font.GetHeight() * 1.5f * textInCells.Page.Zoom /* * StiDpiHelper.DeviceCapsScale */);

			float fontSize = (float)(textInCells.Font.Size * textInCells.Page.Zoom /* * StiDpiHelper.DeviceCapsScale */);

			using (var sf = new StringFormat())
			using (Font font = StiFontUtils.ChangeFontSize(textInCells.Font, fontSize))
			{
				sf.LineAlignment = StringAlignment.Center;
				sf.Alignment = StringAlignment.Center;

				if (!textInCells.ContinuousText)
				{
					#region New mode

					#region Calculate text size
					Size textSize = new Size(1, 1);
					float posX = (float)(rect.X + borderSize + cellWidth);
					while (posX + horSpacing + cellWidth < rect.Right)
					{
						posX += horSpacing + cellWidth;
						textSize.Width++;
					}
					float posY = (float)(rect.Y + borderSize + cellHeight);
					while (posY + vertSpacing + cellHeight < rect.Bottom)
					{
						posY += vertSpacing + cellHeight;
						textSize.Height++;
					}
					if (!textInCells.WordWrap) textSize.Height = 1;
					#endregion

					#region Make string list
					ArrayList stringList = new ArrayList();
					string st = string.Empty;
					if (text == null) text = string.Empty;
					foreach (char ch in text)
					{
						if (char.IsControl(ch))
						{
							if (ch == '\n')
							{
								stringList.Add(StiTextInCellsHelper.TrimEndWhiteSpace(st));
								st = string.Empty;
							}
						}
						else
						{
							st += ch;
						}
					}
					if (st != string.Empty)	stringList.Add(StiTextInCellsHelper.TrimEndWhiteSpace(st));
					if (stringList.Count == 0) stringList.Add(st);
					#endregion

					#region Wordwrap
					if (textInCells.WordWrap)
					{
						for (int indexLine = 0; indexLine < stringList.Count; indexLine++)
						{
							string stt = (string)stringList[indexLine];
							if (stt.Length > textSize.Width)
							{
								int[] wordarr = new int[stt.Length];
								int wordCounter = 0;
								int tempIndexSpace = 0;
								while((tempIndexSpace < stt.Length) && char.IsWhiteSpace(stt[tempIndexSpace]))
								{
									wordarr[tempIndexSpace] = wordCounter;
									tempIndexSpace++;
								}
								for(int tempIndex = tempIndexSpace; tempIndex < stt.Length; tempIndex++)
								{
									if (char.IsWhiteSpace(stt[tempIndex])) wordCounter++;
									wordarr[tempIndex] = wordCounter;
								}
								int index = textSize.Width;
								int index2 = index - 1;
								//check words number; if no first - go to begin, else to end of word
								if (wordarr[index] > 0)	//word is no first
								{
									if (wordarr[index] != wordarr[index2])	//end of word
									{
										while(char.IsWhiteSpace(stt[index])) index++;
									}
									else
									{
										while(!char.IsWhiteSpace(stt[index])) index--;
										index2 = index++;
										while(char.IsWhiteSpace(stt[index2])) index2--;
									}
								}
								stringList[indexLine] = stt.Substring(0, index2 + 1);
								stringList.Insert(indexLine + 1, stt.Substring(index, stt.Length - index));
							}
						}
					}
					#endregion

					#region Paint
					posY = (float)(rect.Y + borderSize);
					for (int indexY = 0; indexY < textSize.Height; indexY++)
					{
						string currentLineText = (indexY < stringList.Count ? (string)stringList[indexY] : string.Empty);

						#region HorAlignment
						int textOffset = 0;
						if (textInCells.HorAlignment == StiTextHorAlignment.Center) textOffset = (textSize.Width - currentLineText.Length) / 2;
						if (textInCells.HorAlignment == StiTextHorAlignment.Right) textOffset = textSize.Width - currentLineText.Length;
						if (textOffset > 0) currentLineText = new string(' ', textOffset) + currentLineText;
						#endregion

						posX = (float)(rect.X + borderSize);
						for (int indexX = 0; indexX < textSize.Width; indexX++)
						{
							RectangleF cellRect = new RectangleF(posX, posY, cellWidth, cellHeight);

							using (Brush backBrush = StiBrush.GetBrush(textInCells.Brush, cellRect))
							{
								g.FillRectangle(backBrush, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
							}

							int indexText = (textInCells.RightToLeft ? textSize.Width - indexX - 1 : indexX);
							if (indexText < currentLineText.Length)
							{
								using (Brush brush = StiBrush.GetBrush(textInCells.TextBrush, cellRect))
								{
									g.DrawString(new string(currentLineText[indexText], 1), font, brush, cellRect, sf);
								}
							}

							PaintBorder(textInCells, g, RectangleD.CreateFromRectangle(cellRect), textInCells.Page.Zoom,
                                true, true);
							
							posX += cellWidth + horSpacing;
						}
						posY += cellHeight + vertSpacing;
					}
					#endregion

					#endregion
				}
				else
				{
					#region Old mode
					float posX = (float)(rect.X + borderSize);
					float posY = (float)(rect.Y + borderSize);

					int widthX = (int)((rect.Width - borderSize) / (cellWidth + horSpacing));
					if (widthX * (cellWidth + horSpacing) > (rect.Width - borderSize)) widthX++;

					bool first = true;
					int index = 0;
					while (1 == 1)
					{
						RectangleF cellRect = new RectangleF(posX, posY, cellWidth, cellHeight);

						if (cellRect.Right + horSpacing < rect.Right || first)
						{
							using (Brush backBrush = StiBrush.GetBrush(textInCells.Brush, cellRect))
							{
								g.FillRectangle(backBrush, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
							}

							int indexX = index;
							if (textInCells.RightToLeft)
							{
								int a1 = (index / widthX);
								int a2 = (index % widthX);
								indexX = (a1 + 1) * widthX - a2 - 1;
							}

							if (text != null && indexX < text.Length)
							{
								using (Brush brush = StiBrush.GetBrush(textInCells.TextBrush, cellRect))
								{
									g.DrawString(new string(text[indexX], 1), font, brush, cellRect, sf);
								}
							}

							PaintBorder(textInCells, g, RectangleD.CreateFromRectangle(cellRect), textInCells.Page.Zoom,
                                true, true);
							
							posX += cellWidth + horSpacing;
							index++;
							first = false;
						}
						else
						{
							posY += cellHeight + vertSpacing;

							posX = (float)rect.X + borderSize;
							first = true;

							if ((!textInCells.WordWrap) || rect.Bottom < (posY + cellHeight + vertSpacing))break;
						}					
					}
					#endregion
				}
			}

            g.Restore(state);
        }

        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var text = component as StiTextInCells;
            double resZoom = text.Report.Info.Zoom;
            text.Report.Info.Zoom = zoom;
            var rect = text.GetPaintRectangle();
            rect.X = 0;
            rect.Y = 0;

            int imageWidth = (int)rect.Width;
            int imageHeight = (int)rect.Height;

            //Bitmap bmp = new Bitmap(imageWidth, imageHeight);
            var bmp = new Bitmap(imageWidth, imageHeight);

            using (var g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                if (format != StiExportFormat.ImagePng)
                {
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, imageWidth, imageHeight));
                }
                else
                {
                    g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, imageWidth, imageHeight));
                }

                rect = PaintIndicator(text, g, rect);

                rect = text.ConvertTextMargins(rect, true);
                rect = text.ConvertTextBorders(rect, true);

                PaintText(text, g, rect);
            }
            text.Report.Info.Zoom = resZoom;
            return bmp;
        }
    }
}
