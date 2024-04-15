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
    public struct StiGisPoint
    {
        public StiGisPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        #region Fields
        public static readonly StiGisPoint Empty = new StiGisPoint();
        #endregion

        #region Properties
        public int X { get; set; }

        public int Y { get; set; }

        public bool IsEmpty => (X == 0 && Y == 0);
        #endregion

        #region Operators
        public static bool operator ==(StiGisPoint left, StiGisPoint right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(StiGisPoint left, StiGisPoint right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods.override
        public override bool Equals(object obj)
        {
            if (!(obj is StiGisPoint))
                return false;

            var comp = (StiGisPoint)obj;
            return comp.X == this.X && comp.Y == this.Y;
        }

        public override int GetHashCode() => (int)(X ^ Y);
        #endregion

        #region Methods
        public void Offset(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        public void Offset(StiGisPoint p)
        {
            Offset(p.X, p.Y);
        }
        public void OffsetNegative(StiGisPoint p)
        {
            Offset(-p.X, -p.Y);
        }

        public override string ToString() => $"X={X}, Y={Y}";
        #endregion
    }
}