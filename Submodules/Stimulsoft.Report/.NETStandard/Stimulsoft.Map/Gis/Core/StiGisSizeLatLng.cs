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

namespace Stimulsoft.Map.Gis.Core
{
    public struct StiGisSizeLatLng
    {
        public StiGisSizeLatLng(StiGisSizeLatLng size)
        {
            this.WidthLng = size.WidthLng;
            this.HeightLat = size.HeightLat;
        }

        public StiGisSizeLatLng(StiGisPointLatLng pt)
        {
            this.HeightLat = pt.Lat;
            this.WidthLng = pt.Lng;
        }

        public StiGisSizeLatLng(double heightLat, double widthLng)
        {
            this.HeightLat = heightLat;
            this.WidthLng = widthLng;
        }

        #region Fields
        public static readonly StiGisSizeLatLng Empty = new StiGisSizeLatLng();
        #endregion

        #region Properties
        public bool IsEmpty => (this.WidthLng == 0d) && (this.HeightLat == 0d);

        public double WidthLng { get; set; }

        public double HeightLat { get; set; }
        #endregion

        #region Operators
        public static bool operator ==(StiGisSizeLatLng sz1, StiGisSizeLatLng sz2)
        {
            return ((sz1.WidthLng == sz2.WidthLng) && (sz1.HeightLat == sz2.HeightLat));
        }

        public static bool operator !=(StiGisSizeLatLng sz1, StiGisSizeLatLng sz2)
        {
            return !(sz1 == sz2);
        }

        public static explicit operator StiGisPointLatLng(StiGisSizeLatLng size)
        {
            return new StiGisPointLatLng(size.HeightLat, size.WidthLng);
        }
        #endregion

        #region Methods.override
        public override bool Equals(object obj)
        {
            if (!(obj is StiGisSizeLatLng))
                return false;

            var ef = (StiGisSizeLatLng)obj;
            return (((ef.WidthLng == this.WidthLng) && (ef.HeightLat == this.HeightLat)) && ef.GetType().Equals(base.GetType()));
        }

        public override int GetHashCode()
        {
            if (this.IsEmpty)
                return 0;

            return (this.WidthLng.GetHashCode() ^ this.HeightLat.GetHashCode());
        }

        public override string ToString() => $"WidthLng={WidthLng}, HeightLng={HeightLat}";
        #endregion
    }
}