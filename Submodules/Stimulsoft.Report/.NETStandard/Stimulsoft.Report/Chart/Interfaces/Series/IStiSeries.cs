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
    public interface IStiSeries :
        ICloneable,
        IStiJsonReportObject
    {
        #region Properties
        StiSeriesCoreXF Core { get; set; }

        bool AllowApplyStyle { get; set; }

        StiTrendLinesCollection TrendLines { get; set; }

        IStiTrendLine TrendLine { get; set; }

        string Format { get; set; }

        string CoreTitle { get; set; }

        string TitleValue { get; set; }

        StiSeriesSortType SortBy { get; set; }

        StiSeriesSortDirection SortDirection { get; set; }

        bool ShowInLegend { get; set; }

        StiShowSeriesLabels ShowSeriesLabels { get; set; }

        bool ShowShadow { get; set; }

        StiChartFiltersCollection Filters { get; set; }

        IStiSeriesTopN TopN { get; set; }

        StiChartConditionsCollection Conditions { get; set; }

        StiSeriesYAxis YAxis { get; set; }

        IStiSeriesLabels SeriesLabels { get; set; }

        IStiChart Chart { get; set; }
        #endregion

        #region Properties.Data
        double?[] ValuesStart { get; set; }

        double?[] Values { get; set; }
        
        object[] Arguments { get; set; }

        object[] OriginalArguments { get; set; }

        string[] ToolTips { get; set; }
        
        object[] Tags { get; set; }
        
        string[] Hyperlinks { get; set; }
        #endregion

        #region Interaction
        IStiSeriesInteraction Interaction { get; set; }
        #endregion

        #region Methods
        Color ProcessSeriesColors(int pointIndex, Color seriesColor);

        StiBrush ProcessSeriesBrushes(int pointIndex, StiBrush seriesBrush);

        StiMarkerType ProcessSeriesMarkerType(int pointIndex, StiMarkerType markerType);

        float ProcessSeriesMarkerAngle(int pointIndex, float markerAngle);

        bool ProcessSeriesMarkerVisible(int pointIndex);
        #endregion

        #region Methods.Types
        Type GetDefaultAreaType();
        #endregion
    }
}