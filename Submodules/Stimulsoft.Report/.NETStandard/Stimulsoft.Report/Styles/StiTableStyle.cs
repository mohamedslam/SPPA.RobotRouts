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
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using System.Drawing.Design;
using System.IO;
using Stimulsoft.Base.Drawing.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Base.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pen = Stimulsoft.Drawing.Pen;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a style for Map components.
    /// </summary>	
    public class StiTableStyle : StiBaseStyle
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyColor("BackColor", BackColor, Color.Transparent);
            jObject.AddPropertyColor("DataColor", DataColor, Color.White);
            jObject.AddPropertyColor("DataForeground", DataForeground, "#222");
            jObject.AddPropertyColor("SelectedDataColor", SelectedDataColor, "#3498db");
            jObject.AddPropertyColor("SelectedDataForeground", SelectedDataForeground, Color.White);
            jObject.AddPropertyColor("AlternatingDataColor", AlternatingDataColor, "#eee");
            jObject.AddPropertyColor("AlternatingDataForeground", AlternatingDataForeground, "#222");
            jObject.AddPropertyColor("HeaderColor", HeaderColor, "#3498db");
            jObject.AddPropertyColor("HeaderForeground", HeaderForeground, Color.White);
            jObject.AddPropertyColor("HotHeaderColor", HotHeaderColor, Color.Transparent);
            jObject.AddPropertyColor("FooterColor", FooterColor, Color.White);
            jObject.AddPropertyColor("FooterForeground", FooterForeground, Color.Black);
            jObject.AddPropertyColor("GridColor", GridColor, Color.Black);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "BackColor":
                        this.BackColor = property.DeserializeColor();
                        break;

                    case "DataColor":
                        this.DataColor = property.DeserializeColor();
                        break;

                    case "DataForeground":
                        this.DataForeground = property.DeserializeColor();
                        break;

                    case "SelectedDataColor":
                        this.SelectedDataColor = property.DeserializeColor();
                        break;

                    case "SelectedDataForeground":
                        this.SelectedDataForeground = property.DeserializeColor();
                        break;

                    case "AlternatingDataColor":
                        this.AlternatingDataColor = property.DeserializeColor();
                        break;

                    case "AlternatingDataForeground":
                        this.AlternatingDataForeground = property.DeserializeColor();
                        break;

                    case "HeaderColor":
                        this.HeaderColor = property.DeserializeColor();
                        break;

                    case "HeaderForeground":
                        this.HeaderForeground = property.DeserializeColor();
                        break;

                    case "HotHeaderColor":
                        this.HotHeaderColor = property.DeserializeColor();
                        break;

                    case "FooterColor":
                        this.FooterColor = property.DeserializeColor();
                        break;

                    case "FooterForeground":
                        this.FooterForeground = property.DeserializeColor();
                        break;

                    case "GridColor":
                        this.GridColor = property.DeserializeColor();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiTableStyle;

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
                propHelper.HeaderColor(),
                propHelper.HeaderForeground(),
                propHelper.FooterForeground(),
                propHelper.DataColor(),
                propHelper.DataForeground(),
                propHelper.GridColor()
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

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color DataColor { get; set; } = Color.White;

        private bool ShouldSerializeDataColor()
        {
            return DataColor != Color.White;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color DataForeground { get; set; } = ColorTranslator.FromHtml("#222");

        private bool ShouldSerializeDataForeground()
        {
            return DataForeground != ColorTranslator.FromHtml("#222");
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color SelectedDataColor { get; set; } = ColorTranslator.FromHtml("#3498db");

        private bool ShouldSerializeSelectedDataColor()
        {
            return SelectedDataColor != ColorTranslator.FromHtml("#3498db");
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color SelectedDataForeground { get; set; } = Color.White;

        private bool ShouldSerializeSelectedDataForeground()
        {
            return SelectedDataForeground != Color.White;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color AlternatingDataColor { get; set; } = ColorTranslator.FromHtml("#eee");

        private bool ShouldSerializeAlternatingDataColor()
        {
            return AlternatingDataColor != ColorTranslator.FromHtml("#eee");
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color AlternatingDataForeground { get; set; } = ColorTranslator.FromHtml("#222");

        private bool ShouldSerializeAlternatingDataForeground()
        {
            return AlternatingDataForeground != ColorTranslator.FromHtml("#222");
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HeaderColor { get; set; } = ColorTranslator.FromHtml("#3498db");

        private bool ShouldSerializeHeaderColor()
        {
            return HeaderColor != ColorTranslator.FromHtml("#3498db");
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HeaderForeground { get; set; } = Color.White;

        private bool ShouldSerializeHeaderForeground()
        {
            return HeaderForeground != Color.White;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotHeaderColor { get; set; } = Color.Transparent;

        private bool ShouldSerializeHotHeaderColor()
        {
            return HotHeaderColor != Color.Transparent;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color FooterColor { get; set; } = Color.White;

        private bool ShouldSerializeFooterColor()
        {
            return FooterColor != Color.White;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color FooterForeground { get; set; } = Color.Black;

        private bool ShouldSerializeFooterForeground()
        {
            return FooterForeground != Color.Black;
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color GridColor { get; set; } = Color.Gainsboro;

        private bool ShouldSerializeGridColor()
        {
            return GridColor != Color.Gainsboro;
        }
        #endregion

        #region Methods.Style
        private Color GetColor(Color color)
        {
            return color == Color.White
                ? Color.FromArgb(180, 255, 255, 255)
                : color;
        }

        public void DrawStyleForGallery(Graphics g, Rectangle rect)
        {
            var rectElement = new Rectangle(rect.X + StiScale.XXI(5), rect.Y + StiScale.YYI(5), rect.Width - StiScale.XXI(10), rect.Height - StiScale.YYI(10));

            int cellSize = rectElement.Height / 5;
            if (cellSize * 5 > rectElement.Height)
                cellSize--;

            rectElement.Height = cellSize * 5;

            int cols = rectElement.Width / cellSize;
            rectElement.Width = cellSize * cols;

            using (var brush = new SolidBrush(GetColor(this.DataColor)))
            {
                g.FillRectangle(brush, rectElement);
            }

            using (var pen = new Pen(GetColor(GridColor), (float)(1f * StiScale.Factor)))
            {
                for (int index = 0; index < 6; index++)
                {
                    g.DrawLine(pen, rectElement.X, rectElement.Y + cellSize * index, rectElement.Right, rectElement.Y + cellSize * index);
                }
                for (int index = 0; index <= cols; index++)
                {
                    g.DrawLine(pen, rectElement.X + cellSize * index, rectElement.Y, rectElement.X + cellSize * index, rectElement.Bottom);
                }
            }

            using (var brush = new SolidBrush(GetColor(this.HeaderColor)))
            {
                g.FillRectangle(brush, new Rectangle(rectElement.X, rectElement.Y, rectElement.Width, cellSize));
            }

            using (var brush = new SolidBrush(GetColor(this.FooterColor)))
            {
                g.FillRectangle(brush, new Rectangle(rectElement.X, rectElement.Bottom - cellSize, rectElement.Width, cellSize));
            }
        }

        public override void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            if (paintImage)
            {
                var imageRect = GetStyleImageRect(rect);
                DrawStyleForGallery(g, imageRect);
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
                throw new Exception("StiTableStyle support only StiStyleElements.All.");

            var table = component as StiTable;
            if (table != null)
            {
                StiTableStyle actualTableStyle = table.TableStyleFX;
                if (actualTableStyle == null && !string.IsNullOrEmpty(table.ComponentStyle))
                    actualTableStyle = table.Report.Styles[table.ComponentStyle] as StiTableStyle;

                if (actualTableStyle != null)
                {
                    this.BackColor = actualTableStyle.BackColor;
                    this.HeaderColor = actualTableStyle.HeaderColor;
                    this.AlternatingDataColor = actualTableStyle.AlternatingDataColor;
                    this.DataColor = actualTableStyle.DataColor;
                    this.AlternatingDataForeground = actualTableStyle.AlternatingDataForeground;
                    this.DataForeground = actualTableStyle.DataForeground;
                    this.FooterColor = actualTableStyle.FooterColor;
                    this.FooterForeground = actualTableStyle.FooterForeground;
                    this.GridColor = actualTableStyle.GridColor;
                    this.HeaderColor = actualTableStyle.HeaderColor;
                    this.HeaderForeground = actualTableStyle.HeaderForeground;
                    this.HotHeaderColor = actualTableStyle.HotHeaderColor;
                    this.SelectedDataColor = actualTableStyle.SelectedDataColor;
                    this.SelectedDataForeground = actualTableStyle.SelectedDataForeground;
                }
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
        /// Creates a new object of the type StiTableStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        internal StiTableStyle(string name, string description, StiReport report)
            : base(name, description, report)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiTableStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiTableStyle(string name, string description)
            : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiTableStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiTableStyle(string name)
            : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiTableStyle.
        /// </summary>
        public StiTableStyle()
            : this("")
        {

        }
    }
}
