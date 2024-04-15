#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Stimulsoft.Drawing.Imaging
{
    public class ImageAttributes : IDisposable
    {
        private System.Drawing.Imaging.ImageAttributes netImageAttributes;

        public void SetColorMatrix(System.Drawing.Imaging.ColorMatrix newColorMatrix)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImageAttributes.SetColorMatrix(newColorMatrix);
        }

        public void SetColorMatrix(System.Drawing.Imaging.ColorMatrix newColorMatrix, System.Drawing.Imaging.ColorMatrixFlag flags, System.Drawing.Imaging.ColorAdjustType type)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImageAttributes.SetColorMatrix(newColorMatrix, flags, type);
        }

        public void SetRemapTable(System.Drawing.Imaging.ColorMap[] map)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImageAttributes.SetRemapTable(map);
        }

        public void SetWrapMode(WrapMode mode)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImageAttributes.SetWrapMode(mode);
        }

        public void SetColorKey(Color colorLow, Color colorHigh)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImageAttributes.SetColorKey(colorLow, colorHigh);
        }

        public void Dispose()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImageAttributes.Dispose();
        }

        public ImageAttributes()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netImageAttributes = new System.Drawing.Imaging.ImageAttributes();
        }

        public static implicit operator System.Drawing.Imaging.ImageAttributes(ImageAttributes imageAttr)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return imageAttr.netImageAttributes;

            return new System.Drawing.Imaging.ImageAttributes();
        }
    }
}

