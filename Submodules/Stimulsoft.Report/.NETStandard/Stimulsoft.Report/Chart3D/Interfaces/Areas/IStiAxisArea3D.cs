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

using Stimulsoft.Report.Chart;

namespace Stimulsoft.Report.Chart
{
    /// <summary>
    /// Describes base class for all axis 3d areas.
    /// </summary>
    public interface IStiAxisArea3D : IStiArea3D
    {
        #region Properties
        StiAxisAreaCoreXF3D AxisCore { get; }

        IStiInterlacingHor InterlacingHor { get; set; }

        IStiInterlacingVert InterlacingVert { get; set; }

        IStiXAxis3D XAxis { get; set; }

        IStiYAxis3D YAxis { get; set; }

        IStiAxis3D ZAxis { get; set; }

        IStiGridLinesHor GridLinesHor { get; set; }

        IStiGridLinesVert GridLinesVert { get; set; }
        #endregion
    }
}
