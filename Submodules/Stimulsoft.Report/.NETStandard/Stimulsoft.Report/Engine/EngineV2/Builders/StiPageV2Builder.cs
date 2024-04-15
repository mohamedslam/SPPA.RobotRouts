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
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiPageV2Builder : StiContainerV2Builder
	{
		public override void Prepare(StiComponent masterComp)
		{
			var masterPage = masterComp as StiPage;

			base.Prepare(masterPage);

			masterPage.PageInfoV2.IsReportTitlesRendered = false;
			masterPage.PageInfoV2.RenderedCount = 0;
            
			#region Prepare Overlaybands
            masterPage.PageInfoV2.Overlays = new StiComponentsCollection();
		    lock (((ICollection) masterPage.Components).SyncRoot)
		    {
		        foreach (StiComponent comp in masterPage.Components)
		        {
		            var overlayBand = comp as StiOverlayBand;
		            if (overlayBand != null)
		                masterPage.PageInfoV2.Overlays.Add(overlayBand);
		        }
		    }
		    #endregion
		}

		/// <summary>
		/// Clears a component after rendering.
		/// </summary>
		public override void UnPrepare(StiComponent masterComp)
		{
			base.UnPrepare(masterComp);

			var masterPage = masterComp as StiPage;
			masterPage.PageInfoV2.Overlays = null;
		}
	}
}
