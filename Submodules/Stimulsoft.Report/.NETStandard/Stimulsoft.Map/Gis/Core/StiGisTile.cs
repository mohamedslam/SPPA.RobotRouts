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
    public struct StiGisTile : IDisposable
    {
        public StiGisTile(int zoom, StiGisPoint pos)
        {
            this.NotEmpty = true;
            this.Zoom = zoom;
            this.Pos = pos;
            this.Image = null;
        }

        #region Fields
        public static readonly StiGisTile Empty = new StiGisTile();
        public readonly bool NotEmpty;
        public StiGisMapImage Image;
        #endregion

        #region Properties
        public int Zoom { get; }

        public StiGisPoint Pos { get; set; }
        #endregion

        #region Methods.override
        public override bool Equals(object obj)
        {
            if (!(obj is StiGisTile))
                return false;

            var comp = (StiGisTile)obj;
            return comp.Zoom == this.Zoom && comp.Pos == this.Pos;
        }

        public override int GetHashCode() => Zoom ^ Pos.GetHashCode();
        #endregion

        #region IDisposable.override
        public void Dispose()
        {
            this.Image?.Dispose();
            this.Image = null;
        }
        #endregion

        #region Operators
        public static bool operator ==(StiGisTile m1, StiGisTile m2)
        {
            return m1.Pos == m2.Pos && m1.Zoom == m2.Zoom;
        }

        public static bool operator !=(StiGisTile m1, StiGisTile m2)
        {
            return !(m1 == m2);
        }
        #endregion
    }
}