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

using System.Drawing;

namespace Stimulsoft.Drawing
{
    public class SolidBrush : Brush
    {
        private SixLabors.ImageSharp.Drawing.Processing.SolidBrush sixBrush;
        internal override SixLabors.ImageSharp.Drawing.Processing.IBrush SixBrush => sixBrush;

        private System.Drawing.SolidBrush netBrush;
        internal override System.Drawing.Brush NetBrush => netBrush;

        public Color Color { get; }

        public SolidBrush(Color color)
        {
            Color = color;

            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netBrush = new System.Drawing.SolidBrush(color);
            else
                sixBrush = new SixLabors.ImageSharp.Drawing.Processing.SolidBrush(ColorExt.ToSixColor(color));
        }
    }
}
