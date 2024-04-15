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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Gauge
{
    [TypeConverter(typeof(Stimulsoft.Report.Gauge.Design.StiGaugeStyleConverter))]
    public abstract class StiGaugeStyleXF :
        StiBaseStyle,
        IStiGaugeStyle
    {
        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiGaugeStyle;

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

        public override void LoadFromJsonObject(JObject jObject)
        {

        }

        internal static StiGaugeStyleXF CreateFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            if (ident == "StiGaugeStyleXF")
                return new StiCustomGaugeStyle();

            var service = StiOptions.Services.GaugeStyles.FirstOrDefault(x => x.GetType().Name == ident);

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
        public sealed override string ServiceCategory => "Gauge";

        /// <summary>
        /// Gets a service type.
        /// </summary>
        [Browsable(false)]
        public sealed override Type ServiceType => typeof(StiGaugeStyleXF);
        #endregion

        #region Properties
        [Browsable(false)]
        public StiGaugeStyleCoreXF Core { get; set; }

        [Browsable(false)]
        public virtual bool AllowDashboard => false;

        [Browsable(false)]
        public abstract StiElementStyleIdent StyleIdent { get; }
        #endregion	

        #region Methods
        public override string ToString()
        {
            if (this is StiCustomGaugeStyle && ((StiCustomGaugeStyleCoreXF)this.Core).ReportGaugeStyle != null)
                return ((StiCustomGaugeStyleCoreXF)this.Core).ReportGaugeStyle.Name;

            return ServiceName;
        }


        public bool CompareGaugeStyle(StiGaugeStyleXF style)
        {
            if (style == null) return false;
            var style1 = this as StiCustomGaugeStyle;
            var style2 = style as StiCustomGaugeStyle;

            if (style1 != null && style2 != null)
            {
                string styleName1 = ((StiCustomGaugeStyleCoreXF)style1.Core).ReportStyleName;
                string styleName2 = ((StiCustomGaugeStyleCoreXF)style2.Core).ReportStyleName;

                return (styleName1 == styleName2);
            }

            return this.GetType() == style.GetType();
        }
        #endregion

        #region Methods.override
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            var rectElement = new Rectangle(rect.X + 5, rect.Y + 5, 40, 30);
            var rectGauge = new Rectangle(
                    rectElement.X + rectElement.Width / 2 - rectElement.Height / 2,
                    rectElement.Y,
                    rectElement.Height,
                    rectElement.Height);

            #region Draw Body Gauge
            using (var brush = StiBrush.GetBrush(this.Core.Brush, rectElement))
                g.FillEllipse(brush, rectGauge);

            g.DrawEllipse(Pens.LightGray, rectGauge);

            var rectCenter = new Rectangle(
                    rectElement.X + rectElement.Width / 2 - 2,
                    rectElement.Y + rectElement.Height / 2 - 2,
                    4,
                    4);
            #endregion

            #region Draw Needle
            using (var brush = StiBrush.GetBrush(this.Core.NeedleCapBrush, rectCenter))
            using (var pen = new Pen(brush))
                g.DrawEllipse(pen, rectCenter);

            g.DrawLine(new Pen(StiBrush.ToColor(this.Core.NeedleBrush)),
                rectGauge.X + rectGauge.Width / 2 + 2,
                rectGauge.Y + rectGauge.Height / 2,
                rectGauge.X + rectGauge.Width + 2,
                rectGauge.Y + rectGauge.Height / 2);
            #endregion

            #region Draw Text
            rect.X += rectElement.Right + 5;
            rect.Width -= rectElement.Width - 10;

            using (var br = new SolidBrush(Color.Black))
            using (var font = new Font("Arial", 10))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                var textRect = rect;
                textRect.Inflate(-2, 0);
                var textSize = g.MeasureString(this.Name, font);
                if (textRect.Width < textSize.Width)
                {
                    sf.Alignment = StringAlignment.Near;
                }

                g.DrawString(this.Name, font, br, rect, sf);
            }
            #endregion
        }

        public override void DrawBox(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            rect.X++;
            rect.Y++;
            rect.Width -= 2;
            rect.Height -= 3;


            this.DrawStyle(g, rect, paintValue, paintImage);

            using (var pen = new Pen(Color.FromArgb(150, Color.Gray)))
            {
                pen.DashStyle = DashStyle.Dash;
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

        }

        public virtual StiGaugeStyleXF CreateNew() => throw new NotImplementedException();
        #endregion
    }
}