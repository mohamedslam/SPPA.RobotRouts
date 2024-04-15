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
using System.Drawing;

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiCardsElement :
        IStiElement,
        IStiDashboardElementStyle,
        IStiConvertibleElement,
        IStiDataFilters,
        IStiGroupElement,
        IStiCrossFiltering,
        IStiTransformActions,
        IStiTransformFilters,
        IStiTransformSorts,
        IStiDataTransformationElement
    {
        void CreateMeters(IStiCardsElement cardsElement);

        void CreateMeters(StiDataSource dataSource);

        void CreateMeter(IStiAppDataCell cell);

        void RemoveMeter(int index);

        void RemoveAllMeters();

        void InsertMeter(int index, IStiMeter meter);

        void InsertNewDimension(int index);

        void InsertNewMeasure(int index);

        IStiMeter GetMeasure(IStiAppDataCell cell);

        IStiMeter GetDimension(IStiAppDataCell cell);

        IStiCardsItem GetCards();

        Color BackColor { get; set; }

        int ColumnCount { get; set; }

        StiItemOrientation Orientation { get; set; }
    }
}
