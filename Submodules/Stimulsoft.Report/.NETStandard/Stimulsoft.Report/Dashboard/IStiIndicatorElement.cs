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
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiIndicatorElement : 
        IStiElement,
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

        void AddValue(IStiMeter meter);

        void RemoveValue();

        IStiMeter GetValue();

        IStiMeter GetValue(IStiMeter meter);

        void CreateNewValue();
        #endregion

        #region Target
        void AddTarget(IStiAppDataCell cell);

        void AddTarget(IStiMeter meter);

        void RemoveTarget();

        IStiMeter GetTarget();

        IStiMeter GetTarget(IStiMeter meter);

        void CreateNewTarget();
        #endregion

        #region Series
        void AddSeries(IStiAppDataCell cell);

        void AddSeries(IStiMeter meter);

        void RemoveSeries();

        IStiMeter GetSeries();

        IStiMeter GetSeries(IStiMeter meter);

        void CreateNewSeries();
        #endregion

        #region IndicatorConditions
        void AddIndicatorCondition(StiIndicatorFieldCondition field, Report.Components.StiFilterCondition condition, string value,
            StiFontIcons icon, Color iconColor, StiFontIcons targetIcon, Color targetIconColor, byte[] customIcon, StiIconAlignment iconAlignment, StiIconAlignment targetIconAlignment,
            StiIndicatorConditionPermissions permissions, Font font, Color textColor, Color backColor);

        List<IStiIndicatorElementCondition> FetchIndicatorConditions();

        void ClearIndicatorConditions();
        #endregion

        #region Properties
        StiFontIconSet IconSet { get; set; }

        StiFontIcons Icon { get; set; }

        StiIconAlignment IconAlignment { get; set; }

        Color GlyphColor { get; set; }

        byte[] CustomIcon { get; set; }

        StiTargetMode TargetMode { get; set; }

        StiFontSizeMode FontSizeMode { get; set; }

        Font Font { get; set; }

        StiIndicatorIconMode IconMode { get; set; }

        StiIndicatorIconRangeMode IconRangeMode { get; set; }
        #endregion

        #region Methods
        List<IStiIndicatorIconRange> GetIconRanges();

        IStiIndicatorIconRange AddRange();

        void RemoveRange(int index);

        void CreatedDefaultRanges();
        #endregion        
    }
}