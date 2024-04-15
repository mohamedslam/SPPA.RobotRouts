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
using Stimulsoft.Base.Drawing;
using System;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    /// <summary>
    /// Describes base class for all chart areas.
    /// </summary>
    public interface IStiArea :
        ICloneable,
        IStiJsonReportObject
    {
        #region Properties
        StiAreaCoreXF Core { get; set; }

        IStiChart Chart { get; set; }

        bool AllowApplyStyle { get; set; }

        bool ColorEach { get; set; }

        bool ShowShadow { get; set; }

        Color BorderColor { get; set; }

        float BorderThickness { get; set; }

        StiBrush Brush { get; set; }

        bool IsDefaultSeriesTypeFullStackedColumnSeries { get; }

        bool IsDefaultSeriesTypeFullStackedBarSeries { get; }
        #endregion

        #region Methods.Types
        Type GetDefaultSeriesType();

        Type[] GetSeriesTypes();

        Type GetDefaultSeriesLabelsType();

        Type[] GetSeriesLabelsTypes();
        #endregion
    }
}