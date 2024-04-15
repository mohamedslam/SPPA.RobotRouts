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

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Summary description for StiComponentHelper.
	/// </summary>
	public class StiComponentHelper
	{
		public static void FillComponentPlacement(StiComponent component)
		{
			var parent = component.Parent;
			while (parent != null && !(parent is StiPage) && (!(parent is StiBand) || parent.IsCross))
			{
				parent = parent.Parent;
			}

			if (parent == null)return;
            if (parent is StiPage)
            {
                if (StiOptions.Export.OptimizeDataOnlyMode && component is StiBand)
                    parent = component as StiContainer;

                else
                    component.ComponentPlacement = "p";
            }
			if (parent is StiBand)
			{
                if (parent is StiChildBand && parent.Parent != null)
                {
                    //find first non-childband band before this childband
                    var cont = parent.Parent;
                    var pos = cont.Components.IndexOf(parent);
                    if (pos > 0)
                    {
                        while (pos > 0 && (cont.Components[pos] is StiChildBand || !(cont.Components[pos] is StiBand))) pos--;
                        if (cont.Components[pos] is StiBand)
                            parent = (StiContainer)cont.Components[pos];
                    }
                }

                if (parent is StiReportTitleBand)
                    component.ComponentPlacement = "rt";

				if (parent is StiReportSummaryBand)
				    component.ComponentPlacement = "rs";

				if (parent is StiPageHeaderBand)
				    component.ComponentPlacement = "ph";

				if (parent is StiPageFooterBand)
				    component.ComponentPlacement = "pf";

			    if (parent is StiHeaderBand)
			        component.ComponentPlacement = ((StiHeaderBand) parent).PrintOnAllPages ? "h.ap" : "h";

			    if (parent is StiFooterBand)
			        component.ComponentPlacement = ((StiFooterBand) parent).PrintOnAllPages ? "f.ap" : "f";

			    if (parent is StiDataBand)
				    component.ComponentPlacement = "d";

				if (parent is StiGroupHeaderBand)
				{
					component.ComponentPlacement = "gh";

					var gh = parent as StiGroupHeaderBand;
					if (gh.GroupHeaderBandInfoV2.IsTableGroupHeader)
						component.ComponentPlacement = gh.PrintOnAllPages ? "h.ap" : "h";
				}

				if (parent is StiGroupFooterBand)
				    component.ComponentPlacement = "gf";

				if (parent is StiEmptyBand)
				    component.ComponentPlacement = "e";
			}

		    if (component.ComponentPlacement.Length > 0)
		    {
		        component.ComponentPlacement += component.Parent.IsCross 
		            ? "." + parent.Name 
		            : "." + component.Parent.Name;
		    }
		}
	}
}
