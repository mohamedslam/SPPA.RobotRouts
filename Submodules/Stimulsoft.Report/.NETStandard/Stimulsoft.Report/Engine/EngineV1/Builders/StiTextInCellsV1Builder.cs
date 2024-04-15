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
using System.Collections;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiTextInCellsV1Builder : StiSimpleTextV1Builder
	{
		#region Methods.Render
		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// The rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">Rendered component.</param>
		/// <param name="outContainer">Panel in what rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiTextInCells masterTextInCells = masterComp as StiTextInCells;
			bool result = base.InternalRender(masterTextInCells, ref renderedComponent, outContainer);
			if (outContainer.Components.IndexOf(renderedComponent) != -1)outContainer.Components.Remove(renderedComponent);

			StiContainer cont = new StiContainer();			
			cont.ClientRectangle = renderedComponent.ClientRectangle;
			cont.Name = renderedComponent.Name;
 
			outContainer.Components.Add(cont);

			StiText masterText = renderedComponent as StiText;

			RectangleD rect = cont.ClientRectangle;
			rect.X = 0;
			rect.Y = 0;

			StiUnit unit = masterTextInCells.Page.Unit;
			double borderSize = unit.ConvertFromHInches(masterTextInCells.Border.Size / 2 * masterTextInCells.Page.Zoom);

			double horSpacing = masterTextInCells.HorSpacing;
			double vertSpacing = masterTextInCells.VertSpacing;
			double cellWidth = masterTextInCells.CellWidth;
			double cellHeight = masterTextInCells.CellHeight;

			if (masterTextInCells.CellWidth == 0)cellWidth = unit.ConvertFromHInches(masterTextInCells.Font.GetHeight() * 1.5f);
			if (masterTextInCells.CellHeight == 0)cellHeight = unit.ConvertFromHInches(masterTextInCells.Font.GetHeight() * 1.5f);

			if (!masterTextInCells.ContinuousText)
			{
				#region New mode

				#region Calculate text size
				Size textSize = new Size(1, 1);
				double posX = (float)(rect.X + borderSize + cellWidth);
				while (posX + horSpacing + cellWidth < rect.Right)
				{
					posX += horSpacing + cellWidth;
					textSize.Width++;
				}
				double posY = (float)(rect.Y + borderSize + cellHeight);
				while (posY + vertSpacing + cellHeight < rect.Bottom)
				{
					posY += vertSpacing + cellHeight;
					textSize.Height++;
				}
				if (!masterTextInCells.WordWrap) textSize.Height = 1;
				#endregion

				#region Make string list
				ArrayList stringList = new ArrayList();
				string st = string.Empty;
				if (masterText.GetTextInternal() != null)
				{
                    string str = masterText.GetTextInternal();
                    foreach (char ch in str)
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
				}
				if (st != string.Empty)	stringList.Add(StiTextInCellsHelper.TrimEndWhiteSpace(st));
				if (stringList.Count == 0) stringList.Add(st);
				#endregion

				#region Wordwrap
				if (masterTextInCells.WordWrap)
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
					if (masterTextInCells.HorAlignment == StiTextHorAlignment.Center) textOffset = (textSize.Width - currentLineText.Length) / 2;
					if (masterTextInCells.HorAlignment == StiTextHorAlignment.Right) textOffset = textSize.Width - currentLineText.Length;
					if (textOffset > 0) currentLineText = new string(' ', textOffset) + currentLineText;
					#endregion

					posX = (float)(rect.X + borderSize);
					for (int indexX = 0; indexX < textSize.Width; indexX++)
					{
						RectangleD sectorRect = new RectangleD(posX, posY, cellWidth, cellHeight);

						StiText text = renderedComponent.Clone() as StiText;
						text.ClientRectangle = sectorRect;
						text.HorAlignment = StiTextHorAlignment.Center;
						text.VertAlignment = StiVertAlignment.Center;
						cont.Components.Add(text);

						string cellText = string.Empty;
						int indexText = (masterTextInCells.RightToLeft ? textSize.Width - indexX - 1 : indexX);
						if (indexText < currentLineText.Length)
						{
							cellText = new string(currentLineText[indexText], 1);
						}
						text.SetTextInternal(cellText);

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
				double posX = rect.X + borderSize;
				double posY = rect.Y + borderSize;
				bool first = true;
				int index = 0;
				while (1 == 1)
				{
					RectangleD sectorRect = new RectangleD(posX, posY, cellWidth, cellHeight);

					if (sectorRect.Right + horSpacing < rect.Right || first)
					{
						StiText text = renderedComponent.Clone() as StiText;
						text.ClientRectangle = sectorRect;
						text.HorAlignment = StiTextHorAlignment.Center;
						text.VertAlignment = StiVertAlignment.Center;
						cont.Components.Add(text);

						string cellText = string.Empty;
                        if (masterText.GetTextInternal() != null && index < masterText.GetTextInternal().Length)
						{
                            cellText = new string(masterText.GetTextInternal()[index], 1);
						}
						text.SetTextInternal(cellText);
							
						posX += cellWidth + horSpacing;
						index++;
						first = false;
					}
					else
					{
						posY += cellHeight + vertSpacing;

						posX = (float)rect.X + borderSize;
						first = true;

						if ((!masterTextInCells.WordWrap) || rect.Bottom < (posY + cellHeight + vertSpacing))break;
					}
				}
				#endregion
			}
			return result;
		}
		#endregion
	}
}
