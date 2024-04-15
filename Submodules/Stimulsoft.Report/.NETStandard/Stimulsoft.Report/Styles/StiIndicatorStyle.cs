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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Pen = Stimulsoft.Drawing.Pen;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a style for Indicator components.
    /// </summary>	
    public class StiIndicatorStyle : StiBaseStyle
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyColor(nameof(BackColor), BackColor, Color.White);
            jObject.AddPropertyColor(nameof(GlyphColor), GlyphColor, "#3498db");
            jObject.AddPropertyColor(nameof(ForeColor), ForeColor, "#8c8c8c");
            jObject.AddPropertyColor(nameof(HotBackColor), HotBackColor, Color.Transparent);
            jObject.AddPropertyColor(nameof(PositiveColor), PositiveColor, Color.Green);
            jObject.AddPropertyColor(nameof(NegativeColor), NegativeColor, Color.Red);
            jObject.AddPropertyBrush(nameof(ToolTipBrush), ToolTipBrush);
            jObject.AddPropertyBrush(nameof(ToolTipTextBrush), ToolTipTextBrush);
            jObject.AddPropertyCornerRadius(nameof(ToolTipCornerRadius), ToolTipCornerRadius);
            jObject.AddPropertyBorder(nameof(ToolTipBorder), ToolTipBorder);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(BackColor):
                        this.BackColor = property.DeserializeColor();
                        break;

                    case nameof(GlyphColor):
                        this.GlyphColor = property.DeserializeColor();
                        break;

                    case nameof(ForeColor):
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case nameof(HotBackColor):
                        this.HotBackColor = property.DeserializeColor();
                        break;

                    case nameof(PositiveColor):
                        this.PositiveColor = property.DeserializeColor();
                        break;

                    case nameof(NegativeColor):
                        this.NegativeColor = property.DeserializeColor();
                        break;

                    case nameof(ToolTipBrush):
                        this.ToolTipBrush = property.DeserializeBrush();
                        break;

                    case nameof(ToolTipTextBrush):
                        this.ToolTipTextBrush = property.DeserializeBrush();
                        break;

                    case nameof(ToolTipCornerRadius):
                        this.ToolTipCornerRadius = property.DeserializeCornerRadius();
                        break;

                    case nameof(ToolTipBorder):
                        this.ToolTipBorder = property.DeserializeSimpleBorder();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var style = base.Clone() as StiIndicatorStyle;
            style.ToolTipBrush = (StiBrush)this.ToolTipBrush.Clone();
            style.ToolTipTextBrush = (StiBrush)this.ToolTipTextBrush.Clone();
            style.ToolTipCornerRadius = (StiCornerRadius)this.ToolTipCornerRadius.Clone();
            style.ToolTipBorder = (StiSimpleBorder)this.ToolTipBorder.Clone();

            return style;
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiIndicatorStyle;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.StyleName(),
                propHelper.Description(),
                propHelper.StyleCollectionName(),
                propHelper.StyleConditions()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            list = new[]
            {
                propHelper.BackColor(),
                propHelper.ForeColor(),
                propHelper.GlyphColor(),
                propHelper.HotBackColor(),
                propHelper.NegativeColor(),
                propHelper.PositiveColor(),
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            return objHelper;
        }
        #endregion

        #region Properties
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color BackColor { get; set; } = Color.White;

        private bool ShouldSerializeBackColor()
        {
            return BackColor != Color.White;
        }

        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color GlyphColor { get; set; } = ColorTranslator.FromHtml("#3498db");

        private bool ShouldSerializeGlyphColor()
        {
            return GlyphColor != ColorTranslator.FromHtml("#3498db");
        }

        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color ForeColor { get; set; } = ColorTranslator.FromHtml("#8c8c8c");

        private bool ShouldSerializeForeColor()
        {
            return ForeColor != ColorTranslator.FromHtml("#8c8c8c");
        }

        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color HotBackColor { get; set; } = Color.Transparent;

        private bool ShouldSerializeHotBackColor()
        {
            return HotBackColor != Color.Transparent;
        }

        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color PositiveColor 
        { 
            get; 
            set; 
        } = Color.Green;

        private bool ShouldSerializePositiveColor()
        {
            return PositiveColor != Color.Green;
        }

        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color NegativeColor { get; set; } = Color.Red;

        private bool ShouldSerializeNegativeColor()
        {
            return NegativeColor != Color.Red;
        }
        #endregion

        #region Properties Tooltip
        [StiCategory("ToolTip")]
        [StiSerializable]
        public StiBrush ToolTipBrush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeToolTipBrush()
        {
            return !(ToolTipBrush is StiSolidBrush && ((StiSolidBrush)ToolTipBrush).Color == Color.White);
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public StiBrush ToolTipTextBrush { get; set; } = new StiSolidBrush(Color.Black);

        private bool ShouldSerializeToolTipTextBrush()
        {
            return !(ToolTipTextBrush is StiSolidBrush && ((StiSolidBrush)ToolTipTextBrush).Color == Color.Black);
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public StiCornerRadius ToolTipCornerRadius { get; set; } = new StiCornerRadius();

        private bool ShouldSerializeToolTipCornerRadius()
        {
            return ToolTipCornerRadius == null || !ToolTipCornerRadius.IsDefault;
        }

        [StiCategory("ToolTip")]
        [StiSerializable]
        public StiSimpleBorder ToolTipBorder { get; set; } = new StiSimpleBorder();

        private bool ShouldSerializeToolTipBorder()
        {
            return !ToolTipBorder.IsDefault;
        }
        #endregion

        #region Methods.Style
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            if (rect.Height < StiScaleUI.I(32))
            {
                var image = global::Stimulsoft.Report.Images.StylesResource.IndicatorStyle16;

#if NETCOREAPP
                g.DrawImage(image, new Point(rect.X + 4, rect.Y + (rect.Height - 16) / 2));
#else
                g.DrawImage(image, new Point(rect.X, rect.Y + (rect.Height - 16) / 2));
#endif
            }
            else
            {
                var pos = new Point(rect.X + StiScaleUI.I17, rect.Y + (rect.Height - StiScaleUI.I(32)) / 2 + StiScaleUI.I2);
                using (var path = new GraphicsPath())
                {
                    path.AddLine(pos.X + StiScaleUI.I8, pos.Y + 0, pos.X + StiScaleUI.I11, pos.Y + StiScaleUI.I5);
                    path.AddLine(pos.X + StiScaleUI.I11, pos.Y + StiScaleUI.I5, pos.X + StiScaleUI.I16, pos.Y + StiScaleUI.I6);
                    path.AddLine(pos.X + StiScaleUI.I16, pos.Y + StiScaleUI.I6, pos.X + StiScaleUI.I12, pos.Y + StiScaleUI.I10);
                    path.AddLine(pos.X + StiScaleUI.I12, pos.Y + StiScaleUI.I10, pos.X + StiScaleUI.I13, pos.Y + StiScaleUI.I16);
                    path.AddLine(pos.X + StiScaleUI.I13, pos.Y + StiScaleUI.I16, pos.X + StiScaleUI.I8, pos.Y + StiScaleUI.I13);
                    path.AddLine(pos.X + StiScaleUI.I8, pos.Y + StiScaleUI.I13, pos.X + StiScaleUI.I3, pos.Y + StiScaleUI.I16);
                    path.AddLine(pos.X + StiScaleUI.I3, pos.Y + StiScaleUI.I16, pos.X + StiScaleUI.I4, pos.Y + StiScaleUI.I10);
                    path.AddLine(pos.X + StiScaleUI.I4, pos.Y + StiScaleUI.I10, pos.X + 0, pos.Y + StiScaleUI.I6);
                    path.AddLine(pos.X + 0, pos.Y + StiScaleUI.I6, pos.X + StiScaleUI.I5, pos.Y + StiScaleUI.I5);
                    path.AddLine(pos.X + StiScaleUI.I5, pos.Y + StiScaleUI.I5, pos.X + StiScaleUI.I8, pos.Y + 0);

                    using (var brush = new SolidBrush(this.GlyphColor))
                        g.FillPath(brush, path);
                }

                using (var br = new SolidBrush(this.ForeColor))
                using (var font = new Font("Arial", 8f))
                using (var sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    g.DrawString("100", font, br, new Rectangle(rect.X + StiScaleUI.I9, rect.Y + (rect.Height - StiScaleUI.I(32)) / 2 + StiScaleUI.I20, StiScaleUI.I(32), StiScaleUI.I12), sf);
                }
            }

            DrawStyleName(g, rect);
        }

        /// <summary>
        /// Gets a style from the component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements)
        {
            if (styleElements != StiStyleElements.All)
                throw new Exception("StiIndicatorStyle support only StiStyleElements.All.");
        }

        /// <summary>
        /// Sets style to a component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void SetStyleToComponent(StiComponent component)
        {
            if (component is StiSparkline)
            {
                var sparkLine = component as StiSparkline;
                sparkLine.Brush = new StiSolidBrush(BackColor);
                sparkLine.PositiveColor = PositiveColor;
                sparkLine.NegativeColor = NegativeColor;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiIndicatorStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        internal StiIndicatorStyle(string name, string description, StiReport report)
            : base(name, description, report)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiIndicatorStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiIndicatorStyle(string name, string description)
            : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiIndicatorStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiIndicatorStyle(string name)
            : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiIndicatorStyle.
        /// </summary>
        public StiIndicatorStyle()
            : this("")
        {
        }
    }
}
