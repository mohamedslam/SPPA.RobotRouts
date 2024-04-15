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
using System.Collections.Generic;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Components;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Pen = Stimulsoft.Drawing.Pen;
#endif

namespace Stimulsoft.Report.Check
{
    public abstract class StiCheck : ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone() as StiCheck;
        }
        #endregion

        #region Properties
        public object Element { get; set; }

        public virtual bool PreviewVisible => false;

        public abstract string ElementName { get; }

        public abstract string ShortMessage { get; }

        public abstract string LongMessage { get; }

        public abstract StiCheckStatus Status { get; }

        public abstract StiCheckObjectType ObjectType { get; }

        public virtual bool DefaultStateEnabled => true;

        public bool Enabled
        {
            get
            {
                StiSettings.Load();
                var str = this.GetType().ToString();
                return StiSettings.GetBool("ReportChecks", str, DefaultStateEnabled);
            }
            set
            {
                if (value == this.Enabled) return;

                StiSettings.Load();

                var str = this.GetType().ToString();

                if (value)
                    StiSettings.ClearKey("ReportChecks", str);
                else
                    StiSettings.Set("ReportChecks", str, false);

                StiSettings.Save();
            }
        }

        private List<StiAction> actions;
        public List<StiAction> Actions
        {
            get
            {
                return actions ?? (actions = new List<StiAction>());
            }
        }
        #endregion

        #region Methods
        public abstract object ProcessCheck(StiReport report, object obj);

        public virtual void CreatePreviewImage(out Image elementImage, out Image highlightedElementImage, bool useScale = false)
        {
            elementImage = null;
            highlightedElementImage = null;

            var comp = Element as StiComponent;
            if (comp == null) return;

            var width = 500;
            var height = 500;
            if (useScale)
            {
                width *= 2;
                height *= 2;
            }

            #region Prepare element image
            if (Element is StiPage)
            {
                var painter = StiPainter.GetPainter(comp.GetType(), Stimulsoft.Base.StiGuiMode.Gdi) as StiGdiPainter;
                elementImage = painter.GetThumbnail(comp, width, height, true);
            }
            else
            {
                var painter = StiPainter.GetPainter(comp.Page.GetType(), Stimulsoft.Base.StiGuiMode.Gdi) as StiGdiPainter;
                elementImage = painter.GetThumbnail(comp.Page, width, height, true);
            }
            #endregion

            #region Prepare highlighted element image
            if (!(Element is StiPage))
            {
                var painter = StiPainter.GetPainter(comp.Page.GetType(), Stimulsoft.Base.StiGuiMode.Gdi) as StiGdiPainter;

                var image = painter.GetThumbnail(comp.Page, width, height, true);
                var page = comp.Page;

                var rect = comp.GetPaintRectangle(false, false);
                rect.Y += page.Margins.Top;
                rect.X += page.Margins.Left;
                var pageRect = page.DisplayRectangle;

                var factorX = image.Width / pageRect.Width;
                var factorY = image.Height / pageRect.Height;

                rect.X *= factorX;
                rect.Y *= factorY;
                rect.Width *= factorX;
                rect.Height *= factorY;

                using (var g = Graphics.FromImage(image))
                using (var pen = new Pen(Color.FromArgb(0x77ff0000)))
                {
                    pen.Width = 3;
                    g.DrawRectangle(pen, (float)rect.X - 2, (float)rect.Y - 2, (float)rect.Width + 3, (float)rect.Height + 3);
                }

                highlightedElementImage = image;
            }
            #endregion
        }
        #endregion
    }
}