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

using System.ComponentModel;
using Stimulsoft.Drawing.Imaging;
using System.Drawing;
using System;
using Stimulsoft.Drawing.Drawing2D;

namespace Stimulsoft.Drawing
{
    public sealed class TextureBrush : Brush
    {
        private SixLabors.ImageSharp.Drawing.Processing.ImageBrush sixBrush;
        internal override SixLabors.ImageSharp.Drawing.Processing.IBrush SixBrush => sixBrush;

        private System.Drawing.TextureBrush netBrush;
        internal override System.Drawing.Brush NetBrush => netBrush;

        public void ScaleTransform(float sx, float sy)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netBrush.ScaleTransform(sx, sy);
        }


        public void TranslateTransform(float dx, float dy)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netBrush.TranslateTransform(dx, dy);
        }

        public TextureBrush(Image bitmap)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netBrush = new System.Drawing.TextureBrush(bitmap.netImage);
            else
            {
                bitmap.RenderDrawingOperations();
                sixBrush = new SixLabors.ImageSharp.Drawing.Processing.ImageBrush(bitmap.sixImage);
            }
        }
    }
}
