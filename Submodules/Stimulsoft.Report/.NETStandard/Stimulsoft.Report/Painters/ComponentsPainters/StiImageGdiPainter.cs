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

using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Helpers;
using System;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Brushes = Stimulsoft.Drawing.Brushes;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiImageGdiPainter : StiViewGdiPainter
    {
        #region Methods.Painter
        public override void Paint(StiComponent component, StiPaintEventArgs e)
        {
            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var image = component as StiImage;
            var g = e.Graphics;
            var rect = image.GetPaintRectangle();

            image.UpdateImageToDrawInDesigner();

			base.Paint(component, e);

            if (rect.Width > 0 && rect.Height > 0 && (e.ClipRectangle.IsEmpty || rect.IntersectsWith(e.ClipRectangle)))
            {
                if (!image.ExistImageToDraw() && image.IsDesigning)
                {
                    string text = null;
                    if (!string.IsNullOrEmpty(image.DataColumn))
                    {
                        text = string.Format("{0}: {1}", StiLocalization.Get("PropertyMain", "DataColumn"), image.DataColumn);
                    }
                    else if (image.ImageURL != null && !string.IsNullOrEmpty(image.ImageURL.Value))
                    {
                        text = StiHyperlinkProcessor.HyperlinkToString(image.ImageURL.Value);
                    }
                    else if (!string.IsNullOrEmpty(image.File))
                    {
                        text = string.Format("{0}: {1}", StiLocalization.Get("PropertyMain", "File"), image.File);
                    }
                    else if (image.ImageData != null && !string.IsNullOrEmpty(image.ImageData.Value))
                    {
                        text = string.Format("{0}: {1}", StiLocalization.Get("PropertyMain", "ImageData"),
                            StiExpressionPacker.PackExpression(image.ImageData.Value, image.Report.Designer, true));
                    }

                    if (text != null)
                    {
                        if (text.Length > 512)
                        {
                            text = text.Substring(0, 512) + "...";
                        }

                        using (Font font = new Font("Arial", 10 * (float)image.Report.Info.Zoom))
                            g.DrawString(text, font, Brushes.Black, rect.ToRectangleF());
                    }
                }
            }            
        }

        
        public override Image GetImage(StiComponent component, ref float zoom, StiExportFormat format)
        {
            var image = (StiImage)component;

            image.UpdateImageToDrawInDesigner();

            return base.GetImage(component, ref zoom, format);
        }
        #endregion
    }
}
