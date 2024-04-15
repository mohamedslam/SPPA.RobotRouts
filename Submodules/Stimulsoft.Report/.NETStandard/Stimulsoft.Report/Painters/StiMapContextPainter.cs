#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Report.Maps;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Painters
{
    public abstract class StiMapContextPainter
    {
        #region Properties
        public float Zoom { get; set; }

        public RectangleF Rect { get; set; }

        public StiMap Map { get; set; }
        #endregion

        #region Methods
        public static Font ChangeFontSize(Font font, float zoom)
        {
            float newFontSize = font.Size * zoom;
            if (newFontSize < 1)
                newFontSize = 1;

            return new Font(
                font.FontFamily.Name,
                newFontSize,
                font.Style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont);
        }
        #endregion

        #region Methods abstract
        public abstract SizeF MeasureString(string text, Font font);
        #endregion

        #region Methods.Render
        public abstract void Render();
        #endregion

        public StiMapContextPainter(StiMap map, RectangleF rect, float zoom)
        {
            this.Map = map;
            this.Rect = rect;
            this.Zoom = zoom;
        }
    }
}