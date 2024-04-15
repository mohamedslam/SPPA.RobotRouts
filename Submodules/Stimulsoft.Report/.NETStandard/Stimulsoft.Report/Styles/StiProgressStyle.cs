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
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
using Pen = Stimulsoft.Drawing.Pen;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a style for Progress components.
    /// </summary>	
    public class StiProgressStyle : StiBaseStyle
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyColor(nameof(TrackColor), TrackColor, "#e7ebec");
            jObject.AddPropertyColor(nameof(BandColor), BandColor, "#3498db");
            jObject.AddPropertyArrayColor(nameof(SeriesColors), SeriesColors);
            jObject.AddPropertyColor(nameof(ForeColor), ForeColor, "#8c8c8c");
            jObject.AddPropertyColor(nameof(BackColor), BackColor, Color.White);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(TrackColor):
                        this.TrackColor = property.DeserializeColor();
                        break;

                    case nameof(BandColor):
                        this.BandColor = property.DeserializeColor();
                        break;

                    case nameof(SeriesColors):
                        this.SeriesColors = property.DeserializeArrayColor();
                        break;

                    case nameof(ForeColor):
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case nameof(BackColor):
                        this.BackColor = property.DeserializeColor();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiProgressStyle;

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
                propHelper.GlyphColor(),
                propHelper.ForeColor(),
                propHelper.HotBackColor(),
                propHelper.HotForeColor(),
            };
            objHelper.Add(StiPropertyCategories.Appearance, list);

            return objHelper;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var style = base.Clone() as StiProgressStyle;
            style.SeriesColors = this.SeriesColors?.ToArray();

            return style;
        }
        #endregion

        #region Consts
        private Color[] DefaultSeriesColors =
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
        public Color TrackColor { get; set; } = ColorTranslator.FromHtml("#e7ebec");

        private bool ShouldSerializeTrackColor()
        {
            return TrackColor != ColorTranslator.FromHtml("#e7ebec");
        }

        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color BandColor { get; set; } = ColorTranslator.FromHtml("#3498db");

        private bool ShouldSerializeBandColor()
        {
            return BandColor != ColorTranslator.FromHtml("#3498db");
        }

        [StiSerializable]
        [TypeConverter(typeof(StiStyleColorsConverter))]
        [StiCategory("Appearance")]
        [Editor("Stimulsoft.Report.Components.Design.StiColorsCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color[] SeriesColors { get; set; }

        private bool ShouldSerializeSeriesColors()
        {
            return SeriesColors == null ||
                   SeriesColors.Length != DefaultSeriesColors.Length ||
                   !SeriesColors.ToList().SequenceEqual(DefaultSeriesColors);
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
        public Color BackColor { get; set; } = Color.White;

        private bool ShouldSerializeBackColor()
        {
            return BackColor != Color.White;
        }

        #endregion

        #region Methods.Style
        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            if (paintImage)
                DrawStyleImage(g, rect, StiReportImages.Styles.Progress(StiImageSize.Double));

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
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiProgressStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        internal StiProgressStyle(string name, string description, StiReport report)
            : base(name, description, report)
        {
            this.SeriesColors = DefaultSeriesColors.Clone() as Color[];
        }

        /// <summary>
        /// Creates a new object of the type StiProgressStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiProgressStyle(string name, string description)
            : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiProgressStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiProgressStyle(string name)
            : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiProgressStyle.
        /// </summary>
        public StiProgressStyle()
            : this("")
        {
        }
    }
}
