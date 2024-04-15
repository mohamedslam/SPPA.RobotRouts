#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Map.Gis.Core;
using Stimulsoft.Map.Gis.Projections;
using System;

namespace Stimulsoft.Map.Gis.Providers
{
    public abstract class StiWikimapiaMapProviderBase : StiGisMapProvider
    {
        public StiWikimapiaMapProviderBase()
        {
            RefererUrl = "http://wikimapia.org/";
            Copyright = string.Format("© WikiMapia.org - Map data ©{0} WikiMapia", DateTime.Today.Year);
        }

        #region GMapProvider Members
        public override int MaxZoom => 18;

        internal override StiGisProjection Projection => StiMercatorGisProjection.Instance;
        #endregion

        #region Methods
        public static int GetServerNum(StiGisPoint pos)
        {
            return (int)(pos.X % 4 + (pos.Y % 4) * 4);
        }
        #endregion
    }
}