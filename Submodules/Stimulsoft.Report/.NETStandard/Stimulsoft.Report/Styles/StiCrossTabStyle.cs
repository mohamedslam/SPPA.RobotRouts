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
using System.Drawing.Design;
using System.IO;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Brushes = Stimulsoft.Drawing.Brushes;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report
{
	/// <summary>
	/// Describes the class that contains a style for CrossTab components.
	/// </summary>	
	public class StiCrossTabStyle : StiBaseStyle
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyColor("BackColor", BackColor, Color.Transparent);
            jObject.AddPropertyColor("CellBackColor", CellBackColor, Color.White);
            jObject.AddPropertyColor("AlternatingCellBackColor", AlternatingCellBackColor, "#eee");
            jObject.AddPropertyColor("AlternatingCellForeColor", AlternatingCellForeColor, "#323a45");
            jObject.AddPropertyColor("SelectedCellBackColor", SelectedCellBackColor, StiColorUtils.Light(ColorTranslator.FromHtml("#3498db"), 30));
            jObject.AddPropertyColor("SelectedCellForeColor", SelectedCellForeColor, Color.White);
            jObject.AddPropertyColor("ColumnHeaderBackColor", ColumnHeaderBackColor, "#3498db");
            jObject.AddPropertyColor("ColumnHeaderForeColor", ColumnHeaderForeColor, "#fff");
            jObject.AddPropertyColor("RowHeaderBackColor", RowHeaderBackColor, "#3498db");
            jObject.AddPropertyColor("RowHeaderForeColor", RowHeaderForeColor, "#eee");
            jObject.AddPropertyColor("HotColumnHeaderBackColor", HotColumnHeaderBackColor, StiColorUtils.Dark(ColorTranslator.FromHtml("#3498db"), 30));
            jObject.AddPropertyColor("HotRowHeaderBackColor", HotRowHeaderBackColor, StiColorUtils.Dark(ColorTranslator.FromHtml("#3498db"), 30));
            jObject.AddPropertyColor("CellForeColor", CellForeColor, "#323a45");
            jObject.AddPropertyColor("LineColor", LineColor, Color.White);
            jObject.AddPropertyColor("TotalCellColumnBackColor", TotalCellColumnBackColor, "#3498db");
            jObject.AddPropertyColor("TotalCellColumnForeColor", TotalCellColumnForeColor, "#fff");
            jObject.AddPropertyColor("TotalCellRowBackColor", TotalCellRowBackColor, "#3498db");
            jObject.AddPropertyColor("TotalCellRowForeColor", TotalCellRowForeColor, "#eee");


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

                    case "CellBackColor":
                        this.CellBackColor = property.DeserializeColor();
                        break;

                    case "AlternatingCellForeColor":
                        this.AlternatingCellForeColor = property.DeserializeColor();
                        break;

                    case "AlternatingCellBackColor":
                        this.AlternatingCellBackColor = property.DeserializeColor();
                        break;

                    case "SelectedCellBackColor":
                        this.SelectedCellBackColor = property.DeserializeColor();
                        break;

                    case "SelectedCellForeColor":
                        this.SelectedCellForeColor = property.DeserializeColor();
                        break;

                    case "ColumnHeaderBackColor":
                        this.ColumnHeaderBackColor = property.DeserializeColor();
                        break;

                    case "ColumnHeaderForeColor":
                        this.ColumnHeaderForeColor = property.DeserializeColor();
                        break;

                    case "RowHeaderBackColor":
                        this.RowHeaderBackColor = property.DeserializeColor();
                        break;

                    case "RowHeaderForeColor":
                        this.RowHeaderForeColor = property.DeserializeColor();
                        break;

                    case "HotColumnHeaderBackColor":
                        this.HotColumnHeaderBackColor = property.DeserializeColor();
                        break;

                    case "HotRowHeaderBackColor":
                        this.HotRowHeaderBackColor = property.DeserializeColor();
                        break;

                    case "CellForeColor":
                        this.CellForeColor = property.DeserializeColor();
                        break;

                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;

                    case "TotalCellColumnBackColor":
                        this.TotalCellColumnBackColor = property.DeserializeColor();
                        break;

                    case "TotalCellColumnForeColor":
                        this.TotalCellColumnForeColor = property.DeserializeColor();
                        break;

                    case "TotalCellRowBackColor":
                        this.TotalCellRowBackColor = property.DeserializeColor();
                        break;

                    case "TotalCellRowForeColor":
                        this.TotalCellRowForeColor = property.DeserializeColor();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiCrossTabStyle;

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
                propHelper.Color(),
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

        [StiNonSerialized]
        [Browsable(false)]
	    public Color Color
	    {
	        get
	        {
	            return ColumnHeaderBackColor;
	        }
	        set
	        {
                ColumnHeaderBackColor = value;
	            RowHeaderBackColor = value;
            }
	    }

	    [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color CellBackColor { get; set; } = Color.White;

        private bool ShouldSerializeCellBackColor()
        {
            return CellBackColor != Color.White;
        }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color AlternatingCellBackColor { get; set; } = ColorTranslator.FromHtml("#eee");

	    private bool ShouldSerializeAlternatingCellBackColor()
	    {
	        return AlternatingCellBackColor != ColorTranslator.FromHtml("#eee");
        }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color AlternatingCellForeColor { get; set; } = StiColor.Get("#323a45");

        private bool ShouldSerializeAlternatingCellForeColor()
        {
            return AlternatingCellForeColor != StiColor.Get("#323a45");
        }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color SelectedCellBackColor { get; set; } = StiColorUtils.Light(ColorTranslator.FromHtml("#3498db"), 30);

	    private bool ShouldSerializeSelectedCellBackColor()
	    {
	        return SelectedCellBackColor != StiColorUtils.Light(ColorTranslator.FromHtml("#3498db"), 30);
        }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color SelectedCellForeColor { get; set; } = Color.White;

	    private bool ShouldSerializeSelectedCellForeColor()
	    {
	        return SelectedCellForeColor != Color.White;
	    }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color ColumnHeaderBackColor { get; set; } = ColorTranslator.FromHtml("#3498db");

	    private bool ShouldSerializeColumnHeaderBackColor()
	    {
	        return ColumnHeaderBackColor != ColorTranslator.FromHtml("#3498db");
        }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color ColumnHeaderForeColor { get; set; } = ColorTranslator.FromHtml("#fff");

	    private bool ShouldSerializeColumnHeaderForeColor()
	    {
	        return ColumnHeaderForeColor != ColorTranslator.FromHtml("#fff");
        }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color RowHeaderBackColor { get; set; } = ColorTranslator.FromHtml("#3498db");

	    private bool ShouldSerializeRowHeaderBackColor()
	    {
	        return RowHeaderBackColor != ColorTranslator.FromHtml("#3498db");
        }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color RowHeaderForeColor { get; set; } = ColorTranslator.FromHtml("#eee");

	    private bool ShouldSerializeRowHeaderForeColor()
	    {
	        return RowHeaderForeColor != ColorTranslator.FromHtml("#eee");
	    }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotColumnHeaderBackColor { get; set; } = StiColorUtils.Dark(ColorTranslator.FromHtml("#3498db"), 30);

	    private bool ShouldSerializeHotColumnHeaderBackColor()
	    {
	        return HotColumnHeaderBackColor != StiColorUtils.Dark(ColorTranslator.FromHtml("#3498db"), 30);
        }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color HotRowHeaderBackColor { get; set; } = StiColorUtils.Dark(ColorTranslator.FromHtml("#3498db"), 30);

	    private bool ShouldSerializeHotRowHeaderBackColor()
	    {
	        return HotRowHeaderBackColor != StiColorUtils.Dark(ColorTranslator.FromHtml("#3498db"), 30);
        }

        [StiCategory("Appearance")]
	    [StiSerializable]
	    [TypeConverter(typeof(StiColorConverter))]
	    [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color CellForeColor { get; set; } = ColorTranslator.FromHtml("#323a45");

	    private bool ShouldSerializeCellForeColor()
	    {
	        return CellForeColor != ColorTranslator.FromHtml("#323a45"); 
	    }

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color LineColor { get; set; } = Color.White;

        private bool ShouldSerializeLineColor()
        {
            return LineColor != Color.White;
        }

        private Color totalCellColumnBackColor = Color.Empty;

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color TotalCellColumnBackColor 
        {
            get 
            {
               if (totalCellColumnBackColor == Color.Empty)
               {
                   totalCellColumnBackColor = ColumnHeaderBackColor;
               }
               return totalCellColumnBackColor;
            }
            set 
            {
                totalCellColumnBackColor = value;
            }
        }

        private bool ShouldSerializeTotalCellColumnBackColor()
        {
            return TotalCellColumnBackColor != Color.Empty;
        }

        private Color totalCellColumnForeColor = Color.Empty;

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color TotalCellColumnForeColor
        {
            get
            {
                if (totalCellColumnForeColor == Color.Empty)
                {
                    totalCellColumnForeColor = ColumnHeaderForeColor;
                }
                return totalCellColumnForeColor;
            }
            set
            {
                totalCellColumnForeColor = value;
            }
        }

        private bool ShouldSerializeTotalCellColumnForeColor()
        {
            return TotalCellColumnForeColor != Color.Empty;
        }

        private Color totalCellRowBackColor = Color.Empty;

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color TotalCellRowBackColor
        {
            get
            {
                if (totalCellRowBackColor == Color.Empty)
                {
                    totalCellRowBackColor = RowHeaderBackColor;
                }
                return totalCellRowBackColor;
            }
            set
            {
                totalCellRowBackColor = value;
            }
        }

        private bool ShouldSerializeTotalCellRowBackColor()
        {
            return TotalCellRowBackColor != Color.Empty;
        }

        private Color totalCellRowForeColor = Color.Empty;

        [StiCategory("Appearance")]
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public virtual Color TotalCellRowForeColor
        {
            get
            {
                if (totalCellRowForeColor == Color.Empty)
                {
                    totalCellRowForeColor = RowHeaderForeColor;
                }
                return totalCellRowForeColor;
            }
            set
            {
                totalCellRowForeColor = value;
            }
        }

        private bool ShouldSerializeTotalCellRowForeColor()
        {
            return TotalCellRowForeColor != Color.Empty;
        }
        #endregion

        #region Methods.Style
        public void DrawStyleForGallery(Graphics g, Rectangle rect)
        {
            var rectElement = new Rectangle(rect.X + StiScale.XXI(5), rect.Y + StiScale.YYI(5), rect.Width - StiScale.XXI(10), rect.Height - StiScale.YYI(10));

            int cellSize = rectElement.Height / 5;
            if (cellSize * 5 > rectElement.Height)
                cellSize -= StiScale.XXI(1);

            rectElement.Height = cellSize * 5;

            int cols = rectElement.Width / cellSize;
            rectElement.Width = cellSize * cols;

            g.FillRectangle(Brushes.White, new Rectangle(rectElement.X + cellSize, rectElement.Y + cellSize, rectElement.Width - cellSize, rectElement.Height - cellSize));
            using (var brush = new SolidBrush((this.Color == Color.White) ? Color.FromArgb(255, 236, 236, 236) : this.Color))
            {
                g.FillRectangle(brush, new Rectangle(rectElement.X + cellSize, rectElement.Y, rectElement.Width - cellSize, cellSize));
                g.FillRectangle(brush, new Rectangle(rectElement.X, rectElement.Y + cellSize, cellSize, rectElement.Height - cellSize));
            }

            for (int index = 0; index < 6; index++)
            {
                if (index == 0)
                    g.DrawLine(Pens.LightGray, rectElement.X + cellSize, rectElement.Y + cellSize * index, rectElement.Right, rectElement.Y + cellSize * index);
                else
                    g.DrawLine(Pens.LightGray, rectElement.X, rectElement.Y + cellSize * index, rectElement.Right, rectElement.Y + cellSize * index);
            }

            for (int index = 0; index <= cols; index++)
            {
                if (index == 0)
                    g.DrawLine(Pens.LightGray, rectElement.X + cellSize * index, rectElement.Y + cellSize, rectElement.X + cellSize * index, rectElement.Bottom);
                else
                    g.DrawLine(Pens.LightGray, rectElement.X + cellSize * index, rectElement.Y, rectElement.X + cellSize * index, rectElement.Bottom);
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
            if (styleElements != StiStyleElements.All)throw new Exception("StiCrossTabStyle support only StiStyleElements.All.");

            var crossTab = component as Stimulsoft.Report.CrossTab.StiCrossTab;
            if (crossTab == null) return;

            string name = crossTab.CrossTabStyle;
            if (!string.IsNullOrEmpty(name) && crossTab.Report != null && crossTab.Report.Styles[name] is StiCrossTabStyle)
            {
                this.Color = ((StiCrossTabStyle)crossTab.Report.Styles[name]).Color;
            }   
            else if (crossTab.CrossTabStyleIndex < StiOptions.Designer.CrossTab.StyleColors.Length)
            {
                if (crossTab.CrossTabStyleIndex >= 0 && crossTab.CrossTabStyleIndex < StiOptions.Designer.CrossTab.StyleColors.Length - 1)
                    this.Color = StiOptions.Designer.CrossTab.StyleColors[crossTab.CrossTabStyleIndex];
            }
        }

        /// <summary>
        /// Sets style to a component.
        /// </summary>
        /// <param name="component">Component.</param>
        public override void SetStyleToComponent(StiComponent component)
        {
            var crossTab = component as Stimulsoft.Report.CrossTab.StiCrossTab;
            if (crossTab != null)
            {
                if (!StiStyleConditionHelper.IsAllowStyle(component, this))
                    return;

                #region Color
                crossTab.CrossTabStyleColor = this.Color;
                crossTab.UpdateStyles();
                #endregion
            }
        }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiCrossTabStyle.
		/// </summary>
		/// <param name="name">Style name.</param>
		/// <param name="description">Style description.</param>
		internal StiCrossTabStyle(string name, string description, StiReport report) : base(name, description, report)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiCrossTabStyle.
		/// </summary>
		/// <param name="name">Style name.</param>
		/// <param name="description">Style description.</param>
		public StiCrossTabStyle(string name, string description) : this(name, description, null)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiCrossTabStyle.
		/// </summary>
		/// <param name="name">Style name.</param>
		public StiCrossTabStyle(string name) : this(name, "")
		{
		}

		/// <summary>
        /// Creates a new object of the type StiCrossTabStyle.
		/// </summary>
        public StiCrossTabStyle()
            : this("")
		{
		}
	}
}
