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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiPictorialEmptySeriesElementGeom : StiSeriesElementGeom
    {
        #region Properties        
        private StiAnimation animation;
        public StiAnimation Animation
        {
            get
            {
                return animation;
            }
        }

        private List<RectangleF> drawRectangles;
        public List<RectangleF> DrawRectangles
        {
            get
            {
                return drawRectangles;
            }
        }

        private List<RectangleF> clipRectangles;
        public List<RectangleF> ClipRectangles
        {
            get
            {
                return clipRectangles;
            }
        }

        private StiFontIcons icon;
        public StiFontIcons Icon
        {
            get
            {
                return icon;
            }
        }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            var point = new PointF(x, y);

            for (var index = 0; index < drawRectangles.Count; index++)
            {
                var drawRect = drawRectangles[index];
                if (drawRect.Contains(point))
                    return true;
            }

            return false;
        }

        public override void Draw(StiContext context)
        {
            var singleSize = ((StiPictorialSeriesCoreXF)(this.Series.Core)).GetSingleSize(context);

            base.Draw(context);

            var fontFamilyIcons = StiFontIconsHelper.GetFontFamilyIcons();
            var font = new Font(fontFamilyIcons, 14f * context.Options.Zoom);
            var fontGeom = new StiFontGeom(font.FontFamily, font.FontFamily.Name, font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);

            for (var index = 0; index < drawRectangles.Count; index++)
            {
                var drawRect = drawRectangles[index];
                var clipRect = clipRectangles[index];
                var draw = true;
                var startPointDraw = new PointF((int)drawRect.X, (int)drawRect.Y);

                context.PushClip(clipRect);

                while (draw)
                {
                    var drawElementRect = new RectangleF(startPointDraw.X + context.Options.Zoom, startPointDraw.Y + context.Options.Zoom, singleSize.Width, singleSize.Height);
                    context.DrawString(StiFontIconsHelper.GetContent(icon), fontGeom, this.SeriesBrush, drawElementRect, this.GetStringFormatGeom(context), true, this.GetToolTip());

                    startPointDraw = new PointF(startPointDraw.X + singleSize.Width, startPointDraw.Y);
                    if (startPointDraw.X + singleSize.Width > drawRect.Right && startPointDraw.Y + singleSize.Height > drawRect.Bottom)
                    {
                        draw = false;
                    }

                    else if (startPointDraw.X + singleSize.Width > drawRect.Right)
                    {
                        startPointDraw = new PointF((int)drawRect.X, (int)startPointDraw.Y + singleSize.Height);
                    }
                }

                context.PopClip();
            }

            fontFamilyIcons.Dispose();
            font.Dispose();
        }

        protected internal StiStringFormatGeom GetStringFormatGeom(StiContext context)
        {
            var sf = context.GetGenericStringFormat();
            sf.Trimming = StringTrimming.None;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            return sf;
        }
        #endregion

        public StiPictorialEmptySeriesElementGeom(StiAreaGeom areaGeom, double value, int index,
            StiBrush seriesBrush, IStiSeries series, StiFontIcons icon, List<RectangleF> drawRectangles, List<RectangleF> clipRectangles, RectangleF clientRectangle, StiAnimation animation)
            : base(areaGeom, value, index, series, clientRectangle, seriesBrush)
        {
            this.icon = icon;
            this.drawRectangles = drawRectangles;
            this.clipRectangles = clipRectangles;
            this.animation = animation;
        }
    }
}
