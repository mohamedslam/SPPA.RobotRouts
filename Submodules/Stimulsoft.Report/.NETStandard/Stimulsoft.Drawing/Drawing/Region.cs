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
using Stimulsoft.Drawing.Drawing2D;

namespace Stimulsoft.Drawing
{
    public sealed class Region : IDisposable
    {
        internal GraphicsPath path;
        internal System.Drawing.Region netRegion;

        public RectangleF GetBounds(Graphics g)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                return netRegion.GetBounds(g.netGraphics);
            else
                return path.GetBounds();
        }

        public void Dispose()
        {
        }

        public Region(Rectangle rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netRegion = new System.Drawing.Region(rect);
            else
            {
                var path = new GraphicsPath();
                path.AddRectangle(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
                this.path = path;
            }
        }

        public Region(RectangleF rect)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netRegion = new System.Drawing.Region(rect);
            else
            {
                var path = new GraphicsPath();
                path.AddRectangle(rect);
                this.path = path;
            }
        }

        public Region(GraphicsPath path)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netRegion = new System.Drawing.Region(path.netPath);
            else
                this.path = path;
        }
    }
}
