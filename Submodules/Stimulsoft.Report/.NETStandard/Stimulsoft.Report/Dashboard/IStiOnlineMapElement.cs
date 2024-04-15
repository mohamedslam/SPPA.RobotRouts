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
using Stimulsoft.Base.Gis;
using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Engine;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Dashboard
{
    public interface IStiOnlineMapElement : 
        IStiElement,
        IStiTransformActions,
        IStiTransformFilters,
        IStiTransformSorts,
        IStiDataTransformationElement,
        IStiGroupElement,
        IStiCrossFiltering,
        IStiDataFilters,
        IStiManuallyEnteredData
    {
        #region Methods.Latitude
        void CreateNextMeter(IStiAppDataCell cell);

        void AddLatitudeMeter(IStiAppDataCell cell);

        void AddLatitudeMeter(IStiMeter meter);

        IStiMeter GetLatitudeMeter();

        void RemoveLatitudeMeter();

        void CreateNewLatitudeMeter();
        #endregion

        #region Methods.Longitude
        void AddLongitudeMeter(IStiAppDataCell cell);

        void AddLongitudeMeter(IStiMeter meter);

        IStiMeter GetLongitudeMeter();

        void RemoveLongitudeMeter();

        void CreateNewLongitudeMeter();
        #endregion

        #region Methods.Location
        void AddLocationMeter(IStiAppDataCell cell);

        void AddLocationMeter(IStiMeter meter);

        IStiMeter GetLocationMeter();

        void RemoveLocationMeter();

        void CreateNewLocationMeter();
        #endregion

        #region Methods.LocationColorMeter
        void AddLocationColorMeter(IStiAppDataCell cell);

        void AddLocationColorMeter(IStiMeter meter);

        IStiMeter GetLocationColorMeter();

        void RemoveLocationColorMeter();

        void CreateNewLocationColorMeter();
        #endregion

        #region Methods.LocationValue
        void AddLocationValueMeter(IStiAppDataCell cell);

        void AddLocationValueMeter(IStiMeter meter);

        IStiMeter GetLocationValueMeter();

        void RemoveLocationValueMeter();

        void CreateNewLocationValueMeter();
        #endregion

        #region Methods.LocationArgument
        void AddLocationArgumentMeter(IStiAppDataCell cell);

        void AddLocationArgumentMeter(IStiMeter meter);

        IStiMeter GetLocationArgumentMeter();

        void RemoveLocationArgumentMeter();

        void CreateNewLocationArgumentMeter();
        #endregion

        #region Methods.Gis
        void AddGisMeter(IStiAppDataCell cell);

        void AddGisMeter(IStiMeter meter);

        IStiMeter GetGisMeter();

        void RemoveGisMeter();

        void CreateNewGisMeter();
        #endregion

        #region Methods.GisColor
        void AddGisColorMeter(IStiAppDataCell cell);

        void AddGisColorMeter(IStiMeter meter);

        IStiMeter GetGisColorMeter();

        void RemoveGisColorMeter();

        void CreateNewGisColorMeter();
        #endregion

        #region Methods.LineSize
        void AddLineSizeMeter(IStiAppDataCell cell);

        void AddLineSizeMeter(IStiMeter meter);

        IStiMeter GetLineSizeMeter();

        void RemoveLineSizeMeter();

        void CreateNewLineSizeMeter();
        #endregion

        #region Methods.Description
        void AddDescriptionMeter(IStiAppDataCell cell);

        void AddDescriptionMeter(IStiMeter meter);

        IStiMeter GetDescriptionMeter();

        void RemoveDescriptionMeter();

        void CreateNewDescriptionMeter();
        #endregion

        StiOnlineMapLocationType LocationType { get; set; }
        
        StiOnlineMapCulture Culture { get; set; }

        Color LocationColor { get; set; }

        StiOnlineMapLocationColorType LocationColorType { get; set; }

        StiOnlineMapHeatmapColorGradientType HeatmapColorGradientType { get; set; }

        StiOnlineMapValueViewMode ValueViewMode { get; set; }

        StiFontIcons Icon { get; set; }

        Color IconColor { get; set; }

        byte[] CustomIcon { get; set; }

        List<IStiHeatmapGradientRange> HeatmapGradientRanges { get; set; }

        int HeatmapRadius { get; set; }

        int GetUniqueCode();

        StiGeoMapProviderType Provider { get; set; }
    }
}