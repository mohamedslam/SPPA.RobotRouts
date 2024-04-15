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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Chart.Design;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Drawing;

#if STIDRAWING
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Brush = Stimulsoft.Drawing.Brush;
using Pen = Stimulsoft.Drawing.Pen;
using Font = Stimulsoft.Drawing.Font;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Chart
{
    [StiServiceBitmap(typeof(StiChartStyle), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [StiServiceCategoryBitmap(typeof(StiChartStyle), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [TypeConverter(typeof(StiChartStyleConverter))]
    public abstract class StiChartStyle :
        StiBaseStyle,
        IStiChartStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiChartStyle;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("Ident", this.GetType().Name);

            return jObject;
        }

        internal static StiChartStyle CreateFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            if (ident == "StiCustomStyle")
                return new StiCustomStyle();

            var service = StiOptions.Services.ChartStyles.FirstOrDefault(x => x.GetType().Name == ident);
            if (service == null)
                throw new Exception($"Type {ident} is not found!");

            return service.CreateNew();
        }
        #endregion

        #region StiService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => Core.LocalizedName;

        /// <summary>
        /// Gets a service category.
        /// </summary>
        [Browsable(false)]
        public sealed override string ServiceCategory => "Chart";

        /// <summary>
        /// Gets a service type.
        /// </summary>
        [Browsable(false)]
        public sealed override Type ServiceType => typeof(StiChartStyle);
        #endregion

        #region Properties
        [Browsable(false)]
        public virtual bool AllowDashboard => false;

        [Browsable(false)]
        public virtual StiElementStyleIdent StyleIdent => StiElementStyleIdent.Blue;

        [Browsable(false)]
        public virtual bool IsOffice2015Style => false;

        [Browsable(false)]
        public StiStyleCoreXF Core { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            if (this is StiCustomStyle && ((StiCustomStyleCoreXF)this.Core).ReportChartStyle != null)
                return ((StiCustomStyleCoreXF)this.Core).ReportChartStyle.Name;

            return ServiceName;
        }

        public bool CompareChartStyle(StiChartStyle style)
        {
            if (style == null)
                return false;

            var style1 = this as StiCustomStyle;
            var style2 = style as StiCustomStyle;

            if (style1 != null && style2 != null)
            {
                var styleName1 = ((StiCustomStyleCoreXF)style1.Core).ReportStyleName;
                if (((StiCustomStyleCoreXF)style1.Core).ReportStyle != null)
                    styleName1 = ((StiCustomStyleCoreXF)style1.Core).ReportStyle.Name;

                var styleName2 = ((StiCustomStyleCoreXF)style2.Core).ReportStyleName;
                if (((StiCustomStyleCoreXF)style2.Core).ReportStyle != null)
                    styleName2 = ((StiCustomStyleCoreXF)style2.Core).ReportStyle.Name;

                return styleName1 == styleName2;
            }

            return this.GetType() == style.GetType();
        }
        #endregion

        #region Methods.Style
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            using (Brush brush = StiBrush.GetBrush(this.Core.ChartBrush, rect))
            {
                g.FillRectangle(brush, rect);
            }

            var rectX = rect.X;

            g.SetClip(rect);
            foreach (var color in this.Core.StyleColors)
            {
                var colorRect = new Rectangle(rectX, rect.Y, 20, rect.Height);

                using (Brush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, colorRect);
                }
                rectX += 20;
            }
            g.ResetClip();

            if (!paintValue)
            {
                #region Draw Image
                if (paintImage)
                {
                    base.DrawStyleImage(g, rect);
                    rect.Width -= 16;
                    rect.X += 16;
                }
                #endregion

                using (var graphicsPath = new GraphicsPath())
                using (var font = new Font("Arial", (float)(10 * StiScale.Factor)))
                using (var sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;

                    var textRect = rect;
                    textRect.Inflate(-2, 0);

                    var textSize = g.MeasureString(this.Core.LocalizedName, font);
                    if (textRect.Width < textSize.Width)
                        sf.Alignment = StringAlignment.Near;

                    graphicsPath.AddString(this.Core.LocalizedName, font.FontFamily, (int)font.Style, font.Size, textRect, sf);

                    using (var pen = new Pen(Color.FromArgb(255, 251, 251, 251), 4))
                    {
                        pen.LineJoin = LineJoin.Round;
                        g.DrawPath(pen, graphicsPath);
                    }

                    using (var br = new SolidBrush(Color.FromArgb(255, 37, 37, 37)))
                    {
                        g.FillPath(br, graphicsPath);
                    }
                }

                #region Draw Image
                if (paintImage)
                {
                    rect.Width += 16;
                    rect.X -= 16;
                }
                #endregion
            }

            using (var pen = new Pen(this.Core.ChartAreaBorderColor))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        /// <summary>
        /// Gets a style from the component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements)
        {
        }

        /// <summary>
        /// Sets style to a component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void SetStyleToComponent(StiComponent component)
        {
            var chart = component as StiChart;
            if (chart == null)return;

            if (this is StiCustomStyle)
            {
                chart.ComponentStyle = ((StiCustomStyleCoreXF)this.Core).ReportStyleName;
            }
            else
            {
                chart.Style = (IStiChartStyle)this.Clone();
                chart.Core.ApplyStyle(chart.Style);
            }
        }
        #endregion

        #region Methods.override
        public virtual StiChartStyle CreateNew()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}