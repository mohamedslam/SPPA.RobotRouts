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
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Images;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Pen = Stimulsoft.Drawing.Pen;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{/// <summary>
 /// Describes the class that contains a style for Cards components.
 /// </summary>	
    public class StiCardsStyle : StiBaseStyle
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyColor(nameof(BackColor), BackColor, Color.White);
            jObject.AddPropertyColor(nameof(ForeColor), ForeColor, StiColor.Get("222"));
            jObject.AddPropertyColor(nameof(LineColor), LineColor, Color.Gainsboro);
            jObject.AddPropertyArrayColor(nameof(SeriesColors), SeriesColors);
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

                    case nameof(ForeColor):
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case nameof(LineColor):
                        this.LineColor = property.DeserializeColor();
                        break;

                    case nameof(SeriesColors):
                        this.SeriesColors = property.DeserializeArrayColor();
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
            var style = base.Clone() as StiCardsStyle;

            style.SeriesColors = this.SeriesColors?.ToArray();
            style.ToolTipBrush = (StiBrush)this.ToolTipBrush.Clone();
            style.ToolTipTextBrush = (StiBrush)this.ToolTipTextBrush.Clone();
            style.ToolTipCornerRadius = (StiCornerRadius)this.ToolTipCornerRadius.Clone();
            style.ToolTipBorder = (StiSimpleBorder)this.ToolTipBorder.Clone();

            return style;
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCardsStyle;

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
                propHelper.LineColor()
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            return objHelper;
        }
        #endregion

        #region Consts
        private static Color[] DefaultSeriesColors =
        {
            ColorTranslator.FromHtml("#3498db"),
            ColorTranslator.FromHtml("#ef717a"),
            ColorTranslator.FromHtml("#6dcbb3"),
            ColorTranslator.FromHtml("#f28161"),
            ColorTranslator.FromHtml("#fccd1b"),
        };
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
        public Color ForeColor { get; set; } = StiColor.Get("222");

        private bool ShouldSerializeForeColor()
        {
            return ForeColor != StiColor.Get("222");
        }

        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color LineColor { get; set; } = Color.Gainsboro;

        private bool ShouldSerializeLineColor()
        {
            return LineColor != Color.Gainsboro;
        }

        /// <summary>
        /// Gets or sets a list of colors which will be used for drawing chart series.
        /// </summary>
        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("Gets or sets a list of colors which will be used for drawing chart series.")]
        [TypeConverter(typeof(StiStyleColorsConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiColorsCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color[] SeriesColors { get; set; } = DefaultSeriesColors;

        private bool ShouldSerializeStyleColors()
        {
            return SeriesColors == null ||
                   SeriesColors.Length != DefaultSeriesColors.Length ||
                   !SeriesColors.ToList().SequenceEqual(DefaultSeriesColors);
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
            if (paintImage)
                DrawStyleImage(g, rect, StiReportImages.Styles.Cards(StiImageSize.Double));

            DrawStyleName(g, rect);
        }

        public override void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements)
        {
            if (styleElements != StiStyleElements.All)
                throw new Exception("StiCardsStyle support only StiStyleElements.All.");
        }

        public override void SetStyleToComponent(StiComponent component)
        {
        }
        #endregion
    }
}
