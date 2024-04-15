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

using System;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiIconSetGdiIndicatorTypePainter : StiGdiIndicatorTypePainter
    {
        #region Methods.Painter
        public override RectangleD Paint(object context, StiComponent component, RectangleD rect)
        {  
            if (!(context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var g = (Graphics)context;
            var textComp = (StiText)component;

            float zoom = (float)(component.Report.Info.Zoom * StiScale.Factor);

            var rectComp = rect;

            if (zoom > 1)
                rectComp.Inflate(-1 * zoom, -1 * zoom);
            else
                rectComp.Inflate(-1, -1);

            var indicator = textComp.Indicator as StiIconSetIndicator;
            if (indicator != null && (indicator.Icon != StiIcon.None || indicator.CustomIcon != null))
            {
                using (var image = StiIconSetHelper.GetIcon(indicator))
                {
                    float imageWidth = (float)image.Width * zoom;
                    float imageHeight = (float)image.Height * zoom;

                    var iconRect = rectComp.ToRectangleF();

                    #region Alignment
                    switch (indicator.Alignment)
                    {
                        case ContentAlignment.TopLeft:
                            iconRect = new RectangleF(iconRect.X, iconRect.Y, imageWidth, imageHeight);
                            rect.X += iconRect.Width + 1;
                            rect.Width -= iconRect.Width + 1;
                            break;

                        case ContentAlignment.TopCenter:
                            iconRect = new RectangleF(iconRect.X + (iconRect.Width - imageWidth) / 2, iconRect.Y, imageWidth, imageHeight);
                            break;

                        case ContentAlignment.TopRight:
                            iconRect = new RectangleF(iconRect.Right - imageWidth, iconRect.Y, imageWidth, imageHeight);
                            rect.Width -= iconRect.Width + 1;
                            break;

                        case ContentAlignment.MiddleLeft:
                            iconRect = new RectangleF(iconRect.X, iconRect.Y + (iconRect.Height - imageHeight) / 2, imageWidth, imageHeight);
                            rect.X += iconRect.Width + 1;
                            rect.Width -= iconRect.Width + 1;
                            break;

                        case ContentAlignment.MiddleCenter:
                            iconRect = new RectangleF(iconRect.X + (iconRect.Width - imageWidth) / 2, iconRect.Y + (iconRect.Height - imageHeight) / 2, imageWidth, imageHeight);
                            break;

                        case ContentAlignment.MiddleRight:
                            iconRect = new RectangleF(iconRect.Right - imageWidth, iconRect.Y + (iconRect.Height - imageHeight) / 2, imageWidth, imageHeight);
                            rect.Width -= iconRect.Width + 1;
                            break;

                        case ContentAlignment.BottomLeft:
                            iconRect = new RectangleF(iconRect.X, iconRect.Bottom - imageHeight, imageWidth, imageHeight);
                            rect.X += iconRect.Width + 1;
                            rect.Width -= iconRect.Width + 1;
                            break;

                        case ContentAlignment.BottomCenter:
                            iconRect = new RectangleF(iconRect.X + (iconRect.Width - imageWidth) / 2, iconRect.Bottom - imageHeight, imageWidth, imageHeight);
                            break;

                        case ContentAlignment.BottomRight:
                            iconRect = new RectangleF(iconRect.Right - imageWidth, iconRect.Bottom - imageHeight, imageWidth, imageHeight);
                            rect.Width -= iconRect.Width + 1;
                            break;
                    }
                    #endregion

                    var gs = g.Save();
                    //g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = SmoothingMode.None;

                    g.DrawImage(image, iconRect);
                    g.Restore(gs);
                }
            }

            return rect;
        }
        #endregion
    }
}
