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

namespace Stimulsoft.Report.Chart
{
    /// <summary>
    /// Describes base class for all axis areas.
    /// </summary>
    public interface IStiAxisArea : IStiArea
	{
		#region Properties
        StiAxisAreaCoreXF AxisCore { get; }

		IStiInterlacingHor InterlacingHor { get; set; }

		IStiInterlacingVert InterlacingVert { get; set; }

		IStiGridLinesHor GridLinesHor { get; set; }

        IStiGridLinesHor GridLinesHorRight { get; set; }

		IStiGridLinesVert GridLinesVert { get; set; }

        IStiYAxis YAxis { get; set; }

        IStiYAxis YRightAxis { get; set; }

        IStiXAxis XAxis { get; set; }

        IStiXAxis XTopAxis { get; set; }

        bool ReverseHor { get; set; }

        bool ReverseVert { get; set; }
		#endregion
	}
}
