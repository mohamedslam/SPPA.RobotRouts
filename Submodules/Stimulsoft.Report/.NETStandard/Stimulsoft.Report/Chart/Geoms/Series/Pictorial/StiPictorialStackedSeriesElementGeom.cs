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
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Chart
{
    public class StiPictorialStackedSeriesElementGeom : StiSeriesElementGeom
    {
        #region Properties        
        public StiAnimation Animation { get; private set; }

        public RectangleF ClipRectangle { get; private set; }

        public StiFontIcons Icon { get; private set; }
        #endregion

        #region Methods
        public override bool Contains(float x, float y)
        {
            if (Invisible) return false;

            return ClipRectangle.Contains(x, y);
        }

        public override void Draw(StiContext context)
        {
            base.Draw(context);

            if (this.ClipRectangle.Width <=0 || this.ClipRectangle.Height <=0) return;

            context.PushClip(this.ClipRectangle);

            var fontGeom = GetFontGeom(context);

            if (fontGeom.FontSize <= 0) return;

            context.DrawAnimationRectangle(Color.Transparent, null, this.ClipRectangle, null, null, GetInteractionData(), this.GetToolTip());

            context.DrawRotatedString(StiFontIconsHelper.GetContent(this.Icon), fontGeom, this.SeriesBrush, this.ClientRectangle,
                GetStringFormatGeom(context), StiRotationMode.CenterCenter, 0f, true);

            context.PopClip();
        }

        private StiFontGeom GetFontGeom(StiContext context)
        {
            var fontFamilyIcons = StiFontIconsHelper.GetFontFamilyIcons();
            var font = new Font(fontFamilyIcons, 1);

            var fontSizeDelta = MeasureFontSize(context, this.ClientRectangle, font);

            var fontNew = new Font(fontFamilyIcons, fontSizeDelta.GetValueOrDefault());
            var fontGeom = new StiFontGeom(fontNew.FontFamily, fontNew.FontFamily.Name, fontNew.Size, fontNew.Style,
                        fontNew.Unit, fontNew.GdiCharSet, fontNew.GdiVerticalFont);

            fontFamilyIcons.Dispose();
            font.Dispose();

            return fontGeom;
        }

        private float? MeasureFontSize(StiContext context, RectangleF rect, Font font)
        {
            float? fontSize = null;
            var textIcon = StiFontIconsHelper.GetContent(this.Icon);

            var size = context.MeasureString(textIcon, new StiFontGeom(new Font(font.FontFamily.Name, 1, font.Style)));

            if (size.Width == 0 || size.Height == 0)
                return 0;
            
            if (rect.Width > rect.Height)
            {
                var factorX = rect.Width / size.Width;
                var factorY = rect.Height / size.Height;
                var factor = factorX > factorY ? factorY : factorX;

                if (fontSize == null)
                    fontSize = factor;

                else if (fontSize > factor)
                    fontSize = factor;
            }

            else
            {
                var factor = rect.Width / size.Width * size.Width / size.Height;

                if (fontSize == null)
                    fontSize = factor;

                else if (fontSize > factor)
                    fontSize = factor;
            }

            return fontSize;
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

        public StiPictorialStackedSeriesElementGeom(StiAreaGeom areaGeom, double value, int index,
            StiBrush seriesBrush, IStiSeries series, StiFontIcons icon,  RectangleF clientRectangle, RectangleF clipRectangle, StiAnimation animation)
            : base(areaGeom, value, index, series, clientRectangle, seriesBrush)
        {
            this.Icon = icon;
            this.ClipRectangle = clipRectangle;
            this.Animation = animation;
        }
    }
}
