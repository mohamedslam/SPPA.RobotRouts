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

using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiGroupHeaderBandV1Builder : StiBandV1Builder
	{
		#region Methods.Helpers.Breakable
		internal static void SecondPassBreak(StiGroupHeaderBand masterGroupHeaderBand, StiComponent renderedComponent, StiComponent masterComponent, ref double freeSpace)
		{
			masterGroupHeaderBand.GroupHeaderBandInfoV1.IsFirstPassOfBreak = true;
			if (masterGroupHeaderBand.GroupHeaderBandInfoV1.BreakableComps != null)
			{
				masterGroupHeaderBand.GroupHeaderBandInfoV1.IsFirstPassOfBreak = false;
				StiComponentDivider.ProcessPreviousBreakedContainer(renderedComponent as StiContainer, masterGroupHeaderBand.GroupHeaderBandInfoV1.BreakableComps, ref freeSpace);
						
				masterGroupHeaderBand.GroupHeaderBandInfoV1.BreakableComps = null;
			}
		}

		internal static void FirstPassBreak(StiGroupHeaderBand masterGroupHeaderBand, StiComponent renderedComponent, StiComponent masterComponent, ref double freeSpace)
		{
			renderedComponent.Height += freeSpace;
			masterGroupHeaderBand.GroupHeaderBandInfoV1.BreakableComps = StiComponentDivider.BreakContainer(renderedComponent as StiContainer);
		}
		#endregion

		#region Methods.Helpers
		/// <summary>
		/// Returns the current value of a condition of grouping.
		/// </summary>
		/// <returns></returns>
		public static object GetCurrentConditionValue(StiGroupHeaderBand masterGroupHeaderBand)
		{
			StiValueEventArgs args = new StiValueEventArgs();
			masterGroupHeaderBand.InvokeGetValue(args);
			return args.Value;
		}

        /// <summary>
        /// Returns the current value of a summary expression of grouping.
        /// </summary>
        /// <returns></returns>
        public static object GetCurrentSummaryExpressionValue(StiGroupHeaderBand masterGroupHeaderBand)
        {
            StiValueEventArgs args = new StiValueEventArgs();
            masterGroupHeaderBand.InvokeGetSummaryExpression(args);
            return args.Value;
        }
		#endregion

		#region Methods.Render
		/// <summary>
		/// Prepares a component for rendering.
		/// </summary>
		public override void Prepare(StiComponent masterComp)
		{
			base.Prepare(masterComp);
			StiGroupHeaderBand masterGroupHeader = masterComp as StiGroupHeaderBand;

			if (masterGroupHeader.CanBreak)
			{
				masterGroupHeader.GroupHeaderBandInfoV1.ForceCanBreak = true;
				return;
			}
			foreach (StiComponent comp in masterGroupHeader.Components)
			{
				if (comp is IStiCrossTab)
				{
					masterGroupHeader.GroupHeaderBandInfoV1.ForceCanBreak = true;
					break;
				}
			}

            //Required for two pass reports
            masterGroupHeader.GroupHeaderBandInfoV1.LastPositionRendering = -1;
            masterGroupHeader.GroupHeaderBandInfoV1.LastPositionLineRendering = -1;
		}

		/// <summary>
		/// Renders a component in the specified container with taking generation of events into consideration. The rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">A component which is being rendered.</param>
		/// <param name="outContainer">A container in which rendering will be done.</param>
		/// <returns>A value which indicates whether rendering of a component is finished or not.</returns>
		public override bool Render(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiGroupHeaderBand masterGroupHeader = masterComp as StiGroupHeaderBand;

			//Store current line value
			int line = masterGroupHeader.Report.Line;
			//Set line value from GroupHeaderBand
			masterGroupHeader.Report.Line = masterGroupHeader.Line;

			masterGroupHeader.RenderedCount++;
			//DoBookmark();
			bool result = base.InternalRender(masterGroupHeader, ref renderedComponent, outContainer);

			//Restore line value
			masterGroupHeader.Report.Line = line;
			return result;
		}
		#endregion
	}
}
