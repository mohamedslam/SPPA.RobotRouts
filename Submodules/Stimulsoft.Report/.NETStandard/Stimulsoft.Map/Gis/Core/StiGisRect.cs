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
    public struct StiGisRect
    {
        public StiGisRect(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public StiGisRect(StiGisPoint location, StiGisSize size)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Width = size.Width;
            this.Height = size.Height;
        }

        #region Fields
        public static readonly StiGisRect Empty = new StiGisRect();
        #endregion

        #region Properties
        public StiGisPoint Location
        {
            get
            {
                return new StiGisPoint(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public StiGisPoint RightBottom => new StiGisPoint(Right, Bottom);

        public StiGisPoint RightTop => new StiGisPoint(Right, Top);

        public StiGisPoint LeftBottom => new StiGisPoint(Left, Bottom);

        public StiGisSize Size
        {
            get
            {
                return new StiGisSize(Width, Height);
            }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Left => X;

        public int Top => Y;

        public int Right => X + Width;

        public int Bottom => Y + Height;

        public bool IsEmpty => Height == 0 && Width == 0 && X == 0 && Y == 0;
        #endregion

        #region Methods.override
        public override bool Equals(object obj)
        {
            if (!(obj is StiGisRect))
                return false;

            var comp = (StiGisRect)obj;

            return (comp.X == this.X) &&
               (comp.Y == this.Y) &&
               (comp.Width == this.Width) &&
               (comp.Height == this.Height);
        }

        public override int GetHashCode()
        {
            if (this.IsEmpty)
                return 0;

            return (int)(((this.X ^ ((this.Y << 13) | (this.Y >> 0x13))) ^ ((this.Width << 0x1a) | (this.Width >> 6))) ^ ((this.Height << 7) | (this.Height >> 0x19)));
        }

        public override string ToString() => $"X={X}, Y={Y}, Width={Width}, Height={Height}";
        #endregion

        #region Operators
        public static bool operator ==(StiGisRect left, StiGisRect right)
        {
            return (left.X == right.X
                       && left.Y == right.Y
                       && left.Width == right.Width
                       && left.Height == right.Height);
        }

        public static bool operator !=(StiGisRect left, StiGisRect right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods
        public bool Contains(int x, int y)
        {
            return this.X <= x &&
               x < this.X + this.Width &&
               this.Y <= y &&
               y < this.Y + this.Height;
        }

        public bool Contains(StiGisPoint pt) => Contains(pt.X, pt.Y);

        public bool Contains(StiGisRect rect)
        {
            return (this.X <= rect.X) &&
               ((rect.X + rect.Width) <= (this.X + this.Width)) &&
               (this.Y <= rect.Y) &&
               ((rect.Y + rect.Height) <= (this.Y + this.Height));
        }
        
        public void Inflate(int width, int height)
        {
            this.X -= width;
            this.Y -= height;
            this.Width += 2 * width;
            this.Height += 2 * height;
        }

        public void OffsetNegative(StiGisPoint pos)
        {
            Offset(-pos.X, -pos.Y);
        }

        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }
        #endregion
    }
}