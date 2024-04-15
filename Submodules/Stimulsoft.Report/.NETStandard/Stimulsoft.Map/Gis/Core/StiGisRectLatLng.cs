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
    public struct StiGisRectLatLng
    {
        public StiGisRectLatLng(double lat, double lng, double widthLng, double heightLat)
        {
            this.Lng = lng;
            this.Lat = lat;
            this.WidthLng = widthLng;
            this.HeightLat = heightLat;
        }

        public StiGisRectLatLng(StiGisPointLatLng location, StiGisSizeLatLng size)
        {
            this.Lng = location.Lng;
            this.Lat = location.Lat;
            this.WidthLng = size.WidthLng;
            this.HeightLat = size.HeightLat;
        }

        #region Fields
        public static readonly StiGisRectLatLng Empty = new StiGisRectLatLng();
        #endregion

        #region Properties
        public StiGisPointLatLng LocationTopLeft
        {
            get
            {
                return new StiGisPointLatLng(this.Lat, this.Lng);
            }
            set
            {
                this.Lng = value.Lng;
                this.Lat = value.Lat;
            }
        }

        public StiGisPointLatLng LocationRightBottom
        {
            get
            {
                StiGisPointLatLng ret = new StiGisPointLatLng(this.Lat, this.Lng);
                ret.Offset(HeightLat, WidthLng);
                return ret;
            }
        }

        public StiGisPointLatLng LocationMiddle
        {
            get
            {
                StiGisPointLatLng ret = new StiGisPointLatLng(this.Lat, this.Lng);
                ret.Offset(HeightLat / 2, WidthLng / 2);
                return ret;
            }
        }

        public StiGisSizeLatLng Size
        {
            get
            {
                return new StiGisSizeLatLng(this.HeightLat, this.WidthLng);
            }
            set
            {
                this.WidthLng = value.WidthLng;
                this.HeightLat = value.HeightLat;
            }
        }

        public double Lng { get; set; }

        public double Lat { get; set; }

        public double WidthLng { get; set; }

        public double HeightLat { get; set; }
        #endregion

        #region Methods
        public static StiGisRectLatLng FromLTRB(double leftLng, double topLat, double rightLng, double bottomLat)
        {
            return new StiGisRectLatLng(topLat, leftLng, rightLng - leftLng, topLat - bottomLat);
        }
        #endregion

        #region Methods.override
        public override bool Equals(object obj)
        {
            if (!(obj is StiGisRectLatLng))
                return false;

            var ef = (StiGisRectLatLng)obj;
            return ((((ef.Lng == this.Lng) && (ef.Lat == this.Lat)) && (ef.WidthLng == this.WidthLng)) && (ef.HeightLat == this.HeightLat));
        }

        public override int GetHashCode()
        {
            return (((this.Lng.GetHashCode() ^ this.Lat.GetHashCode()) ^ this.WidthLng.GetHashCode()) ^ this.HeightLat.GetHashCode());
        }

        public override string ToString()
        {
            return ("{Lat=" + this.Lat.ToString() + ",Lng=" + this.Lng.ToString() + ",WidthLng=" + this.WidthLng.ToString() + ",HeightLat=" + this.HeightLat.ToString() + "}");
        }
        #endregion

        #region Operators
        public static bool operator ==(StiGisRectLatLng left, StiGisRectLatLng right)
        {
            return ((((left.Lng == right.Lng) && (left.Lat == right.Lat)) && (left.WidthLng == right.WidthLng)) && (left.HeightLat == right.HeightLat));
        }

        public static bool operator !=(StiGisRectLatLng left, StiGisRectLatLng right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods
        public bool Contains(double lat, double lng)
        {
            return ((((this.Lng <= lng) && (lng < (this.Lng + this.WidthLng))) && (this.Lat >= lat)) && (lat > (this.Lat - this.HeightLat)));
        }

        public bool Contains(StiGisPointLatLng pt)
        {
            return this.Contains(pt.Lat, pt.Lng);
        }
        #endregion
    }
}