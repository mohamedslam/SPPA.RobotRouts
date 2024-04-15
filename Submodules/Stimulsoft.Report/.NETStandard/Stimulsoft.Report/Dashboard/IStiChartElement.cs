#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiChartElement : 
        IStiElement,
	    IStiUserFilters,
        IStiUserSorts,
        IStiDashboardElementStyle,
        IStiTransformActions,
        IStiTransformFilters,
        IStiTransformSorts,
        IStiDataTopN,
        IStiDataTransformationElement,
        IStiGroupElement,
        IStiCrossFiltering,
        IStiDataFilters,
        IStiConvertibleElement,
        IStiManuallyEnteredData
    {
        #region Value
        void AddValue(IStiAppDataCell cell);

        IStiMeter GetValue(IStiAppDataCell cell);

        IStiMeter GetValue(IStiMeter meter);

        IStiMeter GetGanttStartValue(IStiAppDataCell cell);

        IStiMeter GetGanttEndValue(IStiAppDataCell cell);

        IStiMeter GetY(IStiAppDataCell cell);

        IStiMeter GetX(IStiAppDataCell cell);

        IStiMeter GetValueByIndex(int index);

        List<IStiMeter> FetchAllValues();

        void InsertValue(int index, IStiMeter meter);

        void RemoveValue(int index);

        void RemoveAllValues();

        IStiMeter CreateNewValue();
        #endregion

        #region EndValue
        void AddEndValue(IStiAppDataCell cell);

        IStiMeter GetEndValue(IStiAppDataCell cell);

        IStiMeter GetEndValue(IStiMeter meter);

        IStiMeter GetEndValueByIndex(int index);

        void InsertEndValue(int index, IStiMeter meter);

        void RemoveEndValue(int index);

        void RemoveAllEndValues();

        IStiMeter CreateNewEndValue();
        #endregion

        #region CloseValue
        void AddCloseValue(IStiAppDataCell cell);

        IStiMeter GetCloseValue(IStiAppDataCell cell);

        IStiMeter GetCloseValue(IStiMeter meter);

        IStiMeter GetCloseValueByIndex(int index);

        void InsertCloseValue(int index, IStiMeter meter);

        void RemoveCloseValue(int index);

        void RemoveAllCloseValues();

        IStiMeter CreateNewCloseValue();
        #endregion

        #region LowValue
        void AddLowValue(IStiAppDataCell cell);

        IStiMeter GetLowValue(IStiAppDataCell cell);

        IStiMeter GetLowValue(IStiMeter meter);

        IStiMeter GetLowValueByIndex(int index);

        void InsertLowValue(int index, IStiMeter meter);

        void RemoveLowValue(int index);

        void RemoveAllLowValues();

        IStiMeter CreateNewLowValue();
        #endregion

        #region HighValue
        void AddHighalue(IStiAppDataCell cell);

        IStiMeter GetHighValue(IStiAppDataCell cell);

        IStiMeter GetHighValue(IStiMeter meter);

        IStiMeter GetHighValueByIndex(int index);

        void InsertHighValue(int index, IStiMeter meter);

        void RemoveHighValue(int index);

        void RemoveAllHighValues();

        IStiMeter CreateNewHighValue();
        #endregion

        #region Argument
        void AddArgument(IStiAppDataCell cell);

        IStiMeter GetArgument(IStiAppDataCell cell);

        IStiMeter GetArgument(IStiMeter meter);

        IStiMeter GetArgumentByIndex(int index);

        List<IStiMeter> FetchAllArguments();

        void InsertArgument(int index, IStiMeter meter);

        void RemoveArgument(int index);

        void RemoveAllArguments();

        void CreateNewArgument();
        #endregion

        #region Weight
        void AddWeight(IStiAppDataCell cell);

        IStiMeter GetWeight(IStiAppDataCell cell);

        IStiMeter GetWeight(IStiMeter meter);

        IStiMeter GetWeightByIndex(int index);

        void InsertWeight(int index, IStiMeter meter);

        void RemoveWeight(int index);

        void RemoveAllWeights();

        void CreateNewWeight();
        #endregion

        #region Series
        void AddSeries(IStiAppDataCell cell);

        IStiMeter GetSeries(IStiAppDataCell cell);

        IStiMeter GetSeries(IStiMeter meter);

        IStiMeter GetSeries();

        void InsertSeries(IStiMeter meter);

        void RemoveSeries();

        void CreateNewSeries();
        #endregion

        #region ConstantLines
        List<IStiChartConstantLines> FetchConstantLines();

        void AddConstantLine();

        void RemoveConstantLine(int index);

        void MoveConstantLine(int fromIndex, int toIndex);
        #endregion

        #region TrendLines
        void AddTrendLines(string keyValueMeter, StiChartTrendLineType type, Color lineColor, StiPenStyle lineStyle, float lineWidth);

        List<IStiChartTrendLine> FetchTrendLines();

        void ClearTrendLines();
        #endregion

        #region ChartConditions
        void AddChartCondition(string keyValueMeter, StiFilterDataType dataType, StiFilterCondition condition, string value, Color color, StiMarkerType markerType, float markerAngle, StiChartConditionalField field, bool isExpression);

        List<IStiChartElementCondition> FetchChartConditions();

        void ClearChartConditions();
        #endregion

        #region Properties
        bool IsAxisAreaChart { get; }

        bool IsAxisAreaChart3D { get; }

        bool IsScatterChart { get; }

        bool IsStackedChart { get; }

        bool IsLinesChart { get; }

        bool IsPieChart { get; }

        bool IsPie3dChart { get; }

        bool IsClusteredColumnChart3D { get; }

        bool IsParetoChart { get; }

        bool IsRibbonChart { get; }

        bool IsRadarChart { get; }

        bool IsDoughnutChart { get; }

        bool IsFunnelChart { get; }

        bool IsPictorialStackedChart { get; }

        bool IsTreemapChart { get; }

        bool IsSunburstChart { get; }

        bool IsWaterfallChart { get; }

        bool IsPictorialChart { get; }

        bool IsRange { get; }

        StiFormatService ArgumentFormat { get; set; }

        StiFormatService ValueFormat { get; set; }

        bool ColorEach { get; set; }

        StiFontIcons? Icon { get; set; }

        StiColumnShape3D ColumnShape { get; set; }

        bool RoundValues { get; set; }

        List<StiAnimation> PreviousAnimations { get; set; }
        #endregion

        #region Methods
        void ConvertToBubble();

        void ConvertFromBubble();

        void ConvertToGantt();

        void CheckBrowsableProperties();

        List<string> GetChartSeriesTypes(string seriesTypeStr);

        StiSeries GetChartSeries();

        IStiMeter GetManuallyEnteredChartMeter();
        #endregion
    }
}