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
	public class StiFooterBandV1Builder : StiBandV1Builder
	{
		#region Methods.Helper.Breakable
		internal static void SecondPassBreak(StiFooterBand masterFooterBand, StiComponent renderedComponent, StiComponent masterComponent, ref double freeSpace)
		{
			masterFooterBand.FooterBandInfoV1.IsFirstPassOfBreak = true;
			if (masterFooterBand.FooterBandInfoV1.BreakableComps != null)
			{
				masterFooterBand.FooterBandInfoV1.IsFirstPassOfBreak = false;
				StiComponentDivider.ProcessPreviousBreakedContainer(renderedComponent as StiContainer, masterFooterBand.FooterBandInfoV1.BreakableComps, ref freeSpace);
						
				masterFooterBand.FooterBandInfoV1.BreakableComps = null;
			}
		}

		internal static void FirstPassBreak(StiFooterBand masterFooterBand, StiComponent renderedComponent, StiComponent masterComponent, ref double freeSpace)
		{
			renderedComponent.Height += freeSpace;
			masterFooterBand.FooterBandInfoV1.BreakableComps = StiComponentDivider.BreakContainer(renderedComponent as StiContainer);
		}
		#endregion

		#region Methods.Helper
		/// <summary>
		/// Returns the Master component of an object.
		/// </summary>
		/// <returns>Master component.</returns>
		public static StiComponent GetMaster(StiFooterBand masterFooterBand)
		{
			int index = masterFooterBand.Parent.Components.IndexOf(masterFooterBand) - 1;
			
			while (index >= 0)
			{
				if (masterFooterBand.Parent.Components[index] is StiDataBand && 
					(!(masterFooterBand.Parent.Components[index] is StiEmptyBand)))return masterFooterBand.Parent.Components[index];
				index--;
			}
			
			return null;
		}
		#endregion

		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			base.Prepare(masterComp);

			StiFooterBand masterFooter = masterComp as StiFooterBand; 

			if (masterFooter.CanBreak)
			{
				masterFooter.FooterBandInfoV1.ForceCanBreak = true;
				return;
			}

			foreach (StiComponent comp in masterFooter.Components)
			{
				if (comp is IStiCrossTab)
				{
					masterFooter.FooterBandInfoV1.ForceCanBreak = true;
					break;
				}
			}
		}

		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// Rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in which rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiFooterBand masterFooter = masterComp as StiFooterBand; 

			if (!StiHeaderBandV1Builder.PrintOddEven(masterFooter, masterFooter.PrintOnEvenOddPages))return true;

			bool result = base.InternalRender(masterFooter, ref renderedComponent, outContainer);

			#region Process Column Footer
			if (masterFooter is StiColumnFooterBand)
		    {
		        StiDataBand dataBand = GetMaster(masterFooter) as StiDataBand;
		        double columnWidth = dataBand.GetColumnWidth();
		        double columnGaps = dataBand.ColumnGaps;
		        int columnCount = dataBand.Columns;
		        if (columnCount < 1) columnCount = 1;

		        ((StiContainer) renderedComponent).Components.Clear();

		        for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
		        {
		            //Assign column value only when columnCount > 1
		            if (columnCount > 1) masterFooter.Report.Column = columnIndex + 1;

		            if ((columnIndex < dataBand.Count && (!masterFooter.PrintIfEmpty)) || masterFooter.PrintIfEmpty)
		            {
		                StiContainer cont = new StiContainer
		                {
		                    Name = masterFooter.Name,
		                    Width = columnWidth + columnGaps
		                };
		                ((StiContainer) renderedComponent).Components.Add(cont);

		                if (dataBand.RightToLeft) cont.DockStyle = StiDockStyle.Right;
		                else cont.DockStyle = StiDockStyle.Left;

		                StiComponent renderedComponent2 = null;
		                base.InternalRender(masterFooter, ref renderedComponent2, cont);

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
		        if (columnCount > 1) masterFooter.Report.Column = 1;

		        return true;
		    }
		    #endregion

			return result;
		}

		/// <summary>
		/// Renders a component in the specified container with taking generation of events into consideration. A rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">A component which is being rendered.</param>
		/// <param name="outContainer">A container in which rendering will be done.</param>
		/// <returns>A value which indicates whether rendering of the component is finished or not.</returns>
		public override bool Render(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiFooterBand masterFooter = masterComp as StiFooterBand; 

			masterFooter.RenderedCount++;
			masterFooter.DoBookmark();
			return InternalRender(masterFooter, ref renderedComponent, outContainer);			
		}
		#endregion
	}
}
