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

using System;

namespace Stimulsoft.Map.Gis.Core
{
    internal struct StiGisDrawTile : 
        IEquatable<StiGisDrawTile>, 
        IComparable<StiGisDrawTile>
    {
        #region Fields
        public StiGisPoint PosXY;
        public StiGisPoint PosPixel;
        public double DistanceSqr;
        #endregion

        #region Methods.override
        public override string ToString() => $"{PosXY} , px: {PosPixel}";
        #endregion

        #region Methods
        public bool Equals(StiGisDrawTile other)
        {
            return (PosXY == other.PosXY);
        }

        public int CompareTo(StiGisDrawTile other)
        {
            return other.DistanceSqr.CompareTo(DistanceSqr);
        }
        #endregion
    }
}