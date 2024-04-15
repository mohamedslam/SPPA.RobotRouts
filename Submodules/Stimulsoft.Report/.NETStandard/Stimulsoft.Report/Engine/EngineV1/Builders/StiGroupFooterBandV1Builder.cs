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
	public class StiGroupFooterBandV1Builder : StiBandV1Builder
	{
		#region Methods.Render

		/// <summary>
		/// Renders a component in the specified container with taking generation of events into consideration. The rendered component is returned in the renderedComponent argument.
		/// </summary>
		/// <param name="renderedComponent">A component which is being rendered.</param>
		/// <param name="outContainer">A container in which rendering will be done.</param>
		/// <returns>A value which indicates whether rendering of the component is finished or not.</returns>
		public override bool Render(StiComponent masterComp, ref StiComponent renderedComponent, StiContainer outContainer)
		{
			StiGroupFooterBand masterGroupFooter = masterComp as StiGroupFooterBand;

			//Store current line value
			int line = masterGroupFooter.Report.Line;
			//Set line value from GroupHeaderBand
			masterGroupFooter.Report.Line = masterGroupFooter.GroupFooterBandInfoV1.GroupHeader.Line;

			masterGroupFooter.RenderedCount++;
			//DoBookmark();

			bool result = base.InternalRender(masterGroupFooter, ref renderedComponent, outContainer);

			//Restore line value
			masterGroupFooter.Report.Line = line;
			return result;
		}
		#endregion
	}
}
