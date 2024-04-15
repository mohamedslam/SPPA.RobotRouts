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
    public struct StiGisSize
    {
        public StiGisSize(StiGisPoint pt)
        {
            Width = pt.X;
            Height = pt.Y;
        }

        public StiGisSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        #region Fields
        public static readonly StiGisSize Empty = new StiGisSize();
        #endregion

        #region Properties
        public bool IsEmpty => (Width == 0 && Height == 0);

        public int Width { get; set; }

        public int Height { get; set; }
        #endregion

        #region Operators
        public static bool operator ==(StiGisSize sz1, StiGisSize sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }

        public static bool operator !=(StiGisSize sz1, StiGisSize sz2)
        {
            return !(sz1 == sz2);
        }
        #endregion

        #region Methods.override
        public override bool Equals(object obj)
        {
            if (!(obj is StiGisSize))
                return false;

            var comp = (StiGisSize)obj;
            return (comp.Width == this.Width) && (comp.Height == this.Height);
        }

        public override int GetHashCode()
        {
            if (this.IsEmpty)
                return 0;

            return (Width.GetHashCode() ^ Height.GetHashCode());
        }

        public override string ToString() => $"Width={Width}, Height={Height}";
        #endregion
    }
}