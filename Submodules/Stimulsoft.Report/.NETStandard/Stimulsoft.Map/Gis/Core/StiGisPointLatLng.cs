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
    public struct StiGisPointLatLng
    {
        public StiGisPointLatLng(double lat, double lng)
        {
            this.Lat = lat;
            this.Lng = lng;
        }

        #region Fields
        public static readonly StiGisPointLatLng Empty = new StiGisPointLatLng();
        #endregion

        #region Properties
        public double Lat { get; set; }

        public double Lng { get; set; }
        #endregion

        #region Operators
        public static bool operator ==(StiGisPointLatLng left, StiGisPointLatLng right)
        {
            return ((left.Lng == right.Lng) && (left.Lat == right.Lat));
        }

        public static bool operator !=(StiGisPointLatLng left, StiGisPointLatLng right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods.override
        public override bool Equals(object obj)
        {
            if (!(obj is StiGisPointLatLng))
                return false;

            var tf = (StiGisPointLatLng)obj;
            return (((tf.Lng == this.Lng) && (tf.Lat == this.Lat)) && tf.GetType().Equals(base.GetType()));
        }

        public override int GetHashCode()
        {
            return (this.Lng.GetHashCode() ^ this.Lat.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("{{Lat={0}, Lng={1}}}", this.Lat, this.Lng);
        }
        #endregion

        #region Methods
        public void Offset(double lat, double lng)
        {
            this.Lng += lng;
            this.Lat -= lat;
        }
        #endregion
    }
}