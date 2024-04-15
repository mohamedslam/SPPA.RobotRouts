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

namespace Stimulsoft.Base.Drawing
{
    public static class StiCornerRadiusHelper
    {
        #region Methods
        public static RectangleD FitRectIntoCornerRadius(RectangleD rect, StiCornerRadius radius, double scale = 1d, bool isFixedHeight = false)
        {
            var size = new SizeD();
            return FitRectIntoCornerRadius(rect, ref size, radius, scale, isFixedHeight);
        }

        public static RectangleD FitRectIntoCornerRadius(RectangleD rect, ref SizeD size, StiCornerRadius radius, double scale = 1d, bool isFixedHeight = false)
        {
            if (radius == null)
                return rect;

            var left = StiScale.I(Math.Max(radius.TopLeft / 2, radius.BottomLeft / 2) * scale);
            var right = StiScale.I(Math.Max(radius.TopRight / 2, radius.BottomRight / 2) * scale);
            var top = StiScale.I(Math.Max(radius.TopLeft / 2, radius.TopRight / 2) * scale);
            var bottom = StiScale.I(Math.Max(radius.BottomLeft / 2, radius.BottomRight / 2) * scale);

            rect.X += left;
            rect.Width -= left + right;
            size.Width -= left + right;

            if (!isFixedHeight)
            {
                rect.Y += top;
                rect.Height -= top + bottom;
                size.Height -= top + bottom;
            }

            return rect;
        } 

        public static StiCornerRadius FlipVertical(StiCornerRadius cornerRadius)
        {
            return new StiCornerRadius(cornerRadius.BottomLeft, cornerRadius.BottomRight, cornerRadius.TopRight, cornerRadius.TopLeft);
        }

        public static StiCornerRadius FlipHorizontal(StiCornerRadius cornerRadius)
        {
            return new StiCornerRadius(cornerRadius.TopRight, cornerRadius.TopLeft, cornerRadius.BottomLeft, cornerRadius.BottomRight);
        }

        public static StiCornerRadius Rotation90(StiCornerRadius cornerRadius)
        {
            return new StiCornerRadius(cornerRadius.BottomLeft, cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight);
        }
        #endregion
    }
}