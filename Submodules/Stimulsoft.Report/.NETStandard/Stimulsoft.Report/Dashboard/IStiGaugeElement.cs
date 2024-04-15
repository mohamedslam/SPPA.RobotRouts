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
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Gauge;
using System.Collections.Generic;

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiGaugeElement : 
        IStiElement,
        IStiUserSorts,
        IStiDashboardElementStyle,
        IStiTransformActions,
        IStiTransformFilters,
        IStiTransformSorts,
        IStiDataTransformationElement,
        IStiGroupElement,
        IStiCrossFiltering,
        IStiDataFilters,
        IStiConvertibleElement,
        IStiManuallyEnteredData
    {
        #region Value
        void AddValue(IStiAppDataCell cell);

        void AddValue(IStiMeter meter);

        void RemoveValue();

        IStiMeter GetValue();

        IStiMeter GetValue(IStiMeter meter);

        void CreateNewValue();
        #endregion

        #region Series
        void AddSeries(IStiAppDataCell cell);

        void AddSeries(IStiMeter meter);

        void RemoveSeries();

        IStiMeter GetSeries();

        IStiMeter GetSeries(IStiMeter meter);

        void CreateNewSeries();
        #endregion

        #region Series
        void AddTarget(IStiAppDataCell cell);

        void AddTarget(IStiMeter meter);

        void RemoveTarget();

        IStiMeter GetTarget();

        IStiMeter GetTarget(IStiMeter meter);

        void CreateNewTarget();
        #endregion

        #region Ranges
        StiGaugeRangeType RangeType { get; set; }

        StiGaugeRangeMode RangeMode { get; set; }

        List<IStiGaugeRange> GetRanges();

        IStiGaugeRange AddRange();

        void RemoveRange(int index);

        void CreatedDefaultRanges();
        #endregion

        StiGaugeCalculationMode CalculationMode { get; set; }

        StiGaugeType Type { get; set; }

        decimal Minimum { get; set; }

        decimal Maximum { get; set; }

        StiGauge GetGaugeComponent(SizeD? size = null);

        bool ShortValue { get; set; }

        StiFormatService ValueFormat { get; set; }

        List<StiAnimation> PreviousAnimations { get; set; }
    }
}