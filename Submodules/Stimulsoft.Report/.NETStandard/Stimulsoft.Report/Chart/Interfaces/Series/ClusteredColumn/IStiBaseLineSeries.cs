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

using Stimulsoft.Base.Drawing;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public interface IStiBaseLineSeries : 
        IStiSeries, 
        IStiAllowApplyColorNegative,
        IStiShowNullsSeries,
        IStiShowZerosSeries
	{
        #region Properties
        IStiMarker Marker { get; set; }

        IStiLineMarker LineMarker { get; set; }
        
        Color LineColor { get; set; }

        StiPenStyle LineStyle { get; set; }

		bool Lighting { get; set; } 

		float LineWidth { get; set; }

        int LabelsOffset { get; set; }

        StiShowEmptyCellsAs ShowNullsAs { get; set; }

        StiShowEmptyCellsAs ShowZerosAs { get; set; }
        #endregion
    }
}
