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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;

namespace Stimulsoft.Report.Engine
{
	public class StiCrossTabV1Builder : StiContainerV1Builder
	{
		#region Methods.Helper
		private void MakeHorAlignment(StiCrossTab masterCrossTab, StiContainer outContainer, int startIndex)
		{
			if (masterCrossTab.HorAlignment == StiCrossHorAlignment.Width)
			{
				StiCrossTabHelper.MakeHorAlignmentByWidth(outContainer, startIndex);
			}
			else if (masterCrossTab.HorAlignment != StiCrossHorAlignment.None)
			{
				double maxLeft = 0;
				double maxRight = 0;
				StiCrossTabHelper.CalculateMaxAndMin(outContainer, ref maxLeft, ref maxRight, startIndex);

				for (int index = startIndex; index < outContainer.Components.Count; index++)
				{
					StiComponent comp = outContainer.Components[index];

					if (masterCrossTab.HorAlignment == StiCrossHorAlignment.Left)
					{
						comp.Left -= maxLeft;
					}
					else if (masterCrossTab.HorAlignment == StiCrossHorAlignment.Right)
					{
						comp.Left += outContainer.Width - maxRight;
					}
					else if (masterCrossTab.HorAlignment == StiCrossHorAlignment.Center)
					{
						double newLeft = (outContainer.Width - (maxRight - maxLeft)) / 2;
						comp.Left -= (maxLeft - newLeft);
					}
				}
			}
		}
		#endregion

		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			base.Prepare(masterComp);

			StiCrossTab masterCrossTab = masterComp as StiCrossTab;
			
			masterCrossTab.CrossTabInfoV1.FinishRender = false;

			StiFilterHelper.SetFilter(masterCrossTab);

			if (!(masterCrossTab.Parent is StiBand))StiCrossTabHelper.CreateCross(masterCrossTab);
		}

		/// <summary>
		/// Renders a component in the specified container with taking events generation into consideration. A rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">A component what is being rendered.</param>
		/// <param name="outContainer">A container in what rendering will be done.</param>
		/// <returns>A value that indicates whether rendering of a component is finished or not.</returns>
		public override bool Render(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiCrossTab masterCrossTab = masterComp as StiCrossTab;
		    
			if ((masterCrossTab.DataSource == null || masterCrossTab.DataSource.IsEmpty) && (!masterCrossTab.PrintIfEmpty))return true;

			if (masterCrossTab.CrossTabInfoV1.FinishRender)return true;
			bool placedOnBand = masterCrossTab.Parent is StiBand;
			if (placedOnBand)StiCrossTabHelper.CreateCross(masterCrossTab);
            var cross = masterCrossTab.CrossTabInfo.Cross;

			#region Create a container for Cross
			var rect = masterCrossTab.GetDockRegion(outContainer).ToRectangleM();
			renderedComponent = GetRenderContainer(outContainer, masterCrossTab);

		    if (!placedOnBand)
		        outContainer.Components.Add(renderedComponent);

		    if (masterCrossTab.CanGrow)
			{
				renderedComponent.Width = (double)rect.Right - renderedComponent.Left;
                renderedComponent.Height = (double)rect.Bottom - renderedComponent.Top;

			    if (placedOnBand)
			        renderedComponent.Height = 100000000;
			}

		    if (masterCrossTab.CrossTabInfoV1.RenderRect.IsEmpty)
		        masterCrossTab.CrossTabInfoV1.RenderRect = renderedComponent.ClientRectangle;
		    else
		        renderedComponent.ClientRectangle = masterCrossTab.CrossTabInfoV1.RenderRect;
		    #endregion			

			rect = renderedComponent.ClientRectangle.ToRectangleM();

			int endRow = masterCrossTab.CrossTabInfoV1.StartRow;
			int endCol = masterCrossTab.CrossTabInfoV1.StartCol;

			#region Wrap Cross-Tab
			if (masterCrossTab.Wrap)
			{
				masterCrossTab.CrossTabInfoV1.StartCol = 0;
				endRow = StiCrossTabHelper.GetEndRow(masterCrossTab, 0, rect);

			    while (masterCrossTab.CrossTabInfoV1.StartCol < cross.ColCount)
				{
					endCol = StiCrossTabHelper.GetEndCol(masterCrossTab, masterCrossTab.CrossTabInfoV1.StartCol, rect);

					int startIndex = outContainer.Components.Count;
					
					StiCrossTabHelper.RenderCells(masterCrossTab, outContainer, masterCrossTab.CrossTabInfoV1.StartCol, 0, endCol, endRow, rect);

					var maxHeight = 0m;
					for (int index = startIndex; index < outContainer.Components.Count; index++)
					{
						StiComponent comp = outContainer.Components[index];
						maxHeight = Math.Max((decimal)comp.Bottom, maxHeight);
					}
					rect.Y = maxHeight + (decimal)masterCrossTab.WrapGap;
					masterCrossTab.CrossTabInfoV1.StartCol = endCol;// + 1;
				}				
				return true;
			}
			#endregion

			#region Placed on DataBand
			if (placedOnBand)
			{
				masterCrossTab.CrossTabInfoV1.StartCol = 0;

				StiPage page = outContainer.Page;

				var pageWidthStep = (decimal)(page.PageWidth - page.Margins.Left - page.Margins.Right);
				var pageWidth = pageWidthStep;

				endRow = StiCrossTabHelper.GetEndRow(masterCrossTab, 0, rect);
			    int pageSegment = 1;

				if (!page.UnlimitedBreakable)rect.Width = 1000000000;

				int startIndex = outContainer.Components.Count;

				while (masterCrossTab.CrossTabInfoV1.StartCol < cross.ColCount)
				{
					endCol = StiCrossTabHelper.GetEndCol(masterCrossTab, masterCrossTab.CrossTabInfoV1.StartCol, rect);					
					
					StiCrossTabHelper.RenderCells(masterCrossTab, outContainer, masterCrossTab.CrossTabInfoV1.StartCol, 0, endCol, endRow, rect);

					if (rect.X < pageWidth)rect.X = pageWidth;
					else rect.X += pageWidth;

					rect.Width = pageWidthStep;

					masterCrossTab.CrossTabInfoV1.StartCol = endCol;// + 1;

					pageSegment ++;
					if (masterCrossTab.CrossTabInfoV1.StartCol < cross.ColCount)page.SegmentPerWidth  = pageSegment;
				}

				MakeHorAlignment(masterCrossTab, outContainer, startIndex);

				if (!page.UnlimitedBreakable)
				{
					#region Increase SegmentPerWidth
					double maxRight2 = 0;
					for (int compIndex = startIndex; compIndex < outContainer.Components.Count; compIndex++)
					{
						StiComponent comp = outContainer.Components[compIndex];
						maxRight2 = Math.Max(comp.Right, maxRight2);
					}

					while (page.Width < maxRight2)
					{
						page.SegmentPerWidth ++;
					}
					#endregion
				}
				
				return true;
			}
			#endregion

			rect.X = 0;
			rect.Y = 0;

			endCol = StiCrossTabHelper.GetEndCol(masterCrossTab, masterCrossTab.CrossTabInfoV1.StartCol, rect);
			endRow = StiCrossTabHelper.GetEndRow(masterCrossTab, masterCrossTab.CrossTabInfoV1.StartRow, rect);

			bool crossTitleVisible = 
				cross.LeftCrossTitle != null && cross.LeftCrossTitle.Enabled && cross.RightCrossTitle != null && cross.RightCrossTitle.Enabled;

			bool crossTitlePrintOnAllPages = 
				cross.LeftCrossTitle != null && cross.LeftCrossTitle.PrintOnAllPages && 
				cross.RightCrossTitle != null && cross.RightCrossTitle.PrintOnAllPages && !masterCrossTab.RightToLeft;

			#region Calculation startPos
			var stX = rect.X;
			var stY = rect.Y;
			var resX = rect.X;
			var resY = rect.Y;

			if (masterCrossTab.CrossTabInfoV1.StartCol != 0)
			{
				for (int rowIndex = 0; rowIndex < cross.RowFields.Count; rowIndex++)
				{
                    if (((StiCrossHeader)cross.RowFields[rowIndex]).PrintOnAllPages && (!masterCrossTab.RightToLeft))
					{
                        var colWidth = cross.Widths[rowIndex];
						stX += colWidth;
						rect.Width -= colWidth;
					}
				}				
			}

			if (masterCrossTab.CrossTabInfoV1.StartRow != 0)
			{
				for (int colIndex = 0; colIndex < cross.ColFields.Count; colIndex++)
				{
                    if (((StiCrossHeader)cross.ColFields[colIndex]).PrintOnAllPages && (!masterCrossTab.RightToLeft))
					{
                        stY += cross.Heights[colIndex];
                        rect.Height -= cross.Heights[colIndex];
					}
				}
			}

            if (crossTitleVisible && crossTitlePrintOnAllPages && masterCrossTab.CrossTabInfoV1.StartRow != 0 && (!masterCrossTab.RightToLeft))
			{
                stY += cross.Heights[1];
                rect.Height -= cross.Heights[1];
			}
			#endregion

			#region Row Headers
			if (masterCrossTab.CrossTabInfoV1.StartCol != 0 && (!StiCrossTabHelper.IsRowFieldsEmpty(masterCrossTab)))
			{
				rect.X = resX;
				rect.Y = stY;

				#region Get Actual End Row
				var ht = rect.Height;
				int endRW = masterCrossTab.CrossTabInfoV1.StartRow;				
				for (int rwIndex = masterCrossTab.CrossTabInfoV1.StartRow; rwIndex < endRow; rwIndex++)
				{
                    ht -= cross.Heights[rwIndex];
					if (ht < 0)break;
					endRW++;
				}
				endRow = endRW;
				#endregion

				int rowIndex = 0;
				while (rowIndex < cross.RowFields.Count)
				{
					int rowIndexEnd = rowIndex;

					while (rowIndexEnd < cross.RowFields.Count &&
                        ((StiCrossHeader)cross.RowFields[rowIndexEnd]).PrintOnAllPages && (!masterCrossTab.RightToLeft))
					{
						rowIndexEnd ++;
					}

					if (rowIndex != rowIndexEnd)
					{
                        if (masterCrossTab.CrossTabInfoV1.StartRow > 0)
                        {
                            var rectTitle = rect;
                            rectTitle.Y = 0;
                            StiCrossTabHelper.RenderCells(masterCrossTab, renderedComponent as StiContainer, 0, 0, cross.RowFields.Count, 2, rectTitle);
                        }

						StiCrossTabHelper.RenderCells(masterCrossTab, renderedComponent as StiContainer, rowIndex, masterCrossTab.CrossTabInfoV1.StartRow, rowIndexEnd, endRow, rect);
						rect.X += cross.Widths[rowIndex];
					}
					rowIndex = rowIndexEnd + 1;
				}
			}
			#endregion

			#region Col Headers
			if (masterCrossTab.CrossTabInfoV1.StartRow != 0)
			{
				rect.X = stX;
				rect.Y = resY;

				#region Get Actual EndColumn
				var wd = rect.Width;
				int endColumn = masterCrossTab.CrossTabInfoV1.StartCol;				
				for (int clIndex = masterCrossTab.CrossTabInfoV1.StartCol; clIndex < endCol; clIndex++)
				{
                    wd -= cross.Widths[clIndex];
					if (wd < 0)break;
					endColumn++;
				}
				endCol = endColumn;
				#endregion

				#region Render CrossTitles on All Pages
                if (crossTitleVisible && crossTitlePrintOnAllPages && (!masterCrossTab.RightToLeft))
				{
					StiCrossTabHelper.RenderCells(masterCrossTab, renderedComponent as StiContainer, masterCrossTab.CrossTabInfoV1.StartCol, 0, endCol, 1, rect);
                    rect.Y += cross.Heights[0];
				}
				#endregion

				int colIndex = 0;
				while (colIndex < cross.ColFields.Count)
				{
					int colIndexEnd = colIndex;

					while (colIndexEnd < cross.ColFields.Count &&
                        ((StiCrossHeader)cross.ColFields[colIndexEnd]).PrintOnAllPages && (!masterCrossTab.RightToLeft))
					{
						colIndexEnd ++;
					}

					if (colIndex != colIndexEnd)
					{
                        if (crossTitleVisible)
                        {
                            StiCrossTabHelper.RenderCells(masterCrossTab, renderedComponent as StiContainer, masterCrossTab.CrossTabInfoV1.StartCol, colIndex + 1, endCol, colIndexEnd + 1, rect);
                            rect.Y += cross.Heights[colIndex + 1];
                        }
                        else
                        {
                            StiCrossTabHelper.RenderCells(masterCrossTab, renderedComponent as StiContainer, masterCrossTab.CrossTabInfoV1.StartCol, colIndex, endCol, colIndexEnd, rect);
                            rect.Y += cross.Heights[colIndex];
                        }
						
					}
					colIndex = colIndexEnd + 1;
				}
			}
			rect.X = stX;
			rect.Y = stY;
			#endregion

			endCol = StiCrossTabHelper.GetEndCol(masterCrossTab, masterCrossTab.CrossTabInfoV1.StartCol, rect);
			endRow = StiCrossTabHelper.GetEndRow(masterCrossTab, masterCrossTab.CrossTabInfoV1.StartRow, rect);

			if (masterCrossTab.CrossTabInfoV1.StartRow < endRow)
			{
				StiCrossTabHelper.RenderCells(masterCrossTab, renderedComponent as StiContainer, masterCrossTab.CrossTabInfoV1.StartCol, masterCrossTab.CrossTabInfoV1.StartRow, endCol, endRow, rect);
			}
			bool result = (endCol == cross.ColCount) && (endRow == cross.RowCount);

			masterCrossTab.CrossTabInfoV1.StartCol = endCol;

			if (endCol == cross.ColCount)
			{
				masterCrossTab.CrossTabInfoV1.StartCol = 0;
				masterCrossTab.CrossTabInfoV1.StartRow = endRow;
			}

			#region Finalize Cross
			if (masterCrossTab.CanGrow)
			{
				renderedComponent.CanGrow = false;
				renderedComponent.CanShrink = true;
				SizeD size = renderedComponent.GetActualSize();
				renderedComponent.Width = size.Width;
				renderedComponent.Height = size.Height;
				renderedComponent.CanGrow = true;
			}
			#endregion

			#region Hor Alignment for placed not on databand
			if (!placedOnBand)
			{
				if (masterCrossTab.HorAlignment == StiCrossHorAlignment.Left)
				{
					renderedComponent.Left = 0;
				}
				else if (masterCrossTab.HorAlignment == StiCrossHorAlignment.Right)
				{
					renderedComponent.Left = renderedComponent.Parent.Width - renderedComponent.Width;
				}
				else if (masterCrossTab.HorAlignment == StiCrossHorAlignment.Center)
				{
					renderedComponent.Left = (renderedComponent.Parent.Width - renderedComponent.Width) / 2;
				}
				else if (masterCrossTab.HorAlignment == StiCrossHorAlignment.Width)
				{
					renderedComponent.Left = 0;
					renderedComponent.Width = renderedComponent.Parent.Width;

					StiCrossTabHelper.MakeHorAlignmentByWidth(renderedComponent as StiContainer, 0);
				}
			}
			#endregion

			masterCrossTab.CrossTabInfoV1.FinishRender = result;			
			return result;
		}
		#endregion
	}
}
