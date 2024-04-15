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

using System;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Engine
{
	public class StiSubReportV2Builder : StiContainerV2Builder
	{
		public override StiComponent InternalRender(StiComponent masterComp)
		{
			var masterSubReport = masterComp as StiSubReport;

            if (masterSubReport.SubReportPage != null)
                masterSubReport.SubReportPage.InvokeBeforePrint(masterSubReport.SubReportPage, EventArgs.Empty);

			var containerOfSubReport = base.InternalRender(masterSubReport) as StiContainer;
			containerOfSubReport.CanBreak = true;
            containerOfSubReport.CanGrow = true;
            containerOfSubReport.CanShrink = true;

            if (masterSubReport.Parent is StiPage)
                containerOfSubReport.CanGrow = false;    //fix

			var subReportOnBand = StiSubReportsHelper.GetParentBand(masterSubReport) != null;
		    if (subReportOnBand)
		    {
		        StiSubReportsHelper.RenderSubReport(containerOfSubReport, masterSubReport);
		        StiContainerHelper.CheckSize(containerOfSubReport);
		    }
		    else
		        containerOfSubReport.CanShrink = false;

		    if (masterSubReport.SubReportPage != null)
                masterSubReport.SubReportPage.InvokeAfterPrint(masterSubReport.SubReportPage, EventArgs.Empty);

			return containerOfSubReport;
		}
	}
}
