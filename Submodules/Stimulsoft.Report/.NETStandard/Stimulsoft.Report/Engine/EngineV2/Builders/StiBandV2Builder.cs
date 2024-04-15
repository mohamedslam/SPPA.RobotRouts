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
using Stimulsoft.Report.CrossTab;

namespace Stimulsoft.Report.Engine
{
    public class StiBandV2Builder : StiContainerV2Builder
    {
        #region Methods.Helpers
        /// <summary>
        /// Returns child bands.
        /// </summary>
        public static StiComponentsCollection GetChildBands(StiBand masterBand)
        {
            var comps = new StiComponentsCollection();
            var index = masterBand.Parent.Components.IndexOf(masterBand) + 1;

            while (index < masterBand.Parent.Components.Count)
            {
                if (masterBand.Parent.Components[index] is StiChildBand)
                    comps.Add(masterBand.Parent.Components[index] as StiChildBand);
                else
                    break;

                index++;
            }
            return comps;
        }

        public static StiComponentsCollection GetSubReports(StiBand masterBand)
        {
            var comps = new StiComponentsCollection();

            lock (((ICollection) masterBand.Components).SyncRoot)
            {
                foreach (StiComponent component in masterBand.Components)
                {
                    if (component is StiSubReport)
                        comps.Add(component as StiSubReport);
                }
            }

            return comps;
        }
        #endregion

        #region Methods.Render
        public override void Prepare(StiComponent masterComp)
        {
            base.Prepare(masterComp);

            var masterBand = masterComp as StiBand;

            var comps = masterBand.GetComponents();
            lock (((ICollection) comps).SyncRoot)
            {
                foreach (StiComponent comp in comps)
                {
                    var subReport = comp as StiSubReport;
                    if (subReport != null)
                    {
                        if (!subReport.KeepSubReportTogether)
                            masterBand.BandInfoV2.ForceCanBreak = true;

                        masterBand.BandInfoV2.ForceCanGrow = true;
                        break;
                    }

                    var crossTab = comp as StiCrossTab;
                    if (crossTab != null)
                    {
                        if (!crossTab.KeepCrossTabTogether)
                            masterBand.BandInfoV2.ForceCanBreak = true;

                        masterBand.BandInfoV2.ForceCanGrow = true;
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
