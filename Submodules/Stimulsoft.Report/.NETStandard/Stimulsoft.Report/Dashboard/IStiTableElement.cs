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
using Stimulsoft.Report.Dictionary;
using System.Collections.Generic;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiTableElement : 
        IStiElement,
        IStiUserSorts,
        IStiUserFilters,
        IStiDashboardElementStyle,
        IStiTransformActions,
        IStiTransformFilters,
        IStiTransformSorts,
        IStiDataTransformationElement,
        IStiGroupElement,
        IStiCrossFiltering,
        IStiDataFilters,
        IStiFont,
        IStiConvertibleElement
    {
        void CreateMeters(IStiTableElement tableElement);

	    void CreateMeters(StiDataSource dataSource);

        void CreateMeter(IStiAppDataCell cell);

        void RemoveMeter(int index);

        void RemoveAllMeters();

        void InsertMeter(int index, IStiMeter meter);

        void InsertNewDimension(int index);

        void InsertNewMeasure(int index);

        IStiMeter GetMeasure(IStiAppDataCell cell);

        IStiMeter GetDimension(IStiAppDataCell cell);

        Font HeaderFont { get; set; }

        Color HeaderForeColor { get; set; }

        StiTableSizeMode SizeMode { get; set; }

        Color ForeColor { get; set; }

        Font FooterFont { get; set; }

        Color FooterForeColor { get; set; }

        int FrozenColumns { get; set; }

        List<IStiTableElementCondition> TableConditions { get; set; }

        void AddTableCondition(string[] keyDataFiledMeters, string[] keyDestinationMeters, StiFilterDataType dataType, StiFilterCondition condition, string value, StiTableConditionPermissions permissions, Font font, Color foreColor, Color backColor, bool isExpression);
    }
}
