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
	public class StiReportTitleBandV1Builder : StiBandV1Builder
	{
		#region Methods.Render
		/// <summary>
		/// Renders a component in the specified container without taking generation of BeforePrintEvent and AfterPrintEvent events into consideration and without taking Conditions into consideration.
		/// The rendered component is returned in the renderedComponent.
		/// </summary>
		/// <param name="renderedComponent">A rendered component.</param>
		/// <param name="outContainer">A panel in what rendering will be done.</param>
		/// <returns>Is rendering finished or not.</returns>
		public override bool InternalRender(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiReportTitleBand masterReportTitle = masterComp as StiReportTitleBand;

			if (masterReportTitle.ResetPageNumber && (!masterReportTitle.IsRendered))masterReportTitle.Report.EngineV1.ResetPageNumber();

			#region PrintIfEmpty
			if (!masterReportTitle.PrintIfEmpty)
			{
				bool isEmpty = true;
				foreach (StiComponent comp in masterReportTitle.Parent.Components)
				{
					if (comp is StiDataBand && (!((StiDataBand)comp).IsEmpty))
					{
						isEmpty = false;
						break;
					}
				}
				if (isEmpty)return true;
			}		
			#endregion

			foreach (StiComponent comp in masterReportTitle.Components)if (comp.IsCross)comp.Prepare();
			if (!masterReportTitle.IsRendered)return base.InternalRender(masterReportTitle, ref renderedComponent, outContainer);
			return true;
		}
		#endregion
	}
}
