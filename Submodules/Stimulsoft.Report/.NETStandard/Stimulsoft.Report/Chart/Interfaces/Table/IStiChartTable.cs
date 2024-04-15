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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using System;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public interface IStiChartTable : 
        ICloneable,
        IStiJsonReportObject,
        IStiDefault
    {
        #region Properties
        StiChartTableCore Core { get; set; }

        IStiChart Chart { get; set; }

        bool AllowApplyStyle { get; set; }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Font Font { get; set; }

        bool MarkerVisible { get; set; }

        Color GridLineColor { get; set; }

        Color TextColor { get; set; }

        bool GridLinesHor { get; set; }

        bool GridLinesVert { get; set; }

        bool GridOutline { get; set; }

        bool Visible { get; set; }

        string Format { get; set; }

        IStiChartTableHeader Header { get; set; }

        IStiChartTableDataCells DataCells { get; set; }
        #endregion
    }
}