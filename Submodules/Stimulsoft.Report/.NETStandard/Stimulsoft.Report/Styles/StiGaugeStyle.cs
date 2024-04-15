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
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a style for Gauge components.
    /// </summary>	
    public class StiGaugeStyle : StiBaseStyle
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyColor("BorderColor", BorderColor, Color.Transparent);
            jObject.AddPropertyColor("ForeColor", ForeColor, Color.Black);
            jObject.AddPropertyColor("TargetColor", TargetColor, Color.Black);
            jObject.AddPropertyFloat("BorderWidth", BorderWidth, 0f);
            jObject.AddPropertyBrush("TickMarkMajorBrush", TickMarkMajorBrush);
            jObject.AddPropertyBrush("TickMarkMajorBorder", TickMarkMajorBorder);
            jObject.AddPropertyFloat("TickMarkMajorBorderWidth", TickMarkMajorBorderWidth, 1f);
            jObject.AddPropertyBrush("TickMarkMinorBrush", TickMarkMinorBrush);
            jObject.AddPropertyBrush("TickMarkMinorBorder", TickMarkMinorBorder);
            jObject.AddPropertyFloat("TickMarkMinorBorderWidth", TickMarkMinorBorderWidth, 1f);
            jObject.AddPropertyBrush("TickLabelMajorTextBrush", TickLabelMajorTextBrush);
            jObject.AddPropertyFontArial8("TickLabelMajorFont", TickLabelMajorFont);
            jObject.AddPropertyBrush("TickLabelMinorTextBrush", TickLabelMinorTextBrush);
            jObject.AddPropertyFontArial8("TickLabelMinorFont", TickLabelMinorFont);
            jObject.AddPropertyBrush("MarkerBrush", MarkerBrush);
            jObject.AddPropertyBrush("LinearBarBrush", LinearBarBrush);
            jObject.AddPropertyBrush("LinearBarBorderBrush", LinearBarBorderBrush);
            jObject.AddPropertyBrush("LinearBarEmptyBrush", LinearBarEmptyBrush);
            jObject.AddPropertyBrush("LinearBarEmptyBorderBrush", LinearBarEmptyBorderBrush);
            jObject.AddPropertyBrush("RadialBarBrush", RadialBarBrush);
            jObject.AddPropertyBrush("RadialBarBorderBrush", RadialBarBorderBrush);
            jObject.AddPropertyBrush("RadialBarEmptyBrush", RadialBarEmptyBrush);
            jObject.AddPropertyBrush("RadialBarEmptyBorderBrush", RadialBarEmptyBorderBrush);
            jObject.AddPropertyBrush("NeedleBrush", NeedleBrush);
            jObject.AddPropertyBrush("NeedleBorderBrush", NeedleBorderBrush);
            jObject.AddPropertyBrush("LinearScaleBrush", LinearScaleBrush);
            jObject.AddPropertyFloat("NeedleBorderWidth", NeedleBorderWidth, 1f);
            jObject.AddPropertyBrush("NeedleCapBrush", NeedleCapBrush);
            jObject.AddPropertyBrush("NeedleCapBorderBrush", NeedleCapBorderBrush);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "LinearScaleBrush":
                        this.LinearScaleBrush = property.DeserializeBrush();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "BorderColor":
                        this.BorderColor = property.DeserializeColor();
                        break;

                    case "ForeColor":
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case "TargetColor":
                        this.TargetColor = property.DeserializeColor();
                        break;

                    case "BorderWidth":
                        this.BorderWidth = property.DeserializeFloat();
                        break;

                    case "TickMarkMajorBrush":
                        this.TickMarkMajorBrush = property.DeserializeBrush();
                        break;

                    case "TickMarkMajorBorder":
                        this.TickMarkMajorBorder = property.DeserializeBrush();
                        break;

                    case "TickMarkMajorBorderWidth":
                        this.TickMarkMajorBorderWidth = property.DeserializeFloat();
                        break;

                    case "TickMarkMinorBrush":
                        this.TickMarkMinorBrush = property.DeserializeBrush();
                        break;

                    case "TickMarkMinorBorder":
                        this.TickMarkMinorBorder = property.DeserializeBrush();
                        break;

                    case "TickMarkMinorBorderWidth":
                        this.TickMarkMinorBorderWidth = property.DeserializeFloat();
                        break;

                    case "TickLabelMajorTextBrush":
                        this.TickLabelMajorTextBrush = property.DeserializeBrush();
                        break;

                    case "TickLabelMajorFont":
                        this.TickLabelMajorFont = property.DeserializeFont(this.TickLabelMajorFont);
                        break;

                    case "TickLabelMinorTextBrush":
                        this.TickLabelMinorTextBrush = property.DeserializeBrush();
                        break;

                    case "TickLabelMinorFont":
                        this.TickLabelMinorFont = property.DeserializeFont(this.TickLabelMinorFont);
                        break;

                    case "MarkerBrush":
                        this.MarkerBrush = property.DeserializeBrush();
                        break;

                    case "LinearBarBrush":
                        this.LinearBarBrush = property.DeserializeBrush();
                        break;

                    case "LinearBarBorderBrush":
                        this.LinearBarBorderBrush = property.DeserializeBrush();
                        break;

                    case "LinearBarEmptyBrush":
                        this.LinearBarEmptyBrush = property.DeserializeBrush();
                        break;

                    case "LinearBarEmptyBorderBrush":
                        this.LinearBarEmptyBorderBrush = property.DeserializeBrush();
                        break;

                    case "RadialBarBrush":
                        this.RadialBarBrush = property.DeserializeBrush();
                        break;

                    case "RadialBarBorderBrush":
                        this.RadialBarBorderBrush = property.DeserializeBrush();
                        break;

                    case "RadialBarEmptyBrush":
                        this.RadialBarEmptyBrush = property.DeserializeBrush();
                        break;

                    case "RadialBarEmptyBorderBrush":
                        this.RadialBarEmptyBorderBrush = property.DeserializeBrush();
                        break;

                    case "NeedleBrush":
                        this.NeedleBrush = property.DeserializeBrush();
                        break;

                    case "NeedleBorderBrush":
                        this.NeedleBorderBrush = property.DeserializeBrush();
                        break;

                    case "NeedleBorderWidth":
                        this.NeedleBorderWidth = property.DeserializeFloat();
                        break;

                    case "NeedleCapBrush":
                        this.NeedleCapBrush = property.DeserializeBrush();
                        break;

                    case "NeedleCapBorderBrush":
                        this.NeedleCapBorderBrush = property.DeserializeBrush();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiGaugeStyle;

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
                propHelper.MarkerBrush()
            };
            objHelper.Add(StiPropertyCategories.Market, list);

            list = new[]
            {
                propHelper.BorderWidth(),
                propHelper.BorderColor(),
                propHelper.Brush()
            };
            objHelper.Add(StiPropertyCategories.Misc, list);

            list = new[]
            {
                propHelper.TickMarkMajorBorder(),
                propHelper.TickMarkMajorBrush()
            };
            objHelper.Add(StiPropertyCategories.TickMarkMajor, list);

            list = new[]
            {
                propHelper.TickMarkMinorBorder(),
                propHelper.TickMarkMinorBrush()
            };
            objHelper.Add(StiPropertyCategories.TickMarkMinor, list);

            list = new[]
            {
                propHelper.TickLabelMajorTextBrush(),
                propHelper.TickLabelMajorFont()
            };
            objHelper.Add(StiPropertyCategories.TickLabelMajor, list);

            list = new[]
            {
                propHelper.TickLabelMinorTextBrush(),
                propHelper.TickLabelMinorFont()
            };
            objHelper.Add(StiPropertyCategories.TickLabelMinor, list);

            list = new[]
            {
                propHelper.LinearBarBorderBrush(),
                propHelper.LinearBarBrush(),
                propHelper.LinearBarEmptyBorderBrush(),
                propHelper.LinearBarEmptyBrush()
            };
            objHelper.Add(StiPropertyCategories.LinearScaleBar, list);

            list = new[]
            {
                propHelper.RadialBarBorderBrush(),
                propHelper.RadialBarBrush(),
                propHelper.RadialBarEmptyBorderBrush(),
                propHelper.RadialBarEmptyBrush()
            };
            objHelper.Add(StiPropertyCategories.RadialScaleBar, list);

            list = new[]
            {
                propHelper.NeedleBorderBrush(),
                propHelper.NeedleBrush(),
                propHelper.NeedleCapBorderBrush(),
                propHelper.NeedleCapBrush()
            };
            objHelper.Add(StiPropertyCategories.Needle, list);

            return objHelper;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var style = base.Clone() as StiGaugeStyle;
            style.Brush = this.Brush.Clone() as StiBrush;
            style.TickMarkMajorBrush = this.TickMarkMajorBrush.Clone() as StiBrush;
            style.TickMarkMajorBorder = this.TickMarkMajorBorder.Clone() as StiBrush;
            style.TickMarkMinorBrush = this.TickMarkMinorBrush.Clone() as StiBrush;
            style.TickMarkMinorBorder = this.TickMarkMinorBorder.Clone() as StiBrush;
            style.TickLabelMajorTextBrush = this.TickLabelMajorTextBrush.Clone() as StiBrush;
            style.TickLabelMajorFont = this.TickLabelMajorFont.Clone() as Font;
            style.TickLabelMinorTextBrush = this.TickLabelMinorTextBrush.Clone() as StiBrush;
            style.TickLabelMinorFont = this.TickLabelMinorFont.Clone() as Font;
            style.MarkerBrush = this.MarkerBrush.Clone() as StiBrush;
            style.LinearBarBrush = this.LinearBarBrush.Clone() as StiBrush;
            style.LinearScaleBrush = this.LinearScaleBrush.Clone() as StiBrush;
            style.LinearBarBorderBrush = this.LinearBarBorderBrush.Clone() as StiBrush;
            style.LinearBarEmptyBrush = this.LinearBarEmptyBrush.Clone() as StiBrush;
            style.LinearBarEmptyBorderBrush = this.LinearBarEmptyBorderBrush.Clone() as StiBrush;
            style.RadialBarBrush = this.RadialBarBrush.Clone() as StiBrush;
            style.RadialBarBorderBrush = this.RadialBarBorderBrush.Clone() as StiBrush;
            style.RadialBarEmptyBrush = this.RadialBarEmptyBrush.Clone() as StiBrush;
            style.RadialBarEmptyBorderBrush = this.RadialBarEmptyBorderBrush.Clone() as StiBrush;
            style.NeedleBrush = this.NeedleBrush.Clone() as StiBrush;
            style.NeedleBorderBrush = this.NeedleBorderBrush.Clone() as StiBrush;
            style.NeedleCapBrush = this.NeedleCapBrush.Clone() as StiBrush;
            style.NeedleCapBorderBrush = this.NeedleCapBorderBrush.Clone() as StiBrush;

            return style;
        }
        #endregion

        #region Properties
        [StiSerializable]
        [StiCategory("Appearance")]
        public StiBrush Brush { get; set; } = new StiSolidBrush(Color.White);

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.White);
        }

        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color BorderColor { get; set; } = Color.Transparent;

        private bool ShouldSerializeBorderColor()
        {
            return BorderColor != Color.Transparent;
        }

        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color TargetColor { get; set; } = Color.Black;

        private bool ShouldSerializeTargetColor()
        {
            return TargetColor != Color.Black;
        }

        [StiSerializable]
        [StiCategory("Appearance")]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color ForeColor { get; set; } = Color.Black;

        private bool ShouldSerializeForeColor()
        {
            return ForeColor != Color.Transparent;
        }

        [StiSerializable]
        [StiCategory("Appearance")]
        [DefaultValue(0f)]
        public float BorderWidth { get; set; }

        private bool ShouldSerializeBorderWidth()
        {
            return BorderWidth != 0f;
        }


        [StiCategory("TickMarkMajor")]
        [StiSerializable]
        public StiBrush TickMarkMajorBrush { get; set; } = new StiSolidBrush("#990000");

        private bool ShouldSerializeTickMarkMajorBrush()
        {
            return !(TickMarkMajorBrush is StiSolidBrush && ((StiSolidBrush)TickMarkMajorBrush).Color == ColorTranslator.FromHtml("#990000"));
        }

        [StiCategory("TickMarkMajor")]
        [StiSerializable]
        public StiBrush TickMarkMajorBorder { get; set; } = new StiSolidBrush("#0bac45");

        private bool ShouldSerializeTickMarkMajorBorder()
        {
            return !(TickMarkMajorBorder is StiSolidBrush && ((StiSolidBrush)TickMarkMajorBorder).Color == ColorTranslator.FromHtml("#0bac45"));
        }

        [StiCategory("TickMarkMajor")]
        [StiSerializable]
        public float TickMarkMajorBorderWidth { get; set; } = 1f;

        private bool ShouldSerializeTickMarkMajorBorderWidth()
        {
            return TickMarkMajorBorderWidth != 1f;
        }

        [StiCategory("TickMarkMinor")]
        [StiSerializable]
        public StiBrush TickMarkMinorBrush { get; set; } = new StiSolidBrush("#4472c4");

        private bool ShouldSerializeTickMarkMinorBrush()
        {
            return !(TickMarkMinorBrush is StiSolidBrush && ((StiSolidBrush)TickMarkMinorBrush).Color == ColorTranslator.FromHtml("#4472c4"));
        }

        [StiCategory("TickMarkMinor")]
        [StiSerializable]
        public StiBrush TickMarkMinorBorder { get; set; } = new StiSolidBrush("#4472c4");

        private bool ShouldSerializeTickMarkMinorBorder()
        {
            return !(TickMarkMinorBorder is StiSolidBrush && ((StiSolidBrush)TickMarkMinorBorder).Color == ColorTranslator.FromHtml("#4472c4"));
        }

        [StiCategory("TickMarkMinor")]
        [StiSerializable]
        public float TickMarkMinorBorderWidth { get; set; } = 1f;

        private bool ShouldSerializeTickMarkMinorBorderWidth()
        {
            return TickMarkMinorBorderWidth != 1f;
        }

        [StiCategory("TickLabelMajor")]
        [StiSerializable]
        public StiBrush TickLabelMajorTextBrush { get; set; } = new StiSolidBrush("#33475B");

        private bool ShouldSerializeTickLabelMajorTextBrush()
        {
            return !(TickLabelMajorTextBrush is StiSolidBrush && ((StiSolidBrush)TickLabelMajorTextBrush).Color == ColorTranslator.FromHtml("#FF33475B"));
        }

        private Font tickLabelMajorFont = new Font("Arial", 10);
        [StiCategory("TickLabelMajor")]
        [StiSerializable]
        public Font TickLabelMajorFont
        {
            get
            {
                return tickLabelMajorFont;
            }
            set
            {
                if (tickLabelMajorFont != value)
                {
                    if (value == null)
                        value = new Font("Arial", 10);

                    tickLabelMajorFont = value;
                }
            }
        }

        private bool ShouldSerializeTickLabelMajorFont()
        {
            return !(
                TickLabelMajorFont != null &&
                TickLabelMajorFont.Name == "Arial" &&
                TickLabelMajorFont.Size == 10 &&
                TickLabelMajorFont.Style == FontStyle.Regular);
        }

        [StiCategory("TickLabelMinor")]
        [StiSerializable]
        public StiBrush TickLabelMinorTextBrush { get; set; } = new StiSolidBrush("#33475B");

        private bool ShouldSerializeTickLabelMinorTextBrush()
        {
            return !(TickLabelMinorTextBrush is StiSolidBrush && ((StiSolidBrush)TickLabelMinorTextBrush).Color == ColorTranslator.FromHtml("#FF33475B"));
        }

        private Font tickLabelMinorFont = new Font("Arial", 10);
        [StiCategory("TickLabelMinor")]
        [StiSerializable]
        public Font TickLabelMinorFont
        {
            get
            {
                return tickLabelMinorFont;
            }
            set
            {
                if (tickLabelMinorFont != value)
                {
                    if (value == null)
                        value = new Font("Arial", 10);

                    tickLabelMinorFont = value;
                }
            }
        }

        private bool ShouldSerializeTickLabelMinorFont()
        {
            return !(
                TickLabelMinorFont != null &&
                TickLabelMinorFont.Name == "Arial" &&
                TickLabelMinorFont.Size == 10 &&
                TickLabelMinorFont.Style == FontStyle.Regular);
        }

        [StiCategory("Marker")]
        [StiSerializable]
        public StiBrush MarkerBrush { get; set; } = new StiSolidBrush("#70ad47");

        private bool ShouldSerializeMarkerBrush()
        {
            return !(MarkerBrush is StiSolidBrush && ((StiSolidBrush)MarkerBrush).Color == ColorTranslator.FromHtml("#70ad47"));
        }

        [StiCategory("LinearScaleBar")]
        [StiSerializable]
        public StiBrush LinearBarBrush { get; set; } = new StiSolidBrush("#4472c4");

        private bool ShouldSerializeLinearBarBrush()
        {
            return !(LinearBarBrush is StiSolidBrush && ((StiSolidBrush)LinearBarBrush).Color == ColorTranslator.FromHtml("#4472c4"));
        }

        [StiCategory("LinearScale")]
        [StiSerializable]
        public StiBrush LinearScaleBrush { get; set; } = new StiSolidBrush("#70ad47");

        private bool ShouldSerializeLinearScaleBrush()
        {
            return !(LinearScaleBrush is StiSolidBrush && ((StiSolidBrush)LinearScaleBrush).Color == ColorTranslator.FromHtml("#70ad47"));
        }

        [StiCategory("LinearScaleBar")]
        [StiSerializable]
        public StiBrush LinearBarBorderBrush { get; set; } = new StiEmptyBrush();

        private bool ShouldSerializeLinearBarBorderBrush()
        {
            return !(LinearBarBorderBrush is StiEmptyBrush);
        }

        [StiCategory("LinearScaleBar")]
        [StiSerializable]
        public StiBrush LinearBarEmptyBrush { get; set; } = new StiEmptyBrush();

        private bool ShouldSerializeLinearBarEmptyBrush()
        {
            return !(LinearBarEmptyBrush is StiEmptyBrush);
        }

        [StiCategory("LinearScaleBar")]
        [StiSerializable]
        public StiBrush LinearBarEmptyBorderBrush { get; set; } = new StiEmptyBrush();

        private bool ShouldSerializeLinearBarEmptyBorderBrush()
        {
            return !(LinearBarEmptyBorderBrush is StiEmptyBrush);
        }

        [StiCategory("RadialScaleBar")]
        [StiSerializable]
        public StiBrush RadialBarBrush { get; set; } = new StiSolidBrush("#ffc000");

        private bool ShouldSerializeRadialBarBrush()
        {
            return !(RadialBarBrush is StiSolidBrush && ((StiSolidBrush)RadialBarBrush).Color == ColorTranslator.FromHtml("#ffc000"));
        }

        [StiCategory("RadialScaleBar")]
        [StiSerializable]
        public StiBrush RadialBarBorderBrush { get; set; } = new StiEmptyBrush();

        private bool ShouldSerializeRadialBarBorderBrush()
        {
            return !(RadialBarBorderBrush is StiEmptyBrush);
        }

        [StiCategory("RadialScaleBar")]
        [StiSerializable]
        public StiBrush RadialBarEmptyBrush { get; set; } = new StiSolidBrush("#43682b");

        private bool ShouldSerializeRadialBarEmptyBrush()
        {
            return !(RadialBarEmptyBrush is StiSolidBrush && ((StiSolidBrush)RadialBarEmptyBrush).Color == ColorTranslator.FromHtml("#43682b"));
        }

        [StiCategory("RadialScaleBar")]
        [StiSerializable]
        public StiBrush RadialBarEmptyBorderBrush { get; set; } = new StiEmptyBrush();

        private bool ShouldSerializeRadialBarEmptyBorderBrush()
        {
            return !(RadialBarEmptyBorderBrush is StiEmptyBrush);
        }

        [StiCategory("Needle")]
        [StiSerializable]
        public StiBrush NeedleBrush { get; set; } = new StiSolidBrush("#ffc000");

        private bool ShouldSerializeNeedleBrush()
        {
            return !(NeedleBrush is StiSolidBrush && ((StiSolidBrush)NeedleBrush).Color == ColorTranslator.FromHtml("#ffc000"));
        }

        [StiCategory("Needle")]
        [StiSerializable]
        public StiBrush NeedleBorderBrush { get; set; } = new StiEmptyBrush();

        private bool ShouldSerializeNeedleBorderBrush()
        {
            return !(NeedleBorderBrush is StiEmptyBrush);
        }

        [StiCategory("Needle")]
        [StiSerializable]
        public float NeedleBorderWidth { get; set; } = 1f;

        private bool ShouldSerializeNeedleBorderWidth()
        {
            return NeedleBorderWidth != 1f;
        }

        [StiCategory("Needle")]
        [StiSerializable]
        public StiBrush NeedleCapBrush { get; set; } = new StiSolidBrush("#ffc000");

        private bool ShouldSerializeNeedleCapBrush()
        {
            return !(NeedleCapBrush is StiSolidBrush && ((StiSolidBrush)NeedleCapBrush).Color == ColorTranslator.FromHtml("#ffc000"));
        }

        [StiCategory("Needle")]
        [StiSerializable]
        public StiBrush NeedleCapBorderBrush { get; set; } = new StiSolidBrush("#ffc000");

        private bool ShouldSerializeNeedleCapBorderBrush()
        {
            return !(NeedleCapBorderBrush is StiSolidBrush && ((StiSolidBrush)NeedleCapBorderBrush).Color == ColorTranslator.FromHtml("#ffc000"));
        }
        #endregion

        #region Methods.Style
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            var rectElement = new Rectangle(rect.X + StiScaleUI.I11, rect.Y + StiScaleUI.I6, StiScaleUI.I(29), StiScaleUI.I(29));
            var rectGauge = new Rectangle(
                    rectElement.X + rectElement.Width / 2 - rectElement.Height / 2,
                    rectElement.Y,
                    rectElement.Height,
                    rectElement.Height);

            #region Draw Body Gauge
            using (var brush = StiBrush.GetBrush(this.Brush, rectElement))
                g.FillEllipse(brush, rectGauge);

            g.DrawEllipse(Pens.LightGray, rectGauge);

            var rectCenter = new Rectangle(
                    rectElement.X + rectElement.Width / 2 - 2,
                    rectElement.Y + rectElement.Height / 2 - 2,
                    StiScaleUI.I4, StiScaleUI.I4);
            #endregion

            #region Draw Needle
            using (var brush = StiBrush.GetBrush(this.NeedleCapBrush, rectCenter))
            using (var pen = new Pen(brush))
                g.DrawEllipse(pen, rectCenter);

            g.DrawLine(new Pen(StiBrush.ToColor(this.NeedleBrush)),
                rectGauge.X + rectGauge.Width / 2 + 2,
                rectGauge.Y + rectGauge.Height / 2,
                rectGauge.X + rectGauge.Width + 2,
                rectGauge.Y + rectGauge.Height / 2);
            #endregion

            DrawStyleName(g, rect);
        }

        /// <summary>
        /// Gets a style from the component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements)
        {
            if (styleElements != StiStyleElements.All)
                throw new Exception("StiGaugeStyle support only StiStyleElements.All.");

            var gauge = component as Gauge.StiGauge;
            if (gauge != null)
            {
                #region IStiBrush
                if ((styleElements & StiStyleElements.Brush) > 0)
                {
                    var cmp = component as IStiBrush;
                    this.Brush = cmp.Brush.Clone() as StiBrush;
                }
                #endregion
            }
        }

        /// <summary>
        /// Sets style to a component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void SetStyleToComponent(StiComponent component)
        {
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiGaugeStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        internal StiGaugeStyle(string name, string description, StiReport report)
            : base(name, description, report)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiGaugeStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiGaugeStyle(string name, string description)
            : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiGaugeStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiGaugeStyle(string name)
            : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiGaugeStyle.
        /// </summary>
        public StiGaugeStyle()
            : this("")
        {
        }
    }
}
