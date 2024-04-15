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

using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiHeaderBandV1Builder : StiBandV1Builder
	{
		#region Methods.Helper.Breakable
		internal static void SecondPassBreak(StiHeaderBand masterHeaderBand, StiComponent renderedComponent, StiComponent masterComponent, ref double freeSpace)
		{
			masterHeaderBand.HeaderBandInfoV1.IsFirstPassOfBreak = true;
			if (masterHeaderBand.HeaderBandInfoV1.BreakableComps != null)
			{
				masterHeaderBand.HeaderBandInfoV1.IsFirstPassOfBreak = false;
				StiComponentDivider.ProcessPreviousBreakedContainer(renderedComponent as StiContainer, masterHeaderBand.HeaderBandInfoV1.BreakableComps, ref freeSpace);
						
				masterHeaderBand.HeaderBandInfoV1.BreakableComps = null;
			}
		}

		internal static void FirstPassBreak(StiHeaderBand masterHeaderBand, StiComponent renderedComponent, StiComponent masterComponent, ref double freeSpace)
		{
			renderedComponent.Height += freeSpace;
			masterHeaderBand.HeaderBandInfoV1.BreakableComps = StiComponentDivider.BreakContainer(renderedComponent as StiContainer);
		}
		#endregion

		#region Methods.Helper
		/// <summary>
		/// Returns Master component of this component.
		/// </summary>
		public static StiComponent GetMaster(StiHeaderBand masterHeaderBand)
		{
            if (masterHeaderBand.Parent == null)
                return null;

			int index = masterHeaderBand.Parent.Components.IndexOf(masterHeaderBand) + 1;
			
			while (index < masterHeaderBand.Parent.Components.Count)
			{
                if (masterHeaderBand.Parent.Components[index] is StiDataBand &&
                    (!(masterHeaderBand.Parent.Components[index] is StiEmptyBand)))
                {
                    return masterHeaderBand.Parent.Components[index];
                }

				index++;
			}
			
			return null;
		}

		public static bool PrintOddEven(IStiPrintOnEvenOddPages component, StiPrintOnEvenOddPagesType printOnEvenOddPagesType)
		{
			int pageEvenOdd = ((StiComponent)component).Report.PageNumber & 1;

			if (component.PrintOnEvenOddPages == StiPrintOnEvenOddPagesType.Ignore)return true;
			if (component.PrintOnEvenOddPages == StiPrintOnEvenOddPagesType.PrintOnEvenPages && pageEvenOdd == 0)return true;
			if (component.PrintOnEvenOddPages == StiPrintOnEvenOddPagesType.PrintOnOddPages && pageEvenOdd == 1)return true;

			return false;
		}
		#endregion

		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			base.Prepare(masterComp);

			StiHeaderBand masterHeader = masterComp as StiHeaderBand;

			if (masterHeader.CanBreak)
			{
				masterHeader.HeaderBandInfoV1.ForceCanBreak = true;
				return;
			}

			foreach (StiComponent comp in masterHeader.Components)
			{
				if (comp is IStiCrossTab)
				{
					masterHeader.HeaderBandInfoV1.ForceCanBreak = true;
					break;
				}
			}
		}

		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// The rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in which rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiHeaderBand masterHeader = masterComp as StiHeaderBand;

			if (!PrintOddEven(masterHeader, masterHeader.PrintOnEvenOddPages))return true;

			bool result = base.InternalRender(masterHeader, ref renderedComponent, outContainer);
            			
			#region Process Column Header
			if (masterHeader is StiColumnHeaderBand)
		    {
		        StiDataBand dataBand = GetMaster(masterHeader) as StiDataBand;
		        double columnWidth = dataBand.GetColumnWidth();
		        double columnGaps = dataBand.ColumnGaps;
		        int columnCount = dataBand.Columns;
		        if (columnCount < 1) columnCount = 1;

		        ((StiContainer) renderedComponent).Components.Clear();

		        for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
		        {
		            //Assign column value only when columnCount > 1
		            if (columnCount > 1) masterHeader.Report.Column = columnIndex + 1;
		            if ((columnIndex < dataBand.Count && (!masterHeader.PrintIfEmpty)) || masterHeader.PrintIfEmpty)
		            {
		                StiContainer cont = new StiContainer
		                {
		                    Name = masterHeader.Name,
		                    Width = columnWidth + columnGaps
		                };
		                ((StiContainer) renderedComponent).Components.Add(cont);

		                if (dataBand.RightToLeft) cont.DockStyle = StiDockStyle.Right;
		                else cont.DockStyle = StiDockStyle.Left;

		                StiComponent renderedComponent2 = null;
		                base.InternalRender(masterHeader, ref renderedComponent2, cont);

		                #region Correct location for right to left mode
		                if (dataBand.RightToLeft && renderedComponent2 is StiContainer)
		                {
		                    foreach (StiComponent comp in (renderedComponent2 as StiContainer).Components)
		                    {
		                        comp.Left += columnGaps;
		                    }
		                }
		                #endregion
		            }
		            else break;
		        }

		        //Assign column value only when columnCount > 1
		        if (columnCount > 1) masterHeader.Report.Column = 1;

		        return true;
		    }
		    #endregion

			return result;
		}

		/// <summary>
		/// Renders a component in the specified container with taking generation of events into consideration. The rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">A component which is being rendered.</param>
		/// <param name="outContainer">A container in which rendering will be done.</param>
		/// <returns>A value which indicates whether rendering of the component is finished or not.</returns>
		public override bool Render(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiHeaderBand masterHeader = masterComp as StiHeaderBand;
			
			masterHeader.RenderedCount++;
			masterHeader.DoBookmark();
			return InternalRender(masterHeader, ref renderedComponent, outContainer);
		}
		#endregion
	}
}
