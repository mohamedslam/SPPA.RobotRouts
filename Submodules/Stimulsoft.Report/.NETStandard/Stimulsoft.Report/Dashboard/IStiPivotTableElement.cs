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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiPivotTableElement :
        IStiElement,
        IStiDashboardElementStyle,
        IStiTransformActions,
        IStiTransformFilters,
        IStiTransformSorts,
        IStiDataTransformationElement,
        IStiGroupElement,
        IStiCrossFiltering,
        IStiDataFilters,
        IStiConvertibleElement
    {
        #region Column
        void CreateNewColumn();

        IStiMeter GetColumn(IStiAppDataCell cell);

        IStiMeter GetColumn(IStiMeter meter);

        IStiMeter GetColumnByIndex(int index);

        void InsertColumn(int index, IStiMeter meter);

        void RemoveColumn(int index);

        void RemoveAllColumns();
        #endregion

        #region Row
        void CreateNewRow();

        IStiMeter GetRow(IStiAppDataCell cell);

        IStiMeter GetRow(IStiMeter meter);

        IStiMeter GetRowByIndex(int index);

        void InsertRow(int index, IStiMeter meter);

        void RemoveRow(int index);

        void RemoveAllRows();
        #endregion

        #region Summary
        void CreateNewSummary();

        IStiMeter GetSummary(IStiAppDataCell cell);

        IStiMeter GetSummary(IStiMeter meter);

        IStiMeter GetSummaryByIndex(int index);

        void InsertSummary(int index, IStiMeter meter);

        void RemoveSummary(int index);

        void RemoveAllSummaries();

        StiSummaryDirection SummaryDirection { get; set; }
        #endregion

        void CreateNextMeter(IStiAppDataCell cell);

        List<IStiMeter> GetAllMeters();

        List<IStiPivotTableElementCondition> PivotTableConditions { get; set; }

        void AddPivotTableCondition(string keyValueMeter, Report.Components.StiFilterDataType dataType,
            Report.Components.StiFilterCondition condition, string value, Font font, Color textColor, Color backColor, StiConditionPermissions permissions, StiFontIcons icon, StiIconAlignment iconAlignment, byte[] customIcon, Color iconColor);

        List<IStiMeter> GetUsedMeters();

    }
}
