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
using Stimulsoft.Report.Maps;
using System.Collections.Generic;

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiRegionMapElement : 
        IStiElement,
        IStiDashboardElementStyle,
        IStiManuallyEnteredData,
        IStiUserFilters,
        IStiTransformActions,
        IStiTransformFilters,
        IStiTransformSorts,
        IStiDataTransformationElement,
        IStiGroupElement,
        IStiCrossFiltering,
        IStiDataFilters
    {
        string MapIdent { get; set; }

        StiMapSource DataFrom { get; set; }

        string MapData { get; set; }

        StiMapType MapType { get; set; }

        bool ShowValue { get; set; }

        bool ShowZeros { get; set; }

        bool ColorEach { get; set; }

        bool ShortValue { get; set; }

        bool ShowBubble { get; set; }

        string Language { get; set; }

        StiDisplayNameType ShowName { get; set; }

        List<StiMapData> GetMapData();

        void CreateNextMeter(IStiAppDataCell cell);

        #region Key
        void AddKeyMeter(IStiAppDataCell cell);

        void AddKeyMeter(IStiMeter meter);

        IStiMeter GetKeyMeter();

        void RemoveKeyMeter();

        void CreateNewKeyMeter();
        #endregion

        #region Name
        void AddNameMeter(IStiAppDataCell cell);

        void AddNameMeter(IStiMeter meter);

        IStiMeter GetNameMeter();

        void RemoveNameMeter();

        void CreateNewNameMeter();
        #endregion

        #region Value
        void AddValueMeter(IStiAppDataCell cell);

        void AddValueMeter(IStiMeter meter);

        IStiMeter GetValueMeter();

        void RemoveValueMeter();

        void CreateNewValueMeter();
        #endregion

        #region Group
        void AddGroupMeter(IStiAppDataCell cell);

        void AddGroupMeter(IStiMeter meter);

        IStiMeter GetGroupMeter();

        void RemoveGroupMeter();

        void CreateNewGroupMeter();
        #endregion

        #region Color
        void AddColorMeter(IStiAppDataCell cell);

        void AddColorMeter(IStiMeter meter);

        IStiMeter GetColorMeter();

        void RemoveColorMeter();

        void CreateNewColorMeter();
        #endregion
    }
}