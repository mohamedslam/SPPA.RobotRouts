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
	public class StiGroupFooterBandV2Builder : StiBandV2Builder
    {
        #region Methods
        /// <summary>
        /// Returns the Master component of an object.
        /// </summary>
        /// <returns>Master component.</returns>
        public static StiDataBand GetMaster(StiGroupFooterBand masterFooterBand)
        {
            var index = masterFooterBand.Parent.Components.IndexOf(masterFooterBand) - 1;

            while (index >= 0)
            {
                if (masterFooterBand.Parent.Components[index] is StiDataBand)
                    return masterFooterBand.Parent.Components[index] as StiDataBand;

                index--;
            }

            return null;
        }

		public override void SetReportVariables(StiComponent masterComp)
		{
			var masterGroupFooter = masterComp as StiGroupFooterBand;
			
			masterGroupFooter.Report.GroupLine = masterGroupFooter.Line;
        }
        #endregion
    }
}
