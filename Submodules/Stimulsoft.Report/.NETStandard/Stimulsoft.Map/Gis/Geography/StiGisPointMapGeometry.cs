#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Map.Gis.Core;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows;

#if NETSTANDARD
using Stimulsoft.System.Windows;
using Stimulsoft.System.Windows.Media;
#else
using System.Windows.Media;
#endif

namespace Stimulsoft.Map.Gis.Geography
{
    public sealed class StiGisPointMapGeometry : 
        StiGisMapGeometry
    {
        public StiGisPointMapGeometry(StiGisPointLatLng point)
        {
            this.point = point;
        }

        public StiGisPointMapGeometry(double lat, double lng)
        {
            this.point = new StiGisPointLatLng(lat, lng);
        }

        #region Fields
        private StiGisPointLatLng point;
        private StiGisPoint localPoint;
        private Rectangle iconArea = Rectangle.Empty;
        #endregion

        #region Properties
        public string Text { get; private set; }

        public bool ShowPlacemark { get; private set; }
        #endregion

        #region Methods
        public void SetDescription(string text)
        {
            this.Text = text;
            this.ShowPlacemark = !string.IsNullOrEmpty(text);
        }

        public bool Contains(global::System.Drawing.Point pos) => this.iconArea.Contains(pos);
        #endregion

        #region Methods.override
        public override void Draw(Graphics g, StiGisCore core)
        {
            if (ShowPlacemark)
            {
                var fontFamilyIcons = StiFontIconsHelper.GetFontFamilyIcons();
                using (var font = new Font(fontFamilyIcons, 13f))
                {
                    var colorIcon = core.Icon;
                    using (var brush = new SolidBrush(core.GetIconColorGdi()))
                    {
                        var text = StiFontIconsHelper.GetContent(core.Icon);

                        if (iconArea.IsEmpty)
                        {
                            var textSize = g.MeasureString(text, font);
                            var offset = new global::System.Drawing.Point((int)(-textSize.Width / 2) + 1, (int)(-textSize.Height) + 1);
                            iconArea = new Rectangle(this.localPoint.X + offset.X, this.localPoint.Y + offset.Y, (int)textSize.Width, (int)textSize.Height);
                        }

                        g.DrawString(text, font, brush, iconArea);
                    }
                }
            }
            else
            {
                var x = Stimulsoft.Base.StiScale.XXI(5);
                var xx = Stimulsoft.Base.StiScale.XXI(10);

                using (var fill = new SolidBrush(GetFillGdiColor(core)))
                {
                    g.FillEllipse(fill, localPoint.X - x, localPoint.Y - x, xx, xx);
                }
                using (var stroke = new global::System.Drawing.Pen(GetStrokeGdiColor(core), (float)GetLineSize(core)))
                {
                    g.DrawEllipse(stroke, localPoint.X - x, localPoint.Y - x, xx, xx);
                }
            }
        }

        public override void Draw(DrawingContext dc, StiGisCore core)
        {
            if (ShowPlacemark)
            {
                var text = StiFontIconsHelper.GetContent(core.Icon);

#if NETCOREAPP
                var formatted = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface(core.WpfFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    StiScale.XXI(13), core.GetIconColorWpf(), StiScale.Factor);
#else
                var formatted = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface(core.WpfFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    StiScale.XXI(13), core.GetIconColorWpf());
#endif
                var offset = new global::System.Drawing.Point((int)(-formatted.Width / 2) + 1, (int)(-formatted.Height) + 1);
                iconArea = new Rectangle(this.localPoint.X + offset.X, this.localPoint.Y + offset.Y, (int)formatted.Width, (int)formatted.Height);
                dc.DrawText(formatted, new System.Windows.Point(iconArea.X, iconArea.Y));
            }
            else
            {
                var fill = GetFillWpfColor(core);
                var stroke = new System.Windows.Media.Pen(GetStrokeWpfColor(core), GetLineSize(core));

                dc.DrawEllipse(fill, stroke, new System.Windows.Point(localPoint.X, localPoint.Y), 5, 5);
            }
        }

        public override void UpdateLocalPosition(StiGisCore core)
        {
            var p = core.FromLatLngToLocal(this.point);
            p.OffsetNegative(core.renderOffset);

            this.localPoint = p;
            iconArea = Rectangle.Empty;
        }

        public override void GetAllPoints(ref List<StiGisPointLatLng> points)
        {
            points.Add(point);
        }
        #endregion
    }
}