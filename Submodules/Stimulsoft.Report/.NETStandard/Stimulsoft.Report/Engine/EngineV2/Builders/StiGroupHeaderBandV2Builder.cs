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
	public class StiGroupHeaderBandV2Builder : StiBandV2Builder
	{
		#region Methods.Helper
        /// <summary>
        /// Returns the Master component of this component.
        /// </summary>
        public static StiDataBand GetMaster(StiGroupHeaderBand masterGroupHeaderBand)
        {
            var index = masterGroupHeaderBand.Parent.Components.IndexOf(masterGroupHeaderBand) + 1;

            while (index < masterGroupHeaderBand.Parent.Components.Count)
            {
                if (masterGroupHeaderBand.Parent.Components[index] is StiDataBand)
                    return masterGroupHeaderBand.Parent.Components[index] as StiDataBand;
                index++;
            }

            return null;
        }

		/// <summary>
		/// Returns the current value of a condition of grouping.
		/// </summary>
		/// <returns></returns>
		public static object GetCurrentConditionValue(StiGroupHeaderBand masterGroupHeaderBand)
		{
			var args = new StiValueEventArgs();
			masterGroupHeaderBand.InvokeGetValue(args);
			return args.Value;
		}

        /// <summary>
        /// Returns the current value of a summary expression of grouping.
        /// </summary>
        /// <returns></returns>
        public static object GetCurrentSummaryExpressionValue(StiGroupHeaderBand masterGroupHeaderBand)
        {
            var args = new StiValueEventArgs();
            masterGroupHeaderBand.InvokeGetSummaryExpression(args);
            return args.Value;
        }
		#endregion

		#region Methods.Render
		public override void SetReportVariables(StiComponent masterComp)
		{
			var masterGroupHeader = masterComp as StiGroupHeaderBand;
			
			masterGroupHeader.Report.GroupLine = masterGroupHeader.Line;
		}

		public override void Prepare(StiComponent masterComp)
		{
			var masterGroupHeaderBand = masterComp as StiGroupHeaderBand;
			base.Prepare(masterGroupHeaderBand);
			masterGroupHeaderBand.Line = 1;
		}
		#endregion
	}
}
