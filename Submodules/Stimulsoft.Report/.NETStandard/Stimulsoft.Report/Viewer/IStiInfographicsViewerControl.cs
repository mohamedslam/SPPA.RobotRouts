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

using Stimulsoft.Base.Context;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using System;

namespace Stimulsoft.Report.Viewer
{
	public interface IStiInfographicsViewerControl
    {
        void ShowToolTip(string str);

        void ShowHyperlink(string str);

        void RemoveToolTip();

        void RemoveHyperlink();

        bool InvokeHyperlinkProcessing(object sender, string hyperlink);

        bool ProcessHyperlinkString(string hyperlink);

        void ProcessDrillDown(StiComponent comp, StiInteraction interaction, StiSeriesInteractionData seriesInteraction);
    }
}